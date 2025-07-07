using System;

namespace ScriptFileCreation
{
    public static class StringHelpers
    {
        /// <summary>
        /// Creates a string formatted as a using directive. Format is: "using <paramref name="nameSpace"/>;".
        /// </summary>
        /// <param name="nameSpace">The string namespace.</param>
        /// <returns>The formatted string.</returns>
        public static string CreateUsingString(string nameSpace) => $"using {nameSpace};";
        
        /// <summary>
        /// Remove an ending from a string.
        /// </summary>
        /// <param name="target">The string to manipulate.</param>
        /// <param name="ending">The string ending to remove.</param>
        /// <returns>The string after removing ending.</returns>
        public static string WithoutEnding(string target, string ending = ".cs") => target = target.EndsWith(ending) ? target.Substring(0, target.Length - ending.Length)
            : target;
        
        public static string LineIndentation(int amount) => new String('\t', amount);

        public static string RemoveLineEnds(string target) =>
            target.Replace(Environment.NewLine, string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty);
    }
}