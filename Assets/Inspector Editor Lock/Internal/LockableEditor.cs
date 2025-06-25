using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLockUtilies;
using JetBrains.Annotations;

namespace EditorLock
{

    public abstract class LockableEditor<T> : Editor where T : UnityEngine.Object
    {
        public UnityEngine.UIElements.VisualTreeAsset VisualTree;
        private SerializedProperty m_EditorLockedProps;

        // this is prone to errors as any editor will want to override OnEnable

        protected void OnEnable()
        {
            InitializeLockableEditor();
        }

        protected void InitializeLockableEditor()
        {
            m_EditorLockedProps = EditorLockUtility.OnEnable<T>(VisualTree, target as T);
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
}
