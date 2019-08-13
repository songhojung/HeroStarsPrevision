using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class UIItem_BoardElement : UI_Base 
{
	private Clan_Boards clan_board;
	private Clan_members clan_member;
	private byte clanRank;						 //본 유저의 클랜에서 멤버등급
	private string systemBoardtxt;				 // 시스템 내용
	private CLAN_BOARD_ELEMENT_TYPE BoardType = CLAN_BOARD_ELEMENT_TYPE.NONE;

	//UI
	public List<GameObject> Lst_BoardObj;
	public Image image_flag;
	public Text txt_UserName;
	public List<Text> Lst_txtInfo;



	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();
	}

	public Clan_Boards Get_Clanboard()
	{
		return clan_board;
	}


	/// <summary>
	/// board element에 구성할 정보 설정 (board 타입, 보드 변수, 게시판 인덱스, 클랜멤버등급)
	/// </summary>
	public void Set_Element(Clan_Boards _board)
	{
		clan_board = _board; //보드 데이터 저장


		//내용
		switch (clan_board.BodKind)
		{
			case (byte)CLANBOARD_TYPE.BOARD_CLANREGISTERED: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems;
				systemBoardtxt = string.Format("'{0}' {1} ", clan_board.NkNm, TextDataManager.Dic_TranslateText[183]); // ~ 클렌에 가입되었습니다
				break;
			case (byte)CLANBOARD_TYPE.BOARD_CLANOUT: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems;
				systemBoardtxt = string.Format("'{0}' {1} ", clan_board.NkNm, TextDataManager.Dic_TranslateText[158]); // ~ 클랜에 탈퇴하였습니다.
				break;
			case (byte)CLANBOARD_TYPE.BOARD_CLANBANNED: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems;
				systemBoardtxt = string.Format(TextDataManager.Dic_TranslateText[204], clan_board.NkNm ); //  ~가 클랜에 강퇴 되었습니다
				break;
			case (byte)CLANBOARD_TYPE.BOARD_CLANBUFFGOLD: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems;
				systemBoardtxt = string.Format("'{1}' " + TextDataManager.Dic_TranslateText[139], "GOLD BUFF", clan_board.NkNm); // ~가 ~를 구매하엿습니다
				break;
			case (byte)CLANBOARD_TYPE.BOARD_CLANBUFFEXP: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems;
				systemBoardtxt = string.Format("'{1}' " + TextDataManager.Dic_TranslateText[139], "EXP BUFF", clan_board.NkNm); // ~가 ~를 구매하엿습니다
				break;
			case (byte)CLANBOARD_TYPE.BOARD_CLANREQUEST: BoardType = CLAN_BOARD_ELEMENT_TYPE.Systems; break;
			case (byte)CLANBOARD_TYPE.BOARD_USER: BoardType = CLAN_BOARD_ELEMENT_TYPE.Member; break;
			default:
				break;
		}

		//해당 타입 오브젝트 활성
		Active_BoardObject(BoardType);

		//게시 정보 설정
		Set_boardInfo(BoardType);
			
	}



	//게시물 정보 설정
	void Set_boardInfo(CLAN_BOARD_ELEMENT_TYPE bt)
	{
		if (bt == CLAN_BOARD_ELEMENT_TYPE.Systems)
		{
			//게시글 내용
			Lst_txtInfo[(int)CLAN_BOARD_ELEMENT_TYPE.Systems].text = systemBoardtxt;
		}
		else if (bt == CLAN_BOARD_ELEMENT_TYPE.Member)
		{
			//유저이름
			txt_UserName.text = clan_board.NkNm;

			//국가마크
			//image_flag.sprite = 

			//게시글
			if (clan_board.BodTxt.Length >= 25)
			{
				if (!clan_board.BodTxt.Contains("\n"))
					clan_board.BodTxt = clan_board.BodTxt.Insert(25, "\n");
			}
			
			Lst_txtInfo[(int)CLAN_BOARD_ELEMENT_TYPE.Member].text = clan_board.BodTxt;
		}

	}

	//해당 타입 게시판 오브젝트 활성화
	void Active_BoardObject(CLAN_BOARD_ELEMENT_TYPE boardType)
	{
		for (int i = 0; i < Lst_BoardObj.Count; i++)
			Lst_BoardObj[i].SetActive(i == (int)boardType);
	}		






	//유저정보
	public void ResponseButton_UserInfo()
	{
		webRequest.GetUserInfos(clan_board.UsetID, callback_buttonUserInfo);
	}

	void callback_buttonUserInfo()
	{

		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
		User user = UserDataManager.instance.OtherUser;
		popup.Set_UserInfo(user);
	}

	
	


}

