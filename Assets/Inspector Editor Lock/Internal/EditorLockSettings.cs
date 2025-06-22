using EditorLock;
using UnityEditor;
using UnityEngine;

namespace Editorlock
{
    [CreateAssetMenu(fileName = "Editor Lock Settings", menuName = "Lockable Editor")]
    public class EditorLockSettings: ScriptableObject, IEditorLockable
    {
        public string DefaultAssetPath = "Assets/Scripts";
        public Color UnlockedColor = new Color(0.79f, 0.54f, 0.10f, 1f);
        public Color LockedColor = new Color(0.474f, 89f, 0.8f, 0.2f);

        public float LockedOpacity = 0.85f;

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);


    }


}
