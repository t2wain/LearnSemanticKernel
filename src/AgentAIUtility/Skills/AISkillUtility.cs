using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

#pragma warning disable MAAI001
namespace AgentAIUtility.Skills
{
    public static class AISkillUtility
    {
        #region AgentSkillsProvider

        public static AgentSkillsProvider CreateSkillsProvider(
            string skillPath,
            AgentFileSkillScriptRunner? scriptRunner = null,
            AgentFileSkillsSourceOptions? fileOptions = null,
            AgentSkillsProviderOptions? options = null,
            ILoggerFactory? loggerFactory = null)
        {
            var skillsProvider = new AgentSkillsProvider(
                skillPath,
                scriptRunner,
                fileOptions,
                options,
                loggerFactory);
            return skillsProvider;
        }

        /// <summary>
        /// FileAgentSkillsProvider is AIContextProvider
        /// </summary>
        public static AIContextProvider CreateSkillsProvider(AgentSkillsProvider skillProvider) =>
            skillProvider;

        #endregion

        #region AgentSkillsProviderBuilder

        public static AgentSkillsProviderBuilder BuildSkillProviderBuilder(
            AgentSkillsProviderBuilder builder,
            string skillPath,
            AgentFileSkillScriptRunner? scriptRunner = null,
            AgentSkillsProviderOptions? options = null,
            ILoggerFactory? loggerFactory = null,
            Func<AgentSkill, bool>? skillFilter = null,
            string? promptTemplate = null,
            IEnumerable<AgentSkill>? agentSkills = null,
            AgentSkillsSource? skillSource = null)
        {

            var b = builder.UseFileSkill(skillPath);
            if (scriptRunner != null)
                b = b.UseFileScriptRunner(scriptRunner);
            if (loggerFactory != null)
                b = b.UseLoggerFactory(loggerFactory);
            if (options != null)
                b = b.UseOptions(o =>
                {
                    o.ScriptApproval = options.ScriptApproval;
                    o.DisableCaching = options.DisableCaching;
                    o.SkillsInstructionPrompt = options.SkillsInstructionPrompt;
                });
            if (skillFilter != null)
                b = b.UseFilter(skillFilter);
            if (promptTemplate != null)
                b = b.UsePromptTemplate(promptTemplate);
            if (agentSkills != null)
                b = b.UseSkills(agentSkills);
            if (skillSource != null)
                b = b.UseSource(skillSource);

            return b;
        }

        #endregion

        #region Options

        public static AgentFileSkillsSourceOptions CreateSkillSourceOptions(
            IEnumerable<string>? resourceExts = null,
            IEnumerable<string>? scriptExts = null,
            IEnumerable<string>? resourceDirs = null,
            IEnumerable<string>? scriptsDirs = null) =>
                new AgentFileSkillsSourceOptions()
                {
                    AllowedResourceExtensions = resourceExts,
                    AllowedScriptExtensions = scriptExts,
                    ResourceDirectories = resourceDirs,
                    ScriptDirectories = scriptsDirs,
                };

        public static AgentSkillsProviderOptions CreateSkillProviderOptions(
            string instructionPrompt,
            bool disableCaching,
            bool scriptApproval) =>
                new AgentSkillsProviderOptions()
                {
                    SkillsInstructionPrompt = instructionPrompt,
                    DisableCaching = disableCaching,
                    ScriptApproval = scriptApproval
                };

        public static AgentFileSkillScriptRunner ScriptRunner => RunAsync;

        public static async Task<object?> RunAsync(
            AgentFileSkill skill,
            AgentFileSkillScript script,
            AIFunctionArguments args,
            CancellationToken cancellation)
        {
            var ext = Path.GetExtension(script.FullPath);
            var appName = ext switch
            {
                ".cs" => "",
                ".csx" => "",
                ".ps1" => "powershell",
                ".js" => "node",
                _ => "python3",
            };

            var psi = new ProcessStartInfo(appName)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            psi.ArgumentList.Add(Path.Combine(skill.Path, script.FullPath));
            if (args != null)
            {
                foreach (var (key, value) in args)
                {
                    if (value is not null)
                    {
                        psi.ArgumentList.Add($"--{key}");
                        psi.ArgumentList.Add(value.ToString()!);
                    }
                }
            }
            using var process = Process.Start(psi)!;
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output.Trim();
        }


