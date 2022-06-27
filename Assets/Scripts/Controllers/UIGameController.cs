using System.Collections;
using Components;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGameController : BaseController
{
    public GameObject PanelTop;
    public Button BtnSetting;
    public Button BtnReplay;
    public TextMeshProUGUI LevelTitle;
    
    public TutorialHand tutorialHand;

    public TextMeshProUGUI shuffleText;

    public GameObject PanelBannerAd;

    [Header("Item Buttons")] public ButtonItem BtnAddBranch;
    public ButtonItem BtnUndo;
    public ButtonItem BtnShuffle;
    public Button BtnSkip;

    private int _currentGem;

    [HideInInspector] public ButtonAd buttonAdsTrigger;

    private void Awake()
    {
        if (MainController != null)
        {
            MainController.UIGame = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        BtnSetting.onClick.AddListener(() =>
        {
            if (ConfigGame.ShowInterWhenClickButton)
            {
                MainController.UIGame.ShowInterAds("ClickBtnSetting");
            }

            MainController.Popup.PopupSetting.Show();
        });

        BtnReplay.onClick.AddListener(() =>
        {
            // if (ConfigGame.ShowInterWhenClickButton)
            // {
                MainController.UIGame.ShowInterAds("ClickBtnReplay");
            // }

            HandleReplay();
        });

        BtnAddBranch.button.onClick.AddListener(() =>
        {
            if (MainController.Game.CanAddBranch())
            {
                var saveGameData = DataManager.Instance.SaveGameData;

                if (saveGameData.freeBranch > 0)
                {
                    MainController.Game.AddBranch();
                    UpdateItemButtons();
                }
                else
                {
                    MainController.UIGame.buttonAdsTrigger = BtnAddBranch.GetComponent<ButtonAd>();
                    MainController.AdController.ShowRewardAds(() =>
                    {
                        MainController.Game.AddBranch();
                        DOVirtual.DelayedCall(0.1f, UpdateItemButtons);
                    });
                }
            }
        });

        BtnShuffle.button.onClick.AddListener(() =>
        {
            var saveGameData = DataManager.Instance.SaveGameData;
            // If not in shuffle mode, turn on shuffle mode
            if (!MainController.Game.IsShuffleMode())
            {
                if (saveGameData.freeShuffle > 0)
                {
                    TurnOnUIShuffle();
                    MainController.Game.ToggleShuffleMode();
                }
                else
                {
                    MainController.UIGame.buttonAdsTrigger = BtnShuffle.GetComponent<ButtonAd>();
                    MainController.AdController.ShowRewardAds(() =>
                    {
                        TurnOnUIShuffle();
                        MainController.Game.ToggleShuffleMode();
                    });
                }
            }
            else
            {
                TurnOffUIShuffle();
                MainController.Game.ToggleShuffleMode();
            }
            UpdateItemButtons();
        });

        BtnUndo.button.onClick.AddListener(() =>
        {
            if (MainController.Game.CanUndo())
            {
                var saveGameData = DataManager.Instance.SaveGameData;
                if (saveGameData.freeUndo > 0)
                {
                    MainController.Game.Undo();
                }
                else
                {
                    MainController.UIGame.buttonAdsTrigger = BtnUndo.GetComponent<ButtonAd>();
                    MainController.AdController.ShowRewardAds(() => { MainController.Game.Undo(); });
                }

                UpdateItemButtons();
            }
        });

        BtnSkip.onClick.AddListener(() =>
        {
            MainController.UIGame.buttonAdsTrigger = BtnSkip.GetComponent<ButtonAd>();
            MainController.AdController.ShowRewardAds(() => { MainController.LevelSceneManager.NextLevel(); });
        });

        // StartCoroutine(EffWhenStart());

        MainController.Popup.HideAll();

        if (CONST.VERSION_CODE < ConfigGame.LastestVersionCode)
        {
            MainController.Popup.PopupUpdateNewVersion.Show();
        }
        
        InitBannerAd();
    }

    private bool _isLoadRewardFirstTime = false;

    private void Update()
    {
        if (_isLoadRewardFirstTime) return;

        if (MainController.AdController.IsInitialized())
        {
            MainController.AdController.LoadInterstitial();
            MainController.AdController.LoadRewardAds();
            _isLoadRewardFirstTime = true;
        }
    }

    IEnumerator EffWhenStart()
    {
        yield return new WaitForEndOfFrame();

        BtnSetting.gameObject.SetActive(false);
        BtnReplay.gameObject.SetActive(false);
        
        shuffleText.gameObject.SetActive(false);

        // Init item buttons
        var currentLevel = DataManager.Instance.UserData.CurrentLevel + 1;

        if (currentLevel >= ConfigGame.LevelForSkipHelper)
        {
            BtnSkip.gameObject.SetActive(true);
        }
        else
        {
            BtnSkip.gameObject.SetActive(false);
        }
        
        if (currentLevel >= ConfigGame.LevelForUndoHelper)
        {
            BtnUndo.gameObject.SetActive(true);
        }
        else
        {
            BtnUndo.gameObject.SetActive(false);
        }
        
        if (currentLevel >= ConfigGame.LevelForBranchHelper)
        {
            BtnAddBranch.gameObject.SetActive(true);
        }
        else
        {
            BtnAddBranch.gameObject.SetActive(false);
        }
        
        if (currentLevel >= ConfigGame.LevelForShuffleHelper)
        {
            BtnShuffle.gameObject.SetActive(true);
        }
        else
        {
            BtnShuffle.gameObject.SetActive(false);
        }
        
        BtnAddBranch.button.interactable = true;
        
        UpdateItemButtons();
        DisableUndo();

        if (MainController.Game.CanUndo())
        {
        }

        yield return new WaitForSeconds(2f);

        BtnSetting.gameObject.SetActive(true);
        BtnSetting.transform.localScale = Vector3.one * 0.8f;
        BtnSetting.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);

        BtnReplay.gameObject.SetActive(true);
        BtnReplay.transform.localScale = Vector3.one * 0.8f;
        BtnReplay.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
    }

    void HandleReplay()
    {
        DataManager.Instance.SaveGameData.currentStage = 0;
        MainController.LevelSceneManager.CalculateReplay();
        MainController.LevelSceneManager.LoadNextLevel();
    }

    public void SetUIOnPlaying()
    {
        int currentLevel = MainController.LevelSceneManager.CurrentLevel;

        tutorialHand.Hide();
        StartCoroutine(EffWhenStart());

        SetLevelTitle(currentLevel);
    }

    public void TurnOnUIShuffle()
    {
        BtnReplay.interactable = false;
        BtnSetting.interactable = false;
        BtnSkip.interactable = false;
        PanelTop.transform.DOMove(PanelTop.transform.position + 300 * Vector3.up, 0.1f);
        shuffleText.gameObject.SetActive(true);
    }
    
    public void TurnOffUIShuffle()
    {
        BtnReplay.interactable = true;
        BtnSetting.interactable = true;
        BtnSkip.interactable = true;
        PanelTop.transform.DOMove(PanelTop.transform.position - 300 * Vector3.up, 0.1f);
        shuffleText.gameObject.SetActive(false);
    }

    public void UpdateItemButtons()
    {
        BtnShuffle.button.interactable = true;
        BtnSkip.interactable = true;

        if (MainController.Game.CanAddBranch())
        {
            BtnAddBranch.button.interactable = true;
        }
        else
        {
            BtnAddBranch.button.interactable = false;
        }

        var saveGameData = DataManager.Instance.SaveGameData;
        if (saveGameData.freeBranch > 0)
        {
            BtnAddBranch.itemCount.text = saveGameData.freeBranch.ToString();
            BtnAddBranch.ChangeToFreeType();
        }
        else
        {
            BtnAddBranch.ChangeToAdType();
        }

        if (saveGameData.freeShuffle > 0)
        {
            BtnShuffle.itemCount.text = saveGameData.freeShuffle.ToString();
            BtnShuffle.ChangeToFreeType();
        }
        else
        {
            BtnShuffle.ChangeToAdType();
        }

        if (saveGameData.freeUndo > 0)
        {
            BtnUndo.itemCount.text = saveGameData.freeUndo.ToString();
            BtnUndo.ChangeToFreeType();
        }
        else
        {
            BtnUndo.ChangeToAdType();
        }
    }

    public void DisableUndo()
    {
        BtnUndo.button.interactable = false;
    }

    public void EnableUndo()
    {
        BtnUndo.button.interactable = true;
    }

    public void SetUIOnLobby()
    {
        BtnSetting.gameObject.SetActive(true);
        BtnReplay.gameObject.SetActive(false);
        tutorialHand.Hide();
    }

    public void SetUIComingSoon()
    {
        BtnSetting.gameObject.SetActive(false);
        BtnReplay.gameObject.SetActive(false);
        tutorialHand.Hide();
    }

    public void SetUIOnWin()
    {
        MainController.AdController.SetResetWaitShowReward();
        
        tutorialHand.gameObject.SetActive(false);
        BtnShuffle.button.interactable = false;
        BtnAddBranch.button.interactable = false;
        BtnUndo.button.interactable = false;
        BtnSkip.interactable = false;
    }

    public void ShowInterAdnLoadNextLevel()
    {
        MainController.AdController.ShowFullAdsWithMinDelay();
        MainController.LevelSceneManager.LoadNextLevel();
    }

    public void ShowInterAds(string posName)
    {
        MainController.AdController.ShowFullAdsWithMinDelay();
    }

    public void SetUIOnTutorial()
    {
    }

    public void SetLevelTitle(int levelVal)
    {
        LevelTitle.text = "LEVEL " + (levelVal + 1);
    }

    public void InitBannerAd()
    {
        if (DataManager.Instance.UserData.IsRemoveAds)
        {
            MainController.AdController.HideBannerAds();
            PanelBannerAd.gameObject.SetActive(false);
        }
        else
        {
            // return;
            PanelBannerAd.gameObject.SetActive(true);
            StartCoroutine(HandleAfterSceneLoaded());
        }
    }

    public void RemoveBannerAd()
    {
        PanelBannerAd.gameObject.SetActive(false);
    }

    public void StartLoadingWatchRewardAd()
    {
        Debug.Log("Start loadding watch reward ad");
        if (buttonAdsTrigger != null)
        {
            buttonAdsTrigger.SetLoading();
        }
    }


    public void StopLoadingWatchRewardAd()
    {
        if (buttonAdsTrigger != null)
        {
            buttonAdsTrigger.SetLoadDone();
        }
    }

    IEnumerator HandleAfterSceneLoaded()
    {
        yield return new WaitForEndOfFrame();
        MainController.AdController.LoadBannerBotWhenLoadSceneGameDone();
        yield return null;
    }

    public void SetSizeBannerAd(float height)
    {
        if (!DataManager.Instance.UserData.IsRemoveAds)
        {
            PanelBannerAd.SetActive(true);
            Rect safeArea = Screen.safeArea;
            PanelBannerAd.GetComponent<RectTransform>().sizeDelta =
                new Vector2(0, (height + safeArea.y) / GetComponent<Canvas>().scaleFactor + 10);
        }
    }
}