using SangoUtils.UnityCodeGenerator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Compilation;
using UnityEngine;

namespace SangoUtils.UnityCodeGenerator.TagGens
{
    internal static class TagCodeGenerator
    {
        private const string TemplatePath = "../../../Template~/Tags.template.cs";
        private const string FolderName = "Tags";

        internal static void Execute()
        {
            var tags = UnityEditorInternal.InternalEditorUtility.tags;
            var layers = GetLayers();

            CreateSourceFile(tags, layers);
            CompilationPipeline.RequestScriptCompilation();
        }

        private static void CreateSourceFile(IEnumerable<string> tags, IReadOnlyDictionary<int, string> layers)
        {
            var currentDirectory = Path.GetDirectoryName(GenConfig.GetCallerPath());

            if (string.IsNullOrEmpty(currentDirectory))
                throw new Exception("Can't get parent directory of caller path");

            var generatedDirectoryPath = Path.Combine(GenConfig.RootPath, FolderName);
            var templateText = File.ReadAllText(Path.Combine(currentDirectory, TemplatePath));

            var compiledTags = new StringBuilder();
            var compiledLayers = new StringBuilder();

            if (!Directory.Exists(generatedDirectoryPath))
                Directory.CreateDirectory(generatedDirectoryPath);

            foreach (var tag in tags)
            {
                var tagName = Validator.ValidateVariableName(tag);

                var sourceText0 =
$@"
    public const string {tagName} = ""{tag}"";
";
                compiledTags.Append(sourceText0);
            }

            foreach (var pair in layers)
            {
                var layerName = Validator.ValidateVariableName(pair.Value);

                var sourceText1 =
$@"
    public const string {layerName} = ""{pair.Key}"";
";

                compiledLayers.Append(sourceText1);
            }

            var compiledText = string.Format(templateText, compiledTags.ToString(), compiledLayers.ToString());
            compiledTags.Clear();
            compiledLayers.Clear();
            File.WriteAllText(Path.Combine(generatedDirectoryPath, FolderName + ".g.cs"), compiledText);
        }



        private static IReadOnlyDictionary<int, string> GetLayers()
        {
            const int layersCount = 32;
            var layers = new Dictionary<int, string>();
            var equalKeys = new List<int>();

            for (var i = 0; i < layersCount; i++)
            {
                var name = LayerMask.LayerToName(i);

                if (!string.IsNullOrEmpty(name))
                    layers.Add(i, name);
            }

            foreach (var pair in layers)
            {
                var equalFound = false;

                if (equalKeys.Contains(pair.Key))
                    continue;

                foreach (var innerPair in layers)
                {
                    if (pair.Key == innerPair.Key)
                        continue;

                    if (pair.Value != innerPair.Value)
                        continue;

                    if (!equalFound)
                    {
                        equalKeys.Add(pair.Key);
                        equalFound = true;
                    }

                    equalKeys.Add(innerPair.Key);
                }
            }

            foreach (var key in equalKeys)
                layers[key] += $"_{key}";

            return layers;
        }
    }
}