using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorLockUtilies
{
    public static class USSFileEditorHelper
    {
        public static string EditValueInUSSClass(string filePath, string className, string valueName, string newValue)
        {
            var file = FileAsString(filePath);
            if (file == string.Empty)
            {
                Console.WriteLine($"ERROR: FILE NOT FOUND IN PATH: {filePath}");
                return string.Empty;
            }

            var (startIndex, endIndex) = IndexOfClass(file, className);

            if (endIndex == 0)
            {
                Console.WriteLine($"ERROR: CLASS WITH NAME {filePath} NOT FOUND IN FILE.");
                return string.Empty;
            }

            // Potential error here
            var classString = file.Substring(startIndex, endIndex - startIndex);

            if (!file.Substring(startIndex, endIndex - startIndex).Contains(valueName, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine($"ERROR: VALUE {valueName} NOT FOUND IN {className}.");
                return string.Empty;
            }

            // Find the specified value inside the class
            var relativeValueIndex = classString.IndexOf(valueName, StringComparison.CurrentCultureIgnoreCase);
            var relativeStartEdit = classString[relativeValueIndex..].IndexOf(":", StringComparison.CurrentCultureIgnoreCase);
            var relativeEndEdit = classString[relativeValueIndex..].IndexOf(";", StringComparison.CurrentCultureIgnoreCase);

            var startOffset = (relativeValueIndex + relativeStartEdit); // +2 to keep ': ' at the start of string
            var endOffset = relativeValueIndex + relativeEndEdit;

            var newFileContent = file.Remove(startIndex + startOffset, endOffset - (startOffset));
            newFileContent = newFileContent.Insert(startIndex + startOffset, $": {newValue}");

            return newFileContent;
        }

        private static string FileAsString(string filePath)
        {
            if (!File.Exists(filePath))
            {

                return (string.Empty);
            }

            return File.ReadAllText(filePath);
        }

        private static (int StartIndex, int EndIndex) IndexOfClass(string file, string className)
        {
            if (!file.Contains(className, StringComparison.CurrentCultureIgnoreCase))
            {
                return (0, 0);
            }

            var startIndex = file.IndexOf(className, StringComparison.CurrentCultureIgnoreCase);
            var endIndex = startIndex + file[startIndex..].IndexOf("}", StringComparison.CurrentCultureIgnoreCase);

            return (startIndex, endIndex);
        }

        public static void WriteToFile(string filePath, string fileContent)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            File.WriteAllText(filePath, fileContent);
            Console.WriteLine($"SUCCESS! Wrote to file in path: {filePath}");
        }
    }
}
