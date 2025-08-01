using UnityEngine;
using UnityEditor;
using InspectorLockUtilities;
using UnityEngine.UIElements;
using InspectorLock.Internals;
using System.Text;

namespace InspectorLock
{
    [CustomEditor( typeof(InspectorLockSettings) )]
    public class InspectorLockSettingsEditor: LockableInspector
    {
        protected string m_USSFileLocation = InternalAssetReferences.Instance.ussInspectorLockButtonPath;
        private bool m_ListenToEventChanges = false;

        private VisualElement m_Root;
        private VisualElement m_UnappliedChangesElem;
        private FolderPathSelection m_FolderSelection;
        private InspectorLockSettings m_LockSettings;

        private int borderMinSize = 0;
        private int borderMaxSize = 25;

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
            m_LockSettings = (InspectorLockSettings)target;
            m_DefaultPathPropertyName = nameof(m_LockSettings.DefaultAssetPath);

            m_DefaultPathProperty = serializedObject.FindProperty(m_DefaultPathPropertyName);
            m_UnlockedColorProperty = serializedObject.FindProperty(nameof(m_LockSettings.UnlockedColor));
            m_LockedColorProperty = serializedObject.FindProperty(nameof(m_LockSettings.LockedColor));
            m_LockedOpacityProperty = serializedObject.FindProperty(nameof(m_LockSettings.LockedOpacity));
            m_BorderWidthProperty = serializedObject.FindProperty(nameof(m_LockSettings.BorderWidth));

            m_PreviousLockSettings = serializedObject.FindProperty("PreviousLockSettings");

            var settings = AssetDatabase.FindAssets("a:assets t:InspectorLockSettings");
            if(settings.Length > 1)
            {
                var builder = new StringBuilder();
                foreach (string GUIDPath in settings)
                {
                   builder.AppendLine($"\t - {AssetDatabase.GUIDToAssetPath(GUIDPath)}");
                }

                Debug.LogWarning("More than one Inspector Lock Settings asset was found the current project." +
                    "\nPlease remove any excess assets. Assets are located in the following directories:"
                    + $"\n{builder}");
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = base.CreateInspectorGUI();

            m_FolderSelection = m_Root.Q<FolderPathSelection>();
            m_UnappliedChangesElem = m_Root.Q<VisualElement>("UnappliedChangesElem");

            m_Root.Q<Button>("ApplyButton").RegisterCallback<ClickEvent>(ApplyButtonClicked);
            m_Root.Q<Button>("DiscardButton").RegisterCallback<ClickEvent>(DiscardButtonClicked);
            m_Root.Q<Button>("ResetButton").RegisterCallback<ClickEvent>(RestoreDefaultSettings);
   
            m_Root.RegisterCallback<ChangeEvent<float>>(ChangesRegistered);
            m_Root.RegisterCallback<ChangeEvent<int>>(ChangesRegistered);
            m_Root.RegisterCallback<ChangeEvent<Color>>(ChangesRegistered);
            m_Root.RegisterCallback<ChangeEvent<string>>(ChangesRegistered);

            var borderWidthField = m_Root.Q<IntegerField>("BorderWidth");
            borderWidthField.RegisterValueChangedCallback(evt => { borderWidthField.value = Mathf.Clamp(evt.newValue, borderMinSize, borderMaxSize); });

            if (m_FolderSelection != null)
            {
                m_FolderSelection.SetPathBinding(serializedObject, m_DefaultPathPropertyName);
                m_FolderSelection.SetPrefix("Assets/");
                m_FolderSelection.SetDefaultPath("Scripts");
            }

            ListenToEventChangeCallbacks(500);
            return m_Root;
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
            UnappliedChanges(evt.newValue);
        }

        private void ChangesRegistered(ChangeEvent<int> evt)
        {
            UnappliedChanges(evt.newValue);
        }

        private void ChangesRegistered(ChangeEvent<Color> evt)
        {
            UnappliedChanges(evt.newValue);
        }

        private void ChangesRegistered(ChangeEvent<string> evt)
        {
            UnappliedChanges(evt.newValue);
        } 

        private void UnappliedChanges<T>(T changeType)
        {

            if(!m_ListenToEventChanges) 
                return;

            m_ListenToEventChanges = false;
            m_UnappliedChangesElem.style.display = DisplayStyle.Flex;


        }
        #endregion

        private void ListenToEventChangeCallbacks(long delay) 
        {
            m_ListenToEventChanges = false;
            m_Root.schedule.Execute(() => m_ListenToEventChanges = true).ExecuteLater(delay);
        }


        private void ApplyChanges()
        {
            // Add check to see if update was successful or not
            m_ListenToEventChanges = false;
            
            UpdateUSSFile();            
            m_UnappliedChangesElem.style.display = DisplayStyle.None;
            SetPreviousUpdateSettings();
            serializedObject.ApplyModifiedProperties();

            ListenToEventChangeCallbacks(2000);
            //code
        }

        private void RevertChanges()
        {            
            SetLockSettings(m_LockSettings.PreviousLockSettings);
            m_UnappliedChangesElem.style.display = DisplayStyle.None;

            ListenToEventChangeCallbacks(100);
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
            var path = m_PreviousLockSettings.FindPropertyRelative("Path");
            var lockedCol = m_PreviousLockSettings.FindPropertyRelative("LockedColor");
            var unlockedCol = m_PreviousLockSettings.FindPropertyRelative("UnlockedColor");
            var borderWidth = m_PreviousLockSettings.FindPropertyRelative("BorderWidth");
            var lockedOpacity = m_PreviousLockSettings.FindPropertyRelative("LockedOpacity");            

            path.stringValue = m_DefaultPathProperty?.stringValue ?? path.stringValue;
            lockedCol.colorValue = m_LockedColorProperty?.colorValue ?? lockedCol.colorValue;
            unlockedCol.colorValue = m_UnlockedColorProperty?.colorValue ?? unlockedCol.colorValue;
            lockedOpacity.floatValue = m_LockedOpacityProperty?.floatValue ?? lockedOpacity.floatValue;
            borderWidth.intValue = m_BorderWidthProperty?.intValue ?? borderWidth.intValue;

            serializedObject.ApplyModifiedProperties();

        }

        private void SetLockSettings(LockSettingsData settings)
        {
            m_DefaultPathProperty.stringValue = settings.Path;
            m_UnlockedColorProperty.colorValue = settings.UnlockedColor;
            m_LockedColorProperty.colorValue = settings.LockedColor;
            m_LockedOpacityProperty.floatValue = settings.LockedOpacity;
            m_BorderWidthProperty.intValue = settings.BorderWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private void RestoreDefaultSettings(ClickEvent evt)
        {
            SetLockSettings(m_LockSettings.DefaultLockSettings);
            UnappliedChanges(evt.actionKey);

        }
    }
}
