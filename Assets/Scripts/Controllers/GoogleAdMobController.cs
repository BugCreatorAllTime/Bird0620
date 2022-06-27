using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;

public class GoogleAdMobController : BaseController
{
    private BannerView _bannerView;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardedAd;
    private RewardedInterstitialAd _rewardedInterstitialAd;
    private float _deltaTime;
    
    private bool _isAcceptConsent;

    public UnityEvent OnAdClosedEvent;
    
    private bool _showFpsMeter;

    private bool _isInitialized = false;

    private bool _loadOneTimeWhenFailRewardAd = false;
    private bool _loadOneTimeWhenFailRewardInter = false;
    
    [HideInInspector] public Action ShowCallback; // call when inter, video, inter reward show
    [HideInInspector] public Action CloseCallback; // call when inter, video, inter reward close or show fail

    public bool IsInitialized { get { return _isInitialized; } }

    public enum AdType
    {
        Banner,
        Interstitial,
        RewardVideo,
        RewardInterstitial,
        OpenAd
    }

    #region UNITY MONOBEHAVIOR METHODS

    private void Awake()
    {
        
    }

    public void Start()
    {
        
    }

    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() => {

            Debug.Log("Initialization complete");

            _isInitialized = true;
            
#if UNITY_IOS
            MobileAds.SetiOSAppPauseOnBackground(true);
#endif
            
            // RequestBannerAd();
            // RequestAndLoadInterstitialAd();
        });
    }

    public void InitAdmob()
    {
        // MobileAds.SetiOSAppPauseOnBackground(true);

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }
    
    public void SetConsent(bool isAcceptConsent)
    {
        _isAcceptConsent = isAcceptConsent;
    }

    private void Update()
    {
        // Some handle event banner ad callback
        if (GameInfo.IsBannerShowCallback) HandBannerShow();

        // Some handle event inter ad callback
        if (GameInfo.IsInterShowCallback) HandleOnAdShow();
        
        if (GameInfo.IsInterShowFailCallback) HandleOnAdShowFail();

        if (GameInfo.IsInterCloseCallback) HandleOnAdClosed();

        if (GameInfo.IsInterLoadedCallback) HandleOnAdLoaded();

        if (GameInfo.IsInterLoadFailCallback) HandleOnAdLoadFail();

        // Some handle event reward ad callback
        if (GameInfo.IsRewardShowCallback) HandleRewardedAdOpening();
        
        if (GameInfo.IsRewardShowFailCallback) HandleRewardedAdFailedToShow();

        if (GameInfo.IsRewardCloseCallback) HandleRewardedAdClosed();

        if (GameInfo.IsRewardLoadedCallback) HandleRewardedAdLoaded();

        //if (GameInfo.IsRewardLoadFailCallback) HandleRewardedAdFailedToLoad();

        if (GameInfo.IsRewardEarnedCallback) HandleUserEarnedReward();
    }

    #endregion

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        return _isAcceptConsent
            ? new AdRequest.Builder().Build()
            : new AdRequest.Builder().AddExtra("npa", "1").Build();
    }

    string GetAdUnitId(AdType type)
    {
        string adUnitId = "unused";

        switch (type)
        {
            case AdType.Banner:
                #if UNITY_ANDROID
                adUnitId = PLUGIN_ID.ADMOB_ANDROID_BANNER;
                #elif UNITY_IOS || UNITY_IPHONE
                adUnitId = PLUGIN_ID.ADMOB_IOS_BANNER;
                #endif
                
                break;
            case AdType.Interstitial:
                #if UNITY_ANDROID
                adUnitId = PLUGIN_ID.ADMOB_ANDROID_INTERSTITIAL;
                #elif UNITY_IOS || UNITY_IPHONE
                adUnitId = PLUGIN_ID.ADMOB_IOS_INTERSTITIAL;
                #endif
                
                break;
            case AdType.RewardVideo:
                #if UNITY_ANDROID
                adUnitId = PLUGIN_ID.ADMOB_ANDROID_REWARD;
                #elif UNITY_IOS || UNITY_IPHONE
                adUnitId = PLUGIN_ID.ADMOB_IOS_REWARD;
                #endif
                
                break;
            case AdType.RewardInterstitial:
                #if UNITY_ANDROID
                adUnitId = PLUGIN_ID.ADMOB_ANDROID_REWARD_INTER;
                #elif UNITY_IOS || UNITY_IPHONE
                adUnitId = PLUGIN_ID.ADMOB_IOS_REWARD_INTER;
                #endif
                
                break;

        }

        return adUnitId;
    }

