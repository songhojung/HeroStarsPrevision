using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class web_Manager : MonoBehaviour
{
	public static Dictionary<int, string> ProtocolForder = new Dictionary<int, string>()
	{
		{0, "Info"},
		{1, "Member"},
		{2, "User"},
		{3,"Item"},
		{4,"Game"},
		{5,"Clan"},
		{6,"Clan"},
		{7,"Shop"},
		{8,"Friend"},
		{9,"Ranking"},
		{10,"User"},
	};

	public string URL = "";



	public Queue<netData> Que_NetData = new Queue<netData>();
    private UnityWebRequest www;

	public string webWorkingLog = string.Empty;

	public bool isNetworking = false;
	public bool isWebException = false;
	private float webTryTime = 0f;

	private Coroutine waitCoroutine;

	private static web_Manager _instance;
	public static web_Manager Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.FindObjectOfType(typeof(web_Manager)) as web_Manager;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("web_Manager");
					_instance = instanceObj.AddComponent<web_Manager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}

			return _instance;
		}
	}


	void Awake()
	{
		_instance = this;


	}


    public void Send_WebProtocol()
	{
		if (www != null)
		{
			return;
		}
		else if (Que_NetData.Count == 0)
		{
			isNetworking = false;
			Loadmanager.instance.LoadingUI(isNetworking);
			
			// 3600초 = 1시간동안 아무 프로토콜안쏘면 getservertime 쏘자
			waitCoroutine =StartCoroutine(coroutine_waitNoneBehaviour(3600f));

			UserEditor.Getsingleton.EditLog("networking : " + isNetworking);
			return;
		}


		netData _net = Que_NetData.Dequeue();

		//여기다 쏘는거 만들고 코루틴으로
		//쏘고 응답받고 www = null 시키고  
		StartCoroutine(coroutine_SendProtocol(_net));

		
	}

    IEnumerator coroutine_SendProtocol(netData _pnet)
	{
		string _url = "";
		if (_pnet.protocolName == ProtocolName.CheckServer) // 서버 상태 프로토콜
		{
			_url = DefineKey.CheckServerURL;
		}
		else
		{
			 _url = string.Format("{0}{1}/{2}",
			URL, ProtocolForder[(int)((int)_pnet.protocolName / 10)],
			_pnet.protocolName.ToString());
		}
		www = _pnet.get_WebSend(_url);
		if (!isNetworking)
		{
			isNetworking = true;
			Loadmanager.instance.LoadingUI(isNetworking);

			if (waitCoroutine != null)
			StopCoroutine(waitCoroutine);



			webWorkingLog = string.Format("Try Request data {0}", _pnet.protocolName);
			UserEditor.Getsingleton.EditLog(string.Format("{0} networking : {1}", _pnet.protocolName, isNetworking));
		}
		yield return www.SendWebRequest();

		   try
		   {
#if UNITY_EDITOR
			   UserEditor.Getsingleton.EditLog(string.Format("return : <b><color=#28ff65>{0}</color></b>", www.downloadHandler.text));
#endif
			   string _jsonData = www.downloadHandler.text;

			   object _jsonObject = MiniJSON.Json.Deserialize(_jsonData);

			   Dictionary<string, object> _dicInfo = (Dictionary<string, object>)_jsonObject;

			   //반환데이터 오류에대한 bool 값 초기화
			   webResponse.Resp_Error = false;
			    
			   //갱신 데이터 체크 
			   if (_pnet.protocolName == ProtocolName.CheckServer)
				   webResponse.ResponseWeb_CheckServer(UserDataManager.instance.user,_dicInfo);
			   else if (_pnet.protocolName == ProtocolName.GetServerList)
				   webResponse.ResponseWeb_GetServerList(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.GetVersion)
				   webResponse.ResponseWeb_GetVersion(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.GetServerTime)
				   webResponse.ResponseWeb_serverTime(_dicInfo);
			   else if (_pnet.protocolName == ProtocolName.GetConnectAdress)
				   webResponse.ResponseWeb_GetConnectAdress(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.GetReferenceDB)
				   TableDataManager.instance.Set_tableData(_dicInfo);
			   else if (_pnet.protocolName == ProtocolName.PostGetList)
				   webResponse.ResponseWeb_GetPostList_Data(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.UnitExpRanking)
				   webResponse.ResponseWeb_GetUnitExpRanking_Data(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.ClanList)
				   webResponse.ResponseWebClanList_Data(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.FriendList)
				   webResponse.ResponseWeb_FriendsList_Data(UserDataManager.instance.user, _dicInfo);
			   else if (_pnet.protocolName == ProtocolName.FriendGetInviteUrl)
				   webResponse.ResponseWeb_FriendInviteUrl_Data(UserDataManager.instance.user, _dicInfo, string.Format("{0}{1}/", URL, ProtocolForder[(int)((int)_pnet.protocolName / 10)]));
			   else if (_pnet.protocolName == ProtocolName.SetMemberChange)
				   webResponse.ResponseWeb_MemberChange(_dicInfo);
			   else 
			   {
				   webResponse.Get_ListGainitem(_dicInfo);

				   webWorkingLog = string.Format("complete {0} data", _pnet.protocolName);
#if UNITY_EDITOR
				   UserEditor.Getsingleton.EditLog(webWorkingLog);
#endif
			   }

			   www = null;

			   // 불러서 또 쏠거 있는지 체크하자
			   Send_WebProtocol();


			   //콜백함수가 있다면 콜백함수 부르자
			   if (_pnet.del_result != null && !webResponse.Resp_Error) //매서드가 널이 아니고 에러가 아니면
			   {
				   _pnet.del_result();
			   }

			   if (isWebException)
				   isWebException = false;

			    
		   }
		   catch (Exception e)
		   {
			   UserEditor.Getsingleton.EditLog(e);
			   if (isWebException == false)
			   {
				   isWebException = true;

				   //www 널로
				   www = null;

				   // 로딩바 끄기
				   isNetworking = false;
				   Loadmanager.instance.LoadingUI(isNetworking);

				 
				   Scene _scene = SceneManager.GetActiveScene();
				   if (_scene.name == DefineKey.Main) //메인씬일때만
				   {

					   Set_ProtocalError( _pnet.protocolName);
					   //User _user = UserDataManager.instance.user;
					   //if (_user.user_Users.UserID != 0)
					   //    popup.SetPopupMessage(string.Format("User ID : {0} \n\n {1}", _user.user_Users.UserID, Set_ProtocalError(_pnet.protocolName)));
					   //else
					   //    popup.SetPopupMessage(Set_ProtocalError(popup,_pnet.protocolName));

					   //popup.Set_addEventButton(goto_TitleUI);
				   }
				   else
				   {
					   //팝업
					   UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
					   popup.Set_PopupTitleMessage("오류");
					   popup.SetPopupMessage(string.Format("Network Error,\n Please Restart SuddenGround App \n\n\nError : {0}  Network Error", _pnet.protocolName));
					   popup.Set_addEventButton(Application.Quit);
				   }
			   }
			   else
			   {
				   //www 널로
				   www = null;
				  // exeption_gotoTitleUI();
			   }
			  
		   }
	}

	private void exeption_gotoTitleUI()
	{

		// 재 로그인을 위한 초기화
		UserEditor.Getsingleton.EditLog("웹오류로 타이틀 호출");

		//Reset_Process();
		//webReseponse 변수값 초기화
		webResponse.Init();
		//모든유저정보 초기화
		UserDataManager.instance.user.Init();

		
		//UI_Manager.Getsingleton.CreatUI(UI.TITLE, UI_Manager.Getsingleton.CanvasTr);
		UI_Title.Getsingleton.set_refresh();
	}

	private void exeption_Stay()
	{
		isWebException = false;
	}


	public void Reset_Process()
	{
		

		//webReseponse 변수값 초기화
		webResponse.Init();

		//채팅서버 연결 끊기
		Network_MainMenuSoketManager.Getsingleton.Disconnect(DISCONNECT_STATE.NORMALITY, "에러에의한 서버끊기");

		// UI 다없애기
		UI_Manager.Getsingleton.Init();

		//모든유저정보 초기화
		UserDataManager.instance.user.Init();
		
	}

	//홈키 누르고 돌아올떄 
	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			// 에디터에서 퀵조인 할때 서버정보 받아올때 onApplicationPause가 호출이 된다. 이유 파악못함 -> 호출후 프로토콜오류발생도 파악못함
			if (!UserEditor.Getsingleton.isQuickLoginForEditor)
			{
				
				if (!string.IsNullOrEmpty(URL) && InAppPurchaseManager.instance.isPurchasing == false)
					webRequest.GetServerTime(null);
			}
		}
	}




	//아무행동 없을떄 getserverTime 프로토콜 쏘기
	IEnumerator coroutine_waitNoneBehaviour(float time)
	{
		yield return new WaitForSeconds(time);
		webRequest.GetServerTime(null);
		
	}


	private string Set_ProtocalError(ProtocolName ptName)
	{
		//팝업

		string errorMsg = string.Empty;
		string msg2 = TextDataManager.Dic_TranslateText[433];//"네트워크가 불안정 합니다.";
		string msg1 = "네트워크 오류 발생하엿습니다. \n다시 시도해주세요";




		switch (ptName)
		{

			case ProtocolName.CheckServer:
			case ProtocolName.GetVersion:
			case ProtocolName.SetMemberLogin:
			case ProtocolName.SetMemberJoin:
			case ProtocolName.SetMemberChange:
			case ProtocolName.GetAuthentication:
			case ProtocolName.GetReferenceDB:
			case ProtocolName.GetUserInfos:
			case ProtocolName.ClanInfo:
			case ProtocolName.FriendList:
			case ProtocolName.GetServerList:
			case ProtocolName.GetServerTime:
				UI_Popup_Notice popup3 = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup3.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
				errorMsg = string.Format("{1}\n {0}", ptName, msg1);
				popup3.SetPopupMessage(errorMsg);
				if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TITLE))
				{
					popup3.Set_addEventButton(exeption_gotoTitleUI);
				}
				else
				{
					popup3.Set_addEventButton(exeption_Stay);
				}
				break;



			default:
				UI_Popup_Notice popup2 = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup2.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
				errorMsg = string.Format("{1}\n {0}", ptName, msg1);
				popup2.Set_addEventButton(exeption_Stay);
				popup2.SetPopupMessage(errorMsg);
				break;
		}

	
		

		return errorMsg;
	}

	void chk_serverTimeOutCount()
	{
		UserEditor.Getsingleton.EditLog("CheckServer 프로토콜 재시도 시작");
		StartCoroutine(routine_TimeCount(webTryTime, 20f));

	}

	IEnumerator routine_TimeCount(float time, float maxTime)
	{
		float limitTime = time;
		float countTime = 0;
		Loadmanager.instance.LoadingUI(true);
		while(true)
		{

			limitTime += Time.deltaTime;
			countTime += Time.deltaTime;
			if (limitTime >= maxTime)
			{

				if (UI_Title.Getsingleton.tryWebCheckServerCount != 0)
				{
					Loadmanager.instance.LoadingUI(false);

					UserEditor.Getsingleton.EditLog("CheckServer 완료안함 팝업때리자");
					//UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
					//popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
					//Debug.Log("CheckServer----------------- ");
					//string errorMsg = string.Format("{0}\n {1}", TextDataManager.Dic_TranslateText[433], TextDataManager.Dic_TranslateText[434]);
					//popup.Set_addEventButton(exeption_gotoTitleUI);
					//popup.SetPopupMessage(errorMsg);

					Set_ProtocalError(ProtocolName.CheckServer);

					time = 0f;
					break;
				}
				else
				{
					Loadmanager.instance.LoadingUI(false);
					UserEditor.Getsingleton.EditLog("CheckServer 완료햇네");
					time = 0f;
					break;
				}
			}
			else if (countTime >= 3)
			{
				countTime = 0;
				Loadmanager.instance.LoadingUI(true);

				if (UI_Title.Getsingleton.tryWebCheckServerCount != 0)
				{
					UserEditor.Getsingleton.EditLog("CheckServer 완료안함 다시 시도!");
					UI_Title.Getsingleton.Start_ServerCheck();
				}
				else
				{
					Loadmanager.instance.LoadingUI(false);
					UserEditor.Getsingleton.EditLog("CheckServer 완료햇네");
					time = 0f;
					break;
				}
			}
			yield return null;
		}
	}
}

