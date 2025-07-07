
namespace EditorLock
{
    public interface IEditorLockable
    {
        /// <summary>
        /// Reference to the variable bool[] EditorLockStates in this class.
        /// </summary>
        public string LockablePropertyPath { get; }
    }

}