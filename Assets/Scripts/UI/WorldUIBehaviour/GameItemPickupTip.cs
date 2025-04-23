using System;
using DG.Tweening;
using UnityEngine;

public class GameItemPickupTip : MonoBehaviour
{
    public void PlayInitAnimation()
    {
        DOTween.Kill(gameObject);
        transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.2f);
        
    }

    public void OnDetargeted()
    {
        DOTween.Kill(gameObject);
        transform.DOScale(0f, 0.2f);
    }

    public void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }

    public void OnItemPicked()
    {
        DOTween.Kill(this);
        transform.DOScale(0f, 0.2f).onComplete = () =>
        {
            Destroy(gameObject);
        };
    }
}