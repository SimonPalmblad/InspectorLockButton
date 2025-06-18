using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UIElements;
using System;
using UnityEditor.VersionControl;

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

    public static void CreateScript(string attributes, string name, string content, string directory, string inheritance = "MonoBehaviour")
    {
        // Sanitize name
        name = name.Replace(" ", string.Empty);

        var fileName = name + FileEnding;

        string path = string.Join("/", "Assets", directory, fileName);
        //Directory.CreateDirectory(Path.GetDirectoryName(path));

        if (File.Exists(path))
        {
            Debug.Log("A file with that name already exists!");
            return;
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
        outBuilder.AppendLine($"public class {name} : {inheritance}");
        outBuilder.AppendLine("{");
        outBuilder.AppendLine($"\t{content}");
        outBuilder.AppendLine("}"); 

        Debug.Log(outBuilder.ToString());
        #endregion

    }

    [MenuItem("EditorLock/New Lockable Script")]
    public static void CreateLockableScript(/*string name = "", string path =""*/)
    {
        //if(name == string.Empty)
        //{
        //    name = DefaultName;
        //}

        //if (path == string.Empty)
        //{
        //    path = ScriptsFolder;
        //}

        StringBuilder content = new StringBuilder();
        content.Append("[SerializeField]")
               .AppendLine()
               .AppendLine("\tpublic bool[] m_EditorLockStates;")
               .Append("\tpublic string LockablePropertyPath => nameof(m_EditorLockStates);");   
    

        CreateScript("", "NewName", content.ToString(), directory: "path", inheritance: "MonoBehaviour, IEditorLockable");
    }

    [MenuItem("EditorLock/New Lockable Editor Script")]
    public static void CreateLockableEditorScript()
    {
        var attributes = "[CustomEditor(typeof(TestObject))]";
        var inheritance = "LockableEditor";
        CreateScript(attributes, name: "NewLockableEditor", content: string.Empty, directory: EditorFolder, inheritance);
    }

    [MenuItem("EditorLock/New Lockable UXML Document")]
    public static void CreateLockableUXMLDoc()
    {
        //VisualTreeAsset visualTree = new VisualTreeAsset();
        var visualTree = ScriptableObject.CreateInstance(typeof(VisualTreeAsset));
        var folderDirectory = "Assets/" + UXMLFolder;
        
        #region Test Code
        // Test spoofing
        if (!AssetDatabase.IsValidFolder(folderDirectory))
        {
            //Create parent folder
            if (!AssetDatabase.IsValidFolder("Assets/Scripts/UI"))
            {
                Debug.Log($"Created Folder at: 'Assets/Scripts' with name 'UI'");
            }

            string guid = AssetDatabase.CreateFolder($"Assets/{UIFolder}", "UXML");
            folderDirectory = AssetDatabase.GUIDToAssetPath(guid);

            Debug.Log($"New folder directory created at: '{folderDirectory}'");
        }

        Debug.Log($"Created UXML Asset in: 'Assets/Scripts/UI/UXML/LockableUXMLTemplate.uxml'");  

        #endregion



        #region Runtime Code
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
            AssetDatabase.CopyAsset("Assets/Scripts/UI/UXML/LockableUXMLTemplate.uxml", "Assets/Scripts/UI/UXML/CopiedTemplate.uxml");
            AssetDatabase.Refresh();

        */
        #endregion
    }

    // Add method for attaching the script to this object
    [MenuItem("EditorLock/New Lockable Asset")]
    public static void CreateLockableAssetScript()
    {
        var name = "New Lockable Asset";
        
        //Test
        Debug.Log($"Created GameObject with name: '{name}' ");
        
        //Runtime
        //var asset = ObjectFactory.CreateGameObject(name);
    }

}
