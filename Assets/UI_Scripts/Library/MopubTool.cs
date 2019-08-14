////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;
////using System;

////public class MopubTool
////{
////    public static MopubTool instance = new MopubTool();


////    public enum AdMediationKind { Normal = 0, HighUnitPrice = 1, LowUnitPrice = 2 }


////    private Dictionary<AdMediationKind, string> interstitialUnits = new Dictionary<AdMediationKind, string>();      //전면광고유닛별 유닛아이디
////    private Dictionary<AdMediationKind, string> rewardedVideoUnits = new Dictionary<AdMediationKind, string>(); //보상광고유닛별 유닛아이디

////    private Dictionary<Ads_TYPE, bool> AdLoadStates = new Dictionary<Ads_TYPE, bool>(); // 광고별 로드 상태
////    private Dictionary<Ads_TYPE, bool> AdRequestStates = new Dictionary<Ads_TYPE, bool>(); // 광고 요청 상태
////    private AdMediationKind useAdMediationKind;

////    public del_NextProcess nextProcess;



////#if UNITY_IOS

////    private readonly string[] _interstitialAdUnitKeys =
////        { "86fe5c079f55418e89862fa5ff2a77ed" };

////    private readonly string[] _rewardedVideoAdUnitKeys =
////        { "955a13de4bce48dcbc3813e59214b8db" };

////#elif UNITY_ANDROID || UNITY_EDITOR
////    private readonly string[] _interstitialAdUnitKeys =
////    { "0e6955b89ba44870aab16b5931cdb519" };

////    private readonly string[] _rewardedVideoAdUnitKeys =
////    { "b224300f3e6d43c297882f7344b495e0" };
////#endif



////    public void Start_mopubMediation()
////    {

////        #region  callback Event

////        MoPubManager.OnSdkInitalizedEvent += OnSdkInitializedEvent;

////        MoPubManager.OnConsentStatusChangedEvent += OnConsentStatusChangedEvent;
////        MoPubManager.OnConsentDialogLoadedEvent += OnConsentDialogLoadedEvent;
////        MoPubManager.OnConsentDialogFailedEvent += OnConsentDialogFailedEvent;
////        MoPubManager.OnConsentDialogShownEvent += OnConsentDialogShownEvent;


////        MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
////        MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
////        MoPubManager.OnInterstitialShownEvent += OnInterstitialShownEvent;
////        MoPubManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
////        MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
////        MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;


////        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
////        MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
////        MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
////        MoPubManager.OnRewardedVideoShownEvent += OnRewardedVideoShownEvent;
////        MoPubManager.OnRewardedVideoClickedEvent += OnRewardedVideoClickedEvent;
////        MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
////        MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
////        MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
////        MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedVideoLeavingApplicationEvent;
////        #endregion

////        사용할 광고유닛 설정
////        useAdMediationKind = AdMediationKind.Normal;

////        광고유닛 별 유닛아이디 로드하기

////        Load_UnitIdKeyforKind();

////        string anyUnitId = _rewardedVideoAdUnitKeys[0];
////        MoPub.InitializeSdk(_rewardedVideoAdUnitKeys[0]);
////        MoPub.InitializeSdk(_interstitialAdUnitKeys[0]);
////    }


////    private void OnSdkInitializedEvent(string adUnitId)
////    {
////        Debug.Log("OnSdkInitializedEvent: " + adUnitId);


////        Init_moputMediation();

////        if (MoPub.ShouldShowConsentDialog)
////            MoPub.LoadConsentDialog();

////        Request_Ads(Ads_TYPE.Interstitial_Image);


////        Request_Ads(Ads_TYPE.Reward);

////    }

////    void Init_moputMediation()
////    {



////        MoPub.LoadInterstitialPluginsForAdUnits(_interstitialAdUnitKeys);
////        MoPub.LoadRewardedVideoPluginsForAdUnits(_rewardedVideoAdUnitKeys);



////        AdLoadStates 값들 초기화
////       AdLoadStates[Ads_TYPE.Reward] = false;
////        AdLoadStates[Ads_TYPE.Interstitial_Image] = false;

////        AdRequestStates 값들 초기화

////        AdRequestStates[Ads_TYPE.Reward] = false;
////        AdRequestStates[Ads_TYPE.Interstitial_Image] = false;
////    }




////    광고유닛 별 유닛아이디 로드하기

////    void Load_UnitIdKeyforKind()
////    {
////        for (int i = 0; i < _interstitialAdUnitKeys.Length; i++)
////        {
////            AdMediationKind kind = (AdMediationKind)i;
////            if (Enum.IsDefined(typeof(AdMediationKind), kind))
////            {
////                interstitialUnits[kind] = _interstitialAdUnitKeys[i];
////            }
////        }

