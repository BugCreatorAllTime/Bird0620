using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
public class AdController : BaseController
{
    [HideInInspector] public GoogleAdMobController GoogleAdMobController;
    
    [HideInInspector] public Action InterCloseCallback;
    
    [HideInInspector] public Action RewardCloseCallback;

    [HideInInspector] public bool IsWaitLoadDoneAndShowReward = false; 
    
    private DateTime _lastTimeShowAds = DateTime.MinValue;
    private DateTime _lastTimeShowVideoAds = DateTime.MinValue;
    private DateTime _lastTimeLoadRewardAd = DateTime.MinValue;

    private Coroutine _showBannerCoroutine;

    private void Awake()
    {
        GoogleAdMobController = GetComponentInChildren<GoogleAdMobController>(true);

        _lastTimeShowAds = DateTime.MinValue;
        _lastTimeShowVideoAds = DateTime.MinValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Load an app open ad when the scene starts
        AppOpenAdManager.Instance.LoadAd();
        

        AppOpenAdManager.Instance.ShowCallback += HideBannerAds;
        AppOpenAdManager.Instance.CloseCallback += delegate
        {

            DelayShowBannerAds();
        };

        GoogleAdMobController.ShowCallback += delegate
        {
            if (_showBannerCoroutine != null) StopCoroutine(_showBannerCoroutine);
            HideBannerAds();
        };
        GoogleAdMobController.CloseCallback += DelayShowBannerAds;

    }

    void DelayShowBannerAds()
    {
        if (_showBannerCoroutine != null) StopCoroutine(_showBannerCoroutine);
        _showBannerCoroutine = StartCoroutine(ShowBannerAdsCoroutine());
    }
    
