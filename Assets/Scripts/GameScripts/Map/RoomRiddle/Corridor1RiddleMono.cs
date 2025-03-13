using TMPro;
using UnityEngine;

// 走廊1的谜题
public class Corridor1RiddleMono : RoomRiddleMonoBase
{
    /// <summary>
    /// 获取谜题物体：
    /// (1). 门左边的时间
    /// (2). 门右边的时间
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

    }
    protected override bool BeforeCondition()
    {
        Debug.Log(GetType() + "BeforeCondition() => 解密条件已达成");
        return true;
    }
    protected override void SetRiddleAction()
    {
        base.SetRiddleAction();
        int gameHour = TimeSystemManager.Instance.GameHour;
        int gameMinute = TimeSystemManager.Instance.GameMinute;
        string curTime = string.Format("{0:D2}:{1:D2}", gameHour, gameMinute);
        int randomHourOffset = Random.Range(8, 17);
        int randomMinuteOffset = Random.Range(0, 61);
        string wrongTime = string.Format("{0:D2}:{1:D2}", (gameHour + randomHourOffset) % 24, (gameMinute + randomMinuteOffset) % 60);
        // 随机选择一个时间进行正确的设置，另一个时间进行错误的设置
        int random = Random.Range(0, 2);
        riddleGameObjects[random].GetComponent<TextMeshPro>().text = curTime;
        riddleGameObjects[random ^ 1].GetComponent<TextMeshPro>().text = wrongTime;
    }
    protected override bool SetRiddle()
    {
        base.SetRiddle();
        return true;
    }
}

