using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using EditorLock;

public class DefaultEditorDrawer: MonoBehaviour, IEditorLockable
{
	// Implementation of IEditorLockable interface;
	//
	[SerializeField]
	[HideInInspector]
	private bool[] m_EditorLockStates;
	public string LockablePropertyPath => nameof(m_EditorLockStates);
}
