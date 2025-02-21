using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class CharacterStat
{
    public int ID;
    public string NAME;
    public string DESCRIBE;

    public string VoicePackID;
    public string PrefabName;

    public float MaxHP;
    public float CurrentHp;

    public float MaxSan;
    public float CurrentSan;

    public float MaxFood;
    public float CurrentFood;
    public List<float> DigestRate;
    public float RunSpeedScale;
    public float RunReduce;
    public float RunRestore;

    public float MaxThirsty;
    public float CurrentThirsty;

    public float Strength;

    public int InventorySlots;

    public int ActiveSkillID;
    public int PassiveSkillID;

    public bool Dead;
}

public class CharacterBasicStat
{
    private cfg.cha.Character m_characterData;
    private CharacterStat CharacterStat;
    private bool playerDataInited;

    public void InitCharacter(int m_characterID)
    {
        try
        {
            m_characterData = GameTableDataAgent.CharacterTable.Get(m_characterID);
        }
        catch (Exception e)
        {
            m_characterData = GameTableDataAgent.CharacterTable.Get(GameConstData.DEFAULT_CHARACTER_ID);
            Debug.LogWarning("==========================角色数据不存在，使用默认数据=============================");
            Debug.LogWarning("======================可以正常游玩，你将使用的角色数据ID为=========================");
            Debug.LogWarning("======================================110001=====================================");
        }

        CharacterStat = new()
        {
            ID = m_characterData.ID,
            PrefabName = m_characterData.PrefabName,
            VoicePackID = m_characterData.VoicePackID,
            NAME = m_characterData.NAME,
            DESCRIBE = m_characterData.DESCRIBE,
            MaxHP = m_characterData.MaxHP,
            CurrentHp = m_characterData.MaxHP,
            MaxFood = m_characterData.MaxFood,
            CurrentFood = m_characterData.MaxFood,
            DigestRate = m_characterData.DigestRate,
            Strength = m_characterData.Strength,
            InventorySlots = m_characterData.InventorySlots,
            ActiveSkillID = m_characterData.ActiveSkillID,
            PassiveSkillID = m_characterData.PassiveSkillID,
            MaxThirsty = m_characterData.MaxThirsty,
            CurrentThirsty = m_characterData.MaxThirsty,
            MaxSan = m_characterData.MaxSan,
            CurrentSan = m_characterData.MaxSan,
            RunReduce = m_characterData.RunReduce,
            RunRestore = m_characterData.RunRestore,
            RunSpeedScale = m_characterData.RunSpeedScal,
            Dead = false
        };
        playerDataInited = true;
        GameHUD.Instance.SetHUDStat(CharacterStat);
    }

    public void HurdPlayer(int number)
    {
        CharacterStat.CurrentHp -= number;
    }

    public void HurdPlayer(float number)
    {
        CharacterStat.CurrentHp -= number;
    }

    public ref CharacterStat GetStat()
    {
        return ref CharacterStat;
    }

    // 在 FxiedUpdate中进行调用
    public void UpdatePlayerStat()
    {
        if (playerDataInited)
        {
            // 死亡检测
            LifeChecker();
            
            
            LivePlayerStatUpdater();// 玩家存活时执行的更新
            DeadPlayerStatUpdater();// 玩家死亡时数据执行的更新
            
            ///// 其他更新

            // 生命值合理范围检测
            HpCorrector();
        }
    }
    
    // 保证 HP 在合理的范围内
    private void LivePlayerStatUpdater()
    {
        if (!CharacterStat.Dead) 
        {
            
            
            
            
            
        }
    }

    private void DeadPlayerStatUpdater()
    {
        if (CharacterStat.Dead)
        {
            
            
            
            
        }
    }
    private void HpCorrector()
    {
        CharacterStat.CurrentHp =
        Mathf.Clamp(CharacterStat.CurrentHp, 0f, CharacterStat.MaxHP);
    }

    private void LifeChecker()
    {
        if (CharacterStat.CurrentHp <= 0f)
        {
            CharacterStat.Dead = true;
        }
    }
}