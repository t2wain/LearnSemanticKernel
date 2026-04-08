using AgentAIUtility.MCP;
using AICommon.Tools;
using Microsoft.AspNetCore.Builder;
using ModelContextProtocol.Server;

namespace McpWebServerApp
{
    internal class Program
    {
        /// <summary>
        /// To start your MCP server and launch the Inspector interface, run the following command:
        /// npx @modelcontextprotocol/inspector dotnet run
        /// </summary>
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IEnumerable<McpServerTool> tools = McpUtility.CreateTools(new TimeTool());

            builder
                .Services
                .ConfigureMcpWebServer(tools);

            
            WebApplication app = builder.Build();

            app.MapMcp();

            await app.RunAsync();
        }
    }
}
