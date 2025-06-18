using System;
using System.Collections.Generic;
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

        Toggle nameToggle = root.Q<Toggle>("NameToggle");
        Toggle pathToggle = root.Q<Toggle>("PathToggle");

        m_AssetNameField = root.Q<TextField>("AssetName");

        m_CopyTextToElem = root.Q<VisualElement>("CopyNameTo");

        Button createAssetsButton = root.Q<Button>("CreateButton");

        nameToggle.RegisterValueChangedCallback(NameToggled);
        pathToggle.RegisterValueChangedCallback(PathToggled);

        createAssetsButton.RegisterCallback<ClickEvent>(CreateAssets);

        m_AssetNameField.RegisterCallback<InputEvent>(UpdateAllNames);

        foreach (TextField child in m_CopyTextToElem.Children())
        { 
            m_UniformNameFields.Add(child);
            
            if (child.name == "ScriptName" || child.name == "EditorName")
            {
                var scriptData = new TextFieldData(child, ".cs");
                child.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOutFocused, scriptData);

                continue;
            }

            var uxlmData = new TextFieldData(child, ".uxml");
            child.RegisterCallback<FocusOutEvent, TextFieldData>(TextFieldOutFocused, uxlmData);
        }
    }

    private void CreateAssets(ClickEvent evt)
    {
        //Debug.Log("Button Clicked");
        //CreateNewLockable.CreateLockableScript();
    }

    private void TextFieldOutFocused(FocusOutEvent evt, TextFieldData data)
    {
        if (data.TextField.value == string.Empty)
        { 
            return;
        }

        if (data.TextField.value.EndsWith(data.FileEnding))
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
        pathElem.SetEnabled(!evt.newValue); 
    }

}
