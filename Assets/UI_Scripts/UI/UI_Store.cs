using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;



public class UI_Store : UI_Base 
{
	private Dictionary<ushort, Infos_Shop> Dic_InfosShop = new Dictionary<ushort, Infos_Shop>();
	//private Dictionary<uint, Infos_Unit_shops> Dic_unitShops = new Dictionary<uint, Infos_Unit_shops>();
	//private Dictionary<int, Dictionary<int, Infos_Itembox_reward>> Dic_InfosItemBoxRw = new Dictionary<int, Dictionary<int, Infos_Itembox_reward>>();
	//private Dictionary<int, Infos_Itembox> Dic_Infos_ItemBox = new Dictionary<int, Infos_Itembox>();
	private List<UIItem_StoreElement> Lst_storeElement = new List<UIItem_StoreElement>();

	public Transform Tr_Content;
	public int nowTapIdx = 0;
	private STOREMODE_TYPE nowStoreMode = STOREMODE_TYPE.Gold;
	private UI_Manager ui_Mgr;

	public List<Toggle> Lst_Toggle;

	private static UI_Store _instance;
	public static UI_Store Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Store)) as UI_Store;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("UI_Store");
					_instance = instanceObj.AddComponent<UI_Store>();
				}
			}

			return _instance;
		}
	}

	public override void set_Open()
	{
		base.set_Open();


		ui_Mgr = UI_Manager.Getsingleton;
		Dic_InfosShop = TableDataManager.instance.Infos_shops;	
		

		Lst_Toggle[3].isOn = true;
	}

	public override void set_Close()
	{
		base.set_Close();

		Clear_Element();
		//ui_Mgr.ClearUI_Item(UI.STORE);
	}

	public override void set_refresh()
	{
		base.set_refresh();

		Clear_Element();

		Creat_Element(nowStoreMode);
	}


	public void Set_Element(STOREMODE_TYPE storeMode)
	{
			Creat_Element(storeMode);
	}


	/// <summary>
	/// 각 탭별로 element생성
	/// </summary>
	public void Creat_Element(STOREMODE_TYPE storeMode)
	{

		foreach (var itemshop in Dic_InfosShop)
		{
			if (ChkShopMode(itemshop.Value.SellItTp, storeMode))
			{
				//생성할 element 생성및 정보설정
				CreateEleInfo(itemshop.Value, storeMode);
			}
		}

		//sortIdx 값에 따른 아이템들정렬하기 
		Set_SortOrderByValue(Lst_storeElement);

	}

	//생성할 element 생성및 정보설정
	void CreateEleInfo(Infos_Shop shopinfo,STOREMODE_TYPE _storemode)
	{
		UIItem_StoreElement storeEle = ui_Mgr.CreatUI(UIITEM.ITEM_STOREELEMENT, Tr_Content) as UIItem_StoreElement;

		BonusInfo bouns = new BonusInfo(0, 0, (int)shopinfo.BnsItNum, (ITEMTYPE)shopinfo.BnsItTp, (int)shopinfo.BnsIdx);
		storeEle.Set_info(shopinfo, bouns, _storemode);
		Lst_storeElement.Add(storeEle);
	}


	//상점인덱스가 현재 상점모드랑 맞는지 체크
	bool ChkShopMode(ushort _shopIdx,STOREMODE_TYPE _storemode)
	{
		bool isRight = false;
		int quota = 0;
		if(_storemode == STOREMODE_TYPE.Gem) quota = 2;
		else if(_storemode == STOREMODE_TYPE.Gold) quota = 1;
		else if(_storemode == STOREMODE_TYPE.Package) quota = 3;

		isRight = (_shopIdx / 100 == quota) ? true : false;

		return isRight;
	}

	void Clear_Element()
	{
		for (int i = 0 ; i< Lst_storeElement.Count; i++)
		{
			Destroy(Lst_storeElement[i].gameObject);
		}
		Lst_storeElement.Clear();
	}

	//sortIdx 값에 따른 아이템들정렬하기 
	void Set_SortOrderByValue(List<UIItem_StoreElement> _UIItem_StoreElement)
	{
		for (int i = 0; i < _UIItem_StoreElement.Count; i++ )
		{
			_UIItem_StoreElement[i].transform.SetSiblingIndex((int)_UIItem_StoreElement[i].infos_shop.SortIdx);
		}
	}


	
	/// <summary>
	/// 다른UI에서 상점으로 이동시 셋업해줘야할탭 설정
	/// </summary>
	public void Start_OnTap(ITEMTYPE storeMode)
	{
		
		if (storeMode == ITEMTYPE.GOLD)
		{
			Lst_Toggle[(int)ITEMTYPE.GOLD -1].isOn = true;
		}
		else if (storeMode == ITEMTYPE.GEM)
		{
			Lst_Toggle[(int)ITEMTYPE.GEM -1 ].isOn = true;
		}
	}

	public void ResponseButton_tap(int tapIdx)
	{

		if (nowTapIdx != tapIdx)
		{
			
			nowTapIdx = tapIdx;
			nowStoreMode = (STOREMODE_TYPE)nowTapIdx;

			Clear_Element();

			Set_Element(nowStoreMode);
		}

		
	}






	public void ResponseButton_Back()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		if (UI_Manager.Getsingleton.goBackUI != UI.NONE && UI_Manager.Getsingleton.goBackUI != UI.CHAT) // chat UI 도 goback 기능이 있어서 두개가 같이 되면 오류발생
		{
			UI_Manager.Getsingleton.CreatUI(UI_Manager.Getsingleton.goBackUI, _canvasTr);
			UI_Manager.Getsingleton.goBackUI = UI.NONE;
		}
		else
		{
			
			UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
		}
	}
}
