
public class GameRunTimeData : SingleDataMgr<GameRunTimeData>
{
    public ItemSlotData CharacterItemSlotData = new ();
    public CharacterBasicStat CharacterBasicStat = new ();
    public ItemManager ItemManager = new ();
}
