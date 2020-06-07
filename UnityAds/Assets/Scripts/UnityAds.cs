using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UnityAds : MonoBehaviour, IUnityAdsListener
{
    public Text GoldText;
    public bool TestMode = true;

    public string AndroidID = "3619755";
    public string IOSID = "3619754";

    private string RewardedID = "rewardedVideo";
    private string BannerID = "banner";
    private string InterstitialID = "video";
    private Timer InternetTimer;
    private int InternetCheckPeriod { get; } = 5000;  /// (ms)      time period  to check internet connection.
    private int Gold = 0;


    void Start()
    {
        Gold = PlayerPrefs.GetInt("Gold");
        /// Check Internet Connection
        StartCoroutine(checkInternetConnection((isConnected) => {
            if (isConnected)            /// if can connect
            {
                Debug.Log("Got internet. Initializing Ads");
                InitAds();              /// initialize ads
            }
            else                        /// if no connection
            {
                InternetTimer = new Timer(InternetTimer_Tick, null, 0, InternetCheckPeriod);    /// Start a timer to check internet every InternetCheckPeriod milliseconds
                Debug.Log("No internet. Starting Timer");
            }
        }));
    }
    /// <summary>
    ///  Shows Banner Ad when it is ready
    /// </summary>
    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady(RewardedID))
        {
            Advertisement.Show(RewardedID);
        }
    }
    /// <summary>
    /// Shos Interstitial Ad if it is ready
    /// </summary>
    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady(InterstitialID))
        {
            Advertisement.Show(InterstitialID);
        }
    }
    public void ShowBannerAd()
    {
        StartCoroutine(ShowBannerWhenReady());
    }
    /// <summary>
    /// Occurs once in every specified period and checks internet connection.
    /// </summary>
    /// <param name="state"></param>
    private void InternetTimer_Tick(object state)
    {

        InternetTimer.Change(Timeout.Infinite, Timeout.Infinite);           /// Disable Timer to avoid it to throw another event while this event is being processed.
        StartCoroutine(checkInternetConnection((isConnected) =>
        {           /// Check internet connection
            if (isConnected)                                                /// if can connect
            {
                Debug.Log("Got internet Connection");
                InitAds();                                                  /// initialize ads
                StopTimer();                                                /// Stop and dispose this timer because it wont be needed anymore
            }
            else
            {
                InternetTimer.Change(InternetCheckPeriod, InternetCheckPeriod);               /// Set timer to throw following event
                Debug.Log("No internet Connection");
            }
        }));
    }
    private void StopTimer()
    {
        if (InternetTimer != null)                                          /// if the timer object exists
        {
            InternetTimer.Change(Timeout.Infinite, Timeout.Infinite);       /// Set timer intervval to infinity
            InternetTimer.Dispose();                                        /// Dispose the object
            InternetTimer = null;                                           ///
        }
    }
    private void InitAds()
    {
        Advertisement.AddListener(this);                                    /// advertisement listener to check events 
#if UNITY_ANDROID
        Advertisement.Initialize(AndroidID, TestMode);
#else
        Advertisement.Initialize(IOSID, TestMode);
#endif
    }
    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(BannerID))                            /// while banner ad is not ready
        {
            yield return new WaitForSeconds(0.5f);                          /// wait for 0.5 seconds
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);     /// Set banner position
        Advertisement.Banner.Show(BannerID);                                /// Show banner ad
    }
    /// <summary>
    /// Checks internet connection and returns a boolen
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator checkInternetConnection(Action<bool> action)
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");      /// ping google.com to see if there is internet connection.
        yield return request.SendWebRequest();
        if (request.error == null)                                                /// if error is null, this means connection succeed.
        {
            action(true);                                                        /// return true
        }
        else
        {
            action(false);                                                       /// return false
        }
    }
    public void OnUnityAdsReady(string placementId)
    {
    }

    public void OnUnityAdsDidError(string message)
    {
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == RewardedID)
        {
            switch (showResult)
            {
                case ShowResult.Finished:
                    Gold += 100;                                            /// give 100 gold to user
                    PlayerPrefs.SetInt("GOLD", Gold);                       /// save current gold value to memory
                    GoldText.text = Gold.ToString();                        /// show gold amount on screen;
                    Debug.Log("Ads Finished! Reward the Player!");          
                    return;
            }
        }
    }
}