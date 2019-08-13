using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_ResultUnitSlot : UI_Base 
{

	//UI
	public Image image_unitIcon;
	public Image image_bfRank;
	public Image image_curRank;
	public Text text_kill;
	public Text text_Exp;
	public Text text_bonusGem;
	public List<GameObject> Lst_RankUpUI;
	public List<GameObject> Lst_ActivateSlot;

	private GainItem gainUnitItem;
	private bool isRankUp;
	private bool IsEnableSlot = false;
	public bool IsActiveSlot
	{
		set
		{
			IsEnableSlot = value;
			Set_Slot(IsEnableSlot);
		}
		get { return IsEnableSlot; }
	}


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

	void Set_Slot(bool isActive)
	{
		if (!isActive)
		{
			Activate_SlotObj(false);
		}
		else
		{
			Activate_SlotObj(true);
		}
	}

	public void Set_unitIcon(uint unitIdx)
	{
		//캐릭이미지
		image_unitIcon.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.chicon, unitIdx));
	}

	public void Set_resultUnitSlot(GainItem _gainUnitItem = null)
	{
		

		if (_gainUnitItem != null)
		{
			gainUnitItem = _gainUnitItem;

			if (_gainUnitItem.gainUserUnit.isRankUp)
				isRankUp = true;
		}

		Apply_Unitslot();

	}


	void Apply_Unitslot()
	{
		
		//계급
		image_bfRank.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.unitrank, gainUnitItem.gainUserUnit.UnitRk));
		

		//킬

		text_kill.text = gainUnitItem.gainUserUnit.KillCnt.ToString();
			
		//경험치
		text_Exp.text = gainUnitItem.gainUserUnit.UnitExp.ToString();



		//계급상승 보너스
		if (isRankUp)
		{
			text_bonusGem.text = TableDataManager.instance.Infos_unitRanks[(byte)gainUnitItem.gainUserUnit.UnitRk].RwdItNum.ToString();

			image_curRank.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.unitrank, gainUnitItem.gainUserUnit.UnitRk - 1));
		}

		Activate_RankUpObj(isRankUp);


	}

	public void Clear_UnitslotInfo()
	{
		Activate_SlotObj(false);
		Activate_RankUpObj(false);
	}


	void Activate_RankUpObj(bool isActive)
	{
		for (int i = 0; i < Lst_RankUpUI.Count; i++)
			Lst_RankUpUI[i].SetActive(isActive);
	}


	void Activate_SlotObj(bool isActive)
	{

		if (isActive)
		{
			Lst_ActivateSlot[0].SetActive(true);
			Lst_ActivateSlot[1].SetActive(false);
		}
		else
		{
			Lst_ActivateSlot[0].SetActive(false);
			Lst_ActivateSlot[1].SetActive(true);
		}
	}

	public GainItem Get_gainItem()
	{
		return gainUnitItem;
	}

}
