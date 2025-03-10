
using UnityEngine;

public class LightBehaviour : ToolBehaviour
{
    public GameObject LightNode;
    public bool isOn;
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
