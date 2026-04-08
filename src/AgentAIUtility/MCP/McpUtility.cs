using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Client;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Reflection;

namespace AgentAIUtility.MCP
{
    public static class McpUtility
    {
        #region Server Configuration

        public static IMcpServerBuilder ConfigureMcpStioServer(
            this IServiceCollection services, 
            IEnumerable<McpServerTool> tools)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithStdioServerTransport()
             .WithResourcesFromAssembly()
             .WithPromptsFromAssembly()
             .WithTools(tools);

            return mb;
        }

        public static IMcpServerBuilder ConfigureMcpWebServer(
            this IServiceCollection services, 
            IEnumerable<McpServerTool> tools)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithHttpTransport()
             .WithResourcesFromAssembly()
             .WithPromptsFromAssembly()
             .WithTools(tools);

            return mb;
        }

        public static IEnumerable<McpServerTool> CreateTools(object objectInstance)
        {
            // Get the object's type
            Type type = objectInstance.GetType();

            // Get all methods (public, instance only, inherited included)
            IEnumerable<MethodInfo> methods =
                type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance
                )
                .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null)
                .ToList();

            return CreateTools(objectInstance, methods);
        }

        public static IEnumerable<McpServerTool> CreateTools(
            object objectInstance,
            IEnumerable<MethodInfo> methodInfos) =>
                methodInfos
                    .Select(m => McpServerTool.Create(m, objectInstance))
                    .ToList();

        #endregion

        #region Client

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

        #endregion
    }
}
