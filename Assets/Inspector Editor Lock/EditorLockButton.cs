using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EditorLock
{   
    public static class LockTargetStyle
    {
        public static StyleSheet lockedStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Inspector Editor Lock/UI/USS/LockedButton.uss");
        public static StyleSheet unlockedStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Inspector Editor Lock/UI/USS/UnlockedButton.uss");

        public static StyleSheet GetLockedStyle => lockedStyle;
        public static StyleSheet GetUnlockedStyle => lockedStyle;

        // Get Lock style serialized object
        public static Color UnlockedColor = new Color(0.79f, 0.54f, 0.10f, 1f);
        public static Color LockedColor = new Color(0.474f, 89f, 0.8f, 0.2f);

        public static float DisabledOpacity = 0.85f;
        public static float EnabledOpacity = 1f;

        public static float ActiveBorderWidth = 4f;
        public static float DisabledBorderWidth = 2f;
        public static float BorderRadius = 6f;

        public static float ElementPadding = 4f;        
    }

    [UxmlElement]
    public partial class EditorLockElement : VisualElement
    {
        private SerializedProperty m_EditorLockedProp;
        private SerializedObject m_serializedObject;

        private string m_LockElemName = "LockElem";
        private string m_LockButtonName = "LockButton";

        private List<string> elementsToIgnore;

        private Texture2D m_LockedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Inspector Editor Lock/UI/Icons/locked.png");
        private Texture2D m_UnlockedIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Inspector Editor Lock/UI/Icons/unlocked.png");

        public EditorLockElement()
        {
            Init();
        }

        [UxmlAttribute]
        public float topMargin { get; set; } = 2f;

        [UxmlAttribute]
        public VisualTreeAsset visualTree;

        protected VisualElement m_LockTargetElements;

        public VisualElement LockElement;
        public Button LockButton;


        private void Init()
        {
            if (visualTree == null)
            {
                visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Inspector Editor Lock/UI/UXML/EditorLockButton.uxml");
            }

            elementsToIgnore = new List<string>() { m_LockElemName, m_LockButtonName };

            visualTree.CloneTree(this);
           

            LockButton = this.Q<Button>(m_LockButtonName);
            LockButton.RegisterCallback<ClickEvent>(ButtonClicked);

            this.schedule.Execute(LateUXMLAttributeUpdate).ExecuteLater(10);

        }

        private void LateUXMLAttributeUpdate()
        {
            this.style.marginTop = topMargin;
        }


        /// <summary>
        /// Initialize an Editor Lock with values that allows the lock state to be stored.
        /// </summary>
        /// <param name="serializedObj">The object that has this lock attached to it.</param>
        /// <param name="editorLockedProperty">A bool on the object that stores this lock's state.</param>
        public void Init(SerializedObject serializedObj, SerializedProperty editorLockedProperty)
        {
            m_serializedObject = serializedObj;
            m_EditorLockedProp = editorLockedProperty;
            ToggleLockEvents();
        }

        public void ButtonClicked(ClickEvent evt)
        {
            if (m_EditorLockedProp == null)
            {
                Debug.LogWarning($"No valid SerializedProperty found on this object. Make sure this object's Editor script inherits from LockableEditor.");
                return;
            }

            m_EditorLockedProp.boolValue = !m_EditorLockedProp.boolValue;
            ToggleLockEvents();
        }

        public void ToggleLockEvents()
        {
            ToggleButtonIcon();
            ToggleLockedStyle(this.Q<VisualElement>());
            ToggleLockedElements();

            m_serializedObject.ApplyModifiedProperties();
        }

        public void ToggleLockedStyle(VisualElement visualElement)
        {

            if (m_EditorLockedProp.boolValue)
            {
                this.Q<VisualElement>().AddToClassList("element-styling-locked");                
                this.Q<VisualElement>().RemoveFromClassList("element-styling-unlocked");
                
                LockButton.AddToClassList("button-styling-locked");
                LockButton.RemoveFromClassList("button-styling-unlocked");
            }

            else
            {
                this.Q<VisualElement>().AddToClassList("element-styling-unlocked");
                this.Q<VisualElement>().RemoveFromClassList("element-styling-locked");

                LockButton.AddToClassList("button-styling-unlocked");
                LockButton.RemoveFromClassList("button-styling-locked");
            }
            //this.Q<VisualElement>().AddToClassList("element-styling-locked");
            //this.Q<VisualElement>().ToggleInClassList("element-styling-locked");
            //this.Q<VisualElement>().ToggleInClassList("element-styling-unlocked");

            //#region Set padding
            //visualElement.style.paddingBottom = LockTargetStyle.ElementPadding;
            //visualElement.style.paddingTop = LockTargetStyle.ElementPadding;
            //visualElement.style.paddingLeft = LockTargetStyle.ElementPadding;
            //visualElement.style.paddingRight = LockTargetStyle.ElementPadding;
            //#endregion

            //#region Set border color
            //Color borderColor = m_EditorLockedProp.boolValue ? LockTargetStyle.UnlockedColor
            //                           : LockTargetStyle.LockedColor;

            //visualElement.style.borderBottomColor = borderColor;
            //visualElement.style.borderTopColor = borderColor;
            //visualElement.style.borderLeftColor = borderColor;
            //visualElement.style.borderRightColor = borderColor;
            //#endregion

            //#region Set border width

            //visualElement.style.borderBottomWidth = LockTargetStyle.ActiveBorderWidth;
            //visualElement.style.borderTopWidth = LockTargetStyle.ActiveBorderWidth;
            //visualElement.style.borderLeftWidth = LockTargetStyle.ActiveBorderWidth;
            //visualElement.style.borderRightWidth = LockTargetStyle.ActiveBorderWidth;
            //#endregion

            //#region Set border radius
            //visualElement.style.borderBottomLeftRadius = LockTargetStyle.BorderRadius;
            //visualElement.style.borderBottomRightRadius = LockTargetStyle.BorderRadius;
            //visualElement.style.borderTopLeftRadius = LockTargetStyle.BorderRadius;

            //visualElement.style.borderTopRightRadius = 0f;
            //#endregion

            //LockButton.style.backgroundColor = borderColor;
        }

        private void ToggleLockedElements()
        {
            List<VisualElement> childElements = new List<VisualElement>();

            // Exit early if no valid children are found
            if (!TryGetChildElements(this, out childElements))
            {
                return;
            }

            var disabled = m_EditorLockedProp.boolValue;

            foreach (VisualElement elem in childElements)
            {
                // Disable
                if (disabled)
                {
                    elem.style.opacity = LockTargetStyle.DisabledOpacity;
                    elem.SetEnabled(false);
                    continue;
                }

                elem.style.opacity = LockTargetStyle.EnabledOpacity;

                elem.SetEnabled(true);
            }
        }

        public List<VisualElement> GetElementsOfReferenceType(VisualElement elem, Type[] referenceTypes)
        {
            var results = new List<VisualElement>();

            if (referenceTypes.Contains(elem.GetType()))
            {
                results.Add(elem);
            }

            if (elem.childCount > 0)
            {
                foreach (VisualElement child in elem.Children())
                {
                    results.AddRange(GetElementsOfReferenceType(child, referenceTypes));
                }
            }
            return results;
        }


        public bool TryGetChildElements(VisualElement elem, out List<VisualElement> childElements)
        {
            var results = new List<VisualElement>();

            if (elem.childCount > 0)
            {
                //Debug.Log($"Found {elem.childCount} children.");
                foreach (VisualElement child in elem.Children())
                {
                    // Not very optimized using Linq
                    if (child.name == elementsToIgnore.Find(x => child.name == x))
                    {
                        continue;
                    }

                    results.Add(child);
                }
            }

            // No valid children found
            else
            {
                childElements = results;
                return false;
            }

            childElements = results;
            return true;
        }

        private void ToggleButtonIcon()
        {
            LockButton.iconImage = m_EditorLockedProp.boolValue ? LockButton.iconImage = m_LockedIcon
                                                            : LockButton.iconImage = m_UnlockedIcon;
        }
    }

}