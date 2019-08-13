//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AdsManager 
//{
//	public static AdsManager instance = new AdsManager();

//	public del_NextProcess nextAdsProcess;
//	private ADSTOOL_KIND NowRewardAdsKind = ADSTOOL_KIND.MOPUBMD;
//	private ADSTOOL_KIND NowInterstitialAdsKind = ADSTOOL_KIND.MOPUBMD;

//	public bool IsShowAdsGameEnd = false;				//게임 종료하여 광고봐야하냐?




//	public AdsManager()
//	{
//		set_Ads();
//	}

//	public void set_Ads()
//	{
		
//		//공지팝업을 위해 탭조이 inits
//		TabjoyTool.instance.Start_tapJoy();
//		//AdmobTool.instance.Start_admob();
//		//AdcolonyTool.instance.Start_Adcolony();
//		MopubTool.instance.Start_mopubMediation();


//	}

//	private bool IsLeave(ADSTOOL_KIND adsType)
//	{
//		bool isleave = false;

//		switch (adsType)
//		{
//			case ADSTOOL_KIND.TAPJOY:
			
//				break;
//			case ADSTOOL_KIND.ADCOLONY:
//				isleave = AdcolonyTool.instance.isLeaveAd;
//				break;
//			case ADSTOOL_KIND.ADMOB:
//				break;
//			default:
//				break;
//		}

//		return isleave;
//	}
	


//	public void Show_RewardAds()
//	{
//		Loadmanager.instance.LoadingUI(true);

//		if (NowRewardAdsKind == ADSTOOL_KIND.ADCOLONY)
//		{
//			NowRewardAdsKind = ADSTOOL_KIND.ADMOB;

//			if (AdcolonyTool.instance.IsReadyAds(Ads_TYPE.Reward))
//			{
//				AdcolonyTool.instance.Show_Ads(Ads_TYPE.Reward);
//				AdcolonyTool.instance.nextProcess = callback_complete_rewardAds;
//			}
//			else
//			{
//				Show_RewardAds();
//			}
//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.TAPJOY)
//		{
//			NowRewardAdsKind = ADSTOOL_KIND.ADCOLONY;

//			if (TabjoyTool.instance.IsReadyAds("reawardAd"))
//			{
//				TabjoyTool.instance.Show_TjAds("reawardAd");
//				TabjoyTool.instance.nextProcessEvent = callback_complete_rewardAds;
//			}
//			else
//			{
//				Show_RewardAds();
//			}
//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.ADMOB)
//		{
//			NowRewardAdsKind = ADSTOOL_KIND.ADCOLONY;

//			if (AdmobTool.instance.IsReadyAds(Ads_TYPE.Reward))
//			{
//				AdmobTool.instance.Show_Ads(Ads_TYPE.Reward);
//				AdmobTool.instance.nextProcess = callback_complete_rewardAds;
//			}
//			else
//			{
//				Show_RewardAds();
//			}
//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.MOPUBMD)
//		{
//			//로비캐릭 회전 잠금
//			User.isSelectedCharacter = true;

//			if (MopubTool.instance.IsReadyAds(Ads_TYPE.Reward))
//			{
//				MopubTool.instance.Show_Ads(Ads_TYPE.Reward);
//				MopubTool.instance.nextProcess = callback_complete_rewardAds;

//			}
//			else
//			{
//				MopubTool.instance.nextProcess = callback_complete_rewardAds;
//				UI_Manager.Getsingleton.StartCoroutine(MopubTool.instance.routine_showAds(Ads_TYPE.Reward));

//			}

//			//test
//			//MopubTool.instance.Request_Ads(Ads_TYPE.Reward);
//			//MopubTool.instance.nextProcess = callback_complete_rewardAds;
//		}

		
//	}


//	void callback_complete_rewardAds()
//	{

//		//로비캐릭 회전 잠금 해제
//		User.isSelectedCharacter = false;

//		Loadmanager.instance.LoadingUI(false);


//		UserEditor.Getsingleton.EditLog("finish rewardAds");

	

//		if (IsLeave(NowRewardAdsKind))
//			return;

//		if (nextAdsProcess != null)
//			nextAdsProcess();

//		NowRewardAdsKind = ADSTOOL_KIND.MOPUBMD;
//	}







//	public void Show_interstitalAds()
//	{
//		if (NowInterstitialAdsKind == ADSTOOL_KIND.ADMOB)
//		{
//			if (AdmobTool.instance.IsReadyAds(Ads_TYPE.Interstitial_Image))
//			{
				
//				Loadmanager.instance.LoadingUI(false);
//				AdmobTool.instance.Show_Ads(Ads_TYPE.Interstitial_Image);
//				AdmobTool.instance.nextProcess = callback_complete_interstitalAds;
//			}
//			else
//			{
//				Loadmanager.instance.LoadingUI(true);
//				NowInterstitialAdsKind = ADSTOOL_KIND.TAPJOY;
//				Show_interstitalAds();
//			}
//		}
//		else if (NowInterstitialAdsKind == ADSTOOL_KIND.TAPJOY)
//		{
//			if (TabjoyTool.instance.IsReadyAds("intertitalAd"))
//			{
				
//				Loadmanager.instance.LoadingUI(false);
//				TabjoyTool.instance.Show_TjAds("intertitalAd");
//				TabjoyTool.instance.nextProcessEvent = callback_complete_interstitalAds;
//			}
//			else
//			{
//				Loadmanager.instance.LoadingUI(true);
//				NowInterstitialAdsKind = ADSTOOL_KIND.ADMOB;
//				Show_interstitalAds();
//			}
//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.MOPUBMD)
//		{
			
//			if (MopubTool.instance.IsReadyAds(Ads_TYPE.Interstitial_Image))
//			{
//				MopubTool.instance.Show_Ads(Ads_TYPE.Interstitial_Image);
//				MopubTool.instance.nextProcess = callback_complete_interstitalAds;

//			}
//			//else
//			//{
//			//    MopubTool.instance.nextProcess = callback_complete_interstitalAds;
//			//    UI_Manager.Getsingleton.StartCoroutine(MopubTool.instance.routine_showAds(Ads_TYPE.Interstitial_Image));
//			//}
			
//		}
//	}



//	void callback_complete_interstitalAds()
//	{
//		Loadmanager.instance.LoadingUI(false);

//		Debug.Log("finish interstitalAds");

//		NowInterstitialAdsKind = ADSTOOL_KIND.MOPUBMD;

//		if (nextAdsProcess != null)
//			nextAdsProcess();
//	}






//	public void Reuest_Ads(Ads_TYPE adsType)
//	{
//		if (NowInterstitialAdsKind == ADSTOOL_KIND.ADMOB)
//		{
//			if (adsType == Ads_TYPE.Interstitial_Image)
//				AdmobTool.instance.Request_InterstitialAd(adsType);
//			else if(adsType == Ads_TYPE.Reward)
//				AdmobTool.instance.Requset_RewardAd();
//		}
//		else if (NowInterstitialAdsKind == ADSTOOL_KIND.TAPJOY)
//		{
//			TabjoyTool.instance.Request_Ads(adsType);
//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.ADCOLONY)
//		{
//			AdcolonyTool.instance.Request_Ads(adsType);

//		}
//		else if (NowRewardAdsKind == ADSTOOL_KIND.MOPUBMD)
//		{
//			MopubTool.instance.Request_Ads(adsType);
//		}
//	}

//}
