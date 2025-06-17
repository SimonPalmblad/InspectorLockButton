using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.EditorTools;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine.Playables;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting.Dependencies.NCalc;

public static class LockTargetStyle
{
    public static Color ActiveColor = new Color(0.79f, 0.54f, 0.10f, 1f);
    public static Color DisabledColor = new Color(0.474f, 89f, 0.8f, 0.2f );

    public static float ActiveBorderWidth = 4f;
    public static float DisabledBorderWidth = 2f;
    public static float BorderRadius = 6f;

    public static float ElementPadding = 4f;
    
}

[UxmlElement]
public partial class EditorLock : VisualElement
{
    private SerializedProperty m_EditorLockedProp;

    private SerializedObject m_serializedObject;
    private string m_ButtonElemName = "LockElem";


    private Texture2D lockedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Inspector Editor Lock/UI/Icons/locked.png");
    private Texture2D unlockedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Inspector Editor Lock/UI/Icons/unlocked.png");

    public EditorLock() 
    { 
        Init();
    }

    [UxmlAttribute]
    public string elementToLock { get; set; }
    
    [UxmlAttribute]
    public VisualTreeAsset visualTree;

    protected VisualElement m_LockTargetElements;
    public Button m_Button => this.Q<Button>("LockButton");
    

    private void Init()
    {
        if (visualTree == null)
        {
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Inspector Editor Lock/UI/UXML/EditorLockButton.uxml");
        }

        visualTree.CloneTree(this);
        m_Button.RegisterCallback<ClickEvent>(ButtonClicked);
    }

    public void Init(SerializedObject serializedObj, SerializedProperty editorLockedProperty)
    {
        m_serializedObject = serializedObj;
        m_EditorLockedProp = editorLockedProperty;
        ToggleLockEvents();
        InitializeStyle();
    }

    public void ButtonClicked(ClickEvent evt)
    {
        if(m_EditorLockedProp == null)
        {
            Debug.LogWarning($"No valid SerializedProperty found on {this.name}. Initialize button with Init(args) in Editor class.");
            return;
        }

        m_EditorLockedProp.boolValue = !m_EditorLockedProp.boolValue;
        ToggleLockEvents();
    }

    public void ToggleLockEvents()
    {
        if(m_LockTargetElements == null)
        {
            m_LockTargetElements = this.Q<VisualElement>(elementToLock);
        }

        ToggleButtonIcon();
        ToggleLockedStyle(m_LockTargetElements);
        ToggleLockedElements();
        
        m_serializedObject.ApplyModifiedProperties();
    }

    public void ToggleLockedStyle(VisualElement visualElement)
    {
        #region Set padding
        visualElement.style.paddingBottom = LockTargetStyle.ElementPadding;
        visualElement.style.paddingTop = LockTargetStyle.ElementPadding;
        visualElement.style.paddingLeft = LockTargetStyle.ElementPadding;
        visualElement.style.paddingRight = LockTargetStyle.ElementPadding; 
        #endregion

        #region Set border color
        Color borderColor = m_EditorLockedProp.boolValue ? LockTargetStyle.ActiveColor
                                   : LockTargetStyle.DisabledColor;

        visualElement.style.borderBottomColor = borderColor;
        visualElement.style.borderTopColor = borderColor;
        visualElement.style.borderLeftColor = borderColor;
        visualElement.style.borderRightColor = borderColor; 
        #endregion

        #region Set border width

        //float borderWidth = m_EditorLocked ? EditorLockBorderStyle.ActiveBorderWidth
        //                                   : EditorLockBorderStyle.DisabledBorderWidth;

        visualElement.style.borderBottomWidth = LockTargetStyle.ActiveBorderWidth;
        visualElement.style.borderTopWidth = LockTargetStyle.ActiveBorderWidth;
        visualElement.style.borderLeftWidth = LockTargetStyle.ActiveBorderWidth;
        visualElement.style.borderRightWidth = LockTargetStyle.ActiveBorderWidth; 
        #endregion

        #region Set border radius
        visualElement.style.borderBottomLeftRadius = LockTargetStyle.BorderRadius;
        visualElement.style.borderBottomRightRadius = LockTargetStyle.BorderRadius;
        visualElement.style.borderTopLeftRadius = LockTargetStyle.BorderRadius;
        visualElement.style.borderTopRightRadius = 0f;
        #endregion

        m_Button.style.backgroundColor = borderColor;
    }

    private void ToggleLockedElements()
    {
        List<VisualElement> readOnlyElements = GetElementsOfReferenceType(this, VisualElementReadOnly.ValidTypes);
        List<VisualElement> childElements = GetChildElements(this, ignoreLockButton: true);

        //foreach (VisualElement elem in readOnlyElements)
        //{
        //    elem.GetType().GetProperty("isReadOnly").SetValue(elem, m_EditorLockedProp.boolValue, null);
        //}

        // For some reason this logic is working backwards. Probably my stupid ass not braining.s
        // It's supposed to set picking mode to ignore when locked = true.
        foreach (VisualElement elem in childElements)
        {
            if (m_EditorLockedProp.boolValue)
            {
                elem.SetEnabled(false);
                elem.pickingMode = PickingMode.Position;
                continue;
            }

            elem.SetEnabled(true);

            elem.pickingMode = PickingMode.Ignore;          
        }

    }

    public List<VisualElement> GetElementsOfReferenceType(VisualElement elem, Type[] referenceTypes)
    {
        var results = new List<VisualElement>();


        if (referenceTypes.Contains(elem.GetType())){
            results.Add(elem);
        }

        if(elem.childCount > 0)
        {
            foreach (VisualElement child in elem.Children())
            {
                results.AddRange(GetElementsOfReferenceType(child, referenceTypes));
            }
        }
        return results;
    }


    public List<VisualElement> GetChildElements(VisualElement elem, bool ignoreLockButton = true)
    {
        var results = new List<VisualElement>();

        if (elem.childCount > 0)
        {
            foreach (VisualElement child in elem.Children())
            {
                if(child.name == m_ButtonElemName)
                {
                    continue;
                }

                results.AddRange(GetChildElements(child, ignoreLockButton));
            }
        }

        else
        {
            results.Add(elem);
        }

        return results;
    }

    private void ToggleButtonIcon()
    {
        m_Button.iconImage = m_EditorLockedProp.boolValue ? m_Button.iconImage = lockedIcon
                                                        : m_Button.iconImage = unlockedIcon;
    }

    private void InitializeStyle()
    {
        if (m_LockTargetElements == null)
        {
            m_LockTargetElements = this.Q<VisualElement>(elementToLock);
        }

        //m_LockTargetElements.style.backgroundColor = 
    }

}
