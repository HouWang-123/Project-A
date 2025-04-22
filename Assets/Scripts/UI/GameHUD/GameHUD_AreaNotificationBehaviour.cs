using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameHUD_AreaNotificationBehaviour : MonoBehaviour
{
    public Animator _animation;
    public TextMeshProUGUI Text;
    public void OnNotification(string Area_text)
    {
        if (Area_text.Equals("")) return;
        Text.text = Area_text;
        _animation.enabled = true;
        _animation.Play("HUD_AreaNotificationAnimation",0,0);
        // _animation.Update(0);
    }

    public void OnNotificationComplete()
    {
        _animation.enabled = false;
    }
}
