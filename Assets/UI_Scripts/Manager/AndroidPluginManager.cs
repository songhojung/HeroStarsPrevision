using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class AndroidPluginManager : MonoBehaviour
{

	private static AndroidPluginManager _instance;
	public static AndroidPluginManager Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(AndroidPluginManager)) as AndroidPluginManager;
				if (_instance == null)
				{
					_instance = new GameObject("AndroidPluginManager").AddComponent<AndroidPluginManager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}

	AndroidJavaClass unityPlayer;
	AndroidJavaObject activity;
	public uint addFriendUserID = 0;
	public string addFriendToken = null;

	public string countryCode = string.Empty;


	public del_NextProcess2 webCloseEventDel;
	void Awake()
	{
		Init();
#if UNITY_EDITOR
#else
		
		 unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		 
		 activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
	}

	public void Init()
	{
		addFriendUserID = 0;
		addFriendToken = string.Empty;
	}


	//공유할앱리스트 띄우기
	public void CallShare(string message)
	{

		AndroidJavaObject app = activity.Call<AndroidJavaObject>("getApplicationContext");

		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			//androidclass.Call("callmyshare", app,"babo");
			activity.Call("callmyshare", app, message);
			UserEditor.Getsingleton.EditLog("call callShare function");
		}));
	}

	/// <summary>
	//native의 addfriend 함수를 호출한다.
	/// </summary>
	public void Call_AddFriend(string message)
	{
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			//androidclass.Call("callmyshare", app,"babo");
			activity.Call("callFunc_AddFriend", message);
			UserEditor.Getsingleton.EditLog("call..... callFunc_AddFriend function");
		}));
	}

	/// <summary>
	/// native가 호출하여 친구추가할 ID를받고 프로토콜보내자 (보석보상줌 소식으로)
	/// </summary>
	public void Get_addFriendForReward(string userid)
	{
		UserEditor.Getsingleton.EditLog(userid + " __unity__main___add friend For Reward");
		//
		if (!String.IsNullOrEmpty(userid))
		{
			string[] splitString = userid.Split('/');
			addFriendUserID = uint.Parse(splitString[0]);
			addFriendToken = splitString[1];

			UserEditor.Getsingleton.EditLog("addfriend ID : " + addFriendUserID + "addFriendToken : " + addFriendToken);
			if (AndroidPluginManager.Getsingleton.addFriendUserID != 0)
				webRequest.FriendAdd(addFriendUserID, addFriendToken, callback_Complete_addFriendReward);
		}
	}

	void callback_Complete_addFriendReward()
	{
		UserEditor.Getsingleton.EditLog("complete addfriendReward");
		StartCoroutine(Coroutine_AddedFriendReward());
	}

	IEnumerator Coroutine_AddedFriendReward()
	{
		while (true)
		{
			if (UI_Manager.Getsingleton._UI != UI.TITLE)
			{
				UserEditor.Getsingleton.EditLog("complete friend reward coroutine");
				UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popup.SetPopupMessage("친구초대 보상이 지급되었습니다. ");

				// friend id , friend token 초기화
				Init();

				break;
			}

			yield return null;
		}
	}





	/// <summary>
	/// native의 앱가드 서버인증 함수 호출한다.
	/// </summary>
	public void callSetAuthServer(string clientID)
	{
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			activity.Call("callSetAuthServer", clientID);
			UI_Title.Getsingleton.Write_TitleLog("Checking Authentication...");
			UserEditor.Getsingleton.EditLog("unity call..... callSetAuthServer");
			Loadmanager.instance.LoadingUI(true);
		}));
	}

	//native가 앱가드서버인증 성공시 호출하는 함수이다
	public void ResultAppGuarding(string meg)
	{
		UserEditor.Getsingleton.EditLog("ResultAppGurading : " + meg);
		if (meg.Equals("success")) // 성공하면 로그인절차
		{
			UI_Title.Getsingleton.Write_TitleLog("Sucess Checking Authentication...");
			UI_Title.Getsingleton.Login_AppguradingProcess();
		}
		else// 실패하면 게임끈다
		{
			UI_Title.Getsingleton.Write_TitleLog("Fail Checking Authentication...");
			Application.Quit();
		}
		Loadmanager.instance.LoadingUI(false);
	}




	public int GetAndroidAPILevel()
	{
		int level = 0;
#if UNITY_ANDROID
		var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
		var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
		var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
		level = Convert.ToInt32(sdkLevel);
#endif
		Debug.Log("sdk level : " + level);

		return level;
	}





	/// <summary>
	/// 국가코드 반환 안드로이트 네이티브 함수실행
	/// </summary>
	public void Call_CountryCodeFunction(string message)
	{
		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			//androidclass.Call("callmyshare", app,"babo");
			activity.Call("Get_AndroidCountryCode", message);
			UserEditor.Getsingleton.EditLog("call..... Get_AndroidCountryCode function");
		}));
	}


	public void Get_CountryCode(string code)
	{
		UserEditor.Getsingleton.EditLog("get code !!!! => " + code);
		if (!String.IsNullOrEmpty(code))
		{
			countryCode = code;
		}
	}




	public void StartWebView(string url, int wid, int hei, string btnFileName, int bntWid, int btnHei, bool bShowCheckBox, string checkBoxTxt)
	{


		float addValue = Mathf.Clamp((float)hei / 640f, 1f, 1.5f);
		if (addValue < 1f) addValue = 1f;
		bntWid = (int)(bntWid * addValue);
		btnHei = (int)(btnHei * addValue);



		activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			activity.Call("showWebView", url, wid, hei, bShowCheckBox, checkBoxTxt);
			UserEditor.Getsingleton.EditLog("unity call..... webView");
		}));

	}


	//웹뷰 종료 이벤트 
	public void setWebViewClose(string param)
	{
		//웹뷰가 닫힘
		UserEditor.Getsingleton.EditLog("setWebViewClose: " + param + " webCloseEventDel = " + webCloseEventDel);
		if (webCloseEventDel != null)
		{
			webCloseEventDel(param);
		}
	}
}
