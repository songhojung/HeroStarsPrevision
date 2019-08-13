using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public enum ClanUI_Type
{
	Manage = 0,
	Board = 1,
	ClanList = 2,
    Info = 3,
}

public enum ClanObject_Type
{
	Clan = 0,
	ClanList = 1,
}

public partial class UI_Clan : UI_Base 
{

	public List<Transform> Lst_TrContent = new List<Transform>();
	public List<GameObject> Lst_Obj_UI = new List<GameObject>();
	public Transform tr_mgr;

	private List<UIItem_MemberElement> Lst_MemberElement = new List<UIItem_MemberElement>(); //management(관리) 쪽에 생성된 element 들
	private List<UIItem_BoardElement> Lst_BoardElement = new List<UIItem_BoardElement>(); // noticeBoard(게시판) 쪽에 생성된 element들
	private Dictionary<byte, Clan_members> Dic_ExistClanMember = new Dictionary<byte, Clan_members>(); // 존재하는 클랜멤버
	private Dictionary<ushort, Clan_Boards> Dic_ClanBoard = new Dictionary<ushort, Clan_Boards>(); // 게시판 게시물들

	private ClanUI_Type nowClanUI = ClanUI_Type.ClanList;


	private bool isMaster = false;					//본유저가 클랜의 마스터이냐
	private byte clanRank = 0;
	private byte userMemberIdx = byte.MaxValue;		//본유저의 멤버인덱스
	public string ChangedClanName = "";				//클랜이름생성시
	private string changedClanIntroTxt = "";		// 클랜소개글
	private Dictionary<string, Coroutine> Dic_BuffRoutines = new Dictionary<string, Coroutine>();


	//관리UI
	public InputField input_ClanIntro;
	public Text text_clanID;
	public Image image_clanMark;
	public Text text_clanName;
	public Text text_numOfClan;
	public List<GameObject> Lst_ClanMasterOBJ;				//클랜마스터만 쓰는 오브젝트
	public List<Text> Lst_textClanBuffTm;
	public List<Text> Lst_textClanBuffPrice;
	public List<Text> Lst_textClanBuffRateValue;
	public List<GameObject> Lst_buffTmOBJ;



	//게시판UI
	public InputField input_write;


