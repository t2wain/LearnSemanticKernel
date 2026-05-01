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

        /// <summary>
        /// Multiple skill directories.
        /// You can point the provider to a single parent directory — 
        /// each subdirectory containing a SKILL.md is automatically 
        /// discovered as a skill. Or pass a list of paths to search 
        /// multiple root directories:
        /// </summary>
        public static AgentSkillsProvider CreateSkillsProvider(
            IEnumerable<string> skillPaths,
            AgentFileSkillScriptRunner? scriptRunner = null,
            AgentFileSkillsSourceOptions? fileOptions = null,
            AgentSkillsProviderOptions? options = null,
            ILoggerFactory? loggerFactory = null)
        {
            var skillsProvider = new AgentSkillsProvider(
                skillPaths,
                scriptRunner,
                fileOptions,
                options,
                loggerFactory);
            return skillsProvider;
        }

        /// <summary>
        /// AgentSkillsProvider are context providers that make skills available to agents.
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

            // Pass SubprocessScriptRunner.RunAsync
            // to AgentSkillsProvider to enable execution
            // of file-based scripts:
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

            // Use UseFilter to include only the skills that meet your criteria —
            // for example, to load skills from a shared directory
            // but exclude experimental ones:
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

        public static AgentSkillsProvider CreateSkillsProvider(
            AgentSkillsProviderBuilder skillbuiler) =>
                skillbuiler.Build();

        #endregion

        #region Options

        /// <summary>
        /// Customizing script discovery -
        /// By default, the provider recognizes resources with
        /// extensions .md, .json, .yaml, .yml, .csv, .xml, and .txt 
        /// in references and assets subdirectories. 
        /// Use AgentFileSkillsSourceOptions to change these defaults.
        /// </summary>
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
                    // Custom system prompt
                    // By default, the skills provider injects a system prompt
                    // that lists available skills and instructs the agent
                    // to use load_skill and read_skill_resource.
                    // You can customize this prompt.
                    // 
                    // The custom template must contain {skills} (skill list), 
                    // {resource_instructions} (resource tool hint), 
                    // and {script_instructions} (script tool hint) placeholders. 
                    // Literal braces must be escaped as {{ and }}.
                    SkillsInstructionPrompt = instructionPrompt,

                    // By default, skill tools and instructions are cached
                    // after the first build. Set DisableCaching = true
                    // on AgentSkillsProviderOptions to force
                    // a rebuild on every invocation.
                    DisableCaching = disableCaching,

                    // Use AgentSkillsProviderOptions.ScriptApproval to gate
                    // all script execution behind human approval. When enabled,
                    // the agent pauses and returns an approval request
                    // instead of executing immediately:
                    ScriptApproval = scriptApproval
                };

        #endregion

        #region ScriptRunner

        public static AgentFileSkillScriptRunner ScriptRunner => RunAsync;

        /// <summary>
        /// SubprocessScriptRunner.RunAsync is roughly equivalent to the following:
        /// </summary>
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

        /// <summary>
        /// Agent Skills use a four-stage progressive disclosure pattern to minimize context usage:
        /// 1. Advertise(~100 tokens per skill) — 
        ///     Skill names and descriptions are injected into 
        ///     the system prompt at the start of each run, 
        ///     so the agent knows what skills are available.
        /// 2. Load(< 5000 tokens recommended) — 
        ///     When a task matches a skill's domain, the agent 
        ///     calls the load_skill tool to retrieve the 
        ///     full SKILL.md body with detailed instructions.
        /// 3. Read resources(as needed) — 
        ///     The agent calls the read_skill_resource tool to fetch 
        ///     supplementary files(references, templates, assets) only when required.
        /// 4. Run scripts(as needed) — 
        ///     The agent calls the run_skill_script tool to execute scripts 
        ///     bundled with a skill.
        ///     
        /// This pattern keeps the agent's context window lean while giving 
        /// it access to deep domain knowledge on demand.
        /// 
        /// load_skill is always advertised. read_skill_resource is advertised only 
        /// when at least one skill has resources. run_skill_script is advertised only 
        /// when at least one skill has scripts.
        /// </summary>
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

