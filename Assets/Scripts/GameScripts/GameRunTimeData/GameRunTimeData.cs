
using System;
using UnityEngine.Serialization;

public class GameRunTimeData : Singleton<GameRunTimeData>
{
    public GameState gameState;
    public ItemSlotData CharacterItemSlotData;
    public CharacterBasicStatManager characterBasicStatManager;
    public InventoryManger InventoryManger;
    public MapTrackDataManager MapTrackDataManager;
    public CharacterExtendedStatManager CharacterExtendedStatManager;
    public RiddleItemStatusManager RiddleItemStatusManager;
    
    protected override void Awake()
    {
        base.Awake();
        gameState = gameObject.AddComponent<GameState>();
        CharacterItemSlotData = gameObject.AddComponent<ItemSlotData>();
        characterBasicStatManager = gameObject.AddComponent<CharacterBasicStatManager>();
        InventoryManger = gameObject.AddComponent<InventoryManger>();
        MapTrackDataManager = gameObject.AddComponent<MapTrackDataManager>();
        CharacterExtendedStatManager = gameObject.AddComponent<CharacterExtendedStatManager>();
        RiddleItemStatusManager = gameObject.AddComponent<RiddleItemStatusManager>();
    }
}
