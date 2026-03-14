using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public static class GreetingTool
    {
        [McpServerTool, Description("Returns a personalised greeting for the given name.")]
        public static string Greet(string name) => $"Hello, {name}! Welcome to the Tips AI Agent.";
    }
}
