using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_Setting : UI_Base 
{
	public enum SettingUIType
	{
		None = 99,
		Game =0,
		Language = 1,
		Service = 2,
	}

	private OptionSetting option_Setting;
	private SettingUIType nowTapType = SettingUIType.Game;
	private int nowTapIdx = 0;
	private LanguageCode nowLanguage = LanguageCode.NONE;
	private PLATFORM_TYPE nowUsePlatform = PLATFORM_TYPE.NONE;

	public List<Toggle> Toggle_Languages;
	public List<Toggle> Toggle_fps;
	public List<Toggle> Toggle_AttackType;
	public List<Toggle> Toggle_Push;
    public Slider Slider_viewSens;
	public Slider Slider_ZoomInViewSens;
    public Slider Slider_BGM;
    public Slider Slider_voice;
    public Slider Slider_effect;
	
    

	public GameObject Obj_game;
	public GameObject Obj_Language;

	public List<GameObject> Lst_Obj;
	public List<GameObject> Lst_GameSettingObj;
	public List<GameObject> Lst_PlaformButtonObj;

	public UI_Popup_Country popupCountry;

	//계정 UI
	public Text text_userID;
	public Text text_NkNm;
	public Text text_Killmsg;
	public Text text_Version;
	public Image image_PlatformIcon;
	public List<InputField> lst_InputGame;
	public string changedTxt = string.Empty;

	//쿠폰UI
	public UI_Popup_Coupon popupCoopon;
	public GameObject cooPonObj;			//쿠폰 오프젝트

	private static UI_Setting _instance;
	public static UI_Setting Getinstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Setting)) as UI_Setting;

				if (_instance == null)
				{
					GameObject newObj = new GameObject("UI_Setting");
					_instance = newObj.AddComponent<UI_Setting>();
				}
			}
			return _instance;
		}
	}


	public override void set_Open()
	{
		base.set_Open();

		option_Setting = OptionSetting.instance;

		Set_setting();

		
				

	}

	public override void set_Close()
	{
		base.set_Close();

		//닫힐떄 국가선택팝업닫기
		//popupCountry.gameObject.SetActive(false);

		//setting 값들 로컬에 저장
		Save_SettingValues();
	}

	public override void set_refresh()
	{
		base.set_refresh();

		Set_setting();
	}

	void Set_setting()
	{

		//쿠폰 오브젝트 설정
//#if UNITY_EDITOR
//		cooPonObj.SetActive(true);
//#elif UNITY_ANDROID
//		cooPonObj.SetActive(true);
//#elif UNITY_IOS
//		cooPonObj.SetActive(false);
//#endif


		//탭설정
		Set_ListObject(nowTapType);

		//게임탭쪽 값 설정
		Set_SettingGame();

		//계정탭쪽 값 설정
		Set_SettingService();

		//언어탭쪽 값 설정
		Set_SettingLanguage();

	}

	void Set_ListObject(SettingUIType _nowType)
	{
		for (int i = 0; i < Lst_Obj.Count; i++)
			Lst_Obj[i].SetActive(_nowType == (SettingUIType)i);
	}

	void Set_GameSettingObject(int[] Idxs,bool isActive)
	{
		for (int i = 0; i < Idxs.Length; i++ )
		{
			Lst_GameSettingObj[Idxs[i]].SetActive(isActive);
		}
	}

	void Set_SettingGame()
	{

		//수치
		Slider_viewSens.value = option_Setting.Sensitive / 100f;
		Slider_ZoomInViewSens.value = option_Setting.SensitiveZoomIn / 100f;
		Slider_BGM.value = option_Setting.VolumBGM / 100f;
		Slider_voice.value = option_Setting.VolumVoice / 100f;
		Slider_effect.value = option_Setting.VolumEffect / 100f;

		//토글
		//SetBoolean_Toggle(option_Setting.Notice_pushMsg, Toggle_NoticeMsg);
		//SetBoolean_Toggle(option_Setting.Clan_pushMsg, Toggle_ClanMsg);
		//SetBoolean_Toggle(option_Setting.UnlockBx_pushMsg, Toggle_unlockBxMsg);
		SetBoolean_Toggle(option_Setting.Notice_pushMsg, Toggle_Push);
		SetBoolean_Toggle(option_Setting.FramePerSecond, Toggle_fps);
		SetBoolean_Toggle(option_Setting.AttackType, Toggle_AttackType);


		//게임관련쪽 계정부분
		Set_AccounInfo();
	}

	void Set_AccounInfo()
	{
		User _user = UserDataManager.instance.user;

		//유저아이디
		text_userID.text = _user.user_Users.UserID.ToString();

		//유저명
		text_NkNm.text = _user.user_Users.NkNm;
		//킬메세지
		if (_user.User_Words.ContainsKey(USERWORD_TYPE.WRD_KILL))
			text_Killmsg.text = _user.User_Words[USERWORD_TYPE.WRD_KILL].Words;
		else
			text_Killmsg.text = "";

		//플랫폼 인덱스
		nowUsePlatform = (PLATFORM_TYPE)PlatformManager.Getsingleton.ChkGet_BeforeUsePlatformIndex();

		//플랫폼 이미지
		Set_PlatformIcon((byte)nowUsePlatform);

		//플랫폼 버튼 활성,비활성
		Set_PlatformButton((byte)nowUsePlatform);
	}


	

	// 클랜 푸쉬알림 체크가 변경사항이 있는지 확인하고 setting 값저장
	void Save_SettingValues()
	{
		
		{
			//setting 값 저장
			Apply_SettingValues();
		}

		
	}

	//setting 중에 값들있는것을 가져와 user에 저장한다.
	void Apply_SettingValues()
	{
		//게임쪽...
		option_Setting.Notice_pushMsg = GetBoolean_Toggle(Toggle_Push);
		//option_Setting.Clan_pushMsg = Toggle_Push[(int)PushAlaram_TYPE.CLAN].isOn;
		//option_Setting.UnlockBx_pushMsg = Toggle_Push[(int)PushAlaram_TYPE.BOXOPEN].isOn;
		option_Setting.GraphicQuality = GetBoolean_Toggle(Toggle_fps);
		option_Setting.AttackType = GetBoolean_Toggle(Toggle_AttackType);
		option_Setting.FramePerSecond = GetBoolean_Toggle(Toggle_fps);

		option_Setting.Save_SettingValues();
	}



	//토글 관련 setting 들을 user 정보 토대로 설정시킨다
 	void SetBoolean_Toggle(bool _ison, List<Toggle> _toggles)
	{
		if (_ison)
		{
			_toggles[0].isOn = true;
			_toggles[1].isOn = false;
		}
		else
		{
			_toggles[1].isOn = true;
			_toggles[0].isOn = false;
		}
	}







	// 선택되었던 언어를 설정한다
	void Set_SettingLanguage()
	{
		int langIndex = (int)option_Setting.usingLangueage;

		if (langIndex <= Toggle_Languages.Count)
		{
			for (int i = 0; i < Toggle_Languages.Count; i++)
			{
				if (i == langIndex)
					Toggle_Languages[i].isOn = true;
				else
					Toggle_Languages[i].isOn = false;
			}
		}
	}



	//토글관련 setting들 의 bool 값을 가져온다
	bool GetBoolean_Toggle(List<Toggle> _toggles)
	{
		bool _isOn = false;
		if (_toggles[0].isOn == true) _isOn = true;
		else if (_toggles[1].isOn == true) _isOn = false;

		return _isOn;
	}


    // 시선감도 조정 조절 이벤트 매서드
    public void ResponseSlide_viewSensitive()
    {
		int value = (int)(Slider_viewSens.value * 100);
		option_Setting.Sensitive = value;
    }


	// 줌인시선감도 조정 조절 이벤트 매서드
	public void ResponseSlide_ZoomInViewSensitive()
	{
		int value = (int)(Slider_ZoomInViewSens.value * 100);
		option_Setting.SensitiveZoomIn = value;
	}


	// 이펙트음 조절 이벤트 매서드
	public void ResponseSlide_soundEffect()
	{
		int value = (int)(Slider_effect.value * 100);
		option_Setting.VolumEffect = value;
	}

	// 음성 조절 이벤트 매서드
	public void ResponseSlide_soundVoice()
	{
		int value = (int)(Slider_voice.value * 100);
		option_Setting.VolumVoice = value;
	}

	// 배경음 조절 이벤트 매서드
	public void ResponseSlide_soundBGM()
	{
		int value = (int)(Slider_BGM.value * 100);
		option_Setting.VolumBGM = value;
	}





    // 탭버튼이밴트
	public void ResponseButton_Tap(int tapIdx)
	{
		if (nowTapIdx != tapIdx)
		{
			nowTapIdx = tapIdx;
			nowTapType = (SettingUIType)nowTapIdx;

			Set_ListObject(nowTapType);

		
			 
		}
	}







    // 언어 변경 토글키 이벤트
	public void ResponseButton_Language(int lang)
	{
		LanguageCode _lang = (LanguageCode)Enum.Parse(typeof(LanguageCode), lang.ToString());

		if (nowLanguage != _lang)
		{
			nowLanguage = _lang;

			//language 값 유저에 저장
			option_Setting.usingLangueage = lang;
			LanguageManager.Getsingleton.language = _lang;
			LanguageManager.Getsingleton.SetLanguage(_lang);
			LanguageManager.Getsingleton.SetLanguageRefresh();

			//자살 문구 로컬라이징
			User _user = UserDataManager.instance.user;
			if (_user.User_Words.ContainsKey(USERWORD_TYPE.WRD_SUICIDE))
				_user.User_Words[USERWORD_TYPE.WRD_SUICIDE].Words = TextDataManager.Dic_TranslateText[438]; //자살문구 
			else
			{
				User_Word word1 = new User_Word();
				word1.WrdKind = USERWORD_TYPE.WRD_SUICIDE;
				word1.Words = TextDataManager.Dic_TranslateText[438]; //자살문구 
				_user.User_Words[word1.WrdKind] = word1;
			}

			//언어 변경햇으니 로그인프로토콜 다시 쏘자
			Network_MainMenuSoketManager.Getsingleton.Send_CTS_Login();



		}
	}


	//국가 선택 
	public void ResponButton_Popup_Country()
	{
		popupCountry.gameObject.SetActive(true);

		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.gameObject.SetActive(false);
	}




	//유저이름 변경
	public void ResponseInput_EndChangeName()
	{
		User _user = UserDataManager.instance.user;
		if (!lst_InputGame[0].wasCanceled)
		{
			if (_user.User_Times.ContainsKey((int)TIMEIDX.USERNAME))
			{
				User_Times Usertime = _user.User_Times[(int)TIMEIDX.USERNAME];

				if (Usertime.Etime >= TimeManager.Instance.Get_nowTime())
				{
					//토스트팝업
					UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
					popup.SetPopupMessage(TextDataManager.Dic_TranslateText[244]);//이름변경은 1일 1회 입니다.

					Set_AccounInfo();

					lst_InputGame[0].text = "";
				}
				else
				{
					string changedName = lst_InputGame[0].text;
					if (!TextDataManager.Chk_BannedLetter(ref changedName))
					{
						webRequest.UserNameChange(lst_InputGame[0].text, 0, callback_complete_changeName);
						lst_InputGame[0].text = "";
					}
					else
					{
						UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
						popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

						Set_AccounInfo();

						lst_InputGame[0].text = "";
					}

				}

			}
			else
			{
				string changedName = lst_InputGame[0].text;
				if (!TextDataManager.Chk_BannedLetter(ref changedName))
				{
					webRequest.UserNameChange(lst_InputGame[0].text, 0, callback_complete_changeName);
					lst_InputGame[0].text = "";
				}
				else
				{
					UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
					popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

					Set_AccounInfo();

					lst_InputGame[0].text = "";
				}
			}
		}
		else
		{
			text_NkNm.text = _user.user_Users.NkNm;
		}
	}

	void callback_complete_changeName()
	{
		Set_AccounInfo();
		
		lst_InputGame[0].text = "";

		//이름변경하엿으니 소켓로그인 다시쏘자
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_Login();
		
	}


	//킬문구 변경
	public void ResponseInput_EndChangeKillMsg()
	{
		User _user = UserDataManager.instance.user;


		if (!lst_InputGame[1].wasCanceled)
		{
			string changedName = lst_InputGame[1].text;
			if (!TextDataManager.Chk_BannedLetter(ref changedName))
			{
				webRequest.SetWords(lst_InputGame[1].text, callback_complete_ChangeKillmsg);
				changedTxt = lst_InputGame[1].text;
				lst_InputGame[1].text = "";
			}
			else
			{
				UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

				Set_AccounInfo();

				lst_InputGame[1].text = "";
			}
		}
		else
		{
			if (_user.User_Words.ContainsKey(USERWORD_TYPE.WRD_KILL))
				text_Killmsg.text = _user.User_Words[USERWORD_TYPE.WRD_KILL].Words;
		}
	}
	 
	void callback_complete_ChangeKillmsg()
	{
		Set_AccounInfo();

		//소켓서버 킬문구 갱신을위해 데이터 전송
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_SET_KILLWORD(changedTxt);
		lst_InputGame[1].text = "";
	}

