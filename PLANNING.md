# Dotnet-Azure - Project Planning

## Project Overview

This project is a multi-service .NET solution integrating Azure Functions, WebJobs, ASP.NET Core, and Model Context Protocol (MCP) servers. It enables AI assistants and automation workflows to interact with cloud resources and custom tools through standardized interfaces.

## Architecture

- **Transport Layer**: Stdio (for MCP servers), HTTP (for WebApp, Functions)
- **Protocol**: Model Context Protocol (MCP)
- **Frameworks**: .NET 8/9 SDK, Azure Functions v4, ASP.NET Core
- **Containerization**: Docker Compose for orchestration
- **Integration**: MCP servers for Notion, Playwright, Azure, Terraform, Codacy

## Components

1. **WebApp**

   - ASP.NET Core web application
   - Uses SharedKernel for common logic
   - Integrates with Azure services (Graph, Storage, etc.)

2. **FunctionApp1 & FunctionApp2**

   - Azure Functions (v4, .NET 8/9)
   - Event-driven compute, integrates with Azure Storage, ServiceBus, EventHubs

3. **WebJob1**

   - Background job, packaged for deployment
   - Uses PowerShell for packaging (see Commands.md)

4. **SharedKernel**

   - Shared code and utilities for all services

5. **MyMcpServer**

   - Custom MCP server implemented in C#
   - Exposes tools (e.g., echo) for AI assistants
   - Packaged as a NuGet MCP server

6. **MCP Integrations**
   - Notion, Playwright, Azure, Terraform, Codacy servers (see mcp.json)
   - Configured for stdio or Docker transport

## Environment Configuration

- `AzureWebJobsStorage`: Azure Functions storage connection string
- `FUNCTIONS_WORKER_RUNTIME`: Worker runtime for Azure Functions
- `NOTION_TOKEN`, `codacy_account_token`: API tokens for MCP integrations
- See `local.settings.json`, `mcp.json`, and `.env` files for details

## File Structure

```
Dotnet-Azure/
├── src/
│   ├── WebApp/
│   ├── FunctionApp1/
│   ├── FunctionApp2/
│   ├── WebJob1/
│   ├── SharedKernel/
│   ├── MyMcpServer/
├── .github/
├── docker-compose.yml
├── mcp.json
├── Commands.md
├── README.md
├── PLANNING.md
└── TASK.md
```

## Style Guidelines

- **C#**: Follow .editorconfig and dotnet format conventions
- Use `FluentValidation` and `DataAnnotations` for validation
- Document functions with XML doc comments (Google style)
- Organize code as vertical slices by feature/responsibility
- Never create files longer than 1000 lines; refactor as needed

## Dependencies

- .NET SDK (8/9)
- Azure Functions SDK
- ModelContextProtocol SDK
- Microsoft Graph, Azure Storage, ServiceBus, EventHubs
- Docker, Node.js (for MCP servers)
- MCP server packages: Notion, Playwright, Azure, Terraform, Codacy
