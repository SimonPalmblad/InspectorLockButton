using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLockUtilies;

namespace EditorLock
{

    [CustomEditor(typeof(LockButtonTest))]
    public class LockButtonEditor : Editor
    {
        public VisualTreeAsset VisualTree;
        public SerializedProperty m_EditorLockedProps;

        private void OnEnable()
        {
            if (VisualTree == null)
            {
                Debug.Log($"No Visual Tree Asset present on {target.name}. Could not create custom Inspector with Editor Locks.");
                return;
            }

            // find the bool that controls lock states
            m_EditorLockedProps = serializedObject.FindProperty(nameof(LockButtonTest.m_EditorLocks));
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            if (VisualTree == null)
            {
                Debug.Log($"Drawing default GUI for {target.name}.");
                base.CreateInspectorGUI();
            }

            else
            {
                VisualTree.CloneTree(root);

                // create an array of bools requal to the number of locks attached to this UI element
                EditorLockUtility.InitializeLocks(root, serializedObject, m_EditorLockedProps);
            }

            return root;
        }
    } 
}