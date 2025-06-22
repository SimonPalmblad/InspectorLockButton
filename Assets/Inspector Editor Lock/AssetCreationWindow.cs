using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorLock
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

        private TextField m_AssetNameField;
        private TextField m_RootFolderPath;

        private FolderPathSelection m_RootFolderPathSelection;

        private string m_RootFolderPathName = "RootFolderPath";
        private string m_ScriptName = "ScriptName";
        private string m_EditorName = "EditorName";
        private string m_UXMLName = "UXMLName";

        private string m_ScriptPathName = "ScriptPathSelection";
        private string m_EditorPathName = "EditorPathSelection";
        private string m_UXMLPathName = "UXMLPathSelection";

        [MenuItem("EditorLock/Show Window")]
        public static void ShowExample()
        {
            AssetCreationWindow wnd = GetWindow<AssetCreationWindow>();
            wnd.titleContent = new GUIContent("🔒 LockableAsset");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            m_VisualTreeAsset.CloneTree(root);

            #region Query all relevant Elements


            Toggle nameToggle = root.Q<Toggle>("NameToggle");
            Toggle pathToggle = root.Q<Toggle>("PathToggle");
            Toggle indvPathToggle = root.Q<Toggle>("IndividualPathToggle");

            m_MasterContainer = root.Q<VisualElement>("MasterContainer");
            m_AssetNameField = root.Q<TextField>("AssetName");
            m_CopyNameToElem = root.Q<VisualElement>("CopyNameTo");
            // lazy solution. Should be set to be a FolderPathSelection

            m_RootFolderPath = m_RootFolderPathSelection.Q<TextField>("PathTextField");

            Button createAssetsButton = root.Q<Button>("CreateButton");
            #endregion


            #region Register Event callbacks for Elements
            nameToggle.RegisterValueChangedCallback(NameToggled);
            pathToggle.RegisterValueChangedCallback(DefaultPathToggled);
            indvPathToggle.RegisterValueChangedCallback(IndividualPathsToggled);

            createAssetsButton.RegisterCallback<ClickEvent>(CreateAssets);

            m_RootFolderPath.RegisterCallback<InputEvent>(UpdateAllPathNames);
            m_AssetNameField.RegisterCallback<InputEvent>(UpdateAllNames);

            // Register all CopyTextToElement children's events
            foreach (VisualElement child in m_CopyNameToElem.Children())
            {
                if (child is TextField)
                {
                    var textField = child as TextField;
                    m_UniformNameFields.Add(textField);

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

            m_FolderPathFields = m_MasterContainer.GetChildrenOfType<FolderPathSelection>();

            #endregion
        }

        private void CreateAssets(ClickEvent evt)
        {
            TextField uxmlField = new TextField();
            TextField scriptField = new TextField();
            TextField editorField = new TextField();

            foreach (TextField child in m_UniformNameFields)
            {
                switch (child.name)
                {
                    case "UXMLName":
                        uxmlField = child;
                        break;

                    case "ScriptName":
                        scriptField = child;
                        break;

                    case "EditorName":
                        editorField = child;
                        break;
                }
            }


            // If not using individual creation
            CreateLockableObject.CreateLockableAsset(PlaceholderIfEmpty(m_AssetNameField));

            CreateLockableObject.CreateLockableScript(PlaceholderIfEmpty(scriptField), m_RootFolderPathSelection.CurrentPath);

            CreateLockableObject.CreateLockableEditorScript(PlaceholderIfEmpty(editorField), m_RootFolderPathSelection.CurrentPath);

            CreateLockableObject.CreateLockableUXMLDoc(PlaceholderIfEmpty(uxmlField), m_RootFolderPathSelection.CurrentPath + "/UI" + "/UXML");
        }

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

                if (elem.name == m_ScriptName || elem.name == m_EditorName)
                {
                    elem.value += ".cs";
                    continue;
                }

                elem.value += ".uxml";
            }
        }


        private void UpdateAllPathNames(InputEvent evt)
        {
            UpdateAllPathNames(evt.newData);
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

                var newPath = path + "/";

                // Script folder
                if (elem.name == m_ScriptPathName)
                {
                    elem.TextField.value = newPath + "Scripts";
                    continue;
                }

                // Editor script folder
                if (elem.name == m_EditorPathName)
                {
                    elem.TextField.value = newPath + "Scripts/Editor";
                    continue;
                }

                //Uxml folder
                elem.TextField.value = newPath + "Scripts/UI/UXML";
            }
        }

        private void NameToggled(ChangeEvent<bool> evt)
        {

            foreach (TextField textField in m_UniformNameFields)
            {
                textField.SetEnabled(!evt.newValue);
            }

            //m_CopyNameToElem.SetEnabled( !evt.newValue);

            if (evt.newValue)
            {
                m_AssetNameField.RegisterCallback<InputEvent>(UpdateAllNames);
                return;
            }

            m_AssetNameField.UnregisterCallback<InputEvent>(UpdateAllNames);
        }


        private void DefaultPathToggled(ChangeEvent<bool> evt)
        {
            var pathElem = rootVisualElement.Q<FolderPathSelection>(m_RootFolderPathName);
            var revealElem = rootVisualElement.Q<Toggle>("IndividualPathToggle");

            pathElem.SetEnabled(!evt.newValue);
            revealElem.SetEnabled(!evt.newValue);

            // Set value of reveal toggle to 'false' if set to use DefaultPath
            if (evt.newValue == true)
            {
                UpdateAllPathNames(m_RootFolderPath.textEdition.placeholder); // default root folder
                revealElem.value = false;
                return;
            }

            UpdateAllPathNames(m_RootFolderPath.value); // custom root folder
        }

        private void IndividualPathsToggled(ChangeEvent<bool> evt)
        {

            foreach (FolderPathSelection folderPath in m_FolderPathFields)
            {
                if (folderPath.name == m_RootFolderPathName)
                {
                    folderPath.SetEnabled(!evt.newValue);
                    continue;
                }

                folderPath.SetEnabled(evt.newValue);
            }
        }

    }

}