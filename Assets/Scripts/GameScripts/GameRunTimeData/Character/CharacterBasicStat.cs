using System;
using System.Collections.Generic;
using cfg.buff;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class CharacterStat
{
    public int ID;                
    public string NAME;          
    public string DESCRIBE;       

    public string VoicePackID; 
    public string PrefabName;
    // Hp
    public float MaxHP;
    public float CurrentHp;
    // San
    public float MaxSan;
    public float CurrentSan;
    // Food
    public float MaxFood;
    public float CurrentFood;
    public List<float> DigestRate;
    // Thirsty
    public float MaxThirsty;
    public float CurrentThirsty;
    // Run
    public float WalkSpeed;
    public float RunSpeedScale;
    public float RunReduce;
    public float RunRestore;
    
    public float Strength;

    public int InventorySlots;
    // Skill
    public int ActiveSkillID;
    public int PassiveSkillID;
    // Other Character Stat
    public bool Dead;
    public ItemBase ItemOnHand; // 手中的物品
    public ItemBase LiftedItem; // 举起的物品
    
}

public class CharacterBasicStat
{
    private cfg.cha.Character m_characterData;
    private CharacterStat CharacterStat;
    private bool playerDataInited;
    
    // 提供给存档使用
    public void InitCharacter(CharacterStat stat)
    {
        CharacterStat = stat;
        m_characterData = GameTableDataAgent.CharacterTable.Get(CharacterStat.ID);
        playerDataInited = true;
        GameHUD.Instance.SetHUDStat(CharacterStat);
    }
    
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
            WalkSpeed = m_characterData.WalkSpeed,
            Dead = false
        };
        playerDataInited = true;
        GameHUD.Instance.SetHUDStat(CharacterStat);
    }
    public void HurtPlayer(int number)
    {
        if (CharacterStat.Dead) return;
        float attack = number;
        if (BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.疯狂))
        { 
            attack = number + ((CharacterStat.MaxSan - CharacterStat.CurrentSan) / 2f);
        }
        //todo 如果还有其他增幅 在此处结算
        CharacterStat.CurrentHp -= attack;
    }
    public void HurtPlayer(float number)
    {
        CharacterStat.CurrentHp -= number;
    }

    public void UpdatePlayerSan(float number)
    {
        if (BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.美德))
        {
            CharacterStat.CurrentSan += -number;
        }
        CharacterStat.CurrentSan += number;
        if (Tools.IsInRange(CharacterStat.CurrentSan, 40, 60))
        {
            if (!BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.癫狂1))
            {
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID,(int)BuffEnum.癫狂1,999);
            }
        }
        else if (Tools.IsInRange(CharacterStat.CurrentSan, 20, 39))
        {
            if (!BuffManager.Instance.UpdateBuff(CharacterStat.ID, (int)BuffEnum.癫狂1))
            {
                //直接添加当前buff
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID,(int)BuffEnum.癫狂2,999);
            }
        }
        else if (Tools.IsInRange(CharacterStat.CurrentSan, 0, 19))
        {
            if (!BuffManager.Instance.UpdateBuff(CharacterStat.ID, (int)BuffEnum.癫狂2))
            {
                //直接添加当前buff
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID,(int)BuffEnum.癫狂3,999);
            }
        }
    }
    public ref CharacterStat GetStat()
    {
        return ref CharacterStat;
    }
    
#region RunTimeUpdate
    // 在 FxiedUpdate中进行调用
    public void UpdatePlayerStat()
    {
        if (playerDataInited)
        {
            // 死亡检测
            LifeChecker();

            LivePlayerStatUpdater(); // 玩家存活时执行的更新
            DeadPlayerStatUpdater(); // 玩家死亡时数据执行的更新

            // 其他更新
            // 生命值合理范围检测
            HpCorrector();
            SanCorrector();
        }
    }

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
            Debug.Log("PlayerDead");
        }
    }

    // 保证 HP 在合理的范围内

    private void HpCorrector()
    {
        CharacterStat.CurrentHp =
            Mathf.Clamp(CharacterStat.CurrentHp, 0f, CharacterStat.MaxHP);
    }
    
    private void SanCorrector()
    {
        CharacterStat.CurrentSan =
            Mathf.Clamp(CharacterStat.CurrentSan, 0f, CharacterStat.MaxSan);
    }

    private void LifeChecker()
    {
        if (CharacterStat.CurrentHp <= 0f)
        {
            CharacterStat.Dead = true;
        }
    }
#endregion
}