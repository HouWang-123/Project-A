using System;
using Sirenix.OdinInspector;
using UnityEngine;

// 运行时实时演算，不需要存入内存
[Serializable]
public class CharacterExtendedStat
{
    public bool Dead;
    public bool IsRun;
    public bool IsWalk;

    public bool IsChasing; //todo 玩家是否被追击
    public bool IsHidding;
    public bool IsDropping;
}

public class CharacterExtendedStatManager : SerializedMonoBehaviour
{
    [SerializeField]private CharacterExtendedStat _characterExtendedStat;
    public CharacterExtendedStat GetStat()
    {
        return _characterExtendedStat;
    }
    private void Start()
    {
        _characterExtendedStat = new CharacterExtendedStat();
    }
}