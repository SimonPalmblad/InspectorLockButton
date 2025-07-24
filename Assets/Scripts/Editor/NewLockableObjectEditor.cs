using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using EditorLock;

[CustomEditor(typeof(NewLockableObject))]
public class NewLockableObjectEditor: LockableEditor<NewLockableObject>
{
	protected override VisualTreeAsset VisualTreePath => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UI/UXML/NewLockableObject.uxml");

}
