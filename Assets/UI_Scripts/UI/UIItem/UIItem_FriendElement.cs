using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIItem_FriendElement : UI_Base 
{
	public User_Friends userFriend;

	//UI
	public Text text_lastestLogin;
	public Image image_clanMark;
	public Text text_userName;
	public GameObject Obj_TogetherDisable;

	public bool IsLogining = false;		//접속중이냐?

	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();

	}

	public void Set_friendInfo(User_Friends _userFriend)
	{
		userFriend = _userFriend;

		ApplyInfo();
	}

	void ApplyInfo()
	{
		//최종로그인
		Friend_LastestTime(userFriend.mtime);

		

		//유저이름
		text_userName.text = userFriend.NkNm;

		
		if (string.IsNullOrEmpty(userFriend.ClanName))
		{
			//클랜마크
			image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
		}
		else
		{
			//클랜마크
			image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, userFriend.ClanMark));
		}


		//함께하기 활성/비활성
		if (UserDataManager.instance.user.user_Users.UserID != userFriend.FrUserID) // 나자신이 아니면
		{
			if (userFriend.SvIdx == 0) //접속안함
			{
				IsLogining = false;
				Obj_TogetherDisable.SetActive(true);
			}
			else//접속함
			{
				IsLogining = true;
				Obj_TogetherDisable.SetActive(false);
			}
		}
		else // 나자신이 면 disable활성
			Obj_TogetherDisable.SetActive(true);


	}


	
	// 친구 최종로그인 시간 표시
	void Friend_LastestTime(DateTime lastestTime)
	{
		DateTime _lastTime = lastestTime;
		TimeSpan _timespan = TimeManager.Instance.Get_nowTime() - _lastTime;


		int sec = _timespan.Seconds;
		int min = (int)_timespan.TotalMinutes;
		int hour = min / 60;
		int day = hour / 24;

		if (day > 0)
			text_lastestLogin.text = string.Format("{0} {1} {2}", day, TextDataManager.Dic_TranslateText[181], TextDataManager.Dic_TranslateText[173]); // ~일전
		else if (hour > 0)
			text_lastestLogin.text = string.Format("{0} {1} {2}", hour, TextDataManager.Dic_TranslateText[180], TextDataManager.Dic_TranslateText[173]); // ~시간전
		else if (min > 0)
			text_lastestLogin.text = string.Format("{0} {1} {2}", min, TextDataManager.Dic_TranslateText[179], TextDataManager.Dic_TranslateText[173]); // ~분전
		else
			text_lastestLogin.text = string.Format("{0} {1} {2}", sec, TextDataManager.Dic_TranslateText[245], TextDataManager.Dic_TranslateText[173]); // ~초전
	}


	//친구 정보
	public void ResponseButton_FriendInfo()
	{
		webRequest.GetUserInfos(userFriend.FrUserID, callback_Complete_FriendInfo);
	}

	void callback_Complete_FriendInfo()
	{
		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);

		User _user = UserDataManager.instance.OtherUser;

		popup.Set_UserInfo(_user);
	}


	//친구랑 같이하기
	public void ResponseButton_Together()
	{
		User _user = UserDataManager.instance.user;

		if (_user.User_Units.ContainsKey(_user.User_useUnit.UnitIdx))
		{
			User_Units unit = _user.User_Units[_user.User_useUnit.UnitIdx];

			Network_MainMenuSoketManager.Getsingleton.Send_CTS_FriendRoomJoin(userFriend.FrUserID,
				(int)unit.Unitidx, unit.RefLv, (int)unit.MainWpnIdx, (int)unit.SubWpnIdx,
				unit.DecoIdx1, unit.DecoIdx2,unit.DecoIdx3, unit.SubSkill);
		}
		else
			UserEditor.Getsingleton.EditLog("don't have use unitidx");

	}



	// 친구 삭제 버튼
	public void ResponseButton_delete()
	{

		UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[268]); // 친구삭제
		popup.SetPopupMessage(string.Format("{0}\n{1}", userFriend.NkNm,TextDataManager.Dic_TranslateText[269]));
		popup.Set_addEventButton(callback_Complete_FriendDelete);
	}

	void callback_Complete_FriendDelete()
	{
		webRequest.FriendRemove(userFriend.FrUserID, UI_Friend.Getinstance.set_refresh);
		
	}


}
