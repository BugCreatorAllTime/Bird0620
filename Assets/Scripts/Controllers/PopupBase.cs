using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopupBase : BaseController
{
    [Header("Popup Base")]
    public Button BtnClose;

    public GameObject MainObj;

    [HideInInspector] public UnityEvent CloseCallback;
    public Action HideCallback;

    protected bool showInterWhenClose = true;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;

        CloseCallback = new UnityEvent();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        if (BtnClose != null)
        {
            BtnClose.onClick.AddListener(() =>
            {
                if (ConfigGame.ShowInterWhenClickButton && showInterWhenClose)
                {
                    MainController.UIGame.ShowInterAds("Close" + this.GetType().Name);
                }
                CloseCallback.Invoke();
                Hide();
            });
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        MainObj.transform.DOKill();
        MainObj.transform.localScale = Vector3.one;
        MainObj.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 1f), 0.3f, 5, 1).SetEase(Ease.OutBounce);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        if (HideCallback != null)
        {
            HideCallback.Invoke();
        }
    }

    protected IEnumerator DelayHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}
