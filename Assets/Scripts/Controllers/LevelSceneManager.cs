using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Puzzle;
using Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSceneManager : BaseController
{
    private int _currentLevel;

    private int _levelForRate;

    private List<string> _levelNameUnloads = new List<string>();

    public int CurrentLevel
    {
        get { return _currentLevel; }
    }

    public override void Awake()
    {
        base.Awake();
        if (MainController != null)
        {
            MainController.LevelSceneManager = this;
        }

        _currentLevel = DataManager.Instance.UserData.CurrentLevel;
        if (_currentLevel < 0) _currentLevel = 0;
    }

    public override void Start()
    {
        base.Start();
        DontDestroyOnLoad(this);
        
        // SceneManager.sceneLoaded += (scene, mode) => { MainController.Popup.HideAll(); };

        _levelForRate = ConfigGame.FirstLevelShowRate;
        if (_currentLevel > _levelForRate) _levelForRate = _currentLevel + ConfigGame.NumberLevelBetweenShowRate;
    }

    private float _timeCheck = 60f;
    private float _tick = 0f;

    // Update is called once per frame
    void Update()
    {
        if (GameInfo.IsPopupNoInternetShow) return;

        if (_currentLevel + 1 < ConfigGame.LevelStartCheckInternet) return;

        MainController.CheckInternetReachable();

        if (!ConfigGame.CheckOnlyInternetReachAbility)
        {
            if (_tick >= _timeCheck)
            {
                MainController.CheckInternetConnect();
                _tick = 0f;
            }

            _tick += Time.deltaTime;
        }

        if (GameInfo.IsLastestCheckNetworkFail || GameInfo.IsLastestCheckInternetFail)
        {
            MainController.Popup.PopupNoInternet.Show();
        }
    }

    public void LoadNextLevel()
    {
        int currentLevel = _currentLevel >= CONST.MAX_LEVEL || ConfigGame.LevelOrderList.Count <= 0
            ? -1: _currentLevel + 1;

        var data = RSDManager.Instance.GetLevelData(currentLevel);
        MainController.Game.InitLevel(data);

        // _levelNameUnloads.Add(currentLevelName);

        if (currentLevel == -1)
        {
            MainController.UIGame.SetUIComingSoon();
        }
        else
        {
            MainController.UIGame.SetUIOnPlaying();
        }

        MainController.UIGame.SetLevelTitle(_currentLevel);

        DOVirtual.DelayedCall(0.25f, () =>
        {
            TutorialManager.Instance.CheckTutorial(currentLevel);
        });

        Debug.Log("Check Scene Manager: " + SceneManager.GetActiveScene() + " " + SceneManager.sceneCount);
        // if(_levelNameUnloads.Count > 2){
        //     Debug.Log("Previous Level Name: " + _levelNameUnloads[0] + " " + _levelNameUnloads.Count);
        //     if(!string.IsNullOrEmpty(_levelNameUnloads[0])) SceneManager.UnloadSceneAsync(_levelNameUnloads[0]);
        //     _levelNameUnloads.RemoveAt(0);
        // }

        // Some GC Collect
        Resources.UnloadUnusedAssets();
        GC.Collect();

        // StartCoroutine(PreloadNextScene());
    }

    public void CheckInternet()
    {
        Debug.Log("==> Check Internet: " + _currentLevel + " " + ConfigGame.LevelStartCheckInternet);

        if (_currentLevel + 1 >= ConfigGame.LevelStartCheckInternet)
        {
            if (MainController.CheckInternetReachable())
            {
                if (!ConfigGame.CheckOnlyInternetReachAbility)
                {
                    // Ignoore, has connect network provider
                    MainController.Popup.PopupLoading.Show();
                    MainController.CheckInternetConnect();
                }
                else
                {
                    if (GameInfo.IsLastestCheckNetworkFail)
                    {
                        MainController.AdController.ReloadRewardAndInterAds();
                    }
                }
            }
            else
            {
                MainController.Popup.PopupNoInternet.Show();
            }
        }
    }

    public void WinLevel()
    {
        CalculateCurrentLevel();
        ShowRate();
    }

    public void NextLevel()
    {
        WinLevel();
        LoadNextLevel();
        // MainController.UIGame.ShowInterAdnLoadNextLevel();
    }

    public void SkipLevel()
    {
        CalculateCurrentLevel();
        LoadNextLevel();
    }

    public void GotoLevel(int level)
    {
        if (CalculateLevelCanGo(level))
        {
            LoadNextLevel();
        }
    }

    void ShowRate()
    {
        if (DataManager.Instance.SettingData.HasRate) return;

        if (_currentLevel == _levelForRate) StartCoroutine(DelayShowRate());
        else if (_levelForRate < _currentLevel)
        {
            _levelForRate += ConfigGame.NumberLevelBetweenShowRate;
        }
    }

    IEnumerator DelayShowRate()
    {
        // Add delay show ui win before show rate for scene to completed
        yield return new WaitForSeconds(ConfigGame.DelayShowUIWin + 0.5f);
        MainController.Popup.PopupRate.Show();
    }

    string GetLevelName(int index)
    {
        try
        {
            index = Mathf.Clamp(index, 0, ConfigGame.LevelOrderList.Count - 1);
            string nameLevel = ConfigGame.LevelOrderList[index];
            GameInfo.CurrentLevelId = nameLevel;

            if (ConfigGame.LevelSpecialDic.Count > 0)
            {
                if (ConfigGame.LevelSpecialDic.ContainsKey(nameLevel))
                {
                    nameLevel += "_" + ConfigGame.LevelSpecialDic[nameLevel];
                }
            }

            return nameLevel;
        }
        catch (Exception e)
        {
            return "LevelComingSoon";
        }
    }

    void CalculateCurrentLevel()
    {
        if (_currentLevel >= GetNumLevel() - 1)
        {
            _currentLevel = 0;
        }
        else
        {
            _currentLevel += 1;
        }

        DataManager.Instance.UserData.CurrentLevel = _currentLevel;
    }

    public void CalculateReplay()
    {
        DataManager.Instance.UserData.CurrentLevel = _currentLevel;
    }

    public bool CalculateLevelCanGo(int level)
    {
        if (level >= GetNumLevel() || level < 1) return false;

        _currentLevel = level - 1;
        DataManager.Instance.UserData.CurrentLevel = _currentLevel;
        return true;
    }

    public int GetNumLevel()
    {
        return ConfigGame.LevelOrderList.Count;
    }
}