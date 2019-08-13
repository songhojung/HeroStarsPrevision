using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_ChatMessage : UI_PopupBase 
{
	private User_Chat userChat;
	public float stayTime = 3f;
	private float processStayTime = 0f;

	public Image image_ballon;
	public Image image_ClanMark;
	public Text text_sender;
	public List<Text> Lst_text_main;
	public Button AcceptButton;
	public List<GameObject> Lst_ObjMessage;

	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public override void Set_Refresh()
	{
		base.Set_Refresh();
	}



	public void Set_ChatMessage(User_Chat _userChat)
	{
		//채팅정보 저장
		userChat = _userChat;

		//stay 시간 할당
		processStayTime = stayTime;

		if (userChat.msgTp == ChatMessageType.Notice)
		{
			Lst_text_main[1].text = userChat.chatMsg;
			set_ObjMessage(1);
		}
		else
		{
			set_ObjMessage(0);
			if (userChat.msgTp == ChatMessageType.Room || userChat.msgTp == ChatMessageType.Battle)
			{
				//말풍선
				image_ballon.sprite = ImageManager.instance.Get_Sprite(DefineKey.ChatIcon_all);
				//수락버튼
				AcceptButton.gameObject.SetActive(false);
				//메세지내용
				Lst_text_main[0].text = userChat.chatMsg;
			}
			else if (userChat.msgTp == ChatMessageType.Clan)
			{
				//말풍선
				image_ballon.sprite = ImageManager.instance.Get_Sprite(DefineKey.ChatIcon_clan);
				//수락버튼
				AcceptButton.gameObject.SetActive(false);
				//메세지내용
				Lst_text_main[0].text = userChat.chatMsg;
			}
			else if (userChat.msgTp == ChatMessageType.Whisper)
			{
				//말풍선
				image_ballon.sprite = ImageManager.instance.Get_Sprite(DefineKey.ChatIcon_friend);
				//수락버튼
				AcceptButton.gameObject.SetActive(false);
				//메세지내용
				Lst_text_main[0].text = userChat.chatMsg;
			}
			else if (userChat.msgTp == ChatMessageType.FriendlyMatch)
			{
				//말풍선
				image_ballon.sprite = ImageManager.instance.Get_Sprite(DefineKey.ChatIcon_vs);
				//수락버튼
				AcceptButton.gameObject.SetActive(true);
				//AcceptButton.onClick.AddListener(() => ResponseButton_frendlyMatch());
				//메세지내용
				Lst_text_main[0].text = TextDataManager.Dic_TranslateText[279]; // 친선전을 요청합니다
			}

			else if (userChat.msgTp == ChatMessageType.ClanMatch)
			{
				//말풍선
				image_ballon.sprite = ImageManager.instance.Get_Sprite(DefineKey.ChatIcon_clan);
				//수락버튼
				AcceptButton.gameObject.SetActive(true);
				AcceptButton.onClick.AddListener(() => ResponseButton_ClanMatch());
				//메세지내용
				Lst_text_main[0].text = TextDataManager.Dic_TranslateText[280]; // 클랜천 전 동참을 요청합니다
			}

			//클랜마크
			image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, _userChat.ClanMark));
			//보내는 이
			text_sender.text = _userChat.NkNm;
			
		}

		//시간 줄어들기..제한시간 이후에 팝업사라짐
		StartCoroutine(coroutine_ReduceTime());
		
	}


	void set_ObjMessage(int idx)
	{
		for (int i = 0; i < Lst_ObjMessage.Count; i++)
			Lst_ObjMessage[i].SetActive(i == idx);
	}

	//팝업 업애기
	void endChatPopup()
	{
		//StopCoroutine(coroutine_AlphaText());
		StopCoroutine(coroutine_ReduceTime());
		UI_Manager.Getsingleton.ClearUI(this);
	}


	IEnumerator coroutine_ReduceTime()
	{
		float _time = processStayTime;
		while (_time > 0)
		{

			_time -= Time.deltaTime;
			if (_time <= 0)
			{
				endChatPopup();
			}
			yield return null;
		}
	}


	
	public void ResponseButton_ClanMatch()
	{

        ////클랜전 매치를 시작한다 (요청 수락이니 매개변수에 요청자 유저아이디)
        //Link_Script.ins.Clan_Match_Start(userChat.UesrID);
		UserEditor.Getsingleton.EditLog(string.Format("클랜전 수락~~ 요청한유저 :{0}", userChat.UesrID));

		//매칭 창으로 이동한다.
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.MATCHING, _canvasTr);
		UI_Matching matching = UI_Manager.Getsingleton.Dic_UILst[UI.MATCHING] as UI_Matching;
		matching.Set_Info(MATCHTYPE.TWOVSTWO);

		//팝업삭제
		UI_Manager.Getsingleton.ClearUI(this);
	}
}