#endregion

#region BANNER ADS

    public void RequestBannerAd()
    {
        Debug.Log("Requesting Banner Ad.");
        // These ad units are configured to always serve test ads.
        string adUnitId = GetAdUnitId(AdType.Banner);

        // Clean up banner before reusing
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        _bannerView.OnAdOpening += (sender, args) =>
        {
            GameInfo.IsBannerShowCallback = true;
        };

        _bannerView.OnAdLoaded += HandleBannerLoaded;

        // Load a banner ad
        _bannerView.LoadAd(CreateAdRequest());
        _bannerView.Hide();
    }

    public void HandleBannerLoaded(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if(MainController != null && MainController.UIGame != null)
            {
                MainController.UIGame.SetSizeBannerAd(_bannerView.GetHeightInPixels());
            }
        });
    }

    void HandBannerShow()
    {
        GameInfo.IsBannerShowCallback = false;
    }

    public void DestroyBannerAd()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
        }
    }
    
    public void HideBanner()
    {
        _bannerView?.Hide();
    }

    public void ShowBanner()
    {
        if (_bannerView != null) {
            _bannerView.Show();
        } else {
            RequestBannerAd();
        }
    }

#endregion

#region INTERSTITIAL ADS

    public void LoadInterstitialAd()
    {
        Debug.Log("--> Load new Ad");
        RequestAndLoadInterstitialAd();
        return;

#if UNITY_IOS || UNITY_IPHONE
        RequestAndLoadInterstitialAd();
#else
        if (_interstitialAd != null)
        {
            _interstitialAd.LoadAd(CreateAdRequest());
        } else {
            RequestAndLoadInterstitialAd();
        }
#endif
    }

    public void RequestAndLoadInterstitialAd()
    {
        string adUnitId = GetAdUnitId(AdType.Interstitial);

        // Clean up interstitial before using it
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }

        _interstitialAd = new InterstitialAd(adUnitId);

        _interstitialAd.OnAdClosed += (sender, args) =>
        {
            GameInfo.IsInterCloseCallback = true;
        };

        _interstitialAd.OnAdOpening += (sender, args) =>
        {
            GameInfo.IsInterShowCallback = true;
        };
        
        _interstitialAd.OnAdFailedToShow += (sender, args) =>
        {
            GameInfo.IsInterShowFailCallback = true;
        };

        _interstitialAd.OnAdLoaded += (sender, args) =>
        {
            GameInfo.IsInterLoadedCallback = true;
        };

        _interstitialAd.OnAdFailedToLoad += (sender, args) =>
        {
            GameInfo.IsInterLoadFailCallback = true;
        };

        // Load an interstitial ad
        _interstitialAd.LoadAd(CreateAdRequest());
    }
    
    void HandleOnAdClosed()
    {
        GameInfo.IsInterCloseCallback = false;

        Debug.Log("==> Handle On Ad Closed");
        
        OnAdClosedEvent?.Invoke();
        
        CloseCallback?.Invoke();
    }
    
    void HandleOnAdShow()
    {
        GameInfo.IsInterShowCallback = false;

        Debug.Log("==> Handle On Ad Show");
    }

    void HandleOnAdShowFail()
    {
        Debug.Log("==> Handle On Ad Show FAIL");

        GameInfo.IsPauseByShowOtherAds = false;
        GameInfo.IsInterShowFailCallback = false;
        
        CloseCallback?.Invoke();
    }
    
    void HandleOnAdLoaded()
    {
        GameInfo.IsInterLoadedCallback = false;

        Debug.Log("==> Handle On Ad loaded");
    }
    
    void HandleOnAdLoadFail()
    {
        GameInfo.IsInterLoadFailCallback = false;

        Debug.Log("==> Handle On Ad load fail");
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd.IsLoaded())
        {
            GameInfo.IsPauseByShowOtherAds = true;
            _interstitialAd.Show();
            
            ShowCallback?.Invoke();
        } else {
            Debug.Log("Interstitial ad is not ready yet");
        }
    }

    IEnumerator DelayLoadNewInterAd()
    {
        yield return new WaitForSeconds(0.1f);
        LoadInterstitialAd();
    }

    public void DestroyInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }
    }
    
    public bool IsInterstitialAdLoaded()
    {
        if (_interstitialAd == null) return false;
        return _interstitialAd.IsLoaded();
    }
