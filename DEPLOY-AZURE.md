# Deploying FutScores to Azure (for Students)

> Vault: [[Home]] · concept note: [[Deploy]] · plan: [[PROJECT-PLAN]]

Target: **Azure Container Apps** (the app; scales to zero when idle ≈ free) +
**Azure Database for PostgreSQL Flexible Server** (the DB). The app is containerised via
`ProbaMala/ProbaMala/Dockerfile`; migrations + the seed admin run automatically on first
start, so the schema and data appear by themselves.

## 0. Prerequisites (one-time)

1. Create an **Azure for Students** account: <https://azure.microsoft.com/free/students>
   — verify with your student email, get **$100 credit, no credit card**.
2. Install the **Azure CLI** (`az`): <https://learn.microsoft.com/cli/azure/install-azure-cli>
3. Sign in and add the Container Apps extension:
   ```bash
   az login
   az extension add --name containerapp --upgrade
   az provider register --namespace Microsoft.App
   az provider register --namespace Microsoft.OperationalInsights
   ```

You do **not** need Docker locally — Container Apps builds the image in the cloud.

## 1. Set some variables

Pick a unique server name and a strong DB password.

```bash
RG=futscores-rg
LOC=westeurope
PG=futscores-pg-$RANDOM          # must be globally unique
PGADMIN=pgadmin
PGPASS='ChangeMe_Strong!123'     # choose your own
APP=futscores
ENVNAME=futscores-env
```

## 2. Resource group + PostgreSQL

```bash
az group create -n "$RG" -l "$LOC"

# Cheapest burstable tier; "Allow Azure services" firewall so the app can reach it.
az postgres flexible-server create \
  --resource-group "$RG" --name "$PG" --location "$LOC" \
  --admin-user "$PGADMIN" --admin-password "$PGPASS" \
  --tier Burstable --sku-name Standard_B1ms \
  --storage-size 32 --version 16 \
  --public-access 0.0.0.0 --yes

az postgres flexible-server db create \
  --resource-group "$RG" --server-name "$PG" --database-name probamala
```

## 3. Build + deploy the app from source (one command)

```bash
CONNSTR="Host=$PG.postgres.database.azure.com;Port=5432;Database=probamala;Username=$PGADMIN;Password=$PGPASS;SSL Mode=Require;Trust Server Certificate=true"
JWTKEY="$(openssl rand -base64 48)"   # a real production signing key

az containerapp up \
  --name "$APP" --resource-group "$RG" --location "$LOC" \
  --environment "$ENVNAME" \
  --source ProbaMala/ProbaMala \
  --ingress external --target-port 8080 \
  --env-vars "ConnectionStrings__Postgres=$CONNSTR" "Jwt__Key=$JWTKEY" "ASPNETCORE_ENVIRONMENT=Production"
```

`az containerapp up --source` reads `ProbaMala/ProbaMala/Dockerfile`, builds the image in
an auto-created registry, and deploys it. First run takes a few minutes.

Get the public URL:

```bash
az containerapp show -n "$APP" -g "$RG" --query properties.configuration.ingress.fqdn -o tsv
```

Open `https://<that-fqdn>` — the app runs migrations on startup, so it's ready. Log in with
the seeded admin (`admin@futscores.local` / `Admin123!`) unless you overrode it (below).

## 4. Optional extras

- **Change the seed admin / add the AI key** — set more env vars any time:
  ```bash
  az containerapp update -n "$APP" -g "$RG" --set-env-vars \
    "SeedAdmin__Email=you@example.com" "SeedAdmin__Password=YourStrong!Pass1" \
    "Ai__ApiKey=sk-ant-..." "Ai__Model=claude-haiku-4-5"
  ```
- **Google login** — set `Authentication__Google__ClientId` / `__ClientSecret` and add
  `https://<fqdn>/signin-google` as an authorised redirect URI in the Google console.

## 5. Cost control

- Container Apps **scales to zero** when idle — you pay ~nothing for the app.
- The Postgres server bills while it exists (~$12–13/mo, covered by the credit). Stop it
  when you're not demoing, start it before:
  ```bash
  az postgres flexible-server stop  --resource-group "$RG" --server-name "$PG"
  az postgres flexible-server start --resource-group "$RG" --server-name "$PG"
  ```
- Tear the whole thing down when finished:
  ```bash
  az group delete -n "$RG" --yes --no-wait
  ```

## Why the app already works behind Azure

- The **Dockerfile** listens on `:8080` (matches `--target-port 8080`).
- `Program.cs` uses **ForwardedHeaders** so HTTPS redirection works behind Azure's
  TLS-terminating ingress (no redirect loop).
- Config is read from environment variables (`ConnectionStrings__Postgres`, `Jwt__Key`,
  `SeedAdmin__*`, `Ai__*`) — no secrets are baked into the image.
