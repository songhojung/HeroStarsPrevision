using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_StartGame : UI_PopupBase 
{

	public override void Set_Open()
	{
		base.Set_Open();

		//로비캐릭 회전 잠금
		User.isSelectedCharacter = true;
	}
	public void Set_addEventYESButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}


	public void ResponseToggle_OpenFlag()
	{

	}

	public void ResponseButton_CreateRoom()
	{
		UI_Manager.Getsingleton.ClearUI(this);

		Network_MainMenuSoketManager.Getsingleton.Send_CTS_RoomMake();
		
	}

	
	
	//퀵조인시도
	public void ResponseButton_QuickJoin()
	{
			UI_Manager.Getsingleton.ClearUI(this);

			User _user = UserDataManager.instance.user;

			

			//로딩바
			Loadmanager.instance.LoadingUI(true);

			Network_MainMenuSoketManager.Getsingleton.Operation_State= MMSERVER_STATE.TRY_QUICKJOIN;


			//퀵조인 요청 보내기
			_user.user_Games.battleGameKind = BattleKind.NORMAL;	//배틀종류 저장
			Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);

	
	}



	public void  ResponseButton_Traning()
	{
		UI_Manager.Getsingleton.ClearUI(this);

		User _user = UserDataManager.instance.user;



		//로딩바
		Loadmanager.instance.LoadingUI(true);

		Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.TRY_QUICKJOIN;


		//퀵조인 요청 보내기
		_user.user_Games.battleGameKind = BattleKind.ALONE_PLAY_BATTLE;	//배틀종류 저장
		Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);
	}



	public void ResponseButton_Close()
	{
		//로비캐릭 회전 잠금해제
		User.isSelectedCharacter = false;

		UI_Manager.Getsingleton.ClearUI(this);
	}
}
