using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSetting : BaseController
{
    public SettingEnum SettingType;

    public GameObject On;
    public GameObject Off;

    public enum SettingEnum
    {
        MUSIC,
        SOUND,
        VIBRATION
    }

    private Toggle _toggle;


    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDefault();

        _toggle.onValueChanged.AddListener((val) => {
            SetOnOff(val);
        });
    }

    void SetOnOff(bool flag = true)
    {
        if (flag)
        {
            On.SetActive(false);
            Off.SetActive(true);
        }
        else
        {
            On.SetActive(true);
            Off.SetActive(false);
        }
        if (SettingType == SettingEnum.SOUND)
        {
            DataManager.Instance.SettingData.Sound = !_toggle.isOn;
        }
        else if (SettingType == SettingEnum.MUSIC)
        {
            DataManager.Instance.SettingData.Music = !_toggle.isOn;
        }
        else if (SettingType == SettingEnum.VIBRATION)
        {
            DataManager.Instance.SettingData.Vibration = !_toggle.isOn;
        }
    }

    public void SetDefault()
    {
        var settingData = DataManager.Instance.SettingData;
        if (SettingType == SettingEnum.SOUND)
        {
            _toggle.isOn = !settingData.Sound;
            On.SetActive(settingData.Sound);
            Off.SetActive(!settingData.Sound);
        }

        if (SettingType == SettingEnum.MUSIC)
        {
            _toggle.isOn = !settingData.Music;
            On.SetActive(settingData.Music);
            Off.SetActive(!settingData.Music);
        }

        if (SettingType == SettingEnum.VIBRATION)
        {
            _toggle.isOn = !settingData.Vibration;
            On.SetActive(settingData.Vibration);
            Off.SetActive(!settingData.Vibration);
        }
    }
}
