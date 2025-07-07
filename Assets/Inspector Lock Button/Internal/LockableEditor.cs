using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLockUtilies;
using JetBrains.Annotations;

namespace EditorLock
{
    public abstract class LockableEditor<T> : Editor where T : UnityEngine.Object
    {
        public VisualTreeAsset VisualTree = null;
        protected SerializedProperty m_EditorLockedProps;

        protected virtual VisualTreeAsset VisualTreePath => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Inspector Lock Button/UI/UXML/LockableUXMLTemplate.uxml");
        protected virtual VisualTreeAsset GetVisualTree => VisualTree == null ? VisualTreePath
                                                                              : VisualTree;

        // TODO Fix this. It's prone to errors as a lot of editor scripts want to override OnEnable
        protected void OnEnable()
        {
            InitializeLockableEditor();
        }

        protected void InitializeLockableEditor()
        {
            m_EditorLockedProps = EditorLockUtility.OnEnable<T>(GetVisualTree, target as T);
        }

        public override VisualElement CreateInspectorGUI()
        {
            //if (GetVisualTree == null)
            //{
            //    return base.CreateInspectorGUI();
            //}

            VisualElement root = new VisualElement();
            GetVisualTree.CloneTree(root);

            // create an array of bools equal to the number of locks attached to this UI element
            EditorLockUtility.InitializeLocks(root, serializedObject, m_EditorLockedProps);

            return root;
        }
    }
}
