using AICommon.Plugins.FileSystem;
using AICommon.Tools;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.Reflection;

namespace AgentAIUtility.Utility
{
    public static class AIToolUtility
    {
        public static IEnumerable<AITool> CreateTools(
            object objectInstance, 
            IEnumerable<MethodInfo> methodInfos) =>
                methodInfos
                    .Select(m => AIFunctionFactory.Create(m, objectInstance))
                    .Cast<AITool>()
                    .ToList();

        public static IEnumerable<AITool> CreateTools(object objectInstance)
        {
            // Get the object's type
            Type type = objectInstance.GetType();

            // Get all methods (public, instance only, inherited included)
            IEnumerable<MethodInfo> methods = 
                type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance
                )
                .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null)
                .ToList();

            return CreateTools(objectInstance, methods);
        }

        public static IEnumerable<AITool> GetTimeTools() =>
            CreateTools(new TimeTool());

        public static IEnumerable<AITool> GetFileSystemTools(string rootDirectory) =>
            CreateTools(new FileSystemTool(rootDirectory));
    }
}
