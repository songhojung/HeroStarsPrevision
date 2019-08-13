using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_UserInfo : UI_PopupBase 
{
    public enum UserInfoButtonKind { AddFriend = 0,InviteClan,}

	private User user;
	private Clan_members clan_member;
	private bool isOtherUser = false;			// 다른유저이냐



	//UI
	public Text text_UserName;
	public Text text_UserID;
	public Text text_clanName;
	public Image image_clanMark;
    public Text text_lv;
    public Text text_Exp;
    public Text text_Kd;
    public Text text_WinRate;

    //비활성이미지
    public List<GameObject> Lst_Button;




	

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

		if (isOtherUser)
			webRequest.GetUserInfos(user.user_Users.UserID, callback_RefreshUserInfo);
		else
			callback_RefreshUserInfo();


	}

	void callback_RefreshUserInfo()
	{
		User _user = null;
		if (isOtherUser) //선택한 유저가 나의유저가 아니라면
			_user = UserDataManager.instance.OtherUser;
		else
			_user = UserDataManager.instance.user;
		Set_UserInfo(_user);
	}

	/// <summary>
	/// 팝업띄울유저정보 (요청한유저정보, 유저랭킹, 현재 이유저가 클랜요청을 했는가)
	/// </summary>
	public void Set_UserInfo(User _user)
	{
		user = _user;

		if (user.user_Users.UserID != 0)
		{
			if (user.user_Users.UserID == UserDataManager.instance.OtherUser.user_Users.UserID)
				isOtherUser = true;
			else
				isOtherUser = false;
		}

	
		
		
		//유저정보 적용
		Apply_UserInfo();

	

		//정보에 따라 버튼활성 또는 비활성체크
		Chk_ButtonState();

	}

	void Apply_UserInfo()
	{
		// 유저이름
		text_UserName.text = user.user_Users.NkNm;

		//유저아이디
		text_UserID.text = user.user_Users.UserID.ToString();

        //클랜
        if (user.user_Clans.ClanID != 0)
        {
            //클랜이름
            text_clanName.text = user.user_Users.ClanName;


            //클랜마크
            image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, user.user_Users.ClanMark));

        }
        else
        {
            text_clanName.text = "----";

            //클랜마크
            image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
        }

        //레벨
        text_lv.text = user.Get_user_goods(ITEMTYPE.LV).ToString();

        //경험치
        text_Exp.text = user.Get_user_goods(ITEMTYPE.EXP).ToString();

        //kd
        int allKillCnt = 0;
        int allCnt = 0;
        for (int i = 0; i < user.User_useUnit.UnitIdxs.Length; i++)
        {
            if(user.User_unit_useInfos.ContainsKey(user.User_useUnit.UnitIdxs[i]))
            {
                allKillCnt += (int)user.User_unit_useInfos[user.User_useUnit.UnitIdxs[i]].KillCnt;
                allCnt += (int)user.User_unit_useInfos[user.User_useUnit.UnitIdxs[i]].UseCnt;
            }
        }

       
        float kd = 0;
        if (allCnt <= 0) kd = 0;
        else
            kd = (float)allKillCnt / (float)allCnt;

        text_Kd.text = string.Format("{0:f2}", kd);


        //승률
        int win = user.Get_user_goods(ITEMTYPE.WIN);
        int lose = user.Get_user_goods(ITEMTYPE.LOSE);
        int total = win + lose;
        float winRate = 0;
        if (total <= 0) winRate = 0;
        else
            winRate = (float)win / (float)total;

        text_WinRate.text = string.Format("{0:f2}%", winRate * 100f);


    }


    //정보에 따라 버튼활성 또는 비활성체크
    void Chk_ButtonState()
	{
		// 나 자신의 정보면 활성화
		if (!isOtherUser)
		{
            Lst_Button[(int)UserInfoButtonKind.AddFriend].SetActive(false);
            Lst_Button[(int)UserInfoButtonKind.InviteClan].SetActive(false);

        }
        else
		{
			
			User MyUser = UserDataManager.instance.user;
			//클랜마스터이면 클랜초대버튼 활성화
			if (user.user_Clans.ClanID == 0 && MyUser.MyUserClan_member.CnRank == 2)
			{
                Lst_Button[(int)UserInfoButtonKind.InviteClan].SetActive(true);

			}
			else
                Lst_Button[(int)UserInfoButtonKind.InviteClan].SetActive(false);

            //친구추가가 되어있지 않으면 친구추가버튼 활성화
            if (MyUser.user_Users.UserID == user.user_Users.UserID)
                Lst_Button[(int)UserInfoButtonKind.AddFriend].SetActive(false);
            else if (MyUser.User_Friends.Count > 0)
			{
                foreach (var fr in MyUser.User_Friends)
                {
                    if (fr.Value.FrUserID == user.user_Users.UserID)
                    {
                        Lst_Button[(int)UserInfoButtonKind.AddFriend].SetActive(false);

                        break;
                    }
                    else
                        Lst_Button[(int)UserInfoButtonKind.AddFriend].SetActive(true);

                }
			}
			else
                Lst_Button[(int)UserInfoButtonKind.AddFriend].SetActive(true);
        }
	}



	//친구추가하기 함수
	public void ResponseButton_AddFriend()
	{
		if(isOtherUser)
			webRequest.FriendAdd(user.user_Users.UserID, "", callback_Complte_AddFriend);
	}

	//친구추가 완료시 콜백함수
	void callback_Complte_AddFriend()
	{
		webRequest.FriendList(null);

		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.SetPopupMessage(string.Format("{0}  {1}", user.user_Users.NkNm, "friend have been added"));

		//팝업창닫기
		ResponseButton_Close();

		//친구리스트 갱신
		if(UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.FRIEND))
			UI_Friend.Getinstance.set_refresh();
	}



	public void ResponseButton_inviteClan()
	{
		webRequest.ClanInvite(user.user_Users.UserID, callback_Complete_inviteClan);


		UI_Manager.Getsingleton.ClearUI(UIPOPUP.POPUPUSERINFO);

	}

	void callback_Complete_inviteClan()
	{

		// top UI 갱신하자 
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh(); 

		UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popupToast.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[214], user.user_Users.NkNm)); // {0}님을 클랜에 초대하였습니다.
	}
	
	

	public void ResponseButton_Close()
	{
		
		UI_Manager.Getsingleton.ClearUI(UIPOPUP.POPUPUSERINFO);
	}

	
}
