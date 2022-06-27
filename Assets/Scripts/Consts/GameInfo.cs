using System;

public class GameInfo
{
    public static string CurrentLevelId;
    public static DateTime TimeLoadingStart;

    public static bool IsCampainDayChange;

    // Network check
    public static bool IsLastestCheckNetworkFail = false;
    public static bool IsLastestCheckInternetFail = false;
    public static bool IsPopupNoInternetShow = false;
    public static bool IsPauseByShowOtherAds;

    // Puzzle Level Variable
    public static bool IsLoseOption2;
    

    // Some boolean check for Banner Ad callback event
    public static bool IsBannerShowCallback;

    // Some boolean check for Inter Ad callback event
    public static bool IsInterCloseCallback;
    public static bool IsInterShowCallback;
    public static bool IsInterShowFailCallback;
    public static bool IsInterLoadedCallback;
    public static bool IsInterLoadFailCallback;

    // Some boolean check for Inter Ad callback event
    public static bool IsRewardCloseCallback;
    public static bool IsRewardShowCallback;
    public static bool IsRewardShowFailCallback;
    public static bool IsRewardLoadedCallback;
    public static bool IsRewardLoadFailCallback;
    public static bool IsRewardEarnedCallback;
}