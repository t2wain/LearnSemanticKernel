using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;

namespace AIAgentExample.Example.MCP
{
    public static class McpExample
    {
        public static string ProjectPath = "C:\\devgit\\LearnSemanticKernel\\src\\McpServerApp\\McpServerApp.csproj";
        public static string BaseURL = "https://localhost";

        public static Task<McpClient> GetMcpStioClient(string appPath, string name)
        {
            Task<McpClient> localMcpServer = McpClient.CreateAsync(new StdioClientTransport(new()
            {
                Command = "dotnet",
                Arguments = ["run", "--project", appPath],
                Name = name
            }));

            return localMcpServer;
        }

        public static Task<McpClient> GetMcpHttpClient(string baseUrl, string name)
        {
            Task<McpClient> remoteMcpServer = McpClient.CreateAsync(new HttpClientTransport(new()
            {
                Endpoint = new Uri(baseUrl),
                Name = name
            }));

            return remoteMcpServer;
        }

        public static ValueTask<IList<McpClientTool>> ListTools(McpClient client) =>
            client.ListToolsAsync();


        public static async Task<object?> McpStioServerExample()
        {

            var localMcpServer = await GetMcpStioClient(ProjectPath, "stdio-coordinate-server");

            var tools = ListTools(localMcpServer);
            List<AITool> availableTools = [.. await tools];

            return null;
        }


        public static async Task<object?> McpHttpServerExample()
        {
            var remoteMcpServer = await GetMcpHttpClient(BaseURL, "streamable-http-geocoding-server");

            var tools = ListTools(remoteMcpServer);
            List<AITool> availableTools = [.. await tools];

            return null;
        }

    }
}
