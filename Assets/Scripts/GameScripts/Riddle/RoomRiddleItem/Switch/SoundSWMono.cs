using UnityEngine;

public class SoundSWMono : RiddleSwitch
{
    [Header("检测配置")] 
    public string soundTag = "Sound";
    public string blockerTag = "Blocker";

    [Header("状态显示")]
    [SerializeField] private bool heardSound = false;
    [SerializeField] private int blockingCount = 0;

    [Header("系统链接")]
    public SoundSWGroup groupManager;

    public bool IsBlocked => blockingCount > 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(soundTag))
        {
            if (!heardSound)
            {
                heardSound = true;
                groupManager?.RegisterTrigger(this);
            }
        }
        else if (other.CompareTag(blockerTag))
        {
            blockingCount++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(blockerTag))
        {
            blockingCount = Mathf.Max(0, blockingCount - 1);
        }
    }

    // GroupManager 调用这个来清除状态
    public void ResetTrigger()
    {
        heardSound = false;
    }

    public bool HasHeardSound() => heardSound;

    public override MonoBehaviour getMonoBehaviour()
    {
        return this;
    }
}