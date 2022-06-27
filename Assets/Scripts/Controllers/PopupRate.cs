using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupRate : PopupBase
{
    [Header("Popup Other")]
    public Box5Star Box5Star;

    public Button BtnRate;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        BtnRate.onClick.AddListener(() =>
        {
            DataManager.Instance.SettingData.HasRate = true;
            MainController.OpenAppInStore();

            Hide();
        });

        CloseCallback.AddListener(() =>
        {
        });
    }


    public override void Show()
    {
        base.Show();

        Box5Star.Reset();
    }
}
