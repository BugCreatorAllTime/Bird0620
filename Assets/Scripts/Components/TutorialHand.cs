using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialHand : MonoBehaviour
{
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SuggestClickTarget(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.DOKill();
        transform.position = position;
        transform.DOMove(position + Vector3.down * 10, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
    
    public void MoveTo(Vector3 position, float duration, Action callback)
    {
        gameObject.SetActive(true);
        transform.DOKill();
        transform.DOMove(position, duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        });
    }

    public void ShowTutorial(Vector3[] wayPoints)
    {
        gameObject.SetActive(true);

        transform.DOKill();

        transform.position = wayPoints[0];

        transform.DOPath(wayPoints, 1.5f, PathType.Linear, PathMode.Ignore).SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.OutSine);
    }
}