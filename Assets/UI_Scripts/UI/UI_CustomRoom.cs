using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
public class UI_CustomRoom : UI_Base
{
	public List<UIItem_TeamMemberElement> Lst_MemberSlots;

	private UIItem_TeamMemberElement SeletedMemberSlot ;
	//방
	public bool isRoomMaster = false;
	private bool isClose = true;
	private bool isPopupMaster = false;						//마스터 알림팝업 띄웟엇냐?
	public GameObject Obj_open;
	public List<GameObject> Lst_NoneMasterobj;				//방장아닐떄 활성화할 오브젝트들
	private List<uint> Lst_masterUser = new	 List<uint>();	//방장되었던 유저 리스트들
	//맵
	private int nowMapIdx = 0;
 
	public Image image_selectMap;

	//채팅
	private List<UIItem_ChatElement> Lst_chatElement = new List<UIItem_ChatElement>();
	public Transform Tr_chatContent;
	public InputField input_chat;					
	private string txtChat = string.Empty;							//입력한 채팅텍스트
	private float ElementSizeY = 33f;								//채팅element 오브젝트 높이길이
	private Vector3 targetContentPos;								// 움직일 content의 pos
	private Queue<Coroutine> Que_chatMoveCrt = new Queue<Coroutine>();// 움직임코루틴 Queue

	

