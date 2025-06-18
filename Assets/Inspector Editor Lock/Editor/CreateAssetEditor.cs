using UnityEditor;
using UnityEngine;

public class CreateAssetEditor: Editor
{

    //[MenuItem("EditorLock/Show Window")]
    public static void CreateWindow()
    {
        CreateAssetWindow wnd = new CreateAssetWindow();
        wnd.titleContent = new GUIContent("CreateAssetWindow");
    }
}
