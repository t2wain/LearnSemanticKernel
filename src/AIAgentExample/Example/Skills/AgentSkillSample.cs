using Microsoft.Agents.AI;
using System.Text.Json;

#pragma warning disable MAAI001
namespace AIAgentExample.Example.Skills
{
    public class AgentSkillSample
    {
        #region Skills

        /// <summary>
        /// Basic code skill.
        /// Create an AgentInlineSkill with a name, description, 
        /// and instructions.Attach resources using .AddResource():
        /// </summary>
        public static AgentInlineSkill CreateCodeStyleSkill()
        {
            var codeStyleSkill = new AgentInlineSkill(
                name: "code-style",
                description: "Coding style guidelines and conventions for the team",
                instructions: """
                    Use this skill when answering questions about coding style, conventions, or best practices for the team.
                    1. Read the style-guide resource for the full set of rules.
                    2. Answer based on those rules, quoting the relevant guideline where helpful.
                    """)
                .AddResource(
                    "style-guide",
                    """
                    # Team Coding Style Guide
                    - Use 4-space indentation (no tabs)
                    - Maximum line length: 120 characters
                    - Use type annotations on all public methods
                    """);

            return codeStyleSkill;
        }

        public static AgentInlineSkill CreateProjectInfoSkill()
        {
            var projectInfoSkill = new AgentInlineSkill(
                name: "project-info",
                description: "Project status and configuration information",
                instructions: """
                    Use this skill for questions about the current project.
                    1. Read the environment resource for deployment configuration details.
                    2. Read the team-roster resource for information about team members.
                    """)
                // Dynamic Resources
                // Pass a factory delegate to.AddResource() to compute
                // the content at runtime. The delegate is invoked each
                // time the agent reads the resource:
                .AddResource("environment", () =>
                {
                    string env = Environment.GetEnvironmentVariable("APP_ENV") ?? "development";
                    string region = Environment.GetEnvironmentVariable("APP_REGION") ?? "us-east-1";
                    return $"Environment: {env}, Region: {region}";
                })
                .AddResource(
                    "team-roster",
                    "Alice Chen (Tech Lead), Bob Smith (Backend Engineer)");

            return projectInfoSkill;
        }

        public static AgentInlineSkill CreateUnitConverterSkill()
        {
            var unitConverterSkill = new AgentInlineSkill(
                name: "unit-converter",
                description: "Convert between common units using a conversion factor",
                instructions: """
                    Use this skill when the user asks to convert between units.
                    1. Review the conversion-table resource to find the correct factor.
                    2. Use the convert script, passing the value and factor from the table.
                    3. Present the result clearly with both units.
                    """)
                .AddResource(
                    "conversion-table",
                    """
                    # Conversion Tables
                    Formula: **result = value × factor**
                    | From       | To         | Factor   |
                    |------------|------------|----------|
                    | miles      | kilometers | 1.60934  |
                    | kilometers | miles      | 0.621371 |
                    | pounds     | kilograms  | 0.453592 |
                    | kilograms  | pounds     | 2.20462  |
                    """)
                // Use .AddScript() to register a delegate as an executable script. 
                // Code-defined scripts run in-process as direct delegate calls. 
                // No script runner is needed. The delegate's typed parameters 
                // are automatically converted into a JSON Schema that the agent 
                // uses to pass arguments
                .AddScript("convert", (double value, double factor) =>
                {
                    double result = Math.Round(value * factor, 4);
                    return JsonSerializer.Serialize(new { value, factor, result });
                });

            return unitConverterSkill;
        }

        /// <summary>
        /// Code-defined skills with DI
        /// Declare IServiceProvider as a parameter in AddResource or AddScript delegates — 
        /// the framework resolves and injects it automatically when the agent 
        /// reads a resource or runs a script:
        /// </summary>
        public static AgentInlineSkill CreateDistanceConverterSkill()
        {
            var distanceSkill = new AgentInlineSkill(
                name: "distance-converter",
                description: "Convert between distance units (miles and kilometers).",
                instructions: """
                    Use this skill when the user asks to convert between miles and kilometers.
                    1. Read the distance-table resource for conversion factors.
                    2. Use the convert script to compute the result.
                    """)
                // Declare IServiceProvider as a parameter 
                .AddResource("distance-table", (IServiceProvider sp) =>
                {
                    throw new NotImplementedException();
                    //return sp.GetRequiredService<ConversionService>().GetDistanceTable();
                })
                // Declare IServiceProvider as a parameter 
                .AddScript("convert", (double value, double factor, IServiceProvider sp) =>
                {
                    throw new NotImplementedException();
                    //return sp.GetRequiredService<ConversionService>().Convert(value, factor);
                });
            return distanceSkill;
        }

        #endregion

        #region Provider

        public static AgentSkillsProvider CreateAgentProvider()
        {
            AgentSkill codeStyleSkill = CreateCodeStyleSkill();
            AgentSkill projectInfoSkill = CreateProjectInfoSkill();
            AgentSkill unitConverterSkill = CreateUnitConverterSkill();
            var skillsProvider = new AgentSkillsProvider(
                [codeStyleSkill, projectInfoSkill, unitConverterSkill]);
            return skillsProvider;
        }

        /// <summary>
        /// Customizing script discovery -
        /// By default, the provider recognizes resources with
        /// extensions .md, .json, .yaml, .yml, .csv, .xml, and .txt 
        /// in references and assets subdirectories. 
        /// Use AgentFileSkillsSourceOptions to change these defaults.
        /// </summary>
        public static AgentFileSkillsSourceOptions CreateFileOptions() =>
            new AgentFileSkillsSourceOptions
            {
                AllowedScriptExtensions = [".py"],
                ScriptDirectories = ["scripts", "tools"],
            };

        /// <summary>
        /// Custom system prompt
        /// By default, the skills provider injects a system prompt
        /// that lists available skills and instructs the agent
        /// to use load_skill and read_skill_resource.
        /// You can customize this prompt.
        /// 
        /// The custom template must contain {skills} (skill list), 
        /// {resource_instructions} (resource tool hint), 
        /// and {script_instructions} (script tool hint) placeholders. 
        /// Literal braces must be escaped as {{ and }}.
        /// </summary>
        public static AgentSkillsProviderOptions CreateProviderOption() =>
            new AgentSkillsProviderOptions
            {
                SkillsInstructionPrompt = """
                        You have skills available. Here they are:
                        {skills}
                        {resource_instructions}
                        {script_instructions}
                        """
            };

        #endregion
    }
}
#pragma warning restore MAAI001
