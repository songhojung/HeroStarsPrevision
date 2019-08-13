using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Matching : UI_Base 
{
	public List<Image> Lst_myTeam = new List<Image>();
	public List<Image> Lst_otherTeam = new List<Image>();

	public GameObject DisableBtn;

	private MATCHTYPE match_Type = MATCHTYPE.ONEVSONE;


	public override void set_Open()
	{
		base.set_Open();

		//매칭창 들어올떄 모든팝업창 다꺼버리자
		UI_Manager.Getsingleton.ClearPopupUI();
	}

	public override void set_Close()
	{
		base.set_Close();
	}


	public override void set_refresh()
	{
		base.set_refresh();
	}

	//정보 셋팅
	public void Set_Info(MATCHTYPE matchType)
	{
		match_Type = matchType;

		Init_ActiveTeam(matchType);
		
	}


	////디버그용 AI랑 매치 => 상용시 삭제
	//System.DateTime TouchTime = System.DateTime.Now.AddHours(-1);
	//void Update()
	//{
	//    if (Input.GetMouseButtonDown(0))
	//    {
	//        Debug.Log("GetMouseButtonDown CALL !! : " + (System.DateTime.Now - TouchTime).TotalSeconds);
	//        if ((System.DateTime.Now - TouchTime).TotalSeconds < 1)
	//        {
	//            Debug.Log("Send_CtS_AI_MATCH : CALL");

	//            GameObject.Find("Network_Script").GetComponent<Network_Match_Script>().Send_CtS_AI_MATCH();
	//        }
	//        TouchTime = System.DateTime.Now;
	//    }

	//}

	
	//초기 UI
	void Init_ActiveTeam(MATCHTYPE matchType)
	{
		if (matchType == MATCHTYPE.ONEVSONE)
		{
			Lst_myTeam[0].gameObject.SetActive(true);
			Lst_myTeam[0].color = DefineKey.Yellow;
			Lst_otherTeam[0].gameObject.SetActive(true);
			Lst_otherTeam[0].color = DefineKey.LightBlack;
		}
		else if (matchType == MATCHTYPE.TWOVSTWO)
		{
			Lst_myTeam[0].gameObject.SetActive(true);
			Lst_myTeam[0].color = DefineKey.Yellow;
			Lst_myTeam[1].gameObject.SetActive(true);
			Lst_myTeam[1].color = DefineKey.LightBlack;
			for (int i = 0; i < Lst_otherTeam.Count; i++)
			{
				Lst_otherTeam[i].gameObject.SetActive(true);
				Lst_otherTeam[i].color = DefineKey.LightBlack;
			}
		}
	}

	public void Complete_Matching()
	{
		StartCoroutine(Routine_fillTeam(match_Type));

	}

	IEnumerator Routine_fillTeam(MATCHTYPE matchType)
	{
		int count = 0;

		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (matchType == MATCHTYPE.ONEVSONE)
			{
				if (count >= 1)
				{
					Loadmanager.async.allowSceneActivation = true;
					break;
				}
				else
				{
					Lst_otherTeam[count].color = DefineKey.Yellow;
					count++;
				}
			}
			else
			{
				if (count >= 3)
				{
					Loadmanager.async.allowSceneActivation = true;
					break;
				}
				else
				{
					if (count == 0)
						Lst_myTeam[1].color = DefineKey.Yellow;
					else
						Lst_otherTeam[count - 1].color = DefineKey.Yellow;
					count++;
				}
			}
			yield return null;
		}
	}
	public void ResponseButton_CancleMatching()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		if (UI_Manager.Getsingleton.goBackUI != UI.CHAT)
		{
			//chatui 도 Top 생성안하기 떄문에 
			UI_Manager.Getsingleton.CreatUI(UI.TOP, _canvasTr);
			UI_Manager.Getsingleton.CreatUI(UI_Manager.Getsingleton.goBackUI, _canvasTr);

		}
		else
		{
			UI_Manager.Getsingleton.CreatUI(UI.TOP, _canvasTr);
			UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);

		}
	

        //Link_Script.ins.Quick_Join_Cancel();
	}

	void cancle()
	{
		
	}
}
