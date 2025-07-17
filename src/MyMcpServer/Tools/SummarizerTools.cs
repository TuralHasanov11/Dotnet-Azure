using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace MyMcpServer.Tools;

internal static class SummarizerTools
{
    [McpServerTool(Name = "SummarizeContentFromUrl"), Description("Summarizes content downloaded from a specific URI")]
    public static async Task<string> SummarizeDownloadedContent(
        IMcpServer thisServer,
        HttpClient httpClient,
        [Description("The url from which to download the content to summarize")] string url,
        CancellationToken cancellationToken)
    {
        string content = await httpClient.GetStringAsync(new Uri(url), cancellationToken);

        ChatMessage[] messages =
        [
            new(ChatRole.User, "Briefly summarize the following downloaded content:"),
            new(ChatRole.User, content),
        ];

        ChatOptions options = new()
        {
            MaxOutputTokens = 256,
            Temperature = 0.3f,
        };

        return $"Summary: {await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken)}";
    }
}
