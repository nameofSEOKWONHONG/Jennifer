using Jennifer.Mcp;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    // 👇 Add MCP Server to IoC
    .AddMcpServer()
    // 👇 Register MCP Tool
    .WithTools<TimeTool>();

var app = builder.Build();

// 👇 Map Mcp endpoints
app.MapMcp();

app.Run();