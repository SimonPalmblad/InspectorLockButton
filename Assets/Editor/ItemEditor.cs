using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using EditorLock;
using EditorLockUtilies;

[CustomEditor(typeof(Item))]
public class ItemEditor : LockableEditor<Item>
{
    //public VisualTreeAsset VisualTree;

    private SerializedProperty m_InventoryIconProp;
    private SerializedProperty m_ItemNameProp;
    
    private VisualElement m_ItemPreviewElem;
    private VisualElement m_ItemIconElem;
    private VisualElement m_PreviousButton;
    private Label m_ItemNameElem;

    private Action<SerializedProperty> updateItemIcon;

    private new void OnEnable()
    {
        InitializeLockableEditor();
        m_InventoryIconProp = serializedObject.FindProperty("m_InventoryIcon");
        m_ItemNameProp = serializedObject.FindProperty("m_Name");
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = base.CreateInspectorGUI();

        m_ItemPreviewElem = root.Q<VisualElement>("IconPreview");
        m_ItemIconElem = root.Q<VisualElement>("IconProp");
        m_ItemNameElem = root.Q<Label>("SectionLabel");
        m_PreviousButton = root.Q<Button>("PreviousButton");
        
            m_ItemNameElem.text = (m_ItemNameProp.stringValue).ToUpper() + " (item)";

        if (m_InventoryIconProp.objectReferenceValue != null)
        {
            m_ItemPreviewElem.style.backgroundImage = (Texture2D)m_InventoryIconProp.objectReferenceValue;
        }

        m_ItemIconElem.TrackPropertyValue(m_InventoryIconProp, updateItemIcon = x => m_ItemPreviewElem.style.backgroundImage = (Texture2D)x.objectReferenceValue);
        return root;
    }
}
