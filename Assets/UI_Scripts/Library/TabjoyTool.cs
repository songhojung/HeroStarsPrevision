//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TapjoyUnity;

//public class TabjoyTool 
//{
//	public static TabjoyTool instance = new TabjoyTool();

//	private Dictionary<string, TJPlacement> Dic_TjPlacemt = new Dictionary<string, TJPlacement>();
//	private Dictionary<string, TJPlacement> Dic_RerequestPm = new Dictionary<string, TJPlacement>();	//요청fail 되어 재요청 된 placement들

//	public del_NextProcess nextProcessEvent;

//	public TabjoyTool()
//	{
//		Start_tapJoy();
//	}



//	public void Start_tapJoy()
//	{
//		Tapjoy.OnConnectSuccess += HandleConnectSuccess;
//		Tapjoy.OnConnectFailure += HandleConnectFailure;

//		UserEditor.Getsingleton.EditLog("Start_tapJoy ");

//		if (!Tapjoy.IsConnected)
//		{
//			UserEditor.Getsingleton.EditLog("Try Connect ");
//			Tapjoy.Connect();
//		}
		
//	}


//	public void HandleConnectSuccess()
//	{
//		UserEditor.Getsingleton.EditLog("Connect Success");



//		TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
//		TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
//		TJPlacement.OnContentReady += HandlePlacementContentReady;
//		TJPlacement.OnContentShow += HandlePlacementContentShow;
//		TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
//		TJPlacement.OnVideoStart += HandlePlacementVideoStart;
//		TJPlacement.OnVideoComplete += HandlePlacementVideoComplete;
//		TJPlacement.OnVideoError += HandlePlacementVideoError;


//		모든 탭조이 콘텐츠 요청
//		AllRequest_TjContent();

//	}

//	public void HandleConnectFailure()
//	{
//		UserEditor.Getsingleton.EditLog("Connect fail");

//	}







//	public void AllRequest_TjContent()
//	{
//		TJPlacement plmInterVideo = TJPlacement.CreatePlacement("intertitalAd");
//		Dic_TjPlacemt["intertitalAd"] = plmInterVideo;
//		TJPlacement plmReward = TJPlacement.CreatePlacement("reawardAd");
//		Dic_TjPlacemt["reawardAd"] = plmReward;
//		TJPlacement plmAnncMnt1 = TJPlacement.CreatePlacement("notice");
//		Dic_TjPlacemt["notice"] = plmAnncMnt1;
//		TJPlacement plmAnncMnt2 = TJPlacement.CreatePlacement("notice1");
//		Dic_TjPlacemt["notice1"] = plmAnncMnt2;


//		foreach (var pm in Dic_TjPlacemt)
//		{
//			pm.Value.RequestContent();
//		}
//		UserEditor.Getsingleton.EditLog("Complete All Request content ");
//	}




//	public void Request_Ads(Ads_TYPE adsType)
//	{
//		if(adsType == Ads_TYPE.Interstitial_Image)
//		{
//			Dic_TjPlacemt["intertitalAd"].RequestContent();
			
//		}
//		else if(adsType == Ads_TYPE.Reward)
//		{
//			Dic_TjPlacemt["reawardAd"].RequestContent();

//		}
//	}








//	public bool IsReadyAds(string _plmName)
//	{
//		bool isready = false; 
//		TJPlacement plm = null;
//		if (Dic_TjPlacemt.ContainsKey(_plmName))
//		{
//			plm = Dic_TjPlacemt[_plmName];
//			if (plm.IsContentReady())
//				isready = true;
//			else
//				isready = false;
//		}
//		else
//		{
//			plm = TJPlacement.CreatePlacement(_plmName);
//			Dic_TjPlacemt[_plmName] = plm;
//			plm.RequestContent();
//			isready = false;
//		}

//		return isready;
//	}




//	탭조이 광고 보기 
//	public void Show_TjAds(string _plmName)
//	{
//		로딩바 켬

//		TJPlacement plm = null;
//		if (Dic_TjPlacemt.ContainsKey(_plmName))
//		{
			
//			plm = Dic_TjPlacemt[_plmName];

