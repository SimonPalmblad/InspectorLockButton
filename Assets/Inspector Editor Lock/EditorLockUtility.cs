using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

namespace EditorLockUtilies
{
    public static class EditorLockUtility
    {

        public static SerializedProperty OnEnable<T>(VisualTreeAsset treeAsset, T target) where T : MonoBehaviour
        {
            if (treeAsset == null)
            {
                Debug.LogWarning($"No Visual Tree Asset found on {target.name} Editor Script. Drawing default GUI.");
                return null;
            }

            // find the bool that controls lock states
            if (target is not IEditorLockable)
            {
                Debug.LogWarning($"Interface IEditorLockable not found on {target.name}. Implement the inferface on the object's script.");
                return null;
            }
           
            var serializedObject = new SerializedObject(target);
            var editorLockable = target as IEditorLockable;

            return serializedObject.FindProperty(editorLockable.LockablePropertyPath);
        }

        /// <summary>
        /// Initialize all Editor Locks on a VisualElement.
        /// </summary>
        /// <param name="root">The Visual element.</param>
        /// <param name="serializedObject">The object that has this lock attached to it.</param>
        /// <param name="serializedArrayProp">Bool[] lock state property on the serialzied object.</param>
        public static void InitializeLocks(VisualElement root, SerializedObject serializedObject, SerializedProperty serializedArrayProp)
        {

            if(serializedArrayProp == null)
            {
                Debug.LogWarning($"{serializedObject} needs to have a bool[] property present on it.");
                return;
            }

            var temp = root.Query("LockElem").ToList();
            List<EditorLock> editorLocks = new List<EditorLock>();

            foreach (VisualElement elem in temp)
            {
                var _lock = elem.hierarchy.parent;
                if (_lock is EditorLock)
                {
                    editorLocks.Add((EditorLock)_lock);
                }
            }
            // Might not be safe if adding buttons on the fly?
            // Also lock state will not be bound to the specific element but position in array if re-arranged.
            // This would need a new class instead of just using bools in the LockButtonTest class
            if (serializedArrayProp.arraySize != editorLocks.Count)
            {
                serializedArrayProp.arraySize = editorLocks.Count;
            }

            for (int i = 0; i < serializedArrayProp.arraySize; i++)
            {
                editorLocks[i].Init(serializedObject, serializedArrayProp.GetArrayElementAtIndex(i));
            }
            
            return;
        }
    }
}
