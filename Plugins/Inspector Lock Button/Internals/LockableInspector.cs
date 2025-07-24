using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using InspectorLock.Internals;


namespace InspectorLock
{
    public abstract class LockableInspector : Editor
    {
        public VisualTreeAsset VisualTree = null;
        [Tooltip("Draws the default inspector inside the Inspector Lock element.")]
        protected SerializedProperty m_InspectorLockedProps;

        protected virtual VisualTreeAsset VisualTreePath => InternalAssetReferences.Instance.UxmlLockableObjTemplateTree;
        protected virtual VisualTreeAsset GetVisualTree => VisualTree == null ? VisualTreePath
                                                                              : VisualTree;

        // TODO: Find different solution. It's prone to errors as a lot of editor scripts want to override OnEnable
        protected void OnEnable()
        {
            InitializeLockableInspector();
        }

        protected void InitializeLockableInspector()
        {
            m_InspectorLockedProps = GetSerializedProperty(this.serializedObject, GetVisualTree);
        }

        public override VisualElement CreateInspectorGUI()
        {
            //if (GetVisualTree == null)
            //{
            //    return base.CreateInspectorGUI();
            //}

            VisualElement root = new VisualElement();
            GetVisualTree.CloneTree(root);

            List<InspectorLockElement> inspectorLocks = root.Query<InspectorLockElement>().ToList();
            foreach (var inspectorLock in inspectorLocks)
            {
                if (inspectorLock.DrawDefaultInspector)
                {
                    InspectorElement.FillDefaultInspector(inspectorLock, serializedObject, this);
                }
            }

            // create an array of bools equal to the number of locks attached to this UI element
            InitializeLocks(root, serializedObject, m_InspectorLockedProps);

            DisableScriptProperties(inspectorLocks);

            return root;
        }

        /// <summary>
        /// Disable Inspector editing of the script field if drawing default editor.
        /// </summary>
        /// <param name="targets"></param>
        private void DisableScriptProperties(List<InspectorLockElement> targets)
        {
            foreach (var target in targets)
            {
                var scriptPropertyField = target.Q<PropertyField>("PropertyField:m_Script");
                
                if (scriptPropertyField != null)
                {
                    scriptPropertyField.SetEnabled(false);

                }
            }
        }

        private SerializedProperty GetSerializedProperty(SerializedObject serializedObject, VisualTreeAsset treeAsset)
        {
            if (treeAsset == null)
            {
                Debug.LogWarning($"No Visual Tree Asset found on {target.name} Inspector Script. Drawing default GUI.");
                return null;
            }

            // find the bool that controls lock states
            if (target is not ILockableInspector)
            {
                Debug.LogWarning($"Interface {nameof(ILockableInspector)} not found on {target.name}. Implement the inferface on the object's script.");
                return null;
            }

            var editorLockable = target as ILockableInspector;
            return serializedObject.FindProperty(editorLockable.LockablePropertyPath);
        }

        /// <summary>
        /// Initialize all Editor Locks on a VisualElement.
        /// </summary>
        /// <param name="root">The Visual element.</param>
        /// <param name="serializedObj">The object that has this lock attached to it.</param>
        /// <param name="serializedArrayProp">Bool[] lock state property on the serialzied object.</param>
        public void InitializeLocks(VisualElement root, SerializedObject serializedObj, SerializedProperty serializedArrayProp)
        {

            if (serializedArrayProp == null)
            {
                Debug.LogWarning($"{serializedObj} needs to have a bool[] property present on it.");
                return;
            }

            var temp = root.Query("LockElem").ToList();
            List<InspectorLockElement> editorLocks = new List<InspectorLockElement>();

            foreach (VisualElement elem in temp)
            {
                var _lock = elem.hierarchy.parent;
                if (_lock is InspectorLockElement)
                {
                    editorLocks.Add((InspectorLockElement)_lock);
                }
            }
            // Might not be safe if adding buttons on the fly?
            // Also lock state will not be bound to the specific element but position in array if re-arranged.
            // This would need a new class instead of just using bools in the LockButtonTest class
            if (serializedArrayProp.arraySize != editorLocks.Count)
            {
                Debug.Log("Updated serialzied property size");
                serializedArrayProp.arraySize = editorLocks.Count;
                serializedObj.ApplyModifiedProperties();
            }

            for (int i = 0; i < serializedArrayProp.arraySize; i++)
            {
                if (editorLocks.Count < i)
                {
                    break;
                }

                editorLocks[i].Init(serializedObj, serializedArrayProp.GetArrayElementAtIndex(i));
            }

            return;
        }
    }
}
