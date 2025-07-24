using UnityEditor;
using UnityEngine.UIElements;
using ScriptFileCreation;
using System;

[CustomEditor(typeof(PlaceholderScriptComponent))]
public class PlaceholderScriptComponentEditor: Editor
{
    public VisualTreeAsset visualTree;
    private PlaceholderScriptComponent m_PlaceholderGameObject;

    public void OnEnable()
    {
        m_PlaceholderGameObject = target as PlaceholderScriptComponent;
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        visualTree.CloneTree(root);

       root.Q<Button>().RegisterCallback<ClickEvent>(ReplaceButtonClicked);
        
        return root;
    }

    private void ReplaceButtonClicked(ClickEvent evt)
    {   
        //Fetch the type from string reference
        Type MyScriptType = Type.GetType(StringHelpers.WithoutEnding(m_PlaceholderGameObject.ReplacementName) + ",Assembly-CSharp");
       
        //Use the fetched type to add it as a component
        var result = m_PlaceholderGameObject.gameObject.AddComponent(MyScriptType);
        if (result == null)
        {
            return;
        }

        if(m_PlaceholderGameObject.gameObject.TryGetComponent<PlaceholderScriptComponent>(out var component))
        {
            DestroyImmediate(component);
        }
    }
}
