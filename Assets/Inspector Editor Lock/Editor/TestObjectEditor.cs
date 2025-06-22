using UnityEditor;

namespace EditorLock
{
	[CustomEditor(typeof(TestObject))]
	public class TestObjectEditor : LockableEditor<TestObject>
	{

	}
} 
