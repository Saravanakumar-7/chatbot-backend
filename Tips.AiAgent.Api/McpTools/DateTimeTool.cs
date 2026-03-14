using ModelContextProtocol.Server;
using System.ComponentModel;

namespace Tips.AiAgent.Api.McpTools
{
    [McpServerToolType]
    public static class DateTimeTool
    {
        [McpServerTool, Description("Returns the current server date and time in ISO 8601 format.")]
        public static string GetCurrentDateTime() => DateTime.Now.ToString("o");
    }
}
