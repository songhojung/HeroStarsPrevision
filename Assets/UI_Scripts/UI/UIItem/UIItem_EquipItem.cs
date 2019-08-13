using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIItem_EquipItem : UI_Base 
{

	public enum EquipItem_State
	{
		NONE = 99,
		Lock = 0,
		Equip = 1,
		New = 2,
		SetImage = 3,
        AlreadyEquiped = 4,
	}

	
	public List<Image> Lst_ItemBackImage;
	public List<GameObject> Lst_EquitItemStateObj;
	public Image image_Item;
	public Text text_setNum;
	public Image image_setNum;
	//버튼상태
	private ButtonState_Type buttonBackState = ButtonState_Type.Normal;
	public ButtonState_Type ButtonState
	{
		
		set
		{
			////더블클릭 체크
			if (value == ButtonState_Type.Selected && buttonBackState == ButtonState_Type.Selected)
				isDoubleClick = true;
			else if (value == ButtonState_Type.Normal)
				isDoubleClick = false;

			buttonBackState = value;
			Active_ButtonBackImage(buttonBackState);
		}
		get { return buttonBackState; }
	}

	//아이템 획득햇냐?
	private bool isGetItem = false;
	public bool IsHave
	{
		set
		{
			isGetItem = value;
			if(!isUnEquipBtn) //해제버튼 대한 예외처리
				Active_ItemImage(EquipItem_State.Lock, !isGetItem);
			else // 해제버튼이면
				Active_ItemImage(EquipItem_State.New, false);

			//have 이면 new false하기
			if (isGetItem == true)
				Active_ItemImage(EquipItem_State.New, false);
			

		}
		get { return isGetItem; }
	}

	//아이템을 착용햇냐?
	private bool isEquipItem = false;
	public bool IsEquip
	{
		set
		{
			isEquipItem = value;
			Active_ItemImage(EquipItem_State.Equip, isEquipItem);

			//equip 이면 new 표시 false
			if (isEquipItem == true)
				Active_ItemImage(EquipItem_State.New, false);
			

		}
		get { return isEquipItem; }
	}


    //다른 캐릭터가 이 아이템을 착용햇냐
    private bool isAlreadyEquiped = false;   
    public bool IsEquipedOtherUnit
    {
        set
        {
            isAlreadyEquiped = value;

            Active_ItemImage(EquipItem_State.AlreadyEquiped, isAlreadyEquiped);

        }

        get { return isAlreadyEquiped; }
    }


	public bool isDoubleClick = false;

	public bool isUnEquipBtn = false; // 장비해제 버튼이냐?


	public uint ItemIdx;		//아이템인덱스
	public int PartIdx;		//아이템장착부위
	public int SortIdx;		//정렬인덱스

	public override void set_Open()
	{
		base.set_Open();
	}

	public override void set_refresh()
	{
		base.set_refresh();
	}

	public void Set_ItemInfo(uint _itemIdx, int _partIdx, int _sortIdx)
	{
		ItemIdx = _itemIdx;
		PartIdx = _partIdx;
		SortIdx = _sortIdx;

		//버튼 백이미지 
		Active_ButtonBackImage(ButtonState);

		//장착마크표시 
		Active_ItemImage(EquipItem_State.Equip, false);

		if (_itemIdx == 1) isUnEquipBtn = true;
		else isUnEquipBtn = false;

		if (_itemIdx != 1)
		{
			

			//언락 표시 
			Active_ItemImage(EquipItem_State.Lock, true);

			//아이템이미지
			image_Item.sprite = ImageManager.instance.Get_Sprite(ItemIdx.ToString());
		}
		else // 1이면 해제버튼임
		{
			//언락 표시 
			Active_ItemImage(EquipItem_State.Lock, false);

			//아이템이미지
			image_Item.sprite = ImageManager.instance.Get_Sprite(DefineKey.Remove);
		}

		//세트아이템인지 체크 맞으면 세트이미지 넣기
		Chk_setItem();

		//새로운아이템인지 체크
		Chk_newItem();
	}


	//새로운아이템인지 체크
	void Chk_newItem()
	{
		if (TableDataManager.instance.Infos_weapons.ContainsKey(ItemIdx) && TableDataManager.instance.Infos_weapons[ItemIdx].NewFlg == 1)
			Active_ItemImage(EquipItem_State.New, true);
		else if (TableDataManager.instance.Infos_Decos.ContainsKey(ItemIdx) && TableDataManager.instance.Infos_Decos[ItemIdx].NewFlg == 1)
			Active_ItemImage(EquipItem_State.New, true);
		else
			Active_ItemImage(EquipItem_State.New, false);

	}


	//세트아이템인지 체크 맞으면 세트이미지 넣기
	void Chk_setItem()
	{
		Dictionary<ushort, Infos_SetBuf> infosSetbuf = TableDataManager.instance.Infos_SetBuffs;
		bool isSet = false;

		foreach (var sb in infosSetbuf)
		{
			bool correct = false;

			if (isSet == true) break;

			if (sb.Value.MainWpnIdx == ItemIdx) correct = true;
			if (sb.Value.SubWpnIdx == ItemIdx) correct = true;
			if (sb.Value.DecoIdx1 == ItemIdx) correct = true;
			if (sb.Value.DecoIdx2 == ItemIdx) correct = true;
			if (sb.Value.DecoIdx3 == ItemIdx) correct = true;

			if (correct == true)
			{
				isSet = correct;

				//세트이미지 넣기 
				int setKind = (int)sb.Value.BufKind;
				image_setNum.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.Set, setKind));
				image_setNum.gameObject.SetActive(true);
				//text_setNum.text = ((int)sb.Value.BufKind).ToString();
				//text_setNum.gameObject.SetActive(true);
			}
		}


		if (isSet == false)
		{
			image_setNum.gameObject.SetActive(false);
		}
	}

	public void Clear_ItemInfo()
	{
		ButtonState = ButtonState_Type.Normal;

		IsHave = false;

		IsEquip = false;

	
	}

	

	void Active_ButtonBackImage(ButtonState_Type state)
	{
		for (int i = 0; i < Lst_ItemBackImage.Count; i++)
		{
			//켜져 잇다면 껏다가 다시켤수있게 끄자 
			if (Lst_ItemBackImage[i].gameObject.activeSelf == true)
			{
				Lst_ItemBackImage[i].gameObject.SetActive(false);
			}
			Lst_ItemBackImage[i].gameObject.SetActive(i == (int)state);
		}
	}

	//아이템 상태이미지 (장착마크표시 , 언락표시)
	public void Active_ItemImage(EquipItem_State state , bool isActive)
	{

		Lst_EquitItemStateObj[(int)state].SetActive(isActive);
	}

	public void ResponseButton_Select()
	{

	

		//buttonBackState = ButtonState_Type.Selected;
		Active_ButtonBackImage(buttonBackState);


		//나머지 행동
        if(UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.EQUIPMENT))
		    UI_Equipment.Getsingleton.Doprocess_SelectedEquipItem(this);
        else if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CHARACTERSETTING))
            UI_CharacterSetting .Getsingleton.Doprocess_SelectedEquipItem(this);
    }


}
