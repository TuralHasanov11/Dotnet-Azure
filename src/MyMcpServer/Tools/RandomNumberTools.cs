using System.ComponentModel;
using System.Security.Cryptography;
using ModelContextProtocol.Server;

namespace MyMcpServer.Tools;

/// <summary>
/// Sample MCP tools for demonstration purposes.
/// These tools can be invoked by MCP clients to perform various operations.
/// </summary>
internal class RandomNumberTools
{
    [McpServerTool]
    [Description("Generates a random number between the specified minimum and maximum values.")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100)
    {
        // Use a cryptographically secure random number generator
        if (min >= max)
        {
            throw new ArgumentException("min must be less than max.");
        }

        return RandomNumberGenerator.GetInt32(min, max);
    }
}
