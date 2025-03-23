using UnityEngine;

public class FlashLightBehaviour : ToolBehaviour
{
    public GameObject LightNode;
    public bool isOn;
    [Header("光源的碰撞体")]
    public Collider LightFieldCollider;

    // 仅在首次调用 Update 方法之前调用 Start
    private void Start()
    {
        EventManager.Instance.RunEvent(EventConstName.FlashLightCreate, this);
    }

    public void LightOn()
    {
        LightNode.SetActive(true);
        isOn = true;
    }

    public void LightOff()
    {
        LightNode.SetActive(false);
        isOn = false;
    }
}
