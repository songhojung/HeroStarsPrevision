using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using Assets.SimpleAndroidNotifications;
public class UI_Lobby : UI_Base 
{
    #region sigletone
    private static UI_Lobby _instance;
    public static UI_Lobby Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UI_Lobby)) as UI_Lobby;

                if (_instance == null)
                {
                    GameObject instanceObj = new GameObject("UI_Lobby");
                    _instance = instanceObj.AddComponent<UI_Lobby>();
                }
            }

            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;

    }
    #endregion



    public enum EumLobbyObj
	{
		Event = 0,
		HelperFirstSelect= 1,
		InviteMsg = 2,
		BeginnerGameBtn = 3,
		EventBanner = 4,

	}

    //멤버변수
    private User user;
    private Dictionary<int, characterInfo> Dic_characterInfos = new Dictionary<int, characterInfo>();
	public Dictionary<uint, infos_unit> Dic_infosUnit = new Dictionary<uint, infos_unit>();
	public Dictionary<uint, User_Units> Dic_UserUnit = new Dictionary<uint, User_Units>();



	//상단 UI
	public Image image_clanMark;
	public Image image_Flag;
	public Text text_UserName;
	public Text text_UserID;

    //캐릭터 쪽
    public List<characterInfo> Lst_LobbyChar;
    public List<Text> Lst_TextCharName;
    public List<Text> Lst_TextCharRefLv;
    


    //공통 오브젝트
    public List<GameObject> Lst_publicLobbyObj;

	//이벤트 정보
	public List<GameObject> Lst_EventObj;




	//출첵관련
	public Text text_AtdLeftTime;
	private Coroutine AttdRoutine;
	

	//핑관련
	public Text text_ping;

	//초대메시지 관련
	private uint inviterUserID = 0; 
	public Text text_Sender;

	//버튼 마크 관련
	public List<GameObject> Lst_Mark;

    //상점팝업
    private UI_Popup_Store popupStore;




    public override void set_Open()
    {
        base.set_Open();


		Set_LobbyInfo();

        // 로그인시 각종 팝업들 띄움,표시 하기
        Show_PopupsWhenLogin();
    }

	public override void set_Close()
	{
		base.set_Close();

		
	}



	public override void set_refresh()
	{
		base.set_refresh();

        Set_LobbyInfo();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            UI_Popup_Showing popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Showing>(UIPOPUP.POPUPSHOWINGLEVEL);
            popup.set_showingInfo(new ShowingInfo(), ShowingKind.UserlvUp);
        }
    }


    void CloseProcess()
	{
		//ui나갈떄 출첵표시 코루틴 정지
		if (AttdRoutine != null)
			StopCoroutine(AttdRoutine);
	}


	

	void Set_LobbyInfo()
	{
        user = UserDataManager.instance.user;
        

		//기본적 필요 데이터정보 저장
		Chk_basicalInfo();

		//상단유저정보 설정
		Set_TopLobbyUserInfo();


		//로비 캐릭터 정보 설정
		Set_LobbyCharacterInfo();


        //기타 UI 설정
        Set_OtherInfo();


		

	}

	void Chk_basicalInfo()
	{

		//유닛정보 할당
		
		Dic_UserUnit = UserDataManager.instance.user.User_Units;

		//유닛구성정보 체크
		Dictionary<uint,Infos_EventItemTime> dic_eventUnit = TableDataManager.instance.Infos_EventItemTimes;
		Dictionary<uint,infos_unit>_dicInfosUnit = TableDataManager.instance.Infos_units;
		Dictionary<uint, infos_unit> _DicInfosUnitTemp = new Dictionary<uint, infos_unit>();

        //foreach (var infoUnit in _dicInfosUnit)
        //{
        //	if (dic_eventUnit.ContainsKey(infoUnit.Value.UnitIdx))
        //	{
        //		if (TimeManager.Instance.Get_nowTime() > dic_eventUnit[infoUnit.Value.UnitIdx].BuyEndTm)
        //		{
        //			_DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value; 
        //		}
        //		else
        //		{
        //			if(Dic_UserUnit.ContainsKey(infoUnit.Value.UnitIdx))
        //				_DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value; 
        //		}
        //	}
        //	else
        //		_DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value; 
        //}


        ////유닛구성정보 할당
        Dic_infosUnit = _dicInfosUnit;
        //Dic_infosUnit.Clear();
        //Dic_infosUnit = _DicInfosUnitTemp;


    }


	//상단유저정보 설정
	void Set_TopLobbyUserInfo()
	{
		User _user = UserDataManager.instance.user;

		//클랜마크
		if (_user.user_Clans.ClanID != 0)
			image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}",DefineKey.clanmark,_user.clan_Clans.ClanMark));
		else
			image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
		//이름
		text_UserName.text = _user.user_Users.NkNm;
		//아이디
		text_UserID.text = _user.user_Users.UserID.ToString();


		//버프
			//클랜골드버프
		if (_user.clan_Clans.GoldBufTm > TimeManager.Instance.Get_nowTime())
			Lst_EventObj[(int)BUFF_TYPE.CLANGOLD-1].SetActive(true);
		else
			Lst_EventObj[(int)BUFF_TYPE.CLANGOLD - 1].SetActive(false);

			//클랜exp버프
		if (_user.clan_Clans.ExpBufTm > TimeManager.Instance.Get_nowTime())
			Lst_EventObj[(int)BUFF_TYPE.CLANEXP].SetActive(true);
		else
			Lst_EventObj[(int)BUFF_TYPE.CLANEXP].SetActive(false);


			//이벤트exp버프
		bool expEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.EXP);
		Lst_EventObj[(int)BUFF_TYPE.EVENTEXP + 1].SetActive(expEvent);

			//이벤트 gold버프
		bool goldEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.GOLD);
		Lst_EventObj[(int)BUFF_TYPE.EVENTGOLD].SetActive(goldEvent);




	}





	//로비 캐릭터 정보 설정
	public void Set_LobbyCharacterInfo()
	{
        uint[] useUnitidxs = user.User_useUnit.UnitIdxs;

        for (int i = 0; i < useUnitidxs.Length; i++)
        {
            User_Units userUnit = null;

            if (user.User_Units.ContainsKey(useUnitidxs[i]))
                userUnit = user.User_Units[useUnitidxs[i]];

            //캐릭터 오브젝트정보
            Apply_CharacterConfig(userUnit,(BatchType)i);

            //캐릭터 슬롯정보
            Apply_CharacterSlot(userUnit, (BatchType)i);
        }




    }



    //캐릭터 오브젝트정보
    void Apply_CharacterConfig(User_Units _userUnit,BatchType batch)
    {

        if (_userUnit != null)
        {
            Lst_LobbyChar[(int)batch].gameObject.SetActive(true);

            if (!Dic_characterInfos.ContainsKey((int)batch))
            {
                //캐릭에 구성정보 할당
                infos_unit infoUnit = null;
                if (Dic_infosUnit.ContainsKey(_userUnit.Unitidx))
                    infoUnit = Dic_infosUnit[_userUnit.Unitidx];

                //캐릭 초기화
                Lst_LobbyChar[(int)batch].Init_chrctBasicInfo(_userUnit, infoUnit);


                //초기화된 캐릭터 Dic에 할당
                Dic_characterInfos[(int)batch] = Lst_LobbyChar[(int)batch];
            }
            else
            { 
                infos_unit infoUnit = null;
                if (Dic_infosUnit.ContainsKey(_userUnit.Unitidx))
                    infoUnit = Dic_infosUnit[_userUnit.Unitidx];

                //characterInfo 갱신위한 파라미터 재할당
                Dic_characterInfos[(int)batch].Refresh_ChrctInfo(_userUnit, infoUnit);
            }

        }
        else
        {
            Lst_LobbyChar[(int)batch].gameObject.SetActive(false);
        }
       
    }


    //캐릭터 슬롯정보
    void Apply_CharacterSlot(User_Units _userUnit, BatchType batch)
    {
        if (_userUnit != null)
        {
            infos_unit infoUnit = null;
            if (Dic_infosUnit.ContainsKey(_userUnit.Unitidx))
                infoUnit = Dic_infosUnit[_userUnit.Unitidx];
            //이름
            Lst_TextCharName[(int)batch].text = infoUnit.UnitName;

            //강화 레벨
            Lst_TextCharRefLv[(int)batch].text = string.Format("Lv.{0}", _userUnit.RefLv);
        }
    }







	












	


	// 로그인시  각종 팝업들 띄움,표시 하는 함수
	void Show_PopupsWhenLogin()
	{
		//일일출첵관련 
		Chk_AttendDay();

		//알림판 띄우기
		Chk_Announcement();
	}


	//알림판 띄우기 
	void Chk_Announcement()
	{
		//TabjoyTool.instance.Show_TjContent("notice");
	}



	//출첵보상요청
	void Chk_AttendDay()
	{
		UserDataManager.instance.user.IsChkAttd = true;

		//로비캐릭회전 잠금
		User.isSelectedCharacter = true;

		webRequest.GetAttendDayRwd(ShowPopup_AttendDay);

		
	}

	


	//일일출첵관련 팝업
	void ShowPopup_AttendDay()
	{
		
		User _user = UserDataManager.instance.user;
		GainItem AttendGainItem = null;
		if (webResponse.GetResultGainItem(ITEMTYPE.ATDDAY, ref AttendGainItem))
		{
			DateTime nowTime = TimeManager.Instance.Get_nowTime();
			DateTime _mtime = AttendGainItem.mtime;
			_mtime.AddDays(1);

			if (_mtime.Year <= nowTime.Year)
			{
				if (_mtime.Month <= nowTime.Month)
				{
					if (_mtime.Day <= nowTime.Day)
					{
						// 출첵팝업
						UI_Popup_Attendace popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Attendace>(UIPOPUP.POPUPATTENDANCE);
						popup.Set_RwdInfo(AttendGainItem.num, UI_Popup_Attendace.AttendRwd_Type.AttdToday);


					}
				}
			}
		}

		//소식관련 마크 처리
		if (_user.MarkChanges.ContainsKey(MarkTitleType.NewsCount))
		{
			Lst_Mark[(int)MarkTitleType.NewsCount].SetActive(_user.MarkChanges[MarkTitleType.NewsCount]);
		}

	}



	








	//출첵보상시간표시
	void Set_AttendDayLeftTime()
	{

		AttdRoutine = StartCoroutine(routine_AttendTime(Chk_AttendDay));
	}

	

	IEnumerator routine_AttendTime(del_NextProcess nextProcess)
	{
		DateTime nowTime = TimeManager.Instance.Get_nowTime();
		TimeSpan nowSpan = new TimeSpan(0, nowTime.Hour, nowTime.Minute, nowTime.Second);
		TimeSpan DailySpan = new TimeSpan(24,0,0);
		double totalTime = (DailySpan - nowSpan).TotalSeconds;

		int hour;
		int min;
		int sec;

		while (true)
		{
			if (totalTime <= 0)
			{
				if (nextProcess != null)
					nextProcess();
				break;
			}
			hour = (int)totalTime / 3600;
			min = (int)((totalTime % 3600)/60);
			sec = (int)totalTime % 60;

			text_AtdLeftTime.text = string.Format("{0}:{1}:{2}", hour.ToString("D2"), min.ToString("D2"), sec.ToString("D2"));

			totalTime -= Time.deltaTime;

			yield return null;
		}
	}






	


	


	//기타 UI 설정
	public void Set_OtherInfo()
	{
		//출첵보상시간표시
		Set_AttendDayLeftTime();

		//초대메세지 설정 (초대메세지 있으면 팝업보이기)
		Set_InviteMessage();

        //연출잇는지 체크
        Chk_Showing();

        //리뷰작성 부탁 팝업
        Chk_review();

		//알림 표시 설정 관련
		Set_newsMark();

		//광고관련 체크
		Chk_Ads();



        Find_PopupStore();


    }


    //top의 상점팝업
    void Find_PopupStore()
    {
        if(UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
        {
            if(popupStore==null)
             popupStore = UI_Top.Getsingleton.popupStore;
        }
    }






    //연출이 잇는지 체크
    void Chk_Showing()
    {
        GainItem returnGainItem= null;
        if (BaseData.m_Datas != null && BaseData.m_Datas.Count > 0)
        {
            returnGainItem = (GainItem)BaseData.m_Datas[0];
            BaseData.m_Datas.RemoveAt(0);

            if (returnGainItem != null)
            {
                UI_Popup_Showing popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Showing>(UIPOPUP.POPUPSHOWINGLEVEL);
                popup.set_showingInfo(new ShowingInfo(), ShowingKind.UserlvUp);
            }
        }
        //if (webResponse.GetResultGainItem(ITEMTYPE.LV, ref returnGainItem))
        //{
        //    UI_Popup_Showing popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Showing>(UIPOPUP.POPUPSHOWINGLEVEL);
        //    popup.set_showingInfo(new ShowingInfo(), ShowingKind.UserlvUp);
        //}
    }











	//리뷰작성 부탁 
	void Chk_review()
	{
		if (!PlayerPrefs.HasKey(DefineKey.ReviewLater))
		{
			if (UserDataManager.instance.user.Get_user_goods(ITEMTYPE.GAMEPLAY) >= 3) //3판이상이냐
			{
				UI_Popup_Review popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Review>(UIPOPUP.POPUPREVIEW);
				popup.Set_addEventYESButton(callback_review);

				PlayerPrefs.SetInt(DefineKey.ReviewLater,1);
				PlayerPrefs.Save();
			}
		}
	}

	void callback_review()
	{
		string marketUrl = string.Empty;

#if UNITY_EDITOR
		marketUrl = "market://details?id=com.cle.dy.Suddenground";
#elif UNITY_ANDROID
		marketUrl = "market://details?id=com.cle.dy.Suddenground";
#elif UNITY_IOS
		marketUrl = "https://itunes.apple.com/app/id1395707286";
#endif
		Application.OpenURL(marketUrl);
	}

	

	//알림 표시 설정 관련
	public void Set_newsMark()
	{
		User _User = UserDataManager.instance.user;

		//마크 표시 혹은 미표시 이면 표시 또는 미표시 해주자
		foreach (var mark in _User.MarkChanges)
		{
			Lst_Mark[(int)mark.Key].SetActive(mark.Value);
		}
	}


	void Chk_Ads()
	{

		// 판마다 광고보기 체크
		Chk_GameEndShowAds();
	}


	void Chk_GameEndShowAds()
	{
		User _user = UserDataManager.instance.user;

		//광고표시
		if (_user.Get_user_goods(ITEMTYPE.AD_REMOVE) > 0)
		{

			UserEditor.Getsingleton.EditLog("광고 제거 햇다");
		}
		else
		{
			//겜완료 해서 광고봐야하냐
			//if (AdsManager.instance.IsShowAdsGameEnd)
			//{
			//	UserEditor.Getsingleton.EditLog("판수 : " + _user.Get_user_goods(ITEMTYPE.GAMEPLAY));
			//	if (_user.Get_user_goods(ITEMTYPE.GAMEPLAY) % 
			//		TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_AfterGameShowAd].ConsVal == 0) // 2판마다 광고보기
			//	{
			//		UserEditor.Getsingleton.EditLog("광고 제거 안햇다, 2판마다 광고보기");

			//		AdsManager.instance.Show_interstitalAds();
			//		AdsManager.instance.nextAdsProcess = callback_complete_watchAds;
			//	}
			//}
		}
			

	}

	void callback_complete_watchAds()
	{
		//AdsManager.instance.IsShowAdsGameEnd = false;
	}





    // ============= 핑 설정 ==================
    public void Set_Ping(int ping)
    {
        //핑정보
        text_ping.text = string.Format("{0}ms", ping);
    }











    void Activate_publicLobbyObj(EumLobbyObj obj,bool isActive)
	{
		for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
			if (i == (int)obj)
				Lst_publicLobbyObj[i].SetActive(isActive);
	}

	bool isActive_publicLobbyObj(EumLobbyObj obj)
	{
		bool isactive = false;
		for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
			if (i == (int)obj)
				if (Lst_publicLobbyObj[i].activeSelf)
					isactive = Lst_publicLobbyObj[i].activeSelf;

		return isactive;
	}

	void Activate_EventObj(int idx, bool isActive)
	{
		for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
			if (i == idx)
				Lst_publicLobbyObj[i].SetActive(isActive);
	}



	





	




   
	



	//이벤트배너
	public void ResponseButton_EventBanner()
	{

		
    }


	






#region 초대메세지 관련

	//초대메세지 설정
	void Set_InviteMessage()
	{
		User _user = UserDataManager.instance.user;
		
		//초대메세지가 1개이상이고 , 메세지 오브젝트가 비활성이면 메세지보이기
		if (!isActive_publicLobbyObj(EumLobbyObj.InviteMsg) && _user.User_LstRcvRoomInvites.Count >0)
		{
			Show_InviteMessage();
		}
	}


	//초대 메세지 보이기
	public void Show_InviteMessage()
	{
		User _user = UserDataManager.instance.user;

		
		if (!isActive_publicLobbyObj(EumLobbyObj.InviteMsg))
		{
			Activate_publicLobbyObj(EumLobbyObj.InviteMsg, true);


			text_Sender.text = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count -1].InviterUserNkNm;

			//초대자 아이디 저장
			inviterUserID = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserID;

		}
		else
		{
			Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);
			Activate_publicLobbyObj(EumLobbyObj.InviteMsg, true);

			text_Sender.text = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserNkNm;

			//초대자 아이디 저장
			inviterUserID = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserID;

		}
	}

	//초대수락
	public void ResponseButton_AcceptInvite()
	{
		User _user = UserDataManager.instance.user;
		User_Units unit = null;
		Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);

		//유닛 소유하고 있다면
		if (_user.User_Units.ContainsKey(_user.User_useUnit.UnitIdx))
		{
			unit = _user.User_Units[_user.User_useUnit.UnitIdx];
		}
		else //유닛 소유 않고 있다면 가지고 있는것중 아무거나 할당
		{
			foreach (var hUnit in _user.User_Units)
			{
				unit = hUnit.Value;
				_user.User_useUnit.UnitIdx = unit.Unitidx; //배틀 에 값정보 전달하기위해 User_useUnit 값변경해야한다.
				break;
			}

		}


		Network_MainMenuSoketManager.Getsingleton.Send_CTS_Answer_IntiveRoom(inviterUserID, true, (int)unit.Unitidx, unit.RefLv,
			(int)unit.MainWpnIdx, (int)unit.SubWpnIdx, unit.DecoIdx1, unit.DecoIdx2);

		inviterUserID = 0;

		//초대 수락했으니  초대 리스브 정보들 클리어
		_user.User_LstRcvRoomInvites.Clear();
	}

	//초대메시지 닫기
	public void ResponseButton_CloseInviteMsg()
	{
		User _user = UserDataManager.instance.user;
		//Activate_publicLobbyObj(LobbyObj.InviteMsg, false);
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_Answer_IntiveRoom(inviterUserID, false);
		

		//초대 닫기햇으니 다음 초대메세지 보이게
		if (_user.User_LstRcvRoomInvites.Count > 0)
		{
			var lst_same = _user.User_LstRcvRoomInvites.RemoveAll(n => n.InviterUserID == inviterUserID);

			if (lst_same == 0)
				_user.User_LstRcvRoomInvites.Clear();

			//for (int i = 0; i < _user.User_LstRcvRoomInvites.Count; i++)
			//{
			//	if (inviterUserID == _user.User_LstRcvRoomInvites[i].InviterUserID)
			//		_user.User_LstRcvRoomInvites.Remove(_user.User_LstRcvRoomInvites[i]);
			//}
			//_user.User_LstRcvRoomInvites.Remove(_user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1]);

			inviterUserID = 0;


			if (_user.User_LstRcvRoomInvites.Count != 0)
				Show_InviteMessage();
			else
				Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);

		}

	}


    #endregion





    public void ResponseButton_Attend()
    {
        // 출첵팝업
        UI_Popup_Attendace popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Attendace>(UIPOPUP.POPUPATTENDANCE);
        popup.Set_RwdInfo(UserDataManager.instance.user.Get_user_goods(ITEMTYPE.ATDDAY));

    }



    //게임 시작
    public void ResponseButton_StartGame(int battleIdx)
    {
        BattleEnteranceKind beKnd = (BattleEnteranceKind)battleIdx;
        del_webResp_0 callback = null;

        if (beKnd ==  BattleEnteranceKind.QuickJoin) //일반 퀵조인
        {
            callback = ResponseButton_QuickJoin;
        }
        else if (beKnd == BattleEnteranceKind.Trainning) //초보자 퀵조인
        {
            callback = ResponseButton_Traning;
        }
        else if (beKnd == BattleEnteranceKind.MakeRoom) //초보자 퀵조인
        {
            callback = ResponseButton_CreateRoom;
        }

        uint[] useUnitIdxs = user.User_useUnit.UnitIdxs;


        webRequest.SetUseUnitIdx(useUnitIdxs[0], useUnitIdxs[1], useUnitIdxs[2], callback);
    }

    void callback_complete_StartGamePopup()
    {

        UI_Popup_StartGame popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_StartGame>(UIPOPUP.POPUPSTARTGAME);

    }

    void callback_complete_StartGameBeginner()
    {
        User _user = UserDataManager.instance.user;

        //로딩바
        Loadmanager.instance.LoadingUI(true);

        Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.TRY_QUICKJOIN;

        //퀵조인 요청 보내기
        _user.user_Games.battleGameKind = BattleKind.BEGINNER_TEAM_BATTLE;
        Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);
    }


    public void ResponseButton_CreateRoom()
    {

        Network_MainMenuSoketManager.Getsingleton.Send_CTS_RoomMake();

    }



    //퀵조인시도
    public void ResponseButton_QuickJoin()
    {
        User _user = UserDataManager.instance.user;

        Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.TRY_QUICKJOIN;

        //퀵조인 요청 보내기
        _user.user_Games.battleGameKind = BattleKind.NORMAL;    //배틀종류 저장
        Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);


    }



    public void ResponseButton_Traning()
    {
        User _user = UserDataManager.instance.user;

        Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.TRY_QUICKJOIN;


        //퀵조인 요청 보내기
        _user.user_Games.battleGameKind = BattleKind.ALONE_PLAY_BATTLE; //배틀종류 저장
        Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);
    }





    public void ResponseButton_SelectCharacter(int idx)
    {
        //현재 선택한 배치 값 저장
        user.User_useUnit.nowSelectBatch = (BatchType)idx;

        //캐릭터 셋팅으로
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UIData udata = new UIData(new List<object> { user.User_useUnit.UnitIdxs[(int)user.User_useUnit.nowSelectBatch] });
        UI_Manager.Getsingleton.CreatUI(UI.CHARACTERSETTING, _canvasTr,udata);
    }




    public void ResponseButton_Equipment()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.EQUIPMENT, _canvasTr);

	}

    public void ResponseButton_setting()
    {
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.SETTING, _canvasTr);
		
    }

	public void ResponseButton_Clan()
	{
		//로비캐릭회전 잠금
		User.isSelectedCharacter = true;
		webRequest.ClanInfo(callback_complete_ClanInfo);
	}



	void callback_complete_ClanInfo()
	{
		//로비캐릭회전 잠금해제
		User.isSelectedCharacter = false;

		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.CLAN, _canvasTr);
	}



	public void ResponseButton_Ranking()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.RANKING, _canvasTr);

	}
	public void ResponseButton_Friends()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.FRIEND, _canvasTr);
	}
    public void ResponseButton_News()
    {
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.NEWS, _canvasTr);
    }


    public void ResponseButton_Store()
    {
        //if (popupStore.gameObject.activeSelf == false)
        //    popupStore.Start_OnTap(STOREMODE_TYPE.Package);

        popupStore.gameObject.SetActive(!popupStore.gameObject.activeSelf);

    }



    public void ResponseButton_UserInfo()
    {
        UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
        User _user = UserDataManager.instance.user;
        popup.Set_UserInfo(_user);

    }







    public void ReseponseButton_gotoSUGfacebookPage()
	{
		Application.OpenURL("https://www.facebook.com/HiClegames/");
	}
	

	//뒤로가기 (서버선택으로)
	public void ResponseButton_Back()
	{
		User _user = UserDataManager.instance.user;

		//if (_user.User_GameServerInfos.Count > 1)
		{
			//로그인상태 변경 -> 서버선택으로
			_user.LogInState = Login_State.LogSelectServer;
		}
		//else
		//	_user.LogInState = Login_State.LogOut;
		

		//메인메뉴 소켓서버 끊기 
		Network_MainMenuSoketManager.Getsingleton.Disconnect(DISCONNECT_STATE.NORMALITY, "서버선택로그아웃 인한 Disconnect");

		//타이틀 UI생성
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.TITLE, _canvasTr);
	}




}