#endregion
#region REWARDED ADS

    public void RequestAndLoadRewardedAd()
    {
        string adUnitId = GetAdUnitId(AdType.RewardVideo);

        _rewardedAd = new RewardedAd(adUnitId);

        //// Called when an ad request has successfully loaded.
        //_rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        //// Called when an ad request failed to load.
        _rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        //// Called when an ad is shown.
        //_rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        //// Called when an ad request failed to show.
        //_rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        //// Called when the user should be rewarded for interacting with the ad.
        //_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        //// Called when the ad is closed.
        //_rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        _rewardedAd.OnAdClosed += (sender, args) =>
        {
            GameInfo.IsRewardCloseCallback = true;
        };

        _rewardedAd.OnAdOpening += (sender, args) =>
        {
            GameInfo.IsRewardShowCallback = true;
        };

        _rewardedAd.OnAdLoaded += (sender, args) =>
        {
            GameInfo.IsRewardLoadedCallback = true;
        };

        _rewardedAd.OnAdFailedToLoad += (sender, args) =>
        {
            GameInfo.IsRewardLoadFailCallback = true;
        };

        _rewardedAd.OnUserEarnedReward += (sender, args) =>
        {
            GameInfo.IsRewardEarnedCallback = true;
        };
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        _rewardedAd.LoadAd(request);
    }
    
    public void HandleRewardedAdLoaded()
    {
        GameInfo.IsRewardLoadedCallback = false;

        // reset load one time when fail
        _loadOneTimeWhenFailRewardAd = false;
        
        MonoBehaviour.print("HandleRewardedAdLoaded event received");
        MainController.AdController.HandleLoadRewardAdsSuccess();
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            GameInfo.IsRewardLoadFailCallback = false;

            if (!_loadOneTimeWhenFailRewardAd)
            {
                RequestAndLoadRewardedAd();
                _loadOneTimeWhenFailRewardAd = true;
            }
            else
            {
                MainController.AdController.HandleLoadRewardAdsFail(0);
            }

            MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message");
        });
    }

    public void HandleRewardedAdOpening()
    {
        GameInfo.IsRewardShowCallback = false;

        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow()
    {
        MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message");
        GameInfo.IsPauseByShowOtherAds = false;
        GameInfo.IsRewardShowFailCallback = false;
        
        CloseCallback?.Invoke();
    }

    public void HandleRewardedAdClosed()
    {
        GameInfo.IsRewardCloseCallback = false;

        MonoBehaviour.print("HandleRewardedAdClosed event received");

        if (!IsRewardAdsLoaded())
        {
            MainController.AdController.LoadRewardAds();
        }
        
        CloseCallback?.Invoke();
    }

    public void HandleUserEarnedReward()
    {
        GameInfo.IsRewardEarnedCallback = false;

        MonoBehaviour.print("HandleRewardedAdRewarded event received");
        
        _rewardCallback?.Invoke();
    }

    private Action _rewardCallback;
    public void ShowRewardAds(Action callback, Action skipRewardCallback = null)
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Reward is loaded: " + _rewardedAd.IsLoaded());
            _rewardedAd.Show();
            _rewardCallback = callback;
            
            GameInfo.IsPauseByShowOtherAds = true;
            
            ShowCallback?.Invoke();
        } else {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }
    
    public bool IsRewardAdsLoaded()
    {
        if (_rewardedAd == null) return false;
        return _rewardedAd.IsLoaded();
    }
