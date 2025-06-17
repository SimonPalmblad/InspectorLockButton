using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLockUtilies;

[CustomEditor(typeof(TestObject))]
public class TestObjectEditor : Editor
{
    public VisualTreeAsset VisualTree;
    public SerializedProperty m_EditorLockedProps;

    private void OnEnable()
    {
        m_EditorLockedProps = EditorLockUtility.OnEnable<TestObject>(VisualTree, target as TestObject);
        
        //if (VisualTree == null)
        //{
        //    Debug.LogWarning($"No Visual Tree Asset found on {target.name} Editor Script. Drawing default GUI.");
        //    return;
        //}

        ////// find the bool that controls lock states
        ////if(target is not IEditorLockable)
        ////{
        ////    Debug.LogWarning($"Interface IEditorLockable not found on {target.name}. Implement the inferface on the object's script.");
        ////    return;
        ////}

        // m_EditorLockedProps = serializedObject.FindProperty(nameof(TestObject.m_EditorLockStates));
    }

    public override VisualElement CreateInspectorGUI()
    {
        if (VisualTree == null)
        {
            return base.CreateInspectorGUI();
        }

        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);

        
        // create an array of bools requal to the number of locks attached to this UI element
        EditorLockUtility.InitializeLocks(root, serializedObject, m_EditorLockedProps);

        return root;
    }
}