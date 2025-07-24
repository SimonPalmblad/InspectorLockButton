using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.IO;
using ScriptFileCreation;
using UnityEngine.UIElements;
using InspectorLock.Internals;

namespace InspectorLock
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
        }

        /// <summary>
        /// Creates a .cs file used for a Lockable Object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateLockableScript(string name, string path)
        {
            ScriptBuilder content = new ScriptBuilder(name);
            content.WithUsings(new string[] { "UnityEngine", "UnityEditor", "UnityEngine.UIElements", nameof(InspectorLock) })
                   .WithInheritance(new string[] { "MonoBehaviour", nameof(ILockableInspector)})
                   .AddCodeLine("// Implementation of InspectorLockable interface")
                   .AddCodeLineAttributeField("SerializeField")
                   .AddCodeLineAttributeField("HideInInspector")
                   .AddCodeLine("private bool[] m_InspectorLockStates")
                   .AddCodeLine("public string LockablePropertyPath => nameof(m_InspectorLockStates)");
    
            CreateScript(directory: path, fileName: name, content);
            
            return AssetDatabase.AssetPathToGUID(MakePath(path, name));            
        }

        /// <summary>
        /// Creates an Editor .cs file used for customizing the inspector of a Lockable Object.
        /// </summary>
        /// <param name="name">This file's name and class name.</param>
        /// <param name="scriptName">Name of the script to apply this Editor script to.</param>
        /// <param name="path">The folder path to create this file in.</param>
        /// <param name="uxmlDocPath">Folder path of this file's UXML document.</param>
        public static void CreateLockableEditorScript(string name, string scriptName, string path, string uxmlDocPath)
        {
            ScriptBuilder content = new ScriptBuilder(name);

            content.WithUsings(new string[] { "UnityEditor", "UnityEngine", "UnityEngine.UIElements", nameof(InspectorLock) })
                   .WithInheritance(new string[] { $"LockableInspector" })
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

            AssetDatabase.CopyAsset(InternalAssetReferences.Instance.uxmlLockableObjectTemplatePath, $"{path}/{name}");
            AssetDatabase.Refresh();
        }

        public static GameObject CreateLockableAsset(string name)
        {
            return ObjectFactory.CreateGameObject(name);
        }

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

                // Check if folder exists
                if (AssetDatabase.IsValidFolder(path))
                {           
                    //Debug.LogWarning($"Folder '{currentFolder}' already exists in path '{path}'.");
                    continue;
                }

                AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(currentFolder));
            }

            return folderPath;
        }

        private static string MakePath(params string[] values) => string.Join("/", values);

    }

}