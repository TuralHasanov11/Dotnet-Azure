using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using MyMcpServer.Tools;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

McpServerOptions options = new()
{
    ServerInfo = new Implementation { Name = "MyServer", Version = "1.0.0" },
    Capabilities = new ServerCapabilities
    {
        Tools = new ToolsCapability
        {
            ListToolsHandler = (request, cancellationToken) =>
                ValueTask.FromResult(new ListToolsResult
                {
                    Tools =
                    [
                        new Tool
                        {
                            Name = "echo",
                            Description = "Echoes the input back to the client.",
                            InputSchema = JsonSerializer.Deserialize<JsonElement>("""
                                {
                                    "type": "object",
                                    "properties": {
                                      "message": {
                                        "type": "string",
                                        "description": "The input to echo back"
                                      }
                                    },
                                    "required": ["message"]
                                }
                                """),
                        }
                    ]
                }),

            CallToolHandler = (request, cancellationToken) =>
            {
                if (request.Params?.Name == "echo")
                {
                    if (request.Params.Arguments?.TryGetValue("message", out var message) is not true)
                    {
                        throw new McpException("Missing required argument 'message'");
                    }

                    return ValueTask.FromResult(new CallToolResult
                    {
                        Content = [new TextContentBlock { Text = $"Echo: {message}", Type = "text" }]
                    });
                }

                throw new McpException($"Unknown tool: '{request.Params?.Name}'");
            },
        }
    },
};


// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>();

await builder.Build().RunAsync();
