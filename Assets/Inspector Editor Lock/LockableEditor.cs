using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLockUtilies;

public abstract class LockableEditor: Editor
{
    public VisualTreeAsset VisualTree;
    public SerializedProperty m_EditorLockedProps;

    private void OnEnable()
    {
        m_EditorLockedProps = EditorLockUtility.OnEnable<TestObject>(VisualTree, target as TestObject);
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
