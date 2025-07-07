using EditorLock;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item")]
public class Item: ScriptableObject, IEditorLockable
{
    [SerializeField]
    private Texture2D m_InventoryIcon;
    
    [SerializeField]
    private string m_Description;
   
    [SerializeField]
    private int m_GoldValue;

    public Texture2D InventoryIcon { get => m_InventoryIcon;}
    public string Description { get => m_Description;}
    public int GoldValue { get => m_GoldValue; }

    [SerializeField]
    private bool[] LockedState;
    public string LockablePropertyPath => nameof(LockedState);
}