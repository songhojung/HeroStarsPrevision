using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIItem_ChatElement : UI_Base 
{


	public Text text_sender;
	public Text text_massage;

	
	

	private string TxtMessage;


	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_Close()
	{
		base.set_Close();
	}

	public override void set_refresh()
	{
		base.set_refresh();
	}

	public void Set_ElementInfo(User_Chat _recieveChat, bool _isSendMsg, bool isWaitDestroy)
	{
		int _msgIdx = (int)_recieveChat.msgTp;

		if (!_isSendMsg)
		{
			text_massage.alignment = TextAnchor.MiddleCenter;
			text_sender.gameObject.SetActive(false);
		}
		else
		{
			text_massage.alignment = TextAnchor.MiddleLeft; 
			text_sender.gameObject.SetActive(true);
		}

		text_sender.text = _recieveChat.NkNm;
		text_massage.text = _recieveChat.chatMsg;


		if (isWaitDestroy)
			StartCoroutine(routine_waitDestroy(10f));
	}


	//채팅element 후초뒤 삭제하기
	IEnumerator routine_waitDestroy(float time)
	{
		yield return new WaitForSeconds(time);
		if(UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CHAT))
			UI_Chat.Getsingleton.Remove_Element(this);
		Destroy(this.gameObject);

	}






}
