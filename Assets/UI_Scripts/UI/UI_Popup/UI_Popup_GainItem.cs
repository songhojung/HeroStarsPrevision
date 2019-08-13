using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Popup_GainItem : UI_PopupBase 
{
	public Transform Itemlist_Tr;



	public override void Set_Open()
	{
		base.Set_Open();

		//로비 회전 잠금
		User.isSelectedCharacter = true;
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
	

	public void Set_GainPopup()
	{

		//아이템들 설정
		Apply_Items(webResponse.GetResultInfoList);
	}


	public void Apply_Items(List<GainItem> list_gainItem)
	{
		User _user = UserDataManager.instance.user;

		for (int i = 0; i < list_gainItem.Count; i++)
		{
			switch (list_gainItem[i].ItTp)
			{
				case ITEMTYPE.GEM:
				case ITEMTYPE.GOLD:
				case ITEMTYPE.UNIT:
					UIItem_GainItem gain = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_GAINITEM,Itemlist_Tr) as UIItem_GainItem;
					gain.Set_GainItemInfo(list_gainItem[i]);
					break;
			}
		}

	}


	public void ResponseButton_Close()
	{
		//로비 회전 잠금 해제
		User.isSelectedCharacter = false;

		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Manager.Getsingleton.ClearUI(this);
	}
}
