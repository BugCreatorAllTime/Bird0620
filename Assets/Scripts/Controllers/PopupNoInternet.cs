using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupNoInternet : PopupBase
{
    public Button BtnOk;
    
    public override void Start()
    {
        base.Start();

        BtnOk.onClick.AddListener(HandleClickOk);
    }

    public override void Show()
    {
        base.Show();
        GameInfo.IsPopupNoInternetShow = true;
    }

    public override void Hide()
    {
        base.Hide();
        GameInfo.IsPopupNoInternetShow = false;
    }

    public void HandleClickOk()
    {
        Hide();

        // check internet
        MainController.LevelSceneManager.CheckInternet();
    }
}