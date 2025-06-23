using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;
using UnityEngine.Events;

namespace EditorLock
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

        [UxmlAttribute]
        public string bindToEditorProperty = string.Empty;        

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

        public FolderPathSelection(SerializedObject pathBindingObject)
        {
            Init(pathBindingObject);
        }

        private void Init()
        {
            if (visualTree == null)
            {
                visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Inspector Editor Lock/UI/UXML/FolderPathSelection.uxml");
            }

            visualTree.CloneTree(this);

            m_Description = this.Q<Label>(m_DescriptionName);
            m_Prefix = this.Q<Label>(m_PrefixName);
            m_TextField = this.Q<TextField>(m_TextFieldName);

            this.Q<Button>(m_SetPathButtonName)
                .RegisterCallback<ClickEvent>(ShowFolderDialogue);

            this.schedule.Execute(LateUXMLAttributeUpdate).ExecuteLater(10);
        }

        /// <summary>
        /// Initialize with path binding to update variable inside GameObject.
        /// </summary>
        /// <param name="pathBindingObject"></param>
        public void Init(SerializedObject pathBindingObject)
        {
            Init();
            if (bindToEditorProperty == string.Empty)
            {
                return;
            }

            m_TextField.bindingPath = bindToEditorProperty;
            m_TextField.Bind(pathBindingObject);
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

        private void ShowFolderDialogue(ClickEvent evt)
        {
            SetFolderPathFromDialogue();
        }

        private void SetFolderPathFromDialogue()
        {
            Debug.Log($"Default path: {defaultPath}");
            var removeStringFromLocal = "Assets";
            string chosenFolder = EditorUtility.OpenFolderPanel("Select file location", "", "");

            var localFolder = Application.dataPath;
            Debug.Log($"Local path: {localFolder}");

            if (localFolder.EndsWith(removeStringFromLocal))
            {
                localFolder = localFolder.Substring(0, localFolder.Length - removeStringFromLocal.Length);
            }

            Debug.Log($"Reduntand 'Assets/' path removed from new folder path. Resulting path: {localFolder}");
            Debug.Log($"New folder set to: {chosenFolder}");
            m_TextField.value = chosenFolder.Substring(localFolder.Length);
            
        }


    } 
}
