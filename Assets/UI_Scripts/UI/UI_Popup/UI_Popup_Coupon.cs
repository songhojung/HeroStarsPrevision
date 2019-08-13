using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Coupon : UI_PopupBase 
{

	public InputField Inputfield_SendCoupon;

	
	private string CouponNum = string.Empty;


	public override void Set_Open()
	{
		base.Set_Open();
	}


	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_addEventYESButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public void Set_addEventNOButton(del_ResPopup _Action)
	{
		delegate_ResponseNo = _Action;
	}

	public void ResponseInpunt_End()
	{
		
		CouponNum = Inputfield_SendCoupon.text;
		UserEditor.Getsingleton.EditLog("CouponNum : " + CouponNum);
	}

	public override void ResponseButton_Yes()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		webRequest.GetCooponReward(CouponNum, callback_completeCooponReward);

		UI_Manager.Getsingleton.ClearUI(this);
	}

	void callback_completeCooponReward()
	{
		UI_Popup_Toast popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
		popup.SetPopupMessage(TextDataManager.Dic_TranslateText[171]);

		this.gameObject.SetActive(false);

		Inputfield_SendCoupon.text = "";

	}


	public void ResponseButton_Close()
	{

		if (delegate_ResponseNo != null)
			delegate_ResponseNo();

		UI_Manager.Getsingleton.ClearUI(this);
	}
}
