using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public VisualTreeAsset VisualTree;

    private SerializedProperty m_InventoryIconProp;
    private SerializedProperty m_ItemNameProp;
    private SerializedProperty m_EditorLockedProp;
    
    private VisualElement m_ItemPreviewElem;
    private VisualElement m_ItemIconElem;
    private VisualElement m_PreviousButton;
    private Label m_ItemNameElem;

    private Action<SerializedProperty> updateItemIcon;


    private void OnEnable()
    {

        m_InventoryIconProp = serializedObject.FindProperty("m_InventoryIcon");
        m_ItemNameProp = serializedObject.FindProperty("m_Name");
        m_EditorLockedProp = serializedObject.FindProperty("m_EditorLocked");
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);


        m_ItemPreviewElem = root.Q<VisualElement>("IconPreview");
        m_ItemIconElem = root.Q<VisualElement>("IconProp");
        m_ItemNameElem = root.Q<Label>("SectionLabel");
        m_PreviousButton = root.Q<Button>("PreviousButton");
        
        root.Q<EditorLock>().Init(serializedObject, m_EditorLockedProp);

        m_PreviousButton.RegisterCallback<ClickEvent>(PreviousSelection);

        m_ItemNameElem.text = (m_ItemNameProp.stringValue).ToUpper() + " (item)";

        if (m_InventoryIconProp.objectReferenceValue != null)
        {
            m_ItemPreviewElem.style.backgroundImage = (Texture2D)m_InventoryIconProp.objectReferenceValue;
        }

        m_ItemIconElem.TrackPropertyValue(m_InventoryIconProp, updateItemIcon = x => m_ItemPreviewElem.style.backgroundImage = (Texture2D)x.objectReferenceValue);
        return root;
    }

    private void PreviousSelection(ClickEvent evt)
    {
        throw new NotImplementedException();
    }
}
