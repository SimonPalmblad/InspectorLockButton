using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace EditorLockUtilies
{
    public static class EditorLockUtility
    {
        public static void InitializeLocks(VisualElement root, SerializedObject serializedObject, SerializedProperty serializedArrayProp)
        {
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

            //Might not be safe if adding buttons on the fly? Also lock state will not be bound to the specific element but position in array if re-arranged.
            if (serializedArrayProp.arraySize != editorLocks.Count)
            {
                serializedArrayProp.arraySize = editorLocks.Count;
            }

            for (int i = 0; i < serializedArrayProp.arraySize; i++)
            {
                editorLocks[i].GetType()
                    .GetMethod("Init")
                    .Invoke(editorLocks[i], new object[]
                    {
                            serializedObject,
                            serializedArrayProp.GetArrayElementAtIndex(i)
                    });
            }
        }
    }
}
