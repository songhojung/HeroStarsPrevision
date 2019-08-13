using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PopupBuyItem_Type
{
	SpecialPack_0 = 0, SpecialPack_1 = 1, SpecialPack_2 = 2,
}

public class UI_Popup_BuyItem : UI_PopupBase 
{

	private PopupBuyItem_Type popupBuyItemType;


	public List<GameObject> Lst_popupInfoOBJ;


	public Text text_main;
	public Text text_TitleName;
	public Text text_price;
	public Image image_price;




	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_popupName(string _popupName)
	{
		text_TitleName.text = _popupName;
	}

	public void Set_AddEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public void Set_info(PopupBuyItem_Type popupType , string massege, string price, string buyItemType)
	{
		popupBuyItemType = popupType;

		text_main.text = massege;
		text_price.text = price;
		image_price.sprite = ImageManager.instance.Get_Sprite(buyItemType);

		// 활성시킬 팝업내용오브젝트 
		Active_popupMainOBJ(popupBuyItemType);
	}




	// 활성시킬 팝업내용오브젝트 
	void Active_popupMainOBJ(PopupBuyItem_Type popupType)
	{
		for (int i = 0; i < Lst_popupInfoOBJ.Count; i++)
			Lst_popupInfoOBJ[i].SetActive(i == (int)popupType);
	}











	public override void ResponseButton_Yes()
	{
		

		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Manager.Getsingleton.ClearUI(this);
	}


	public void ResponseButton_Close()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	}
}
