using System;
using System.Reflection;
using UnityEngine;
using FEVM.Data;

// 自定义的派生数据类
[Serializable]
public class PlayerData : BaseData
{
    public int playerLevel;
    public string playerName;
    public int Hp;
}

// 测试类
public class DataKeeperKeeperExample: BaseDataKeeperKeeper<PlayerData>
{
    // 这里编写业务逻辑
    public PlayerData data;
    void Execution()
    {
        bool historyData;
        // 从自身获取 dataEntity
        // 从文件读取数据
        data = baseData;
        PlayerData loadedData = ReadData(out historyData);
        LoadData(null); // 读取并写入进baseData
        
        Debug.Log("存储器对象创建存储实例后进行读取（如果为空则为正常现象，如果存在值则是上次存储数据）");
        Debug.Log($"Loaded Player Level: {loadedData.playerLevel}");
        Debug.Log($"Loaded Player Name: {loadedData.playerName}");
        Debug.Log($"Loaded Player Name: {loadedData.Hp}");
        
        // 对象本身属性
        PlayerData dataEntity = GetDataEntity();
        
        // 自调用
        baseData.playerLevel = 5;
        baseData.playerName = "TestPlayer";
        baseData.Hp = 10;
        
        Debug.Log("存储器对象实例本身数据");
        Debug.Log($"Loaded Player Level: {dataEntity.playerLevel}");
        Debug.Log($"Loaded Player Name: {dataEntity.playerName}");
        
        PlayerData playerData = FEVMDataManager.Instance.GetData<PlayerData>();
        Debug.Log("通过存储管理器获取存储器实例数据");
        Debug.Log($"Loaded Player Level: {playerData.playerLevel}");
        Debug.Log($"Loaded Player Name: {playerData.playerName}");
        Debug.Log($"--------> 存储当前数据");
        
        SaveData();
        Debug.Log($"当前数据存储完成--------> ");
        // 新数据
        PlayerData overWrite = new PlayerData();
        overWrite.playerLevel = 10;
        overWrite.playerName ="玩家114514";
        
        // 自身新数据写入
        FEVMDataManager.Instance.WriteData(overWrite);
        
        Debug.Log("通过存储器本身写入新数据");
        Debug.Log($"Loaded Player Level: {baseData.playerLevel}");
        Debug.Log($"Loaded Player Name: {baseData.playerName}");
        
        Debug.Log("读取数据，应该为上次存储的数据");
        
        PlayerData saveFile = ReadData(out historyData);
        Debug.Log($"Loaded Player Level: {saveFile.playerLevel}");
        Debug.Log($"Loaded Player Name: {saveFile.playerName}");
        
        Debug.Log("通过存储器本身写入并存储新数据");
        
        FEVMDataManager.Instance.WriteAndSaveData(overWrite);
        Debug.Log($"Loaded Player Level: {dataEntity.playerLevel}");
        Debug.Log($"Loaded Player Name: {dataEntity.playerName}");

        PlayerData saveFile2 = ReadData(out historyData);
        Debug.Log("读取数据，应该为最新数据");
        Debug.Log($"Loaded Player Level: {saveFile2.playerLevel}");
        Debug.Log($"Loaded Player Name: {saveFile2.playerName}");
        
    }
}