#region 서비스 관련
	void Set_SettingService()
	{
		//버젼정보
		int ver = DefineKey.Release_AOS_Version;
#if UNITY_ANDROID
		ver = DefineKey.Release_AOS_Version;
#elif UNITY_IOS
		ver = DefineKey.Release_IOS_Version;
#endif
		



		if (ver >= 1000)
		{
			int _fir = (int)ver / 1000;
			int _firdiff = ver - (1000 * _fir);
			int _sec = (int)(_firdiff / 100);
			int _thir = _firdiff - (100 * _sec);


			if(_thir != 0)
				text_Version.text = string.Format("version : {0}.{1}.{2:D2}", _fir, _sec, _thir);
			else
				text_Version.text = string.Format("version : {0}.{1}.00", _fir, _sec);

		}
		else if (ver >= 100)
		{

			int _sec = (int)(ver / 100);
			int _thir = ver - (100 * _sec);
			text_Version.text = string.Format("version : 0.{0}.{1:D2}", _sec, _thir);
		}
		else if (ver >= 10)
		{
			text_Version.text = string.Format("version : 0.0.{0}", ver);
		}
		else
		{
			text_Version.text = string.Format("version : 0.0.{0:D2}", ver);
		}



	}



	public void ResponseButton_Comfirm_ServiceProvision()
	{
		//http://183.98.145.233:40002/Info/GetAgree1
		//Application.OpenURL(string.Format("{0}/Info/GetAgree1", web_Manager.Getsingleton.URL));

	}

	public void ResponseButton_Comfirm_PersonalInfoProvision()
	{
		//http://183.98.145.233:40002/Info/GetAgree2
		//Application.OpenURL(string.Format("{0}/Info/GetAgree2", web_Manager.Getsingleton.URL));
	}


	//문의관련 버튼
	public void ResposeButton_Question()
	{
		User _user = UserDataManager.instance.user;

		//string mailto = "help@clegames.com";
		//string subject = EscapeURL(TextDataManager.Dic_TranslateText[227]);
		//string body = EscapeURL
		//    (
		//     TextDataManager.Dic_TranslateText[228] + "\n\n\n\n\n\n\n"
		//     + "______________________" + "\n\n" +
		//    "UserID : " + _user.user_Users.UserID + "\n\n" +
		//    "User Nickname : " + _user.user_Users.NkNm + "\n\n" +
		//    "App Version : " + "\n\n" +
		//    "Device Model : " + SystemInfo.deviceModel + "\n\n" +
		//    "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
		//    "______________________"
		//    );



		//Application.OpenURL("mailto:" + mailto + "?subject=" + subject + "&body=" + body);

		string serverUrl = _user.server_Info.WebSvIp;
		string lastUrl = "/InfoCenterSend?UserID=";
		string userID = _user.user_Users.UserID.ToString();


		//Application.OpenURL(string.Format("{0}{1}{2}",serverUrl,lastUrl,userID));
		string viewUrl = string.Format("{0}{1}{2}", serverUrl, lastUrl, userID);
		//#if UNITY_EDITOR
		//Application.OpenURL(viewUrl);
		//#elif UNITY_ANDROID
		//AndroidPluginManager.Getsingleton.StartWebView(viewUrl, Screen.width, Screen.height, "btn_close", 80, 80, false, "");
		//#elif UNITY_IOS
		//IosPluginManager.Getsingleton.StartWebView (viewUrl);
		//#endif
	}



