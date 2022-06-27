
using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

public class PopupController : BaseController
{
    public PopupSetting PopupSetting;

    public PopupRate PopupRate;

    public PopupGDPR PopupGDPR;

    public PopupUpdateNewVersion PopupUpdateNewVersion;
    
    public PopupNoInternet PopupNoInternet;
    
    public PopupLoading PopupLoading;
    
    public PopupAlert PopupAlert;

    public PopupComplete PopupComplete;

    [HideInInspector] public Queue<PopupFullScreen> PopupQueue = new Queue<PopupFullScreen>();
    public Action FinalAction;

    private void Awake()
    {
        MainController.Popup = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void ProcessNext()
    {
        if (PopupQueue.Count > 0)
        {
            var nextPopup = PopupQueue.Dequeue();
            nextPopup.Show();
        }
        else
        {
            FinalAction.Invoke();
        }
    }

    public void HideAll()
    {
        var popups = GetComponentsInChildren<PopupBase>();
        foreach (var popup in popups)
        {
            popup.gameObject.SetActive(false);
        }
    }
}
