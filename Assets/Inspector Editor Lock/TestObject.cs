using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EditorLock
{
    public class TestObject : MonoBehaviour, IEditorLockable
    {
        public string Yo;

        [SerializeField]
        public bool[] m_EditorLockStates;
        public string LockablePropertyPath => nameof(m_EditorLockStates);
    }

}