////        for (int i = 0; i < _rewardedVideoAdUnitKeys.Length; i++)
////        {
////            AdMediationKind kind = (AdMediationKind)i;
////            if (Enum.IsDefined(typeof(AdMediationKind), kind))
////            {
////                rewardedVideoUnits[kind] = _rewardedVideoAdUnitKeys[i];
////            }
////        }
////    }






////    private void OnConsentStatusChangedEvent(MoPub.Consent.Status oldStatus, MoPub.Consent.Status newStatus,
////                                           bool canCollectPersonalInfo)
////    {
////        Debug.Log("OnConsetStatusChangedEvent: old=" + oldStatus + " new=" + newStatus + " personalInfoOk=" + canCollectPersonalInfo);

////        if (canCollectPersonalInfo)
////        {

////        }

////    }


////    private void OnConsentDialogLoadedEvent()
////    {
////        Debug.Log("OnConsentDialogLoadedEvent: dialog loaded");
////        MoPub.ShowConsentDialog();
////    }


////    private void OnConsentDialogFailedEvent(string err)
////    {
////        Debug.Log("OnConsentDialogFailedEvent: " + err);
////    }


////    private void OnConsentDialogShownEvent()
////    {
////        Debug.Log("OnConsentDialogShownEvent: dialog shown");
////    }









////    public void Request_Ads(Ads_TYPE adsType)
////    {
////        if (adsType == Ads_TYPE.Interstitial_Image)
////        {
////            if (AdLoadStates[Ads_TYPE.Interstitial_Image] == false && AdRequestStates[Ads_TYPE.Interstitial_Image] == false) //로드 안되어있고 요청상태가 아닌경우
////            {
////                AdRequestStates[Ads_TYPE.Interstitial_Image] = true; // 요청상태임
////                MoPub.RequestInterstitialAd(interstitialUnits[useAdMediationKind]);
////                UserEditor.Getsingleton.EditLog("RequestInterstitialAd : " + rewardedVideoUnits[useAdMediationKind]);
////            }
////        }
////        else if (adsType == Ads_TYPE.Reward)
////        {
////            if (AdLoadStates[Ads_TYPE.Reward] == false && AdRequestStates[Ads_TYPE.Reward] == false) //로드 안되어있고 요청상태가 아닌경우
////            {
////                AdRequestStates[Ads_TYPE.Reward] = true; // 요청상태임
////                MoPub.RequestRewardedVideo(rewardedVideoUnits[useAdMediationKind]);
////                UserEditor.Getsingleton.EditLog("RequestRewardedVideo : " + rewardedVideoUnits[useAdMediationKind]);
////            }
////        }

////    }









////    public bool IsReadyAds(Ads_TYPE adType)
////    {
////        bool isReady = false;
////        if (adType == Ads_TYPE.Reward)
////        {
////            if (AdLoadStates[Ads_TYPE.Reward])
////                isReady = true;
////            else
////            {
////                Request_Ads(Ads_TYPE.Reward);
////                UserEditor.Getsingleton.EditLog("RequestRewardedVideo : " + rewardedVideoUnits[useAdMediationKind]);
////                MoPub.RequestRewardedVideo(rewardedVideoUnits[useAdMediationKind]);
////                isReady = false;
////            }
////        }
////        else if (adType == Ads_TYPE.Interstitial_Image)
////        {
////            if (AdLoadStates[Ads_TYPE.Interstitial_Image])
////                isReady = true;
////            else
////            {
////                Request_Ads(Ads_TYPE.Interstitial_Image);
////                UserEditor.Getsingleton.EditLog("RequestInterstitialAd : " + interstitialUnits[useAdMediationKind]);
////                MoPub.RequestInterstitialAd(interstitialUnits[useAdMediationKind]);
////                isReady = false;
////            }
////        }

////        UserEditor.Getsingleton.EditLog("Mopub isReady : " + isReady);

////        return isReady;
////    }






////    public void Show_Ads(Ads_TYPE adType)
////    {
////        if (adType == Ads_TYPE.Reward)
////        {
////            MoPub.ShowRewardedVideo(rewardedVideoUnits[useAdMediationKind]);

////            UserEditor.Getsingleton.EditLog("Mopub reward Ad show");
////        }
////        else if (adType == Ads_TYPE.Interstitial_Image)
////        {
////            MoPub.ShowInterstitialAd(interstitialUnits[useAdMediationKind]);

////            UserEditor.Getsingleton.EditLog("Mopub Intertitial Ad show");
////        }
////    }



////    public IEnumerator routine_showAds(Ads_TYPE adType)
////    {
////        if (adType == Ads_TYPE.Reward)
////        {
////            float time = 0;

////            while (true)
////            {
////                if (time > 7) // 7초
////                {
////                    로비캐릭 회전 잠금 해제

////                    User.isSelectedCharacter = false;

////                    Loadmanager.instance.LoadingUI(false);