#endregion



#region 계정로그인 관련
	//========================= 계정관련 =========================

	public void ResponseButton_LogOut()
	{
		
		//UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
		//popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[45]); // 로그아웃
		//popup.SetPopupMessage(TextDataManager.Dic_TranslateText[219]); // 로그아웃 하겠습니까?
		//popup.Set_addEventButton(Process_LogOut);
	
	}

	//로그아웃 처리
	void Process_LogOut()
	{

		//로그아웃하므로 변수 값 변경
		UserDataManager.instance.user.LogInState = Login_State.LogOut;

		//기존 연동된 계정 로그아웃
		PlatformManager.Getsingleton.Platform_LogOut();

		//채팅서버 연결 끊기
		Network_MainMenuSoketManager.Getsingleton.Disconnect(DISCONNECT_STATE.NORMALITY, "로그아웃 의한 서버끊기");

		//webReseponse 변수값 초기화
		webResponse.Init();
	

		//모든유저정보 초기화
		UserDataManager.instance.user.Init();

		
		UI_Manager ui_manager = UI_Manager.Getsingleton;
		ui_manager.CreatUI(UI.TITLE, ui_manager.CanvasTr);
	}

	public void ResponseButton_Link_GoogleAccount()
	{
		//로그아웃하므로 변수 값 변경
		//UserDataManager.instance.user.LogInState = Login_State.LogedIn;

		////기존 연동된 계정 로그아웃
		//PlatformManager.Getsingleton.Platform_LogOut();

		//PlatformManager.Getsingleton.nextPorcess = Login_GoogleID;
		//PlatformManager.Getsingleton.Google_SignIn();
	}

	public void Login_GoogleID(bool success)
	{

		if (success) // 구글계정 연결에 성공했다.
		{
			UserEditor.Getsingleton.EditLog(" 구글 계정으로 로그인~");


			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (int)PLATFORM_TYPE.GOOGLE; //구글


			if (_user.user_Users.UserID != 0 )// 유저아이디가 유효하면
			{
				//플랫폼 전환이 완료되면 로그인으로..
				UserEditor.Getsingleton.EditLog(string.Format("계정전환~ userID : {0},plfID :{1}, lgnKey :{2} , Email : {3}"
				, _user.user_Users.UserID, _user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email));

				webRequest.SetMemeberChange(_user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email, callback_Complete_ChangePlatform);
			}
			else
			{

				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 알림
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[220]); // 구글 계정 연결 실패 하였습니다
				Fail_ChangePlatform();
			}

		}
		else
		{

			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[220]);// 구글 계정 연결 실패 하였습니다
			Fail_ChangePlatform();
		}
	}

	public void ResponseButton_Link_GamecenterAccount()
	{
		//로그아웃하므로 변수 값 변경
		//UserDataManager.instance.user.LogInState = Login_State.LogOut;

		////기존 연동된 계정 로그아웃
		//PlatformManager.Getsingleton.Platform_LogOut();


		//PlatformManager.Getsingleton.nextPorcess = Login_GameCenter;
		//PlatformManager.Getsingleton.Gamecenter_SignIn();
	}


	public void Login_GameCenter(bool success)
	{
		if (success)
		{
			UserEditor.Getsingleton.EditLog(" 게임센터 계정으로 로그인~");
			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (int)PLATFORM_TYPE.GAMECENTER; //페북

			UserEditor.Getsingleton.EditLog(" user id : " + _user.user_Users.UserID + "PlfID : " +_user.user_Users.PlfID);
			if (_user.user_Users.UserID != 0 )//아이디가 유효하면
			{
				//플랫폼 전환이 완료되면 로그인으로..
				webRequest.SetMemeberChange(_user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email, callback_Complete_ChangePlatform);
			}
			else
			{
				UserEditor.Getsingleton.EditLog(" user id or  plfID not matched");
				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 알림
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[273]);// 게임센터 계정 연결 실패 하였습니다

				Fail_ChangePlatform();
			}
		}
		else
		{
			UserEditor.Getsingleton.EditLog("gamecenter login fail");
			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 알림
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[273]);// 게임센터 계정 연결 실패 하였습니다

			Fail_ChangePlatform();
		}
	}

	public void ResponseButton_Link_FacebookAccount()
	{

		//로그아웃하므로 변수 값 변경
		//UserDataManager.instance.user.LogInState = Login_State.LogedIn;

		////기존 연동된 계정 로그아웃
		//PlatformManager.Getsingleton.Platform_LogOut();

		//PlatformManager.Getsingleton.nextPorcess = Login_Facebook;
		//PlatformManager.Getsingleton.FB_Initialize();
		
	}

	public void Login_Facebook(bool sucess)
	{
		if (sucess) // 페북계정 연결에 성공했다.
		{
			UserEditor.Getsingleton.EditLog(" 페북 계정으로 로그인~");

			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (int)PLATFORM_TYPE.FACEBOOK; //페북
			UserEditor.Getsingleton.EditLog("key :" + _user.user_logins.Lgnkey + "email : " + _user.user_logins.Email);


			if (_user.user_Users.UserID != 0 )//아이디가 유효하면
			{
				//플랫폼 전환이 완료되면 로그인으로..
				UserEditor.Getsingleton.EditLog(string.Format("계정전환~ userID : {0},plfID :{1}, lgnKey :{2} , Email : {3}"
				, _user.user_Users.UserID, _user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email));

				webRequest.SetMemeberChange(_user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email, callback_Complete_ChangePlatform);
			}
			else
			{
				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[221]);//페이스북 계정 연결 실패 하였습니다
				Fail_ChangePlatform();

			}
		}
		else
		{
			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[221]);//페이스북 계정 연결 실패 하였습니다
			Fail_ChangePlatform();
		}
	}

	void callback_Complete_ChangePlatform()
	{
		User _user = UserDataManager.instance.user;

		//플랫폼 인덱스 저장
		PlatformManager.Getsingleton.Save_NowUsePlatformIndex(_user.user_logins.PlfID);

		//플랫폼 버튼 활성/ 비활성
		Set_PlatformButton(_user.user_logins.PlfID);
		// 플랫폼 아이콘 이미지 설정
		Set_PlatformIcon(_user.user_logins.PlfID);

		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		if (_user.user_logins.PlfID == (int)PLATFORM_TYPE.GOOGLE)
		{
			popup.SetPopupMessage(string.Format("{0} {1}", TextDataManager.Dic_TranslateText[223], TextDataManager.Dic_TranslateText[226])); //구글 계정에 연결 하였습니다. 게임을 재실행 해주세요
			
		}
		else if (_user.user_logins.PlfID == (int)PLATFORM_TYPE.FACEBOOK)
		{
			popup.SetPopupMessage(string.Format("{0} {1}", TextDataManager.Dic_TranslateText[224], TextDataManager.Dic_TranslateText[226]));//페이스북 계정에 연결 하였습니다.  게임을 재실행 해주세요
		}
		else if (_user.user_logins.PlfID == (int)PLATFORM_TYPE.GAMECENTER)
		{
			popup.SetPopupMessage(string.Format("{0} {1}", TextDataManager.Dic_TranslateText[225], TextDataManager.Dic_TranslateText[226]));//게임센터 계정에 연결 하였습니다.  게임을 재실행 해주세요
		}

		popup.Set_addEventButton(Application.Quit);
	}

	void Set_PlatformIcon(byte _PfID)
	{
		//플랫폼 이미지
		if (_PfID == (int)PLATFORM_TYPE.GUEST)		//게스트
			image_PlatformIcon.sprite = ImageManager.instance.Get_Sprite(DefineKey.loginicon_guest);
		else if (_PfID == (int)PLATFORM_TYPE.GOOGLE) // 구글
			image_PlatformIcon.sprite = ImageManager.instance.Get_Sprite(DefineKey.loginicon_google);
		else if (_PfID == (int)PLATFORM_TYPE.FACEBOOK) // 페북
			image_PlatformIcon.sprite = ImageManager.instance.Get_Sprite(DefineKey.loginicon_facebook);
		else if (_PfID == (int)PLATFORM_TYPE.GAMECENTER) // 게임센터
			image_PlatformIcon.sprite = ImageManager.instance.Get_Sprite(DefineKey.loginicon_gamecenter);


	}

	void Set_PlatformButton(byte _PfID)
	{
		if (_PfID == (int)PLATFORM_TYPE.NONE || _PfID == (int)PLATFORM_TYPE.GUEST)
		{
#if UNITY_EDITOR
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST -1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE-1].SetActive(true);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK-1].SetActive(true);
#elif UNITY_ANDROID
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST -1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE -1].SetActive(true);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK-1].SetActive(true);
#elif UNITY_IOS
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST -1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE -1].SetActive(false);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK-1].SetActive(true);
#endif
		}
		else if (_PfID == (int)PLATFORM_TYPE.GOOGLE) // 구글
		{
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST - 1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE - 1].SetActive(false);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK - 1].SetActive(true);
		}
		else if (_PfID == (int)PLATFORM_TYPE.FACEBOOK) // 페이스북
		{
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST - 1].SetActive(true); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE - 1].SetActive(false);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK - 1].SetActive(false);
		}
		else if (_PfID == (int)PLATFORM_TYPE.GAMECENTER) // 게임센터
		{
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GUEST - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GOOGLE - 1].SetActive(false);
			Lst_PlaformButtonObj[(int)PLATFORM_TYPE.GAMECENTER - 1].SetActive(false); Lst_PlaformButtonObj[(int)PLATFORM_TYPE.FACEBOOK - 1].SetActive(true);
		}
	
	}


	//계정연동 실패시 user_login값 초기화
	void Fail_ChangePlatform()
	{
		User _user = UserDataManager.instance.user;
		_user.user_logins.Lgnkey = "";
		_user.user_logins.Email = "";
		_user.user_logins.PlfID = (byte)nowUsePlatform; //현재 로그인중인 플랫폼으로
	}
#endregion


	void Active_LstPlatformButton(int idx)
	{
		for(int i = 0 ; i < Lst_PlaformButtonObj.Count; i++) 
		{
			Lst_PlaformButtonObj[i].SetActive(idx == i);
		}
	}


	//쿠폰관련
	public void ResponseButton_Coopon()
	{
		//webRequest.GetCooponReward()
		//User.isSelectedCharacter = !popupCoopon.gameObject.activeSelf;
		//popupCoopon.gameObject.SetActive(!popupCoopon.gameObject.activeSelf);
		UI_Popup_Coupon popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Coupon>(UIPOPUP.POPUPCOUPON);

	}



	public void ResponseButton_Back()
	{
		UI_Manager ui_manager = UI_Manager.Getsingleton;
		ui_manager.CreatUI(UI.LOBBY, ui_manager.CanvasTr);
	}

}
