using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIItem_newsElement : UI_Base 
{
	public enum newsButton_TYPE
	{
		Dedail = 0,
		Accept = 1,
		Supply = 2,
		Delete = 3,
	}


	[HideInInspector]
	public User_Posts user_post;
	[HideInInspector]
	public Infos_Notice infos_notice;
	private NEWSTYPE newsType = NEWSTYPE.WHOLE;
	[HideInInspector]
	public NewsToggle_Type toggleType = NewsToggle_Type.NONE;

	public DateTime ctime;		 // sorting 을위한 시간변수

	public Button button_Element;			//첨부물받기를 위한 버튼 첨부물일때만 버튼이벤트추가

	//소식UI 부분
	public Text text_remainday;
	public Text text_newsMain;
	public Text text_sender;
	public List<GameObject> Lst_ButtonObj = new List<GameObject>();

	// 선물버튼 UI부분
	public Image Image_supply;
	public Text text_supply;
	public Text text_supplyItemNum;

		

	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();

	}

	public override void set_refresh()
	{
		base.set_refresh();

	}


	// 선물 , 알림 대한 구성설정
	public void Set_ElementInfo(User_Posts _userPost)
	{
		//newsType = _type;
		user_post = _userPost;

		ctime = user_post.ctime;
		Set_NewsType(_userPost);
		Apply_element(newsType);

		// element 의 어느 토글키 소속인지 알도록 설정
		Set_ToggleType(newsType);

	}

	// 공지사항 대한 구성설정
	public void Set_ElementInfo(Infos_Notice _notice)
	{
		//newsType = _type;
		infos_notice = _notice;

		ctime = infos_notice.ctime;
		Set_NewsType(_notice);
		Apply_element(newsType);


		// element 의 어느 토글키 소속인지 알도록 설정
		Set_ToggleType(newsType);
	}

	void Set_NewsType(User_Posts _userPost)
	{
		if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinRquestMsg)
			newsType = NEWSTYPE.REQUEST;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinInviteMsg)
			newsType = NEWSTYPE.REQUEST;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.GiftFromOperator)
		{
			if (!string.IsNullOrEmpty(_userPost.PostTxt) && (_userPost.ItTp == ITEMTYPE.NONE))
				newsType = NEWSTYPE.INFORM;
			else
				newsType = NEWSTYPE.SUPPLY;
		}
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.GiftBonusFromInAppPurchase)
			newsType = NEWSTYPE.SUPPLY;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinSucces)
			newsType = NEWSTYPE.INFORM;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.CompleteRwdTutorial)
			newsType = NEWSTYPE.SUPPLY;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.Monthly)
			newsType = NEWSTYPE.SUPPLY;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.ClanKickMsg)
			newsType = NEWSTYPE.INFORM;
		else if (_userPost.PstPasIdx == (byte)PostIdx_TYPE.AttdReward)
			newsType = NEWSTYPE.SUPPLY;
	}

	void Set_NewsType(Infos_Notice _notice)
	{
		newsType = NEWSTYPE.INFORM;
	}

	
	// element 의 어느 토글키 소속인지 알도록 설정
	void Set_ToggleType(NEWSTYPE _type)
	{
		if (_type == NEWSTYPE.INFORM || newsType == NEWSTYPE.REQUEST)
			toggleType = NewsToggle_Type.NORMAL;
		else if (_type == NEWSTYPE.SUPPLY)
			toggleType = NewsToggle_Type.ATTACH;
	}

	public void Apply_element(NEWSTYPE _type)
	{


		//=====================우편물
		if (user_post != null)  
		{

			//메인 내용
			if (!string.IsNullOrEmpty(user_post.PostTxt))
			{
				if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinInviteMsg)
					text_newsMain.text = string.Format("{0} {1}", user_post.PostTxt, TextDataManager.Dic_TranslateText[182]); // ~클랜에 초대합니다
			
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.GiftFromOperator)
					text_newsMain.text = user_post.PostTxt;
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanKickMsg)
				{
					if (user_post.PostTxt == "0")
						text_newsMain.text = TextDataManager.Dic_TranslateText[288]; //게임초대 보상
					else
						text_newsMain.text = string.Format(TextDataManager.Dic_TranslateText[288], user_post.PostTxt); //게임초대 보상 ~회
				}
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.Monthly)
				{
					text_newsMain.text = string.Format("{0}{1}{2}, {3}{4}{5}",TableDataManager.instance.Infos_shops[301].SellItNum
						,TextDataManager.Dic_TranslateText[181],TextDataManager.Dic_TranslateText[307]
						,TextDataManager.Dic_TranslateText[176],user_post.PostTxt, TextDataManager.Dic_TranslateText[181]); //25일패키지 보상,남은기간 ~일
				}
				
			}
			else
			{
				if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinInviteMsg)
					text_newsMain.text = TextDataManager.Dic_TranslateText[262];
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinRquestMsg)
					text_newsMain.text = string.Format("{0}",TextDataManager.Dic_TranslateText[205]); // ~클랜 가입을 요청합니다
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.GiftFromOperator)
					text_newsMain.text = TextDataManager.Dic_TranslateText[263];
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.GiftBonusFromInAppPurchase)
					text_newsMain.text = TextDataManager.Dic_TranslateText[264];
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanJoinSucces)
					text_newsMain.text = string.Format("{0} {1}", user_post.SndNkNm, TextDataManager.Dic_TranslateText[183]); // ~클랜에 가입이 되었습니다
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.CompleteRwdTutorial)
					text_newsMain.text = TextDataManager.Dic_TranslateText[267];
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.ClanKickMsg)
					text_newsMain.text = string.Format(TextDataManager.Dic_TranslateText[204], user_post.SndNkNm); //~클랜에 강퇴되었습니다.
				else if (user_post.PstPasIdx == (byte)PostIdx_TYPE.AttdReward)
				{
					text_newsMain.text = string.Format(TextDataManager.Dic_TranslateText[287]);//출석보상
				}
				
			}
		
				//보낸사람
				text_sender.text = user_post.SndNkNm.ToString();

				// 남은 시간
				if (newsType == NEWSTYPE.SUPPLY) //선물만 남은시간 표시 
					RemainDay(user_post.ctime, user_post.ExpiryDay, text_remainday);
				else
					text_remainday.gameObject.SetActive(false);
				//StartCoroutine(routine_RemainDay(user_post.ctime, text_remainday));
		}
		//========================== notice 
		else if (infos_notice != null)
		{
			//메인내용
			text_newsMain.text = infos_notice.NotiText;

			//보낸사람
			text_sender.text = infos_notice.NotiTitle;
			// 남은 시간
			StartCoroutine(routine_RemainDay(infos_notice.Noticetime, text_remainday));
		}
			

		

		//버튼 활성화 
		Set_ButtonObj(_type);

	}

	void Set_ButtonObj(NEWSTYPE newsType)
	{
		if (newsType == NEWSTYPE.INFORM)
			Lst_ButtonObj[(int)newsButton_TYPE.Delete].SetActive(true); // 삭제버튼만 active
		else if (newsType == NEWSTYPE.REQUEST)
		{
			Lst_ButtonObj[(int)newsButton_TYPE.Dedail].SetActive(true); // 정보 버튼만 active
			Lst_ButtonObj[(int)newsButton_TYPE.Accept].SetActive(true); // 수락 버튼만 active
			Lst_ButtonObj[(int)newsButton_TYPE.Delete].SetActive(true); // 삭제버튼만 active
		}
		else if (newsType == NEWSTYPE.SUPPLY)
		{
			Lst_ButtonObj[(int)newsButton_TYPE.Supply].SetActive(true); // 수령버튼만 active

			if (user_post.ItTp == ITEMTYPE.GEM)
			{
				text_supply.text = TextDataManager.Dic_TranslateText[37];
				Image_supply.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}", DefineKey.Gem));
			}
			else if (user_post.ItTp == ITEMTYPE.GOLD)
			{
				text_supply.text = TextDataManager.Dic_TranslateText[36];
				Image_supply.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}", DefineKey.Gold));
			}
			else if (user_post.ItTp == ITEMTYPE.ITEMBOX)
			{
				if (user_post.ItIdx == (uint)SUPPLYBOX_TYPE.BronzeSupply)
					text_supply.text = TextDataManager.Dic_TranslateText[1001]; // 일반상자
				else if (user_post.ItIdx == (uint)SUPPLYBOX_TYPE.SilverSupply)
					text_supply.text = TextDataManager.Dic_TranslateText[1002]; // 고급상자
				else if (user_post.ItIdx == (uint)SUPPLYBOX_TYPE.GoldSupply)
					text_supply.text = TextDataManager.Dic_TranslateText[1003]; //최고급상자
				Image_supply.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.boxIcon, user_post.ItIdx));
			}
			else if (user_post.ItTp == ITEMTYPE.UNIT)
			{
				if (TableDataManager.instance.Infos_units.ContainsKey(user_post.ItIdx))
					text_supply.text = TableDataManager.instance.Infos_units[user_post.ItIdx].UnitName;
				Image_supply.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.boxIcon, 0));
			}
			else if (user_post.ItTp == ITEMTYPE.EXP)
			{
				text_supply.text = TextDataManager.Dic_TranslateText[177];
				Image_supply.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}", DefineKey.Exp));
			}

			//선물일떄 element 본체에 버튼이벤트부여
			button_Element.onClick.AddListener(ResponseButton_Gift);

			text_supplyItemNum.text = string.Format("x {0}", user_post.ItNum);
			Image_supply.SetNativeSize();
		}
	}




	//남은 날 표시하는 코루틴
	IEnumerator routine_RemainDay(DateTime ctime, Text _text_reaminday)
	{

		while (true)
		{
			TimeSpan _timespan = ctime - TimeManager.Instance.Get_nowTime();
			int day = (int)_timespan.TotalDays;

			if (day > 0)
				_text_reaminday.text = string.Format("{0}{1}", day, TextDataManager.Dic_TranslateText[181]); // ~일
			else
				_text_reaminday.text = string.Format("1{0}",TextDataManager.Dic_TranslateText[181]); //~일

			yield return null;
		}

	}

	//남은 날 표시하는 함수
	void RemainDay(DateTime ctime, byte expireDay, Text _text_reamainday)
	{
		TimeSpan expire = new TimeSpan(expireDay, 0, 0, 0, 0);
		DateTime untilTime = ctime + expire;
		TimeSpan _timespan = untilTime - TimeManager.Instance.Get_nowTime();

		int day = (int)_timespan.TotalDays;

		if (day > 0)
			_text_reamainday.text = string.Format("{0}{1}", day, TextDataManager.Dic_TranslateText[181]); // ~일
		else
			_text_reamainday.text = string.Format("1{0}", TextDataManager.Dic_TranslateText[181]); //~일
	}



	//정보 버튼
	public void ResponseButton_detail()
	{
		if (user_post.PstPasIdx == (int)PostIdx_TYPE.ClanJoinInviteMsg)
			webRequest.SearchUserClan(3, user_post.SndClanID.ToString(), callback_complete_detail);
		else if(user_post.PstPasIdx == (int)PostIdx_TYPE.ClanJoinRquestMsg)
			webRequest.SearchUserClan(4, user_post.SndUserID.ToString(), callback_complete_detail);
	}

	void callback_complete_detail()
	{
		if (user_post.PstPasIdx == (int)PostIdx_TYPE.ClanJoinInviteMsg)
		{
			UI_Popup_ClanInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ClanInfo>(UIPOPUP.POPUPCLANINFO);
			User user = UserDataManager.instance.OtherUser;
			popup.Set_ClanInfo(user.clan_Clans, user.Clan_members);
		}
		else if (user_post.PstPasIdx == (int)PostIdx_TYPE.ClanJoinRquestMsg)
		{
			UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
			User user = UserDataManager.instance.OtherUser;
			popup.Set_UserInfo(user);
		}
	}



	//삭제 버튼
	public void ResponseButton_delete()
	{
		webRequest.PostRecv(user_post.PostIdx, (uint)user_post.ctime.Month, false, callback_complete_delete);
	}

	void callback_complete_delete()
	{
		UI_News.Getsingleton.ClearElement_One(this);
	}


	//선물 버튼
	public void ResponseButton_Gift()
	{
		//int m = user_post.ctime.Month;
		webRequest.PostRecv(user_post.PostIdx, (uint)user_post.ctime.Month,false, callback_complete_Gift);
	}

	void callback_complete_Gift()
	{
		UI_News.Getsingleton.ClearElement_One(this);

		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.SetPopupMessage(TextDataManager.Dic_TranslateText[250]); // 선물 수령을 완료 하였습니다

		//UI_Popup_GainItem popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_GainItem>(UIPOPUP.POPUPGAINITEM);
		//popup.Set_GainPopup();

		//top ui 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();
		
	}

	void callback_completeShowing()
	{
		UI_News.Getsingleton.ClearElement_One(this);

		//레업 됫을시 렙업 연출을 위함
		User _user = UserDataManager.instance.user;
		if (_user.Dic_Showing.ContainsKey(ITEMTYPE.LV))
		{
			UI_Popup_Showing popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Showing>(UIPOPUP.POPUPSHOWINGLEVEL);
			popup.set_showingInfo(_user.Dic_Showing[ITEMTYPE.LV], ShowingKind.UserlvUp);

			_user.Dic_Showing.Remove(ITEMTYPE.LV);
		}
	}
	

	//초대수락 버튼
	public void ResponseButton_AcceptInvite()
	{
		webRequest.PostRecv(user_post.PostIdx, (uint)user_post.ctime.Month, true ,callback_Complete_Acceptinvite);
	}

	

	void callback_Complete_Acceptinvite()
	{

		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 알림
		popup.SetPopupMessage(string.Format("{0} {1}.", user_post.PostTxt,TextDataManager.Dic_TranslateText[183]));
		popup.Set_addEventButton(() => webRequest.ClanInfo(callback_Clear_OverlapedPost));
		
	}



	void callback_Clear_OverlapedPost()
	{
		UI_News.Getsingleton.Overlapedpost_UserID = user_post.SndUserID;
		UI_News.Getsingleton.OverlapedPost = user_post;
		UI_News.Getsingleton.Clear_OverlapedPosts(NEWSTYPE.REQUEST);
	}


	
}