////                    if (adType == Ads_TYPE.Reward)
////                    {
////                        UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
////                        popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
////                        popup.SetPopupMessage(TextDataManager.Dic_TranslateText[515]);//광고 불러오기 실패 하엿습니다.

////                    }
////                    break;
////                }

////                광고로드 되엇으면 광고보여주기

////                if (AdLoadStates[adType] == true)
////                {
////                    Show_Ads(adType);
////                    break;
////                }

////                time += Time.deltaTime;


////                yield return null;
////            }


////        }
////        else if (adType == Ads_TYPE.Interstitial_Image)
////            Show_Ads(adType);
////    }





////    #region Interstitial Events

////    Interstitial Events


////    private void OnInterstitialLoadedEvent(string adUnitId)
////    {
////        Debug.Log("OnInterstitialLoadedEvent: " + adUnitId);

////        AdLoadStates[Ads_TYPE.Interstitial_Image] = true;

////        AdRequestStates[Ads_TYPE.Interstitial_Image] = false; // 요청완료임
////    }


////    private void OnInterstitialFailedEvent(string adUnitId, string error)
////    {
////        Debug.Log("OnInterstitialFailedEvent: " + adUnitId + " Error : " + error);

////        AdLoadStates[Ads_TYPE.Interstitial_Image] = false;

////        AdRequestStates[Ads_TYPE.Interstitial_Image] = false; // 요청완료임

////    }


////    private void OnInterstitialShownEvent(string adUnitId)
////    {
////        Debug.Log("OnInterstitialShownEvent: " + adUnitId);
////    }


////    private void OnInterstitialClickedEvent(string adUnitId)
////    {
////        Debug.Log("OnInterstitialClickedEvent: " + adUnitId);
////    }


////    private void OnInterstitialDismissedEvent(string adUnitId)
////    {
////        Debug.Log("OnInterstitialDismissedEvent: " + adUnitId);

////        AdLoadStates[Ads_TYPE.Interstitial_Image] = false;

////        닫을떄광고 요청

////        MoPub.RequestInterstitialAd(interstitialUnits[useAdMediationKind]);

////        광고닫은후 프로세스

////        if (nextProcess != null)
////            nextProcess();
////    }


////    private void OnInterstitialExpiredEvent(string adUnitId)
////    {
////        Debug.Log("OnInterstitialExpiredEvent: " + adUnitId);
////    }



////    #endregion



////    #region  Rewarded Video Events
////    Rewarded Video Events


////    private void OnRewardedVideoLoadedEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoLoadedEvent: " + adUnitId);

////        var availableRewards = MoPub.GetAvailableRewards(adUnitId);


////        AdLoadStates[Ads_TYPE.Reward] = true;

////        AdRequestStates[Ads_TYPE.Reward] = false; // 요청완료임


////    }


////    private void OnRewardedVideoFailedEvent(string adUnitId, string error)
////    {
////        Debug.Log("OnRewardedVideoFailedEvent: " + adUnitId + " Error :" + error);

////        AdLoadStates[Ads_TYPE.Reward] = false;

////        AdRequestStates[Ads_TYPE.Reward] = false; // 요청완료임
////    }


////    private void OnRewardedVideoExpiredEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoExpiredEvent: " + adUnitId);


////        AdLoadStates[Ads_TYPE.Reward] = false;
////    }


////    private void OnRewardedVideoShownEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoShownEvent: " + adUnitId);

////        AdLoadStates[Ads_TYPE.Reward] = false;

////        로비캐릭 회전 잠금 해제

////        User.isSelectedCharacter = false;

////    }


////    private void OnRewardedVideoClickedEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoClickedEvent: " + adUnitId);
////    }


////    private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
////    {
////        Debug.Log("OnRewardedVideoFailedToPlayEvent: " + adUnitId + " Error :" + error);


////    }


////    private void OnRewardedVideoReceivedRewardEvent(string adUnitId, string label, float amount)
////    {
////        Debug.Log("OnRewardedVideoReceivedRewardEvent for ad unit id " + adUnitId + " currency:" + label + " amount:" + amount);

////        AdLoadStates[Ads_TYPE.Reward] = false;

////        if (amount != 0)
////        {
////            보상받은후 다음 프로세스

////            if (nextProcess != null)
////                nextProcess();
////        }
////    }


////    private void OnRewardedVideoClosedEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoClosedEvent: " + adUnitId);

////        AdLoadStates[Ads_TYPE.Reward] = false;

////        로비캐릭 회전 잠금 해제

////        User.isSelectedCharacter = false;

////        광고다시 요청

////        MoPub.RequestRewardedVideo(rewardedVideoUnits[useAdMediationKind]);
////    }


////    private void OnRewardedVideoLeavingApplicationEvent(string adUnitId)
////    {
////        Debug.Log("OnRewardedVideoLeavingApplicationEvent: " + adUnitId);
////    }




////    #endregion

////}
