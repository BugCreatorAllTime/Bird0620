using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupGDPR : PopupBase
{
    [Header("Popup Other")]
    public TextMeshProUGUI Desc;

    public Button BtnPrivacyPolicy;

    public Button BtnAccpet;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        Time.timeScale = 0;

        BtnAccpet.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            DataManager.Instance.UserData.IsAcceptGDPR = true;
            Hide();
        });

        BtnPrivacyPolicy.onClick.AddListener(() =>
        {
            Application.OpenURL(CONST.POLICY_LINK);
        });
    }


    public override void Show()
    {
        base.Show();
    }
}
