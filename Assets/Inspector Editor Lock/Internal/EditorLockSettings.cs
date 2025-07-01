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

        public static LockSettingsData DefaultLockSettings = new LockSettingsData()
        {
            Path = "Scripts",
            UnlockedColor = new Color(0.29f, 0.29f, 0.29f, 1f),
            LockedColor = new Color(0.79f, 0.54f, 0.10f, 1f),
            BorderWidth = 2,
            LockedOpacity = 0.85f
        };
    }

    [CreateAssetMenu(fileName = "Editor Lock Settings", menuName = "Lockable Editor")]
    public class EditorLockSettings: ScriptableObject, IEditorLockable
    {
        public string DefaultAssetPath;
        public Color UnlockedColor;
        public Color LockedColor;
        public int BorderWidth;
        public float LockedOpacity;

        public LockSettingsData DefaultLockSettings = LockSettingsData.DefaultLockSettings;
        public LockSettingsData PreviousLockSettings = LockSettingsData.DefaultLockSettings;

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);


    }
}
