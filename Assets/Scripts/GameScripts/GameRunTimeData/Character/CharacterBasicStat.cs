using System.Collections.Generic;

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
    public List<float>DigestRate;
    public float RunSpeedScale;
    public float RunReduce;
    public float RunRestore;
    
    public float MaxThirsty;
    public float CurrentThirsty;
    
    public float Strength;
    
    public int InventorySlots;

    public int ActiveSkillID;
    public int PassiveSkillID;
}
public class CharacterBasicStat
{
    private cfg.cha.Character m_characterData;
    private CharacterStat CharacterStat;

    public void InitCharacter(int m_characterID)
    {
        m_characterData = GameTableDataAgent.CharacterTable.Get(m_characterID);
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
            CurrentFood =  m_characterData.MaxFood,
            DigestRate = m_characterData.DigestRate,
            Strength = m_characterData.Strength,
            InventorySlots = m_characterData.InventorySlots,
            ActiveSkillID = m_characterData.ActiveSkillID,
            PassiveSkillID = m_characterData.PassiveSkillID,
            MaxThirsty =  m_characterData.MaxThirsty,
            CurrentThirsty =  m_characterData.MaxThirsty,
            MaxSan = m_characterData.MaxSan,
            CurrentSan = m_characterData.MaxSan,
            RunReduce =  m_characterData.RunReduce,
            RunRestore = m_characterData.RunRestore,
            RunSpeedScale = m_characterData.RunSpeedScal
        };
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
        
    }
    
}
