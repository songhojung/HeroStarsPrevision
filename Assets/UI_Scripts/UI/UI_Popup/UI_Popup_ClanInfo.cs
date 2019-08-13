using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_Popup_ClanInfo : UI_PopupBase 
{
	private Clan_Lists clanInfo;
	private Clans clans;
	private Dictionary<byte, Clan_members> Dic_clanMembers = new Dictionary<byte, Clan_members>();

	public Text text_clanName;
	public Text text_personCnt;
	public Text text_clanIntro;
	public Image image_clanMark;

	//spec
	public Text text_clanMasterName;
	public Text text_clanID;

	//
	public GameObject Obj_Disable;


	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}



	/// <summary>
	/// 클랜찾기 에서 클랜정보볼떄
	/// </summary>
	public void Set_ClanInfo(Clans _clans, Dictionary<byte,Clan_members> _clan_members)
	{
		clans = _clans;
		Dic_clanMembers = _clan_members;
		

		Apply_ClanInfo();

		Check_userClan();
	}



	private void Apply_ClanInfo()
	{
		//클랜 마크
		image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, clans.ClanMark));

		//클랜 이름
		text_clanName.text = clans.ClanName;

		//클랜원 수
		text_personCnt.text = string.Format("{0}/{1}", Get_ClanMemberCount(Dic_clanMembers), clans.PersonCnt);

		//클랜 소개글
		text_clanIntro.text = clans.ClanInfoTxt;


		//클랜아이디
		text_clanID.text = clans.ClanID.ToString();


	
		//클랜마스터
		var ClanMaster = Dic_clanMembers.Where(n => n.Value.CnRank == 2);
		foreach (var master in ClanMaster)
		{
			text_clanMasterName.text = master.Value.NkNm;
		}

	
	}

	private void Check_userClan()
	{
		User user = UserDataManager.instance.user;
		if (user.user_Clans.ClanID == 0)
			Obj_Disable.SetActive(false);
		else
			Obj_Disable.SetActive(true);
	}

	int Get_ClanMemberCount(Dictionary<byte, Clan_members> _clan_members)
	{
		var existMember = from member in _clan_members
						  where member.Value.NkNm != "" || member.Value.UserID != 0 //닉네임 없거나 아이디가 0이면 제외
						  select member;


		return existMember.Count();
	
	}

	public void ResponseButton_Close()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	
	}

	public void ResponseButton_joinClan()
	{
		webRequest.ClanJoin(clans.ClanID, callback_JoinClan);
		UI_Manager.Getsingleton.ClearUI(this);
	}

	void callback_JoinClan()
	{
		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); //알림
		popup.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[164], clans.ClanName));//~ 클랜에 가입요청을 하였습니다.가입요청이 수락이 되면 클랜에 가입이 됩니다"
		popup.Set_addEventButton(() => UI_Top.Getsingleton.set_refresh());
	}

}
