using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Prime31;


public struct BonusInfo
{
	public int MinUnitRank;
	public int MaxUnitRank;
	public int BonusNum;
	public ITEMTYPE BonusType;
	public int bonusIdx;

	public BonusInfo(int _MinRandPer, int _MaxRandPer, int _BonusNum, ITEMTYPE _BonusType, int _bonusIdx)
	{
		MinUnitRank = _MinRandPer;
		MaxUnitRank = _MaxRandPer;
		BonusNum = _BonusNum;
		BonusType = _BonusType;
		bonusIdx = _bonusIdx;
	}
}

public class UIItem_StoreElement : UI_Base 
{
	private enum Enum_Object
	{
		DiscountLine = 0,
		Bonus = 1,
		SoldOut = 2,
		UnlockTime = 3,
		IconBonus = 4,
	}



	private enum MainTextType
	{
		normalMain = 0,
		AdMain = 1,
	}

	[HideInInspector]
	public Infos_Shop infos_shop;

	

	[HideInInspector]
	public BonusInfo bounusInfo;




	public STOREMODE_TYPE nowSotreMode = STOREMODE_TYPE.NONE;


	//월정액 시간잠금
	public GameObject UnlockTimeObj;
	public Text UnlockRemainTime;				//잠금남은시간


	//버튼
	public Button button_buyItem;

	//메인UI
	public Image image_Icon;
	public Image Image_useGoods;
	public Text text_Title;
	public Text text_SubTitle;
	public List<GameObject> List_maintextOBJ;
	public List<Text> List_textMain;
	public Text text_PriceBefore;
	public Text text_PriceAfter;
	public List<GameObject> Lst_Obj;


	//보너스
	public Image image_bonusIcon;
	public Text text_bonusValue;


	public override void set_Open()
	{
		base.set_Open();

		
	}

	public override void set_Close()
	{
		base.set_Close();
	}


	public void Set_info(Infos_Shop _infos_shop, BonusInfo _bounusInfo, STOREMODE_TYPE _nowStoreMode)
	{
		nowSotreMode = _nowStoreMode;
		infos_shop = _infos_shop;
		bounusInfo = _bounusInfo;

		ApplyInfo(nowSotreMode);
	}



