using System;
using System.Collections.Generic;
using cfg.buff;
using Sirenix.OdinInspector;
using Unity.Mathematics.Geometry;
using UnityEngine;

[Serializable]
public class CharacterStat
{
    // 基本信息
    [TitleGroup("基本信息", Order = 0)]
    [GUIColor(1f, 1f, 0.7f)]
    public int ID;

    [TitleGroup("基本信息")]
    [GUIColor(1f, 1f, 0.7f)]
    public string NAME;

    [TitleGroup("基本信息")]
    [MultiLineProperty(3)]
    [GUIColor(1f, 1f, 0.7f)]
    public string DESCRIBE;

    [TitleGroup("基本信息")]
    [GUIColor(1f, 1f, 0.7f)]
    public string VoicePackID;

    [TitleGroup("基本信息")]
    [GUIColor(1f, 1f, 0.7f)]
    public string PrefabName;

    // 生命状态
    [TitleGroup("生命状态", Order = 1)]
    [TitleGroup("生命状态/HP", Order = 1)]
    [GUIColor(0.8f, 1f, 0.8f)]
    public float MaxHP;

    [TitleGroup("生命状态/HP")]
    [GUIColor(0.8f, 1f, 0.8f)]
    public float CurrentHp;

    [TitleGroup("生命状态/SAN")]
    [GUIColor(0.8f, 0.9f, 1f)]
    public float MaxSan;

    [TitleGroup("生命状态/SAN")]
    [GUIColor(0.8f, 0.9f, 1f)]
    public float CurrentSan;

    [TitleGroup("生命状态/饥饿")]
    [GUIColor(1f, 0.9f, 0.8f)]
    public float MaxFood;

    [TitleGroup("生命状态/饥饿")]
    [GUIColor(1f, 0.9f, 0.8f)]
    public float CurrentFood;

    [TitleGroup("生命状态/饥饿")]
    [GUIColor(1f, 0.9f, 0.8f)]
    public List<float> DigestRate;

    [TitleGroup("生命状态/口渴")]
    [GUIColor(0.8f, 1f, 1f)]
    public float MaxThirsty;

    [TitleGroup("生命状态/口渴")]
    [GUIColor(0.8f, 1f, 1f)]
    public float CurrentThirsty;

    // 移动与体能
    [TitleGroup("运动属性", Order = 2)]
    [GUIColor(0.9f, 0.9f, 1f)]
    public float WalkSpeed;

    [TitleGroup("运动属性")]
    [GUIColor(0.9f, 0.9f, 1f)]
    public float RunSpeedScale;

    [TitleGroup("运动属性")]
    [GUIColor(0.9f, 0.9f, 1f)]
    public float RunReduce;

    [TitleGroup("运动属性")]
    [GUIColor(0.9f, 0.9f, 1f)]
    public float RunRestore;

    [TitleGroup("运动属性")]
    [GUIColor(0.9f, 0.9f, 1f)]
    public float Strength;

    // 背包与技能
    [TitleGroup("技能与背包", Order = 3)]
    [GUIColor(0.8f, 1f, 0.95f)]
    public int InventorySlots;

    [TitleGroup("技能与背包")]
    [GUIColor(0.8f, 1f, 0.95f)]
    public int ActiveSkillID;

    [TitleGroup("技能与背包")]
    [GUIColor(0.8f, 1f, 0.95f)]
    public int PassiveSkillID;

    // 手持与举起物品
    [TitleGroup("物品状态", Order = 4)]
    [LabelText("手中物品")]
    [GUIColor(1f, 0.8f, 0.8f)]
    [SerializeField]
    private ItemBase itemOnHand;

