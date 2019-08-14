//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GoogleMobileAds.Api;
//using System;

//public class AdmobTool 
//{

//	public static AdmobTool instance = new AdmobTool();

//	//Dev ID
//	//private string android_AppId = "ca-app-pub-6591726594385567~3889778129";
//	//private string ios_AppId = "ca-app-pub-6591726594385567~4660115492";

//	//private string android_interstitial_id_Image = "ca-app-pub-6591726594385567/1532992306";
//	//private string android_interstitial_id_Video = "ca-app-pub-6591726594385567/8541253785";
//	//private string android_rewardVideoAd_id = "ca-app-pub-6591726594385567/3257871669";

//	//private string ios_interstitial_id_Image = "ca-app-pub-6591726594385567/3672070484";
//	//private string ios_interstitial_id_Video = "ca-app-pub-6591726594385567/2561210938";
//	//private string ios_rewardVideoAd_id = "ca-app-pub-6591726594385567/1674859509";



//	//Dis ID
//	private string android_AppId = "ca-app-pub-6591726594385567~5113760922";
//	private string ios_AppId = "ca-app-pub-6591726594385567~9891428977";


//	private string android_interstitial_id_Image = "ca-app-pub-6591726594385567/5903673380";
//	private string android_interstitial_id_Video = "ca-app-pub-6591726594385567/2315140874";
//	private string android_rewardVideoAd_id = "ca-app-pub-6591726594385567/7020672643";

//	private string ios_interstitial_id_Image = "ca-app-pub-6591726594385567/4639102291";
//	private string ios_interstitial_id_Video = "ca-app-pub-6591726594385567/4205424158";
//	private string ios_rewardVideoAd_id = "ca-app-pub-6591726594385567/2070215408";

//	public del_NextProcess nextProcess;

//	private RewardBasedVideoAd rewardAds;
//	private InterstitialAd InterstitialAds_Image;
//	private InterstitialAd InterstitialAds_Video;

//	private Dictionary<Ads_TYPE, int> Dic_RerequestAds = new Dictionary<Ads_TYPE, int>(); // 재요청된 광고들



//	public void Start_admob()
//	{
//		UserEditor.Getsingleton.EditLog("AdmobTool start ");

//		Init_Ads();

//		Set_RewardAd();

//		Set_InterstitialAd();

		
//	}


//	//광고 모듈 초기화
//	public void Init_Ads()
//	{
//#if UNITY_ANDROID
//		MobileAds.Initialize(android_AppId);
//#elif UNITY_IOS
//		MobileAds.Initialize(ios_AppId);
//#endif
//	}

//	//보상동영상광고 기초설정
//	public void Set_RewardAd()
//	{

//		rewardAds = RewardBasedVideoAd.Instance;

//		UserEditor.Getsingleton.EditLog("reward instance : " + rewardAds);

//		rewardAds.OnAdClosed += HandleOnRwdAdClosed;
//		rewardAds.OnAdFailedToLoad += HandleOnRwdAdFailedToLoad;
//		rewardAds.OnAdLoaded += HandleOnRwdAdLoaded;
//		rewardAds.OnAdOpening += HandleOnRwdAdOpening;
//		rewardAds.OnAdRewarded += HandleOnRwdAdReward;
//		rewardAds.OnAdStarted += HandleOnRwdAdStarted;

//		//동영상광고 광고요청
//		Requset_RewardAd();
//	}

//	//전면 광고 기초설정
//	public void Set_InterstitialAd()
//	{
//		string adUnitIdImg = string.Empty;
//		string adUnitVideo = string.Empty;

//#if UNITY_ANDROID
//		adUnitIdImg = android_interstitial_id_Image;
//		adUnitVideo = android_interstitial_id_Video;
//		UserEditor.Getsingleton.EditLog("Ads_ID : " + android_interstitial_id_Image);
//#elif UNITY_IOS
//		adUnitIdImg = ios_interstitial_id_Image;
//		adUnitVideo = ios_interstitial_id_Video;
//		UserEditor.Getsingleton.EditLog("Ads_ID : " + ios_interstitial_id_Image);
//#endif
//		InterstitialAds_Image = new InterstitialAd(adUnitIdImg);
//		InterstitialAds_Image.OnAdClosed += HandleOnInterstitialAdImage_Closed;
//		InterstitialAds_Image.OnAdFailedToLoad += HandleOnInterstitialAdImage_FailLoad;
//		InterstitialAds_Image.OnAdLoaded += HandleOnInterstitialAdImage_Loaded;

//		InterstitialAds_Video = new InterstitialAd(adUnitVideo);
//		InterstitialAds_Video.OnAdClosed += HandleOnInterstitialAdVideo_Closed;
//		InterstitialAds_Video.OnAdFailedToLoad += HandleOnInterstitialAdVideo_FailLoad;
//		InterstitialAds_Video.OnAdLoaded += HandleOnInterstitialAdVideo_Loaded;
//		//전면광고 요청 
//		Request_InterstitialAd(Ads_TYPE.Interstitial_Image);
//		Request_InterstitialAd(Ads_TYPE.Interstitial_Video);

