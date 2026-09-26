# LearnIT — containerisation & Azure deployment

The solution ships as **one image**: `LearnIT.WebUI.Server` (ASP.NET Core API) which also
serves the Blazor WebAssembly client from its `wwwroot` (`UseBlazorFrameworkFiles` +
`MapFallbackToFile("index.html")`). The client resolves the API from
`HostEnvironment.BaseAddress`, so nothing host-specific is baked into the WASM payload —
one container, one origin, no CORS setup and no client rebuild per environment.

```
LearnIT/Dockerfile  ->  Azure Container Registry  ->  Container Apps (or ACI)  ->  Azure SQL Database
```

---

## 1. Which target: Container Apps or Container Instances?

**Use Azure Container Apps.** Identical image, no Dockerfile change, and it fixes three
things ACI cannot:

| | Container Instances | Container Apps |
| --- | --- | --- |
| HTTPS | **none** — HTTP only on `*.azurecontainer.io` | free managed TLS on the FQDN |
| Deployment | container group deleted and recreated → **outage every deploy** | new revision, zero downtime |
| Rollback | redeploy the old tag (another outage) | shift traffic to the previous revision |
| Idle cost | billed 24/7 while it exists | scale-to-zero possible |
| Logs/metrics | `az container logs` only | Log Analytics, metrics, console access |
| Setup | slightly simpler, no environment resource | needs a Container Apps environment (one command) |

The HTTPS difference is the decisive one here: the app issues JWTs and emails confirmation
links, and those must not travel over plain HTTP. Making ACI serve HTTPS means putting
Front Door or Application Gateway in front of it, which costs more than the compute does.

Running always-on, the two are roughly comparable in price; Container Apps becomes
much cheaper if you allow scale-to-zero. Check the Azure pricing calculator for
your region rather than trusting a figure here.

**ACI still makes sense if** you want the absolute simplest resource model, the app is
internal/short-lived, or a per-deploy outage is genuinely fine. Both paths are wired up —
`azure-pipelines.yml` takes a `deployTarget` parameter.

---

## 2. Files

| File | Purpose |
| --- | --- |
| `LearnIT/Dockerfile` | Multi-stage build (restore / publish / runtime), non-root, port 8080 |
| `LearnIT/.dockerignore` | Keeps `bin/`, `obj/`, `.git/`, `.env` out of the build context |
| `LearnIT/docker-compose.yml` | Local stack: app + SQL Server 2022, health-gated startup |
| `LearnIT/docker-compose.prod.yml` | The ACR image against Azure SQL — pre-flight check of the real config |
| `LearnIT/.env.example` | Template for `.env` / `.env.prod` |
| `azure-pipelines.yml` | Azure DevOps: build → push to ACR → deploy → wait for `/health` |
| `deploy/deploy-aca.sh` + `aca.env.example` | Provision/deploy Container Apps from a workstation |
| `deploy/deploy-aci.sh` + `aci.env.example` | Same for Container Instances |

---

## 3. Run it locally first

```bash
cd LearnIT
cp .env.example .env          # set MSSQL_SA_PASSWORD and JWT_KEY
docker compose up --build
```

- app → <http://localhost:5000>
- swagger → <http://localhost:5000/swagger>
- health → <http://localhost:5000/health>

Compose starts SQL Server 2022, waits for it to accept connections (`sqlcmd` healthcheck),
then starts the app, which applies EF migrations on boot. `ASPNETCORE_ENVIRONMENT` is
`Development` here, which also enables Swagger and the Bogus `DBSeeder`.

---

## 4. Deploy from scratch — the actual runbook

These are the real values this repo is configured for. Run the steps in order; everything
in Part A and B is one-off, after which deploys are `git push` (Part C) or one script call.

| Setting | Value |
| --- | --- |
| Subscription | `Azure subscription 1` — `87f800b8-0c86-4693-9837-41192429a956` |
| Resource group | `rg-learnit` |
| Region | `westeurope` ((Europe) West Europe) |
| Container registry | `acrlearnit` → `acrlearnit.azurecr.io` |
| SQL server / database | `learnit-sql` / `LearnIT` |
| Container Apps env / app | `learnit-env` / `learnit` |
| Image | `acrlearnit.azurecr.io/learnit:<tag>` |
| Container port | `8080` |

### Step 0 — tools

```bash
# Fedora
sudo dnf install azure-cli
# or, any distro:  curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

az login
az account set --subscription 87f800b8-0c86-4693-9837-41192429a956
az account show --query "{name:name, id:id, tenant:tenantId}" -o table

az extension add --name containerapp --upgrade
```

`az login` opens a browser — sign in with the account that owns the subscription. If you
have more than one, the `az account set` line above is what guarantees the rest of the
runbook targets the right one; `az account show` confirms it.

---

### Part A — provision Azure (one-off)

