using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UI_Popup_WriteBoard : UI_PopupBase 
{
	public InputField input_boardTxt;

	private string Boardtxt;

	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public void ResponseInput_valueChanged()
	{
		Boardtxt = input_boardTxt.text;
	}

	public void ResponseInput_End()
	{
		Boardtxt = input_boardTxt.text;
	}


	//게시하기 
	public void ResponseButton_post()
	{
		webRequest.ClanBodWrite(Boardtxt, callback_complete_Post);
	}

	void callback_complete_Post()
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
