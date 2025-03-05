using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameHUD_AreaNotificationBehaviour : MonoBehaviour
{
    public Animator _animation;
    public TextMeshProUGUI Text;
    public void OnNotification(string Area_text)
    {
        Text.text = Area_text;
        _animation.enabled = true;
        _animation.Play("HUD_AreaNotificationAnimation",0,0);
        // _animation.Update(0);
    }

    public void OnNotificationComplete()
    {
        Debug.Log("StopAnimation");
        _animation.enabled = false;
    }
}
