using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_NetworkError : UI_PopupBase 
{
	public Text text_message;

	public override void Set_Open()
	{
		base.Set_Open();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void SetPopupMessage(string message)
	{
		text_message.text = message;
	}

	public void  ResponseButton_close()
	{
		//UI_Manager.Getsingleton.ClearUI(this);
	}

	
}
