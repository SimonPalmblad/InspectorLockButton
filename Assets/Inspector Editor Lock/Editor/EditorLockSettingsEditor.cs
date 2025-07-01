using UnityEngine;
using UnityEditor;
using EditorLock;
using UnityEngine.UIElements;
using System;
using System.IO;
using PlasticPipe.PlasticProtocol.Messages;
using EditorLockUtilies;

namespace Editorlock
{
    [CustomEditor( typeof(EditorLockSettings) )]
    public class EditorLockSettingsEditor: LockableEditor<EditorLockSettings>
    {
        private string m_USSFileLocation = "Assets/Inspector Editor Lock/UI/USS/EditorLockButtonStyle.uss";

        private FolderPathSelection m_FolderSelection;
        private EditorLockSettings m_LockSettings;

        private SerializedProperty m_DefaultPathProperty;
        private SerializedProperty m_UnlockedColorProperty;
        private SerializedProperty m_LockedColorProperty;
        private SerializedProperty m_LockedOpacityProperty;

        private string m_DefaultPathPropertyName;
        // dirty solution
        protected new void OnEnable()
        {
            base.OnEnable();
            m_LockSettings = (EditorLockSettings)target;
            m_DefaultPathPropertyName = nameof(m_LockSettings.DefaultAssetPath);

            m_DefaultPathProperty = serializedObject.FindProperty(m_DefaultPathPropertyName);
            m_UnlockedColorProperty = serializedObject.FindProperty(nameof(m_LockSettings.UnlockedColor));
            m_LockedColorProperty = serializedObject.FindProperty(nameof(m_LockSettings.LockedColor));
            m_LockedOpacityProperty = serializedObject.FindProperty(nameof(m_LockSettings.LockedOpacity));
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            m_FolderSelection = root.Q<FolderPathSelection>();
            root.Q<Button>("Reset").RegisterCallback<ClickEvent>(GetRestoreDefaultSettings);
            root.Q<Button>("Apply").RegisterCallback<ClickEvent>(UpdateUSSFile);

            if (m_FolderSelection != null)
            {
                m_FolderSelection.SetPathBinding(serializedObject, m_DefaultPathPropertyName);
                m_FolderSelection.SetPrefix("Assets/");
                m_FolderSelection.SetDefaultPath("Scripts");
            }

            return root;
        }

        private void UpdateUSSFile(ClickEvent evt)
        {
            // Set Locked button color
            var fileData = USSFileParser
                                .EditValueInUSSClass(
                                    m_USSFileLocation,
                                    "button-styling-locked",
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_LockedColorProperty.colorValue)
                                    );

            // Set Unlocked border color
            fileData.EditClassValue("element-styling-locked",
                                    "border-color",
                                    USSFileParser.ColorToUSS(m_UnlockedColorProperty.colorValue));

            // Set Unlocked button color
            fileData.EditClassValue("button-styling-unlocked",
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_UnlockedColorProperty.colorValue));

            
            // Set lock opacity - probably different param tho.
            fileData.EditClassValue("element-styling-locked",
                                    "opacity",
                                    m_LockedOpacityProperty.floatValue.ToString("0.00"));

            Debug.Log(fileData.FileContent);
            USSFileParser.WriteToFile(fileData);

        }

        private void GetRestoreDefaultSettings(ClickEvent evt)
        {
            var defaultSettings = m_LockSettings.DefaultLockSettings;
            //var defaultPath = serializedObject.FindProperty("m_DefaultAssetPath");
            //var defaultUnlockedCol = serializedObject.FindProperty("m_DefaultUnlockedColor");
            //var defaultLockedCol = serializedObject.FindProperty("m_DefaultLockedColor");
            //var defaultOpacity = serializedObject.FindProperty("m_DefaultLockedOpacity");

            m_DefaultPathProperty.stringValue = defaultSettings.Path;
            m_UnlockedColorProperty.colorValue = defaultSettings.UnlockedColor;
            m_LockedColorProperty.colorValue = defaultSettings.LockedColor;
            m_LockedOpacityProperty.floatValue = defaultSettings.LockedOpacity;

            serializedObject.ApplyModifiedProperties();

        }
    }
}
