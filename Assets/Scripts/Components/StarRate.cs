using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarRate : MonoBehaviour
{
    private Button _button;

    private Image _starLight;

    public int StarID;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _starLight = transform.GetChild(0).GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDisable();
    }

    public void SetActive()
    {
        _starLight.gameObject.SetActive(true);
    }

    public void SetDisable()
    {
        _starLight.gameObject.SetActive(false);
    }
}
