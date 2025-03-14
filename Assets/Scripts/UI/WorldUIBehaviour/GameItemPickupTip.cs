using System;
using DG.Tweening;
using UnityEngine;

public class GameItemPickupTip : MonoBehaviour
{
    public void PlayInitAnimation(bool isReversed)
    {
        DOTween.Kill(this);
        transform.localScale = Vector3.zero;
        if (isReversed)
        {
            transform.DOScale(new Vector3(-1,1,1), 0.2f);
        }
        else
        {
            transform.DOScale(1f, 0.2f);
        }
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