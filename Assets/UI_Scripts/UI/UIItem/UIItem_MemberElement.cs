using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIItem_MemberElement : UI_Base 
{
	[HideInInspector]
	public Clan_members clan_member;
	public Image image_clanRank;
	public Text text_lastLogin;
	public Text text_memberName;
	public Text text_memberLv;
	public Button button_KickOut;
	public GameObject Obj_togetherDisable;


	private int leftTime = 0;
	private bool isMaster = false;
	public bool isLogining = false;			//로그인중이냐

	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();

		//ui 닫힐때 지난시간 체크하는 코루틴 중지하자
		//StopCoroutine(coroutine_GetLeftTime(DateTime.MinValue));

	}

	/// <summary>
	/// member element UI 값 설정하기
	/// </summary>
	public void Set_Element(Clan_members member , bool _ismaster)
	{
		clan_member = member;
		isMaster = _ismaster;

		//멤버 등급
		if (clan_member.CnRank == 2)
			image_clanRank.sprite = ImageManager.instance.Get_Sprite(DefineKey.Icon_master);
		else
			image_clanRank.gameObject.SetActive(false);

		//최종로그인
		LeftLoginTime(clan_member.mtime);


		//멤버이름
		text_memberName.text = clan_member.NkNm;

        //레벨
        text_memberLv.text = clan_member.UserLv.ToString() ;


        //같이하기 활성/비활성
        if (UserDataManager.instance.user.user_Users.UserID != member.UserID) // 나 자신 아니면
		{
			if (clan_member.SvIdx == 0) //접속안함
			{
				Obj_togetherDisable.SetActive(true);
				isLogining = false;
			}
			else  //접속함
			{
				Obj_togetherDisable.SetActive(false);
				isLogining = true;
			}
		}
		else// 나 자신이면 disable활성화
			Obj_togetherDisable.SetActive(true);

		//강퇴버튼 활성화 / 비활성화
		if (isMaster)
			button_KickOut.gameObject.SetActive(false);
		else
			button_KickOut.gameObject.SetActive(true);

	}

	
	



	void LeftLoginTime(DateTime lastestTime)
	{
		DateTime nowtime = TimeManager.Instance.Get_nowTime();

		TimeSpan _timespan = nowtime - lastestTime;

		int sec = _timespan.Seconds;
		int min = (int)_timespan.TotalMinutes;
		int hour = min / 60;
		int day = hour / 24;

		if (day > 0)
			text_lastLogin.text = string.Format("{0} {1} {2}", day, TextDataManager.Dic_TranslateText[181], TextDataManager.Dic_TranslateText[173]); // ~일전
		else if (hour > 0)
			text_lastLogin.text = string.Format("{0} {1} {2}", hour, TextDataManager.Dic_TranslateText[180], TextDataManager.Dic_TranslateText[173]); // ~시간전
		else if (min > 0)
			text_lastLogin.text = string.Format("{0} {1} {2}", min, TextDataManager.Dic_TranslateText[179], TextDataManager.Dic_TranslateText[173]); // ~분전
		else
			text_lastLogin.text = string.Format("{0} {1} {2}", sec, TextDataManager.Dic_TranslateText[245], TextDataManager.Dic_TranslateText[173]); // ~초전
	}




	public void ResponseButton_UserInfo()
	{
		if (UserDataManager.instance.user.user_Users.UserID == clan_member.UserID)
		{
			UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
			User _user = UserDataManager.instance.user;
			popup.Set_UserInfo(_user);
		}
		else
			webRequest.GetUserInfos(clan_member.UserID, callback_ClanUserInfo);

	}

	void callback_ClanUserInfo()
	{
		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);

		User _user = null;
		if (UserDataManager.instance.user.user_Users.UserID != clan_member.UserID) //선택한 유저가 나의유저가 아니라면
			_user = UserDataManager.instance.OtherUser;
		else
			_user = UserDataManager.instance.user;

		popup.Set_UserInfo(_user);
	}






	//같이하기
	public void ResponseButton_Together()
	{
		User _user = UserDataManager.instance.user;

		if (_user.User_Units.ContainsKey(_user.User_useUnit.UnitIdx))
		{
			User_Units unit = _user.User_Units[_user.User_useUnit.UnitIdx];

			Network_MainMenuSoketManager.Getsingleton.Send_CTS_FriendRoomJoin(clan_member.UserID,
				(int)unit.Unitidx, unit.RefLv, (int)unit.MainWpnIdx, (int)unit.SubWpnIdx,
				unit.DecoIdx1, unit.DecoIdx2,unit.DecoIdx3, unit.SubSkill);
		}
		else
			UserEditor.Getsingleton.EditLog("don't have use unitidx");
	}








	public void ResponseButton_KickOut()
	{
		if (UserDataManager.instance.user.MyUserClan_member.CnRank == 2)
		{
			if (isMaster)
			{
				UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[200]); // 클랜마스터는 강퇴 할 수 없습니다
			}
			else
			{
				UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[201]);
				popup.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[202], clan_member.NkNm)); // ~을 클랜에서 강퇴 하겠습니까?
				popup.Set_addEventButton(callback_kickOut);
			}
		}
		else
		{
			UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popupToast.SetPopupMessage(TextDataManager.Dic_TranslateText[203]);
		}
	}

	void callback_kickOut()
	{
		webRequest.ClanOut(clan_member.UserID, callback_Complte_kickout);
	}

	void callback_Complte_kickout()
	{
		UI_Clan.Getinstance.set_refresh();
		UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popupToast.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[204], clan_member.NkNm)); // ~ 클랜에 강퇴되었습니다

		
		
	}




	// 닫기 버튼시 element 들 닫기 or 삭제
	public void ResponseButton_Close(int tapIdx)
	{
		UI_Clan.Getinstance.ClearElementOne(tapIdx, this);
	}
}
 