    [ShowInInspector, HideLabel]
    [GUIColor(1f, 0.8f, 0.8f)]
    public ItemBase ItemOnHand
    {
        get => itemOnHand;
        set
        {
            itemOnHand = value;
            if (itemOnHand != null)
            {
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Animation, EPAHandState.Hand);
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Item);
            }
            else
            {
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Animation, EPAHandState.Default);
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Item);
            }
        }
    }

    [TitleGroup("物品状态")]
    [LabelText("举起物品")]
    [GUIColor(1f, 0.8f, 0.8f)]
    [SerializeField]
    private ItemBase liftedItem;

    [ShowInInspector, HideLabel]
    [GUIColor(1f, 0.8f, 0.8f)]
    public ItemBase LiftedItem
    {
        get => liftedItem;
        set
        {
            liftedItem = value;
            if (liftedItem != null)
            {
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Animation, EPAHandState.Head);
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Item);
            }
            else
            {
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Animation, EPAHandState.Default);
                EventManager.Instance.RunEvent(EventConstName.OnPlayerHandItemChanges_Item);
            }
        }
    }
}

public class CharacterBasicStatManager : SerializedMonoBehaviour
{
    private cfg.cha.Character m_characterData;
    [SerializeField] private CharacterStat CharacterStat;
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
        catch(Exception e)
        {
            Debug.LogWarning("错误信息：" + e.Message);
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
        };
        playerDataInited = true;
        GameHUD.Instance.SetHUDStat(CharacterStat);
    }
    public void HurtPlayer(int number)
    {
        if(GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().Dead)
            return;
        float attack = number;
        if(BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.疯狂))
        {
            attack = number + ((CharacterStat.MaxSan - CharacterStat.CurrentSan) / 2f);
        }
        //todo 如果还有其他增幅 在此处结算
        CharacterStat.CurrentHp -= attack;
        if (CharacterStat.CurrentHp < 0)
        {
            EventManager.Instance.RunEvent(EventConstName.PlayerOnDeadAnimation);
        }
        else
        {
            EventManager.Instance.RunEvent(EventConstName.PlayerHurtAnimation);
        }
        GameHUD.Instance.UpdateHp();
        
    }
    public void PlayerLifeMinus(float number)
    {
        CharacterStat.CurrentHp -= number;
        GameHUD.Instance.UpdateHp();
    }

    public void UpdatePlayerSan(float number)
    {
        if(BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.美德))
        {
            CharacterStat.CurrentSan += -number;
        }
        CharacterStat.CurrentSan += number;
        if(Tools.IsInRange(CharacterStat.CurrentSan, 40, 60))
        {
            if(!BuffManager.Instance.HasComponet(CharacterStat.ID, (int)BuffEnum.癫狂1))
            {
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID, (int)BuffEnum.癫狂1, 999);
            }
        }
        else if(Tools.IsInRange(CharacterStat.CurrentSan, 20, 39))
        {
            if(!BuffManager.Instance.UpdateBuff(CharacterStat.ID, (int)BuffEnum.癫狂1))
            {
                //直接添加当前buff
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID, (int)BuffEnum.癫狂2, 999);
            }
        }
        else if(Tools.IsInRange(CharacterStat.CurrentSan, 0, 19))
        {
            if(!BuffManager.Instance.UpdateBuff(CharacterStat.ID, (int)BuffEnum.癫狂2))
            {
                //直接添加当前buff
                BuffManager.Instance.AddBuff<ChaosI>(CharacterStat.ID, (int)BuffEnum.癫狂3, 999);
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
        if(playerDataInited)
        {
            // 死亡检测
            LifeChecker();
            LivePlayerStatUpdater(); // 玩家存活时执行的更新
            DeadPlayerStatUpdater(); // 玩家死亡时数据执行的更新
            
            HpCorrector();
            SanCorrector();
            
            GameHUD.Instance.UpdateHp();
        }
    }

    private void LivePlayerStatUpdater()
    {
        if(!GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().Dead)
        {
            
        }
    }

    private void DeadPlayerStatUpdater()
    {
        if(GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().Dead)
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
        if(CharacterStat.CurrentHp <= 0f)
        {
            GameRunTimeData.Instance.CharacterExtendedStatManager.GetStat().Dead = true;
        }
    }
    #endregion
}