using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using InspectorLock.Internals;

namespace InspectorLock
{   
    [UxmlElement]
    public partial class InspectorLockElement : VisualElement
    {
        public InspectorLockElement()
        {
            Init();
        }

        private VisualTreeAsset m_VisualTree;
        private VisualElement m_Root;

        private SerializedProperty m_InspectorLockedProp;
        private SerializedObject m_serializedObject;

        private string m_LockElemName = "LockElem";
        private string m_LockButtonName = "LockButton";
        private string m_ScriptProperty = "PropertyField:m_Script";

        private List<string> m__ElementsToIgnore;

        private Texture2D m_LockedIcon = InternalAssetReferences.Instance.tex2DLockedIcon;
        private Texture2D m_UnlockedIcon = InternalAssetReferences.Instance.text2DUnlockedIcon;

        [UxmlAttribute]
        public float TopMargin { get; set; } = 2f;

        [UxmlAttribute]
        public bool DrawDefaultInspector = true;

        public VisualElement LockElement;
        public Button LockButton;


        private void Init()
        {
            if (m_VisualTree == null)
            {
                m_VisualTree = InternalAssetReferences.Instance.UxmlInspectorLockButtonTree;
            }

            m__ElementsToIgnore = new List<string>() { m_LockElemName, m_LockButtonName, m_ScriptProperty };

            m_VisualTree.CloneTree(this);

            m_Root = this.Q<VisualElement>();

            LockButton = this.Q<Button>(m_LockButtonName);
            LockButton.RegisterCallback<ClickEvent>(ButtonClicked);

            this.schedule.Execute(LateUXMLAttributeUpdate).ExecuteLater(2);

        }

        private void LateUXMLAttributeUpdate()
        {
            this.style.marginTop = TopMargin;
        }

        /// <summary>
        /// Initialize an Inspector Lock with values that allows the lock state to be stored.
        /// </summary>
        /// <param name="serializedObj">The object that has this lock attached to it.</param>
        /// <param name="inspectorLockedProperty">A bool on the object that stores this lock's state.</param>
        public void Init(SerializedObject serializedObj, SerializedProperty inspectorLockedProperty)
        {
            m_serializedObject = serializedObj;
            m_InspectorLockedProp = inspectorLockedProperty;  
            ToggleLockEvents();
        }

        public void ButtonClicked(ClickEvent evt)
        {
            if (m_InspectorLockedProp == null)
            {
                Debug.LogWarning($"No valid SerializedProperty found on this object. Make sure this object's Editor script inherits from LockableInspector.");
                return;
            }

            m_InspectorLockedProp.boolValue = !m_InspectorLockedProp.boolValue;
            ToggleLockEvents();
        }

        public void ToggleLockEvents()
        {
            ToggleButtonIcon();
            ToggleLockedStyle(m_Root);
            ToggleLockedElements();

            m_serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Toggles USS style sheets for this lock element.
        /// </summary>
        /// <param name="visualElement"></param>
        public void ToggleLockedStyle(VisualElement visualElement)
        {
            if (m_InspectorLockedProp.boolValue)
            {
                visualElement.AddToClassList("element-styling-locked");
                visualElement.RemoveFromClassList("element-styling-unlocked");

                LockButton.AddToClassList("button-styling-locked");
                LockButton.RemoveFromClassList("button-styling-unlocked");
            }

            else
            {
                visualElement.AddToClassList("element-styling-unlocked");
                visualElement.RemoveFromClassList("element-styling-locked");

                LockButton.AddToClassList("button-styling-unlocked");
                LockButton.RemoveFromClassList("button-styling-locked");
            }
        }

        /// <summary>
        /// Toggles UI elements inside the lock element as enabled/disabled depending on lock state.
        /// </summary>
        private void ToggleLockedElements()
        {
            List<VisualElement> childElements = new List<VisualElement>();

            // Exit early if no valid children are found
            if (!TryGetChildElements(this, out childElements))
            {
                return;
            }

            var disabled = m_InspectorLockedProp.boolValue;

            foreach (VisualElement elem in childElements)
            {
                // Disable
                if (disabled)
                {
                    elem.SetEnabled(false);
                    continue;
                }

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

        /// <summary>
        /// Attemps to find any child Visual Elements, excluding those listed in elementsToIgnore.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="childElements"></param>
        /// <returns></returns>
        public bool TryGetChildElements(VisualElement elem, out List<VisualElement> childElements)
        {
            var results = new List<VisualElement>();

            if (elem.childCount > 0)
            {
                //Debug.Log($"Found {elem.childCount} children.");
                foreach (VisualElement child in elem.Children())
                {
                    // Not very optimized using Linq
                    if (child.name == m__ElementsToIgnore.Find(x => child.name == x))
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

        /// <summary>
        /// Toggles the lock button icon depending on lock state.
        /// </summary>
        private void ToggleButtonIcon()
        {
            LockButton.iconImage = m_InspectorLockedProp.boolValue ? LockButton.iconImage = m_LockedIcon
                                                                   : LockButton.iconImage = m_UnlockedIcon;
        }
    }

}