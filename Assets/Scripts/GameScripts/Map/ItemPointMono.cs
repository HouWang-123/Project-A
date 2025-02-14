using System.Collections.Generic;
using UnityEngine;

public class ItemPointMono : MonoBehaviour
{
    [Header("生成的物品")]
    public List<ItemTip> ItemTips;
    [Header("生成的次数，即生成几个物品")]
    public int Number;
    [Header("是否直接生成")]
    [Tooltip("true为直接生成，false为需要交互后生成")]
    public bool IsInFlood = true;

}

[System.Serializable]
public class ItemTip
{
    public int ID;
    public int Number;
    [Range(0f, 100f)]
    public int Odds;
}