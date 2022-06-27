
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupUpdateNewVersion : PopupBase
{
    [Header("Popup Other")]
    public Button BtnUpdate;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        BtnUpdate.onClick.AddListener(() =>
        {
            MainController.OpenAppInStore();
        });
    }


    public override void Show()
    {
        base.Show();
    }
}
