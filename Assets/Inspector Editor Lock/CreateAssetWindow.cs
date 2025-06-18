using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

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


public class CreateAssetWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement m_CopyTextToElem;
    private List<TextField> m_UniformNameFields = new List<TextField>();

    private TextField m_AssetNameField;

    private Label m_FilePathElement;

    [MenuItem("EditorLock/Show Window")]
    public static void ShowExample()
    {
        CreateAssetWindow wnd = GetWindow<CreateAssetWindow>();
        wnd.titleContent = new GUIContent("CreateAssetWindow");
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

        m_AssetNameField = root.Q<TextField>("AssetName");
        m_CopyTextToElem = root.Q<VisualElement>("CopyNameTo");
        // lazy solution
        m_FilePathElement = (Label)root.Q<VisualElement>("PathString").Children()
                                                                      .Where(x => x is Label)
                                                                      .First();

        Button createAssetsButton = root.Q<Button>("CreateButton");
        #endregion


        #region Register Event callbacks for Elements
        nameToggle.RegisterValueChangedCallback(NameToggled);
        pathToggle.RegisterValueChangedCallback(PathToggled);

        createAssetsButton.RegisterCallback<ClickEvent>(CreateAssets);

        m_AssetNameField.RegisterCallback<InputEvent>(UpdateAllNames);

        // Register all CopyTextToElement children's events
        foreach (TextField child in m_CopyTextToElem.Children())
        {
            m_UniformNameFields.Add(child);

            if (child.name == "ScriptName" || child.name == "EditorName")
            {
                var scriptData = new TextFieldData(child, ".cs");
                child.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOnFocusedOut, scriptData);

                continue;
            }

            var uxlmData = new TextFieldData(child, ".uxml");
            child.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOnFocusedOut, uxlmData);
        } 
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

        CreateLockableObject.CreateLockableAsset(PlaceholderIfEmpty(m_AssetNameField));
        CreateLockableObject.CreateLockableScript(PlaceholderIfEmpty(scriptField), m_FilePathElement.text);
        CreateLockableObject.CreateLockableEditorScript(PlaceholderIfEmpty(editorField), m_FilePathElement.text);
        CreateLockableObject.CreateLockableUXMLDoc(PlaceholderIfEmpty(editorField), m_FilePathElement.text + "/UI" + "/UXML");
        //Debug.Log("Button Clicked");
        //CreateNewLockable.CreateLockableScript();
    }

    private string PlaceholderIfEmpty(TextField textField)
    {
        if(textField.text == string.Empty)
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
            if(evt.newData == "")
            {
                elem.value = elem.textEdition.placeholder;
                continue;
            }
            
            elem.value = evt.newData;
            
            if(elem.name == "ScriptName" || elem.name == "EditorName")
            {
                elem.value += ".cs";
                continue;
            }

            elem.value += ".uxml";          
        }
    }

    private void NameToggled(ChangeEvent<bool> evt)
    {
        m_CopyTextToElem.SetEnabled(!evt.newValue);

        if (evt.newValue)
        {
            m_AssetNameField.RegisterCallback<InputEvent>(UpdateAllNames);
            return;
        }

        m_AssetNameField.UnregisterCallback<InputEvent>(UpdateAllNames);

    }


    private void PathToggled(ChangeEvent<bool> evt)
    {
        var pathElem = rootVisualElement.Q<VisualElement>("PathElem");
        var revealElem = rootVisualElement.Q<VisualElement>("PathToggleRevealElem");

        pathElem.SetEnabled(!evt.newValue);
        // Enable and disable foldout element
        revealElem.style.display = evt.newValue ? DisplayStyle.None 
                                                   : DisplayStyle.Flex;
        
        // Set value of reveal toggle to 'false' if set to use DefaultPath
        if(evt.newValue == true)
        {
            revealElem.Children().OfType<Toggle>().First().value = false;
        }
    }

}
