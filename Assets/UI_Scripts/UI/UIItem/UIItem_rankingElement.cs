using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_rankingElement : UI_Base 
{
	private Rank_UnitExp rankingUnit;

	public Text text_Ranking;
	public Text text_UserName;
	public Image Image_clanMark;
	public Text text_Exp;
	public Text text_KD;
    public Text text_Lv;

	private RankingType rankingType;

	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();
	}

	// 리그랭킹 element 정보들 설정 (순위,유저이름,클랜이름,트로피)
	public void Set_elementInfo(Rank_UnitExp _rankingUnit, RankingType _type)
	{
		rankingUnit = _rankingUnit;
		rankingType = _type;

		//적용하자
		Apply_Element_private();
	}



	//개인 랭킹에대한 
	void Apply_Element_private()
	{
		//순위
		text_Ranking.text = rankingUnit.Ranking.ToString();

		if (rankingUnit.ClanName == "" || rankingUnit.ClanMark == 0) //클랜이 없다
		{
			//클랜마크
			Image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
			
		}
		else
		{
			//클랜마크
			Image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, rankingUnit.ClanMark));
			
		}

		//유저이름 
		text_UserName.text = rankingUnit.NkNm;

		//랭킹
		text_Ranking.text = rankingUnit.Ranking.ToString();

		// exp
		text_Exp.text = rankingUnit.Exp.ToString();

		//kd
		text_KD.text = rankingUnit.KillDeath.ToString();

        //lv 
        text_Lv.text = rankingUnit.UserLv.ToString(); //test;



    }



	public void ResponseButton_UserInfo()
	{
		User user = UserDataManager.instance.user;
		if (rankingUnit.UserID == user.user_Users.UserID)
		{
			UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
			popup.Set_UserInfo(user);
		}
		else
			webRequest.GetUserInfos(rankingUnit.UserID, callback_buttonUserInfo);
	}

	void callback_buttonUserInfo()
	{
		//선택한 유저가 나 자신인지 다른유저인지 판단 
		User _user = null;
		//if (rankingLeague.UserID == UserDataManager.instance.user.user_Users.UserID)
		//	_user = UserDataManager.instance.user;
		//else
			_user = UserDataManager.instance.OtherUser;


		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
		popup.Set_UserInfo(_user);
	}


}
