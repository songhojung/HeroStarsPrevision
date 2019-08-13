using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Reinforce : UI_PopupBase
{
	
	public Text text_RfItemName;
	public Text text_RfLv;
	public List<Text> Lst_textSpecName;
	public List<Text> Lst_textNowSpecValue;
	public List<Text> Lst_textNxtSpecValue;
	public List<GameObject> Lst_nextSpecLockObj;
	public List<GameObject> Lst_nextSpecUnlockObj;
	public List<GameObject> Lst_SpecObj;
	public List<GameObject> Lst_nowSpecObj;
	public List<GameObject> Lst_nextSpecObj;
	public List<GameObject> Lst_ButtonObj;
	public GameObject HighlightObj;
	public GameObject SkillhelpObj;
	public List<Text> Lst_textPrice;
	public List<Text> Lst_textSuccessRate;

	public REINFORCE_TYPE reinforceType; 
	private uint UnitIdx;
	private uint ItemIdx;
	private uint[] ItemPrice = new uint[2];
	private bool isFirstBuySubskill= false;

	Dictionary<byte, Infos_UnitReinforce> Dic_infosUnitRfprice;
	Dictionary<byte, Infos_WeaponReinforce> Dic_infosWpnRfprice;


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

	public void Set_ReinforceInfo(REINFORCE_TYPE rfType, uint _unitIdx, uint _ItemIdx)
	{
		
		User _user = UserDataManager.instance.user;
		reinforceType = rfType;
		UnitIdx = _unitIdx;
		ItemIdx = _ItemIdx;
		
		string itemName = string.Empty;
		string[] rate = new string[2];

		if (rfType == REINFORCE_TYPE.UNIT)
		{
			Dic_infosUnitRfprice = TableDataManager.instance.Infos_UnitReinforces;

			itemName = TableDataManager.instance.Infos_units[UnitIdx].UnitName;
			byte maxRfLv = TableDataManager.instance.Get_MaxRefLv();
			byte nextRfLv = (byte)(_user.User_Units[UnitIdx].RefLv + 1);
			if (nextRfLv >= maxRfLv)
			{
				nextRfLv = maxRfLv;
				ItemPrice[0] = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_BuySubSkillPrice].ConsVal; //스킬 보석 가격
			}

			ItemPrice[1] = Dic_infosUnitRfprice[nextRfLv].NeedGold;
			//rate[0] = string.Format("100%");
			//rate[1] = string.Format("{0}%", Dic_infosRfprice[nextRfLv].GoldSucPer); 

			//이름
			text_RfItemName.text = string.Format("{0} <color=#FF3E56>Lv.{1}</color>", itemName, _user.User_Units[UnitIdx].RefLv);

			//레벨표시
			//text_RfLv.text = string.Format("Lv{0} / Lv{1}", _user.User_Units[UnitIdx].RefLv, maxRfLv);
		}
		else if (rfType == REINFORCE_TYPE.WEAPON)
		{
			Dic_infosWpnRfprice = TableDataManager.instance.Infos_WeaponReinforces;
			itemName = TableDataManager.instance.Infos_weapons[ItemIdx].WpnName;
			byte maxRfLv = TableDataManager.instance.Get_MaxRefLv();
			byte nextRfLv = (byte)(_user.User_Weapons[ItemIdx].RefLv + 1);
			if (nextRfLv >= maxRfLv)
			{
				nextRfLv = maxRfLv;
			}
			//ItemPrice[0] = Dic_infosRfprice[nextRfLv].NeedGem;
			ItemPrice[1] = Dic_infosWpnRfprice[nextRfLv].NeedGold;
			//rate[0] = string.Format("100%");
			//rate[1] = string.Format("{0}%", Dic_infosRfprice[nextRfLv].GoldSucPer); 

			//이름
			text_RfItemName.text = string.Format("{0} <color=#ff3e56>Lv.{1}</color>", itemName, _user.User_Weapons[ItemIdx].RefLv);
		}
		


		//스펙표시
		Set_SpecInfo(reinforceType);


		

		// 가격및 확률표시
		Lst_textPrice[(int)(ITEMTYPE.GEM) - 1].text = ItemPrice[0].ToString();
		Lst_textPrice[(int)(ITEMTYPE.GOLD) - 1].text = ItemPrice[1].ToString();
		Lst_textSuccessRate[(int)(ITEMTYPE.GEM) - 1].text = rate[0];
		Lst_textSuccessRate[(int)(ITEMTYPE.GOLD) - 1].text = rate[1];

	}

	//스펙 표시 설정
	void Set_SpecInfo(REINFORCE_TYPE rfType)
	{
		User _user = UserDataManager.instance.user;

		if (rfType == REINFORCE_TYPE.UNIT)
		{
			Dictionary<uint, infos_unit> _dic_unitinfo = TableDataManager.instance.Infos_units ;
			byte maxRfLv = TableDataManager.instance.Get_MaxRefLv();
			byte nowRfLv =  (byte)(_user.User_Units[UnitIdx].RefLv);
			byte nextRfLv = (byte)(_user.User_Units[UnitIdx].RefLv + 1);

			if (nextRfLv >= maxRfLv)
				nextRfLv = maxRfLv;

			//레벨 대하여 spec 오브젝트 활성화
			Activate_ReinfObects(rfType, nowRfLv, nextRfLv);

			
			
				//능력치이름
				Lst_textSpecName[0].text = TextDataManager.Dic_TranslateText[400]; //체력
				Lst_textSpecName[1].text = TextDataManager.Dic_TranslateText[401]; //이동속도
				Lst_textSpecName[2].text = TextDataManager.Dic_TranslateText[402];	//장전속도
				Lst_textSpecName[3].text = TextDataManager.Dic_TranslateText[403];	//서브스킬

				//현재능력치
				Lst_textNowSpecValue[0].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].Hp, nowRfLv, true).ToString();
				//다음능력치
				Lst_textNxtSpecValue[0].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].Hp, nextRfLv, true).ToString();

			if (nowRfLv >= 4)
			{
				//현재능력치
				Lst_textNowSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].MvSpeed, nowRfLv, false,0.05f,4).ToString();

				//다음능력치
				Lst_textNxtSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_unitinfo[UnitIdx].MvSpeed, nextRfLv, false,0.05f,4).ToString();

			}
			else
			{
				//현재능력치
				Lst_textNowSpecValue[1].text = _dic_unitinfo[UnitIdx].MvSpeed.ToString();

			}
			if (nowRfLv >= 9)
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
			if (nowRfLv >= 15)
			{
				User_Units userUnit = _user.User_Units[UnitIdx];
				
				if(userUnit.SubSkill == 0)
				{
					isFirstBuySubskill = true; //처음 서브스킬 구매한다.

					//현재능력치
					Lst_textNowSpecValue[3].text = "----";
				}
				else
				{
					//현재능력치
					Lst_textNowSpecValue[3].text = TableDataManager.instance.Infos_Skills[userUnit.SubSkill].SkillName;
				}

				//다음능력치
				Lst_textNxtSpecValue[3].text = "?";
			}



			
		}
		else if (rfType == REINFORCE_TYPE.WEAPON)
		{
			Dictionary<uint, Infos_Weapon> _dic_weaponinfo = TableDataManager.instance.Infos_weapons;
			byte nowRfLv = (byte)(_user.User_Weapons[ItemIdx].RefLv);
			byte nextRfLv = (byte)(_user.User_Weapons[ItemIdx].RefLv + 1);

			//spec 오브젝트 활성화
			Activate_ReinfObects(rfType, nowRfLv,nextRfLv);

			Lst_textSpecName[0].text = TextDataManager.Dic_TranslateText[405]; //위력
			Lst_textSpecName[1].text = TextDataManager.Dic_TranslateText[406]; // 크리티컬

			//현재능력치
			float nowAtkMin = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMin, nowRfLv, true);
			float nowAtkMax = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMax, nowRfLv, true);
			Lst_textNowSpecValue[0].text = string.Format("{0}~{1}",nowAtkMin,nowAtkMax);
			Lst_textNowSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].Critical, nowRfLv, false).ToString();

			//다음능력치
			float nextAtkMin = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMin, nextRfLv, true);
			float nextAtkMax = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].AtkMax, nextRfLv, true);
			Lst_textNxtSpecValue[0].text = string.Format("{0}~{1}", nextAtkMin, nextAtkMax);
			Lst_textNxtSpecValue[1].text = StaticMethod.Get_nextSpec(_dic_weaponinfo[ItemIdx].Critical, nextRfLv, false).ToString();
		}
	}


	//강화레벨 따른 오브젝트 활성화 설정
	void Activate_ReinfObects(REINFORCE_TYPE reinfType, byte nowRflv , byte nextRflv)
	{
		if (reinfType == REINFORCE_TYPE.UNIT)
		{
			//스펙 element 4개 활성화하기 
			Activate_SpecElements(3);

			//스펙4번쨰 하이라이트 오브젝트 활성
			HighlightObj.SetActive(true);

			//lv5 스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[0].SetActive(nowRflv >= 5);
			//lv5 스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[0].SetActive(!(nowRflv >= 5));


			//lv10 스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[1].SetActive(nowRflv >= 10);
			//lv10 스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[1].SetActive(!(nowRflv >= 10));


			//lv15 스펙 언락오브젝트 활성
			Lst_nextSpecUnlockObj[2].SetActive(nowRflv >= 15);
			//lv15 스펙 락오브젝트 비활성
			Lst_nextSpecLockObj[2].SetActive(!(nowRflv >= 15));

			if (nowRflv >= 5)
			{
				
			}
			if (nowRflv >= 10)
			{
				
			}
			if (nowRflv >= 15)
			{


                //User_Units userUnit = UserDataManager.instance.user.User_Units[UnitIdx];
                //if (userUnit.SubSkill == 0)//서브 스킬 처음 구매이면 
                //{
                //	//업글 버튼 활성
                //	Lst_ButtonObj[0].SetActive(true);
                //	Lst_ButtonObj[1].SetActive(false);
                //}
                //else // 서브 스킬 두번쨰 구매이면
                //{
                //	//서브스킬 구매 버튼 활성
                //	Lst_ButtonObj[0].SetActive(false);
                //	Lst_ButtonObj[1].SetActive(true);
                //}

                //Test 15레벨이면 버튼 잠금
                Lst_ButtonObj[0].SetActive(false);
                Lst_ButtonObj[1].SetActive(false);

                //다음 스펙오브젝트 비활성화
                Lst_SpecObj[0].SetActive(true);
				Lst_SpecObj[1].SetActive(false);
			}
			else
			{
				

				//업글 버튼 활성
				Lst_ButtonObj[0].SetActive(true);
				Lst_ButtonObj[1].SetActive(false);

				//현재,다음 스펙오브젝트 활성화
				Lst_SpecObj[0].SetActive(true);
				Lst_SpecObj[1].SetActive(true);

			}
		}
		else if (reinfType == REINFORCE_TYPE.WEAPON)
		{
			//스펙 element 2개 활성화하기 
			Activate_SpecElements(2);

			//스펙4번쨰 하이라이트 오브젝트 비활성
			HighlightObj.SetActive(false);
			
			//모든 언락 오브젝트 활성, 모든 락오브젝트 비활성
			int totalcount = Lst_nextSpecUnlockObj.Count;
			for (int i = 0; i < totalcount; i++ )
			{
				Lst_nextSpecUnlockObj[i].SetActive(true);
				Lst_nextSpecLockObj[i].SetActive(false);

			}

			//업글 버튼 활성
			Lst_ButtonObj[0].SetActive(true);
			Lst_ButtonObj[1].SetActive(false);

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


	//강화하기 버튼
	public void ResponseButton_Rf(int goodType)
	{
		Do_ReinforceProtocol((ITEMTYPE)goodType);

		UI_Manager.Getsingleton.ClearUI(this);
	}

	void Do_ReinforceProtocol(ITEMTYPE _goodType)
	{

		uint price = 0;
		if (_goodType == ITEMTYPE.GOLD)
		{
			price = ItemPrice[1];

		}
		else if (_goodType == ITEMTYPE.GEM)
		{
			price = ItemPrice[0];
		}

		if (isFirstBuySubskill)
		{
			webRequest.ShopBuySkill(UnitIdx, 0, Callback_Complete_DoRfProtocol);
		}
		else
			webRequest.SetUnitRef(UnitIdx, ItemIdx, (byte)_goodType, price, Callback_Complete_DoRfProtocol);
	}


	//스킬 바꾸기 (사기)
	public void ResponseButton_BuySkill()
	{
		//필요 골드량
		uint price = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_BuySubSkillPrice].ConsVal;

		webRequest.ShopBuySkill(UnitIdx, price, Callback_Complete_DoRfProtocol);

		UI_Manager.Getsingleton.ClearUI(this);
		
	}




	void Callback_Complete_DoRfProtocol()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		
	}

	public void ResponseButton_Close()
	{
		if (delegate_ResponseNo != null)
			delegate_ResponseNo();

		//로비캐릭회전 해제
		User.isSelectedCharacter = false;

		UI_Manager.Getsingleton.ClearUI(this);
	}
	






	//스킬 설명도움
	public void ResponseButton_SkillHelp(int active)
	{
		if (active == 0) // 활성
			SkillhelpObj.SetActive(true);
		else if(active == 1) //비활성
			SkillhelpObj.SetActive(false);

	}

}
