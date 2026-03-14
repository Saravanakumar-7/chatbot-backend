using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public static class EchoTool
    {
        [McpServerTool, Description("Echoes the input message back to the caller. Useful for testing connectivity.")]
        public static string Echo(string message) => $"Echo: {message}";
    }
}
