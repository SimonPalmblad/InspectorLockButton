using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class SliderWithInput: VisualElement
{
    #region Boilerplate for showing up in UI Builder
    public class UXMLFactory: UxmlElementAttribute { }
    public SliderWithInput() { }
    #endregion

    public VisualTreeAsset VisualTree;

    private Slider m_Slider => this.Q<Slider>("Slider");
    private FloatField m_SliderInput => this.Q<FloatField>("SliderInput");
    

    public SliderWithInput(
        SerializedProperty Property = null,
        string Label = "",
        float MinValue = 0,
        float MaxValue = 10)
    {
        Init(Property, Label, MinValue, MaxValue);
    }

    public void Init(SerializedProperty Property, string Label="", float MinValue=0, float MaxValue = 10)
    {
        if (VisualTree == null)
        {
            VisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/UI/UXML/SliderWithInput.uxml");
        }

        VisualTree.CloneTree(this);

        m_Slider.lowValue = MinValue;
        m_Slider.highValue = MaxValue;
        m_Slider.label = Label;
        
        m_Slider.BindProperty(Property);
        m_SliderInput.BindProperty(Property);


    }

    private void SliderValueChanged(ChangeEvent<float> value)
    {
        Debug.Log("slider clicked");
    }


}
