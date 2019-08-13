using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_Popup_RfProcess : UI_PopupBase
{
	public enum Reinforce_State
	{
		NONE = 99,
		Waiting = 0,
		Success = 1,
		Fail = 2,
		Retry = 3,
	}

	private bool isProcessComplete = false;				//강화진행완료햇냐?


	//UI
	public Text text_RfItemName;
	public List<Text> Lst_textSpecName;
	public List<Text> Lst_textNowSpecValue;
	public List<Text> Lst_textNxtSpecValue;
	public Slider Slider_Reinforce;
	public List<GameObject> Lst_nextSpecUnlockObj;
	public List<GameObject> Lst_nextSpecLockObj;
	public List<GameObject> Lst_nowSpecObj;
	public List<GameObject> Lst_nextSpecObj;
	public List<GameObject> Lst_RfProcessObj;
	public GameObject HighlightObj;
	public Reinforce_State RfState = Reinforce_State.Waiting;

	private uint UnitIdx;
	private uint ItemIdx;
	private bool IsAskChangeSubSkill = false;		//서브스킬 바꿀로직상이냐?

	public Animator Ani;

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

	public void Set_RfProcess(REINFORCE_TYPE RfType,bool isRfSuccess,uint _unitIdx, uint _ItemIdx)
	{
		
		Active_RfProcessObj(RfState);

		UnitIdx = _unitIdx;
		ItemIdx = _ItemIdx;


		if (isRfSuccess)
		{
			// 강화아이템 정보
			Set_RfInfo(RfType);

			//강화 성공시 분석 떄리기
			Analyze_successRf(RfType);

			Ani.SetTrigger("Rf_success");
		}
		else
		{
			Ani.SetTrigger("Rf_fail");
		}

		


	}

	// 강화아이템 정보 설정
	void Set_RfInfo(REINFORCE_TYPE RfType)
	{

		User _user = UserDataManager.instance.user;
		uint _unitIdx = UnitIdx;

		if (RfType == REINFORCE_TYPE.UNIT)
		{
			Dictionary<uint, infos_unit> _dic_unitinfo = TableDataManager.instance.Infos_units;
			byte maxRfLv = TableDataManager.instance.Get_MaxRefLv();
			byte nowRfLv = (byte)(_user.User_Units[UnitIdx].RefLv-1);
			byte nextRfLv = (byte)(_user.User_Units[UnitIdx].RefLv);

			if (nextRfLv >= maxRfLv)
			{
				nowRfLv = maxRfLv;
				nextRfLv = maxRfLv;

			}

			//레벨 대하여 spec 오브젝트 활성화
			Activate_ReinfObects(RfType,nextRfLv);

			//강화 이름 
			text_RfItemName.text = string.Format("{0} <color=#ff3e56>Lv.{1}</color>", _dic_unitinfo[UnitIdx].UnitName, nextRfLv);

			//능력치이름
			Lst_textSpecName[0].text = TextDataManager.Dic_TranslateText[400]; //체력
			Lst_textSpecName[1].text = TextDataManager.Dic_TranslateText[401]; //이동속도
			Lst_textSpecName[2].text = TextDataManager.Dic_TranslateText[402];	//장전속도
			Lst_textSpecName[3].text = TextDataManager.Dic_TranslateText[403];	//서브스킬
			//현재능력치
			Lst_textNowSpecValue[0].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].Hp, nowRfLv, true).ToString();
			//다음능력치
			Lst_textNxtSpecValue[0].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].Hp, nextRfLv, true).ToString();

			if (nextRfLv >= 5)
			{

				//현재능력치
				Lst_textNowSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].MvSpeed, nowRfLv, false, 0.05f, 4).ToString();

				//다음능력치
				Lst_textNxtSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].MvSpeed, nextRfLv, false, 0.05f, 4).ToString();

			}
			else
			{
				//현재능력치
				Lst_textNowSpecValue[1].text =_dic_unitinfo[UnitIdx].MvSpeed.ToString();

			}
			if (nextRfLv >= 10)
			{
				//현재능력치
				Lst_textNowSpecValue[2].text = string.Format("+{0}", StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].RldSpeed, nowRfLv, true, 5f, 9));

				//다음능력치
				Lst_textNxtSpecValue[2].text = string.Format("+{0}", StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].RldSpeed, nextRfLv, true, 5f, 9));
			}
			else
			{
				//현재능력치
				Lst_textNowSpecValue[2].text = "+"+_dic_unitinfo[UnitIdx].RldSpeed.ToString();

			}
			if (nextRfLv >= 15)
			{
				User_Units userUnit = _user.User_Units[UnitIdx];

				if (userUnit.SubSkill == 0)
				{
					//현재능력치
					Lst_textNowSpecValue[3].text = "----";

					
				}
				else
				{
					//현재능력치
					Lst_textNowSpecValue[3].text = /*Get_BfSubSkillName()*/TableDataManager.instance.Infos_Skills[userUnit.SubSkill].SkillName;

					
				}

				if (_user.User_DrawSkills[UnitIdx].SubSkill != 0)
				{
					//다음능력치
					Lst_textNxtSpecValue[3].text = TableDataManager.instance.Infos_Skills[_user.User_DrawSkills[UnitIdx].SubSkill].SkillName;

					//현재 가진 서브스킬있으므로 서브스킬바꿀껀지 로직들어감
					IsAskChangeSubSkill = true;
				}
				else
				{
					//현재 가진 서브스킬없으므로 서브스킬바꿀껀지 로직안들어가도됨
					IsAskChangeSubSkill = false;
				}
			}
		}
		else if (RfType == REINFORCE_TYPE.WEAPON)
		{
			

	//스펙=============
			Dictionary<uint, Infos_Weapon> _dic_weaponinfo = TableDataManager.instance.Infos_weapons;
			byte nowRfLv = (byte)(_user.User_Weapons[ItemIdx].RefLv -1);
			byte nextRfLv = (byte)(_user.User_Weapons[ItemIdx].RefLv);

			//강화 아이템 이름
			text_RfItemName.text = string.Format("{0} <color=#ff3e56>Lv.{1}</color>", TableDataManager.instance.Infos_weapons[ItemIdx].WpnName, nextRfLv);

			//레벨 대하여 spec 오브젝트 활성화
			Activate_ReinfObects(RfType, nextRfLv);

			Lst_textSpecName[0].text = TextDataManager.Dic_TranslateText[405]; //위력
			Lst_textSpecName[1].text = TextDataManager.Dic_TranslateText[406]; // 크리티컬

			//현재능력치
			float nowAtkMin = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMin, nowRfLv, true);
			float nowAtkMax = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMax, nowRfLv, true);
			Lst_textNowSpecValue[0].text = string.Format("{0}~{1}", nowAtkMin, nowAtkMax);
			Lst_textNowSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].Critical, nowRfLv, false).ToString();

			//다음능력치
			float nextAtkMin = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMin, nextRfLv, true);
			float nextAtkMax = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMax, nextRfLv, true);
			Lst_textNxtSpecValue[0].text = string.Format("{0}~{1}", nextAtkMin, nextAtkMax);
			Lst_textNxtSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].Critical, nextRfLv, false).ToString();
		}
	}




	//강화레벨 따른 오브젝트 활성화 설정
	void Activate_ReinfObects(REINFORCE_TYPE reinfType , byte nowRflv)
	{
		if (reinfType == REINFORCE_TYPE.UNIT)
		{
			//스펙 element 4개만큼 활성화하기 
			Activate_SpecElements(3);

			//스펙4번쨰 하이라이트 오브젝트 활성
			HighlightObj.SetActive(false);//test


			//moveSpeed스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[0].SetActive(nowRflv >= 5);
			//moveSpeed스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[0].SetActive(!(nowRflv >= 5));


			//reloadSpeed스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[1].SetActive(nowRflv >= 10);
			//reloadSpeed스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[1].SetActive(!(nowRflv >= 10));


			//subSkill스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[2].SetActive(nowRflv >= 15);
			//subSkill스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[2].SetActive(!(nowRflv >= 15));

			//if (nowRflv >= 5)
			//{
				
			//}
			//else
			//{
			//    //moveSpeed스펙 언락오브젝트 비활성
			//    Lst_nextSpecUnlockObj[0].SetActive(!(nowRflv >= 5));
			//    //moveSpeed스펙 락오브젝트 활성
			//    Lst_nextSpecLockObj[0].SetActive(nowRflv >= 5);
			//}
			//if (nowRflv >= 10)
			//{
				
			//}
			//else
			//{
			//    //moveSpeed스펙 언락오브젝트 비활성
			//    Lst_nextSpecUnlockObj[1].SetActive(!(nowRflv >= 5));
			//    //moveSpeed스펙 락오브젝트 활성
			//    Lst_nextSpecLockObj[1].SetActive(nowRflv >= 5);
			//}
			//if (nowRflv >= 15)
			//{
			

			//}
			//else
			//{
			//    //moveSpeed스펙 언락오브젝트 비활성
			//    Lst_nextSpecUnlockObj[2].SetActive(!(nowRflv >= 15));
			//    //moveSpeed스펙 락오브젝트 활성
			//    Lst_nextSpecLockObj[2].SetActive(nowRflv >= 15);
			//}
		}
		else if (reinfType == REINFORCE_TYPE.WEAPON)
		{
			//스펙 element 2개만큼 활성화하기 
			Activate_SpecElements(2);

			//스펙4번쨰 하이라이트 오브젝트 활성
			HighlightObj.SetActive(false);

			//모든 언락 오브젝트 활성, 모든 락오브젝트 비활성
			int totalcount = Lst_nextSpecUnlockObj.Count;
			for (int i = 0; i < totalcount; i++)
			{
				Lst_nextSpecUnlockObj[i].SetActive(true);
				Lst_nextSpecLockObj[i].SetActive(false);

			}
		}
	}





	//스펙 element 정해진 갯수만큼 활성화하기 
	void Activate_SpecElements(int activeCount)
	{
		int totalcount = Lst_nowSpecObj.Count;

		for (int i = 0; i < totalcount; i++)
		{
			if (i < activeCount)
			{
				Lst_nowSpecObj[i].SetActive(true);
				Lst_nextSpecObj[i].SetActive(true);
			}
			else
			{
				Lst_nowSpecObj[i].SetActive(false);
				Lst_nextSpecObj[i].SetActive(false);
			}

		}
	}



	void Active_RfProcessObj(Reinforce_State state)
	{
		for (int i = 0; i < Lst_RfProcessObj.Count; i++)
			Lst_RfProcessObj[i].SetActive(i == (int)state);
	}



	//강화 이전 서브 스킬이름 가져오기
	public string Get_BfSubSkillName()
	{
		string name = string.Empty;
		int bfIdx = 0;
		User_Units userUnit = UserDataManager.instance.user.User_Units[UnitIdx];
		for (int i = 0; i < webResponse.GetResultInfoList.Count; i++ )
		{
			if (webResponse.GetResultInfoList[i].ItTp == ITEMTYPE.UNIT)
			{
				bfIdx = userUnit.SubSkill + webResponse.GetResultInfoList[i].gainUserUnit.SubSkillIdxDiff;
				break;
			}
		}

		if (TableDataManager.instance.Infos_Skills.ContainsKey((byte)bfIdx))
			name = TableDataManager.instance.Infos_Skills[(byte)bfIdx].SkillName;
		else
			name = "----";

		return name;
	}


	
	//강화 성공시 분석 떄리기
	void Analyze_successRf(REINFORCE_TYPE rfType)
	{
		User _user = UserDataManager.instance.user;

		//if(rfType == REINFORCE_TYPE.UNIT)
		//{
		//	Dictionary<uint, infos_unit> _dic_unitinfo = TableDataManager.instance.Infos_units;

		//	AnalysisManager.instance.Anl_CustomEvt(DefineKey.Reinforce, DefineKey.ReinforceUnit, //유닛강화
		//	_dic_unitinfo[UnitIdx].UnitName,												//유닛이름
		//	string.Format("{0}{1}", DefineKey.RfLV, _user.User_Units[UnitIdx].RefLv),			//강화레벨
		//	1);
		//}
		//else if (rfType == REINFORCE_TYPE.WEAPON)
		//{
		//	Dictionary<uint, Infos_Weapon> _dic_weaponinfo = TableDataManager.instance.Infos_weapons;

		//	AnalysisManager.instance.Anl_CustomEvt(DefineKey.Reinforce, DefineKey.ReinforceWpn,				 //무기강화
		//	_dic_weaponinfo[ItemIdx].WpnName,															//무기이름
		//	string.Format("{0}{1}", DefineKey.RfLV, _user.User_Weapons[UnitIdx][ItemIdx].RefLv),				//강화레벨
		//	1);

		//}
	}



	//진행상태가 다 했을떄 콜백 함수 
	public void DoProcessReinfoce(Reinforce_State state)
	{
		RfState = state;
		Active_RfProcessObj(RfState);

		//강화진행완료
		isProcessComplete = true;

		if (IsAskChangeSubSkill)
		{

		}
		else
		{
			//강화완료후 갱신로직
			CompleteRf_Refresh();
		}
	}



	//강화완료후 갱신로직
	void CompleteRf_Refresh()
	{
		//로비라면 로비정보 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
		{

			if (UserDataManager.instance.user.User_Units.ContainsKey(UnitIdx))
			{
                UI_LobbyOld.Getsingleton.Dic_lobbyCharacter[UnitIdx].userUnit = UserDataManager.instance.user.User_Units[UnitIdx];
                UI_LobbyOld.Getsingleton.Apply_LobbyInfoUsingSelectedChar(UI_LobbyOld.Getsingleton.Dic_lobbyCharacter[UnitIdx]);
			}
		}
		else if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.EQUIPMENT))// 장비ui라면 장비정보 갱신
		{
			//레벨업후 선택아이템 정보 갱신
			UI_Equipment.Getsingleton.Set_Item_Have(UI_Equipment.Getsingleton.nowEquipInven);
		}
	}


	//스킬 바꾸기 알림팝업 => 스킬바꿀꺼냐 ?
	void PopupSelectingSkill()
	{

		User _user = UserDataManager.instance.user;
		string bfSkill = TableDataManager.instance.Infos_Skills[_user.User_Units[UnitIdx].SubSkill].SkillName;
		byte subSkill = _user.User_DrawSkills[UnitIdx].SubSkill;
		string NewSkill = TableDataManager.instance.Infos_Skills[subSkill].SkillName;

		UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
		popup.Set_addEventButton(callback_DoChange_Subskill);
		popup.Set_addNoEventButton(callback_DontChange_Subskill);
		popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
		popup.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[146], bfSkill,NewSkill));//서브스킬을 변경 하시겠습니까?
	}

	void callback_DoChange_Subskill()
	{
		UI_Manager.Getsingleton.ClearUI(this);
		webRequest.BuySkillCommit(UnitIdx, 
			UserDataManager.instance.user.User_DrawSkills[UnitIdx].SubSkill, 
			true,
			refresh_AfterChangeSubSkill);
	}

	void callback_DontChange_Subskill()
	{
		UI_Manager.Getsingleton.ClearUI(this);
		webRequest.BuySkillCommit(UnitIdx,
			UserDataManager.instance.user.User_DrawSkills[UnitIdx].SubSkill,
			false,
			refresh_AfterChangeSubSkill);
	}


	public void refresh_AfterChangeSubSkill()
	{
		CompleteRf_Refresh();

		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		//UI_Manager.Getsingleton.ClearUI(this);
	}
	

	public void ResponseButton_ReinforceEnd()
	{
		if (RfState == Reinforce_State.Success || RfState == Reinforce_State.Fail)
		{

			if (isProcessComplete)
			{
				if (!IsAskChangeSubSkill)
				{


					if (delegate_ResponseOk != null)
						delegate_ResponseOk();

					UI_Manager.Getsingleton.ClearUI(this);
				}
				else // 스킬바꿀껀지 물어보기 팝업띄우기 
				{
					PopupSelectingSkill();
				}
			}
		}
	}
}
