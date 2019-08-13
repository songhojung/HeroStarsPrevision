using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_ClanListElement : UI_Base
{
	private Clan_Lists clanList;

	public Text text_ClanName;
	public Image image_ClanMark;
	public Text text_ClanMasterName;
	public Text text_ClanMemberCount;

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

	public void Set_ClanElementInfo(Clan_Lists _clanlist)
	{
		clanList = _clanlist;

		//클래마크
		image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, clanList.ClanMark));
		//클랜이름
		text_ClanName.text = clanList.ClanName;

		//클랜마스터
		text_ClanMasterName.text = clanList.NkNm;

		//클랜인원
		text_ClanMemberCount.text = string.Format("{0}/{1}",clanList.MemberCnt,clanList.PersonCnt);
	}


	//클랜정보 보기
	public void ResponseButton_Claninfo()
	{
		webRequest.SearchUserClan(3, clanList.ClanID.ToString(), callback_complete_ClanInfo);

	}

	void callback_complete_ClanInfo()
	{
		UI_Popup_ClanInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ClanInfo>(UIPOPUP.POPUPCLANINFO);
		User _user = null;
		if (clanList.ClanID == UserDataManager.instance.user.clan_Clans.ClanID)
			_user = UserDataManager.instance.user;
		else
			_user = UserDataManager.instance.OtherUser;
		popup.Set_ClanInfo(_user.clan_Clans, _user.Clan_members);
	}
		

}
