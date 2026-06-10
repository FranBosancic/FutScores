# Google sign-in (3rd-party authentication)

The app supports logging in with Google on top of the local Identity accounts.
The integration is **always compiled in**, but the "Continue with Google" button
only appears once Google OAuth credentials are configured. Without credentials the
app runs exactly as before — nothing breaks.

## 1. Create OAuth credentials in Google Cloud Console

1. Go to <https://console.cloud.google.com/> and create (or pick) a project.
2. **APIs & Services → OAuth consent screen** → configure it (External, app name,
   support email). Add your Google account as a *test user* while it's in testing.
3. **APIs & Services → Credentials → Create Credentials → OAuth client ID**.
   - Application type: **Web application**
   - **Authorized redirect URIs** — add the callback path `/signin-google` for
     every origin you run on:
     - `https://localhost:7075/signin-google`  (the `https` launch profile)
     - `http://localhost:5009/signin-google`   (the `http` launch profile)
4. Copy the generated **Client ID** and **Client secret**.

> The callback path `/signin-google` is the default handled by the Google
> middleware — you don't write that endpoint yourself.

## 2. Store the credentials in user-secrets

Secrets must never be committed. This project already has a `UserSecretsId`, so
from the `ProbaMala/ProbaMala` folder run:

```bash
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

Verify with `dotnet user-secrets list`.

In production, supply the same two keys via environment variables / a secret
store instead, e.g. `Authentication__Google__ClientId`.

## 3. Run and test

1. Start the database: `docker compose up -d` (from the repo `ProbaMala` folder).
2. Run with an HTTPS profile (Google prefers HTTPS):
   `dotnet run --launch-profile https`
3. Open the login page → click **Continue with Google**.
4. First time for a Google account: you'll be sent to the **Complete your
   registration** page to enter **OIB** and **JMBG** (both required on `AppUser`),
   which finishes creating the local account. Returning users are signed straight in.

## How it's wired up (for reference)

- `Program.cs` — registers Google only when both config values are present.
- `Areas/Identity/Pages/Account/Login.cshtml(.cs)` — lists external schemes and
  renders the Google button.
- `Areas/Identity/Pages/Account/ExternalLogin.cshtml(.cs)` — runs the OAuth
  challenge/callback and collects OIB/JMBG on first login. New external accounts
  get the `User` role, same as local registration.
