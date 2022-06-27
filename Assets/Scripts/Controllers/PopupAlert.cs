using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAlert : PopupBase
{
    public TextMeshProUGUI DescText;

    public Button BtnOk;

    // Start is called before the first frame update
    public virtual void Start()
    {
        base.Start();
        showInterWhenClose = false;

        BtnOk.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public virtual void Show(string msg)
    {
        DescText.text = msg;
        base.Show();
    }
}