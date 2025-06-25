using UnityEngine;
using UnityEditor;
using EditorLock;
using UnityEngine.UIElements;

namespace Editorlock
{
    [CustomEditor( typeof(EditorLockSettings) )]
    public class EditorLockSettingsEditor: LockableEditor<EditorLockSettings>
    {
        private string m_DefaultPathProperty;
        private FolderPathSelection m_FolderSelection;
        private EditorLockSettings m_LockSettings;

        // dirt solution.
        protected new void OnEnable()
        {
            base.OnEnable();
            m_LockSettings = (EditorLockSettings)target;
            m_DefaultPathProperty = nameof(m_LockSettings.DefaultAssetPath);
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            m_FolderSelection = root.Q<FolderPathSelection>();
            root.Q<Button>("Reset").RegisterCallback<ClickEvent>(GetRestoreDefaultSettings);

            if (m_FolderSelection != null)
            {
                m_FolderSelection.SetPathBinding(serializedObject, m_DefaultPathProperty);
                m_FolderSelection.SetPrefix("Assets/");
                m_FolderSelection.SetDefaultPath("Scripts");
            }

            return root;
        }

        private void GetRestoreDefaultSettings(ClickEvent evt)
        {
            var defaultPath = serializedObject.FindProperty("m_DefaultAssetPath");
            var defaultUnlockedCol = serializedObject.FindProperty("m_DefaultUnlockedColor");
            var defaultLockedCol = serializedObject.FindProperty("m_DefaultLockedColor");
            var defaultOpacity = serializedObject.FindProperty("m_DefaultLockedOpacity");
            


        }
    }
}
