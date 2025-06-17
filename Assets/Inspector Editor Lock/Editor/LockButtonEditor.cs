using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using EditorLockUtilies;

[CustomEditor(typeof(LockButtonTest))]
public class LockButtonEditor : Editor
{
    public VisualTreeAsset VisualTree;
    public SerializedProperty m_EditorLockedProps;

    private void OnEnable()
    {
        m_EditorLockedProps = serializedObject.FindProperty(nameof(LockButtonTest.m_EditorLocks));
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);

        EditorLockUtility.InitializeLocks(root, serializedObject, m_EditorLockedProps);

        return root;
    }
}