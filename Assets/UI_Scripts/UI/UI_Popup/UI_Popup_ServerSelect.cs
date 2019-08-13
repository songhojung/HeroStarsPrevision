using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_ServerSelect : UI_PopupBase 
{
	private int selectServerIdx = 0;

	public List<GameObject> Lst_severElementOBJ;
	public List<Text> Lst_textServerName;
	public List<Slider> Lst_SliderServerRate;
	public List<Text> Lst_textServerRate;

	public override void Set_Open()
	{
		base.Set_Open();

		Clear_ServerElements();
		Chk_ServerInfo();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_addEventYESButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}


	void Chk_ServerInfo()
	{
		User _user = UserDataManager.instance.user;

		//
		if (_user.LogInState == Login_State.LogSelectServer)
		{
			//서버리스트 불러오기
			string CtCode = StaticMethod.Get_OrinCountryCodeString();
			webRequest.GetServerList(CtCode, Set_ServerInfo);
		}
		else
			Set_ServerInfo();
	}


	void Set_ServerInfo()
	{
		User _user = UserDataManager.instance.user;
		int idx = 0;
		List<int> activeServers = new List<int>();

		foreach (var server in _user.User_GameServerInfos)
		{
			idx = server.Value.SubIdx;
			//서버 elemet 활성화
			Lst_severElementOBJ[idx].SetActive(true);

			//서버이름
			Lst_textServerName[idx].text = server.Value.ServerName;

			//서버 동접비율
			Lst_SliderServerRate[idx].value = (float)server.Value.ConPer;
			Lst_textServerRate[idx].text = string.Format("{0}%", (Lst_SliderServerRate[idx].value / Lst_SliderServerRate[idx].maxValue) * 100f);

			activeServers.Add(idx);
		}


		//서버 없는 것들은 오브젝트들 비활성
		for (int i = 0; i < Lst_severElementOBJ.Count; i++ )
		{

			if(!activeServers.Contains(i))
				Lst_severElementOBJ[i].SetActive(false);
		}

		//순서대로 정렬
		for (int i = 0; i < activeServers.Count; i++ )
		{
			Lst_severElementOBJ[activeServers[i]].transform.SetSiblingIndex(i);
		}


		//디폴트로 선택 되기 하기 
		
		Lst_severElementOBJ[_user.DefaultConnectServerIdx].GetComponent<Toggle>().isOn = true;
	}



	void Clear_ServerElements()
	{
		//오브젝트들 비활성
		for (int i = 0; i < Lst_severElementOBJ.Count; i++)
		{
				Lst_severElementOBJ[i].SetActive(false);
		}
	}



	public void ReseponseToggle_Select(int serverIdx)
	{
		if (selectServerIdx != serverIdx)
		{
			selectServerIdx = serverIdx;
		}
	}

	public void ResponseButton_SelectServer()
	{
		//현재 사용 서버 정보 저장
		UserDataManager.instance.user.Set_useGameServer((byte)(selectServerIdx));


		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Manager.Getsingleton.ClearUI(this);
	}
}
