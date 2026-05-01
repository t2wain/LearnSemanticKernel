using Microsoft.Agents.AI;
using System.ComponentModel;
using System.Text.Json;

#pragma warning disable MAAI001
namespace AIAgentExample.Example.Skills
{
    /// <summary>
    /// Class-based skills let you bundle all skill components — 
    /// name, description, instructions, resources, and scripts — 
    /// into a single C# class. Derive from AgentClassSkill<T> 
    /// (where T is your class), then annotate properties with 
    /// [AgentSkillResource] and methods with [AgentSkillScript] 
    /// for automatic discovery:
    /// 
    /// Class-based skills can also resolve dependencies 
    /// through their constructor. Register the skill class 
    /// in the ServiceCollection and resolve it from the 
    /// container instead of calling new directly:
    /// </summary>
    public class UnitConverterSkillSample : AgentClassSkill<UnitConverterSkillSample>
    {
        public override AgentSkillFrontmatter Frontmatter { get; } = new(
            "unit-converter",
            "Convert between common units using a multiplication factor. Use when asked to convert miles, kilometers, pounds, or kilograms.");

        protected override string Instructions => """
        Use this skill when the user asks to convert between units.

        1. Review the conversion-table resource to find the correct factor.
        2. Use the convert script, passing the value and factor from the table.
        3. Present the result clearly with both units.
        """;

        /// <summary>
        /// When the [AgentSkillResource] attribute is applied 
        /// to a property or method, its return value is used 
        /// as the resource content when the agent reads the resource — 
        /// use a method when the content needs to be computed at read time.
        /// </summary>
        [AgentSkillResource("conversion-table")]
        [Description("Lookup table of multiplication factors for common unit conversions.")]
        public string ConversionTable => """
        # Conversion Tables
        Formula: **result = value × factor**
        | From       | To         | Factor   |
        |------------|------------|----------|
        | miles      | kilometers | 1.60934  |
        | kilometers | miles      | 0.621371 |
        | pounds     | kilograms  | 0.453592 |
        | kilograms  | pounds     | 2.20462  |
        """;

        /// <summary>
        /// When [AgentSkillScript] is applied to a method, 
        /// the method is invoked when the agent calls the script. 
        /// Use [Description] from System.ComponentModel to describe 
        /// each resource and script for the agent.
        /// </summary>
        [AgentSkillScript("convert")]
        [Description("Multiplies a value by a conversion factor and returns the result as JSON.")]
        public static string ConvertUnits(double value, double factor)
        {
            double result = Math.Round(value * factor, 4);
            return JsonSerializer.Serialize(new { value, factor, result });
        }
    }
}
#pragma warning restore MAAI001
