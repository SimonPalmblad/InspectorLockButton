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
        public int BorderWidth;
        public float LockedOpacity;         
    }

    [CreateAssetMenu(fileName = "Editor Lock Settings", menuName = "Lockable Editor")]
    public class EditorLockSettings: ScriptableObject, IEditorLockable
    {
        public string DefaultAssetPath;
        public Color UnlockedColor;
        public Color LockedColor;
        public float BorderWidth;
        public float LockedOpacity;

        public LockSettingsData DefaultLockSettings = new LockSettingsData()
        {
            Path = "Scripts",
            UnlockedColor = new Color(0f, 0.42f, 0.29f, 1f),
            LockedColor = new Color(0.79f, 0.54f, 0.10f, 1f),
            BorderWidth = 6,
            LockedOpacity = 0.85f
        };

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);


    }
}
