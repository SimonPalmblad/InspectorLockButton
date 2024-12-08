using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item")]
public class Item: ScriptableObject
{
    [SerializeField]
    private bool m_EditorLocked;

    [SerializeField]
    private Texture2D m_InventoryIcon;
    
    [SerializeField]
    private string m_Description;
   
    [SerializeField]
    private int m_GoldValue;

    public Texture2D InventoryIcon { get => m_InventoryIcon;}
    public string Description { get => m_Description;}
    public int GoldValue { get => m_GoldValue; }
}