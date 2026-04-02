using AgentAIUtility.MCP;
using Microsoft.Extensions.AI;

namespace AIAgentExample.Example.MCP
{
    public static class McpExample
    {
        public static string ProjectPath = "C:\\devgit\\LearnSemanticKernel\\src\\McpServerApp\\McpServerApp.csproj";
        public static string BaseURL = "http://localhost:5000";

        public static async Task<object?> McpStioServerExample()
        {

            var localMcpServer = await McpUtility.GetMcpStioClient(ProjectPath, "stdio-coordinate-server");

            var tools = await McpUtility.ListTools(localMcpServer);
            List<AITool> availableTools = [.. tools];

            return availableTools;
        }


        public static async Task<object?> McpHttpServerExample()
        {
            var remoteMcpServer = await McpUtility.GetMcpHttpClient(BaseURL, "streamable-http-geocoding-server");

            var tools = await McpUtility.ListTools(remoteMcpServer);
            List<AITool> availableTools = [.. tools];

            return availableTools;
        }

    }
}
