using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Popup_Selective : UI_PopupBase
{
    public Text Text_Message;
	public Text Text_titleMessage;
    public Button Button_Yes;
    public Button Button_No;

	public GameObject ClanJoin;

    // : ===================================================================



    public override void Set_Open()
    {
        base.Set_Open();

        //Button_No.onClick.AddListener(() => UI_Manager.Getsingleton.ClearUI(UIPOPUP.POPUPSELECTIVE));

		//로비UI라면 로비리더기능 비활성
		Set_selectCharacterRotating(true);
    }

    public override void Set_Close()
    {
        base.Set_Close();

    }
	public void SetPopupMessage(string _messgae)
	{
		Text_Message.text = _messgae;
	}

	public void Set_PopupTitleMessage(string _message)
	{
		Text_titleMessage.text = _message;
	}

	public void ActiveClanRecommand()
	{
		ClanJoin.SetActive(true);
	}

	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}

	public void Set_addNoEventButton(del_ResPopup _Action)
	{
		delegate_ResponseNo = _Action;
	}


	public override void ResponseButton_Yes()
	{
		if (delegate_ResponseOk != null)
		delegate_ResponseOk();

		//로비UI라면 로비리더기능 비활성
		Set_selectCharacterRotating(false);

		UI_Manager.Getsingleton.ClearUI(this);



	}


	public void ResponseButton_No()
	{
		if (delegate_ResponseNo != null)
			delegate_ResponseNo();

		//로비UI라면 로비리더기능 비활성
		Set_selectCharacterRotating(false);

		UI_Manager.Getsingleton.ClearUI(this);

	}



	public override void Set_ErrorMassage()
	{
		base.Set_ErrorMassage();


	}

	//로비 UI라면 로비리더선택 기능 활성/ 비활성 => true면 리더선택했다 그래서 리더선택버튼 비활성된다 , false 면 리더선택안햇다 그래서 리더선택버튼 활성된다
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

	public void ResponseButton_RecommandClan()
	{


	}

}
