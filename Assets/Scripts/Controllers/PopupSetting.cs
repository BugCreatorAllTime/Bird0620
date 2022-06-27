using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PopupSetting : PopupBase
{
    [Header("Popup Other")]

    public Button BtnPrivacyPolicy;

    public TextMeshProUGUI Version;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Version.text = $"Version {CONST.VERSION_CODE}.{CONST.VERSION_NAME}";

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
