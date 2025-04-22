using DG.Tweening;
using UnityEngine;

public class GameInteractTip : MonoBehaviour
{
    public void PlayInitAnimation()
    {
        DOTween.Kill(this);
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.2f);
        
    }
    public void OnDetargeted()
    {
        DOTween.Kill(this);
        transform.DOScale(0f, 0.2f);
    }

    public void OnDestroy()
    {
        DOTween.Kill(this);
    }
}