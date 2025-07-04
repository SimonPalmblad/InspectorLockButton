using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.IO;
using ScriptFileCreation;

namespace EditorLock
{
    public class CreateLockableObject : Editor
    {
        public static string AssetFolder = "Assets";
        public static string ScriptsFolder = "Scripts";
        public static string EditorFolder = "Scripts/Editor";
        public static string UIFolder = "Scripts/UI";
        public static string UXMLFolder = "Scripts/UI/UXML";

        public static string AssetName = "NewAsset";
        public static string DefaultName = "NewLockableObject";

        public static string ScriptName = "NewLockableScript";
        public static string UXMLName = string.Empty;
        public static string EditorName = string.Empty;

        public static string FileEnding = ".cs";

        public static void CreateScript(string directory, string fileName, ScriptBuilder content)
        {

            string path = string.Join("/", directory, fileName);

            if (System.IO.File.Exists(path))
            {
                Debug.Log("A file with that name already exists!");
                return;
            }

            if (!AssetDatabase.IsValidFolder(directory))
            {
                Debug.Log($"Created directory at: {directory}");

                CreateDirectories(directory);
            }

            #region Runtime script creation

            // create the script
            //using StreamWriter outfile = new StreamWriter(path);
            //outfile.Write(content.ToString());
            //outfile.Close();
            //AssetDatabase.Refresh();

            #endregion


            #region Test script creation
            // spoof creating the script
            //StringBuilder outBuilder = new StringBuilder(path);
            //outBuilder.AppendLine();
            //outBuilder.AppendLine("using UnityEngine;");
            //outBuilder.AppendLine("");
            //outBuilder.AppendLine(attributes);
            //outBuilder.AppendLine($"public class {fileName} : {inheritance}");
            //outBuilder.AppendLine("{");
            //outBuilder.AppendLine($"\t{content}");
            //outBuilder.AppendLine("}");

            Debug.Log($"Created script with name '{fileName}' in path '{path}'");
            Debug.Log($"Content of {content}");
            //Debug.Log("");

            #endregion

        }

        //[MenuItem("EditorLock/New Lockable Script")]

        public static void CreateLockableScript(string name, string path)
        {
            ScriptBuilder content = new ScriptBuilder(name);
            content.WithUsings(new string[] { "UnityEditor", "UnityEngine.UIElements", "EditorLock" })
                   .WithNamespace("EditorLock")
                   .WithInheritance(new string[] { "MonoBehaviour", "IEditorLockable" })
                   .AddCodeLineAttributeField("SerializeField")
                   .AddCodeLine("public bool[] m_EditorLockStates")
                   .AddCodeLine("public sting LockablePropertyPath => nameof(m_EditorLockStates)");
     

            //StringBuilder content = new StringBuilder();
            //content.Append("[SerializeField]")
            //       .AppendLine()
            //       .AppendLine("\tpublic bool[] m_EditorLockStates;")
            //       .Append("\tpublic string LockablePropertyPath => nameof(m_EditorLockStates);");


            CreateScript(directory: path, fileName: name, content);
        }

        //[MenuItem("EditorLock/New Lockable Editor Script")]
        public static void CreateLockableEditorScript(string name, string path)
        {
            ScriptBuilder content = new ScriptBuilder(name);
            
            content.WithUsings(new string[] { "UnityEditor", "UnityEngine", "UnityEngine.UIElements", "EditorLock" })
                   .WithInheritance(new string[] { "UnityEditor" })
                   .AddClassAttributeField($"CustomEditor(typeof({StringHelpers.WithoutEnding(name)}))");
  

            //var attributes = "[CustomEditor(typeof(TestObject))]";
            //var inheritance = "LockableEditor";
            CreateScript(directory: path, fileName: name, content);
        }

        //[MenuItem("EditorLock/New Lockable UXML Document")]
        public static void CreateLockableUXMLDoc(string name, string path)
        {
            string folderDirectory = path;

            if (System.IO.File.Exists(path + "/" + name))
            {
                Debug.Log($"A UXML file with the name '{name}' already exists!");
                return;
            }

            #region Test Code

            // Test spoofing
            if (!AssetDatabase.IsValidFolder(folderDirectory))
            {
                folderDirectory = CreateDirectories(folderDirectory);

            }

            Debug.Log($"Created UXML Asset in: '{folderDirectory}/{name}'");
            Debug.Log("");

            #endregion

            #region Runtime Code
            AssetDatabase.CopyAsset("Assets/Inspector Editor Lock/UI/UXML/LockableUXMLTemplate.uxml", $"{folderDirectory}/{name}.uxml");
            AssetDatabase.Refresh();

            #endregion
        }

        // Add method for attaching the script to this object
        //[MenuItem("EditorLock/New Lockable Asset")]
        public static void CreateLockableAsset()
        {
            CreateLockableAsset(DefaultName);
        }

        public static void CreateLockableAsset(string name)
        {
            var asset = ObjectFactory.CreateGameObject(name);
        }

        /// ⚠ WARNING ⚠ This doubles the root folder for every path creation
        private static string CreateDirectories(string folderPath, string parentDirectory = "Assets")
        {
            string[] folders = folderPath.Split("/");

            if (folders.Count() <= 0)
            {
                throw new Exception($"Exception: No valid folders found in string {folderPath}. Make sure to use '/' path separators in your input.");
            }

            string path = folders[0];

            for (int i = 0; i < folders.Length; i++)
            {
                var currentFolder = folders[i];      

                // Sets path if unassigned
                if (path == string.Empty)
                {
                    path = currentFolder;
                }

                if (i > 0)
                {
                    path += "/" + currentFolder;
                }

                // Continue if path is valid and add current directory to path
                if (AssetDatabase.IsValidFolder(path))
                {           
                    Debug.Log($"Found folder: {currentFolder} in path {path}");
                    continue;
                }

                AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(currentFolder));
                //path += "/" + currentFolder;
                Debug.Log($"Directory name:{Path.GetDirectoryName(currentFolder)}");
                Debug.Log($"File name:{Path.GetFileName(currentFolder)}");
                Debug.Log($"Created folder: {currentFolder} in path {path}");
            }

            return folderPath;
        }

    }

}