using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Enumeration;
using UnityEditor.U2D.Aseprite;


public class CreateNewLockable : Editor
{
    public string AssetFolder = "Assets";
    public string ScriptsFolder = "Scripts";
    public string EditorFolder = "Scripts/Editor";
    public string UXMLFolder = "Scripts/UXML";

    public string AssetName = "NewAsset";
    public string ScriptName = string.Empty;
    public string UXMLName = string.Empty;
    public string EditorName = string.Empty;

    public string FileEnding = ".cs";

    public void CreateScript(string name, string content, string folder, string inheritance = "MonoBehaviour")
    {
        string path = Path.Combine(Application.dataPath, folder, name, FileEnding);
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        var scriptContent = $"{name}: {inheritance} \n {content}";

        File.WriteAllText(path, content);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
        Debug.Log($"Script {name + FileEnding} created.");
    }

    //MenuItem() 
    public void CreateLockableAsset()
    {
        CreateScript(ScriptName, "Script content here.", ScriptsFolder, inheritance: "");

    }


}
