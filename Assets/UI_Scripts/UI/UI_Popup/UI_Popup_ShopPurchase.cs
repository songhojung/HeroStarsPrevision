using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Popup_ShopPurchase :UI_PopupBase 
{

	public List<GameObject> Lst_popupInfoOBJ;

	public Text text_packageMain;
	public List<Text> Lst_textprice;
	public List<Image> Lst_imageBuyIcon;

	//스폐셜
	public GameObject buySpecialSoldOut;			//스폐셜구입 솔드아웃오브젝트


	private Infos_Shop InfosShop;
	private STOREMODE_TYPE ShopType = STOREMODE_TYPE.NONE;
	private Package_Type packageType;
	private int activeIDX = 0;




	public override void Set_Open()
	{
		base.Set_Open();
	}
	public override void Set_Close()
	{
		base.Set_Close();


		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
		{
			if (UI_Top.Getsingleton.popupStore.gameObject.activeSelf == true)
			{
				//상점 팝업창이 아직 살아 있으니 isSelectedCharacter 은 true로 하자
				User.isSelectedCharacter = true;
			}
			else
			{
				//캐릭로비 로테이트 활성
				User.isSelectedCharacter = false;
			}
		}


		
	}
	public override void Set_Refresh()
	{
		base.Set_Refresh();
	}

	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}


	public void Set_ShopPuchase(Infos_Shop _infosShop, STOREMODE_TYPE _storemode)
	{
		 InfosShop =_infosShop;
		 ShopType = _storemode;

		//ui 정보 적용
		 Apply_popupUI();
	}



	//패키지시 인덱스 등 파라미터 설정
	void Apply_PackageParameter()
	{
		//패키지 타입
		packageType = (Package_Type)(InfosShop.ShopIdx % ((int)STOREMODE_TYPE.Package * 100));


		//활성될 인덱스 설정
		if (packageType == Package_Type.Special)
			activeIDX = 1;
		else
			activeIDX = 0;
	}

	//ui 정보 적용
	void Apply_popupUI()
	{

		string buyIconName = string.Empty;

		//패키지시 인덱스 등 파라미터 설정
		Apply_PackageParameter();

		// 활성시킬 팝업내용오브젝트 
		Active_popupOBJ(activeIDX);


		switch (ShopType)
		{
			case STOREMODE_TYPE.Gold:
				buyIconName = DefineKey.Gem;
				break;
			case STOREMODE_TYPE.Gem:
				buyIconName = DefineKey.Cash;

				break;
			case STOREMODE_TYPE.Package:
				buyIconName = DefineKey.Cash;

				//스폐셜패키지 예외처리
				if (packageType == Package_Type.Special)
				{
					if (UserDataManager.instance.user.chk_PurchaseLimit(InfosShop.PurLimit))
						buySpecialSoldOut.SetActive(true); //솔드아웃 활성
					else
						buySpecialSoldOut.SetActive(false); // 솔드아웃 비활성
				}

				//메인텍스트
				int textIdx = 21000 + (int)packageType;
				text_packageMain.text = TextDataManager.Dic_TranslateText[textIdx];

				break;
			case STOREMODE_TYPE.Special:
				//구매아이콘이름
				buyIconName = DefineKey.Gem;

				break;

			default:
				break;
		}

		//구매 아이템아이콘
		Lst_imageBuyIcon[activeIDX].sprite = ImageManager.instance.Get_Sprite(buyIconName);


		//ui 공통사항 설정 
		UIInfo_Common();
	}


	//ui 공통사항 설정 
	void UIInfo_Common()
	{

		//구매 가격표시
#if UNITY_EDITOR
		Lst_textprice[activeIDX].text = InfosShop.BuyItNum.ToString();
#else
		if (InfosShop.ShopIdx / 100 == 2 || InfosShop.ShopIdx/100 ==3)
			Lst_textprice[activeIDX].text = InAppPurchaseManager.instance.Get_priceForMobile(InfosShop.ShopIdx);
		else
			Lst_textprice[activeIDX].text = InfosShop.BuyItNum.ToString();
#endif
	
	}







	// 활성시킬 팝업내용오브젝트 
	void Active_popupOBJ(int _activeIDX)
	{

		for (int i = 0; i < Lst_popupInfoOBJ.Count; i++)
			Lst_popupInfoOBJ[i].SetActive(i == _activeIDX);
	}



	public void ResponseButton_OK()
	{
		if (ShopType == STOREMODE_TYPE.Gold)
		{
			webRequest.ShopBuyItem(InfosShop.ShopIdx, callback_complete_purchase);
		}
		else
		{

#if UNITY_EDITOR
			if (packageType == Package_Type.Special)
			{
				//상점팝업닫기
				if (UI_Top.Getsingleton.popupStore.gameObject.activeSelf == true)
					UI_Top.Getsingleton.popupStore.gameObject.SetActive(false);

				//현재팝업 끄기
				UI_Manager.Getsingleton.ClearUI(this);

				//test
				//webRequest.ShopBuyUnit(10009, 100, callback_complete_BuySpecial);
				webRequest.ShopBuyItem(InfosShop.ShopIdx, callback_complete_BuySpecial);
			}
			else
				webRequest.ShopBuyItem(InfosShop.ShopIdx, callback_complete_purchase);

#else
			InAppPurchaseManager _IAP = InAppPurchaseManager.instance;


			if (packageType == Package_Type.Special)
			{
				//상점팝업닫기
				if (UI_Top.Getsingleton.popupStore.gameObject.activeSelf == true)
					UI_Top.Getsingleton.popupStore.gameObject.SetActive(false);

				//현재팝업 끄기
				UI_Manager.Getsingleton.ClearUI(this);

				_IAP.nextPrcs = callback_complete_BuySpecial;
			}
			else
				_IAP.nextPrcs = callback_complete_purchase;
			
			
			_IAP.RequestPayment(_IAP.Get_ProductID(InfosShop.ShopIdx), InfosShop.ShopIdx);
		
	
#endif
		}
		

	}

	//골드 ,보석, 패키지 구매 콜백 함수 , 구매후 TopUI refresh 하기
	void callback_complete_purchase()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Top.Getsingleton.set_refresh();

		// 월정액 상품 , 패키지 상품 정보 갱신
		UI_Top.Getsingleton.popupStore.Refresh_store();

		UI_Manager.Getsingleton.ClearUI(this);
	}



	//스폐셜상품 구매후 처리
	void callback_complete_BuySpecial()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		UI_Top.Getsingleton.set_refresh();

		// 월정액 상품 , 패키지 상품 정보 갱신
		UI_Top.Getsingleton.popupStore.Refresh_store();




			//이벤트 캐릭 unitidx 찾기
			uint boughtSpecialUnitidx = 0;
			if (TableDataManager.instance.Infos_EventShopItems.ContainsKey(InfosShop.ShopIdx))
			{
				Dictionary<byte, Infos_EventShopItem> evtShopItems = TableDataManager.instance.Infos_EventShopItems[InfosShop.ShopIdx];
				foreach (var evnItem in evtShopItems)
				{
					if (evnItem.Value.SellItTp == ITEMTYPE.UNIT)
					{
						boughtSpecialUnitidx = evnItem.Value.SellItIdx;
						break;
					}
				}

			}

			UserDataManager.instance.user.User_useUnit.UnitIdx = boughtSpecialUnitidx;

			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
			{

				//로비캐릭 갱신
				if (boughtSpecialUnitidx != 0)
				{


                    //UI_LobbyOld
                    UI_LobbyOld.Getsingleton.set_refresh();
					UI_LobbyOld.Getsingleton.Dic_lobbyCharacter[boughtSpecialUnitidx].playBuyEffect(0.5f);
				}

			}
	}



	public void ResponseButton_Cancle()
	{
		UI_Manager.Getsingleton.ClearUI(this);
	}


	
}
