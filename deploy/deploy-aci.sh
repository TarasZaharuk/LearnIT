#!/usr/bin/env bash
#
# Manual / local equivalent of the Deploy stage in azure-pipelines.yml.
# Useful for the first provisioning run and for debugging the pipeline.
#
# Usage:
#   az login
#   cp deploy/aci.env.example deploy/aci.env   # then fill it in
#   ./deploy/deploy-aci.sh [image-tag]
#
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ENV_FILE="${ENV_FILE:-$SCRIPT_DIR/aci.env}"

if [[ ! -f "$ENV_FILE" ]]; then
  echo "Missing $ENV_FILE (copy deploy/aci.env.example and fill it in)." >&2
  exit 1
fi

# shellcheck disable=SC1090
set -a; source "$ENV_FILE"; set +a

IMAGE_TAG="${1:-${IMAGE_TAG:-latest}}"
: "${ACR_NAME:?}" "${RESOURCE_GROUP:?}" "${LOCATION:?}" "${ACI_NAME:?}" "${DNS_NAME_LABEL:?}"
: "${SQL_CONNECTION_STRING:?}" "${JWT_KEY:?}"

if [[ -n "${SUBSCRIPTION_ID:-}" ]]; then
  echo "==> Targeting subscription $SUBSCRIPTION_ID"
  az account set --subscription "$SUBSCRIPTION_ID"
fi

ACR_LOGIN_SERVER="${ACR_LOGIN_SERVER:-${ACR_NAME}.azurecr.io}"
IMAGE_NAME="${IMAGE_NAME:-learnit}"
CONTAINER_PORT="${CONTAINER_PORT:-8080}"
FQDN="${DNS_NAME_LABEL}.${LOCATION}.azurecontainer.io"
PUBLIC_BASE_URL="http://${FQDN}:${CONTAINER_PORT}"

# Fail fast: the image pull needs the ACR admin user, and discovering that only after
# a three-minute build is a waste of time.
if [[ "$(az acr show -n "$ACR_NAME" --query adminUserEnabled -o tsv)" != "true" ]]; then
  echo "ACR '$ACR_NAME' has no admin user, which the container group needs to pull the image." >&2
  echo "Enable it with:  az acr update -n $ACR_NAME --admin-enabled true" >&2
  exit 1
fi

echo "==> Building and pushing ${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG} via ACR Tasks"
az acr build \
  --registry "$ACR_NAME" \
  --image "${IMAGE_NAME}:${IMAGE_TAG}" \
  --image "${IMAGE_NAME}:latest" \
  --file "$SCRIPT_DIR/../LearnIT/Dockerfile" \
  "$SCRIPT_DIR/../LearnIT"

echo "==> Reading ACR credentials"
ACR_USER=$(az acr credential show -n "$ACR_NAME" --query username -o tsv)
ACR_PASS=$(az acr credential show -n "$ACR_NAME" --query 'passwords[0].value' -o tsv)

echo "==> Replacing container group ${ACI_NAME}"
az container delete --resource-group "$RESOURCE_GROUP" --name "$ACI_NAME" --yes 2>/dev/null || true

az container create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACI_NAME" \
  --image "${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${IMAGE_TAG}" \
  --os-type Linux \
  --location "$LOCATION" \
  --cpu "${ACI_CPU:-1}" \
  --memory "${ACI_MEMORY_GB:-1.5}" \
  --ports "$CONTAINER_PORT" \
  --protocol TCP \
  --ip-address Public \
  --dns-name-label "$DNS_NAME_LABEL" \
  --restart-policy Always \
  --registry-login-server "$ACR_LOGIN_SERVER" \
  --registry-username "$ACR_USER" \
  --registry-password "$ACR_PASS" \
  --environment-variables \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS="$CONTAINER_PORT" \
    BaseServerAddress="$PUBLIC_BASE_URL" \
    EmailConfirmationControllerAddress="${PUBLIC_BASE_URL}/email" \
    HomeFrontedPageAddress="${PUBLIC_BASE_URL}/" \
    Jwt__Issuer="${JWT_ISSUER:-LearnITServer}" \
    Jwt__Audience="${JWT_AUDIENCE:-LearnITServer}" \
    SmtpClientSettings__Host="${SMTP_HOST:-smtp.gmail.com}" \
    SmtpClientSettings__Port="${SMTP_PORT:-587}" \
  --secure-environment-variables \
    LearnITDbConnectionString="$SQL_CONNECTION_STRING" \
    Jwt__Key="$JWT_KEY" \
    SmtpClientSettings__UserName="${SMTP_USERNAME:-}" \
    SmtpClientSettings__Password="${SMTP_PASSWORD:-}" \
    SenderEmail="${SENDER_EMAIL:-}"

echo "==> Waiting for ${PUBLIC_BASE_URL}/health"
for i in {1..30}; do
  if curl -fsS --max-time 10 "${PUBLIC_BASE_URL}/health" >/dev/null; then
    echo "Healthy. App is live at ${PUBLIC_BASE_URL}"
    exit 0
  fi
  echo "Attempt $i - not ready yet, retrying in 10s..."
  sleep 10
done

echo "Container did not become healthy. Recent logs:" >&2
az container logs --resource-group "$RESOURCE_GROUP" --name "$ACI_NAME" --tail 200 || true
exit 1
