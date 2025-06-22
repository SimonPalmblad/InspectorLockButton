using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Runtime.InteropServices;
using UnityEditor.UIElements;

namespace EditorLock
{

    [UxmlElement]
    public partial class FolderPathSelection : VisualElement
    {
        private VisualTreeAsset visualTree;

        [UxmlAttribute]
        public string labelText { get; set; } = "Folder Path";

        [UxmlAttribute]
        public string defaultPath { get; set; } = "Assets/Scripts";

        [UxmlAttribute]
        public string bindToEditorProperty = string.Empty;        

        private string m_SetPathButtonName = "PathButton";
        private string m_TextFieldName = "PathTextField";
        private TextField m_Label;

        public TextField TextField => m_Label;
        public string DefaultPath => defaultPath;
        public string CurrentPath => m_Label.value;

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
                        
            m_Label = this.Q<TextField>(m_TextFieldName);
            this.Q<Button>(m_SetPathButtonName)
                .RegisterCallback<ClickEvent>(ShowFolderDialogue);

            this.schedule.Execute(LateUXMLAttributeUpdate).ExecuteLater(10);
        }

        public void Init(SerializedObject pathBindingObject)
        {
            Init();
            if (bindToEditorProperty == string.Empty)
            {
                return;
            }

            m_Label.bindingPath = bindToEditorProperty;
            m_Label.Bind(pathBindingObject);
        }

        /// <summary>
        /// Wait before updating any UXML attributes because their values are set later than Init();
        /// </summary>
        private void LateUXMLAttributeUpdate()
        {
            if (m_Label == null)
            {
                Debug.LogWarning("FolderPathSelection TextField not found.");
            }

            m_Label.label = labelText;
            m_Label.textEdition.placeholder = defaultPath;
        }

        private void ShowFolderDialogue(ClickEvent evt)
        {
            Debug.Log(defaultPath);
            var removeStringFromLocal = "Assets";
            string chosenFolder = EditorUtility.OpenFolderPanel("Select file location", "", "");

            var localFolder = Application.dataPath;
            Debug.Log($"Local path: {localFolder}");

            if (localFolder.EndsWith(removeStringFromLocal))
            {
                localFolder = localFolder.Substring(0, localFolder.Length - removeStringFromLocal.Length);
            }

            Debug.Log($"Asset folder removed from path: {localFolder}");
            Debug.Log($"New folder set to: {chosenFolder}");
            m_Label.value = chosenFolder.Substring(localFolder.Length);
        }


    } 
}
