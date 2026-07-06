using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using ProbaMala.Mcp;

var builder = Host.CreateApplicationBuilder(args);

// The stdio transport uses stdout for the MCP protocol itself, so every log line MUST
// go to stderr — a stray stdout write would corrupt the protocol stream.
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Typed HTTP client pointed at the running FutScores web app. Base URL and admin
// credentials come from configuration (appsettings.json / env), with dev defaults.
var baseUrl = builder.Configuration["FutScores:BaseUrl"] ?? "http://localhost:5009";
builder.Services.AddHttpClient<FutScoresApiClient>(client => client.BaseAddress = new Uri(baseUrl));

// Register the MCP server over stdio and auto-discover the [McpServerToolType] classes.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
