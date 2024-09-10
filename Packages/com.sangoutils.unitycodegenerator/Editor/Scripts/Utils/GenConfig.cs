using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SangoUtils.UnityCodeGenerator.Utils
{
    internal static class GenConfig
    {
        public static string RootPath { get => Path.Combine(Application.dataPath, "Generateds"); }

        public static string GetCallerPath([CallerFilePath] string path = "")
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Caller path can't be empty or null");

            return path;
        }
    }
}
