
using System;
using UnityEngine.Serialization;

public class GameRunTimeData : Singleton<GameRunTimeData>
{
    /// <summary>
    /// Chararcter
    /// </summary>
    public ItemSlotData CharacterItemSlotData;
    public CharacterBasicStatManager characterBasicStatManager;
    public InventoryManger InventoryManger;
    public MapTrackDataManager MapTrackDataManager;
    public CharacterExtendedStatManager CharacterExtendedStatManager;
    
    protected override void Awake()
    {
        base.Awake();
        CharacterItemSlotData = gameObject.AddComponent<ItemSlotData>();
        characterBasicStatManager = gameObject.AddComponent<CharacterBasicStatManager>();
        InventoryManger = gameObject.AddComponent<InventoryManger>();
        MapTrackDataManager = gameObject.AddComponent<MapTrackDataManager>();
        CharacterExtendedStatManager = gameObject.AddComponent<CharacterExtendedStatManager>();
    }
}
