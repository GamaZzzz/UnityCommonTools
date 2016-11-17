using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;

public class UIAnimation : Widget
{
    [SerializeField]
    private bool fadeInEnable;
    [SerializeField]
    private bool fadeOuEnable;

    private Vector2 _originalPos;

    private ParticleSystem ps;

    void Start()
    {
        float h = RectRoot.sizeDelta.y;
        Vector2 newPos = RectRoot.anchoredPosition;
        newPos.y = -1.5f * h;
        RectRoot.anchoredPosition = newPos;
        _originalPos = newPos;
    }

    public void Play(bool isForward = true)
    {
        if (isForward)
        {
            PlayForward();
        }
        else
        {
            PlayBackward();
        }
    }
    
    public void PlayForward()
    {
        Tweener tweener = RectRoot.DOAnchorPosY(0, 1, false)
            .SetEase(Ease.OutBounce)
            .OnComplete(() => { Debug.Log("Show finished!"); });
        tweener.Play();
    }

    public void PlayBackward()
    {
        Tweener tweener = RectRoot.DOAnchorPosY(_originalPos.y, 0.75f, false)
            .SetEase(Ease.Linear)
            .OnComplete(() => { Debug.Log("Show finished!"); });
        tweener.Play();
    }
}

