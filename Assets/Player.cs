using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{

    public enum Class { Bard, Joker, King }
    
    #region Attributes
    [SerializeField]
    private float m_Health = 10f;
    
    [SerializeField]
    private float m_MoveSpeed = 5f;
    
    [SerializeField]
    private int m_Arms;
    
    [SerializeField]
    private int m_Teeth;
    
    [SerializeField]
    private int m_ItemSlots;
    
    [SerializeField]
    private float m_SliderValue;
    #endregion

    [SerializeField]
    private Class m_Class;
    [SerializeField]
    private ClassStats m_ClassStats;
    [SerializeField]
    private Item m_StartingIem;
    
    [SerializeField]
    private ClassDatabase m_ClassDatabase;

    public void RandomizeHealth()
    {
        m_Health = Random.Range(10f, 24f);
    }

    public void SetNewClass(int newClassIndex)
    {
        Class newClass = (Class)newClassIndex;
        m_ClassStats = m_ClassDatabase.classStats[newClassIndex];
        UpdateClass();
    }

    public void UpdateClass()
    {
        m_Health = m_ClassStats.Health;
        m_MoveSpeed = m_ClassStats.MoveSpeed;
        m_Arms = m_ClassStats.Arms;
        m_Teeth = m_ClassStats.Teeth;
        m_ItemSlots = m_ClassStats.ItemSlots;
        m_StartingIem = m_ClassStats.StartingItem;
    }
}
