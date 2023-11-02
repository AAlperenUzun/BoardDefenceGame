using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [NonSerialized]public GridDefenceItemType _defenceItemType;
    public TMP_Text typeT;

    public void SetType(GridDefenceItemType type)
    {
        _defenceItemType = type;
        typeT.text = (int)type + "";
        GameController.Instance.BoardController.InventoryItems.Add(this);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
