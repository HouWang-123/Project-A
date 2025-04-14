
using System;

public class GameRunTimeData : Singleton<GameRunTimeData>
{
    /// <summary>
    /// Chararcter
    /// </summary>
    public ItemSlotData CharacterItemSlotData;

    public CharacterBasicStat CharacterBasicStat;
    public InventoryManger InventoryManger;
    public MapTrackDataManager MapTrackDataManager;

    protected override void Awake()
    {
        base.Awake();
        CharacterItemSlotData = gameObject.AddComponent<ItemSlotData>();
        CharacterBasicStat = gameObject.AddComponent<CharacterBasicStat>();
        InventoryManger = gameObject.AddComponent<InventoryManger>();
        MapTrackDataManager = gameObject.AddComponent<MapTrackDataManager>();
    }
}