**A1. Register resource providers.** New subscriptions often have these unregistered, and
every later step fails without them. Takes a couple of minutes to complete.

```bash
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.OperationalInsights
az provider register --namespace Microsoft.ContainerRegistry
az provider register --namespace Microsoft.Sql

# wait until all say "Registered"
az provider show -n Microsoft.App --query registrationState -o tsv
```

**A2. Resource group.**

```bash
az group create -n rg-learnit -l westeurope
```

**A3. Container registry.** The name is global, so confirm it is free first.

```bash
az acr check-name -n acrlearnit          # expect "nameAvailable": true
az acr create -n acrlearnit -g rg-learnit --sku Basic --admin-enabled true

# If the registry already exists without it, the container app cannot pull the image:
az acr update -n acrlearnit --admin-enabled true
az acr show -n acrlearnit --query adminUserEnabled -o tsv    # -> true
```

**A4. Azure SQL.** Serverless with auto-pause is what keeps this cheap — compute billing
stops after an hour idle.

```bash
SQLPASS='<pick-a-strong-password>'

az sql server create -n learnit-sql -g rg-learnit -l westeurope \
  --admin-user learnitadmin --admin-password "$SQLPASS"

az sql db create -g rg-learnit -s learnit-sql -n LearnIT \
  --edition GeneralPurpose --family Gen5 --capacity 1 \
  --compute-model Serverless --auto-pause-delay 60 \
  --backup-storage-redundancy Local

# Container Apps has no fixed outbound IP unless VNet-integrated, so allow Azure services.
az sql server firewall-rule create -g rg-learnit -s learnit-sql \
  -n AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

# Add your own IP too, so you can inspect the database from your machine.
MYIP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create -g rg-learnit -s learnit-sql \
  -n MyWorkstation --start-ip-address $MYIP --end-ip-address $MYIP
```

**A5. Container Apps environment.**

```bash
az containerapp env create -n learnit-env -g rg-learnit -l westeurope
```

---

### Part B — first deployment (from your workstation)

**B1. Fill in the deploy config.** `deploy/aca.env` is git-ignored; the example already
carries the subscription, ACR, region and names.

```bash
cp deploy/aca.env.example deploy/aca.env
openssl rand -base64 48        # paste as JWT_KEY
```

Set in `deploy/aca.env`:

- `JWT_KEY` — the generated value
- `SQL_CONNECTION_STRING`:
  ```bash
  SQL_CONNECTION_STRING='Server=tcp:learnit-sql.database.windows.net,1433;Initial Catalog=LearnIT;User ID=learnitadmin;Password=<SQLPASS>;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;Connection Timeout=60;'
  ```
- `SMTP_USERNAME` / `SMTP_PASSWORD` / `SENDER_EMAIL` — a Gmail **app password**, not the
  account password. Leave blank to deploy without working confirmation emails.

> **Quote the values.** The file is sourced by bash, so anything containing a space breaks
> without quotes — `Initial Catalog=LearnIT` would be read as a command named `Initial`.
> Use single quotes for secrets so `$` and backticks in a password are not expanded.

The app applies EF migrations at startup, so this login needs DDL rights. `learnitadmin` is
the server admin and has them; create a dedicated contained user before this goes public.

**B2. Deploy.**

```bash
./deploy/deploy-aca.sh v1
```

This one command: pins the subscription, builds the image with ACR Tasks (server-side — no
local Docker needed), creates the container app, reads back its ingress FQDN and injects it
as `BaseServerAddress`, then polls `/health` until the app answers.

The script prints the real URL when it finishes. It has the shape
`https://learnit.<random-suffix>.westeurope.azurecontainerapps.io`, where `<random-suffix>`
is generated when the Container Apps environment is created — there is no way to guess or
construct it, always read it back from Azure:

```bash
az containerapp show -g rg-learnit -n learnit \
  --query properties.configuration.ingress.fqdn -o tsv
```

That FQDN is **stable across revisions** — safe to bookmark and to put in email templates.

**B3. Verify.**

```bash
APP="https://$(az containerapp show -g rg-learnit -n learnit \
  --query properties.configuration.ingress.fqdn -o tsv)"

curl -s $APP/health          # -> Healthy
curl -s -o /dev/null -w '%{http_code}\n' $APP/          # -> 200, Blazor client
curl -s -o /dev/null -w '%{http_code}\n' $APP/tutors    # -> 200, API
```

Open the URL in a browser: the tutor list should render with logo images. Broken images
mean `BaseServerAddress` does not match the FQDN.

---

### Part C — Azure DevOps CI/CD

Everything above is done from your machine. This part makes `git push` deploy.

**C1. Push the repo to Azure Repos** (or point the pipeline at GitHub — either works).

```bash
git remote add azure https://dev.azure.com/<org>/<project>/_git/LearnIT
git push azure main
```

