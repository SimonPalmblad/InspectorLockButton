
namespace InspectorLock
{
    public interface ILockableInspector
    {
        /// <summary>
        /// Reference to the variable bool[] EditorLockStates in this class.
        /// </summary>
        public string LockablePropertyPath { get; }
    }

}