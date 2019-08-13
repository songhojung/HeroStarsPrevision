using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Notice : UI_PopupBase 
{
	public Text text_Title;
    public Text Text_Message;
    public Button Button_Close;


    // : ===================================================================



    public override void Set_Open()
    {
        base.Set_Open();

		//로비캐릭회전 잠금
		Set_selectCharacterRotating(true);
    }

    public override void Set_Close()
    {
        base.Set_Close();
     
    }

	public void Set_PopupTitleMessage(string _message)
	{
		text_Title.text = _message;
	}
    public void SetPopupMessage(string _messgae)
    {
        Text_Message.text = _messgae;
    }

	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public override void ResponseButton_Yes()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		//로비캐릭회전 잠금해제
		Set_selectCharacterRotating(false);

		UI_Manager.Getsingleton.ClearUI(this);

	}

	public void ResponseButton_close()
	{
		//로비캐릭회전 잠금해제
		Set_selectCharacterRotating(false);

		UI_Manager.Getsingleton.ClearUI(this);
	}

	public override void Set_ErrorMassage()
	{
		base.Set_ErrorMassage();

		//빨간색 표시
		Text_Message.color = new Color(1, 0, 0);
	}


	//로비캐릭회전 설정
	void Set_selectCharacterRotating(bool _IsActive)
	{
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
		{
			if (_IsActive)
				User.isSelectedCharacter = true;
			else if (!_IsActive)
				User.isSelectedCharacter = false;
		}
	}

}
