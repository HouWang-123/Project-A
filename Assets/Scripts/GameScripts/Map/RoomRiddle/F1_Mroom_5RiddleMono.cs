using UnityEngine;

public class F1_Mroom_5RiddleMono : RoomRiddleMonoBase
{
    protected override bool BeforeCondition()
    {
        foreach (var item in riddleItems)
        {
            var roomRiddleItem = item.GetComponent<IRoomRiddleItem>();
            if (roomRiddleItem.GetGO().activeSelf && !roomRiddleItem.isItemDone())
            {
                return false;
            }
        }
        return true;
    }

    protected override void SetRiddleAction()
    {
        foreach (var item in riddleItems)
        {
            IRoomRiddleItem roomRiddleItem = item.GetComponent<IRoomRiddleItem>();
            // 解开躲藏区域
            if (roomRiddleItem is HideAreaMono hideAreaMono)
            {
                hideAreaMono.gameObject.SetActive(true);
            }
        }

        isRiddleReady = true;
    }

    public override void DoRiddle()
    {
        
    }
}

