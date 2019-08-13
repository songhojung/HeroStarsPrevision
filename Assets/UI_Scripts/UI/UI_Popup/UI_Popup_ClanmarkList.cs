using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup_ClanmarkList : UI_PopupBase {

	private int nowClanMarkIndx = 99;


	public override void Set_Open()
	{
		base.Set_Open();
	}
	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void ResponseToggle_markButton(int index)
	{
		if (nowClanMarkIndx != index)
		{
			nowClanMarkIndx = index;
			UserEditor.Getsingleton.EditLog(nowClanMarkIndx);
		}
	}

	public void ResponseButton_ChangeMark()
	{
		if (nowClanMarkIndx == 99)
		{
			UI_Popup_Toast popupToast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
			popupToast.SetPopupMessage("변경할 클랜마크를 선택하십시요");
		}
		else
		{
			webRequest.ClanMarkChange(5, (ushort)nowClanMarkIndx, callback_Complete_changeMark);
			//마크리스트UI 지우자
			UI_Manager.Getsingleton.ClearUI(this);
		}

		//재화부족 팝업창이 뜨면 UI_clan set_close 에서 모든 UIITEM 삭제 하므로 uiitem_clanmarkList는 따로 삭제 할필요 업다.

	}

	void callback_Complete_changeMark()
	{



		//clan ui refresh
		webRequest.ClanInfo(()=>webRequest.GetUserInfos(UserDataManager.instance.user.user_Users.UserID,callback_refreshClanInfo));

	}

	void callback_refreshClanInfo()
	{
		//클랜정보 갱신
		UI_Clan.Getinstance.Set_Clan();

		//제화 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();
	}

	public void ResponseButton_Back()
	{
		//UI_Manager.Getsingleton.RemoveUI_Item(this);
		//Destroy(this.gameObject);
		UI_Manager.Getsingleton.ClearUI(this);
	}
}