	public void ApplyInfo(STOREMODE_TYPE nowStoreMode)
	{
		string bodyText = string.Empty;
		string bonusText = string.Empty;

		//오픈 여부 체크 !!!
		Chk_openFlag();


		if (nowStoreMode == STOREMODE_TYPE.Gold)
		{
			//버튼 설정
			button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gold);
			button_buyItem.onClick.AddListener(ResponseButton_Buy_Gold);

			image_Icon.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.shopIcon, infos_shop.ShopIdx));

			text_Title.text = string.Format("{0} {1}", infos_shop.SellItNum, TextDataManager.Dic_TranslateText[36]); 

			
		}
		else if (nowStoreMode == STOREMODE_TYPE.Gem)
		{
			//버튼 설정
			button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
			button_buyItem.onClick.AddListener(ResponseButton_Buy_Gem);

			image_Icon.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.shopIcon, infos_shop.ShopIdx));

			text_Title.text = string.Format("{0} {1}", infos_shop.SellItNum, TextDataManager.Dic_TranslateText[37]);



			

		}
		else if (nowStoreMode == STOREMODE_TYPE.Package)
		{

			Package_Type packageIdx = (Package_Type)(infos_shop.ShopIdx % ((int)STOREMODE_TYPE.Package * 100));



            //이미지
            if (infos_shop.OpenFlg != 0) //오픈아닐때 스프라이트 처리안함 우선 패키지쪽만 예외적으로 
                image_Icon.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.shopIcon, infos_shop.ShopIdx));
			//구석탱이보너스 아이콘
			if (packageIdx == Package_Type.Month)
				Lst_Obj[(int)Enum_Object.IconBonus].SetActive(false);
			else
				Lst_Obj[(int)Enum_Object.IconBonus].SetActive(true);

			//패키지 상품 적용시킬 것들
			Apply_ShopPackage(packageIdx);
		
			////타이틀 텍스트
			int textIdx = 20000 + (int)packageIdx;
			switch (packageIdx)
			{
				case Package_Type.Month: //월정액
					text_Title.text =  string.Format( TextDataManager.Dic_TranslateText[textIdx], infos_shop.SellItNum);//25일 패키지
					break;
				case Package_Type.NewUser: //신규유저 //신규유저 패키지 단 1회
				case Package_Type.Gold_1: //골드패키지1
				case Package_Type.Gold_2: //골드패키지2
				case Package_Type.Substance_1: //실속패키지1
				case Package_Type.Substance_2: //실속패키지2
				case Package_Type.Substance_3: //실속패키지3
				case Package_Type.Special: //실속패키지3
					text_Title.text =  TextDataManager.Dic_TranslateText[textIdx];
					break;
			
			
			}
			
			
		}
		else if (nowStoreMode == STOREMODE_TYPE.Special)
		{


			//이미지
			image_Icon.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.shopIcon, infos_shop.ShopIdx));
			//구석탱이보너스 아이콘
				Lst_Obj[(int)Enum_Object.IconBonus].SetActive(true);


			if (UserDataManager.instance.user.chk_PurchaseLimit(infos_shop.PurLimit))
			{
				//스폐셜상품 구매했음
				Lst_Obj[(int)Enum_Object.SoldOut].SetActive(true); //솔드아웃 활성
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
			}
			else
			{
				//스폐셜상품 구매안했음
				Lst_Obj[(int)Enum_Object.SoldOut].SetActive(false); //솔드아웃 비활성

				//버튼 설정
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
				button_buyItem.onClick.AddListener(ResponseButton_Buy_Gem);
			}

			//타이틀 텍스트
			//text_Title.text = string.Format(TextDataManager.Dic_TranslateText[textIdx], infos_shop.SellItNum);//25일 패키지

			

		}



		//광고제거상품 체크
		if (infos_shop.AdDelFlg == 1)
		{
			Activate_MaintextOBJ(MainTextType.AdMain);
			//bodyText = "광고제거 추가";
		}

		// 메인텍스트 
		List_textMain[(int)MainTextType.normalMain].text = bodyText;//본내용 텍스트
		//List_textMain[(int)MainTextType.AdMain].text = bodyText;//광고보기제거용 텍스트
	
		
		//구매할 아이템이미지 설정
		Apply_ImageUsergoods((ITEMTYPE)infos_shop.BuyItTp);
		

		image_Icon.SetNativeSize();
		Image_useGoods.SetNativeSize();



		//=============================== 보너스  ==========================
		if (bounusInfo.BonusType != ITEMTYPE.NONE) // 보너스 아이템 갯수가 있고, 보너스아이템타입이 NONE 아니면 보너스 표시 한다
		{

				
				 if (bounusInfo.BonusType == ITEMTYPE.GOLD)
				{
					Lst_Obj[(int)Enum_Object.Bonus].SetActive(true);
					image_bonusIcon.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);
					image_bonusIcon.SetNativeSize();
					text_bonusValue.text = string.Format("{0}", bounusInfo.BonusNum);


				}
				else if (bounusInfo.BonusType == ITEMTYPE.GEM)
				{
					Lst_Obj[(int)Enum_Object.Bonus].SetActive(true);
					image_bonusIcon.sprite = ImageManager.instance.Get_Sprite( DefineKey.Gem);
					image_bonusIcon.SetNativeSize();
					text_bonusValue.text = string.Format("{0}", bounusInfo.BonusNum);

				}
				else
				{
					Lst_Obj[(int)Enum_Object.Bonus].SetActive(false);
				}
		}
		else  // 보너스 아이템 갯수가 없고 보너스아이템타입이 NONE 이면 보너스 표시 안한다
		{
			Lst_Obj[(int)Enum_Object.Bonus].SetActive(false);

		}



		
		
		//할인 이전가격
		if (infos_shop != null)
		{
			if (infos_shop.BuyOriItNum != infos_shop.BuyItNum && infos_shop.BuyOriItNum != 0)
			{
				Lst_Obj[(int)Enum_Object.DiscountLine].SetActive(true);
//#if UNITY_IOS
//                text_PriceBefore.text = infos_shop.OriginPrice;
//#else
//                text_PriceBefore.text = infos_shop.BuyOriItNum.ToString();
//#endif

				text_PriceBefore.text = Get_orinPrice();
			}
			else
			{
				Lst_Obj[(int)Enum_Object.DiscountLine].SetActive(false);
			}
		}



