using Microsoft.Extensions.DependencyInjection;

namespace AIAgentExample.Example.MCP
{
    public static class McpServerConfiguration
    {
        public static IServiceCollection ConfigureMcpStioServer(this IServiceCollection services)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithStdioServerTransport()
             .WithResourcesFromAssembly()
             .WithPromptsFromAssembly()
             .WithToolsFromAssembly();

            return services;
        }

        public static IServiceCollection ConfigureMcpWebServer(this IServiceCollection services)
        {
            IMcpServerBuilder mb = services
             .AddMcpServer()
             .WithHttpTransport()
             .WithResourcesFromAssembly()
             .WithPromptsFromAssembly()
             .WithToolsFromAssembly();

            return services;
        }
    }
}
