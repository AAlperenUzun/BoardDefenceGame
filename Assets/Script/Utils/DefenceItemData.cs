using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Data", menuName = "Defence Item Data", order = 51)]
public class DefenceItemData : ScriptableObject
{
    public List<DefenceItem> defenceItems;
}

// DefenceItem sınıfınız
[System.Serializable]
public class DefenceItem
{
    public GridDefenceItemType defenceItemType;
    public float damage;
    public float range;
    public float interval;
    public Direction direction;
    public enum Direction
    {
        Forward,
        Horizontal,
        All,
        Diagonal
    }
}

