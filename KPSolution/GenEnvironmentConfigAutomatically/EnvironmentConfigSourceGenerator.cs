using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tomlyn;

namespace GenSolutionConfigAutomatically
{
    [Generator]
    public class EnvironmentConfigSourceGenerator : ISourceGenerator
    {
        const string tomlFile = "environmentconfig.toml";

        public void Execute(GeneratorExecutionContext context)
        {
            // Debugger.Launch();
            // Debugger.Break();
            var model = EnvironmentConfigHelper.ReadConfig();

            string source = """
                            partial class EnvironmentConfig
                            {{
                                public const string Root = @"{0}";
                                public const string Client = @"{1}";
                                public const string Solution = @"{2}";
                                public const string UnityEditorDlls = @"{3}";
                            }}
                            """;

            source = string.Format(source,model.Root ,model.Client, model.Solution, model.UnityEditorDlls);
            context.AddSource("EnvironmentConfig.generate.cs", source);

            // #region 注入注释
            //
            // var a = context.Compilation.SyntaxTrees.First(v =>
            //     v.FilePath ==
            //     @"D:\Repository\RainTown2SurvivalGame\RainTown2SurvivalGameSolution\EnvironmentConfig\EnvironmentConfig.cs");
            // foreach (var propertyDeclarationSyntax in a.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
            // {
            //     if (propertyDeclarationSyntax.Identifier.ValueText == "Root")
            //     {
            //         
            //     }
            // }
            // return;
            //
            // #endregion
        }
        
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}