using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UI_Title : UI_Base
{
	private byte MarketIdx;
	private int Ver;
	public PLATFORM_TYPE selectedPlatform = PLATFORM_TYPE.NONE;
	private bool isCompleteLogin = false;
	public int tryWebCheckServerCount = 0;
	private IEnumerator ct_CheckServer;
	private IEnumerator ct_TitleLog;

	//ui
	public Text text_titleLog;

	private static UI_Title _instance;

	public static UI_Title Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Title)) as UI_Title;
			}
			return _instance;
		}
	}

	public override void set_Close()
	{
		base.set_Close();

		ClearData();
	}

	public override void set_Open()
	{
		base.set_Open();

		//리소스데이터 로드하기 
		Start_ResourcesLoadData();


	}

	public override void set_refresh()
	{
		base.set_refresh();
		User _user = UserDataManager.instance.user;

		if (_user.LogInState == Login_State.LogOut)
		{
			//연결할 계정플랫폼 UI active
			Activate_SelectPlatformLoginPopup();
		}
		else if (_user.LogInState == Login_State.LogSelectServer)
		{
			//서버 선택 팝업 active
			Activate_SelectServerPopup();
		}
		else
		{
			//Start_ServerCheck();
			startRoutine_TryServerCheck();
		}

	}



	//리소스데이터 로드하기 
	void Start_ResourcesLoadData()
	{
		StartCoroutine(routine_wait());
	}

	IEnumerator routine_wait()
	{
		yield return new WaitForSeconds(0.0f);

		//동적으로 필요한 이미지및 리소스 로드하기
		//Loadmanager.instance.nextResourceLoadProcess = Start_ServerCheck; // 다음프로세스는 서버체크~
		Loadmanager.instance.nextResourceLoadProcess = startRoutine_TryServerCheck; // 다음프로세스는 서버체크~
		Loadmanager.instance.loadResourcesData();


		//국가코드 받기시도 
//#if UNITY_EDITOR
//#elif UNITY_ANDROID
//		UserEditor.Getsingleton.EditLog("국가코드 받기시도");
//		AndroidPluginManager.Getsingleton.Call_CountryCodeFunction("plz get code~");
//#endif
	}







	bool isCompleteServerChk = false;
	void startRoutine_TryServerCheck()
	{
		isCompleteServerChk = false;
		isCompleteLogin = false;

		Debug.Log("rt_CheckServer : " + ct_CheckServer);
		if (ct_CheckServer == null)
		{
			ct_CheckServer = tryServerCheck();
			StartCoroutine(ct_CheckServer);
		}
	}

	IEnumerator tryServerCheck()
	{
		Loadmanager.instance.LoadingUI(true);
		Start_ServerCheck();

		while (true)
		{
			if (isCompleteServerChk == true)
			{
				UserEditor.Getsingleton.EditLog("다시 서버체크 완료 끝");
				ct_CheckServer = null;
				break;
			}
			yield return new WaitForSeconds(2f);

			if (isCompleteServerChk == false)
			{
				if (tryWebCheckServerCount >= 5)
				{
					//로딩바 안사라졋으니 사라지게하자
					Loadmanager.instance.LoadingUI(false);

					UserEditor.Getsingleton.EditLog("5번 체크 시도 완료 팝업알림!");
					UI_Popup_Notice popup2 = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
					popup2.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
					popup2.Set_addEventButton(startRoutine_TryServerCheck);
					popup2.SetPopupMessage(TextDataManager.Dic_TranslateText[433]); //네트워크 불안정합니다.
					ct_CheckServer = null;
					tryWebCheckServerCount = 0;
					break;
				}
				else
				{
					UserEditor.Getsingleton.EditLog("다시 서버체크시도 횟수:" + tryWebCheckServerCount);
					//팝업창띄어진거 잇으면 없애기
					UI_Manager.Getsingleton.ClearPopupUI();
					Start_ServerCheck();
				}
			}
			yield return null;
		}
	}





	//서버 체크 시작
	public  void Start_ServerCheck()
	{

		//개발서버로 변경 접속할지 체크
		Chk_DevServerChange();

		//서버 체크 시도 횟수카운트
		tryWebCheckServerCount++;

		int ClientVer = DefineKey.ReleaseVersion();
#if UNITY_EDITOR
		MarketIdx = (int)PLATFORM_TYPE.GAMECENTER - 2;
#elif UNITY_ANDROID
		MarketIdx = (int)PLATFORM_TYPE.GOOGLE - 1; // 1

#elif UNITY_IOS
		MarketIdx = (int)PLATFORM_TYPE.GAMECENTER - 2; // 2
		 
#endif
		OptionSetting setting = OptionSetting.instance;

		Write_TitleLog("Try Request CheckServer...");
		if (isCompleteServerChk == false)
			webRequest.CheckServer(MarketIdx, ClientVer, (byte)setting.usingLangueage, CheckServerInfo);
	}

	//개발서버로 변경 접속할지 체크
	void Chk_DevServerChange()
	{
		if (Input.touchCount == 4)
		{
			byte _chkFlg = 0;
			for (int i = 0; i < Input.touchCount; i++)
			{
				Vector2 _touchPos = Input.GetTouch(i).position;
				if (_touchPos.x < Screen.width / 4 && _touchPos.y < Screen.height / 4) _chkFlg |= 1;
				else if (_touchPos.x > Screen.width - (Screen.width / 4) && _touchPos.y < Screen.height / 4) _chkFlg |= 2;
				else if (_touchPos.x < Screen.width / 4 && _touchPos.y > Screen.height - (Screen.height / 4)) _chkFlg |= 4;
				else if (_touchPos.x > Screen.width - (Screen.width / 4) && _touchPos.y > Screen.height - (Screen.height / 4)) _chkFlg |= 8;
			}

			if ((_chkFlg & 7) == 7) DefineKey.LogKind = ServerKind.Developer;
			if (_chkFlg == 15) UserEditor.Getsingleton.ConnectServer = ServerKind.Developer;
		}
	}


	void CheckServerInfo()
	{
		//코루틴떄메 두번이상 들어오는경우가 발생 이미 true 라면 리턴떄리자
		if (isCompleteServerChk == true)
			return;

		isCompleteServerChk = true;
		if (ct_CheckServer != null)
		{
			StopCoroutine(ct_CheckServer);
			ct_CheckServer = null;
		}
		//서버 체크 시도 횟수카운트 초기화
		tryWebCheckServerCount = 0;

		Server_infos server_info = UserDataManager.instance.user.server_Info;

		UserEditor.Getsingleton.EditLog("전달 받은 웹서버 IP :" + server_info.WebSvIp);


#if UNITY_EDITOR
		web_Manager.Getsingleton.URL = string.Format("{0}/", server_info.WebSvIp);

		//web_Manager.Getsingleton.URL = string.Format("{0}/", "http://192.168.0.9:40005");

#else
		web_Manager.Getsingleton.URL = string.Format("{0}/", server_info.WebSvIp);
		//web_Manager.Getsingleton.URL = string.Format("{0}/", "http://192.168.0.9:40005");
#endif

		Write_TitleLog("CheckServer Complete...");

		
		//test
		//server_info.SvStatus = 1;
		if (server_info.SvStatus == 0) //서버상태 : 정상
		{
			//버전체크하기
			Start_CheckVersion();
			UserEditor.Getsingleton.EditLog("서버체크 완료");
		}
		else
		{
			UserEditor.Getsingleton.EditLog("서버체크 비정상 점검중");
			//서버 상태 비정상(점검등)이면 메세지띄움

			//치트 사용 (오른쪽상단에서 왼쪽 하단으로 드래그후 비번입력 (비번 : dyg1018))
			UseCheatWhenChkServer();

			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);

			if (server_info.SvChkTm > DateTime.MinValue)
			{
				TimeSpan span = server_info.SvChkTm - server_info.ServerTime;

				if (span.Days > 0)
				{
					popup.SetPopupMessage(string.Format("{0}\n\n {1} : {2}{3} {4}{5}", server_info.SvChkMsg, TextDataManager.Dic_TranslateText[176],
						span.Days, TextDataManager.Dic_TranslateText[181], span.Hours, TextDataManager.Dic_TranslateText[180]));
				}
				else if (span.Hours > 0)
				{
					popup.SetPopupMessage(string.Format("{0}\n\n {1} : {2}{3} {4}{5}", server_info.SvChkMsg, TextDataManager.Dic_TranslateText[176],
						span.Hours, TextDataManager.Dic_TranslateText[180], span.Minutes, TextDataManager.Dic_TranslateText[179]));
				}
				else if (span.Minutes > 0)
				{
					popup.SetPopupMessage(string.Format("{0}\n\n {1} : {2}{3}", server_info.SvChkMsg, TextDataManager.Dic_TranslateText[176]
						, span.Minutes, TextDataManager.Dic_TranslateText[179]));
				}
				else
				{
					popup.SetPopupMessage(string.Format("{0}\n\n {1} : 1{2}", server_info.SvChkMsg, TextDataManager.Dic_TranslateText[176], TextDataManager.Dic_TranslateText[179]));
				}

			}
			else
			{
				popup.SetPopupMessage(string.Format("{0}", server_info.SvChkMsg));
			}


			popup.Set_addEventButton(() => Application.Quit());


		}
	}





	//버전 체크 시작
	public void Start_CheckVersion()
	{
		Write_TitleLog("Try Request CheckVersion...");
		webRequest.GetVersion(MarketIdx, GetVersion);
	}

	void GetVersion()
	{

		// 로그인에 필요한 버전 값 전달
		Ver = DefineKey.ReleaseVersion();
		int UpdateVersion = 99;
		if (UserDataManager.instance.user.Market_versions.ContainsKey(DefineKey.Ver))
			UpdateVersion = Convert.ToInt32(UserDataManager.instance.user.Market_versions[DefineKey.Ver]);

		Write_TitleLog("Complete CheckVersion...");
		UserEditor.Getsingleton.EditLog("현재 어플 버젼 : " + Ver + " , 업데이트 버젼 : " + UpdateVersion);

		// 지금은 이렇지만, 클라버전 < 서버버전 이면 업데이트으로 수정해야함 버전은 int 형으로
		if (Ver < UpdateVersion)
		{
			//서버 상태 비정상(점검등)이면 메세지띄움
			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
			popup.SetPopupMessage(string.Format("업데이트가 필요합니다. 확인 누르면 마켓으로 이동합니다")); // 업데이트가 필요합니다. 확인 누르면 마켓으로 이동합니다7
			popup.Set_addEventButton(() => Application.OpenURL(UserDataManager.instance.user.Market_versions[DefineKey.MktUrl]));

		}
		else
		{
			UserEditor.Getsingleton.EditLog("어플 버젼 맞음");


			//그리고 로그인하러~~
			Init_Login();


		}
	}

	public void Init_Login()
	{
		User _user = UserDataManager.instance.user;

		if (_user.LogInState == Login_State.LogOut)
		{
			//연결할 계정플랫폼 UI active
			Activate_SelectPlatformLoginPopup();
		}
		else
		{
			//임시 로그인
			//Start_login();

			selectedPlatform = (PLATFORM_TYPE)PlatformManager.Getsingleton.ChkGet_BeforeUsePlatformIndex();

			//플랫폼별 로그인 시작
			if (selectedPlatform == PLATFORM_TYPE.NONE)
			{
				//연결할 계정플랫폼 UI active
				Activate_SelectPlatformLoginPopup();

			}
			else if (selectedPlatform == PLATFORM_TYPE.GUEST)// 게스트
			{
				Try_Login_GuestID();
			}
			else if (selectedPlatform == PLATFORM_TYPE.GOOGLE)//구글
			{
				Try_Login_Google();
			}
			else if (selectedPlatform == PLATFORM_TYPE.FACEBOOK)// 페이스북
			{
				Try_Login_Facebook();
			}
			else if (selectedPlatform == PLATFORM_TYPE.GAMECENTER)// 게임센터
			{
				Try_Login_GameCenter();
			}
		}
	}



	public void Try_Login_GuestID()
	{
		PlatformManager.Getsingleton.nextPorcess = Login_guestID;
		PlatformManager.Getsingleton.Guest_Signin();
	}

	public void Login_guestID(bool sucess)
	{
		Write_TitleLog("Login guest account...");
		UserEditor.Getsingleton.EditLog("로그인 게스트 계정");

		selectedPlatform = PLATFORM_TYPE.GUEST;

		User _user = UserDataManager.instance.user;
		_user.user_Users.Init(); //

		//유저로그인정보에 플랫폼인덱스 저장
#if UNITY_EDITOR

		_user.user_logins.PlfID = (byte)UserEditor.Getsingleton.PlfID;
#else
		_user.user_logins.PlfID = (byte)selectedPlatform;
#endif
		//로그인시도
		Start_login();


	}

	public void Try_Login_GameCenter()
	{
		PlatformManager.Getsingleton.nextPorcess = Login_GamecenterID;
		PlatformManager.Getsingleton.Gamecenter_SignIn();
	}


	//페북계정연동시도 완료시 콜백
	public void Login_GamecenterID(bool success)
	{
		if (success) // 구글계정 연결에 성공했다.
		{
			Write_TitleLog("Login gamecenter account...");
			UserEditor.Getsingleton.EditLog(" 게임센터 계정으로 로그인~");

			selectedPlatform = PLATFORM_TYPE.GAMECENTER;

			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (byte)selectedPlatform; //페북


			if (_user.user_Users.UserID != 0 && _user.user_logins.PlfID != (byte)PLATFORM_TYPE.GAMECENTER)// 로그인 했엇다. 플랫폼 전환을 한다.
			{
				//플랫폼 전환이 완료되면 로그인으로..
				ChangePlatform();
			}
			else
				//로그인시도
				Start_login();
		}
		else
		{

			selectedPlatform = PLATFORM_TYPE.NONE;
			PlatformManager.Getsingleton.Save_NowUsePlatformIndex((int)selectedPlatform);

			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[273]);// 게임센터 계정 연결 실패 하였습니다
			//다시 계정띄우는곳으로 
			popup.Set_addEventButton(Init_Login);
		}
	}




	public void Try_Login_Facebook()
	{
		PlatformManager.Getsingleton.nextPorcess = Login_FacebookID;
		PlatformManager.Getsingleton.FB_Initialize();
	}

	//페북계정연동시도 완료시 콜백
	public void Login_FacebookID(bool success)
	{
		if (success) // 구글계정 연결에 성공했다.
		{
			Write_TitleLog("Login facebook account...");
			UserEditor.Getsingleton.EditLog(" 페북 계정으로 로그인~");

			selectedPlatform = PLATFORM_TYPE.FACEBOOK;

			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (byte)selectedPlatform; //페북


			if (_user.user_Users.UserID != 0 && _user.user_logins.PlfID != (byte)PLATFORM_TYPE.FACEBOOK)// 로그인 했엇다. 플랫폼 전환을 한다.
			{
				//플랫폼 전환이 완료되면 로그인으로..
				ChangePlatform();
			}
			else
				//로그인시도
				Start_login();
		}
		else
		{

			selectedPlatform = PLATFORM_TYPE.NONE;
			PlatformManager.Getsingleton.Save_NowUsePlatformIndex((int)selectedPlatform);

			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[221]);// 구글 계정 연결 실패 하였습니다
			//다시 계정띄우는곳으로 
			popup.Set_addEventButton(Init_Login);
		}
	}




	public void Try_Login_Google()
	{
		PlatformManager.Getsingleton.nextPorcess = Login_GoogleID;
		PlatformManager.Getsingleton.Google_SignIn();
	}

	//구글계정연동시도  완료시 콜백
	public void Login_GoogleID(bool success)
	{
		if (success) // 구글계정 연결에 성공했다.
		{
			Write_TitleLog("Login google account...");
			UserEditor.Getsingleton.EditLog(" 구글 계정으로 로그인~");

			selectedPlatform = PLATFORM_TYPE.GOOGLE;

			User _user = UserDataManager.instance.user;
			_user.user_logins.Lgnkey = PlatformManager.Getsingleton.Platform_UserID;
			_user.user_logins.Email = PlatformManager.Getsingleton.Platform_UserEmail;
			_user.user_logins.PlfID = (byte)selectedPlatform; //구글


			if (_user.user_Users.UserID != 0 && _user.user_logins.PlfID != (byte)PLATFORM_TYPE.GOOGLE)// 로그인 했엇다. 플랫폼 전환을 한다.
			{
				//플랫폼 전환이 완료되면 로그인으로..
				ChangePlatform();
			}
			else
				//로그인시도
				Start_login();
		}
		else
		{

			selectedPlatform = PLATFORM_TYPE.NONE;
			PlatformManager.Getsingleton.Save_NowUsePlatformIndex((int)selectedPlatform);

			UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
			popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
			popup.SetPopupMessage(TextDataManager.Dic_TranslateText[220]);// 구글 계정 연결 실패 하였습니다
			//다시 계정띄우는곳으로 
			popup.Set_addEventButton(Init_Login);
		}
	}


















	//플랫폼 전환 프로토콜 호출
	public void ChangePlatform()
	{
		User _user = UserDataManager.instance.user;

		Write_TitleLog("Try ChangePlatform...");
		webRequest.SetMemeberChange(_user.user_logins.PlfID, _user.user_logins.Lgnkey, _user.user_logins.Email, Start_login);
	}




	// 로그인 시작
	public void Start_login()
	{



		//로그인 프로세스~
		Login_Process();


		//로그인 하는동안 기달렷다가 로그인완료되면 완료프로세스 진행하자
		StartCoroutine(Complete_Login());
	}




	// 로그인완료시 완료프로세스
	IEnumerator Complete_Login()
	{
		while (true)
		{
			if (web_Manager.Getsingleton.isNetworking == false && isCompleteLogin && web_Manager.Getsingleton.Que_NetData.Count <= 0)
			{
				isCompleteLogin = false;
				Write_TitleLog("Complete Login processing");

				int serverCnt = UserDataManager.instance.user.User_GameServerInfos.Count;
                //서버가 여러개면 서버선택
                if (serverCnt > 1)
                {
					Activate_SelectServerPopup();
				}
                else //서버 하나라면 바로 로비이동
                {
                    UserDataManager.instance.user.Set_useGameServer(1);


                    GotoLobby();
                }

                break;
			}

			yield return null;
		}
	}




	//로그인 프로토콜 쏘기
	public void Login_Process()
	{
		User _user = UserDataManager.instance.user;
		OptionSetting optionsetting = OptionSetting.instance;

		string pushToken = "";



		string CtCode = string.Empty;

		CtCode = StaticMethod.Get_CountryCodeStringData();

		Write_TitleLog("Try Requset SetMemberLogin...");
		UserEditor.Getsingleton.EditLog("PlfID : " + _user.user_logins.PlfID + " _logkey : "
	+ _user.user_logins.Lgnkey + "Ver : " + Ver + " marketIdx : " + MarketIdx + " Lggcd : " + (byte)optionsetting.usingLangueage + " 로그인시 국가코드 : " + CtCode);

		webRequest.SetMemberLogin(_user.user_logins.PlfID, _user.user_logins.Lgnkey, Ver, MarketIdx, (byte)optionsetting.usingLangueage, CtCode, pushToken, callback_LoginProcess);
	}


	//안드로이드 서버인증후 로그인 프로토콜
	public void Login_AppguradingProcess()
	{
		User _user = UserDataManager.instance.user;
		OptionSetting optionsetting = OptionSetting.instance;



		string CtCode = string.Empty;

		CtCode = StaticMethod.Get_CountryCodeStringData();

		Write_TitleLog("Try Requset GetAuthentication...");
		UserEditor.Getsingleton.EditLog("Login_AppguradingProcess    PlfID : " + _user.user_logins.PlfID + " _logkey : "
	+ _user.user_logins.Lgnkey + "Ver : " + Ver + " marketIdx : " + MarketIdx + " Lggcd : " + (byte)optionsetting.usingLangueage + " 로그인시 국가코드 : " + CtCode);

		//webRequest.SetMemberLogin(_user.user_logins.PlfID, _user.user_logins.Lgnkey, Ver, MarketIdx, (byte)optionsetting.usingLangueage, CtCode, pushToken, Get_necessaryInfo);

		webRequest.GetAuthentication(Ver, _user.user_logins.PlfID, _user.user_logins.Lgnkey, Get_necessaryInfo);
	}




	//로그인 프로세스
	void callback_LoginProcess()
	{

		Dictionary<string, object> _dicData = webResponse.Dic_data;


		if (_dicData.Count == 0)
		{
			Write_TitleLog("Try to regist new user");
			// 회원가입으로..
			Register_Process();

			
#if UNITY_EDITOR
			UserEditor.Getsingleton.EditLog("regist new account");
#endif

		}
		else
		{
#if UNITY_EDITOR
			Get_necessaryInfo();
#elif UNITY_ANDROID
			if (UserEditor.Getsingleton.ConnectServer == ServerKind.Live)
			{ // 라이브 이면 서버인증후 로그인처리
				User _user = UserDataManager.instance.user;
				if (UserEditor.Getsingleton.isUseAppgurdLogin)
				{
					if (!string.IsNullOrEmpty(_user.AuthenticationKEY))
						AndroidPluginManager.Getsingleton.callSetAuthServer(_user.AuthenticationKEY);
				}
				else
				{
					//라이브지만 앱가드로그인 사용안함
					Get_necessaryInfo();
				}
			}
			else if (UserEditor.Getsingleton.ConnectServer == ServerKind.Developer)
			{// 개발 이면 그냥 로그인처리
				Get_necessaryInfo();
			}
#elif UNITY_IOS
			Get_necessaryInfo();
#endif
		}
	}



	//로그인 완료~ 필요정보 받기
	void Get_necessaryInfo()
	{
		User _user = UserDataManager.instance.user;


		//로그인 완료됫당
		isCompleteLogin = true;

		//플랫폼매니저에 현재플랫폼인덱스 저장 ===> 완전히 로그인이 완료 될때  플랫폼 저장하자 
		PlatformManager.Getsingleton.Save_NowUsePlatformIndex((int)selectedPlatform);


		//로그인후 구성정보
		webRequest.GetReferenceDB(webRequest.newIdxLst(22), callback_Log);
		callback_Log();

		//구성정보후 유저정보
		webRequest.GetUserInfos(_user.user_Users.UserID, callback_Log);

		//클랜정보 불러오기
		webRequest.ClanInfo(callback_Log);

		//친구정보 불러오기
		webRequest.FriendList(callback_Log);

		//서버리스트 불러오기
		string CtCode = StaticMethod.Get_OrinCountryCodeString();
		webRequest.GetServerList(CtCode,callback_Log);

		//유저정보후 서버시간
		webRequest.GetServerTime(callback_Log);




	}

	void callback_Log()
	{
		Write_TitleLog(web_Manager.Getsingleton.webWorkingLog);
	}



	//계정 아이디 생성 프로토콜쏘기
	public void Register_Process()
	{
		//UI_Popup_MakeName popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_MakeName>(UIPOPUP.POPUPMAKENAME);
		//popup.Set_Info(MAKENAMEPOPUP_TYPE.REGISTERUSERNAME, 0);
		//popup.Set_AddEventButton(callback_Complete_Register);

		User _user = UserDataManager.instance.user;
		Write_TitleLog("Try Request SetMemeberJoin...");
		webRequest.SetMemeberJoin(_user.user_logins.PlfID, _user.user_logins.Lgnkey, "",
			_user.user_logins.Email, callback_RegisterProcess);
	}



	//계정아이디 생성완료 ~ 다시 로그인
	void callback_RegisterProcess()
	{
		//회원가입완료 햇으니 다시 로그인
		Login_Process();


	}


	//로비로 이동
	void GotoLobby()
	{
		//로그인중으로 
		UserDataManager.instance.user.LogInState = Login_State.LogedIn;

		//인앱결제 초기화 => 상품정보 받아올떄 재컨슘처리도 하기떄문에 userid 받아올 지금 프로세스에 추가한다.
		//InAppPurchaseManager.instance.Init_Payment();

		// 메인메뉴서버 연결하기
		Network_MainMenuSoketManager.Getsingleton.Start_mainMenuServer();


		//유저아이디 분석날리기
		//AnalysisManager.instance.Anl_SetUserID(UserDataManager.instance.user.user_Users.UserID);


		Transform tr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.TOP, tr);
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, tr);
	}


	// 서버 선택 팝업 활성하기
	public void Activate_SelectServerPopup()
	{
		UI_Popup_ServerSelect popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ServerSelect>(UIPOPUP.POPUPSERVERSELECT);
		popup.Set_addEventYESButton(GotoLobby);
	}


	// 플랫폼 로그인 팝업창 활성하기
	public void Activate_SelectPlatformLoginPopup()
	{
		UI_Popup_Platform popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Platform>(UIPOPUP.POPUPPLATFORM);
		popup.Set_Platform();
	}









	public void Write_TitleLog(string logMassage)
	{
		text_titleLog.text = logMassage;
	}




	//타이틀 관련 변수 초기화
	void ClearData()
	{
		//MarketIdx = 0;
		//Ver = 0;
		selectedPlatform = PLATFORM_TYPE.NONE;
		isCompleteLogin = false;
	}






	//서버 점검시 치트로 게임진입하기
	void UseCheatWhenChkServer()
	{
		GameObject orin = Resources.Load("Prefebs/TestSecret") as GameObject;
		GameObject sc = Instantiate(orin);
		sc.transform.SetParent(GameObject.Find("Canvas").transform);
		sc.GetComponent<RectTransform>().sizeDelta = orin.GetComponent<RectTransform>().sizeDelta;
		sc.GetComponent<RectTransform>().anchoredPosition = orin.GetComponent<RectTransform>().anchoredPosition;
		sc.transform.localScale = orin.transform.localScale;
		sc.gameObject.name = "TestSecret";
	}

}
