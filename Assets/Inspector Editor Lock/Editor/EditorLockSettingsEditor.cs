using UnityEngine;
using UnityEditor;
using EditorLock;
using UnityEngine.UIElements;

namespace Editorlock
{
    [CustomEditor( typeof(EditorLockSettings) )]
    public class EditorLockSettingsEditor: LockableEditor<EditorLockSettings>
    {   

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            var folderPathSelection = root.Q<FolderPathSelection>();
            folderPathSelection.Init(serializedObject); // Give a binding path reference to the folder path

            return root;
        }
    }
}
