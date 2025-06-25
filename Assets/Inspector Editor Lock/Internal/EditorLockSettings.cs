using EditorLock;
using UnityEditor;
using UnityEngine;

namespace Editorlock
{
    [CreateAssetMenu(fileName = "Editor Lock Settings", menuName = "Lockable Editor")]
    public class EditorLockSettings: ScriptableObject, IEditorLockable
    {
        public string DefaultAssetPath;
        public Color UnlockedColor;
        public Color LockedColor;

        public float LockedOpacity;

        [SerializeField]
        private string m_DefaultAssetPath = "Scripts";
        
        [SerializeField]
        private Color m_DefaultUnlockedColor = new Color(0.79f, 0.54f, 0.10f, 1f);
        
        [SerializeField]
        private Color m_DefaultLockedColor = new Color(0.474f, 89f, 0.8f, 0.2f);
        
        [SerializeField]
        private float m_DefaultLockedOpacity = 0.85f;

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);


    }
}