//	}



//	// 광고 보기
//	public void Show_Ads(Ads_TYPE Ads)
//	{
//		if (Ads == Ads_TYPE.Reward)
//		{
//			if (!rewardAds.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob Reward Not IsLoaded ");

//				//Loadmanager.instance.LoadingUI(true);
//				//Dic_RerequestAds[Ads] = (int)Ads;
//				//Requset_RewardAd();
//			}
//			else
//			{
//				UserEditor.Getsingleton.EditLog("enable Show admob");
//				rewardAds.Show();
//			}
//		}
//		else if (Ads == Ads_TYPE.Interstitial_Image)
//		{
//			if (!InterstitialAds_Image.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면광고이미지 로드안되서 show 안됨");

//				//Loadmanager.instance.LoadingUI(true);
//				//Dic_RerequestAds[Ads] = (int)Ads;
//				//Request_InterstitialAd(Ads);
				
//			}
//			else
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면광고이미지 로드 완료 광고재생  ");
//				InterstitialAds_Image.Show();
//			}

//		}
//		else if (Ads == Ads_TYPE.Interstitial_Video)
//		{
//			if (!InterstitialAds_Image.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면광고동영상 로드안되서 show 안됨");

//				//Loadmanager.instance.LoadingUI(true);
//				//Dic_RerequestAds[Ads] = (int)Ads;
//				//Request_InterstitialAd(Ads);
				
//			}
//			else
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면광고동영상 로드 완료 광고재생");
//				//전면동영상 보이자
//				InterstitialAds_Video.Show();
//			}
//		}
//	}



//	public bool IsReadyAds(Ads_TYPE Ads)
//	{
//		bool isReady = false;

//		if (Ads == Ads_TYPE.Reward)
//		{
//			if (!rewardAds.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob Reward Not ready ");
//				Requset_RewardAd();
//				isReady = false;
//			}
//			else
//			{
//				isReady = true;
//			}
//		}
//		else if (Ads == Ads_TYPE.Interstitial_Image)
//		{
//			if (!InterstitialAds_Image.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면이미지광고 준비 안됨~~~~~");

//				//Request_InterstitialAd(Ads);
//				Request_InterstitialAd(Ads_TYPE.Interstitial_Image);

//				isReady = false;
//			}
//			else
//			{
//				isReady = true;
//			}
//		}
//		else if (Ads == Ads_TYPE.Interstitial_Video)
//		{
//			if (!InterstitialAds_Video.IsLoaded())
//			{
//				UserEditor.Getsingleton.EditLog("admob 전면동영상광고 준비 안됨~~~~~");

//				Request_InterstitialAd(Ads_TYPE.Interstitial_Video);
//				isReady = false;
//			}
//			else
//			{
//				isReady = true;
//			}
//		}


//		return isReady;
//	}




//	//보상동영상광고 광고요청
//	public void Requset_RewardAd()
//	{
//		string adUnitId = string.Empty;

//#if UNITY_ANDROID
//		adUnitId = android_rewardVideoAd_id;
//		UserEditor.Getsingleton.EditLog("Ads_ID : " + android_rewardVideoAd_id);
//#elif UNITY_IOS
//		adUnitId = ios_rewardVideoAd_id;
//		UserEditor.Getsingleton.EditLog("Ads_ID : " + ios_rewardVideoAd_id);
//#endif

//		rewardAds.LoadAd(new AdRequest.Builder().Build(), adUnitId);

//	}



//	#region RewardAs 콜백
	
//	//============================================================================================================================
//	//============================================ RewardAs 콜백 ===========================================================
//	//============================================================================================================================

//	public void HandleOnRwdAdClosed(object sender, EventArgs args)
//	{

//		//광고를 닫앗으니 다시 광고 로드하자
//		if (!rewardAds.IsLoaded())
//		{
//			UserEditor.Getsingleton.EditLog("Not IsLoaded ");
//			Requset_RewardAd();
//		}
//	}

//	public void HandleOnRwdAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//	{
//		//광고로드 실패했으니 다시 광고 로드하자
////		if (!rewardAds.IsLoaded())
////		{
//			UserEditor.Getsingleton.EditLog("Not IsLoaded ");
////			Requset_RewardAd();
////		}
//	}

//	public void HandleOnRwdAdLoaded(object sender, EventArgs args)
//	{

	
//		//재요청으로 들어온 광고
//		//if (Dic_RerequestAds.ContainsKey(Ads_TYPE.Reward))
//		//{
//		//    UserEditor.Getsingleton.EditLog("재요청으로 들어온광고-> 준비완료됨 바로 광고실행 ");

//		//    Loadmanager.instance.LoadingUI(false);
//		//    Dic_RerequestAds.Remove(Ads_TYPE.Reward);
//		//    rewardAds.Show();
//		//}
//		//else
//		{
//			//광고보기 준비 완료
//			UserEditor.Getsingleton.EditLog("동영상 광고보기 준비 완료 ");
//		}

