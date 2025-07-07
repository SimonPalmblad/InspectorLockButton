using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorLockUtilies
{
    public class USSFileData 
    {
        public string FilePath;
        public string FileContent;
        public bool IsValid;

        public USSFileData(string filePath, string fileContent, bool isValid)
        {
            this.FilePath = filePath;
            this.FileContent = fileContent;
            this.IsValid = isValid;
        }

        public static USSFileData InvalidData => new USSFileData(string.Empty, string.Empty, false);
    }

    public static class USSFileParser
    {

        public static USSFileData EditValueInUSSClass(string filePath, string className, string valueName, string newValue)
        {
            var file = FileAsString(filePath);
            if (file == string.Empty)
            {
                Debug.Log($"ERROR: FILE NOT FOUND IN PATH: {filePath}");
                return USSFileData.InvalidData;
            }

            var (startIndex, endIndex) = IndexOfClass(file, className);

            if (endIndex == 0)
            {
                Debug.Log($"ERROR: CLASS WITH NAME {filePath} NOT FOUND IN FILE.");
                return USSFileData.InvalidData;
            }

            // Potential error here
            var classString = file.Substring(startIndex, endIndex - startIndex);

            if (!file.Substring(startIndex, endIndex - startIndex).Contains(valueName, StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.Log($"ERROR: VALUE {valueName} NOT FOUND IN {className}.");
                return USSFileData.InvalidData;
            }

            // Find the specified value inside the class
            var relativeValueIndex = classString.IndexOf(valueName, StringComparison.CurrentCultureIgnoreCase);
            var relativeStartEdit = classString[relativeValueIndex..].IndexOf(":", StringComparison.CurrentCultureIgnoreCase);
            var relativeEndEdit = classString[relativeValueIndex..].IndexOf(";", StringComparison.CurrentCultureIgnoreCase);

            var startOffset = (relativeValueIndex + relativeStartEdit); // +2 to keep ': ' at the start of string
            var endOffset = relativeValueIndex + relativeEndEdit;

            var newFileContent = file.Remove(startIndex + startOffset, endOffset - (startOffset));
            newFileContent = newFileContent.Insert(startIndex + startOffset, $": {newValue}");

            return new USSFileData(filePath, newFileContent, true);
        }

        public static USSFileData EditClassValue(this USSFileData data, string className, string valueName, string newValue)
        {
            if (!data.IsValid)
            {
                Debug.LogWarning("USSFileData not valid. No modifications where made");
                return data;
            }

            // duplicate code from EditValueInUSSClass - here to method end.
            var (startIndex, endIndex) = IndexOfClass(data.FileContent, className);

            if (endIndex == 0)
            {
                Debug.Log($"ERROR: CLASS WITH NAME {data.FilePath} NOT FOUND IN FILE.");
                return USSFileData.InvalidData;
            }

            // Potential error here
            var classString = data.FileContent.Substring(startIndex, endIndex - startIndex);

            if (!data.FileContent.Substring(startIndex, endIndex - startIndex).Contains(valueName, StringComparison.CurrentCultureIgnoreCase))
            {
                Debug.Log($"ERROR: VALUE {valueName} NOT FOUND IN {className}.");
                return USSFileData.InvalidData;
            }

            // Find the specified value inside the class
            var (startOffset, endOffset) = RangeOfClassInUSS(valueName, newValue, startIndex, classString);
                        
            var newDataContent = ReplaceValueInUSSClass(data, newValue, startIndex, startOffset, endOffset);
            return newDataContent;
        }

        public static USSFileData EditClassPixelValue(this USSFileData data, string className, string valueName, int newPixelValue)
        {
            return EditClassValue(data, className, valueName, $"{newPixelValue.ToString()}px");            
        }

        private static USSFileData ReplaceValueInUSSClass(USSFileData data, string newValue, int startIndex, int startOffset, int endOffset)
        {
            var newFileContent = data.FileContent.Remove(startIndex + startOffset, endOffset - (startOffset));
            newFileContent = newFileContent.Insert(startIndex + startOffset, $": {newValue}");
            
            data.FileContent = newFileContent;
            return data;
        }

        private static (int StartIndex, int EndIndex) RangeOfClassInUSS(string valueName, string newValue, int startIndex, string classString)
        {
            var relativeValueIndex = classString.IndexOf(valueName, StringComparison.CurrentCultureIgnoreCase);
            var relativeStartEdit = classString[relativeValueIndex..].IndexOf(":", StringComparison.CurrentCultureIgnoreCase);
            var relativeEndEdit = classString[relativeValueIndex..].IndexOf(";", StringComparison.CurrentCultureIgnoreCase);

            var startOffset = (relativeValueIndex + relativeStartEdit); // +2 to keep ': ' at the start of string
            var endOffset = relativeValueIndex + relativeEndEdit;

            return (startOffset, endOffset);

            //var newFileContent = data.FileContent.Remove(startIndex + startOffset, endOffset - (startOffset));
            //newFileContent = newFileContent.Insert(startIndex + startOffset, $": {newValue}");
            
            //return newFileContent;
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
                Debug.LogWarning($"FilePath {filePath} is not valid. Could not write to file.");
                return;
            }

            File.WriteAllText(filePath, fileContent);
            Debug.Log($"SUCCESS! Wrote to file in path: {filePath}");
            AssetDatabase.Refresh();
        }

        public static void WriteToFile(USSFileData data)
        {
            if (!data.IsValid)
            {
                Debug.LogWarning("USSFileData not valid. Could not write to file.");
                return;
            }

            WriteToFile(data.FilePath, data.FileContent);
        }

        public static string ColorToUSS(Color color)
        {
            Color32 color32 = color;

            StringBuilder result = new StringBuilder();
            
            result.Append($"rgba(")
                  .Append($"{color32.r}, ")
                  .Append($"{color32.g}, ")
                  .Append($"{color32.b}, ")
                  .Append(color.a.ToString("0.00"))
                  .Append(")");

            return result.ToString();            
        }
    }
}
