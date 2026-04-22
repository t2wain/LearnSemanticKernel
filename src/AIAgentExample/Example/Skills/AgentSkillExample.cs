using Microsoft.Agents.AI;
using System.Text.Json;

#pragma warning disable MAAI001
namespace AIAgentExample.Example.Skills
{
    public class AgentSkillExample
    {
        public AgentInlineSkill CreateCodeStyleSkill()
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

        public AgentInlineSkill CreateProjectInfoSkill()
        {
            var projectInfoSkill = new AgentInlineSkill(
                name: "project-info",
                description: "Project status and configuration information",
                instructions: """
                    Use this skill for questions about the current project.
                    1. Read the environment resource for deployment configuration details.
                    2. Read the team-roster resource for information about team members.
                    """)
                // Dynamic resources
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

        public AgentInlineSkill CreateUnitConverterSkill()
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
                // Code-defined scripts
                .AddScript("convert", (double value, double factor) =>
                {
                    double result = Math.Round(value * factor, 4);
                    return JsonSerializer.Serialize(new { value, factor, result });
                });

            return unitConverterSkill;
        }

        public AgentSkillsProvider CreateAgentProvider()
        {
            AgentSkill codeStyleSkill = CreateCodeStyleSkill();
            AgentSkill projectInfoSkill = CreateProjectInfoSkill();
            AgentSkill unitConverterSkill = CreateUnitConverterSkill();
            var skillsProvider = new AgentSkillsProvider(
                [codeStyleSkill, projectInfoSkill, unitConverterSkill]);
            return skillsProvider;
        }

    }
}
#pragma warning restore MAAI001
