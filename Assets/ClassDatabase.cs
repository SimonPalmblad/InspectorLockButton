using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class Database", menuName = "Custom/Class Database")]
public class ClassDatabase : ScriptableObject
{
    public List<ClassStats> classStats = new List<ClassStats>();
}
