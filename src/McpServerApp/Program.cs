using AgentAIUtility.MCP;
using AICommon.Tools;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;

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

            IEnumerable<McpServerTool> tools = McpUtility.CreateTools(new TimeTool());

            builder
                .Services
                .ConfigureMcpStioServer(tools);

           await builder.Build().RunAsync();
        }
    }
}
