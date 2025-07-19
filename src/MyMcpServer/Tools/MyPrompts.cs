using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace MyMcpServer.Tools;

[McpServerPromptType]
public static class MyPrompts
{
    [McpServerPrompt(Name = "Summarize"), Description("Creates a prompt to summarize the provided message.")]
    public static ChatMessage Summarize([Description("The content to summarize")] string content)
    {
        return new(ChatRole.User, $"Please summarize this content into a single sentence: {content}");
    }

    [McpServerPrompt(Name = "FunctionalProgramming"), Description("Creates a Notion prompt to generate the functional programming way of refactoring the code.")]
    public static ChatMessage RefactorWithFunctionalProgramming()
    {
        return new(ChatRole.User, $"Please generate the a prompt for Notion MCP which will go through Functional Programming page and generate the refactored code according to the examples and use cases defined there");
    }
}
