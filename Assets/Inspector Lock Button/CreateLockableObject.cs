using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.IO;
using ScriptFileCreation;
using Editorlock;
using UnityEngine.UIElements;

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

            string path = MakePath(directory, fileName);

            if (System.IO.File.Exists(path))
            {
                Debug.LogWarning($"A file with the name {fileName} already exists in the path: {directory}. File was not created.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(directory))
            {
                CreateDirectories(directory);
            }

            // Create the script file
            using StreamWriter outfile = new StreamWriter(path);
            outfile.Write(content.ToString());
            outfile.Close();
            AssetDatabase.Refresh();

            Debug.Log($"Created script with name '{fileName}' in path '{path}'");
            Debug.Log($"Click this message to see script content: \n {content}");
        }

        public static string CreateLockableScript(string name, string path)
        {
            ScriptBuilder content = new ScriptBuilder(name);
            content.WithUsings(new string[] { "UnityEngine", "UnityEditor", "UnityEngine.UIElements", "EditorLock" })
                   .WithInheritance(new string[] { "MonoBehaviour", "IEditorLockable" })
                   .AddCodeLine("// Implementation of IEditorLockable interface")
                   .AddCodeLineAttributeField("SerializeField")
                   .AddCodeLineAttributeField("HideInInspector")
                   .AddCodeLine("private bool[] m_EditorLockStates")
                   .AddCodeLine("public string LockablePropertyPath => nameof(m_EditorLockStates)");
    
            CreateScript(directory: path, fileName: name, content);
            
            return AssetDatabase.AssetPathToGUID(MakePath(path, name));            
        }

        public static void CreateLockableEditorScript(string name, string scriptName, string path, string uxmlDocPath)
        {
            ScriptBuilder content = new ScriptBuilder(name);
            //var fullUXML

            content.WithUsings(new string[] { "UnityEditor", "UnityEngine", "UnityEngine.UIElements", "EditorLock" })
                   .WithInheritance(new string[] { $"LockableEditor<{StringHelpers.WithoutEnding(scriptName)}>" })
                   .AddClassAttributeField($"CustomEditor(typeof({StringHelpers.WithoutEnding(scriptName)}))")
                   .AddCodeLine($"protected override VisualTreeAsset VisualTreePath => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(\"{uxmlDocPath}\");");

            CreateScript(directory: path, fileName: name, content);
        }

        public static void CreateLockableUXMLDoc(string name, string path)
        {
            if (System.IO.File.Exists(path + "/" + name))
            {
                Debug.LogWarning($"A UXML file with the name '{name}' in path already exists. No file was created.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(path))
            {
                path = CreateDirectories(path);

            }

            Debug.Log($"Created UXML Asset in: '{path}/{name}'");
            Debug.Log("");
    
            AssetDatabase.CopyAsset(InternalAssetReferences.Instance.uxmlLockableObjectTemplatePath, $"{path}/{name}");
            AssetDatabase.Refresh();
        }

        // Add method for attaching the script to this object
        //[MenuItem("EditorLock/New Lockable Asset")]
        public static void CreateLockableAsset()
        {
            CreateLockableAsset(DefaultName);
        }

        public static GameObject CreateLockableAsset(string name)
        {
            return ObjectFactory.CreateGameObject(name);
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
            }

            return folderPath;
        }

        private static string MakePath(params string[] values) => string.Join("/", values);

    }

}