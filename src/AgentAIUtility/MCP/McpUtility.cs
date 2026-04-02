using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using System.Reflection;

namespace AgentAIUtility.MCP
{
    public static class McpUtility
    {
        public static IServiceCollection ConfigureMcpStioServer(this IServiceCollection services, Assembly toolAssembly)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithStdioServerTransport()
             .WithResourcesFromAssembly(toolAssembly)
             .WithPromptsFromAssembly(toolAssembly)
             .WithToolsFromAssembly(toolAssembly);

            return services;
        }

        public static IServiceCollection ConfigureMcpWebServer(this IServiceCollection services, Assembly toolAssembly)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithHttpTransport()
             .WithResourcesFromAssembly(toolAssembly)
             .WithPromptsFromAssembly(toolAssembly)
             .WithToolsFromAssembly(toolAssembly);

            return services;
        }


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

        public static async Task<IEnumerable<AITool>> ListAITools(McpClient client)
        {
            IList<McpClientTool> tools = await client.ListToolsAsync();
            return [.. tools];
        }

    }
}
