using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLock;

public class NewLockableObject: MonoBehaviour, IEditorLockable

{
	// Implementation of IEditorLockable interface;
	[SerializeField]
	private bool[] m_EditorLockStates;
	public string LockablePropertyPath => nameof(m_EditorLockStates);

}
