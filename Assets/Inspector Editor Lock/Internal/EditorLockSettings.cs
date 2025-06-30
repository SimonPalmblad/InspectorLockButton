using EditorLock;
using UnityEditor;
using UnityEngine;

namespace Editorlock
{
    public record LockSettingsData 
    {
        public string Path;
        public Color UnlockedColor;
        public Color LockedColor;
        public float LockedOpacity;
         
    }

    [CreateAssetMenu(fileName = "Editor Lock Settings", menuName = "Lockable Editor")]
    public class EditorLockSettings: ScriptableObject, IEditorLockable
    {
        public string DefaultAssetPath;
        public Color UnlockedColor;
        public Color LockedColor;
        public float LockedOpacity;

        public LockSettingsData DefaultLockSettings = new LockSettingsData()
        {
            Path = "Scripts",
            UnlockedColor = new Color(0.79f, 0.54f, 0.10f, 1f),
            LockedColor = new Color(0.474f, 89f, 0.8f, 0.2f),
            LockedOpacity = 0.85f
        };

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);


    }
}
