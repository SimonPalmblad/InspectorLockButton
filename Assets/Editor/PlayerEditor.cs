using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using EditorLock;

[CustomEditor(typeof(Player ))]
public class PlayerEditor : Editor
{
    private Color m_ColorActive = Color.yellow;
    private Color m_ColorDefault = new Color(0.8235294f, 0.8235294f, 0.8235294f);
    
    private Player m_TargetPlayer;

    public VisualTreeAsset VisualTree;
    private VisualElement m_DebugElement;
    private VisualElement m_ClassElement;
    private EditorLockElement m_OptionsSlider;

    private SerializedProperty m_ClassProp;
    private SerializedProperty m_SliderValueProp;

    private Button debug1;
    private Button debug2;

    private Label m_DebugToggleLabel;
    private Toggle m_DebugToggle;
   
    private bool m_ShowDebug;

    Action<SerializedProperty> setClass;

    private void OnEnable()
    {
        m_TargetPlayer = target as Player;
        m_ClassProp = serializedObject.FindProperty("m_Class");
        m_SliderValueProp = serializedObject.FindProperty("m_SliderValue");
        ColorUtility.TryParseHtmlString("#CA8B1B", out m_ColorActive);
        ColorUtility.TryParseHtmlString("#D2D2D2", out m_ColorDefault);
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        VisualTree.CloneTree(root);

        debug1 = root.Q<Button>("Debug_1");
        debug1.RegisterCallback<ClickEvent>(DebugButton1);
        
        debug2 = root.Q<Button>("Debug_2");
        debug2.RegisterCallback<ClickEvent>(DebugButton2);

        m_DebugToggle = root.Q<Toggle>("DebugToggle");
        m_DebugToggle.RegisterCallback<ChangeEvent<bool>>(OnBoolChanged);

        m_ClassElement = root.Q<VisualElement>("ClassField");
        m_ClassElement.RegisterCallback<ChangeEvent<Enum>>(OnClassChanged);

        m_DebugElement = root.Q<VisualElement>("DebugElement");
        m_DebugToggleLabel = root.Q<Label>("DebugArrow");

        m_OptionsSlider = root.Q<EditorLockElement>("OptionsSlider");
        //m_OptionsSlider.Init(m_SliderValueProp);

        ToggleElementVisibility(m_DebugElement);
        
        return root;
    }

    private void OnClassChanged(ChangeEvent<Enum> evt)
    {
        m_TargetPlayer.SetNewClass(m_ClassProp.enumValueIndex);
    }

    private void OnBoolChanged(ChangeEvent<bool> evt)
    {
        m_ShowDebug = evt.newValue;
        m_DebugToggleLabel.style.color = ToggleTextColor(evt.newValue);
        ToggleElementVisibility(m_DebugElement);
    }

    private Color ToggleTextColor(bool newValue) => 
        newValue ? m_ColorActive 
        : m_ColorDefault;

    private void ToggleElementVisibility(VisualElement element)
    {
        if (m_ShowDebug)
        {
            element.style.display = DisplayStyle.Flex;
        }

        else
        {
            element.style.display = DisplayStyle.None;

        }
    }

    public void DebugButton1(ClickEvent evt)
    {
        m_TargetPlayer.RandomizeHealth();
    }

    public void DebugButton2(ClickEvent evt)
    {

    }

}
