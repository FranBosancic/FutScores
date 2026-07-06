# FutScores MCP server

An [MCP](https://modelcontextprotocol.io) server that exposes FutScores to agentic IDEs
(Claude Code, Cursor, VS Code…). It's a thin translator: each MCP **tool** calls the
FutScores REST API over HTTP, so all business logic, validation and auth stay in the web
app.

```
Agentic IDE ──stdio──▶ ProbaMala.Mcp ──HTTP──▶ FutScores /api/* ──▶ Postgres
```

## Tools

Reads (public GET endpoints — no auth):
`search`, `list_leagues`, `list_clubs`, `list_players`, `get_player`, `list_matches`,
`get_match`, `list_ratings`, `list_users`.

Write (obtains a JWT from `/api/auth/token` using the admin account):
`add_rating`.

## Configuration

`appsettings.json` (or environment variables), all with dev defaults:

- `FutScores:BaseUrl` — the running web app (default `http://localhost:5009`).
- `FutScores:Admin:Email` / `FutScores:Admin:Password` — used only by `add_rating` to
  fetch a JWT (default: the seeded dev admin).

## Run it in Claude Code

1. **Start the web app** (the tools call its API):
   ```
   dotnet run --project ProbaMala/ProbaMala --launch-profile http
   ```
   (Postgres must be up: `docker compose -f ProbaMala/docker-compose.yml up -d postgres`.)
2. **Build the MCP server once** so the DLL exists:
   ```
   dotnet build ProbaMala/ProbaMala.Mcp
   ```
3. The repo-root [`.mcp.json`](../../.mcp.json) already registers the server. Open the
   project in Claude Code and approve the `futscores` MCP server when prompted (or run
   `claude mcp list` to see it). Then ask things like:
   - *"Use futscores to list the Premier League clubs."*
   - *"Search FutScores for Salah and show his ratings."*
   - *"Add a rating of 8 for player 1 in match 1 (find a user id first)."*

Other IDEs (Cursor, VS Code agent mode) use the same `command`/`args`, just in their own
MCP config location.

## Verify without an IDE

Pipe a JSON-RPC handshake to the server over stdio (stdout carries the protocol, logs go
to stderr):

```bash
{ printf '%s\n' \
  '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"cli","version":"1.0.0"}}}' \
  '{"jsonrpc":"2.0","method":"notifications/initialized"}' \
  '{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}'; sleep 3; } \
  | dotnet ProbaMala.Mcp/bin/Debug/net8.0/ProbaMala.Mcp.dll 2>/dev/null
```

The [MCP Inspector](https://github.com/modelcontextprotocol/inspector) is a nicer GUI for
the same thing.
