//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TapjoyUnity;
//public class AnalysisManager 
//{
//	public static AnalysisManager instance = new AnalysisManager();


//	public AnalysisManager()
//	{
//		AppsflyerTool.instance.Init_Appsflyer();
//	}






//	//========유저정보 분석 =========

//	public void Anl_SetUserID(uint userid)
//	{
//		Tapjoy.SetUserID(userid.ToString());
//	}


//	// ======구매 분석 =======

//	public void Anl_Purchace(string productName , string currencyCode ,double price , string campaignID = null)
//	{
//		UserEditor.Getsingleton.EditLog("구매 통계  : " + productName + " / " + currencyCode + " / " + price);
//		Tapjoy.TrackPurchase(productName, currencyCode, price, campaignID);


//		//골드 구매 통계는 무시
//		string FindStr = "골드";
//		bool isGold = productName.Contains(FindStr);

//		if (isGold == false)
//		{

//			//앱스플라이어 구매통계
//			AppsFlyer.trackRichEvent(AFInAppEvents.PURCHASE, new Dictionary<string, string>()
//			{
//				{AFInAppEvents.CONTENT_ID,productName},
//				{AFInAppEvents.QUANTITY,"1"},
//				{AFInAppEvents.REVENUE,price.ToString()},
//				{AFInAppEvents.CURRENCY,currencyCode},
//			});
//		}
//		else
//		{
//			UserEditor.Getsingleton.EditLog("골드 구매 앱스플라이어통계  는 안함");
//		}
		
//	}



//	//=========커스텀 이벤트 ==============

//	public void Anl_CustomEvt(string evtName , long value  = 0)
//	{
//		UserEditor.Getsingleton.EditLog("custom evnet !!!!  1");
//		Tapjoy.TrackEvent(evtName, value);

//		//앱스플라이어 이벤트
//		AppsFlyer.trackRichEvent(evtName, new Dictionary<string, string>()
//		{
//			{"value",value.ToString()},
//		});
//	}

//	public void Anl_CustomEvt(string category ,string evtName, long value)
//	{
//		UserEditor.Getsingleton.EditLog("custom evnet !!!!    2");
//		Tapjoy.TrackEvent(category,evtName, value);

//		//앱스플라이어 이벤트
//		AppsFlyer.trackRichEvent(evtName, new Dictionary<string, string>()
//		{
//			{"value",value.ToString()},
//		});
//	}

//	public void Anl_CustomEvt(string category, string evtName, string parameta1, string parameta2 = null, long value = 0)
//	{
//		UserEditor.Getsingleton.EditLog("custom evnet !!!!    3");
//		Tapjoy.TrackEvent(category, evtName, parameta1, parameta2,value);

//		//앱스플라이어 이벤트
//		Dictionary<string, string> dic_event = new Dictionary<string, string>();
//		dic_event.Add(parameta1, value.ToString());
//		AppsFlyer.trackRichEvent(evtName, dic_event);
//	}

//	public void Anl_CustomEvt(string category, string evtName, string parameta1, string parameta2,
//		string valueName1 , long value1 , string valueName2 = null , long value2 = 0 , string valueName3 = null, long value3 = 0 )
//	{
//		UserEditor.Getsingleton.EditLog("custom evnet !!!!    4");
//		Tapjoy.TrackEvent(category, evtName, parameta1, parameta2, valueName1, value1,valueName2,value2,valueName3,value3);

//		//앱스플라이어 이벤트
//		Dictionary<string, string> dic_event = new Dictionary<string, string>();
//		dic_event.Add(parameta1, "");
//		dic_event.Add(parameta2, "");
//		dic_event.Add(valueName1, value1.ToString());
//		if(!string.IsNullOrEmpty(valueName2))
//			dic_event.Add(valueName2, value2.ToString());
//		if (!string.IsNullOrEmpty(valueName3))
//			dic_event.Add(valueName3, value3.ToString());
//		AppsFlyer.trackRichEvent(evtName, dic_event);
//	}
	
//}
