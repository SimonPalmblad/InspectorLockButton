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
        private SerializedProperty m_BorderWidthProperty;

        private Foldout m_ResetFoldout;

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
            m_BorderWidthProperty = serializedObject.FindProperty(nameof(m_LockSettings.BorderWidth));
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            m_FolderSelection = root.Q<FolderPathSelection>();
            root.Q<Button>("Reset").RegisterCallback<ClickEvent>(GetRestoreDefaultSettings);
            root.Q<Button>("Apply").RegisterCallback<ClickEvent>(ApplyButtonClicked);
            m_ResetFoldout = root.Q<Foldout>();

            if (m_FolderSelection != null)
            {
                m_FolderSelection.SetPathBinding(serializedObject, m_DefaultPathPropertyName);
                m_FolderSelection.SetPrefix("Assets/");
                m_FolderSelection.SetDefaultPath("Scripts");
            }

            return root;
        }

        private void ApplyButtonClicked(ClickEvent evt)
        {
            UpdateUSSFile();
        }

        private void UpdateUSSFile()
        {
            // Set Locked button color
            var fileData = USSFileParser
                                .EditValueInUSSClass(
                                    m_USSFileLocation,
                                    "button-styling-locked",
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_LockedColorProperty.colorValue)
                                    );

            // Set Locked border color
            fileData.EditClassValue("element-styling-locked",
                                    "border-color",
                                    USSFileParser.ColorToUSS(m_LockedColorProperty.colorValue));


            // Set Lock opacity - probably different param tho.
            fileData.EditClassValue("element-styling-locked",
                                    "opacity",
                                    m_LockedOpacityProperty.floatValue.ToString("0.00"));


            // Set Locked border width
            fileData.EditClassValue("element-styling-locked",
                                    "border-width",
                                    m_BorderWidthProperty.intValue.ToString());

            // Set Unlocked button color
            fileData.EditClassValue("button-styling-unlocked",
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_UnlockedColorProperty.colorValue));


            // Set Unlocked border width
            fileData.EditClassValue("element-styling-unlocked",
                                    "border-width",
                                    m_BorderWidthProperty.intValue.ToString());


            USSFileParser.WriteToFile(fileData);
        }

        private void GetRestoreDefaultSettings(ClickEvent evt)
        {
            var defaultSettings = m_LockSettings.DefaultLockSettings;

            m_DefaultPathProperty.stringValue = defaultSettings.Path;
            m_UnlockedColorProperty.colorValue = defaultSettings.UnlockedColor;
            m_LockedColorProperty.colorValue = defaultSettings.LockedColor;
            m_LockedOpacityProperty.floatValue = defaultSettings.LockedOpacity;

            m_ResetFoldout.value = false;
            serializedObject.ApplyModifiedProperties();
            //UpdateUSSFile();

        }
    }
}