    IEnumerator ShowBannerAdsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        ShowBannerAds();
    }

    public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded
        if (!paused)
        {
            Debug.Log("Pause Game: " + paused + " " + GameInfo.IsPauseByShowOtherAds);
            if (!GameInfo.IsPauseByShowOtherAds)
            {
                Debug.Log("APP OPEN SHOW AD IF AVAILABLE <--");
                AppOpenAdManager.Instance.ShowAdIfAvailable();
            } else
            {
                GameInfo.IsPauseByShowOtherAds = false;
            }
        }
    }

    public void InitAds()
    {
        if (!ConfigGame.EnableAds) return;

        GoogleAdMobController.SetConsent(true);
        GoogleAdMobController.InitAdmob();
        
        GoogleAdMobController.OnAdClosedEvent.AddListener(() =>
        {
            Debug.Log("Ad Inter Close Callback");
            HandleInterAdClose();
        });
    }

    public bool IsInitialized()
    {
        return GoogleAdMobController.IsInitialized;
    }

    public void LoadBannerBotWhenLoadSceneGameDone()
    {
        if (!ConfigGame.EnableAds) return;

        if (DataManager.Instance.UserData.IsRemoveAds)
        {
            Debug.Log("Is No Ads - Not Show Ads");
        } else {

            Debug.Log("Show Ads Banner Bot");
            GoogleAdMobController.RequestBannerAd();
            GoogleAdMobController.ShowBanner();
        }
    }

    public void HideBannerAds()
    {
        if (!ConfigGame.EnableAds) return;
        Debug.Log("==> Hide Banner Ads");
        GoogleAdMobController.HideBanner();
    }
    
    public void DestroyBannerAds()
    {
        if (!ConfigGame.EnableAds) return;
        
        GoogleAdMobController.DestroyBannerAd();
    }
    
    public void DestroyInterAds()
    {
        if (!ConfigGame.EnableAds) return;

        GoogleAdMobController.DestroyInterstitialAd();
    }

    public void ShowBannerAds()
    {
        if (!ConfigGame.EnableAds) return;

        if (DataManager.Instance.UserData.IsRemoveAds) return;
        
        Debug.Log("==> Show Banner Ads");
        GoogleAdMobController.ShowBanner();
    }

    public void LoadInterstitial()
    {
        if (!ConfigGame.EnableAds) return;

        if (DataManager.Instance.UserData.IsRemoveAds)
        {
            Debug.Log("Is No Ads - Not Show Interstitial Ads");
        }
        else
        {
            GoogleAdMobController.RequestAndLoadInterstitialAd();
        }
    }

    public void ShowFullAdsWithMinDelay(Action closeCallback = null)
    {
        #if UNITY_EDITOR
        closeCallback?.Invoke();
        return;
        #endif
        
        if (!ConfigGame.EnableAds)
        {
            closeCallback?.Invoke();
            return;
        }

        if (DataManager.Instance.UserData.IsRemoveAds)
        {
            closeCallback?.Invoke();
            return;
        }

        if (DataManager.Instance.UserData.CurrentLevel >= ConfigGame.LevelStartForShowInterAd
           && (_lastTimeShowAds == DateTime.MinValue && (DateTime.Now - MainController.TimeStart).TotalSeconds > ConfigGame.WaitTimeBetweenShowInterAd || _lastTimeShowAds != DateTime.MinValue && (DateTime.Now - _lastTimeShowAds).TotalSeconds > ConfigGame.WaitTimeBetweenShowInterAd)
           && (_lastTimeShowVideoAds == DateTime.MinValue || _lastTimeShowVideoAds != DateTime.MinValue && (DateTime.Now - _lastTimeShowVideoAds).TotalSeconds > ConfigGame.WaitTimeAfterShowVideoAd)
        ){
            Debug.Log("==> Show Full Ads Delay");
            if (GoogleAdMobController.IsInterstitialAdLoaded())
            {
                GoogleAdMobController.ShowInterstitialAd();
                InterCloseCallback = null;
                if(closeCallback != null) InterCloseCallback = closeCallback;
            }
            else
            {
                GoogleAdMobController.LoadInterstitialAd();
                closeCallback?.Invoke();
            }
        }
        else
        {
            closeCallback?.Invoke();   
        }
    }

    void HandleInterAdClose()
    {
        InterCloseCallback?.Invoke();
        
        _lastTimeShowAds = DateTime.Now;

        if (!GoogleAdMobController.IsInterstitialAdLoaded()){
            GoogleAdMobController.LoadInterstitialAd();
        }
    }

    private bool _isUseRewardInter = false;
    public void LoadRewardAds()
    {
        if (!ConfigGame.EnableAds) return;

        int rand = UnityEngine.Random.Range(0, 101);
        if (rand <= (100 - ConfigGame.RateShowRewardInter))
        {
            GoogleAdMobController.RequestAndLoadRewardedAd();
            _isUseRewardInter = false;
        } else
        {
            GoogleAdMobController.RequestAndLoadRewardedInterstitialAd();
            _isUseRewardInter = true;
        }
    }

    public void SetResetWaitShowReward()
    {
        IsWaitLoadDoneAndShowReward = false;
        RewardCloseCallback = null;
    }

    public void HandleLoadRewardAdsSuccess()
    {
        MainController.UIGame.StopLoadingWatchRewardAd();
        
        if (!IsWaitLoadDoneAndShowReward || RewardCloseCallback == null) return;

        IsWaitLoadDoneAndShowReward = false;

        ReShowRewardAds();
    }
    
    public void HandleLoadRewardAdsFail(int code)
    {
        MainController.UIGame.StopLoadingWatchRewardAd();
        Debug.Log("IsWaitLoadDoneAndShowReward: " + IsWaitLoadDoneAndShowReward);
        Debug.Log("RewardCloseCallback: " + RewardCloseCallback);
        
        if (!IsWaitLoadDoneAndShowReward || RewardCloseCallback == null) return;
        
        IsWaitLoadDoneAndShowReward = false;
        Debug.Log("Show popup alert");
        
        MainController.Popup.PopupAlert.Show("Something wrong! Cannot load Ad.");
    }

    public void ShowRewardAds(Action closeCallback = null, bool reShow = false)
    {
#if UNITY_EDITOR
        closeCallback?.Invoke();
        return;
#endif
        
        if (!ConfigGame.EnableAds) return;
        
        IsWaitLoadDoneAndShowReward = reShow;

        MainController.UIGame.StartLoadingWatchRewardAd();
        
        if (!_isUseRewardInter)
            ShowRewardVideoAds(closeCallback);
        else
            ShowRewardInterAds(closeCallback);
    }

    public void ReShowRewardAds()
    {
        Debug.Log("==> Re Show Reward Ads <==");
        if (!_isUseRewardInter)
            GoogleAdMobController.ShowRewardAds(() =>
            {
                HandleWhenRewardAds(RewardCloseCallback);
            });
        else
            GoogleAdMobController.ShowRewardedInterstitialAd(() =>
            {
                HandleWhenRewardAds(RewardCloseCallback);
            });
    }

    public void ShowRewardVideoAds(Action closeCallback = null)
    {
        Debug.Log(" -- Show Reward Video Ads -- ");
        if (GoogleAdMobController.IsRewardAdsLoaded())
        {
            IsWaitLoadDoneAndShowReward = false;
            RewardCloseCallback = null;
            MainController.UIGame.StopLoadingWatchRewardAd();
            
            GoogleAdMobController.ShowRewardAds(() =>
            {
                HandleWhenRewardAds(closeCallback);
            });
        }
        else {
            HandleWhenRewardAdNotLoaded(closeCallback);
        }
    }
    
    public void ShowRewardInterAds(Action closeCallback = null)
    {
        Debug.Log(" -- Show Reward Inter Ads -- ");
        if (GoogleAdMobController.IsRewardInterLoaded())
        {
            IsWaitLoadDoneAndShowReward = false;
            RewardCloseCallback = null;
            MainController.UIGame.StopLoadingWatchRewardAd();
            
            GoogleAdMobController.ShowRewardedInterstitialAd(() =>
            {
                HandleWhenRewardAds(closeCallback);
            });
        } 
        else {
            HandleWhenRewardAdNotLoaded(closeCallback);
        }
    }

    void HandleWhenRewardAds(Action closeCallback = null) {
        _lastTimeShowAds = DateTime.Now;
        closeCallback?.Invoke();
    }

    void HandleWhenRewardAdNotLoaded(Action closeCallback = null)
    {
        _lastTimeLoadRewardAd = DateTime.Now;
        
        if (!MainController.CheckInternetReachable())
        {
            MainController.Popup.PopupAlert.Show("Something wrong! Cannot load Ad.");
            
            IsWaitLoadDoneAndShowReward = false;
            
            MainController.UIGame.StopLoadingWatchRewardAd();

            return;
        }
            
        LoadRewardAds();

        if (closeCallback != null) RewardCloseCallback = closeCallback;
    }
    
    public void ReloadRewardAndInterAds()
    {
        if (!ConfigGame.EnableAds) return;

        if(!GoogleAdMobController.IsInterstitialAdLoaded()) GoogleAdMobController.RequestAndLoadInterstitialAd();
        if(!GoogleAdMobController.IsRewardAdsLoaded()) GoogleAdMobController.RequestAndLoadRewardedAd();
    }

    IEnumerator OnShowAdSuccess(Action closeCallback = null)
    {
        yield return new WaitForEndOfFrame();
        InvokeAction(closeCallback);
    }

    public bool IsRewardLoaded()
    {
        #if UNITY_EDITOR
        return true;
        #endif
        return GoogleAdMobController.IsRewardAdsLoaded() || GoogleAdMobController.IsRewardInterLoaded();
    }
}
