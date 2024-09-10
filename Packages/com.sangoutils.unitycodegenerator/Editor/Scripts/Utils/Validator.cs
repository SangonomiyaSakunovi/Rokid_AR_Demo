using System.Text.RegularExpressions;

namespace SangoUtils.UnityCodeGenerator.Utils
{
    internal static class Validator
    {
        public static string ValidateVariableName(string name)
        {
            var varName = "";
            var matches = Regex.Matches(name, "[A-Za-z0-9_]+");

            for (var i = 0; i < matches.Count; i++)
            {
                if (i > 0)
                    varName += "_";
                varName += matches[i].Value;
            }

            return varName;
        }
    }
}
