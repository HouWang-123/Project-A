
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class RiddleItemStatusManager : SerializedMonoBehaviour
{
    public Dictionary<int, Dictionary<string, RiddleItemBaseStatus>> AllStatusList = new ();

    public void SaveRiddleStatusFromManager(RiddleManager riddleManager)
    {
        int RoomId = GameControl.Instance.GetRoomData().ID;
        if (AllStatusList.ContainsKey(RoomId))
        {
            Dictionary<string,RiddleItemBaseStatus> riddleItemStatusFromRiddleManager = GetRiddleItemStatusFromRiddleManager(riddleManager);
            AllStatusList[RoomId] = riddleItemStatusFromRiddleManager;
        }
        else
        {
            Dictionary<string,RiddleItemBaseStatus> riddleItemStatusFromRiddleManager = GetRiddleItemStatusFromRiddleManager(riddleManager);
            AllStatusList.Add(RoomId,riddleItemStatusFromRiddleManager);
        }
    }

    public Dictionary<string, RiddleItemBaseStatus> LoadRiddleItemBaseStatusMap(int RoomId)
    {
        if (AllStatusList.ContainsKey(RoomId))
        {
            return AllStatusList[RoomId];
        }

        return null;
    }
    private Dictionary<string,RiddleItemBaseStatus> GetRiddleItemStatusFromRiddleManager (RiddleManager riddleManager)
    {
        Dictionary<string,RiddleItemBaseStatus> toadd = new Dictionary<string,RiddleItemBaseStatus>();
        List<RiddleItemBase> riddleItemBases = riddleManager.GetAllRiddleItem();
        foreach (var riddleItemBase in riddleItemBases)
        {
            RiddleItemBaseStatus riddleItemBaseStatus = riddleItemBase.GetRiddleStatus();
            string riddleKey = riddleItemBase.GetRiddleKey();
            toadd.Add(riddleKey,riddleItemBaseStatus);
        }

        return toadd;
    }
}
