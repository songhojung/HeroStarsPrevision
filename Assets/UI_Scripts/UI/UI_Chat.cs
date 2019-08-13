using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_Chat : UI_Base 
{
	private List<UIItem_ChatElement> Lst_chatElement = new List<UIItem_ChatElement>();


	public Transform Tr_context;
	private float ElementSizeY = 33f;								//채팅element 오브젝트 높이길이
	private Vector3 targetContentPos;								// 움직일 content의 pos
	private Queue<Coroutine> Que_chatMoveCrt = new Queue<Coroutine>();// 움직임코루틴 Queue

	private string TxtMessage;



	private static UI_Chat _instance;
	public static UI_Chat Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Chat)) as UI_Chat;
				//if (_instance == null)
				//{
				//	_instance = new GameObject("UI_Chat").AddComponent<UI_Chat>();
				//}
			}
			return _instance;
		}
	}


	public override void set_Open()
	{
		base.set_Open();

		

		Set_ChatInfo();
	}

	public override void set_Close()
	{
		base.set_Close();

		Clear_element();

	}

	public override void set_refresh()
	{
		base.set_refresh();

		
	}

	
	//채팅 UI 기본정보 설정하기
	void Set_ChatInfo()
	{
		
	}


	//대기방 수신메세지 처리
	public void RecieveMessage_BattleChat()
	{
		User _user = UserDataManager.instance.user;

		User_Chat _userchat = new User_Chat();
		_user.user_RecieveChat.AppyChatInfo(ref _userchat);

		Create_ChatElement(_userchat, true);
	}


	//배틀방 채팅창ㅇ 자신만보이는 알림메세지
	public void RecieveMessage_BattleChatNotice(string msg)
	{
		User_Chat _userchat = new User_Chat();
		_userchat.NkNm = "";
		_userchat.chatMsg = msg;

		Create_ChatElement(_userchat, false);
	}



	// 채팅 elem 생성
	void Create_ChatElement(User_Chat recieveChat, bool isSendmsg)
	{
		UIItem_ChatElement item = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_CHATELEMENT, Tr_context) as UIItem_ChatElement;
		item.Set_ElementInfo(recieveChat, isSendmsg,true);

		Lst_chatElement.Add(item);

		//Apply_ChatContent();

		
	}


	//채팅 콘텐츠 조정하기
	void Apply_ChatContent()
	{
		//이전 routine_moveChatcontent 코루틴이 있으면 위치 목적지까지 강제이동후 코루틴종료
		if (Que_chatMoveCrt.Count >= 1)
		{
			Tr_context.localPosition = targetContentPos;
			StopCoroutine(Que_chatMoveCrt.Dequeue());
		}

		//타겟 pos 설정
		targetContentPos = new Vector3(Tr_context.localPosition.x, Tr_context.localPosition.y + (ElementSizeY / 2), Tr_context.localPosition.z);

		//스무스 움직임 코루틴시작
		Coroutine crt = StartCoroutine(routine_moveChatcontent());
		Que_chatMoveCrt.Enqueue(crt);
	}



	// 채팅element 스무스 움직임
	IEnumerator routine_moveChatcontent()
	{

		float posY;
		Vector3 targetPos = targetContentPos;
		Vector3 nowPos = Tr_context.localPosition;
		float distance = 0f;
		while (true)
		{
			nowPos = Tr_context.localPosition;
			distance = Mathf.Abs(nowPos.y - targetPos.y);
			if (distance <= 1)
			{
				//Debug.Log("break함");
				Que_chatMoveCrt.Dequeue();
				break;
			}

			posY = Mathf.Lerp(Tr_context.localPosition.y, targetPos.y, Time.deltaTime * 3f);

			Tr_context.localPosition = new Vector3(Tr_context.localPosition.x, posY, Tr_context.localPosition.z);

			yield return null;
		}
	}



	public void Remove_Element(UIItem_ChatElement chatEle)
	{
		Lst_chatElement.Remove(chatEle);
	}
	

	void Clear_element()
	{
		for (int i = 0; i < Lst_chatElement.Count; i++ )
		{
			Destroy(Lst_chatElement[i].gameObject);
		}
		Lst_chatElement.Clear();
		
	}

	
}
