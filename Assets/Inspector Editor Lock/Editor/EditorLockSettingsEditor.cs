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
        private bool m_UnappliedSettings = false;

        private VisualElement m_UnappliedChangesElem;
        private FolderPathSelection m_FolderSelection;
        private EditorLockSettings m_LockSettings;
        private Foldout m_ResetFoldout;

        #region Serialiation
        private string m_DefaultPathPropertyName;

        private SerializedProperty m_DefaultPathProperty;
        private SerializedProperty m_UnlockedColorProperty;
        private SerializedProperty m_LockedColorProperty;
        private SerializedProperty m_LockedOpacityProperty;
        private SerializedProperty m_BorderWidthProperty;

        private SerializedProperty m_PreviousLockSettings;

        #endregion
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

            m_PreviousLockSettings = serializedObject.FindProperty(nameof(m_LockSettings.PreviousLockSettings));
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();

            m_FolderSelection = root.Q<FolderPathSelection>();
            m_UnappliedChangesElem = root.Q<VisualElement>("UnappliedChangesElem");

            root.Q<Button>("ApplyButton").RegisterCallback<ClickEvent>(ApplyButtonClicked);
            root.Q<Button>("DiscardButton").RegisterCallback<ClickEvent>(DiscardButtonClicked);
            root.Q<Button>("ResetButton").RegisterCallback<ClickEvent>(RestoreDefaultSettings);
   
            root.RegisterCallback<ChangeEvent<float>>(ChangesRegistered);
            root.RegisterCallback<ChangeEvent<int>>(ChangesRegistered);
            root.RegisterCallback<ChangeEvent<Color>>(ChangesRegistered);
            root.RegisterCallback<ChangeEvent<string>>(ChangesRegistered);

            m_ResetFoldout = root.Q<Foldout>();

            if (m_FolderSelection != null)
            {
                m_FolderSelection.SetPathBinding(serializedObject, m_DefaultPathPropertyName);
                m_FolderSelection.SetPrefix("Assets/");
                m_FolderSelection.SetDefaultPath("Scripts");
            }

            return root;
        }

        private void DiscardButtonClicked(ClickEvent evt)
        {
            RevertChanges();
        }

        private void ApplyButtonClicked(ClickEvent evt)
        {
            ApplyChanges();
        }

        #region Event Change Callbacks
        private void ChangesRegistered(ChangeEvent<float> evt)
        {
            UnappliedChanges();
            //Debug.Log($"Change event of type float with value '{evt.newValue}' registered.");
        }

        private void ChangesRegistered(ChangeEvent<int> evt)
        {
            UnappliedChanges();
            //Debug.Log($"Change event of type int with value '{evt.newValue}' registered.");
        }

        private void ChangesRegistered(ChangeEvent<Color> evt)
        {
            UnappliedChanges();
            //Debug.Log($"Change event of type color with value '{evt.newValue}' registered.");
        }

        private void ChangesRegistered(ChangeEvent<string> evt)
        {
            UnappliedChanges();
            //Debug.Log($"Change event of type string with value '{evt.newValue}' registered.");
        } 

        private void UnappliedChanges()
        {
            m_UnappliedChangesElem.style.display = DisplayStyle.Flex;
            m_UnappliedSettings = true;

        }
        #endregion

        private void ApplyChanges()
        {
            // Add check to see if update was successful or not
            
            UpdateUSSFile();
            
            m_UnappliedChangesElem.style.display = DisplayStyle.None;
            m_UnappliedSettings = false;
            SetPreviousUpdateSettings();

            serializedObject.ApplyModifiedProperties();
        }

        private void RevertChanges()
        {
            SetLockSettings(m_LockSettings.PreviousLockSettings);
            m_UnappliedChangesElem.style.display = DisplayStyle.None;
            m_UnappliedSettings = false;
        }

        /// <summary>
        ///  Updates the USS file associated with this UI Element.
        /// </summary>
        private void UpdateUSSFile()
        {
            var elementStylingUnlocked = "element-styling-unlocked";
            var buttonStylingUnlocked = "button-styling-unlocked";

            var elementStylingLocked = "element-styling-locked";
            var buttonStylingLocked = "button-styling-locked";

            #region Button Colors
            // Set Locked button color
            var fileData = USSFileParser
                                .EditValueInUSSClass(
                                    m_USSFileLocation,
                                    buttonStylingLocked,
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_LockedColorProperty.colorValue)
                                    );


            // Set Unlocked button color
            fileData.EditClassValue(buttonStylingUnlocked,
                                    "background-color",
                                    USSFileParser.ColorToUSS(m_UnlockedColorProperty.colorValue)); 
            #endregion

            #region Border Color
            // Set Locked border color
            fileData.EditClassValue(elementStylingLocked,
                                    "border-color",
                                    USSFileParser.ColorToUSS(m_LockedColorProperty.colorValue));

            // Set Unlocked border color
            fileData.EditClassValue(elementStylingUnlocked,
                                    "border-color",
                                    USSFileParser.ColorToUSS(m_UnlockedColorProperty.colorValue));

            #endregion
            #region Border Width
            // Set Border Width
            fileData.EditClassPixelValue(elementStylingLocked, "border-width", m_BorderWidthProperty.intValue);
            fileData.EditClassPixelValue(elementStylingUnlocked, "border-width", m_BorderWidthProperty.intValue);

            // Set Button top margin negative value to match border
            fileData.EditClassPixelValue(buttonStylingLocked, "margin-top", - m_BorderWidthProperty.intValue);
            fileData.EditClassPixelValue(buttonStylingUnlocked, "margin-top", - m_BorderWidthProperty.intValue);

            #endregion

            // Set Lock opacity - probably different param tho.
            fileData.EditClassValue(elementStylingLocked,
                                    "opacity",
                                    m_LockedOpacityProperty.floatValue.ToString("0.00"));

            USSFileParser.WriteToFile(fileData);
        }

        private void SetPreviousUpdateSettings()
        {
            m_PreviousLockSettings.FindPropertyRelative("Path").stringValue = m_DefaultPathProperty.stringValue;
            m_PreviousLockSettings.FindPropertyRelative("UnlockedColor").colorValue = m_UnlockedColorProperty.colorValue;
            m_PreviousLockSettings.FindPropertyRelative("LockedColor").colorValue = m_LockedColorProperty.colorValue;
            m_PreviousLockSettings.FindPropertyRelative("LockedOpacity").floatValue = m_LockedOpacityProperty.floatValue;
            m_PreviousLockSettings.FindPropertyRelative("BorderWidth").intValue = m_BorderWidthProperty.intValue;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SetLockSettings(LockSettingsData settings)
        {
            m_DefaultPathProperty.stringValue = settings.Path;
            m_UnlockedColorProperty.colorValue = settings.UnlockedColor;
            m_LockedColorProperty.colorValue = settings.LockedColor;
            m_LockedOpacityProperty.floatValue = settings.LockedOpacity;
            m_BorderWidthProperty.floatValue = settings.BorderWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private void RestoreDefaultSettings(ClickEvent evt)
        {
            SetLockSettings(m_LockSettings.DefaultLockSettings);
            serializedObject.ApplyModifiedProperties();

            UnappliedChanges();

            m_ResetFoldout.value = false;
        }
    }
}
