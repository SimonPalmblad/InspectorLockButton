using PlasticPipe.PlasticProtocol.Messages;
using ScriptFileCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InspectorLock
{
    internal struct TextFieldData
    {
        public TextField TextField;
        public string FileEnding;

        public TextFieldData(TextField textField, string fileEnding)
        {
            TextField = textField;
            FileEnding = fileEnding;
        }
    }

    
    public class AssetCreationWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        private VisualElement m_CopyNameToElem;
        private VisualElement m_MasterContainer;

        private List<TextField> m_UniformNameFields = new List<TextField>();
        private List<FolderPathSelection> m_FolderPathFields = new List<FolderPathSelection>();

        private TextField m_SharedNameField;
        private TextField m_RootFolderPath;

        private Toggle indvPathToggle;

        private FolderPathSelection m_RootFolderPathElem;
        private FolderPathSelection m_ScriptFolderPathElem;
        private FolderPathSelection m_EditorFolderPathElem;
        private FolderPathSelection m_UXMLFolderPathElem;

        private readonly string m_ScriptName = "ScriptName";
        private readonly string m_EditorName = "EditorName";
        private readonly string m_GameObjectName = "GameObjectName";

        private string m_RootFolderPathName = "RootFolderPath";
        private string m_ScriptFolderPathName = "ScriptPathSelection";
        private string m_EditorFolderPathName = "EditorPathSelection";
        private string m_UXMLFolderPathName = "UXMLPathSelection";
        
        private bool m_UseDefaultFolder;

        public InspectorLockSettings m_InspectorLockSettings;

        private static readonly Vector2 minDimensions = new Vector2(590, 370);

        [MenuItem("Assets/Create/🔒 Inspector Lock/Lockable Asset Creation")]
        public static void ShowWindow()
        {
            AssetCreationWindow wnd = GetWindow<AssetCreationWindow>();
            wnd.titleContent = new GUIContent("🔒 New Lockable Assets");
            wnd.minSize = minDimensions;
        }

        public void OnEnable()
        {
            var lockSettingsGUID = AssetDatabase.FindAssets("t:InspectorLockSettings a:assets").First();
            m_InspectorLockSettings = AssetDatabase.LoadAssetAtPath<InspectorLockSettings>(AssetDatabase.GUIDToAssetPath(lockSettingsGUID));
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(root);

            // ------ Get Visual Elements ------ //

            Toggle nameToggle = root.Q<Toggle>("NameToggle");
            indvPathToggle = root.Q<Toggle>("IndividualPathToggle");

            m_MasterContainer = root.Q<VisualElement>("MasterContainer");
            m_SharedNameField = root.Q<TextField>("SharedName");
            m_CopyNameToElem = root.Q<VisualElement>("CopyNameTo");

            m_RootFolderPathElem = root.Q<FolderPathSelection>(m_RootFolderPathName);
            m_ScriptFolderPathElem = root.Q<FolderPathSelection>(m_ScriptFolderPathName);
            m_EditorFolderPathElem = root.Q<FolderPathSelection>(m_EditorFolderPathName);
            m_UXMLFolderPathElem = root.Q<FolderPathSelection>(m_UXMLFolderPathName);
            m_FolderPathFields = m_MasterContainer.GetChildrenOfType<FolderPathSelection>();

            m_RootFolderPath = m_RootFolderPathElem.Q<TextField>("PathTextField");

            Button createAssetsButton = root.Q<Button>("CreateButton");


            // ------ Register Event Callbacks ------ //
            nameToggle.RegisterValueChangedCallback(NameToggled);

            indvPathToggle.RegisterValueChangedCallback(IndividualPathsToggled);

            m_RootFolderPath.RegisterValueChangedCallback(RootFolderChanged);

            createAssetsButton.RegisterCallback<ClickEvent>(CreateAssets);

            m_SharedNameField.RegisterCallback<InputEvent>(UpdateAllNames);

            // Register all CopyTextToElement children's events
            foreach (VisualElement child in m_CopyNameToElem.Children())
            {
                if (child is TextField)
                {
                    var textField = child as TextField;
                    m_UniformNameFields.Add(textField);

                    if(child.name == m_GameObjectName)
                    {
                        var gameObjectData = new TextFieldData(textField, string.Empty);
                        continue;
                    }

                    if (child.name == m_ScriptName || textField.name == m_EditorName)
                    {
                        var scriptData = new TextFieldData(textField, ".cs");
                        textField.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOnFocusedOut, scriptData);

                        continue;
                    }

                    var uxlmData = new TextFieldData(textField, ".uxml");
                    textField.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOnFocusedOut, uxlmData);
                }
            }

            // ------ Copy values from Inspector Lock Settings object ------ //
            if (m_InspectorLockSettings != null)
            {
                m_RootFolderPath.value = m_InspectorLockSettings.DefaultAssetPath;
                m_ScriptFolderPathElem.defaultPath = m_InspectorLockSettings.DefaultAssetPath;
                m_EditorFolderPathElem.defaultPath = string.Join('/', m_InspectorLockSettings.DefaultAssetPath, "Editor");
                m_UXMLFolderPathElem.defaultPath = string.Join('/', m_InspectorLockSettings.DefaultAssetPath, "UI/UXML");
            }
        }

        private void RootFolderChanged(ChangeEvent<string> evt)
        {
            UpdateAllPathNames(m_RootFolderPathElem.PathRaw);
        }

        private void CreateAssets(ClickEvent evt)
        {
            var uxmlField = new TextField();
            var scriptField = new TextField();
            var editorField = new TextField();

            GameObject newAsset = ObjectFactory.CreateGameObject("New Lockable Object");
          
            // Create assets from UXML info in the Asset Creation Window
            foreach (TextField textField in m_UniformNameFields)
            {
                switch (textField.name)
                {
                    case "UXMLName":
                        uxmlField = textField;
                        CreateLockableObject.CreateLockableUXMLDoc(PlaceholderIfEmpty(uxmlField), m_UXMLFolderPathElem.PathFull);
                        break;

                    case "ScriptName":
                        scriptField = textField;
                        CreateLockableObject.CreateLockableScript(PlaceholderIfEmpty(scriptField), m_ScriptFolderPathElem.PathFull);
                        break;

                    case "EditorName":
                        editorField = textField;
                        break;

                    case "GameObjectName":
                        newAsset.name = (PlaceholderIfEmpty(textField));
                        break;
                }
            }

            var uxmlDocPath = m_UXMLFolderPathElem.PathFull + "/" + PlaceholderIfEmpty(uxmlField);
            
            CreateLockableObject.CreateLockableEditorScript(PlaceholderIfEmpty(editorField),
                                                   PlaceholderIfEmpty(scriptField),
                                                   m_EditorFolderPathElem.PathFull,
                                                   uxmlDocPath);                
            
            // ------ Add placeholder component to GameObject ------ //
            
            if(newAsset == null)
            {
                return;
            }

            String ScriptName = StringHelpers.WithoutEnding(PlaceholderIfEmpty(scriptField));
            newAsset.AddComponent<PlaceholderScriptComponent>();
            newAsset.GetComponent<PlaceholderScriptComponent>().ReplacementName = ScriptName;
            
            Selection.objects = new UnityEngine.Object[] { newAsset };
            EditorUtility.RequestScriptReload();
        }

        /// <summary>
        /// Get the content of a <see cref="TextField"/> or the placeholder text if the field is empty.
        /// </summary>
        /// <param name="textField"></param>
        /// <returns>The contents as a string.</returns>
        private string PlaceholderIfEmpty(TextField textField)
        {
            if (textField.text == string.Empty)
            {
                return textField.textEdition.placeholder;
            }

            return textField.text;
        }

        private void TextFieldOnFocusedOut(FocusOutEvent evt, TextFieldData data)
        {
            if (data.TextField.text == string.Empty)
            {
                return;
            }

            if (data.TextField.text.EndsWith(data.FileEnding))
            {
                return;
            }

            data.TextField.value += data.FileEnding;
        }

        private void UpdateAllNames(InputEvent evt)
        {
            foreach (TextField elem in m_UniformNameFields)
            {
                if (evt.newData == "")
                {
                    elem.value = elem.textEdition.placeholder;
                    continue;
                }

                elem.value = evt.newData;

                if (elem.name == m_ScriptName)
                {
                    elem.value += ".cs";
                    continue;
                }

                if(elem.name == m_EditorName)
                {
                    elem.value += "Editor.cs";
                    continue;
                }

                if (elem.name == m_GameObjectName)
                {
                    continue;
                }

                elem.value += "_uxml.uxml";
            }
        }

        private void UpdateAllPathNames(string path)
        {
            foreach (FolderPathSelection elem in m_FolderPathFields)
            {
                // Ignore root path folder
                if (elem.name == m_RootFolderPathName)
                {
                    continue;
                }

                // If field is left empty, restore placeholder text
                if (path == "")
                {
                    elem.TextField.value = elem.TextField.textEdition.placeholder;
                    continue;
                }

                var newPath = path; // Prefix all folders with 'Assets/'

                // Script folder
                if (elem.name == m_ScriptFolderPathName)
                {
                    elem.TextField.value = newPath;
                    continue;
                }

                // Editor script folder
                if (elem.name == m_EditorFolderPathName)
                {
                    elem.TextField.value = newPath + "/" +"Editor";
                    continue;
                }

   
                //Uxml folder
                elem.TextField.value = newPath + "/" + "UI/UXML";
            }
        }

        private void NameToggled(ChangeEvent<bool> evt)
        {

            foreach (TextField textField in m_UniformNameFields)
            {
                textField.SetEnabled(!evt.newValue);
            }

            m_SharedNameField.SetEnabled(evt.newValue);

            if (evt.newValue)
            {
                m_SharedNameField.RegisterCallback<InputEvent>(UpdateAllNames);
                return;
            }

            m_SharedNameField.UnregisterCallback<InputEvent>(UpdateAllNames);
        }

        private void IndividualPathsToggled(ChangeEvent<bool> evt)
        {
            ToggleIndividualPaths(evt.newValue);
        }

        private void ToggleIndividualPaths(bool toggle)
        {
            indvPathToggle.value = toggle;

            foreach (FolderPathSelection folderPath in m_FolderPathFields)
            {
                if (folderPath.name == m_RootFolderPathName && !m_UseDefaultFolder)
                {
                    folderPath.SetEnabled(!toggle);
                    continue;
                }

                folderPath.SetEnabled(toggle);
            }
        }
    }

}