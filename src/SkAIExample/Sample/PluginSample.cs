using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SkAIExample.Sample
{
    public static class PluginSample
    {
        #region Plugin

        const string _descLightModel = """
            { 
                "type": "object",
                "properties": {
                    "id": { "type": "integer", "description": "Light ID" },
                    "name": { "type": "string", "description": "Light name" },
                    "is_on": { "type": "boolean", "description": "Is light on" },
                    "brightness": { 
                        "type": "string", 
                        "enum": ["Low", "Medium", "High"], 
                        "description": "Brightness level" 
                    },
                    "color": { "type": "string", "description": "Hex color code" }
                },
                "required": ["id", "name"]
            } 
            """;

        public class LightModel
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("name")]
            public string? Name { get; set; }
            [JsonPropertyName("is_on")]
            public bool? IsOn { get; set; }
            [JsonPropertyName("brightness")]
            public Brightness? Brightness { get; set; }
            [JsonPropertyName("color")]
            [Description("The color of the light with a hex code (ensure you include the # symbol)")]
            public string? Color { get; set; }
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum Brightness
        {
            Low,
            Medium,
            High
        }

        /// <summary>
        /// A native plugin with functions that have 
        /// complex input/output parameters that needs
        /// attributes to describe the semantic 
        /// of these parameters for the LLM to understand
        /// </summary>
        public class LightsPlugin
        {
            private readonly List<LightModel> _lights;
            public LightsPlugin()
            {
                _lights = [
                    new() { Id = 1, Name = "Table Lamp", IsOn = false,
                        Brightness = Brightness.Medium, Color = "#FFFFFF" },
                    new() { Id = 2, Name = "Porch light", IsOn = false,
                        Brightness = Brightness.High, Color = "#FF0000" },
                    new() { Id = 3, Name = "Chandelier", IsOn = true,
                        Brightness = Brightness.Low, Color = "#FFFF00" }
                ];
            }

            [KernelFunction("get_lights")]
            [Description("Gets a list of lights and their current state")]
            public Task<List<LightModel>> GetLightsAsync()
            {
                return Task.FromResult(_lights);
            }

            [KernelFunction("change_state")]
            // include detail metadata of return type
            [Description("Changes the state of the light and returns: " + _descLightModel)] 
            public Task<LightModel?> ChangeStateAsync([Description(_descLightModel)] LightModel changeState)
            {
                // Find the light to change
                var light = _lights.FirstOrDefault(l => l.Id == changeState.Id);
                // If the light does not exist, return null
                if (light == null)
                {
                    return Task.FromResult(light);
                }

                // Update the light state
                light.IsOn = changeState.IsOn;
                light.Brightness = changeState.Brightness;
                light.Color = changeState.Color;
                return Task.FromResult(light);
            }
        }

        #endregion

        #region Examples

        /// <summary>
        /// Register plugin of native plugins 
        /// from class, object instance with Kernel
        /// </summary>
        public static void EX1(IKernelBuilder builder, int mode)
        {
            IKernelBuilderPlugins b = builder.Plugins;
            b = mode switch {
                0 => b.AddFromObject(new LightsPlugin(), "Lights"),
                1 => b.AddFromType<LightsPlugin>("Lights"),
                _ => b.AddFromFunctions("time_plugin", GetKernelFunctions())
            };
        }

        /// <summary>
        /// Create a KernelPluginCollection of native plugins 
        /// from class, object instance
        /// </summary>
        public static void EX2(Kernel kernel, int mode)
        {
            KernelPluginCollection pcol = kernel.Plugins;
            KernelPlugin p = mode switch
            {
                0 => pcol.AddFromObject(new LightsPlugin(), "Lights"),
                1 => pcol.AddFromType<LightsPlugin>("Lights"),
                _ => pcol.AddFromFunctions("time_plugin", GetKernelFunctions())
            };
        }

        /// <summary>
        /// Create native KernelFunction from lambda
        /// </summary>
        public static IEnumerable<KernelFunction> GetKernelFunctions() =>
            [
                KernelFunctionFactory.CreateFromMethod(
                    method: () => DateTime.Now,
                    functionName: "get_time",
                    description: "Get the current time"
                ),
                KernelFunctionFactory.CreateFromMethod(
                    method: (DateTime start, DateTime end) => (end - start).TotalSeconds,
                    functionName: "diff_time",
                    description: "Get the difference between two times in seconds"
                )
            ];

        /// <summary>
        /// Using DI container to register native plugins
        /// </summary>
        public static void EX3(IServiceCollection services)
        {
            // Create native plugin collection
            services.AddTransient((serviceProvider) => {
                KernelPluginCollection pluginCollection = [];
                pluginCollection.AddFromType<LightsPlugin>("Lights");
                return pluginCollection;
            });

            // Create the kernel service
            services.AddTransient((serviceProvider) => {
                KernelPluginCollection pluginCollection =
                    serviceProvider.GetRequiredService<KernelPluginCollection>();
                return new Kernel(serviceProvider, pluginCollection);
            });
        }

        /// <summary>
        /// Using DI container to register native plugins
        /// </summary>
        public static void EX4(IServiceCollection services)
        {
            // Create singletons of your plugin
            services.AddKeyedSingleton("LightPlugin", (serviceProvider, key) =>
            {
                KernelPlugin p = KernelPluginFactory.CreateFromType<LightsPlugin>();
                return p;
            });

            // Create a kernel service with singleton plugin
            services.AddTransient((serviceProvider) => {
                KernelPluginCollection pluginCollection = [
                        serviceProvider.GetRequiredKeyedService<KernelPlugin>("LightPlugin")
                    ];
                return new Kernel(serviceProvider, pluginCollection);
            });

        }

        #endregion
    }
}
