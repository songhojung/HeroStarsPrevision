using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_Clan : UI_Base {

	public InputField input_ClanCreate;

	private List<UIItem_ClanListElement> Lst_ClanlistElement = new List<UIItem_ClanListElement>();
	private string createClanName ;

	void Set_Clanlist()
	{
		Request_ClanList();
	}


	void Request_ClanList()
	{
		if (Lst_ClanlistElement.Count <= 0)
			webRequest.ClanList(Create_Clanlist);
		else
			Refresh_Clanlist();
	}

	void Create_Clanlist()
	{
		Loadmanager.instance.LoadingUI(true);
		StartCoroutine(routine_CreateClanList());
		Loadmanager.instance.LoadingUI(false);
		
	}

	IEnumerator routine_CreateClanList()
	{
		User _user = UserDataManager.instance.user;
		foreach (var clan in _user.Clan_clanLists)
		{
			UIItem_ClanListElement clanE = UI_Manager.Getsingleton.CreatUI(
				UIITEM.ITEM_CLANLISTELEMENT, Lst_TrContent[(int)ClanUI_Type.ClanList]) as UIItem_ClanListElement;
			clanE.Set_ClanElementInfo(clan.Value);

			Lst_ClanlistElement.Add(clanE);

			yield return null;
		}
	}


	void Refresh_Clanlist()
	{
		ClearElements(nowClanUI);
		Request_ClanList();
	}



	







	public void ResponseInput_ValueChangedCreateClan()
	{
		createClanName = input_ClanCreate.text;
	}

	public void ResponseInput_EndCreateClan()
	{
		//createClanName = input_ClanCreate.text;
	}


	//클랜생성
	public void ResponseButton_ClanCreate()
	{

		if (createClanName == null || createClanName.Length <= 1)
		{
			UI_Popup_Toast popuptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popuptoast.SetPopupMessage(TextDataManager.Dic_TranslateText[152]);
			return;
		}
		else
		{
			if (!TextDataManager.Chk_BannedLetter(ref createClanName))
			{
				webRequest.ClanMake(createClanName, callback_complete_CreateClan);
			}
			else
			{
				UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
				popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[111]);//사용 불가능한 문자열이 포함 되어있습니다.

				input_ClanCreate.text = "";
			}
			
		}
	}
	//클랜생성 완료 콜백
	void callback_complete_CreateClan()
	{
		input_ClanCreate.text = "";

		webRequest.ClanInfo(callback_complete_claninfo);

		//제화 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();
	}

	void callback_complete_claninfo()
	{
		nowClanUI = ClanUI_Type.Manage;
		Active_ClanObject(nowClanUI);
	}



	//클랜찾기
	public void ResponsetButton_SearChingClan()
	{
		UI_Popup_Find popup =  UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Find>(UIPOPUP.POPUPFIND);
		popup.Set_FindPopup(FindID_Type.CLAN);
	}
}
