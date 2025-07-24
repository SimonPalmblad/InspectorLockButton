using UnityEditor;
using UnityEngine;

namespace EditorLock
{
    public class CreateAssetEditor : Editor
    {

        //[MenuItem("EditorLock/Show Window")]
        public static void CreateWindow()
        {
            AssetCreationWindow wnd = new AssetCreationWindow();
            wnd.titleContent = new GUIContent("CreateAssetWindow");
        }
    } 
}
