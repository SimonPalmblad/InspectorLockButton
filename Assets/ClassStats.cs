using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "New Class stats")]
public class ClassStats : ScriptableObject
{
    [SerializeField] [HideInInspector]
    private bool m_EditingLocked;

    public Player.Class Class;
    public float Health;
    public float MoveSpeed;
    public int Arms;
    public int Teeth;
    public int ItemSlots;
    public Item StartingItem;
}
