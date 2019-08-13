using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct InviteInfo
{
	public uint userID;
	public string Name;
	public ushort ClanMark;
	public string countryCode;	  //국가코드
	public byte SvIdx; // 접속서버 0이면 접속안함
	public Invite_Type inviteType;

	public InviteInfo(uint _userID, string _name, ushort _clanMark, string _ctCd, byte _svIdx, Invite_Type _inviteType)
	{
		userID = _userID;
		Name = _name;
		ClanMark = _clanMark;
		countryCode = _ctCd;
		SvIdx = _svIdx;
		inviteType = _inviteType;
	}
}

public class UIItem_GameInviteElement : UI_Base 
{

	private InviteInfo st_inviteInfo;

	public Image image_ClanMark;
	public Text text_userName;
	public GameObject DisableInvite;


	public bool isLogining = false;

	public override void set_Open()
	{
		base.set_Open();
	}
	public override void set_Close()
	{
		base.set_Close();
	}

	public void Set_inviteInfo(InviteInfo inviteInfo)
	{
		st_inviteInfo = inviteInfo;

		image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, st_inviteInfo.ClanMark));
		text_userName.text = st_inviteInfo.Name;


		//우선 disable 끄자
		if (inviteInfo.SvIdx == 0)
		{
			DisableInvite.SetActive(true);  // 접속 안했으니 disable 활성
			isLogining = false;
		}
		else 
		{
			User _user = UserDataManager.instance.user;
				if(inviteInfo.SvIdx == _user.User_userGameServer.SubIdx) //이친구가 현재 자신이랑 같은 서버녀
					DisableInvite.SetActive(false); // 같은서버이면 disable 비활성
				else
					DisableInvite.SetActive(true); // 같은서버 아니면 disable 활성

				isLogining = true;
		}
	


	}


	public void ResponseButton_GameInvite()
	{
		//팝업
		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.SetPopupMessage(string.Format("{0} 에게 초대를 보냈습니다", st_inviteInfo.Name));


		//초대보내기 send
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_InviteRoom(st_inviteInfo.userID, st_inviteInfo.Name);
	}

}
