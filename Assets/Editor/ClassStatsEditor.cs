using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Runtime;
using System.Linq;
using EditorLock;

[CustomEditor(typeof(ClassStats))]
public class ClassStatsEditor : Editor
{
    public VisualTreeAsset VisualTree;
    private SerializedProperty m_EditingLockedProp;


    private void OnEnable()
    {
        m_EditingLockedProp = serializedObject.FindProperty("m_EditingLocked");
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);

        root.Q<EditorLockElement>().Init(serializedObject, m_EditingLockedProp);

        
        return root;
    }

  
}