//			컨텐츠 보여주기
//			if (plm.IsContentReady())
//			{
//				UserEditor.Getsingleton.EditLog("탭조이광고 SHOW");
//				plm.ShowContent();
//			}
//			else
//			{
//				컨텐츠준비 안되엇으니 재요청 할당하고 요청시도
//				Loadmanager.instance.LoadingUI(true);
//				Dic_RerequestPm[plm.GetName()] = plm;
//				plm.RequestContent();
//			}
//		}
//		else
//		{
//			플레이스먼트 생성하고 요청하고 재요청dic에 할당
//			UserEditor.Getsingleton.EditLog("플레이먼트 없다 생성한다 : " + _plmName);
//			Loadmanager.instance.LoadingUI(true);
//			plm = TJPlacement.CreatePlacement(_plmName);
//			Dic_TjPlacemt[_plmName] = plm;
//			Dic_RerequestPm[plm.GetName()] = plm;
//			plm.RequestContent();
//		}

		
//	}



//	탭조이 컨텐트 보여주기 
//	public void Show_TjContent(string _plmName)
//	{
//		TJPlacement plm = null;
//		if (Dic_TjPlacemt.ContainsKey(_plmName))
//		{

//			plm = Dic_TjPlacemt[_plmName];

//			컨텐츠 보여주기
//			if (plm.IsContentReady())
//			{
//				UserEditor.Getsingleton.EditLog(_plmName + " Tapjoy content SHOW");
//				plm.ShowContent();
//			}
//			else
//			{
//				컨텐츠준비 안되엇으니 재요청 할당하고 요청시도
//				Loadmanager.instance.LoadingUI(true);
//				Dic_RerequestPm[plm.GetName()] = plm;
//				plm.RequestContent();
//			}
//		}
//		else
//		{
//			플레이스먼트 생성하고 요청하고 재요청dic에 할당
//			UserEditor.Getsingleton.EditLog("플레이먼트 없다 생성한다 : " + _plmName);
//			plm = TJPlacement.CreatePlacement(_plmName);
//			Dic_TjPlacemt[_plmName] = plm;
//			Dic_RerequestPm[plm.GetName()] = plm;
//			plm.RequestContent();
//		}
//	}





//	public void HandlePlacementRequestSuccess(TJPlacement placement)
//	{
//		UserEditor.Getsingleton.EditLog("RequestSuccess : " + placement.GetName());
//		로딩바 끔

//		Loadmanager.instance.LoadingUI(false);
//		//재요청되었던 플레이스먼트가 success 되엇으니 컨텐츠 보여주자
//		if (Dic_RerequestPm.ContainsKey(placement.GetName()))
//		{
//		    Dic_RerequestPm.Remove(placement.GetName());

//		    placement.ShowContent();
//		}
		

//	}



//	public void HandlePlacementRequestFailure(TJPlacement placement, string error)
//	{
//		UserEditor.Getsingleton.EditLog("Request Failure : " + error);

	
	
//	}

//	public void HandlePlacementContentReady(TJPlacement placement)
//	{
//		 This gets called when content is ready to show.
//		UserEditor.Getsingleton.EditLog("Content Ready :" +placement.GetName());
//	}


//	public void HandlePlacementContentShow(TJPlacement placement)
//	{
//		UserEditor.Getsingleton.EditLog("Content Show : " + placement.GetName());

		



//		보기 완료햇으니 요청시도
//		placement.RequestContent();

//	}

//	public void HandlePlacementContentDismiss(TJPlacement placement)
//	{
//		보기 완료햇으니 다음 행동개시
//		if (nextProcessEvent != null)
//			nextProcessEvent();

//		UserEditor.Getsingleton.EditLog("Content Dismiss : " + placement.GetName());
//	}







//	public void HandlePlacementVideoStart(TJPlacement placement)
//	{


//		UserEditor.Getsingleton.EditLog("HandlePlacement Video Start : " + placement.GetName());
//	}

//	public void HandlePlacementVideoComplete(TJPlacement placement)
//	{


//		UserEditor.Getsingleton.EditLog("HandlePlacement Video Complete : " + placement.GetName());
//	}

//	public void HandlePlacementVideoError(TJPlacement placement, string errorMessage)
//	{

//		UserEditor.Getsingleton.EditLog("HandlePlacement Video Error : " + placement.GetName() + "errorMessage : " + errorMessage);
//	}

	
//}
