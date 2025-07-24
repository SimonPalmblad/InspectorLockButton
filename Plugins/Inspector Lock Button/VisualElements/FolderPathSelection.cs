using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;
using UnityEngine.Events;
using InspectorLock.Internals;

namespace InspectorLock
{
    [UxmlElement]
    public partial class FolderPathSelection : VisualElement
    {
        private VisualTreeAsset visualTree;

        [UxmlAttribute]
        public string description { get; set; } = "Folder Path";

        [UxmlAttribute]
        public string prefix { get; set; } = string.Empty;

        [UxmlAttribute]
        public string defaultPath { get; set; } = "Assets/Scripts";   

        private string m_SetPathButtonName = "PathButton";
        private string m_TextFieldName = "PathTextField";
        private string m_DescriptionName = "Description";
        private string m_PrefixName = "Prefix";


        private TextField m_TextField;
        private Label m_Description;
        private Label m_Prefix;

        public UnityEvent FolderSelectionUpdated;

        public TextField TextField => m_TextField;
        public string DefaultPath => defaultPath;
        
        public string PathFull => m_TextField.value != string.Empty ? m_Prefix.text + m_TextField.value 
                                                                    : m_Prefix.text + defaultPath;
        
        public string PathRaw => m_TextField.value != string.Empty ? m_TextField.value : defaultPath;

        public FolderPathSelection()
        {
            Init();
        }

        public void SetPathBinding(SerializedObject pathBindingObject, string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                Debug.LogWarning($"No property path on {pathBindingObject.FindProperty("m_Name")} found when Initializing FolderPathSelection");
                return;
            }

            m_TextField.bindingPath = propertyPath;
            m_TextField.Bind(pathBindingObject);
        }

        public void SetPrefix(string newPrefix)
        {
            prefix = newPrefix;
            LateUXMLAttributeUpdate();
        }
        
        public void SetDescription(string newDescription)
        {
            description = newDescription;
            LateUXMLAttributeUpdate();
        }
        
        public void SetDefaultPath(string newPath)       
        {
              defaultPath = newPath;
            LateUXMLAttributeUpdate();
        }
  

        private void Init()
        {
            if (visualTree == null)
            {
                visualTree = InternalAssetReferences.Instance.UxmlFolderPathSelectionTree;
            }

            visualTree.CloneTree(this);

            m_Description = this.Q<Label>(m_DescriptionName);
            m_Prefix = this.Q<Label>(m_PrefixName);
            m_TextField = this.Q<TextField>(m_TextFieldName);

            m_TextField.RegisterValueChangedCallback(RemovePrefixWhenTyping);

            this.Q<Button>(m_SetPathButtonName)
                .RegisterCallback<ClickEvent>(ShowFolderDialogue);

            this.schedule.Execute(LateUXMLAttributeUpdate).ExecuteLater(10);
        }

        /// <summary>
        /// Wait before updating any UXML attributes because their values are set later than Init();
        /// </summary>
        private void LateUXMLAttributeUpdate()
        {
            if (m_TextField == null)
            {
                Debug.LogWarning("FolderPathSelection TextField not found.");
            }

    
            m_Description.text = description;
            m_TextField.textEdition.placeholder = defaultPath;
            
            if (string.IsNullOrEmpty(prefix))
            {
                m_Prefix.style.display = DisplayStyle.None;
                return;
            }

            m_Prefix.text = prefix;
        }

        private void RemovePrefixWhenTyping(ChangeEvent<string> evt)
        {
            m_TextField.value = RemovePrefixFromString(evt.newValue);

        }

        private void ShowFolderDialogue(ClickEvent evt)
        {
            SetFolderPathFromDialogue();
        }

        private void SetFolderPathFromDialogue()
        {
            var removeStringFromLocal = "Assets";
            string chosenFolder = EditorUtility.OpenFolderPanel("Select file location", "", "");

            if (string.IsNullOrEmpty(chosenFolder))
            {
                return;
            }

            var localFolder = Application.dataPath;
            Debug.Log($"Local path: {localFolder}");

            if (localFolder.EndsWith(removeStringFromLocal))
            {
                Debug.Log($"Reduntand 'Assets/' path removed from new folder path. Resulting path: {localFolder}");
                localFolder = localFolder.Substring(0, localFolder.Length - removeStringFromLocal.Length);
            }

            Debug.Log($"New folder set to: {chosenFolder}");

            chosenFolder = RemovePrefixFromString(chosenFolder.Substring(localFolder.Length));

            m_TextField.value = chosenFolder;
            
        }

        private string RemovePrefixFromString(string text)
        {
            if (text.StartsWith(prefix))
            {
                return text.Substring(prefix.Length);
            }

           return text;
        }


    } 
}
