using System.Threading.Tasks;
using cfg.buff;
public class BuffComponet
{
    public Conditions buff;

    public int BuffTime;

    public CharacterStat playerData;
    public void InitBuff(int id , int time){
        buff = GameTableDataAgent.ConditionsTable.Get(id);
        BuffTime = time * 1000;
        playerData = GameRunTimeData.Instance.characterBasicStatManager.GetStat();
    }
    public virtual void Execute(){}
}