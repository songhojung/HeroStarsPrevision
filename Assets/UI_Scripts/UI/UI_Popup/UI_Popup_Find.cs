using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Popup_Find :UI_PopupBase 
{
	public List<InputField> Lst_inputField;
	public List<GameObject> Lst_InputfiedObject;
	public List<Text> Lst_inputTitleName;
	public List<Text> Lst_inputPlaceHolder;

	private FindID_Type findTYPE = FindID_Type.NONE;
	private string findName;

	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_ErrorMassage()
	{
		base.Set_ErrorMassage();
	}

	public void Set_FindPopup(FindID_Type findType)
	{
		findTYPE = findType;
		Active_InputObject(findType);
	}

	void Active_InputObject(FindID_Type findType)
	{

		int activeCount = 0;
		string[] tName = null;
		string[] hName = null;

		if (findType == FindID_Type.USER)
		{
			activeCount = 3;
			tName = new string[] { TextDataManager.Dic_TranslateText[28], TextDataManager.Dic_TranslateText[29], TextDataManager.Dic_TranslateText[27] }; //클랜명,클랜ID,유저명
			hName = new string[] { TextDataManager.Dic_TranslateText[33], TextDataManager.Dic_TranslateText[34], TextDataManager.Dic_TranslateText[32] }; //클랜명을 입력하세요, 클랜ID를 입력하세요 , 유저명을입력하세요

		}
		else if (findType == FindID_Type.CLAN )
		{
			activeCount = 2;
			tName = new string[] { TextDataManager.Dic_TranslateText[28], TextDataManager.Dic_TranslateText[29] };//클랜명,클랜ID
			hName = new string[] { TextDataManager.Dic_TranslateText[33], TextDataManager.Dic_TranslateText[34] };//클랜명을 입력하세요, 클랜ID를 입력하세요 
		
		}
		else if (findType == FindID_Type.FRIEND)
		{
			activeCount = 2;
			 tName = new string[] { "FRIEND ID", "FRIEND NAME"};
			 hName = new string[] { TextDataManager.Dic_TranslateText[284], TextDataManager.Dic_TranslateText[285] };//친구ID를 입력하세요, 친구이름을 입력하세요
		}

		for (int i = 0; i < activeCount; i++)
			Lst_InputfiedObject[i].SetActive(true);

		Apply_inputNamesInfo(activeCount, tName, hName);
	}

	void Apply_inputNamesInfo(int _activeCount, string[] titleName, string[] holderName)
	{
		for (int i = 0; i <_activeCount; i++ )
		{
			Lst_inputTitleName[i].text = titleName[i];
			Lst_inputPlaceHolder[i].text = holderName[i];
		}
	}

	public void ResponseInput_Changed(int inputIdx)
	{
		UserEditor.Getsingleton.EditLog(inputIdx);
	}

	public void ResponseInput_End(int inputIdx)
	{
		//GameObject obj = EventSystem.current.currentSelectedGameObject;
		//InputField input =obj.GetComponent<InputField>();
		//if (input == null)
		//{
		//    Lst_inputField[inputIdx].text = "";
		//}
		//else
			findName = Lst_inputField[inputIdx].text;


	}

	
	
	
	/// <summary>
	/// 유저 찾기
	/// </summary>
	public void ResponseButton_FindUser()
	{
		User user = UserDataManager.instance.user;
		if (string.Equals(user.user_Users.NkNm, findName)) // 나를 찾는 다면
		{
			// 바로 팝업띄움
			UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
			popup.Set_UserInfo(user);

			UI_Manager.Getsingleton.ClearUI(this);
		}
		else // 다른유저라면 프로토콜 호출
			webRequest.SearchUserClan(1, findName, callback_Complete_findUser);
	}

	/// <summary>
	/// 유저찾기 프로토콜 완료시 실행함수
	/// </summary>
	void callback_Complete_findUser()
	{
		UI_Popup_UserInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_UserInfo>(UIPOPUP.POPUPUSERINFO);
		User user = UserDataManager.instance.OtherUser;
		popup.Set_UserInfo(user);

		UI_Manager.Getsingleton.ClearUI(this);
	}

	// 클랜이름으로 찾기
	public void ResponseButton_FindName_Clan()
	{
		if (findTYPE == FindID_Type.CLAN)
			webRequest.SearchUserClan(2, findName, callback_Complete_findClan);
		else if (findTYPE == FindID_Type.FRIEND)
			webRequest.SearchUserClan(4, findName, callback_Complete_findUser);

	}

	// 클랜 아이디로 찾기
	public void ResponseButton_FindID_Clan()
	{
		if (findTYPE == FindID_Type.CLAN)
			webRequest.SearchUserClan(3, findName, callback_Complete_findClan);
		else if (findTYPE == FindID_Type.FRIEND)
			webRequest.SearchUserClan(1, findName, callback_Complete_findUser);
	}


	/// <summary>
	/// 클랜 아이디및 이름으로 찾기 프로토콜 호출완료시 실행함수
	/// </summary>
	void callback_Complete_findClan()
	{

		UI_Popup_ClanInfo popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ClanInfo>(UIPOPUP.POPUPCLANINFO);
		User user = UserDataManager.instance.OtherUser;
		popup.Set_ClanInfo(user.clan_Clans, user.Clan_members);

		UI_Manager.Getsingleton.ClearUI(this);
	}

	public void ResponseButton_Close()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	}
}
