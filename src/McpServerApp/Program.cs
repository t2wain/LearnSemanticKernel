using AgentAIUtility.MCP;
using AIAgentExample.Example.MCP;
using Microsoft.Extensions.Hosting;

namespace McpServerApp
{
    internal class Program
    {
        /// <summary>
        /// To start your MCP server and launch the Inspector interface, run the following command:
        /// npx @modelcontextprotocol/inspector dotnet run
        /// </summary>
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder
                .Services
                .ConfigureMcpStioServer(McpTimeTool.GetAssembly());

           await builder.Build().RunAsync();
        }
    }
}
