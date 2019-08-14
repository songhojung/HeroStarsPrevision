//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using AdColony;

//public class AdcolonyTool
//{
//    public static AdcolonyTool instance = new AdcolonyTool();

//    private Dictionary<Ads_TYPE, string> Dic_AdZone = new Dictionary<Ads_TYPE, string>();


//    private InterstitialAd Ad_intert;
//    private InterstitialAd Ad_reward;

//    public del_NextProcess nextProcess;

//    public bool isLeaveAd = false;              //광고재생중 홈버튼등으로 앱을나갓냐

//    public void Start_Adcolony()
//    {
//        string AppID = string.Empty;
//        string[] zoneIds = new string[] { };

//#if UNITY_EDITOR
//        UserEditor.Getsingleton.EditLog("start adcolony EDITOR");
//#elif UNITY_ANDROID

//		UserEditor.Getsingleton.EditLog("start AOS adcolony configure");
//		AppID = "app1c9a08659ecf4620bb";
//		zoneIds = new string[] { "vzce45c0eeaa724f8d98", "vzd43e8b58bf134c8b8c" };

//#elif UNITY_IOS
//		UserEditor.Getsingleton.EditLog("start IOS adcolony configure");
//		AppID = "appe61691873c0d4e27a0";
//		zoneIds = new string[] { "vz80392909619045af94", "vzffb5cd44d95944d9b9" };
//#endif


//#if UNITY_EDITOR
//        UserEditor.Getsingleton.EditLog("start Init configure EDITOR");
//#else
//		UserEditor.Getsingleton.EditLog("start Init ADcolony configure");

//		 AppOptions are optional
//		AdColony.AppOptions appOptions = new AdColony.AppOptions();
//		appOptions.AdOrientation = AdColony.AdOrientationType.AdColonyOrientationAll;

		
//		Dic_AdZone[Ads_TYPE.Interstitial_Video] = zoneIds[0];
//		Dic_AdZone[Ads_TYPE.Reward] = zoneIds[1];

//		Ads.Configure(AppID, appOptions, zoneIds);

//		Ads.OnConfigurationCompleted += CallbackOnConfigureCompleted;
//		Ads.OnRequestInterstitial += CallbackRequestAd;
//		Ads.OnExpiring += CallbackExpireAd;
//		Ads.OnRewardGranted += CallbackRewardAd;
//		Ads.OnClosed += CallbackClosed;
//		Ads.OnLeftApplication += CallbackLeaveApplication;
	
//#endif



//    }

//    void AllRequest_Ad()
//    {
//        Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Interstitial_Video], null);
//        Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Reward], null);
//        UserEditor.Getsingleton.EditLog("all request   adColony ADs !!!!!");
//    }


//    public void Show_Ads(Ads_TYPE adType)
//    {
//        if (adType == Ads_TYPE.Reward)
//        {
//            if (Ad_reward != null)
//            {
//                UserEditor.Getsingleton.EditLog("show  adColony");
//                Ads.ShowAd(Ad_reward);
//            }
//            else
//                UserEditor.Getsingleton.EditLog("not show adColony");
//        }
//        else if (adType == Ads_TYPE.Interstitial_Video)
//        {
//            if (Ad_intert != null)
//            {
//                UserEditor.Getsingleton.EditLog("show adColony ad");
//                Ads.ShowAd(Ad_intert);
//            }
//            else
//                UserEditor.Getsingleton.EditLog("not show adColony ad");
//        }
//    }




//    public bool IsReadyAds(Ads_TYPE adType)
//    {
//        bool isReady = false;
//        if (adType == Ads_TYPE.Reward)
//        {
//            if (Ad_reward != null)
//                isReady = true;
//            else
//            {
//                Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Reward], null);
//                isReady = false;
//            }
//        }
//        else if (adType == Ads_TYPE.Interstitial_Video)
//        {
//            if (Ad_intert != null)
//                isReady = true;
//            else
//            {
//                Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Interstitial_Video], null);
//                isReady = false;
//            }
//        }
//        UserEditor.Getsingleton.EditLog("AdColony isReady : " + isReady);

//        return isReady;
//    }





//    public void Request_Ads(Ads_TYPE adType)
//    {
//        if (adType == Ads_TYPE.Reward)
//        {
//            Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Reward], null);
//        }
//        else if (adType == Ads_TYPE.Interstitial_Video)
//        {
//            Ads.RequestInterstitialAd(Dic_AdZone[Ads_TYPE.Interstitial_Video], null);
//        }
//    }








//    void CallbackOnConfigureCompleted(List<AdColony.Zone> zones)
//    {
//        UserEditor.Getsingleton.EditLog("AdColony.Ads.OnConfigurationCompleted called");

//        if (zones == null || zones.Count <= 0)
//        {
//            UserEditor.Getsingleton.EditLog(" AdColony Configure Failed");
//        }
//        else
//        {
//            UserEditor.Getsingleton.EditLog("AdColony Configure Succeeded.");
//            AllRequest_Ad();
//        }



//    }


//    void CallbackRequestAd(InterstitialAd ad)
//    {
//        isLeaveAd = false;

//        if (Get_AdType(ad.ZoneId) == Ads_TYPE.Reward)
//        {
//            Ad_reward = ad;
//            UserEditor.Getsingleton.EditLog("adColony RequestAd : reward , " + ad.ZoneId);

//        }
//        else if (Get_AdType(ad.ZoneId) == Ads_TYPE.Interstitial_Video)
//        {
//            Ad_intert = ad;
//            UserEditor.Getsingleton.EditLog("adColony RequestAd : Interst ," + ad.ZoneId);

//        }
//    }

//    void CallbackExpireAd(InterstitialAd ad)
//    {
//        UserEditor.Getsingleton.EditLog("adColony expire :" + ad.ZoneId);
//        광고끝낫으니 다시 요청

//        Ads.RequestInterstitialAd(ad.ZoneId, null);
//    }

//    void CallbackClosed(InterstitialAd ad)
//    {
//        UserEditor.Getsingleton.EditLog("adColony Closed :" + ad.ZoneId);
//        광고끝낫으니 다시 요청

//        if (Get_AdType(ad.ZoneId) == Ads_TYPE.Interstitial_Video)
//        {
//            if (nextProcess != null)
//                nextProcess();
//        }

//        Ads.RequestInterstitialAd(ad.ZoneId, null);
//    }

//    void CallbackRewardAd(string zoneId, bool success, string name, int amount)
//    {
//        UserEditor.Getsingleton.EditLog("adColony RewardAd ==> " + "zoneId : " + zoneId + " success : " + success + " name : " + name + " amount : " + amount);

//        if (Get_AdType(zoneId) == Ads_TYPE.Reward)
//        {
//            if (success)
//            {
//                if (nextProcess != null)
//                    nextProcess();
//            }
//        }

//    }


//    void CallbackLeaveApplication(InterstitialAd ad)
//    {
//        UserEditor.Getsingleton.EditLog("adColony Leaving Application : " + ad.ZoneId);
//    }





//    Ads_TYPE Get_AdType(string zoneId)
//    {
//        Ads_TYPE type = Ads_TYPE.NONE;
//        if (string.Equals(Dic_AdZone[Ads_TYPE.Reward], zoneId))
//        {

//            type = Ads_TYPE.Reward;
//        }
//        else if (string.Equals(Dic_AdZone[Ads_TYPE.Interstitial_Video], zoneId))
//        {

//            type = Ads_TYPE.Interstitial_Video;
//        }
//        return type;
//    }
//}
