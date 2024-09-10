using SangoUtils.UnityCodeGenerator.Utils;
using System;
using System.IO;
using UnityEditor.Animations;
using UnityEditor.Compilation;

namespace SangoUtils.UnityCodeGenerator.AnimatorGens
{
    internal static class AnimatorStateCodeGenerator
    {
        private const string FolderName = "AnimatorStates";

        internal static void Execute(AnimatorController animator, string namespaceName = "")
        {
            if (animator == null)
                throw new Exception("You need a animator to generate!");

            CreateSourceFile(animator, namespaceName);
            CompilationPipeline.RequestScriptCompilation();
        }

        private static void CreateSourceFile(AnimatorController animator, string namespaceName)
        {
            var codeWriter = new CodeWriter();

            var currentDirectory = Path.GetDirectoryName(GenConfig.GetCallerPath());
            var generatedDirectoryPath = Path.Combine(GenConfig.RootPath, FolderName);

            var typeNameSourceText = "partial class " + animator.name;

            codeWriter.AppendLine(Def.Dom_Declaration);

            if (!string.IsNullOrEmpty(namespaceName))
            {
                codeWriter.AppendLine("namespace " + namespaceName);
                codeWriter.BeginBlock();
            }

            codeWriter.AppendLine(typeNameSourceText);
            codeWriter.BeginBlock();

            if (!Directory.Exists(generatedDirectoryPath))
                Directory.CreateDirectory(generatedDirectoryPath);

            AppendPrivateField(codeWriter, animator);

            codeWriter.EndBlock();

            if (!string.IsNullOrEmpty(namespaceName))
            {
                codeWriter.EndBlock();
            }

            var sourceText = codeWriter.ToString();

            File.WriteAllText(Path.Combine(generatedDirectoryPath, animator.name + ".g.cs"), sourceText);

            codeWriter.Clear();
            codeWriter = null;
        }

        private static void AppendPrivateField(in CodeWriter codeWriter, in AnimatorController animator)
        {
            codeWriter.AppendLine("private class Parameters");
            codeWriter.BeginBlock();
            var parameters = animator.parameters;
            foreach (var parameter in parameters)
            {
                var parameterName = Validator.ValidateVariableName(parameter.name);
                string sourceText0 = "public const string " + parameterName + " = \"" + parameter.name + "\";";
                codeWriter.AppendLine(sourceText0);
            }
            codeWriter.EndBlock();

            codeWriter.AppendLine("private class States");
            codeWriter.BeginBlock();
            var machine = animator.layers[0].stateMachine;
            foreach (var info in machine.states)
            {
                var stateName = Validator.ValidateVariableName(info.state.name);
                string sourceText1 = "public const string " + stateName + " = \"" + info.state.name + "\";";
                codeWriter.AppendLine(sourceText1);
            }
            codeWriter.EndBlock();
        }
    }
}