	private static UI_CustomRoom _instance;
	public static UI_CustomRoom Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_CustomRoom)) as UI_CustomRoom;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("UI_CustomRoom");
					_instance = instanceObj.AddComponent<UI_CustomRoom>();
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


		Init_RoomInfo();


		//룸정보 설정
		Set_RoomInfo();
	}

	public override void set_Close()
	{
		base.set_Close();

		//닫을떄 정보 초기화
		Clear_info();
	}

	public override void set_refresh()
	{
		base.set_refresh();

		//룸 정보 갱신
		Set_RoomInfo();
	}

	void Init_RoomInfo()
	{

		//룸의 슬롯들 번호 및 팀지정
		for (int i = 0; i < Lst_MemberSlots.Count; i++)
		{
			Lst_MemberSlots[i].SlotIdx = i;

			if (i <= 5)
				Lst_MemberSlots[i].TeamIdx = 0; //레드팀
			else if (i > 5)
				Lst_MemberSlots[i].TeamIdx = 1; //블루팀
		}


		isRoomMaster = false;
		isClose = true;

		Clear_Chatelement();

	}


	//정보들 설정하기 
	public void Set_RoomInfo()
	{

		//설정 맵 정보  
		Set_SelectMap();

		//방 공개 or 비공개 
		Set_RoomOpen();

		//설정 슬롯정보
		Apply_MemberSlot();

		//방장 관련 설정체크
		Chk_roomMaster();
		
		
		//input_chat.OnPointerClick(OnPointerClick)
	}



	public void Apply_MemberSlot()
	{
		User _user = UserDataManager.instance.user;
		List<int> lst_Idx = new List<int>();


		//우선 클리어
		for (int i = 0; i < Lst_MemberSlots.Count; i++)
			Lst_MemberSlots[i].Clear_Slot();

		foreach (var user in _user.User_RoomUserInfos)
		{
			int slot = user.Value.roomUserSlot-1;
			Lst_MemberSlots[slot].SlotActive = true;
			Lst_MemberSlots[slot].Set_roomSlot(user.Value);
			lst_Idx.Add(slot);
		}

		
	}



	


	//맵 관련 정보 설정
	void Set_SelectMap()
	{
		nowMapIdx = UserDataManager.instance.user.User_readyRoomInfo.MapIndex;

		image_selectMap.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}",DefineKey.mapicon,nowMapIdx));
	}



	// 비공개방인지 아닌 지 설정
	void Set_RoomOpen()
	{
		User _user =  UserDataManager.instance.user;

		isClose = _user.User_readyRoomInfo.isCloseRoom;
		Obj_open.SetActive(isClose);
	}




	//방장 관련 설정체크
	void Chk_roomMaster()
	{
		User _user = UserDataManager.instance.user;

		//방장인지 체크
		if (_user.User_readyRoomInfo.RoomMsterUserID == _user.user_Users.UserID)
		{
			isRoomMaster = true;
			
		}
		else
			isRoomMaster = false;

		//방장이면 방장관련 오브젝트 활성 및 비활성
		Activate_NoneMasterObj(!isRoomMaster);


		//방장 변경되엇으면 알림토스트팝업
		if (isPopupMaster == false)
		{
			if (isRoomMaster)
			{
				isPopupMaster = true;
				UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[316]); //방장이 되엇습니다.
			}
		}

	}


	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			채팅관련 		================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////


	//입력완료시 
	public void ResponseInput_End()
	{

		input_chat.gameObject.SetActive(false);
		if (!input_chat.wasCanceled)
		{
			txtChat = input_chat.text;
			if (!string.IsNullOrEmpty(txtChat))
			{
				Debug.Log("txtChat : " + txtChat);
				//욕설인지 체크 : 욕설은 *로 바껴서 나옴
				TextDataManager.Chk_BannedLetter(ref txtChat);

				//채팅메세지 데이터 보내기
				Network_MainMenuSoketManager.Getsingleton.Send_CTS_ChatMessage(1, txtChat);
			}
			else
			{
				Debug.Log("empty");
				input_chat.text = "";
			}
		}
		else
		{
			Debug.Log("cancle");
		}
		input_chat.text = "";
	}



	//대기방 수신메세지 처리
	public void RecieveMessage_readyRoom()
	{
		User _user = UserDataManager.instance.user;

		User_Chat _userchat = new User_Chat();
		_user.user_RecieveChat.AppyChatInfo(ref _userchat);

		Create_ChatElement(_userchat);
	}

	
	// 채팅 elem 생성
	void Create_ChatElement(User_Chat recieveChat)
	{
		//채팅 갯수 체크후 삭제 
		Chk_removeChatElement();

		UIItem_ChatElement item = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CHATELEMENT, Tr_chatContent) as UIItem_ChatElement;
		item.Set_ElementInfo(recieveChat, true,false);
		Lst_chatElement.Add(item);

		//Apply_ChatContent();
	}


	//채팅 콘텐츠 조정하기
	void Apply_ChatContent()
	{
		//이전 routine_moveChatcontent 코루틴이 있으면 위치 목적지까지 강제이동후 코루틴종료
		if (Que_chatMoveCrt.Count >= 1)
		{
			Tr_chatContent.localPosition = targetContentPos;
			StopCoroutine(Que_chatMoveCrt.Dequeue());
		}

		//타겟 pos 설정
		targetContentPos = new Vector3(Tr_chatContent.localPosition.x, Tr_chatContent.localPosition.y + (ElementSizeY / 2), Tr_chatContent.localPosition.z);

		//스무스 움직임 코루틴시작
			Coroutine crt = StartCoroutine(routine_moveChatcontent());
			Que_chatMoveCrt.Enqueue(crt);
	}

	// 채팅element 스무스 움직임
	IEnumerator routine_moveChatcontent()
	{
		
		float posY;
		Vector3 targetPos = targetContentPos;
		Vector3 nowPos = Tr_chatContent.localPosition;
		float distance = 0f;
		while (true)
		{
			nowPos = Tr_chatContent.localPosition;
			distance = Mathf.Abs(nowPos.y - targetPos.y);
			if (distance <= 1)
			{
				//Debug.Log("break함");
				Que_chatMoveCrt.Dequeue();
				break;
			}

			posY = Mathf.Lerp(Tr_chatContent.localPosition.y, targetPos.y, Time.deltaTime * 3f);

			Tr_chatContent.localPosition = new Vector3(Tr_chatContent.localPosition.x, posY, Tr_chatContent.localPosition.z);

			yield return null;
		}
	}



	public void Clear_Chatelement()
	{
		for (int i = 0; i < Lst_chatElement.Count; i++)
		{
			Destroy(Lst_chatElement[i].gameObject);
		}
		Lst_chatElement.Clear();

	}


	//대기룸 close 될떄 초기화및 설정해야 할떄
	void Clear_info()
	{
		isPopupMaster = false;
		isClose = false;
	}


	// 갯수 체크후 삭제
	void Chk_removeChatElement()
	{
		if (Lst_chatElement.Count >4)
		{
		
			Destroy(Lst_chatElement[0].gameObject);
			Lst_chatElement.RemoveAt(0);
		}
	}



	//방장 아닐떄 활성화 할 오브젝트 
	void Activate_NoneMasterObj(bool isActive)
	{
		for (int i = 0; i < Lst_NoneMasterobj.Count; i++) { Lst_NoneMasterobj[i].SetActive(isActive); }
	}





	public void ResponseButton_roomOpen()
	{
		if (isRoomMaster)
		{
			isClose = !isClose;
			Network_MainMenuSoketManager.Getsingleton.Send_CTS_RoomOpen(isClose);
		}

		Debug.Log("roomOpen");
	}


	//맵선택
	public void ResponseButton_MapSelect(int direction)
	{

		// 자신이 방장이면 맵변경
		if (isRoomMaster)
		{
			int lastMapIdx = 6;  // 마지막 맵 인덱스
			int firstMapIdx = 0; //첫번쨰 맵인덱스

			if (direction == 0) //왼쪽버튼
			{
				nowMapIdx--;
				if (nowMapIdx < 0)
					nowMapIdx = lastMapIdx;
			}
			else if (direction == 1) //왼쪽버튼
			{
				nowMapIdx++;
				if (nowMapIdx > lastMapIdx)
					nowMapIdx = firstMapIdx;

			}

			Network_MainMenuSoketManager.Getsingleton.Send_CTS_map_Change((byte)nowMapIdx);
		}
		//popupMapOBJ.SetActive(true);
	}


	public void ResponseToggle_Map(int mapIdx)
	{
		if (nowMapIdx != mapIdx)
		{
			nowMapIdx = mapIdx;

			Set_SelectMap();
		}
	}



	//채팅하기, 끄기,켜기
	public void ResponseButton_Chat()
	{

		if (input_chat.isFocused == false)
		{
			input_chat.gameObject.SetActive(true);
			input_chat.text = "";
			EventSystem.current.SetSelectedGameObject(input_chat.gameObject, null);
			input_chat.OnPointerClick(new PointerEventData(EventSystem.current));
		}

		//isOnChat = !isOnChat;
		//input_chat.gameObject.SetActive(isOnChat);
	}




	//초대 하기
	public void ResponseButton_Invite()
	{
		UI_Popup_Invite popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Invite>(UIPOPUP.POPUPINVITE);

	}




	public void ResponseButton_Start()
	{

		//방장만 게임시작
		if (isRoomMaster)
		{
			if (isClose == true)
			{
				UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[298]); // 비공개 상태로 게임시작시 결과 보상을 받지 못하게 됩니다. \n 그래도 게임 시작 하시겠습니까?
				popup.Set_addEventButton(callback_getStartReadyRoom);
			}
			else
			{
				callback_getStartReadyRoom();
			}
		}
	}

	void callback_getStartReadyRoom()
	{

		Network_MainMenuSoketManager.Getsingleton.Send_CTS_READYROOM_START();
	}


	public void ResponseButton_Back()
	{
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_RoomOUT();

		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
		UI_Manager.Getsingleton.CreatUI(UI.TOP, _canvasTr);
	
	}
}
