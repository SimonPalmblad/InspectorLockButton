using UnityEngine;
using UnityEditor;
using EditorLock;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace Editorlock
{
    [CustomEditor( typeof(EditorLockSettings) )]
    public class EditorLockSettingsEditor: LockableEditor<EditorLockSettings>
    {
        SerializedProperty m_UlockedColProp;
        SerializedProperty m_LockedColProp;

        public override VisualElement CreateInspectorGUI()
        {
            var root = base.CreateInspectorGUI();
            m_UlockedColProp = serializedObject.FindProperty("UnlockedColor");
            m_LockedColProp = serializedObject.FindProperty("LockedColor");

            root.Q<ColorField>("UnlockedCol").RegisterValueChangedCallback(UnlockedColorChange);
            root.Q<ColorField>("LockedCol").RegisterValueChangedCallback(LockedColorChange);

            return root;
        }

        private void UnlockedColorChange(ChangeEvent<Color> evt)
        {
            ColorChange(evt, m_UlockedColProp);
        }

        private void LockedColorChange(ChangeEvent<Color> evt)
        {
            ColorChange(evt, m_LockedColProp);
        }

        private void ColorChange(ChangeEvent<Color> evt, SerializedProperty property)
        {
            if(property == null)
            {
                Debug.LogWarning("Serialized property for color change was null.");
                return;
            }
            
            property.colorValue = evt.newValue;

        }
    }
}
