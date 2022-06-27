
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Controllers;
#if UNITY_IOS && !UNITY_EDITOR
using SA.CrossPlatform.UI;
using SA.iOS.AppTrackingTransparency;
#endif

public class MainController : BaseController
{
    // Game Scene
    [HideInInspector] public GameController Game;
    [HideInInspector] public UIGameController UIGame;
    
    [HideInInspector] public PopupController Popup;

    [HideInInspector] public LevelSceneManager LevelSceneManager;

    // Other plugin
    [HideInInspector] public AdController AdController;

    [HideInInspector] public DateTime TimeStart;

    [HideInInspector] public bool IsFirstTimeOpen;

    public override void Awake()
    {
        base.Awake();

        CONST.SCREEN_WIDTH = CONST.CANVAS_WIDTH;
        CONST.SCREEN_HEIGHT = CONST.CANVAS_WIDTH * Screen.height / Screen.width;
        print(CONST.SCREEN_WIDTH + "x" + CONST.SCREEN_HEIGHT);
        
        AdController = GetComponentInChildren<AdController>(true);
        Popup = GetComponentInChildren<PopupController>(true);
    }

       // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        // Calling app tracking transparency
        #if UNITY_IOS && !UNITY_EDITOR
        ISN_ATTrackingManager.RequestTrackingAuthorization(status =>
        {
            Debug.Log($"Tracking status: {ISN_ATTrackingManager.TrackingAuthorizationStatus}");
        });
        #endif
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        Screen.sleepTimeout = (int) 0f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Input.multiTouchEnabled = false;

        TimeStart = DateTime.Now;
        
        DontDestroyOnLoad(this);

        Debug.Log("Bundle ID: " + Application.identifier);

        StartCoroutine(InitPlugin());
    }

    IEnumerator InitPlugin()
    {
        yield return new WaitForEndOfFrame();

        if (ConfigGame.EnableAds) AdController.InitAds();
        
        HandleFirstOpen();
    }

    public bool CheckInternetReachable()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            GameInfo.IsLastestCheckNetworkFail = true;
            return false;
        }

        GameInfo.IsLastestCheckNetworkFail = false;
        return true;
    }

    public void CheckInternetConnect()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            Debug.Log("No Connection!");
            MainController.Popup.PopupLoading.Hide();
            GameInfo.IsLastestCheckInternetFail = true;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {
            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
            Debug.Log("Not Found Connection!");
            MainController.Popup.PopupLoading.Hide();
            GameInfo.IsLastestCheckInternetFail = true;
        }
        else
        {
            //success
            Debug.Log("Connect Internet success!");
            MainController.Popup.PopupLoading.Hide();
            GameInfo.IsLastestCheckInternetFail = false;

            if (GameInfo.IsLastestCheckInternetFail)
            {
                MainController.AdController.ReloadRewardAndInterAds();
            }
        }
    }

    public string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }

    //void InitializeFirebaseComponents()
    //{
    //    Task.WhenAll(

    //        InitializeRemoteConfig()

    //      ).ContinueWith(task => {

    //          IsRemoteConfigFetchDone = true;

    //      });
    //}

    //Task InitializeRemoteConfig()
    //{
    //    return RemoteConfigController.InitializeFirebase();
    //}

    public void HandleFirstOpen()
    {
        Debug.Log("==> Handle First Open");

        IsFirstTimeOpen = DataManager.Instance.UserData.IsFirstOpen;

        if (DataManager.Instance.UserData.IsFirstOpen)
        {
            DataManager.Instance.UserData.Gem = ConfigGame.GemGiftFirstOpen;
            DataManager.Instance.UserData.IsFirstOpen = false;
        }
    }

    private void OnApplicationPause(bool isPause) {
        if (isPause) OnAppInactive();
        else OnResume();
    }

    private void OnApplicationQuit() {
        OnAppInactive();
    }

    public void OnResume()
    {
        
    }

    public void OnAppInactive() {
        // add some function here before close app
        DataManager.Instance.SaveGame();
    }

    public void AddGem(int val)
    {
        DataManager.Instance.UserData.Gem += val;
        // if(UIGame != null) UIGame.UpdateGemText();

        Debug.Log("==> Gem: normal " + val + " " + DataManager.Instance.UserData.Gem);
    }
    
    public void AddGemSilent(int val)
    {
        DataManager.Instance.UserData.Gem += val;
        Debug.Log("==> Gem: slient " + val + " " + DataManager.Instance.UserData.Gem);
    }

    public bool SpendGem(int val)
    {
        if(val > DataManager.Instance.UserData.Gem)
        {
            Debug.Log("You not enough Gem!");
            return false;
        } else
        {
            DataManager.Instance.UserData.Gem -= val;
            // if (UIGame != null) UIGame.UpdateGemText();

            return true;
        }
    }

    public void DiffGem(int val)
    {
        if (val > DataManager.Instance.UserData.Gem)
        {
            DataManager.Instance.UserData.Gem = 0;
        } else {
            DataManager.Instance.UserData.Gem -= val;
        }

        // if (UIGame != null) UIGame.UpdateGemText();
    }
    
    public void OpenAppInStore()
    {
        string url = "market://details?id=" + Application.identifier;
#if UNITY_IOS || UNITY_IPHONE
        url ="itms-apps://itunes.apple.com/app/" + CONST.IOS_APPID;
#endif
        Application.OpenURL(url);
    }
}