	private static UI_Clan _instance;
	public static UI_Clan Getinstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Clan)) as UI_Clan;

				if (_instance == null)
				{
					GameObject newObj = new GameObject("UI_Clan");
					_instance = newObj.AddComponent<UI_Clan>();
				}
			}
			return _instance;
		}
	}
	void Awake()
	{
		_instance = this;
	}


	public override void set_Open()
	{
		base.set_Open();



		Init_set();
		



	}

	public override void set_Close()
	{
		base.set_Close();


	}

	public override void set_refresh()
	{
		base.set_refresh();

		Init_set();
	}

	



	//초기 설정
	void Init_set()
	{
		User _user = UserDataManager.instance.user;
		if (_user.user_Clans.ClanID != 0)
		{
			if (nowClanUI == ClanUI_Type.Board)
				nowClanUI = ClanUI_Type.Board;
			else
				nowClanUI = ClanUI_Type.Manage;
			
		}
		else
		{
			nowClanUI = ClanUI_Type.ClanList;
		}


		Active_ClanObject(nowClanUI);
	}


	// 클랜 활성화 할 오브젝트
	 void Active_ClanObject(ClanUI_Type type)
	{
		Set_clanUI(nowClanUI);

		if (type == ClanUI_Type.Manage || type == ClanUI_Type.Board)
		{
			Set_Clan();
		}
		else
		{
			Set_Clanlist();
		}

	}



	// 클랜관련 정보 설정
	public void Set_Clan()
	{
		//클랜 관련 정보 초기화
		Init_ClanInfo();

		//관리UIor게시판UI 별로 만들 element 생성및설정
		Set_Post_Elements(nowClanUI);


		// 클랜 board 갯수 체크 및 알림표시
		check_clanBoardCount();
	}


	//클랜 관련 정보 초기화
	void Init_ClanInfo()
	{
		//존재 하는 멤버만 추출
		Get_MembersAndBoards();

		userMemberIdx = Get_userMemberIdx();

		input_ClanIntro.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
	}


	//존재 하는 멤버만 추출및 게시판 데이터 가져오기 
	void Get_MembersAndBoards()
	{
		User _user = UserDataManager.instance.user;
		//존재 하는 멤버만 추출
		var existMember = from mem in _user.Clan_members
						  where /*mem.Value.NkNm != "" ||*/ mem.Value.UserID != 0 //닉네임 없거나 아이디가 0이면 제외
						  select mem;

		Dic_ExistClanMember = existMember.ToDictionary(s => s.Value.MemIdx, s => s.Value);

		Dic_ClanBoard = _user.Clan_boards;
	}

	

	// 클랜 board 갯수 체크
	void check_clanBoardCount()
	{

	
		
	}

	


	//본유저의 클랜멤버 인덱스 가져오기
	byte Get_userMemberIdx()
	{
		User _user = UserDataManager.instance.user;
		byte _memIdx = byte.MaxValue;

		var memIdx = from mem in Dic_ExistClanMember
					 where mem.Value.UserID == _user.user_Users.UserID
					 select mem;

		foreach (var pmem in memIdx)
		{
			_memIdx =  pmem.Value.MemIdx;
		}

		return _memIdx;
	}


	#region CLAN
	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			클랜 정보			================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////

	/// <summary>
	/// 관리UIor게시판UI 별로 만들 element 생성및설정
	/// </summary>
	public void Set_Post_Elements(ClanUI_Type tapIdx)
	{
		switch (tapIdx)
		{
			case ClanUI_Type.Manage:
				if (Lst_MemberElement.Count > 0)
				{
					Set_InfoManagement();
					Refresh_memberElement();
				}
				else
				{
					Set_InfoManagement();
					Creat_memberElement();
				}
				break;

			case ClanUI_Type.Board:
				if (Lst_BoardElement.Count > 0)
					Refresh_BoardElement();
				else
					Creat_BoardElement();

				break;

		}


	}

	// 관리UI의 정보들 설정
	private void Set_InfoManagement()
	{
		User _user = UserDataManager.instance.user;


		//유저의 클랜멤버등급 저장
		clanRank = Dic_ExistClanMember[userMemberIdx].CnRank;

		//마스터인지 체크
		if (clanRank == (byte)CLAN_MEMBER_TYPE.MASTER)
			isMaster = true;
		else
			isMaster = false;

		//클마 이면 관련오브젝트 활성 , 아니라면 비활성
		Activate_ClanMaterOBJ(isMaster);

		//클랜아이디
		text_clanID.text = _user.user_Clans.ClanID.ToString();

		//클랜 마크
		image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, _user.clan_Clans.ClanMark));

		//클랜이름
		text_clanName.text = _user.clan_Clans.ClanName.ToString();
		//Lst_inputs[1].text = user.clan_Clans.ClanName.ToString();


		//클랜현재 수
		text_numOfClan.text = string.Format("{0}/{1}", Dic_ExistClanMember.Count, _user.clan_Clans.PersonCnt);

		//클랜 소개글
		if (_user.clan_Clans.ClanInfoTxt != "")
			input_ClanIntro.text = _user.clan_Clans.ClanInfoTxt;

	//=====클랜버프
		//골드 버프 가격
		//Lst_textClanBuffPrice[(int)BUFF_TYPE.CLANGOLD - 2].text = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffPrice].ConsVal.ToString();

		//골드버프 비율
		Lst_textClanBuffRateValue[(int)BUFF_TYPE.CLANGOLD - 2].text = string.Format("{0}%", TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanGoldBuffRateValue].ConsVal);

		//골드버프 시간
		if (_user.clan_Clans.GoldBufTm >= TimeManager.Instance.Get_nowTime())
		//if (_user.clan_Clans.GoldBufTm > DateTime.MinValue) // 임시임 위에가 진짜임
		{
			if (!Dic_BuffRoutines.ContainsKey("GoldBuff"))
			{
				Coroutine routine = StartCoroutine(StaticMethod.routine_GetLeftTime(_user.clan_Clans.GoldBufTm, Lst_textClanBuffTm[(int)BUFF_TYPE.CLANGOLD - 2], 1));
				Dic_BuffRoutines["GoldBuff"] = routine;
				Lst_buffTmOBJ[(int)BUFF_TYPE.CLANGOLD - 2].SetActive(true);
			}
			else
			{
				StopCoroutine(Dic_BuffRoutines["GoldBuff"]);

				Coroutine routine = StartCoroutine(StaticMethod.routine_GetLeftTime(_user.clan_Clans.GoldBufTm, Lst_textClanBuffTm[(int)BUFF_TYPE.CLANGOLD - 2], 1));
				Dic_BuffRoutines["GoldBuff"] = routine;
				Lst_buffTmOBJ[(int)BUFF_TYPE.CLANGOLD - 2].SetActive(true);
			}
		}
		else
		{
			Dic_BuffRoutines.Remove("GoldBuff");
			Lst_textClanBuffTm[(int)BUFF_TYPE.CLANGOLD - 2].text = TextDataManager.Dic_TranslateText[112];//미적용
			Lst_buffTmOBJ[(int)BUFF_TYPE.CLANGOLD - 2].SetActive(false);
		}


		//exp 버프 가격
		//Lst_textClanBuffPrice[(int)BUFF_TYPE.CLANEXP +1].text = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffPrice].ConsVal.ToString();

		//exp 버프 비율
		Lst_textClanBuffRateValue[(int)BUFF_TYPE.CLANEXP + 1].text = string.Format("{0}%", TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanExpBuffRateValue].ConsVal);
		//exp 버프
		if (_user.clan_Clans.ExpBufTm >= TimeManager.Instance.Get_nowTime())
		//if (_user.clan_Clans.ExpBufTm > DateTime.MinValue) // 임시임 위에가 진짜임
		{
			if (!Dic_BuffRoutines.ContainsKey("ExpBuff"))
			{
				Coroutine routine = StartCoroutine(StaticMethod.routine_GetLeftTime(_user.clan_Clans.ExpBufTm, Lst_textClanBuffTm[(int)BUFF_TYPE.CLANEXP +1], 1));
				Dic_BuffRoutines["ExpBuff"] = routine;
				Lst_buffTmOBJ[(int)BUFF_TYPE.CLANEXP + 1].SetActive(true);
			}
			else
			{
				StopCoroutine(Dic_BuffRoutines["ExpBuff"]);

				Coroutine routine = StartCoroutine(StaticMethod.routine_GetLeftTime(_user.clan_Clans.ExpBufTm, Lst_textClanBuffTm[(int)BUFF_TYPE.CLANEXP + 1], 1));
				Dic_BuffRoutines["ExpBuff"] = routine;
				Lst_buffTmOBJ[(int)BUFF_TYPE.CLANEXP + 1].SetActive(true);
			}
		}
		else
		{
			Dic_BuffRoutines.Remove("ExpBuff");
			Lst_textClanBuffTm[(int)BUFF_TYPE.CLANEXP + 1].text = TextDataManager.Dic_TranslateText[112];//미적용
			Lst_buffTmOBJ[(int)BUFF_TYPE.CLANEXP + 1].SetActive(false);

		}

		
	}

	/// <summary>
	/// 관리의 멤버들 생성
	/// </summary>
	private void Creat_memberElement()
	{
		foreach (var member in Dic_ExistClanMember)
		{

			UIItem_MemberElement _element = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CLANMEMBERELEMENT, Lst_TrContent[(int)ClanUI_Type.Manage]) as UIItem_MemberElement;
			_element.Set_Element(member.Value, member.Value.CnRank == (byte)CLAN_MEMBER_TYPE.MASTER);

			Lst_MemberElement.Add(_element);
		}

		Sort_memberList(Lst_MemberElement);

	}

	/// <summary>
	/// 관리의 멤버들 정보 갱신 (시간 정보, 및 이름변경 , 함께 기능) 
	/// </summary>
	void Refresh_memberElement()
	{
		//Lst_MemberElement 리스트를 dic으로 변환후 dic의 데이터안에 Dic_ExistClanMember 키값에 잇는지 체크 => 즉Dic_ExistClanMember 데이터중에  Lst_MemberElement 가 없는 거 반환
		var noCreatedEle = Dic_ExistClanMember.Where(n => !Lst_MemberElement.ToDictionary(g => g.clan_member.MemIdx, g => g.clan_member).ContainsKey(n.Value.MemIdx));

		if (noCreatedEle.Count() > 0)
		{
			foreach (var ele in noCreatedEle)
			{
				UIItem_MemberElement _element = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CLANMEMBERELEMENT, Lst_TrContent[(int)ClanUI_Type.Manage]) as UIItem_MemberElement;
				_element.Set_Element(ele.Value, ele.Value.CnRank == (byte)CLAN_MEMBER_TYPE.MASTER);

				Lst_MemberElement.Add(_element);
			}

			
		}


		//클랜탈퇴, 클랜강퇴 등 사라진 멤버에 대해 기존생성된 elememt 삭제 해준다
		var deletedEle = Lst_MemberElement.Where(n => !Dic_ExistClanMember.ContainsKey(n.clan_member.MemIdx));

		if (deletedEle.Count() > 0)
		{
			for (int i = 0; i < deletedEle.ToList().Count; i++)
			{
				Destroy(deletedEle.ToList()[i].gameObject);
				Lst_MemberElement.Remove(deletedEle.ToList()[i]);
			}

			
		}


		//member 데이터 다시받고 element 정보 갱신한다.

		for (int i = 0; i < Lst_MemberElement.Count; i++ )
		{
			foreach (var member in Dic_ExistClanMember)
			{
				if (Lst_MemberElement[i].clan_member.MemIdx == member.Value.MemIdx)
					Lst_MemberElement[i].Set_Element(member.Value, member.Value.CnRank == (byte)CLAN_MEMBER_TYPE.MASTER);
			}
		}

		//정렬 다시한다
		Sort_memberList(Lst_MemberElement);

	}


	/// <summary>
	/// 게시판 게시물 생성
	/// </summary>
	private void Creat_BoardElement()
	{

		foreach (var board in Dic_ClanBoard)
		{
			UIItem_BoardElement _element = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CLANBOARDELEMENT, Lst_TrContent[(int)ClanUI_Type.Board]) as UIItem_BoardElement;
			_element.Set_Element(board.Value);

			// list board 에 element 담기
			Lst_BoardElement.Add(_element);
		}

		Sort_Boardlist(Lst_BoardElement);

	}

	/// <summary>
	/// 게시판 게시물 갱신
	/// </summary>
	void Refresh_BoardElement()
	{
		//lst_boardElement 리스트를 dic으로 변환후 dic의 데이터안에 dic_clanboard 키값에 잇는지 체크 => 즉Dic_ClanBoard 데이터중에  Lst_BoardElement 가 없는 거 반환
		var noCreatedEle = Dic_ClanBoard.Where(n=> !Lst_BoardElement.ToDictionary(g => g.Get_Clanboard().BodIdx, g => g.Get_Clanboard()).ContainsKey(n.Value.BodIdx));						  

		if (noCreatedEle.Count() > 0)
		{
			foreach (KeyValuePair<ushort,Clan_Boards> board in noCreatedEle)
			{
				UIItem_BoardElement _element = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CLANBOARDELEMENT, Lst_TrContent[(int)ClanUI_Type.Board]) as UIItem_BoardElement;
				_element.Set_Element(board.Value);

				// list board 에 element 담기
				Lst_BoardElement.Add(_element);
			}
		}


		//모든 element 들 다시 정보 할당하여 갱신
		for (int i = 0; i < Lst_BoardElement.Count; i++ )
		{
			foreach (var Bd in Dic_ClanBoard)
			{
				if (Lst_BoardElement[i].Get_Clanboard().BodIdx == Bd.Value.BodIdx)
					Lst_BoardElement[i].Set_Element(Bd.Value);
			}
		}


		Sort_Boardlist(Lst_BoardElement);
	}



	// 게시글들 솔팅하기 
	void Sort_Boardlist(List<UIItem_BoardElement> lst_board)
	{
		if (lst_board.Count > 0)
		{
			var sortlst = from br in lst_board
						  orderby br.Get_Clanboard().mtime descending
						  select br;

			for (int i = 0; i < sortlst.ToList().Count; i++ )
			{
				sortlst.ToList()[i].transform.SetAsLastSibling();
			}
		}
	}


	//클랜멤버 솔팅하기
	void Sort_memberList(List<UIItem_MemberElement> lst_member)
	{
		if (lst_member.Count > 0)
		{
			var sortlst = from mem in lst_member
						  //orderby mem.clan_member.MemIdx
						  orderby mem.clan_member.mtime 
						  orderby mem.isLogining 
						  select mem;

			for (int i = 0; i < sortlst.ToList().Count; i++)
			{
				sortlst.ToList()[i].transform.SetAsFirstSibling();
			}
		}
	}








	/// <summary>
	/// 현재 UI의 오브젝트 활성화/비활성화 설정, list 관리
	/// </summary>
	private void Set_clanUI(ClanUI_Type tapIdx)
	{
		if (tapIdx == ClanUI_Type.Manage)
		{
			// sort, 관리 만 활성
			for (int i = 0; i < Lst_Obj_UI.Count; i++)
				if(i == 0 || i == 3)
					Lst_Obj_UI[i].SetActive(true);
				else
					Lst_Obj_UI[i].SetActive(false);

		}
		else if (tapIdx == ClanUI_Type.Board)
		{
			// sort, 게시판만 활성
			for (int i = 0; i < Lst_Obj_UI.Count; i++)
				if (i == 1 || i == 3)
					Lst_Obj_UI[i].SetActive(true);
				else
					Lst_Obj_UI[i].SetActive(false);

			
		}
		else if (tapIdx == ClanUI_Type.ClanList)
		{
			// 클랜리스트 만 활성
			for (int i = 0; i < Lst_Obj_UI.Count; i++)
				if (i == 2)
					Lst_Obj_UI[i].SetActive(true);
				else
					Lst_Obj_UI[i].SetActive(false);

			
		}

	}




	//클랜마스터 관련 오브젝트 활성/ 비활성
	void Activate_ClanMaterOBJ(bool _isMaster)
	{

		for (int i = 0; i < Lst_ClanMasterOBJ.Count; i++)
		{
			Lst_ClanMasterOBJ[i].SetActive(_isMaster == true);
		}
		
	}




	/// <summary>
	/// 해당 element들 모두삭제
	/// </summary>
	public void ClearElements(ClanUI_Type clanType)
	{

		if (clanType == ClanUI_Type.Manage)
		{
			if (Lst_MemberElement.Count > 0)
			{
				//for (int i = 0; i < Lst_MemberElement.Count; i++)
				//	_uiMgr.RemoveUI_Item(Lst_MemberElement[i]);

				for (int i = 0; i < Lst_MemberElement.Count; i++)
				{
					Destroy(Lst_MemberElement[i].gameObject);

				}
				Lst_MemberElement.Clear();
			}
		}
		else if (clanType == ClanUI_Type.Board)
		{
			if (Lst_BoardElement.Count > 0)
			{
				//for (int i = 0; i < Lst_BoardElement.Count; i++)
				//	_uiMgr.RemoveUI_Item(Lst_BoardElement[i]);

				for (int i = 0; i < Lst_BoardElement.Count; i++)
				{
					Destroy(Lst_BoardElement[i].gameObject);

				}
				Lst_BoardElement.Clear();
			}
		}
		else if (clanType == ClanUI_Type.ClanList)
		{
			if (Lst_ClanlistElement.Count > 0)
			{
				//for (int i = 0; i < Lst_BoardElement.Count; i++)
				//	_uiMgr.RemoveUI_Item(Lst_BoardElement[i]);

				for (int i = 0; i < Lst_ClanlistElement.Count; i++)
				{
					Destroy(Lst_ClanlistElement[i].gameObject);

				}
				Lst_ClanlistElement.Clear();
			}
		}
		


	}
	
	
	/// <summary>
	/// 특정 element 하나만 삭제
	/// </summary>
	public void ClearElementOne(int tapIdx, UIItem_MemberElement element_)
	{
		List<UIItem_MemberElement> _getMemberElement = null;
		//List<UIItem_BoardElement> _getBoardElement = null;
		if (tapIdx == 0)
		{
			

			var _delElement = from el in Lst_MemberElement
							  where el.Equals(element_)
							  select el;

			_getMemberElement = _delElement.ToList();


				
		}
		else if (tapIdx == 1)
		{
		

			var _delElement = from el in Lst_MemberElement
							  where el.Equals(element_)
							  select el;

			_getMemberElement = _delElement.ToList();

		}

		for (int i = 0; i < _getMemberElement.Count; i++)
		{
			if (tapIdx == 0)
				Lst_MemberElement.Remove(_getMemberElement[i]);
			//else if (tapIdx == 1)
			//	Lst_BoardElement.Remove(_getMemberElement[i]);

			// element 파괴 하고 ui_manager에있는 컬랙션 remove
			Destroy(_getMemberElement[i].gameObject);
			UI_Manager.Getsingleton.Lst_UiItem.Remove(_getMemberElement[i]);
		}

		
	}




	/// <summary>
	/// tap 누를때 UI 생성
	/// </summary>
	public void ResponseButton_Tap(int tapIdx)
	{
		if (nowClanUI != (ClanUI_Type)tapIdx)
		{

			
			nowClanUI = (ClanUI_Type)tapIdx;
		
			// UI type에 관련된 설정
			Set_clanUI(nowClanUI);

			//type에 맞는 element 생성
			Set_Post_Elements(nowClanUI);

			

		}

	}




	// 클랜 소개글 변경 매서드 
	public void ResponseButton_ChangeClanIntroduce()
	{
		if (isMaster)
		{
			if (input_ClanIntro.isFocused == false)
			{
				User _user = UserDataManager.instance.user;
				input_ClanIntro.gameObject.SetActive(true);
				if (string.IsNullOrEmpty(_user.clan_Clans.ClanInfoTxt))
					input_ClanIntro.text = "";
				EventSystem.current.SetSelectedGameObject(input_ClanIntro.gameObject, null);
				input_ClanIntro.OnPointerClick(new PointerEventData(EventSystem.current));
			}
		}
		else
		{
			//클랜원일떄 팝업 주석
			//UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			//popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[91]);
		}
		
	}


	// 클랜 소개글 input 이 끝난 뒤 호출 되는 매서드
	public void ResponseInput_End_ChangeIntro()
	{
		changedClanIntroTxt = input_ClanIntro.text;
		input_ClanIntro.gameObject.SetActive(false);

		if (!TextDataManager.Chk_BannedLetter(ref changedClanIntroTxt))
		{
			webRequest.ClanInfoTxtChange(changedClanIntroTxt, null);
		}
		else
		{
			UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

			input_ClanIntro.text = "";
		}


	}



	//클랜마크 변경 시도 호출 매서드
	public void ResponseButton_ChangeClanMark()
	{
		if (isMaster)
		{
			//UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CLANMARKLIST, UI_Manager.Getsingleton.CanvasTr);
			UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ClanmarkList>(UIPOPUP.POPUPCLANMARKLIST);
		}
		else
		{
			//클랜원일떄 팝업 주석
			//UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			//popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[159]); //클랜마스터만 변경할수있습니다
		}
	}

	//클랜명 변경 시도시 호출 매서드
	public void ResponseButton_ChangeClanName()
	{
		if (isMaster)
		{
			UI_Popup_MakeName popupName = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_MakeName>(UIPOPUP.POPUPMAKENAME);
			popupName.Set_Info(MAKENAMEPOPUP_TYPE.CHANGECLANNAME, 5);
			popupName.Set_AddEventButton(callback_Complete_ClanName);

		}
		else
		{
			//클랜원일떄 팝업 주석
			//UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			//popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[159]);//클랜마스터만 변경할수있습니다
		}
	}


	//클랜명변경 완료시 버튼 매서드
	public void callback_Complete_ClanName()
	{

		nowClanUI = ClanUI_Type.Manage;
		Set_Clan();

		//제화 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();

	}



	//클랜원수 변경시 호출 매서드
	public void ResponseButton_ChangeNumOfClan()
	{
		User _user = UserDataManager.instance.user;
		if (isMaster)
		{
			if (_user.clan_Clans.PersonCnt >= 50)
			{
				UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[161]);
			}
			else
			{
				int needGem = (_user.clan_Clans.PersonCnt - 5) + 1;
				UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[162]);
				popup.SetPopupMessage(string.Format("{0}{1}", needGem,TextDataManager.Dic_TranslateText[163]));
				popup.Set_addEventButton(callback_AcceptChangeNumOfClan);
			}
		}
		else
		{
			//클랜원일떄 팝업 주석
			//UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			//popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[159]);//클랜마스터만 변경할수있습니다
		}
	}
	// 클랜원수 팝업 확인이후 콜백 매서드
	void callback_AcceptChangeNumOfClan()
	{
		User _user = UserDataManager.instance.user;
		int needGem = (_user.clan_Clans.PersonCnt - 5) + 1;
			webRequest.ClanPersonsCntUp(needGem, callback_complte_ChangeNumOfClan);
	}

	//클랜원수 프로토콜 호출 이후 콜백 매서드
	void callback_complte_ChangeNumOfClan()
	{
		nowClanUI = ClanUI_Type.Manage;
		Set_Clan();

		//제화 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();
	}



	//버프 버튼 매서드
	public void ResponseButton_Buff(int buffIdx)
	{
        //int buffType = buffIdx; //clanexp = 2 , clangold = 1
        //uint price = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_ClanBuffPrice].ConsVal;

        //webRequest.ClanSetBuf((byte)buffType, price, callback_complete_setBuff);
        UI_Popup_BuyBuff pop = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_BuyBuff>(UIPOPUP.POPUPBUYBUFF);
        pop.Set_BuyBuff((BUFF_TYPE)buffIdx);
	}

	void callback_complete_setBuff()
	{

		//clan_board 갱신을 하기위해 clanInfo 프로토콜 한번호출하자 
		webRequest.ClanInfo(callback_complte_RequestClaninfo);

		
	}

	public void callback_complte_RequestClaninfo()
	{
		nowClanUI = ClanUI_Type.Manage;
		Set_Clan();

		//제화 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();
	}







	//탈퇴  버튼 매서드
	public void ResponseButton_ClanOUT()
	{
		User _user = UserDataManager.instance.user;
		UI_Popup_Selective popoup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
		if (isMaster)
		{
			popoup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[153]);
			popoup.SetPopupMessage(TextDataManager.Dic_TranslateText[154]);
			popoup.Set_addEventButton(callback_Accept_Clanout);
		}
		else
		{
			popoup.Set_PopupTitleMessage(string.Format("{0}{1}", TextDataManager.Dic_TranslateText[18], TextDataManager.Dic_TranslateText[90]));
			popoup.SetPopupMessage(string.Format("{0} {1}", _user.clan_Clans.ClanName, TextDataManager.Dic_TranslateText[155]));
			popoup.Set_addEventButton(callback_Accept_Clanout);
		}
	}


	void callback_Accept_Clanout()
	{
		User _user = UserDataManager.instance.user;

		if (isMaster)
		{



			if (Dic_ExistClanMember.Count <= 1) // 멤버가 없다. 클랜을 삭제 시킨다는뜻
			{
				string _clanname = _user.user_Users.ClanName;
				webRequest.ClanOut(_user.user_Users.UserID, () => callback_Complete_Clanout(_clanname));
			}
			else
			{
				UI_Popup_Toast poptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				poptoast.SetPopupMessage(TextDataManager.Dic_TranslateText[156]);
			}
		}
		else // 클랜원이 클랜을 탈퇴한다.
		{
			string _clanname = _user.user_Users.ClanName;
			webRequest.ClanOut(_user.user_Users.UserID, () => callback_Complete_Clanout(_clanname));
		}
	}



	void callback_Complete_Clanout(string _clanName)
	{
		//클랜탈퇴했으니 clan_clans 값 초기화, clan_clanlist 초기화 , 클랜소개글, 클랜게시판 초기화 하자 
		User __user = UserDataManager.instance.user;
		__user.Clan_clanLists.Clear();
		__user.clan_Clans.Init();
		__user.Clan_boards.Clear();
		__user.Clan_members.Clear();
		input_ClanIntro.text = TextDataManager.Dic_TranslateText[91];//클랜마스터는 클랜소개글을 수정할수 있습니다

		ClearElements(ClanUI_Type.Board);
		ClearElements(ClanUI_Type.Manage);


		//이름변경하엿으니 소켓로그인 다시쏘자
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_Login();

		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);

		

		if (isMaster) // 토스트: 클랜을 삭제했다
		{
			UI_Popup_Toast poptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			poptoast.SetPopupMessage(string.Format("{0} {1}", _clanName, TextDataManager.Dic_TranslateText[157]));
		}
		else //토스트: 클랜 탈퇴했따
		{
			UI_Popup_Toast poptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			poptoast.SetPopupMessage(string.Format("{0} {1}", _clanName, TextDataManager.Dic_TranslateText[158]));
		}
	}
#endregion




	/// <summary>
	/// 게시물 글쓰기
	/// </summary>
	public void ResponseButton_Write()
	{
		UI_Popup_WriteBoard popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_WriteBoard>(UIPOPUP.POPUPWRITEBOARD);
		popup.Set_addEventButton(callback_Complete_writeBoard);
	}

	public void ReseponseInput_writeEnd()
	{
		string bodText = input_write.text;

		if (!string.IsNullOrEmpty(bodText))
		{
			if (!TextDataManager.Chk_BannedLetter(ref bodText))
			{
				webRequest.ClanBodWrite(bodText, callback_Complete_writeBoard);
				input_write.text = "";
			}
			else
			{
				UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

				input_write.text = "";
			}
		}

	}

	void callback_Complete_writeBoard()
	{
		nowClanUI = ClanUI_Type.Board;
		Set_Clan();
	}




	// back 버튼 누를때 로비로
	public void ResponseButton_Back()
	{

		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);

	}


}