//	}

//	public void HandleOnRwdAdReward(object sender, Reward args)
//	{
//		UserEditor.Getsingleton.EditLog("HandleOnRwdAdReward : " + args.ToString());


//		// 보상 내용처리
//		if (nextProcess != null)
//		{
//			nextProcess();
//			nextProcess = null;
//		}


//	}


//	public void HandleOnRwdAdOpening(object sender, EventArgs args)
//	{
//		// Pause the action.
//	}

//	public void HandleOnRwdAdStarted(object sender, EventArgs args)
//	{
//		// Mute Audio.
//	}

//#endregion





//	// 전면광고 이미지 , 동영상 요청
//	public void Request_InterstitialAd(Ads_TYPE Ads)
//	{
//		if (Ads == Ads_TYPE.Interstitial_Image)
//		{
//			InterstitialAds_Image.LoadAd(new AdRequest.Builder().Build());
			
//			UserEditor.Getsingleton.EditLog("complete Request InterstitialAds_Image");
//		}
//		else if (Ads == Ads_TYPE.Interstitial_Video)
//		{
//			InterstitialAds_Video.LoadAd(new AdRequest.Builder().Build());
			

//			UserEditor.Getsingleton.EditLog("complete Request InterstitialAds_Video");
//		}

//	}



//#region  interstitial 콜백관련

//	//============================================================================================================================
//	//============================================ interstitial 콜백 ===========================================================
//	//============================================================================================================================

//	//전면이미지 로드할떄
//	public void HandleOnInterstitialAdImage_Loaded(object sender, EventArgs args)
//	{

//		//if (Dic_RerequestAds.ContainsKey(Ads_TYPE.Interstitial_Image))
//		//{
//		//    UserEditor.Getsingleton.EditLog("전면이미지 재요청 로드완료 , 바로 광고 실행");
//		//    Loadmanager.instance.LoadingUI(false);
//		//    Dic_RerequestAds.Remove(Ads_TYPE.Interstitial_Image);
//		//    InterstitialAds_Image.Show();
//		//}
//		//else
//		{
//			UserEditor.Getsingleton.EditLog("전면이미지 로드완료");

//		}

//	}


//	//전면동영상 로드할때
//	public void HandleOnInterstitialAdVideo_Loaded(object sender, EventArgs args)
//	{

//		//if (Dic_RerequestAds.ContainsKey(Ads_TYPE.Interstitial_Video))
//		//{
//		//    UserEditor.Getsingleton.EditLog("전면동영상 재요청 로드완료 , 바로 광고 실행");
//		//    Loadmanager.instance.LoadingUI(false);
//		//    Dic_RerequestAds.Remove(Ads_TYPE.Interstitial_Video);
//		//    InterstitialAds_Video.Show();
//		//}
//		//else
//		{
//			UserEditor.Getsingleton.EditLog("전면동영상 로드 완료 ");


//		}

//	}




//	// 전면이미지광고 닫을떄
//	public void HandleOnInterstitialAdImage_Closed(object sender, EventArgs args)
//	{
//		UserEditor.Getsingleton.EditLog("HandleOnInterstitialAdImage_Closed:" + args.ToString());
//		if (!InterstitialAds_Image.IsLoaded())
//		{
//			Request_InterstitialAd(Ads_TYPE.Interstitial_Image);
//		}

//		//이미지광고 닫을떄 다음행동 하자 
//		if (nextProcess != null)
//		{
//			nextProcess();
//			nextProcess = null;
//		}
//	}

//	// 전면동영상광고 닫을떄
//	public void HandleOnInterstitialAdVideo_Closed(object sender, EventArgs args)
//	{

//		UserEditor.Getsingleton.EditLog("HandleOnInterstitialAdVideo_Closed:" + args.ToString());

//		if (!InterstitialAds_Video.IsLoaded())
//		{
//			Request_InterstitialAd(Ads_TYPE.Interstitial_Video);
//		}

//		//동영상광고 닫을떄 다음행동 하자 
//		if (nextProcess != null)
//		{
//			nextProcess();
//			nextProcess = null;
//		}

//	}

//	// 전면이미지광고 로드 실패할떄
//	public void HandleOnInterstitialAdImage_FailLoad(object sender, EventArgs args)
//	{
//		UserEditor.Getsingleton.EditLog("HandleOnInterstitialAdImage_FailLoad:" + args.ToString());

////		if (!InterstitialAds_Image.IsLoaded())
////		{
////			Request_InterstitialAd(Ads_TYPE.Interstitial_Image);
////		}
	
//	}

//	// 전면동영상광고 로드 실패할떄
//	public void HandleOnInterstitialAdVideo_FailLoad(object sender, EventArgs args)
//	{
//		UserEditor.Getsingleton.EditLog("HandleOnInterstitialAdVideo_FailLoad:" + args.ToString());

////		if (!InterstitialAds_Video.IsLoaded())
////		{
////			Request_InterstitialAd(Ads_TYPE.Interstitial_Video);
////		}
		
//	}
//#endregion


//}
