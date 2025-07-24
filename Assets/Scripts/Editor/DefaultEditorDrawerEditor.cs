using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using EditorLock;
using UnityEditor.UIElements;

[CustomEditor(typeof(DefaultEditorDrawer))]
public class DefaultEditorDrawerEditor: LockableEditor<DefaultEditorDrawer>
{
	protected override VisualTreeAsset VisualTreePath => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/UI/UXML/DefaultEditorDrawer_uxml.uxml");

    //public override VisualElement CreateInspectorGUI()
    //{
    //    var rootVisualElement = base.CreateInspectorGUI();
    //    InspectorElement.FillDefaultInspector(rootVisualElement.Q<EditorLockElement>(), serializedObject, this);
    //    return rootVisualElement;
    //}
}
