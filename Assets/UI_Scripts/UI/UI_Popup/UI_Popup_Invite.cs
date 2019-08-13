using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public enum Invite_Type
{
	NONE = 99,
	Friend = 0,
	Clan = 1,
}
public class UI_Popup_Invite : UI_PopupBase 
{


	public List<Toggle> List_Toggle;
	public List<InputField> Lst_inputField;
	public List<GameObject> Lst_ScrollListObj;
	public List<Transform> Lst_ContentTr;

	private List<UIItem_GameInviteElement> Lst_friendMember = new List<UIItem_GameInviteElement>();
	private List<UIItem_GameInviteElement> Lst_clanMember = new List<UIItem_GameInviteElement>();
	private Invite_Type nowTapType	= Invite_Type.Friend;
	private string findName = string.Empty;


	public override void Set_Open()
	{
		base.Set_Open();



		//초기세팅
		Set_InviteList();	
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public override void Set_Refresh()
	{
		base.Set_Refresh();
	}


	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public void Set_InviteList()
	{

		//Clear_Element();

		//보여질 스크롤 리스트오브젝트 활성화하기
		Active_ScrollListObject(nowTapType);

		// 초대  element 생성 및 체크
		Chk_InviteInfo(nowTapType);

	}

	void Chk_InviteInfo(Invite_Type inviteType)
	{
		User _user = UserDataManager.instance.user;

		if (inviteType == Invite_Type.Friend)
		{
			if (Lst_friendMember.Count <= 0)
			{
					webRequest.FriendList(() => Creat_InviteElement(inviteType));
			
			}
		}
		else if (inviteType == Invite_Type.Clan)
		{
			if (Lst_clanMember.Count <= 0)
			{

				if (_user.clan_Clans.ClanID != 0)
				{
				
						webRequest.ClanInfo(() => Creat_InviteElement(inviteType));
				

				}
			}
		}
	}


	// 초대  element 생성 및 체크
	void Creat_InviteElement(Invite_Type inviteType)
	{
		User _user = UserDataManager.instance.user;

		if (inviteType == Invite_Type.Friend)
		{
			foreach (var fr in _user.User_Friends)
			{
				UIItem_GameInviteElement ele = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_GAMEINVITEELEMENT, 
					Lst_ContentTr[(int)Invite_Type.Friend]) as UIItem_GameInviteElement;

				ele.Set_inviteInfo(new InviteInfo(fr.Value.FrUserID, fr.Value.NkNm, fr.Value.ClanMark, "kr", fr.Value.SvIdx,inviteType));
				Lst_friendMember.Add(ele);
			}

			//친구목록 정렬
			Sort_Invitelist(Lst_friendMember);
		}
		else if (inviteType == Invite_Type.Clan)
		{
			//존재 하는 멤버만 추출
			var existMember = from mem in _user.Clan_members
							  where string.IsNullOrEmpty(mem.Value.NkNm) == false || mem.Value.UserID != 0 //닉네임 없거나 아이디가 0이면 제외
							  select mem;

			foreach (var clanMember in existMember)
			{
				if (clanMember.Value.UserID == UserDataManager.instance.user.user_Users.UserID) continue;

				UIItem_GameInviteElement ele = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_GAMEINVITEELEMENT,
					Lst_ContentTr[(int)Invite_Type.Clan]) as UIItem_GameInviteElement;

				ele.Set_inviteInfo(new InviteInfo(clanMember.Value.UserID, clanMember.Value.NkNm, _user.clan_Clans.ClanMark, clanMember.Value.CtrCd, clanMember.Value.SvIdx, inviteType));
				Lst_clanMember.Add(ele);
			}

			//클랜목록 정렬
			Sort_Invitelist(Lst_clanMember);
		}

	}


	// 목록 솔팅하기 
	void Sort_Invitelist(List<UIItem_GameInviteElement> lst_invite)
	{
		if (lst_invite.Count > 0)
		{
			var sortlst = from br in lst_invite
						  orderby br.isLogining
						  select br;

			for (int i = 0; i < sortlst.ToList().Count; i++)
			{
				sortlst.ToList()[i].transform.SetAsFirstSibling();
			}
		}
	}


	//보여질 스크롤 리스트오브젝트 활성화하기
	void Active_ScrollListObject(Invite_Type inviteType)
	{
		for (int i = 0; i < Lst_ScrollListObj.Count; i++)
			Lst_ScrollListObj[i].SetActive(i == (int)inviteType);
	}


	void Clear_Element()
	{
		for (int i = 0; i < Lst_clanMember.Count; i++ )
		{
			Destroy(Lst_clanMember[i].gameObject);
		}
		Lst_clanMember.Clear();


		for (int i = 0; i < Lst_friendMember.Count; i++)
		{
			Destroy(Lst_friendMember[i].gameObject);
		}
		Lst_friendMember.Clear();

	}



	public void ResponseInput_End(int inputIdx)
	{
		findName = Lst_inputField[inputIdx].text;

		

	}


	public void ResponseToggle_Tap(int tapIdx)
	{
		if(nowTapType != (Invite_Type)tapIdx)
		{
			nowTapType = (Invite_Type)tapIdx;

			Set_InviteList();
		}
	}

	public void Response_findUser(int inputIdx)
	{
		
		
			if (inputIdx == 0)//아이디 찾기
				webRequest.SearchUserClan(4, findName, callback_InviteForFindingUser);
			else if (inputIdx == 1) // 이름찾기
				webRequest.SearchUserClan(1, findName, callback_InviteForFindingUser);

			Debug.Log(findName);
			findName = string.Empty;
			Lst_inputField[inputIdx].text = string.Empty;

			
		
	
	}



	void callback_InviteForFindingUser()
	{
		User otheruser = UserDataManager.instance.OtherUser;

		//팝업
		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[317], otheruser.user_Users.NkNm)); // ~를 초대하엿습니다

		//초대보내기 send
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_InviteRoom(otheruser.user_Users.UserID, otheruser.user_Users.NkNm);
	}

	public void ResponseButton_Close()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	}
}
