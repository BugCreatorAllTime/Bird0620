using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : BaseController
{
    public TextMeshProUGUI loadingText;
    public Slider sliderBar;
    
    // Start is called before the first frame update
    void Start()
    {
        sliderBar.gameObject.SetActive(true);
        loadingText.text = "Loading...";
        
        StartCoroutine(LoadYourAsyncScene());
        
        GameInfo.TimeLoadingStart = DateTime.Now;
    }
    
    // private float _tick = 0f;
    // private bool _isShowOpenAdsFirstTime;
    // private bool _isTimeOut;
    // void Update()
    // {
    //     if (!_isShowOpenAdsFirstTime)
    //     {
    //         if (AppOpenAdManager.Instance.IsAdAvailable)
    //         {
    //             AppOpenAdManager.Instance.ShowAdIfAvailable();
    //             _isShowOpenAdsFirstTime = true;
    //         }
    //     }
    //
    //     if (_tick > 7f) _isTimeOut = true;
    //     _tick += Time.deltaTime;
    // }

    
    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;
        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            sliderBar.DOValue(progress, 0.1f);
            loadingText.text = $"{progress * 100f:n0}%";
            yield return new WaitForSeconds(0.1f);
            
            //if (asyncLoad.progress >= 0.9f && MainController.IsRemoteConfigFetchDone)
            // if (asyncLoad.progress >= 0.9f && MainController.RemoteConfigController.IsLoaded() && (_isShowOpenAdsFirstTime || _isTimeOut))
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
