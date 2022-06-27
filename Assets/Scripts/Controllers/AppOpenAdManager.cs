using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class AppOpenAdManager
{
    private static AppOpenAdManager instance;

    private AppOpenAd ad;

    private bool isShowingAd = false;
    
    private DateTime loadTime;
    
    private DateTime _lastTimeShowAds;
    private bool _loadedOneMoreTimeWhenFail;

    [HideInInspector] public Action ShowCallback;
    [HideInInspector] public Action CloseCallback;
    [HideInInspector] public Action ShowFailCallback;
    [HideInInspector] public Action<AdValue> PaidCallback;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    public bool IsAdAvailable
    {
        get
        {
            return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
        }
    }


    public void LoadAd()
    {
        if (IsAdAvailable)
        {
            ShowFailCallback?.Invoke();
            return;
        }
        
        AdRequest request = new AdRequest.Builder().Build();

        string adUnit = "unexpected_platform";
        #if UNITY_ANDROID
            adUnit = PLUGIN_ID.ADMOB_ANDROID_OPEN;
        #elif UNITY_IOS
            adUnit = PLUGIN_ID.ADMOB_IOS_OPEN;
        #endif
        // Load an app open ad for portrait orientation
        AppOpenAd.LoadAd(adUnit, ScreenOrientation.Portrait, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    // Handle the error.
                    #if UNITY_IOS || UNITY_IPHONE
                    Debug.LogFormat("Failed to load OPEN AD the ad.");
                    #else
                    Debug.LogFormat("Failed to load OPEN AD the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    #endif
                    
                    if (!_loadedOneMoreTimeWhenFail)
                    {
                        LoadAd();
                        _loadedOneMoreTimeWhenFail = true;
                    }
                });
                return;
            }
            
            MobileAdsEventExecutor.ExecuteInUpdate(() =>
            {
                // App open ad is loaded.
                ad = appOpenAd;
                loadTime = DateTime.UtcNow;
                _loadedOneMoreTimeWhenFail = false;
            });
        }));
    }
    
    public void ShowAdIfAvailable()
    {
        if (!IsAdAvailable)
        {
            LoadAd();
            return;
        }

        if (isShowingAd || (DateTime.UtcNow - _lastTimeShowAds).TotalSeconds < ConfigGame.WaitTimeBetweenShowOpenAd)
        {
            return;
        }

        ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
        ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
        ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
        ad.OnPaidEvent += HandlePaidEvent;

        ad.Show();
        
        _lastTimeShowAds = DateTime.UtcNow;
        
        ShowCallback?.Invoke();
    }

    private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Closed app open ad");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            ad = null;
            isShowingAd = false;
            LoadAd();

            CloseCallback?.Invoke();
        });
    }

    private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            // Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            ad = null;
            LoadAd();
            
            CloseCallback?.Invoke();
        });
    }

    private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.Log("Displayed app open ad");
            isShowingAd = true;
        });
    }

    private void HandleAdDidRecordImpression(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() => { Debug.Log("Recorded ad impression"); });
    }

    private void HandlePaidEvent(object sender, AdValueEventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            Debug.LogFormat("Received paid event. (currency: {0}, value: {1}", args.AdValue.CurrencyCode, args.AdValue.Value);
            PaidCallback?.Invoke(args.AdValue);
        });
    }
}
