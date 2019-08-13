using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Text;


public class Network_MainMenuSoketManager : MonoBehaviour
{

	private NetWork_Script Net_Script;

	public MMSERVER_STATE ConnectLogin_State = MMSERVER_STATE.IDEL;		//로그인연결 상태
	public MMSERVER_STATE Operation_State = MMSERVER_STATE.IDEL;			//작동 상태
	private DISCONNECT_STATE DisConnect_State = DISCONNECT_STATE.IDLE;		//연결 끊김 상태
	private float connecting_Time = 0f;								//연결 지연되고있는 타임값
	private int connectingTry_Count = 0;							// 연결 시도 횟수
	private float tryQuickJoin_Time = 0f;
	private NETKIND Recieved_NetKind;

	private ByteData SendBuffer;
	private ByteData RecieveBuffer;

	//퀵조인시 필요한정보
	public byte MapIdx = 0;
	public bool isCompleteQuickjoin = false;


	//핑체크 관련정보
	private DateTime startPingTime;
	private DateTime endPingTime;
	private TimeSpan resultPing;

	//리텐션코루틴
	private List<Coroutine> Lst_routineRetention = new List<Coroutine>();
	private IEnumerator ct_Retention;

	private static Network_MainMenuSoketManager _instance;
	public static Network_MainMenuSoketManager Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(Network_MainMenuSoketManager)) as Network_MainMenuSoketManager;
				if (_instance == null)
				{
					_instance = new GameObject("Network_MainMenuSoketManager").AddComponent<Network_MainMenuSoketManager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}



	void Awake()
	{
		Net_Script = new NetWork_Script();
		Net_Script.Delegate_Init(ReciveData_Delegate, ErrorData_Delegate);

		UserEditor.Getsingleton.EditLog("메인메뉴소켓서버 초기화완료 ");
	}

	// 소켓 연결중인지 체크
	public bool IsConnect()
	{
		if (Net_Script.IsConnected)
			return true;
		else
			return false;
	}

	bool TimeCount(ref float _Time, float _Max_Time)
	{
		_Time += Time.deltaTime;
		if (_Time >= _Max_Time)
		{
			_Time = 0.0f;

			return true;
		}

		return false;
	}

	public void Start_mainMenuServer()
	{

		ConnectLogin_State = MMSERVER_STATE.CONNECT_START;
	}

	public void mainMenuServerConnect()
	{
		//채팅 IP 및 포트 값 전달 
		GameServer_Info serverInfo = UserDataManager.instance.user.User_userGameServer;
		string ChattingServer_IP = string.Empty;
		ushort Server_Port = 0;
#if UNITY_EDITOR

		ChattingServer_IP = serverInfo.PubIp;
		Server_Port = serverInfo.CnPort;
#else
		ChattingServer_IP = serverInfo.PubIp;
		Server_Port = serverInfo.CnPort;
#endif

		Net_Script.Connect_Start(ChattingServer_IP, Server_Port);
		UserEditor.Getsingleton.EditLog("MainMenuServer is complete to connect , IP : " + ChattingServer_IP + " /Port: " + Server_Port);
	}

	void Update()
	{

		//소켓  작동 행동
		Behaviour_Operation();

		//서버 연결및 로그인 작동
		ConnectLoginServer_Operation();

		//리시브 받은 데이터들 처리 
		Chk_RecieveProcess();

		
	}


	//소켓  작동 행동
	void Behaviour_Operation()
	{
		switch (Operation_State)
		{
			case MMSERVER_STATE.IDEL:
				break;


			case MMSERVER_STATE.TRY_QUICKJOIN:

				isCompleteQuickjoin = false;
				StartCoroutine(RecieceComplete_Try_Quickjoin());
				Operation_State = MMSERVER_STATE.IDEL;
				break;

			case MMSERVER_STATE.COMPLETE_QUICKJOIN:
				tryQuickJoin_Time = 0;
				isCompleteQuickjoin = true;
				Operation_State = MMSERVER_STATE.IDEL;
				RecieveComplete_QuickJoin();
				break;

			case MMSERVER_STATE.ERROR:
				RecieveError_Process();
				break;
			case MMSERVER_STATE.ERROR_END:
				RecieveError_EndProcess();
				break;
		}
	}

	void ConnectLoginServer_Operation()
	{
		switch (ConnectLogin_State)
		{
			case MMSERVER_STATE.IDEL:
				break;
			case MMSERVER_STATE.CONNECT_FAIL:
				break;
			case MMSERVER_STATE.RECONNECT_START:
				//테스트2초용
				//if (TimeCount(ref connecting_Time, 2.0f))
				{
					Start_mainMenuServer();
					//ConnectLogin_State = MMSERVER_STATE.IDEL;
					connecting_Time = 0f;
				}
				break;
			case MMSERVER_STATE.CONNECT_START:
				mainMenuServerConnect();
				ConnectLogin_State = MMSERVER_STATE.CONNECTING;
				UserEditor.Getsingleton.EditLog("네트웍  CONNECT_START");

				break;
			case MMSERVER_STATE.CONNECTING:
				if (Net_Script.IsConnected)
				{
					UserEditor.Getsingleton.EditLog("네트웍 접속 완료");

					//네트웍 이상없으니 DIsconnect state 를 IDLE로
					DisConnect_State = DISCONNECT_STATE.IDLE;
 
					connecting_Time = 0f;
					ConnectLogin_State = MMSERVER_STATE.LOGIN_START;
				}
				else
				{
					//연결시도 체크
					if (connectingTry_Count > 1)
					{
						//재연결시도 실패 해서 그냥 타이틀로 돌려보냄
						ConnetingRryFailProcess();
					}
					else
					{
						if (TimeCount(ref connecting_Time, 10.0f))
						{
							UserEditor.Getsingleton.EditLog("네트웍 접속 시도 : " + connectingTry_Count + 1);


							UserEditor.Getsingleton.EditLog("일정 시간 접속 불가상태여서 끊어버린다");

							Disconnect(DISCONNECT_STATE.NORMALITY);

							ConnectLogin_State = MMSERVER_STATE.CONNECT_START;
							connectingTry_Count++;
						}
					}
				}
				break;
			case MMSERVER_STATE.LOGIN_START:
				//로그인위해 서버로 로그인정보보내기
				Send_CTS_Login();
				ConnectLogin_State = MMSERVER_STATE.IDEL;

				break;
			case MMSERVER_STATE.LOGIN_OK:
				ConnectLogin_State = MMSERVER_STATE.LOGIN_COMPLETE;
				//로그인됫으니 , 리텐션 시작

				if (ct_Retention == null)
				{
					ct_Retention = Retention_Checking();
					StartCoroutine(ct_Retention);
				}
				else
				{
					StopCoroutine(ct_Retention);
					ct_Retention = null;
					ct_Retention = Retention_Checking();
					StartCoroutine(ct_Retention);
				}
				UserEditor.Getsingleton.EditLog("리텐션 시작");
				break;
			case MMSERVER_STATE.LOGIN_COMPLETE:
				connectingTry_Count = 0;
				break;
		}
	}



	Queue<ByteData> Que_Bytedata = new Queue<ByteData>();
	readonly object Lockobject = new object();

	bool Chk_readBuffer() { lock (Lockobject) { return Que_Bytedata.Count > 0; } }
	ByteData Read_ByteBuffer() { lock (Lockobject) { return Que_Bytedata.Dequeue(); } }

	public void ReciveData_Delegate(byte[] packet_Data)
	{

		RecieveBuffer = new ByteData(packet_Data);
		Recieved_NetKind = (NETKIND)RecieveBuffer.Getbyte();

		if (IsOwnRecieveNetKind(Recieved_NetKind))
		{
			lock (Lockobject)
			{
				RecieveBuffer.DataIndex--;
				Que_Bytedata.Enqueue(RecieveBuffer);
			}
		}
		else if (Recieved_NetKind == NETKIND.STC_CONNECTION_RETENTION)
		{
			lock (Lockobject)
			{
				RecieveBuffer.DataIndex--;
				Que_Bytedata.Enqueue(RecieveBuffer);
			}

			Link_Script.i.GamePlay_Receive_Data(packet_Data);
		}
		else
			//나머지는 배틀 쪽 KIND다 그쪽으로 보내서 처리토록하자
			Link_Script.i.GamePlay_Receive_Data(packet_Data);

	}

	void Chk_RecieveProcess()
	{
		while (Chk_readBuffer() == true)
		{
			RecieveBuffer = Read_ByteBuffer();
			Recieved_NetKind = (NETKIND)RecieveBuffer.Getbyte();

			switch (Recieved_NetKind)
			{
				case NETKIND.NONE:
					break;
				case NETKIND.STC_LOGIN:
					Recieve_STC_Login();
					break;
				case NETKIND.STC_CONNECTION_RETENTION:
					Recieve_STC_Retention();
					break;
				case NETKIND.STC_ROOM_MAKE:
					Recieve_STC_RoomMake();
					break;
				case NETKIND.STC_OPEN_ROOM:
					Recieve_STC_OpenRoom(RecieveBuffer);
					break;
				case NETKIND.STC_INVITE_ROOM:
					Recieve_STC_InviteRoom(RecieveBuffer);
					break;
				case NETKIND.STC_READYROOM_INFO:
					Recieve_STC_RoomInfo(RecieveBuffer);
					break;
				case NETKIND.STC_FRIEND_ROOM_JOIN:
					Recieve_STC_FriendRoomjoin(RecieveBuffer);
					break;
				case NETKIND.STC_CHAT_MESSAGE:
					Recieve_STC_ChatMessage(RecieveBuffer);
					break;
				case NETKIND.STC_TOAST_MSG:
					Recieve_STC_ToastMsg(RecieveBuffer);
					break;
				case NETKIND.STC_POPUP_MSG:
					Recieve_STC_PopupMsg(RecieveBuffer);
					break;
				case NETKIND.STC_ERROR_CODE:
					Recieve_STC_ERROR(RecieveBuffer);
					break;
			}
		}
	}





	//서버에 리텐션 날리기, 10초마다
	IEnumerator Retention_Checking()
	{
		//핑체크
		startPingTime = DateTime.Now;

		//리텐션 날리자
		Net_Script.Send_Data(NETKIND.CTS_CONNECTION_RETENTION, null);

		yield return new WaitForSeconds(10.0f);
		if (Net_Script.IsConnected && ct_Retention != null)
		{
			ct_Retention = Retention_Checking();
			StartCoroutine(ct_Retention);
		}
		else
			yield return null;
	}


	void ConnetingRryFailProcess()
	{
		UserEditor.Getsingleton.EditLog("2회 연결시도 실패 해서 타이틀 보냄");

		//연결 실패
		ConnectLogin_State = MMSERVER_STATE.CONNECT_FAIL;

		//로딩바
		Loadmanager.instance.LoadingUI(false);

		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
		popup.SetPopupMessage(string.Format("{0} \n {1}", TextDataManager.Dic_TranslateText[433], TextDataManager.Dic_TranslateText[434])); //네트워크가 불안정합니다 , 네트워크 확인해주세요
		popup.Set_addEventButton(callback_connectingfail);

		connectingTry_Count = 0;
	}


	void callback_connectingfail()
	{


		// 재 로그인을 위한 초기화
		web_Manager.Getsingleton.Reset_Process();

		UI_Manager.Getsingleton.CreatUI(UI.TITLE, UI_Manager.Getsingleton.CanvasTr);
	}


	#region Recieve 관련
	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			Related Recieving			================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////
	void Recieve_STC_Login()
	{

		Debug.Log("소켓서버 로그인 완료!");

		ConnectLogin_State = MMSERVER_STATE.LOGIN_OK;
	}

	void Recieve_STC_Retention()
	{
		Scene _scene = SceneManager.GetActiveScene();
		if (_scene.name == DefineKey.Main)
		{
			//핑설정 
			endPingTime = DateTime.Now;
			resultPing = endPingTime - startPingTime;

			UserEditor.Getsingleton.EditLog("리시브 리텐션 \n" + "핑 시간 :" + resultPing.Milliseconds);

			//핑 표시
			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
				UI_Lobby.Getsingleton.Set_Ping(resultPing.Milliseconds);
		}
	}


	void Recieve_STC_RoomMake()
	{
		//이전 방에 만들떄 채팅element 가 남아 있을수 있으니 그걸 삭제해야한다
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CUSTOMROOM))
		{
			UI_CustomRoom.Getsingleton.Clear_Chatelement();
		}

	}


	void Recieve_STC_RoomInfo(ByteData _recieveBuffer)
	{
		User _user = UserDataManager.instance.user;


		// uint : 방장 UserID
		// byte : 맵 인덱스
		// bool : (true : 비공개, false : 공개)
		// byte : 유저수
		//		uint : 방 유저 WebUserID 
		//		byte : 방 슬롯 위치 1~12
		//		string : 닉네임
		//		byte : 팀 (Red:0, Blue:1, Draw:2)
		//		ushort : 클랜 마크
		_user.User_readyRoomInfo.Init();
		_user.User_readyRoomInfo.RoomMsterUserID = _recieveBuffer.Getuint();
		_user.User_readyRoomInfo.MapIndex = _recieveBuffer.Getbyte();
		_user.User_readyRoomInfo.isCloseRoom = _recieveBuffer.Getbool();
		_user.User_readyRoomInfo.PersonCnt = _recieveBuffer.Getbyte();

		byte[] _CtyCd = new byte[2];

		_user.User_RoomUserInfos.Clear();
		for (int i = 0; i < _user.User_readyRoomInfo.PersonCnt; i++)
		{
			User_RoomInfo roomUser = new User_RoomInfo();

			roomUser.roomUserID = _recieveBuffer.Getuint();
			roomUser.roomUserSlot = _recieveBuffer.Getbyte();
			roomUser.roomUserNkNm = _recieveBuffer.Getstring();
			roomUser.roomUserTeam = _recieveBuffer.Getbyte();
			roomUser.roomUserClanMark = _recieveBuffer.Getushort();
			_CtyCd[0] = _recieveBuffer.Getbyte();
			_CtyCd[1] = _recieveBuffer.Getbyte();
			roomUser.roomUserFlag = Encoding.UTF8.GetString(_CtyCd, 0, _CtyCd.Length);

			_user.User_RoomUserInfos[roomUser.roomUserID] = roomUser;
		}


		RecieveComplete_RoomInfo();

		//MainMenuNetwork_State = MMSERVER_STATE.ROOM_INFO;

	}



	void Recieve_STC_FriendRoomjoin(ByteData _recieveBuffer)
	{
		User _user = UserDataManager.instance.user;


		//byte : 참여 실패 사유
		//  0:성공
		//	1:친구가 접속중이지 않음
		//	2:친구가 방에 들어가 있지 않음
		//	3:방 입장 유저수 초과
		//byte : 성공시 방상태값 들어감
		_user.User_RcvJoinTogether.Init();
		_user.User_RcvJoinTogether.JoinResult = _recieveBuffer.Getbyte();
		_user.User_RcvJoinTogether.JoinSuccessState = _recieveBuffer.Getbyte();


		RecieveComplete_FriendJoin();

	}




	void Recieve_STC_OpenRoom(ByteData _recieveBuffer)
	{
		User _user = UserDataManager.instance.user;
		_user.User_readyRoomInfo.isCloseRoom = _recieveBuffer.Getbool();


		// 대기방룸 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CUSTOMROOM))
			UI_CustomRoom.Getsingleton.set_refresh();

	}


	void Recieve_STC_InviteRoom(ByteData _recieveBuffer)
	{
		User _user = UserDataManager.instance.user;
		_user.User_RcvRoomInvite.Init();

		//초대 받기
		//uint : 초대자 UserID
		//string : 초대자 닉네임
		//_user.User_RcvRoomInvite.InviterUserID = _recieveBuffer.Getuint();
		//_user.User_RcvRoomInvite.InviterUserNkNm = _recieveBuffer.Getstring();

	
		User_RoomInfo _Rcvinvite = new User_RoomInfo();
		_Rcvinvite.InviterUserID = _recieveBuffer.Getuint();
		_Rcvinvite.InviterUserNkNm = _recieveBuffer.Getstring();
	
		//초대 정보 담기
		_user.User_LstRcvRoomInvites.Add(_Rcvinvite);
		RecieveComplete_InviteRoom();

		

		//MainMenuNetwork_State = MMSERVER_STATE.ROOM_INVITE;

	}


	void Recieve_STC_ChatMessage(ByteData _recieveBuffer)
	{

		User _user = UserDataManager.instance.user;
		_user.user_RecieveChat.Init();
		//byte : 종류 (1:방 채팅)
		//uint : WebUserID 유저아이디
		//ushort : 클랜 마크
		//string : 닉네임
		//string : 메세지 내용

		_user.user_RecieveChat.msgTp = (ChatMessageType)_recieveBuffer.Getbyte();
		_user.user_RecieveChat.UesrID = _recieveBuffer.Getuint();
		_user.user_RecieveChat.ClanMark = _recieveBuffer.Getushort();
		_user.user_RecieveChat.NkNm = _recieveBuffer.Getstring();
		_user.user_RecieveChat.chatMsg = _recieveBuffer.Getstring();

		ReciveComplete_Chatmessage();

		//MainMenuNetwork_State = MMSERVER_STATE.CHAT_MESSAGE;

	}



	void Recieve_STC_ToastMsg(ByteData _recieveBuffer)
	{
		//string : 공지 메세지

		string toastMsg = _recieveBuffer.Getstring();

		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.GetComponent<RectTransform>().localPosition = new Vector3(0f,150f,0f);
		popup.transform.SetAsLastSibling();
		popup.SetPopupMessage(toastMsg);
	}






	void Recieve_STC_PopupMsg(ByteData _recieveBuffer)
	{
		//string : 공지 메세지
		//bool : true(확인 터치시 어플종료), false(팝업만 띄움)

		string popupMsg = _recieveBuffer.Getstring();
		bool isOut = _recieveBuffer.Getbool();

		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
		popup.transform.SetAsLastSibling();
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
		popup.SetPopupMessage(popupMsg);

		if (isOut)
			popup.Set_addEventButton(Application.Quit);
		
	}












	void Recieve_STC_ERROR(ByteData _recieveBuffer)
	{
		byte Error_Code = 0;
		RecieveBuffer.OutPutVariable(ref Error_Code);

		ErrorCode errorCode = (ErrorCode)Error_Code;
		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
		switch (errorCode)
		{
			case ErrorCode.LOGIN_OVERLAP_MATCH:
			case ErrorCode.LOGIN_OVERLAP_BATTLE:
			case ErrorCode.LOGIN_OVERLAP_CHATTING:
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 알림
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[248]);// 로그인 중복!\n 게임을 종료합니다.
				popup.Set_addEventButton(Application.Quit);
				break;
			case ErrorCode.ROOM_IS_FULL:
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[507]); // 해당 방이 꽉 찼습니다.
				break;
			case ErrorCode.ROOM_IS_NULL:
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[508]); // 해당 방이 존재하지 않습니다."
				break;
			default:
				break;
		}


	}



	#endregion
	#region 리시브 받기 완료시 Fuctions
	public void RecieveComplete_RoomMake()
	{

	}


	public void RecieveComplete_RoomInfo()
	{
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CUSTOMROOM))
			UI_CustomRoom.Getsingleton.set_refresh();
		else
		{
			Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
			UI_Manager.Getsingleton.CreatUI(UI.CUSTOMROOM, _canvasTr);
		}
	}


	public void RecieveComplete_FriendJoin()
	{
		User _user = UserDataManager.instance.user;

		if (_user.User_RcvJoinTogether.JoinResult == 0) // 같이하기 성공
		{
			Debug.Log("같이하기 성공");
			Debug.Log("종류 : " + _user.User_RcvJoinTogether.JoinSuccessState);

			if (_user.User_RcvJoinTogether.JoinSuccessState == 0) // 같이하기 종류 대기방
			{

			}
			else //  같이하기 종류 배틀
			{

			}
		}
		else
		{
			UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			byte failReason = _user.User_RcvJoinTogether.JoinResult;
			switch (failReason)
			{
				case 1: // 친구 접속 안함
					popup.SetPopupMessage(TextDataManager.Dic_TranslateText[295]);//선택한 친구는 접속중이 아닙니다
					break;
				case 2: // 친구가 방에들어가지 않음
					popup.SetPopupMessage(TextDataManager.Dic_TranslateText[296]);//선택한 친구는 배틀중이 아닙니다
					break;
				case 3: // 방입장유저수 초과
					popup.SetPopupMessage(TextDataManager.Dic_TranslateText[297]);//접속 방 인원초과 입니다.
					break;
				default:
					break;
			}
		}
	}




	IEnumerator RecieceComplete_Try_Quickjoin()
	{

		tryQuickJoin_Time = 3f;

		if (!IsConnect())
		{
			//네트워크 소켓 재연결시도
			if (ConnectLogin_State != MMSERVER_STATE.RECONNECT_START)
			{
				UserEditor.Getsingleton.EditLog("퀵조인 : 네트웍끊김, 네트웍재연결시도");
				ConnectLogin_State = MMSERVER_STATE.RECONNECT_START;
			}
		}

		while (true)
		{

			if (ConnectLogin_State == MMSERVER_STATE.CONNECT_FAIL)
			{
				break;
			}
			if (ConnectLogin_State == MMSERVER_STATE.LOGIN_COMPLETE)
			{
				if (!IsConnect())
				{
					UserEditor.Getsingleton.EditLog("퀵조인 : LOGIN_COMPLETE 이지만 네트웍끊김, 네트웍재연결시도");
					StartCoroutine(RecieceComplete_Try_Quickjoin());
					break;
				}

				if (!isCompleteQuickjoin)
				{
					if (TimeCount(ref tryQuickJoin_Time, 3f))
					{
						tryQuickJoin_Time = 0f;
						//퀵조인 요청 다시 보내기
						UserEditor.Getsingleton.EditLog("퀵조인 재요청 , 배틀 종류 : " + UserDataManager.instance.user.user_Games.battleGameKind);
						Link_Script.i.GamePlay_Send_Quick_Join(UserDataManager.instance.user.user_Games.battleGameKind);
					}
				}
				else
				{
					UserEditor.Getsingleton.EditLog("퀵조인 완료");
					break;
				}
			}
			yield return null;
		}
	}



	public void RecieveComplete_QuickJoin()
	{
		Loadmanager.instance.LoadingUI(false);

		//퀵조인시 사용유닛 분석날리기
		User _user = UserDataManager.instance.user;
		uint unitidx = _user.User_useUnit.UnitIdx;

		Dictionary<uint, infos_unit> _dic_unitinfo = TableDataManager.instance.Infos_units;
		Dictionary<uint, Infos_Weapon> _dic_weaponinfo = TableDataManager.instance.Infos_weapons;
		Dictionary<uint, Infos_Deco> _dic_decoInfo = TableDataManager.instance.Infos_Decos;

		//AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,		//인게임 사용
		//	DefineKey.Character, _dic_unitinfo[unitidx].UnitName,							//캐릭이름
		//	DefineKey.UsedCount,														//사용횟수
		//	1);

		//AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,		//인게임 사용
		//	DefineKey.Weapon, _dic_weaponinfo[_user.User_Units[unitidx].MainWpnIdx].WpnName,	//메인무기이름
		//	DefineKey.UsedCount,														//사용횟수
		//	1);


		//AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,			//인게임 사용
		//	DefineKey.subWeapon, _dic_weaponinfo[_user.User_Units[unitidx].SubWpnIdx].WpnName,	//서브무기이름
		//	DefineKey.UsedCount,															//사용횟수
		//	1);

		//if (_dic_decoInfo.ContainsKey((uint)_user.User_Units[unitidx].DecoIdx1))
		//{
		//	AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,		//인게임 사용
		//		DefineKey.Deco, _dic_decoInfo[(uint)_user.User_Units[unitidx].DecoIdx1].DecoName,//데코1이름
		//		DefineKey.UsedCount,														//사용횟수
		//		1);
		//}


		//if (_dic_decoInfo.ContainsKey((uint)_user.User_Units[unitidx].DecoIdx2))
		//{
		//	AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,		//인게임 사용
		//		DefineKey.Deco, _dic_decoInfo[(uint)_user.User_Units[unitidx].DecoIdx2].DecoName,//데코2이름
		//		DefineKey.UsedCount,														//사용횟수
		//		1);
		//}

		//if (_dic_decoInfo.ContainsKey((uint)_user.User_Units[unitidx].DecoIdx3))
		//{
		//	AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.UsedInGameplay,		//인게임 사용
		//		DefineKey.Deco, _dic_decoInfo[(uint)_user.User_Units[unitidx].DecoIdx3].DecoName,//데코2이름
		//		DefineKey.UsedCount,														//사용횟수
		//		1);
		//}


		//씬넘기자
		string _secneName = string.Format("REMAP_{0}", MapIdx);
		SendManager.Instance.Load_Scene(_secneName);
	}

	public void RecieveComplete_InviteRoom()
	{
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
			UI_Lobby.Getsingleton.Show_InviteMessage();

		UI_CustomRoom.Getsingleton.Clear_Chatelement();
	}


	void ReciveComplete_Chatmessage()
	{
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CUSTOMROOM))
			UI_CustomRoom.Getsingleton.RecieveMessage_readyRoom();
		else if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CHAT))
			UI_Chat.Getsingleton.RecieveMessage_BattleChat();
	}

	#endregion






	#region Send 관련

	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			Related Sending 			================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////

	public void NetSendData(NETKIND _Protocol, byte[] _Data)
	{
		Net_Script.Send_Data(_Protocol, _Data);
	}

	// 클라에서 서버로 채팅서버 로그인 정보 보내기
	public void Send_CTS_Login()
	{
		//uint:웹 유저 ID
		//string:닉네임
		//ushort : 버전
		//string : KILL 문구
		//byte[2] : 국가코드
		///ushort : 클랜마크
		///byte : 언어코드
		User _user = UserDataManager.instance.user;

		SendBuffer = new ByteData(512, 0);
		string word = string.Empty;
		byte[] ctCd = new byte[2];

		//유저아이디
		SendBuffer.InPutByte(_user.user_Users.UserID);
		//닉네임
		SendBuffer.InPutByte(_user.user_Users.NkNm);
		//버전
		SendBuffer.InPutByte((ushort)DefineKey.ReleaseVersion());

		//킬문구
		if (_user.User_Words.ContainsKey(USERWORD_TYPE.WRD_KILL))
			word = _user.User_Words[USERWORD_TYPE.WRD_KILL].Words;
		else
			word = "None";
		SendBuffer.InPutByte(word);

		//국가코드
		Debug.Log("소켓서버 로그인 국가코드 :  " + _user.user_Users.CtrCd);
		ctCd = Encoding.UTF8.GetBytes(_user.user_Users.CtrCd);


		SendBuffer.InPutByte(ctCd[0]);
		SendBuffer.InPutByte(ctCd[1]);

		//클랜마크
		if (_user.clan_Clans.ClanID == 0)
			SendBuffer.InPutByte((ushort)0);
		else
			SendBuffer.InPutByte(_user.clan_Clans.ClanMark);

		//언어코드
		byte langCode = (byte)OptionSetting.instance.usingLangueage;
		SendBuffer.InPutByte(langCode);

		Net_Script.Send_Data(NETKIND.CTS_LOGIN, SendBuffer.GetTrimByteData());

	}



	//클라에서 서버로 방만들기 정보보내기
	public void Send_CTS_RoomMake()
	{

		//네트웍끊어졋는지 체크하고 끊겻으면 재연결 이후에 버퍼채우고 방만들기 프로토콜날린다
		StartCoroutine(Chk_RoomMake());
		//Net_Script.Send_Data(NETKIND.CTS_ROOM_MAKE, SendBuffer.GetTrimByteData());
	}


	void SetSendBuffer_roomMake()
	{
		//비공개 방 만들기
		//int : 유닛 인덱스
		//byte : 유닛 강화도
		//int : 메인무기
		//int : 보조무기
		//int : 치장1 인덱스
		//int : 치장2 인덱스


		User _user = UserDataManager.instance.user;

		User_Units unit = _user.User_Units[_user.User_useUnit.UnitIdxs[0]];

		SendBuffer = new ByteData(512, 0);
		SendBuffer.InPutByte((int)unit.Unitidx);
		SendBuffer.InPutByte(unit.RefLv);
		SendBuffer.InPutByte((int)unit.MainWpnIdx);
		SendBuffer.InPutByte((int)unit.SubWpnIdx);
		SendBuffer.InPutByte((int)unit.DecoIdx1);
		SendBuffer.InPutByte((int)unit.DecoIdx2);
		SendBuffer.InPutByte((int)unit.DecoIdx3);
		SendBuffer.InPutByte(unit.SubSkill);
	}

	IEnumerator Chk_RoomMake()
	{
		//로비 로테이트 비활성
		User.isSelectedCharacter = true;

		//로딩바
		Loadmanager.instance.LoadingUI(true);

		if (!IsConnect())
		{
			//네트워크 소켓 재연결시도
			if (ConnectLogin_State != MMSERVER_STATE.RECONNECT_START)
			{
				UserEditor.Getsingleton.EditLog("방만들기 : 네트웍끊김, 네트웍재연결시도");
				ConnectLogin_State = MMSERVER_STATE.RECONNECT_START;
			}
		}

		while (true)
		{
			if (ConnectLogin_State == MMSERVER_STATE.CONNECT_FAIL)
			{
				break;
			}
			else if (ConnectLogin_State == MMSERVER_STATE.LOGIN_COMPLETE)
			{
				if (!IsConnect())
				{
					UserEditor.Getsingleton.EditLog("방만들기 : LOGIN_COMPLETE 이지만 네트웍끊김, 네트웍재연결시도");
					StartCoroutine(Chk_RoomMake());
					break;
				}

				UserEditor.Getsingleton.EditLog("방만들기 : 네트웍및 로그인이상무 방만들기 시도");

				SetSendBuffer_roomMake();
				Net_Script.Send_Data(NETKIND.CTS_ROOM_MAKE, SendBuffer.GetTrimByteData());
				//로비 로테이트 비활성
				User.isSelectedCharacter = false;

				//로딩바
				Loadmanager.instance.LoadingUI(false);
				break;
			}
			yield return null;
		}
	}


	//클라에서 서버로 공개방 설정 정보보내기
	public void Send_CTS_RoomOpen(bool isClose)
	{
		//bool : (true : 비공개, false : 공개)
		SendBuffer = new ByteData(512, 0);
		SendBuffer.InPutByte(isClose);

		Net_Script.Send_Data(NETKIND.CTS_OPEN_ROOM, SendBuffer.GetTrimByteData());
	}



	//클라에서 서버로 대기방초대 정보보내기
	public void Send_CTS_InviteRoom(uint inviteUserID, string inviteUserNkNm)
	{

		//초대 보내기
		//uint : 초대자 UserID
		//string : 초대자 닉네임

		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(inviteUserID);
		SendBuffer.InPutByte(inviteUserNkNm);

		Net_Script.Send_Data(NETKIND.CTS_INVITE_ROOM, SendBuffer.GetTrimByteData());
	}


	//클라에서 서버로 대기방초대 대한 답변 정보보내기
	public void Send_CTS_Answer_IntiveRoom(uint invitorUserID, bool isAccept, int unitIdx = 0,
		byte refLv = 0, int mainWpIdx = 0, int subWpIdx = 0, int decoIdx1 = 0, int decoIdx2 = 0, int decoIdx3 = 0, byte subSkill = 0)
	{
		//uint : 초대자 UsreID
		//bool : true : 초대 수락, false : 초대 거절
		//int : 유닛 인덱스
		//byte : 유닛 강화도
		//int : 메인무기
		//int : 보조무기
		//int : 치장1 인덱스
		//int : 치장2 인덱스

		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(invitorUserID);
		SendBuffer.InPutByte(isAccept);
		SendBuffer.InPutByte(unitIdx);
		SendBuffer.InPutByte(refLv);
		SendBuffer.InPutByte(mainWpIdx);
		SendBuffer.InPutByte(subWpIdx);
		SendBuffer.InPutByte(decoIdx1);
		SendBuffer.InPutByte(decoIdx2);
		SendBuffer.InPutByte(decoIdx3);
		SendBuffer.InPutByte(subSkill);



		Net_Script.Send_Data(NETKIND.CTS_INVITE_ROOM_ANSWER, SendBuffer.GetTrimByteData());

	}


	//클라에서 서버로 이동할 팀 정보 보내기
	public void Send_CTS_TeamMove(byte moveTeamIdx, byte slotIdx)
	{

		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(moveTeamIdx);
		SendBuffer.InPutByte(slotIdx);

		Net_Script.Send_Data(NETKIND.CTS_TEAM_MOVE, SendBuffer.GetTrimByteData());

	}


	//클라에서 서버로 맵변경 정보 본내기
	public void Send_CTS_map_Change(byte _mapIdx)
	{
		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(_mapIdx);

		Net_Script.Send_Data(NETKIND.CTS_MAP_CHANGE, SendBuffer.GetTrimByteData());
	}



	//클라에서 서버로 방나가기 정보 보내기 
	public void Send_CTS_RoomOUT()
	{

		Net_Script.Send_Data(NETKIND.CTS_ROOM_OUT, null);
	}


	//클라에서 서버로 채팅매세지 정보 보내기
	public void Send_CTS_ChatMessage(byte type, string msg)
	{
		//byte : 종류 (1:방 채팅)
		//string : 메세지 내용

		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(type);
		SendBuffer.InPutByte(msg);

		Net_Script.Send_Data(NETKIND.CTS_CHAT_MESSAGE, SendBuffer.GetTrimByteData());

	}


	//클라에서 서버로 대기방게임시작 정보보내기
	public void Send_CTS_READYROOM_START()
	{
		Net_Script.Send_Data(NETKIND.CTS_READYROOM_START, null);
	}

	IEnumerator Chk_READYROOM_START()
	{

		//로딩바
		Loadmanager.instance.LoadingUI(true);
		while (true)
		{
			if (ConnectLogin_State == MMSERVER_STATE.LOGIN_COMPLETE)
			{
				Net_Script.Send_Data(NETKIND.CTS_READYROOM_START, null);

				//로딩바
				Loadmanager.instance.LoadingUI(false);
				break;
			}
			yield return null;
		}
	}


	public void Send_CTS_FriendRoomJoin(uint _freindID, int _unitIdx, byte _refLv,
		int _mainWpnIdx, int _subWpnIdx, int _decoIdx1, int _decoIdx2, int _decoIdx3, byte subSkill)
	{
		//함께 하기
		//uint : 방 들어갈 친구 UserID
		//int : 유닛 인덱스
		//byte : 유닛 강화도
		//int : 메인무기
		//int : 보조무기
		//int : 치장1 인덱스
		//int : 치장2 인덱스
		SendBuffer = new ByteData(512, 0);

		SendBuffer.InPutByte(_freindID);
		SendBuffer.InPutByte(_unitIdx);
		SendBuffer.InPutByte(_refLv);
		SendBuffer.InPutByte(_mainWpnIdx);
		SendBuffer.InPutByte(_subWpnIdx);
		SendBuffer.InPutByte(_decoIdx1);
		SendBuffer.InPutByte(_decoIdx2);
		SendBuffer.InPutByte(_decoIdx3);
		SendBuffer.InPutByte(subSkill);

		Net_Script.Send_Data(NETKIND.CTS_FRIEND_ROOM_JOIN, SendBuffer.GetTrimByteData());
	}


	public void Send_CTS_SET_KILLWORD(string _killword)
	{
		SendBuffer = new ByteData(512, 0);
		SendBuffer.InPutByte(_killword);

		Net_Script.Send_Data(NETKIND.CTS_SET_KILL_WORDS, SendBuffer.GetTrimByteData());

		UserEditor.Getsingleton.EditLog("소켓서버 킬문구 보냄완료");
	}

	#endregion




	bool IsOwnRecieveNetKind(NETKIND kind)
	{
		bool isUse = false;
		switch (kind)
		{
			case NETKIND.STC_LOGIN:
			case NETKIND.STC_ROOM_MAKE:
			case NETKIND.STC_OPEN_ROOM:
			case NETKIND.STC_INVITE_ROOM:
			case NETKIND.STC_READYROOM_INFO:
			case NETKIND.STC_CHAT_MESSAGE:
			case NETKIND.STC_FRIEND_ROOM_JOIN:
			case NETKIND.STC_ERROR_CODE:
			case NETKIND.STC_POPUP_MSG:
			case NETKIND.STC_TOAST_MSG:
				

				isUse = true;
				break;
		}
		return isUse;
	}







	#region 에러처리

	public void ErrorData_Delegate()
	{
		User _user = UserDataManager.instance.user;

		//서버선택위한 로그아웃, 그냥로그아웃 할떄 소켓서버 disconnect 하게 되면 errorDelegate 들어 오는데 이경우는 정상플로우 이므로 무시하자
		if (_user.LogInState == Login_State.LogOut || _user.LogInState == Login_State.LogSelectServer)
		{
			UserEditor.Getsingleton.EditLog("정상로그아웃 인한 ErrorDelegate");
			return;
		}
		else
		{
			if (DisConnect_State == DISCONNECT_STATE.IDLE)
			{
				Disconnect(DisConnect_State, "메인메뉴서버에 네트워크끊어짐 disconnect 실행");
			}
		}
	}


	//소켓 에러 리시브
	public void RecieveError_Process()
	{
		Scene _scene = SceneManager.GetActiveScene();



		if (_scene.name == DefineKey.Main)
		{
			//재연결 작동 시작~
			//ConnectLogin_State = MMSERVER_STATE.RECONNECT_START;

			Operation_State = MMSERVER_STATE.IDEL;

			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CUSTOMROOM))
			{
				//팝업
				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
				popup.SetPopupMessage(string.Format("{0} \n {1}", TextDataManager.Dic_TranslateText[433], TextDataManager.Dic_TranslateText[434]));

				//로비로 이동 
				Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
				UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
				UI_Manager.Getsingleton.CreatUI(UI.TOP, _canvasTr);
			}

		}
		else
		{
			//모든팝업 클리어하기
			UI_Manager.Getsingleton.ClearPopupUI();

			UI_Manager.Getsingleton.ClearALL_UI();

			UI_Manager.Getsingleton._UI = UI.LOBBY;

			SceneManager.LoadScene(DefineKey.Main);

			Operation_State = MMSERVER_STATE.ERROR_END;
		}
	}

	public void RecieveError_EndProcess()
	{
		Scene _scene = SceneManager.GetActiveScene();

		if (_scene.name == DefineKey.Main)
		{
			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
			{

				//ConnectLogin_State = MMSERVER_STATE.RECONNECT_START;
				Operation_State = MMSERVER_STATE.IDEL;
				//로딩바있을수 있으니 로딩방 false
				Loadmanager.instance.LoadingUI(false);

				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
				popup.SetPopupMessage(string.Format("{0} \n {1}", TextDataManager.Dic_TranslateText[433], TextDataManager.Dic_TranslateText[434]));
				//popup.Set_addEventButton(callback_ConfirmError);
			}
		}
	}

	void callback_ConfirmError()
	{
		//타이틀로 돌아가 다시 서버선택으로
		UserDataManager.instance.user.LogInState = Login_State.LogSelectServer;


		UI_Manager.Getsingleton.CreatUI(UI.TITLE, UI_Manager.Getsingleton.CanvasTr);

	}

	#endregion





	public void Disconnect(DISCONNECT_STATE _DisConnect_State, String Error_Note = "")
	{
		if (Net_Script == null)
		{
			Debug.Log("Net_Script = 널");

			return;
		}
		else
		{
			Debug.Log("Net_Script = 널아님");


			if (_DisConnect_State == DISCONNECT_STATE.IDLE)
			{
				DisConnect_State = DISCONNECT_STATE.ERROR;

				Operation_State = MMSERVER_STATE.ERROR;

				if (Net_Script != null)
					Net_Script.Disconnect();

				Debug.LogError("Disconnet_Note : " + Error_Note);
			}
			else if (_DisConnect_State == DISCONNECT_STATE.NORMALITY)
			{
				if (Net_Script != null)
					Net_Script.Disconnect();

				Debug.LogError("Disconnet_Note : " + Error_Note);
			}
		}

	}


	void OnApplicationQuit()
	{
		if (Net_Script == null) return;

		UserEditor.Getsingleton.EditLog("어플리케이션 종료로 인한 채팅서버 소켓 종료");

		Net_Script.Disconnect();
	}
}
