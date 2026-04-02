using AIAgentExample.Example.MCP;
using Microsoft.AspNetCore.Builder;

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

            builder
                .Services
                .ConfigureMcpWebServer();

            
            WebApplication app = builder.Build();

            app.MapMcp();

            await app.RunAsync();
        }
    }
}