**C2. Service connections** — Project settings → Service connections → New:

- *Docker Registry* → **Azure Container Registry** → subscription `Azure subscription 1`,
  registry `acrlearnit` → name it exactly **`learnit-acr`**
- *Azure Resource Manager* → Workload identity federation (automatic) → subscription
  `Azure subscription 1` → name it exactly **`Azure subscription 1`**

> The ARM connection name must match the `azureSubscription` pipeline variable. ADO defaults
> to `Azure subscription 1(87f800b8-…)` — rename it, or change the variable to match.
>
> **This is the step most likely to fail.** The automatic flow creates an Entra ID app
> registration, which managed tenants (university, corporate) often forbid for non-admins —
> subscription Owner does not grant it. If it fails, keep deploying with
> `deploy/deploy-aca.sh` (Part B), which needs none of this, and ask a tenant admin for an
> app registration or a manually-configured service principal.

**C3. Variable group** — Pipelines → Library → New variable group named **`learnit-secrets`**,
with these, each marked secret (padlock):

| Name | Value |
| --- | --- |
| `SQL_CONNECTION_STRING` | same string as `deploy/aca.env` |
| `JWT_KEY` | same key as `deploy/aca.env` |
| `SMTP_USERNAME` / `SMTP_PASSWORD` | Gmail address + app password |
| `SENDER_EMAIL` | sender address |

**C4. Environment** — Pipelines → Environments → New → **`learnit-aca`**. This is what gives
you deployment history and optional approval gates.

**C5. Create the pipeline** — Pipelines → New pipeline → your repo → *Existing Azure Pipelines
YAML file* → `/azure-pipelines.yml` → Save.

**C6. Run it.** *Deployment target* defaults to `containerapp`. Each run builds a new image
tagged with the build number, rolls out a revision, waits for `/health`, and **shifts traffic
back to the previous revision if the new one fails**.

The pipeline expects the container app to already exist — that is why Part B comes first.

---

### Day-to-day

```bash
RG=rg-learnit

az containerapp logs show -g $RG -n learnit --follow
az containerapp revision list -g $RG -n learnit -o table
az containerapp ingress traffic set -g $RG -n learnit --revision-weight <old-revision>=100
az containerapp secret set -g $RG -n learnit --secrets jwt-key=<new-value>

# cost control: scale to zero when idle (cold start on the next request)
az containerapp update -g $RG -n learnit --min-replicas 0

# tear everything down
az group delete -n rg-learnit --yes
```

---

## 5. Alternative target — Azure Container Instances

Only if you deliberately choose ACI over Container Apps (see §1 — no HTTPS, outage per deploy).
Part A applies except A5; then:

```bash
cp deploy/aci.env.example deploy/aci.env    # fill in JWT_KEY + SQL_CONNECTION_STRING
./deploy/deploy-aci.sh v1
```

URL is derived from the DNS label: `http://learnit-app.westeurope.azurecontainer.io:8080`.
In Azure DevOps, queue the pipeline with *Deployment target* = `containerinstance` and create
an environment named `learnit-aci`.

```bash
az container logs -g rg-learnit -n learnit-app --follow
az container restart -g rg-learnit -n learnit-app
az container show -g rg-learnit -n learnit-app --query instanceView.state -o tsv
```

---

## 6. Cost

Always-on, this setup runs roughly $35-50/month — ACR Basic ~$5, Container Apps at
`min-replicas 1` ~$25-30, SQL serverless $5-15 depending on how much it is awake. On a
credit-limited subscription that is about two months of a $100 grant, after which resources
are disabled rather than billed onward.

Setting `--min-replicas 0` together with the SQL `--auto-pause-delay 60` already configured
drops an idle app to a few dollars a month. The cost is a double cold start on the first
request — container boot plus EF migrations plus the database resuming, so 10-40 seconds.
Fine for a portfolio project; not something to rely on during a live demo. Verify current
prices in the Azure pricing calculator for your region rather than trusting these figures.

---

## 7. Configuration reference

Nested keys use `__` (double underscore) as the section separator in environment variables.

| Variable | Notes |
| --- | --- |
| `LearnITDbConnectionString` | **secret** — Azure SQL, `Encrypt=True` |
| `Jwt__Key` | **secret** — HS256 key, 32+ chars; the app refuses to start without it |
| `Jwt__Issuer`, `Jwt__Audience` | `LearnITServer` |
| `BaseServerAddress` | Public origin; used to build tutor logo URLs |
| `EmailConfirmationControllerAddress` | `<public origin>/email` |
| `HomeFrontedPageAddress` | `<public origin>/` |
| `SmtpClientSettings__UserName` / `__Password` | **secret** — read at startup; `__Port` must be numeric |
| `SenderEmail` | Sender for confirmation emails |
| `ASPNETCORE_ENVIRONMENT` | `Production` in Azure. `Development` enables Swagger **and** the Bogus `DBSeeder` — never set it in Azure |
| `ASPNETCORE_HTTP_PORTS` | `8080`, must match the ingress/exposed port |