//#if UNITY_EDITOR
		text_PriceAfter.text = infos_shop.BuyItNum.ToString();
//#else
		//if (infos_shop.ShopIdx / 100 == 2 || infos_shop.ShopIdx/100 ==3)
		//	text_PriceAfter.text = InAppPurchaseManager.instance.Get_priceForMobile(infos_shop.ShopIdx);
		//else
		//	text_PriceAfter.text = infos_shop.BuyItNum.ToString();
//#endif

	}



	string Get_orinPrice()
	{
		string orinPriceStr = string.Empty;
		int OriginPriceRatio = Convert.ToInt32(infos_shop.OriginPrice);
		string vPrice = string.Empty ;

//#if UNITY_EDITOR
			orinPriceStr = infos_shop.BuyOriItNum.ToString();


//#elif UNITY_ANDROID

//		GoogleSkuInfo _skus = InAppPurchaseManager.instance.Get_AOSgoogleSkuInfo(infos_shop.ShopIdx);
//		//Debug.Log("priceCurrencyCode : " + _skus.priceCurrencyCode);
//		vPrice = _skus.price;
//		string currency = vPrice.Remove(1, vPrice.Length - 1);// 화폐단위 얻기
//		string reStr = vPrice.Remove(0, 1); //\  문자 지우기
//		double dprice = Convert.ToDouble(reStr);
//		dprice = dprice * OriginPriceRatio;
//		orinPriceStr = string.Format("{0}{1}",currency, dprice);


//#elif UNITY_IOS
//		StoreKitProduct stkit = InAppPurchaseManager.instance.Get_StoreKitPtInfoIOS(infos_shop.ShopIdx);
//		vPrice = stkit.price;

//		double ddPrice = Convert.ToDouble(vPrice);

//		ddPrice = ddPrice * OriginPriceRatio;

//		orinPriceStr = string.Format("{0}{1}",stkit.currencySymbol, ddPrice);

	
//#endif

		return orinPriceStr;
	}




	//패키지 상품 적용시킬 것들
	void Apply_ShopPackage(Package_Type _packageIdx)
	{
		if (_packageIdx == Package_Type.Month)
		{
			//월정액 남을 일수 있으면 버튼 막고 남은시간 표시
			int RemainMonthCash = UserDataManager.instance.user.Get_user_goods(ITEMTYPE.MONTH_CASH);
			if (RemainMonthCash > 0)
			{
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);

				Lst_Obj[(int)Enum_Object.UnlockTime].SetActive(true);
				UnlockRemainTime.text = string.Format("{0}{1}", RemainMonthCash, TextDataManager.Dic_TranslateText[181]); // ~일
			}
			else
			{
				Lst_Obj[(int)Enum_Object.UnlockTime].SetActive(false);
				//버튼 설정
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
				button_buyItem.onClick.AddListener(ResponseButton_Buy_Gem);
			}
		}
		else 
		{

			if (UserDataManager.instance.user.chk_PurchaseLimit(infos_shop.PurLimit))
			{
				//패키지 구매했음
				Lst_Obj[(int)Enum_Object.SoldOut].SetActive(true); //솔드아웃 활성
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
			}
			else
			{
				//패키지 구매안했음
				Lst_Obj[(int)Enum_Object.SoldOut].SetActive(false); //솔드아웃 비활성

				//버튼 설정
				button_buyItem.onClick.RemoveListener(ResponseButton_Buy_Gem);
				button_buyItem.onClick.AddListener(ResponseButton_Buy_Gem);
			}

			
			
		}
	}



	//구매할 아이템 이미지 설정
	void Apply_ImageUsergoods(ITEMTYPE buyItType)
	{
		switch (buyItType)
		{
			case ITEMTYPE.NONE:
				//구매 할 아이템
				Image_useGoods.sprite = ImageManager.instance.Get_Sprite(DefineKey.Cash);
				break;
			case ITEMTYPE.GEM:
				//구매 할 아이템
				Image_useGoods.sprite = ImageManager.instance.Get_Sprite( DefineKey.Gem);
				break;
			case ITEMTYPE.GOLD:
				//구매 할 아이템
				Image_useGoods.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);
				break;
			
		}
	}


	//오픈여부 체크
	void Chk_openFlag()
	{
		if (infos_shop.OpenFlg == 0)
			gameObject.SetActive(false);
		else if (infos_shop.OpenFlg >= 1)
			gameObject.SetActive(true);
	}


	void Activate_MaintextOBJ(MainTextType type)
	{
		for (int i = 0; i < List_maintextOBJ.Count; i++)
		{
			if (i == (int)type)
				List_maintextOBJ[i].SetActive(true);
			else
				List_maintextOBJ[i].SetActive(false);
		}
	}








	





	//골드 구매 버튼 함수
	public void ResponseButton_Buy_Gold()
	{
		//UI_Popup_BuyItem popup = ui_mgr.CreatAndGetPopup<UI_Popup_BuyItem>(UIPOPUP.POPUPBUYITEM);
		//string _massage = string.Format("{0} {1} {2}", infos_shop.SellItNum, TextDataManager.Dic_TranslateText[36], TextDataManager.Dic_TranslateText[138]);
		//popup.Set_info(_massage, infos_shop.BuyItNum.ToString(), Image_useGoods.sprite);
		//popup.Set_AddEventButton(callback_buy_GoldGem);

		webRequest.ShopBuyItem(infos_shop.ShopIdx, callback_complete_BuyGold);
	}


	void callback_complete_BuyGold()
	{
		//탑 갱신
		UI_Top.Getsingleton.set_refresh();

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("에디터는 골드 통계 안넣음");
#else
		//골드 통계넣쟈
		//UserEditor.Getsingleton.EditLog("에디터는 골드 통계 !");
		//string productTile = string.Format("골드 {0}", infos_shop.SellItNum);
		//AnalysisManager.instance.Anl_Purchace(productTile, "kr", (double)infos_shop.BuyItNum);
#endif
	}
	
	//보석구매 버튼 함수
	public void ResponseButton_Buy_Gem()
	{

		//패키지 상품은 팝업띄우기
		if (nowSotreMode == STOREMODE_TYPE.Package || nowSotreMode == STOREMODE_TYPE.Special)
		{
			UI_Popup_ShopPurchase popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ShopPurchase>(UIPOPUP.POPUPSHOPPURCHASE);
			popup.Set_ShopPuchase(infos_shop, nowSotreMode);
		}
		else
		{

//#if UNITY_EDITOR
			webRequest.ShopBuyItem(infos_shop.ShopIdx, callback_buy_GoldGem);

//#else

		
//		InAppPurchaseManager _IAP = InAppPurchaseManager.instance;
//		_IAP.nextPrcs = callback_buy_GoldGem;
//		_IAP.RequestPayment(_IAP.Get_ProductID(infos_shop.ShopIdx), infos_shop.ShopIdx);
		
		
//#endif

		}

	}

	//골드 및 보석 구매 콜백 함수 , 구매후 TopUI refresh 하기
	void callback_buy_GoldGem()
	{
		UI_Top.Getsingleton.set_refresh();


		// 월정액 상품 , 패키지 상품 정보 갱신
		UI_Top.Getsingleton.popupStore.Refresh_store();
	

		
	}
}
