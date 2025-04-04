
public class GameRunTimeData : SingleDataMgr<GameRunTimeData>
{
    /// <summary>
    /// Chararcter
    /// </summary>
    public ItemSlotData CharacterItemSlotData = new ();
    public CharacterBasicStat CharacterBasicStat = new ();
    public InventoryManger InventoryManger = new InventoryManger();

    public MapTrackDataManager MapTrackDataManager = new MapTrackDataManager();
}