#endregion

    private bool _isRewardInterLoaded;
    public void RequestAndLoadRewardedInterstitialAd()
    {
        Debug.Log("Requesting Rewarded Interstitial Ad.");
        // These ad units are configured to always serve test ads.
        string adUnitId = GetAdUnitId(AdType.RewardInterstitial);

        // Create an interstitial.
        RewardedInterstitialAd.LoadAd(adUnitId, CreateAdRequest(), (rewardedInterstitialAd, error) =>
        {

            if (error != null)
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    print("RewardedInterstitialAd load failed, error: " + error);

                    _isRewardInterLoaded = false;

                    if (!_loadOneTimeWhenFailRewardInter)
                    {
                        RequestAndLoadRewardedInterstitialAd();
                        _loadOneTimeWhenFailRewardInter = true;
                    } else
                    {
                        #if UNITY_IOS || UNITY_IPHONE
                        MainController.AdController.HandleLoadRewardAdsFail(0);
                        #else
                        MainController.AdController.HandleLoadRewardAdsFail(0);
                        #endif
                    }
                });
                return;
            }

            _rewardedInterstitialAd = rewardedInterstitialAd;
            _isRewardInterLoaded = false;
            MobileAdsEventExecutor.ExecuteInUpdate(() => {
                print("RewardedInterstitialAd loaded");

                _isRewardInterLoaded = true;

                _loadOneTimeWhenFailRewardInter = false;
                
                MainController.AdController.HandleLoadRewardAdsSuccess();

                // if (MainController != null && MainController.UIGame != null) MainController.UIGame.StopLoadingWatchRewardAd();
            });

            // Register for ad events.
            _rewardedInterstitialAd.OnAdDidPresentFullScreenContent += HandleAdDidPresent;
            _rewardedInterstitialAd.OnAdDidDismissFullScreenContent += HandleAdDidDismiss;
            _rewardedInterstitialAd.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresent;
        });
    }

    private void HandleAdFailedToPresent(object sender, AdErrorEventArgs args)
    {
        _rewardedInterstitialAd = null;
        MobileAdsEventExecutor.ExecuteInUpdate(() => {
            print("Rewarded Interstitial failed to present.");
            GameInfo.IsPauseByShowOtherAds = false;
            
            CloseCallback?.Invoke();
        });
    }

    private void HandleAdDidPresent(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() => {
            print("Rewarded Interstitial presented.");
        });
    }

    private void HandleAdDidDismiss(object sender, EventArgs args)
    {
        _rewardedInterstitialAd = null;

        MobileAdsEventExecutor.ExecuteInUpdate(() => {
            print("Rewarded Interstitial dismissed.");

            if (!IsRewardInterLoaded())
            {
                MainController.AdController.LoadRewardAds();
            }
            
            CloseCallback?.Invoke();
        });
    }

    public void ShowRewardedInterstitialAd(Action callback)
    {
        if (_rewardedInterstitialAd != null)
        {
            GameInfo.IsPauseByShowOtherAds = true;
            _rewardedInterstitialAd.Show((reward) => {
                MobileAdsEventExecutor.ExecuteInUpdate(() => {
                    callback?.Invoke();
                });
            });
            
            ShowCallback?.Invoke();
        } else {
            print("Rewarded ad is not ready yet.");
        }
    }

    public bool IsRewardInterLoaded()
    {
        if (_rewardedInterstitialAd == null) return false;
        return _isRewardInterLoaded;
    }
}
