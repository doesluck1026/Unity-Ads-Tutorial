using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour,IUnityAdsListener
{
    public bool TestMode = true;

    public string AndroidID = "3619755";
    public string IOSID = "3619754";

    private string RewardedID = "rewardedVideo";
    private string BannerID = "banner";
    private string InterstitialID = "video";

    void Start()
    {
        Advertisement.AddListener(this);
#if UNITY_ANDROID
        Advertisement.Initialize(AndroidID, TestMode);
#else
        Advertisement.Initialize(IOSID, TestMode);
#endif

    }
    public void ShowRewardedAd()
    {
       if(Advertisement.IsReady(RewardedID))
        {
            Advertisement.Show(RewardedID);
        }
    }
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
    IEnumerator ShowBannerWhenReady()
    {
        while(!Advertisement.IsReady(BannerID))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(BannerID);
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
        if(placementId== RewardedID)
        {
            switch(showResult)
            {
                case ShowResult.Finished:
                    Debug.Log("Ads Finished! Reward the Player!");
                    return;
            }
        }
    }
}
