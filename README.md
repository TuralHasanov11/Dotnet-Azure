# Dotnet-Azure

A multi-service .NET solution integrating Azure Functions, WebJobs, ASP.NET Core, and Model Context Protocol (MCP) for AI-powered workflows and automation.

## Prerequisites

- [.NET 8+ SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [Node.js (for MCP servers)](https://nodejs.org/)
- Azure account (for cloud deployment)

## Installation

Clone the repository:

```sh
git clone https://github.com/TuralHasanov11/Dotnet-Azure.git
cd Dotnet-Azure
```

### Docker Setup

Build and run services with Docker Compose:

```sh
docker-compose up --build
```

For MCP servers (Notion, Playwright, Azure, Terraform, Codacy), see `mcp.json` for Docker and npx commands.

## Usage

### Available Tools

- **Azure Functions**: Event-driven serverless compute
- **WebApp**: ASP.NET Core web application
- **WebJob1**: Background job, package with PowerShell (see `Commands.md`)
- **MyMcpServer**: Custom MCP server exposing tools for AI assistants
- **MCP Integrations**: Notion, Playwright, Azure, Terraform, Codacy (see `mcp.json`)

## Development

### Project Structure

```
Dotnet-Azure/
├── src/
│   ├── WebApp/           # ASP.NET Core app
│   ├── FunctionApp1/     # Azure Function (v4)
│   ├── FunctionApp2/     # Azure Function (v4)
│   ├── WebJob1/          # WebJob project
│   ├── SharedKernel/     # Shared code
│   ├── MyMcpServer/      # Custom MCP server
├── .github/
├── docker-compose.yml
├── mcp.json              # MCP server config
├── Commands.md           # Workflow commands
└── README.md
```

### Running Tests

- Use Visual Studio Test Explorer or run:
  ```sh
  dotnet test
  ```
- Add new tests in `/tests` folders mirroring the main app structure.

## Model Context Protocol Integration

### How It Works

- MCP servers expose tools for AI agents (Copilot, etc.) via stdio or Docker.
- Custom tools (e.g., echo) are implemented in `src/MyMcpServer`.

### Using with AI Assistants

- Configure your AI assistant (e.g., Copilot Chat) to use the MCP servers defined in `mcp.json`.
- Provide required tokens via prompts or environment variables.

## Environment Variables

- `AzureWebJobsStorage`: Azure Functions storage connection string
- `FUNCTIONS_WORKER_RUNTIME`: Worker runtime for Azure Functions
- `NOTION_TOKEN`, `codacy_account_token`: API tokens for MCP integrations
- See `local.settings.json` and `mcp.json` for details

## License

MIT License

Copyright (c) 2025 TuralHasanov11

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
