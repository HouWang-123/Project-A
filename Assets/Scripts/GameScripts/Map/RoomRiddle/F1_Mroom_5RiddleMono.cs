using UnityEngine;

public class F1_Mroom_5RiddleMono : RoomRiddleMonoBase
{
    protected override bool BeforeCondition()
    {
        foreach (var item in riddleItems)
        {
            if (item.gameObject.activeSelf && !item.IsDone)
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
            // 解开躲藏区域
            if (item is HideAreaMono hideAreaMono)
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

