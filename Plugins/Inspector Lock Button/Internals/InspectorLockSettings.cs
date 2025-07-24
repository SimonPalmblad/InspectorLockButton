using UnityEngine;

namespace InspectorLock
{
    [System.Serializable]
    public class LockSettingsData 
    {
        public string Path;
        public Color UnlockedColor;
        public Color LockedColor;
        public int BorderWidth;
        public float LockedOpacity;

        public static LockSettingsData DefaultLockSettings => new LockSettingsData()
        {
            Path = "Scripts",
            UnlockedColor = new Color(0.29f, 0.29f, 0.29f, 1f),
            LockedColor = new Color(0.82f, 0.46f, 0.14f, 1f),
            BorderWidth = 2,
            LockedOpacity = 0.85f
        };


        public LockSettingsData Clone()
        {
            return new LockSettingsData
            {
                Path = this.Path,
                UnlockedColor = this.UnlockedColor,
                LockedColor = this.LockedColor,
                BorderWidth = this.BorderWidth,
                LockedOpacity = this.LockedOpacity
            };
        }
    }    

    [CreateAssetMenu(fileName = "Inspector Lock Settings", menuName = "🔒 Inspector Lock/Inspector Lock Settings")]
    public class InspectorLockSettings: ScriptableObject, ILockableInspector
    {
        public string DefaultAssetPath = LockSettingsData.DefaultLockSettings.Path;
        public Color UnlockedColor = LockSettingsData.DefaultLockSettings.UnlockedColor;
        public Color LockedColor = LockSettingsData.DefaultLockSettings.LockedColor;
        public int BorderWidth = LockSettingsData.DefaultLockSettings.BorderWidth;
        public float LockedOpacity = LockSettingsData.DefaultLockSettings.LockedOpacity;

        public LockSettingsData DefaultLockSettings = LockSettingsData.DefaultLockSettings;
        public LockSettingsData PreviousLockSettings = LockSettingsData.DefaultLockSettings.Clone();

        [SerializeField]
        private bool[] m_LockableStates;
        public string LockablePropertyPath => nameof(m_LockableStates);
    }
}