---

## 8. What changed in the repo, and why

**Dockerfile**
- `COPY . .` happened *before* `dotnet restore`, so every source edit re-downloaded all
  NuGet packages. Project files are copied first now and restore is cached.
- No `.dockerignore` existed, so local `bin/`/`obj/` (built on a different SDK/OS) went into
  the build context and could leak into the output.
- Removed the `RUN ls -la /app/wwwroot` debug layer.
- Runs as the non-root `app` user (UID 1654) shipped with the .NET 8 images.
- Port 5000/5001 → **8080**, the .NET 8 image default. 5001 was exposed with no HTTPS
  certificate configured, so nothing ever listened there.

**docker-compose.yml**
- The SA password was hard-coded and committed; it comes from git-ignored `.env` now, using
  the current `MSSQL_SA_PASSWORD` variable name.
- `depends_on` waited only for the SQL *container*, not for SQL to accept connections. Since
  `Program.cs` calls `Database.Migrate()` during startup, the app crash-looped on a cold
  start. There is a `sqlcmd` healthcheck with `condition: service_healthy` now.
- `Jwt__Key` was never supplied, so the app threw `JwtSettings:SecretKey is not configured`
  on boot. It is a required `.env` value now.
- `BaseServerAddress` / `EmailConfirmationControllerAddress` / `HomeFrontedPageAddress`
  stayed at `http://localhost:5133`, so tutor logo URLs and email confirmation links pointed
  at a port nothing was listening on. They follow the published port now.

**Program.cs** — two small deployment-driven changes
- `EnableRetryOnFailure` on the SQL Server provider. Azure SQL raises transient errors on
  failover and when a serverless database wakes from auto-pause; without retries the startup
  migration fails and the platform restart-loops the container.
- `MapHealthChecks("/health")` — the liveness signal used by the deploy gate.

---

## 9. Constraints worth knowing

- **Migrations run at app startup.** With more than one replica, several instances can call
  `Database.Migrate()` at once, which EF does not coordinate. `acaMaxReplicas` is therefore
  pinned to **1**. To scale out, move migrations to a separate pipeline step
  (`dotnet ef database update` or a bundle) and drop the `Migrate()` call from `Program.cs`.
- **Compose does not deploy to ACI.** The Docker → ACI cloud integration (`docker compose up`
  against an `aci` context) was retired. The compose files are local-only; the cloud path is
  `az containerapp` / `az container`.
- **Storage is ephemeral** on both targets. Not a problem here: tutor logos live in SQL as
  `varbinary`, and the only files read at runtime (`defultTutorLogo.jpg`, the email template)
  ship inside the image.
- **The Data Protection warning in the logs is expected.** Keys are written to the container
  filesystem and lost on restart. Harmless here — auth is JWT signed with the configured
  `Jwt__Key`, and nothing depends on Data Protection surviving a restart.
- **ACR admin credentials** are used for image pulls to keep the first deployment simple. For
  a longer-lived setup, give the container app/group a user-assigned managed identity with
  the `AcrPull` role and drop the username/password flags.
- **CORS is wide open** (`AllowAnyOrigin`) in `Program.cs`. Harmless while the API and client
  share one origin, but worth tightening before this is public.

---

## 10. Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| `JwtSettings:SecretKey is not configured` | `Jwt__Key` not set on the container |
| `DBConectionString is null` | `LearnITDbConnectionString` not set |
| `Cannot open server 'learnit-sql' … Client with IP address … is not allowed` (error 40615) | Azure SQL firewall — the `0.0.0.0`/`0.0.0.0` "allow Azure services" rule is missing. See A4; allow up to 5 minutes to apply, then restart the revision |
| Container exits with **139** and the platform reports a segfault | Misleading — look at the *console* logs (`--type console`), not the system events. An unhandled exception during `Database.Migrate()` at startup surfaces this way |
| Startup hangs, then `A network-related or instance-specific error` | Cannot reach the server at all; on serverless the first connection also has to wake the database |
| `Login failed for user` | Wrong credentials, or the login lacks rights on the `LearnIT` database |
| `CREATE TABLE permission denied` | Migration login needs `db_owner` / `db_ddladmin` |
| App up, tutor images broken | `BaseServerAddress` does not match the public origin |
| Email links point at localhost | `EmailConfirmationControllerAddress` / `HomeFrontedPageAddress` not set |
| Swagger reachable in Azure | `ASPNETCORE_ENVIRONMENT` is `Development` — it must be `Production` |
| Container Apps revision stuck provisioning | `az containerapp logs show -g <rg> -n <app> --tail 200` |
