using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Review : UI_PopupBase 
{

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

	public override void ResponseButton_Yes()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Manager.Getsingleton.ClearUI(this);
	}

	public void ResponseButton_Close()
	{
		if (delegate_ResponseNo != null)
			delegate_ResponseNo();

		UI_Manager.Getsingleton.ClearUI(this);
	}

}
