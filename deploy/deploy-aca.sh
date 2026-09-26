#!/usr/bin/env bash
#
# Deploy LearnIT to Azure Container Apps (recommended target).
# Idempotent: first run creates the app, later runs push a new revision.
#
# Usage:
#   az login
#   cp deploy/aca.env.example deploy/aca.env   # then fill it in
#   ./deploy/deploy-aca.sh [image-tag]
#
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="${ENV_FILE:-$SCRIPT_DIR/aca.env}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE (copy deploy/aca.env.example and fill it in)." >&2
  exit 1
fi

# shellcheck disable=SC1090
set -a; source "$ENV_FILE"; set +a

IMAGE_TAG="${1:-${IMAGE_TAG:-latest}}"
: "${ACR_NAME:?}" "${RESOURCE_GROUP:?}" "${LOCATION:?}" "${ACA_ENV:?}" "${ACA_APP:?}"
: "${SQL_CONNECTION_STRING:?}" "${JWT_KEY:?}"

if [[ -n "${SUBSCRIPTION_ID:-}" ]]; then
  echo "==> Targeting subscription $SUBSCRIPTION_ID"
  az account set --subscription "$SUBSCRIPTION_ID"
fi

ACR_LOGIN_SERVER="${ACR_LOGIN_SERVER:-${ACR_NAME}.azurecr.io}"
IMAGE_NAME="${IMAGE_NAME:-learnit}"
CONTAINER_PORT="${CONTAINER_PORT:-8080}"
IMAGE="${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}"

# Fail fast: the image pull needs the ACR admin user, and discovering that only after
# a three-minute build is a waste of time.
if [[ "$(az acr show -n "$ACR_NAME" --query adminUserEnabled -o tsv)" != "true" ]]; then
  echo "ACR '$ACR_NAME' has no admin user, which the container app needs to pull the image." >&2
  echo "Enable it with:  az acr update -n $ACR_NAME --admin-enabled true" >&2
  exit 1
fi

echo "==> Building and pushing ${IMAGE} via ACR Tasks"
az acr build \
  --registry "$ACR_NAME" \
  --image "${IMAGE_NAME}:${IMAGE_TAG}" \
  --image "${IMAGE_NAME}:latest" \
  --file "$SCRIPT_DIR/../LearnIT/Dockerfile" \
  "$SCRIPT_DIR/../LearnIT"

ACR_USER=$(az acr credential show -n "$ACR_NAME" --query username -o tsv)
ACR_PASS=$(az acr credential show -n "$ACR_NAME" --query 'passwords[0].value' -o tsv)

APP_EXISTS=$(az containerapp show -g "$RESOURCE_GROUP" -n "$ACA_APP" --query id -o tsv 2>/dev/null || true)

if [[ -z "$APP_EXISTS" ]]; then
  echo "==> Creating container app ${ACA_APP}"
  # The public FQDN is only known after creation, and BaseServerAddress must match it,
  # so create first with placeholder URLs and correct them below.
  az containerapp create \
    --name "$ACA_APP" \
    --resource-group "$RESOURCE_GROUP" \
    --environment "$ACA_ENV" \
    --image "$IMAGE" \
    --target-port "$CONTAINER_PORT" \
    --ingress external \
    --transport auto \
    --cpu "${ACA_CPU:-0.5}" \
    --memory "${ACA_MEMORY:-1.0Gi}" \
    --min-replicas "${ACA_MIN_REPLICAS:-1}" \
    --max-replicas "${ACA_MAX_REPLICAS:-1}" \
    --registry-server "$ACR_LOGIN_SERVER" \
    --registry-username "$ACR_USER" \
    --registry-password "$ACR_PASS" \
    --secrets \
      "sql-connection-string=$SQL_CONNECTION_STRING" \
      "jwt-key=$JWT_KEY" \
      "smtp-username=${SMTP_USERNAME:-}" \
      "smtp-password=${SMTP_PASSWORD:-}" \
      "sender-email=${SENDER_EMAIL:-}" \
    --env-vars \
      ASPNETCORE_ENVIRONMENT=Production \
      "ASPNETCORE_HTTP_PORTS=$CONTAINER_PORT" \
      "LearnITDbConnectionString=secretref:sql-connection-string" \
      "Jwt__Key=secretref:jwt-key" \
      "Jwt__Issuer=${JWT_ISSUER:-LearnITServer}" \
      "Jwt__Audience=${JWT_AUDIENCE:-LearnITServer}" \
      "SmtpClientSettings__Host=${SMTP_HOST:-smtp.gmail.com}" \
      "SmtpClientSettings__Port=${SMTP_PORT:-587}" \
      "SmtpClientSettings__UserName=secretref:smtp-username" \
      "SmtpClientSettings__Password=secretref:smtp-password" \
      "SenderEmail=secretref:sender-email" \
    --output none
fi

FQDN=$(az containerapp show -g "$RESOURCE_GROUP" -n "$ACA_APP" \
  --query properties.configuration.ingress.fqdn -o tsv)
PUBLIC_BASE_URL="${PUBLIC_BASE_URL:-https://${FQDN}}"

echo "==> Updating ${ACA_APP} -> ${IMAGE} (public URL ${PUBLIC_BASE_URL})"
# `update` rolls out a new revision with zero downtime; secrets are refreshed via `secret set`.
az containerapp secret set \
  --name "$ACA_APP" \
  --resource-group "$RESOURCE_GROUP" \
  --secrets \
    "sql-connection-string=$SQL_CONNECTION_STRING" \
    "jwt-key=$JWT_KEY" \
    "smtp-username=${SMTP_USERNAME:-}" \
    "smtp-password=${SMTP_PASSWORD:-}" \
    "sender-email=${SENDER_EMAIL:-}" \
  --output none

az containerapp update \
  --name "$ACA_APP" \
  --resource-group "$RESOURCE_GROUP" \
  --image "$IMAGE" \
  --set-env-vars \
    ASPNETCORE_ENVIRONMENT=Production \
    "ASPNETCORE_HTTP_PORTS=$CONTAINER_PORT" \
    "LearnITDbConnectionString=secretref:sql-connection-string" \
    "Jwt__Key=secretref:jwt-key" \
    "Jwt__Issuer=${JWT_ISSUER:-LearnITServer}" \
    "Jwt__Audience=${JWT_AUDIENCE:-LearnITServer}" \
    "BaseServerAddress=$PUBLIC_BASE_URL" \
    "EmailConfirmationControllerAddress=${PUBLIC_BASE_URL}/email" \
    "HomeFrontedPageAddress=${PUBLIC_BASE_URL}/" \
    "SmtpClientSettings__Host=${SMTP_HOST:-smtp.gmail.com}" \
    "SmtpClientSettings__Port=${SMTP_PORT:-587}" \
    "SmtpClientSettings__UserName=secretref:smtp-username" \
    "SmtpClientSettings__Password=secretref:smtp-password" \
    "SenderEmail=secretref:sender-email" \
  --output none

echo "==> Waiting for ${PUBLIC_BASE_URL}/health"
for i in {1..30}; do
  if curl -fsS --max-time 10 "${PUBLIC_BASE_URL}/health" >/dev/null; then
    echo "Healthy. App is live at ${PUBLIC_BASE_URL}"
    exit 0
  fi
  echo "Attempt $i - not ready yet, retrying in 10s..."
  sleep 10
done

echo "Revision did not become healthy. Recent logs:" >&2
az containerapp logs show -g "$RESOURCE_GROUP" -n "$ACA_APP" --tail 200 || true
exit 1