        #endregion

        #region AgentInlineSkill

        public static AgentSkillFrontmatter CreateFrontmatter(
            string name,
            string description,
            string? allowedTools,
            string? compatibility,
            string? license,
            AdditionalPropertiesDictionary? metaData
        )
        {
            AgentSkillFrontmatter frontMatter = new(name, description)
            {
                AllowedTools = allowedTools,
                Compatibility = compatibility,
                License = license,
                Metadata = metaData
            };
            return frontMatter;
        }

        public static AgentInlineSkill CreateInlineSkill (
            AgentSkillFrontmatter frontmatter,
            string instruction,
            JsonSerializerOptions? serializerOptions) =>
                new AgentInlineSkill(frontmatter, instruction, serializerOptions);

        /// <summary>
        /// Registers a dynamic resource with this skill, backed by a C# delegate.
        /// The delegate's parameters and return type are automatically 
        /// marshaled via AIFunctionFactory.
        /// </summary>
        public static AgentInlineSkill AddResourceToSkill(
            AgentInlineSkill skill,
            string name,
            Delegate method,
            string description,
            JsonSerializerOptions? serializerOptions) =>
                skill.AddResource(name, method, description, serializerOptions);
        
        public static AgentInlineSkill AddResourceToSkill(
            AgentInlineSkill skill,
            string name,
            object value,
            string? description = null) =>
                skill.AddResource(name, value, description);

        /// <summary>
        /// Registers a script with this skill, backed by a C# delegate. 
        /// The delegate's parameters and return type are automatically 
        /// marshaled via AIFunctionFactory.
        /// </summary>
        public static AgentInlineSkill AddScriptToSkill(
            AgentInlineSkill skill,
            string name,
            Delegate method,
            string description,
            JsonSerializerOptions? serializerOptions) =>
                skill.AddScript(name, method, description, serializerOptions);

        #endregion

        #region Execute

        public static Task<object?> RunSkill(
            AgentSkillScript script, 
            AgentSkill skill, 
            AIFunctionArguments args) =>
                script.RunAsync(skill, args);

        public static Task<object?> ReadResouse(
            AgentSkillResource resource,
            IServiceProvider? provider) =>
                resource.ReadAsync(provider);

        #endregion

        #region Explore

        public static void Explore(AgentFileSkill fileSkill)
        {
            string path = fileSkill.Path;
            AgentSkill a = fileSkill;
            Explore(a);
        }

        public static void Explore(AgentSkill skill)
        {
            var c = skill.Content;

            var f = skill.Frontmatter;
            Explore(f);

            if (skill.Scripts is IEnumerable<AgentSkillScript> scripts 
                && scripts.Count() > 0)
            {
                foreach (AgentSkillScript script in scripts)
                {
                    if (script is AgentFileSkillScript fileScript)
                        Explore(fileScript);
                    else Explore(script);
                }
            }

            if (skill.Resources is IEnumerable<AgentSkillResource> resources
                && resources.Count() > 0)
            {
                foreach (AgentSkillResource resource in resources)
                {
                    Explore(resource);
                }
            }
        }

        public static void Explore(AgentSkillFrontmatter frontmatter)
        {
            string? a = frontmatter.AllowedTools;
            string? c = frontmatter.Compatibility;
            string d = frontmatter.Description;
            string? l = frontmatter.License;
            AdditionalPropertiesDictionary? m = frontmatter.Metadata;
            string n = frontmatter.Name;
        }

        public static void Explore(AgentFileSkillScript script)
        {
            string p = script.FullPath;
            AgentSkillScript s = script;
            Explore(s);
        }

        public static void Explore(AgentSkillScript script)
        {
            string? d = script.Description;
            string n = script.Name;
            JsonElement? p = script.ParametersSchema;
        }

        public static void Explore(AgentSkillResource resource)
        {
            string? d = resource.Description;
            string n = resource.Name;
        }

        #endregion
    }
}
#pragma warning restore MAAI001

