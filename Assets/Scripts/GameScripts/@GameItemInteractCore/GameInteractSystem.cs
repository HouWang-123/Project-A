// 处理物品交互补充特殊逻辑

using FEVM.Timmer;
using UnityEngine;

public static partial class GameInteractSystemExtendedCode
{
    
    public static void ExecuteInteraction(int ExecuteID , GameObject Source = null ,GameObject Target = null)
    {
        if (ExecuteID == -1)
        {
            return;
        }
        // 放入篮球代码
        // 删除手中的篮球
        if (ExecuteID == 1000)
        {
            Execute_1000(Source,Target);
        }
        if (ExecuteID == 1001)
        {
            Execute_1001(Target);
        }
        if (ExecuteID == 1002)
        {
            Execute_1002(Target);
        }
        if (ExecuteID == 1003)
        {
            Execute_1003(Target);
        }
        if (ExecuteID == 1004)
        {
            Execute_1004(Target);
        }
        if (ExecuteID == 1005)
        {
            Execute_1005(Target);
        }
        if (ExecuteID == 1006)
        {
            Execute_1006(Target);
        }
        if (ExecuteID == 1007)
        {
            Execute_1007(Target);
        }
        if (ExecuteID == 1008)
        {
            Execute_1008(Target);
        }
    }

    private static void Execute_1000(GameObject source ,GameObject Target)
    {
        BasketballHoopMono basketballHoopMono = source.transform.GetComponent<BasketballHoopMono>();

        GameObject itemBaseGameObject = GameRunTimeData.Instance.CharacterBasicStat.GetStat().LiftedItem.gameObject;
        basketballHoopMono.PushBasketBall();
        PlayerControl.Instance.DropItem(false);
        TimeMgr.Instance.AddTask(0.3f,false, () =>
        {
            GameObject.Destroy(itemBaseGameObject);
        });

    }
    private static void Execute_1001(GameObject Target)
    {
        
    }
    private static void Execute_1002(GameObject Target)
    {
        
    }
    private static void Execute_1003(GameObject Target)
    {
        
    }
    private static void Execute_1004(GameObject Target)
    {
        
    }
    private static void Execute_1005(GameObject Target)
    {
        
    }
    private static void Execute_1006(GameObject Target)
    {
        
    }
    private static void Execute_1007(GameObject Target)
    {
        
    }
    private static void Execute_1008(GameObject Target)
    {
        
    }
}
