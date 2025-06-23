using UnityEditor;
using UnityEngine;
using System.Text;
using System;
using System.Linq;
using System.IO;

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

        public static void CreateScript(string attributes, string fileName, string content, string directory, string inheritance = "MonoBehaviour")
        {
            // Sanitize name
            fileName = fileName.Replace(" ", string.Empty);

            if (!fileName.EndsWith(".cs"))
            {
                fileName += FileEnding;
            }

            string path = string.Join("/", directory, fileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (System.IO.File.Exists(path))
            {
                Debug.Log("A file with that name already exists!");
                return;
            }

            if (!AssetDatabase.IsValidFolder(directory))
            {
                CreateDirectories(directory);
            }

            #region Runtime script creation
            /*
            // create the script
            using StreamWriter outfile = new StreamWriter(path);
            outfile.WriteLine("using UnityEngine;");
            outfile.WriteLine("");
            outfile.WriteLine(attributes);
            outfile.WriteLine($"public class {name} : {inheritance}");
            outfile.WriteLine("{");
            outfile.WriteLine($"\t{content}");
            outfile.WriteLine("}");
            AssetDatabase.Refresh();
            */
            #endregion


            #region Test script creation
            // spoof creating the script
            StringBuilder outBuilder = new StringBuilder(path);
            outBuilder.AppendLine();
            outBuilder.AppendLine("using UnityEngine;");
            outBuilder.AppendLine("");
            outBuilder.AppendLine(attributes);
            outBuilder.AppendLine($"public class {fileName} : {inheritance}");
            outBuilder.AppendLine("{");
            outBuilder.AppendLine($"\t{content}");
            outBuilder.AppendLine("}");

            Debug.Log($"Created script with name '{fileName}' in path '{path}'");
            Debug.Log($"Content of {outBuilder.ToString()}");
            Debug.Log("");

            #endregion

        }

        //[MenuItem("EditorLock/New Lockable Script")]

        public static void CreateLockableScript(string name, string path)
        {
            StringBuilder content = new StringBuilder();
            content.Append("[SerializeField]")
                   .AppendLine()
                   .AppendLine("\tpublic bool[] m_EditorLockStates;")
                   .Append("\tpublic string LockablePropertyPath => nameof(m_EditorLockStates);");


            CreateScript("", name, content.ToString(), directory: path, inheritance: "MonoBehaviour, IEditorLockable");
        }

        //[MenuItem("EditorLock/New Lockable Editor Script")]
        public static void CreateLockableEditorScript(string name, string path)
        {
            var attributes = "[CustomEditor(typeof(TestObject))]";
            var inheritance = "LockableEditor";
            CreateScript(attributes, name, content: string.Empty, directory: path, inheritance);
        }

        //[MenuItem("EditorLock/New Lockable UXML Document")]
        public static void CreateLockableUXMLDoc(string name, string path)
        {
            //VisualTreeAsset visualTree = new VisualTreeAsset();
            //var visualTree = ScriptableObject.CreateInstance(typeof(VisualTreeAsset));
            //var folderDirectory = "Assets/" + UXMLFolder;

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
                //Debug.Log($"New folder directory created at: '{folderDirectory}'");
            }

            //string guid = AssetDatabase.CreateFolder($"Assets/{UIFolder}", "UXML");
            //folderDirectory = AssetDatabase.GUIDToAssetPath(guid);  

            Debug.Log($"Created UXML Asset in: '{folderDirectory}/{name}'");
            Debug.Log("");

            #endregion

            #region Runtime Code
            AssetDatabase.CopyAsset("Assets/Inspector Editor Lock/UI/UXML/LockableUXMLTemplate.uxml", $"{folderDirectory}/{name}.uxml");
            AssetDatabase.Refresh();

            /*
            // Create folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder(folderDirectory))
            {
                Create parent folder
                if (!AssetDatabase.IsValidFolder("Assets/Scripts/UI"))
                {
                    AssetDatabase.CreateFolder("Assets/Scripts", "UI");
                }

                string guid = AssetDatabase.CreateFolder($"Assets/{UIFolder}", "UXML");
                folderDirectory = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"New folder directory {folderDirectory}");
            }
                AssetDatabase.CopyAsset("Assets/Inspector Editor Lock/UI/UXML/LockableUXMLTemplate.uxml", "Assets/Scripts/UI/UXML/CopiedTemplate.uxml");
                AssetDatabase.Refresh();

            */
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

            //Test
            Debug.Log($"Created GameObject with name: '{name}' ");
            Debug.Log("");

            //Runtime
            //var asset = ObjectFactory.CreateGameObject(name);
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