using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupLoading : PopupBase
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        
    }

    public override void Show()
    {
        base.Show();
        
        StopAllCoroutines();
        
        StartCoroutine(DelayAutoHide());
    }

    IEnumerator DelayAutoHide()
    {
        yield return new WaitForSeconds(10);
        
        Hide();
    }
}
