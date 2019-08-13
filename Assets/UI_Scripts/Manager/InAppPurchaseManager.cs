	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using System.IO;
using System;


public class InAppPurchaseManager
{


	public static InAppPurchaseManager instance = new InAppPurchaseManager();
	private string buyProductID;
	private ushort ShopIdx = 0;
	public bool IsGetQueryInventory = false;	//인앱상품이 다 받아졋는지 확인 변수
	private string[] productIDs;				//인앱빌링 초기화를 위한 상품이름들
	public Dictionary<ushort, string> Dic_productIDs; //상점 인덱스 키에 따른 인앱상품이름

	private GameObject Loadingobj = null;												// 상품 구매중 필요한 로딩바 오브젝트 (IOS 에만적용)
	public bool isPurchasing = false;													//구매진행중?
	public bool isForceConsume = false;													//강제소모 실행하냐?

	public del_NextProcess nextPrcs;

#if UNITY_ANDROID
	public List<GooglePurchase> lst_NoneConsumedProduct = new List<GooglePurchase>();				//소모하지않은 구매상품들
	List<GoogleSkuInfo> lst_Product = new List<GoogleSkuInfo>();							//구글에서 받온 상품리스트들
	Dictionary<string, GoogleSkuInfo> Dic_ProductInfo = new Dictionary<string, GoogleSkuInfo>(); //상품ID 키값에 대한 구글에 받아온 상품리스트 들

#elif UNITY_IOS
	List<StoreKitProduct> Products = new List<StoreKitProduct>();
	StoreKitTransaction transactionProduct ; 										 // 구매 진행한 상품
											
	Dictionary<string, StoreKitProduct> Dic_StorkitProductInfo = new Dictionary<string, StoreKitProduct> (); 	//상품ID 키값에 대한 애플 에서 받아온 상품리스트 들
	public List<StoreKitTransaction> storekitTracs = new List<StoreKitTransaction>();			// 소모되지 않아 다시 소모 처리 시도된 상품
#endif



	//결제 관련 정보위해 초기화 
	public void Init_Payment()
	{
		////상품이름이 없으면 초기화 진행하자
		if (Dic_productIDs == null)
		{
			Dic_productIDs = new Dictionary<ushort, string>()
			{
				{201,"com.cle.dy.suddenground.gem_200"},
				{202,"com.cle.dy.suddenground.gem_1000"},
				{203,"com.cle.dy.suddenground.gem_2000"},
				{204,"com.cle.dy.suddenground.gem_6000"},
				{205,"com.cle.dy.suddenground.gem_12000"},
				{301,"com.cle.dy.suddenground.cash_1"},
				{302,"com.cle.dy.suddenground.cash_2"},
				{303,"com.cle.dy.suddenground.cash_3"},
				{304,"com.cle.dy.suddenground.cash_4"},
				{305,"com.cle.dy.suddenground.cash_5"},
				{306,"com.cle.dy.suddenground.cash_6"},
				{307,"com.cle.dy.suddenground.cash_7"},
				{308,"com.cle.dy.suddenground.cash_8"},
				
			};

			productIDs = new string[Dic_productIDs.Count];
			int idx = 0;
			foreach (var pId in Dic_productIDs)
			{
				productIDs[idx] = pId.Value;
				idx++;
			}

			//productIDs = new string[] { "com.cle.dy.suddenground.gem_200", "com.cle.dy.suddenground.gem_1000","com.cle.dy.suddenground.gem_2000"
			//,"com.cle.dy.suddenground.gem_6000","com.cle.dy.suddenground.gem_12000", "com.cle.dy.suddenground.cash_1",
			//"com.cle.dy.suddenground.cash_2","com.cle.dy.suddenground.cash_3",/*"com.cle.dy.suddenground.cash_4",*/"com.cle.dy.suddenground.cash_5"};



			//int _productCount = productIDs.Length;
			//int idx = 0;
			//
			//foreach (var shopItem in TableDataManager.instance.Infos_shops)
			//{
			//	if (idx < _productCount)
			//	{
			//		if (shopItem.Value.ShopIdx / 100 == 2 || shopItem.Value.ShopIdx / 100 == 3 || shopItem.Value.ShopIdx / 100 == 4) //상품인덱스가 200 번대 이거나 300번대 이거나 400번대
			//		{
			//			Dic_productIDs[shopItem.Value.ShopIdx] = productIDs[idx];
			//			idx++;
			//		}
			//
			//	}
			//}

			Debug.Log("초기화~~~~~~~~~~~~~Dic_productIDs");
			Prime31.Utils.logObject(Dic_productIDs);

			//에디터는 IsGetQueryInventory 값 true
#if UNITY_EDITOR

			IsGetQueryInventory = true;
#endif


			//각 플랫폼별로 인앱결제초기화하기
#if UNITY_ANDROID
			//구글개발자 센터에서받은 어플리케이션 라이센스키 (구글콘솔개발자센터 -> 개발도구 -> 서비스및 API ->어플리케이션 라이센스키 )
			string key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqADPACKXd7+ygFtcAnh5wrpULFbK9cbPch2v0EOfzzsn1mOTJk3lFT432GKcFUpj01oGEha2bIt8e6YEFVgbYIIo3JRPYfrUMsqRGf82ItyGLwzZm2DNodnGJpRXvI0hqHrH0Z1aTpME6cgu9GrtbaqZ8t3pqZ38/w1nKUDfGzJ0FurS9zyWW+x7e4tTRnrBAxnQFk5OXmazW2LIT1+9vq9AI5ad6MntT7ZufubbTbbzX6kwzEUaiWRJQjZhCQzyq/MgsXCCgHTJ1GjzZZY2Zal9w/kX7s+z3T61ElT65zrz1rKBJRhFzp/CdGetrBFJvmYB3vfpxnIABu2EgF7jQQIDAQAB";
			GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
			GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
			GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;

			GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;

			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;

			//상품 구매 성공 콜백함수
			GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
			//상품 구매 실패 콜백함수
			GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;

			//상품 컨슘(소모)처리 성공 콜백함수
			GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
			//상품 컨슘(소모)처리 실패 콜백함수
			GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;

			// 인앱결제 초기화
			GoogleIAB.init(key);
			UserEditor.Getsingleton.EditLog("안드로이드 인앱결제 초기화완료");
#elif UNITY_IOS

		UserEditor.Getsingleton.EditLog("IOS 인앱결제 초기화");

		StoreKitManager.transactionUpdatedEvent += transactionUpdatedEvent;
		StoreKitManager.productPurchaseAwaitingConfirmationEvent += productPurchaseAwaitingConfirmationEvent;
		StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessfulEvent;
		StoreKitManager.purchaseCancelledEvent += purchaseCancelledEvent;
		StoreKitManager.purchaseFailedEvent += purchaseFailedEvent;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailedEvent;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
		StoreKitManager.paymentQueueUpdatedDownloadsEvent += paymentQueueUpdatedDownloadsEvent;

		StoreKitManager.productListReceivedEvent += allProducts =>
		{
		    UserEditor.Getsingleton.EditLog( "received total products: " + allProducts.Count );
		    Products = allProducts;
		};

		//자동 컨슘처리 비활성화 -> 내가 컨슘처리할것이기 때문
		StoreKitManager.autoConfirmTransactions = false;
		
		StoreKitBinding.requestProductData(productIDs);
#endif
		}
	}


	//결제요청 하기
	public void RequestPayment(string _productID,ushort _shopIdx)
	{
		UserEditor.Getsingleton.EditLog("purchase productID :" + _productID);
		UserEditor.Getsingleton.EditLog("purchase _shopIdx :" + _shopIdx);
		buyProductID = _productID;
		ShopIdx = _shopIdx;

		

#if UNITY_ANDROID
		Loadmanager.instance.LoadingUI(true);
		GoogleIAB.purchaseProduct(_productID);
#elif UNITY_IOS

		//구매진행중 ON
		isPurchasing = true; 

		//로딩방 활성화
		Loadmanager.instance.LoadingUI(true);

		StoreKitBinding.purchaseProduct(_productID,1);
#endif
	}


	public void CompletedPaymentProcess()
	{
		Loadmanager.instance.LoadingUI(false);
	}


	public string Get_ProductID(ushort _shopIdx)
	{
		return Dic_productIDs[_shopIdx];
	}

	public ushort Get_GemShopIdx(string _productID)
	{
		ushort _shopIdx = 0;
		foreach (var item in Dic_productIDs) 
		{
			if (item.Value.Equals (_productID)) 
			{
				_shopIdx = item.Key;
				break;
			}
		}
		return _shopIdx;
	}

	public string Get_priceForMobile(ushort _shopIdx)
	{
		
		string price = string.Empty;
#if UNITY_ANDROID
		if (Dic_ProductInfo.ContainsKey(Get_ProductID(_shopIdx)))
			price = Dic_ProductInfo[Get_ProductID(_shopIdx)].price;
#elif UNITY_IOS
		if(Dic_StorkitProductInfo.ContainsKey(Get_ProductID(_shopIdx)))
			price = Dic_StorkitProductInfo[Get_ProductID(_shopIdx)].formattedPrice;

#endif
		return price;
	}
	

#if UNITY_ANDROID

	public string Get_priceForAOS(ushort _shopIdx)
	{
		string price = string.Empty;
		if (Dic_ProductInfo.ContainsKey(Get_ProductID(_shopIdx)))
			price = Dic_ProductInfo[Get_ProductID(_shopIdx)].price;

		return price;
	}


	public GoogleSkuInfo Get_AOSgoogleSkuInfo(ushort _shopIdx)
	{
		GoogleSkuInfo _sku = null;
		if (Dic_ProductInfo.ContainsKey(Get_ProductID(_shopIdx)))
			_sku = Dic_ProductInfo[Get_ProductID(_shopIdx)];
		return _sku;
	}


	//구글결제 구매성공시 콜백함수
	void purchaseSucceededEvent(GooglePurchase purchase)
	{
		UserEditor.Getsingleton.EditLog("purchaseSucceededEvent: " + purchase);

		
		//웹서버에 구매영수증 보냄
		if (Dic_productIDs.ContainsValue(purchase.productId))
		{
			webRequest.ShopBuyItem(ShopIdx, purchase.originalJson, purchase.signature, callback_complete_buyGem);
		}
	}

	// 인앱결제 성공 웹서버 콜백 
	void callback_complete_buyGem()
	{
		//결제 성공 시 바로 소모 전송
		UserEditor.Getsingleton.EditLog("인앱결제상품 소모하기 ~~ : " + buyProductID);
		GoogleIAB.consumeProduct(buyProductID);


		//구매완료했으니 통계에 넣자 
		GoogleSkuInfo buyProductInfo = Dic_ProductInfo[buyProductID];
		string reStr = buyProductInfo.price.Remove(0, 1); //\  문자 지우기
		double dprice = Convert.ToDouble(reStr);
		//AnalysisManager.instance.Anl_Purchace(buyProductInfo.title, buyProductInfo.priceCurrencyCode, dprice);


		if (nextPrcs != null)
			nextPrcs();
		////top 갱신
		//UI_Top.Getsingleton.set_refresh();

		//if (Get_GemShopIdx(buyProductID) == 301) // 월정액 상품 이라면 상점ui 자체를 refresh하자
		//{
		//    UI_Top.Getsingleton.popupStore.Refresh_store();
		//}

	}


	//구글결제 구매실패시 콜백함수
	void purchaseFailedEvent(string error, int response)
	{
		UserEditor.Getsingleton.EditLog("purchaseFailedEvent: " + error + ", response: " + response);

		

		//로딩바 끄자
		CompletedPaymentProcess();
	}


	//구글 결제 소모성공시 콜백함수
	void consumePurchaseSucceededEvent(GooglePurchase purchase)
	{
		UserEditor.Getsingleton.EditLog("consumePurchaseSucceededEvent: " + purchase);

		//lst_NoneConsumedProduct 리스트 삭제
		lst_NoneConsumedProduct.Remove(purchase);

		if (isForceConsume)
		{
			ForceConsumePurchase();
		}

		//로딩바 끄자
		CompletedPaymentProcess();
	}


	//구매상품 강제소모하기
	public void ForceConsumePurchase()
	{
		Loadmanager.instance.LoadingUI(true);

		if (lst_NoneConsumedProduct.Count > 0)
		{
			UserEditor.Getsingleton.EditLog("강제 소모 처리 Product ID : " + lst_NoneConsumedProduct[0].productId);
			GoogleIAB.consumeProduct(lst_NoneConsumedProduct[0].productId);
		}
		else
		{
			Loadmanager.instance.LoadingUI(false);
			UserEditor.Getsingleton.EditLog("강제소모 종료 => lst_NoneConsumedProduct.Count : " + lst_NoneConsumedProduct.Count);
			isForceConsume = false;
		}

	}


	//구글 결제 소모실패시 콜백함수
	void consumePurchaseFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog("consumePurchaseFailedEvent : " + error);
		if (isForceConsume)
		{
			isForceConsume = false; //무한 반복 컨슘처리 되니 그만두자
			ForceConsumePurchase();
			lst_NoneConsumedProduct.Remove(lst_NoneConsumedProduct[0]);

		}
		else
		{


			//다시 소모 전송
			//GoogleIAB.consumeProduct(buyProductID);
		}

		//로딩바 끄자
		CompletedPaymentProcess();
	}


	void billingSupportedEvent()
	{
		UserEditor.Getsingleton.EditLog("billingSupportedEvent");

		//sku : 재고 보관 단위상품을 의미 , 등록한 상품을 선언한다
		GoogleIAB.queryInventory(productIDs);
	}


	void billingNotSupportedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog("billingNotSupportedEvent: " + error);
	}


	void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		UserEditor.Getsingleton.EditLog(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));
		Prime31.Utils.logObject(purchases);
		Prime31.Utils.logObject(skus);

		//소모하지않은 상품들 (소모처리 위해서 저장하자)
		lst_NoneConsumedProduct = purchases;

		// 소모하지 않은 내역있다마 소모 처리하기
		CheckConsumePayment();

		//실제 상품리스트들
		lst_Product = skus;


		//상품ID 에따른 상품리스트 만들기 
		for (int i = 0; i < lst_Product.Count; i++)
		{
			Dic_ProductInfo[lst_Product[i].productId] = lst_Product[i];
		}

			IsGetQueryInventory = true;

	
	}

	// 소모하지 않은 내역있다마 소모 처리하기
	void CheckConsumePayment()
	{
		UserEditor.Getsingleton.EditLog("NONE Consumed Product count :  " + lst_NoneConsumedProduct.Count);
		

		if (lst_NoneConsumedProduct.Count > 0)
		{
			//로딩바 돌리자
			Loadmanager.instance.LoadingUI(true);

			for (int i = 0; i < lst_NoneConsumedProduct.Count; i++)
			{

				UserEditor.Getsingleton.EditLog("NONE Consumed Product :  " + lst_NoneConsumedProduct[i].productId + " / " + "product State : " +lst_NoneConsumedProduct[i].purchaseState);

				GooglePurchase nonConsumePt = lst_NoneConsumedProduct[i];

				


				if(nonConsumePt.purchaseState ==  GooglePurchase.GooglePurchaseState.Purchased)
				{
					UserEditor.Getsingleton.EditLog("구매 완료했던 내역 소모처리 :  " + nonConsumePt);

					//웹서버에 구매영수증 보냄
					webRequest.ShopBuyItem(Get_GemShopIdx(lst_NoneConsumedProduct[i].productId), lst_NoneConsumedProduct[i].originalJson,
						lst_NoneConsumedProduct[i].signature, () => callback_complete_NoneConsumeProduct(nonConsumePt));
				}
				//임시
				//GoogleIAB.consumeProduct(lst_NoneConsumedProduct[i].productId);
			}
		}
	}

	// 소모되않은 상품 소모처리완료
	void callback_complete_NoneConsumeProduct(GooglePurchase noneConsumePt)
	{
		//결제 성공 시 바로 소모 전송
		UserEditor.Getsingleton.EditLog("소모되않은 상품 소모처리 완료 ~~ : " + buyProductID);
		GoogleIAB.consumeProduct(noneConsumePt.productId);


		//구매완료했으니 통계에 넣자 
		GoogleSkuInfo buyProductInfo = Dic_ProductInfo[noneConsumePt.productId];
		string reStr = buyProductInfo.price.Remove(0, 1); //\  문자 지우기
		double dprice = Convert.ToDouble(reStr);
		//AnalysisManager.instance.Anl_Purchace(buyProductInfo.title, buyProductInfo.priceCurrencyCode, dprice);
		
	}


	void queryInventoryFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog("queryInventoryFailedEvent: " + error);
		IsGetQueryInventory = false;
	}


	void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
	{
	}


	
//======================================       IOS In app Event             =================================================


#elif UNITY_IOS

	public StoreKitProduct Get_StoreKitPtInfoIOS(ushort shopidx)
	{
		StoreKitProduct storekit = null;
		if (Dic_StorkitProductInfo.ContainsKey (Get_ProductID (shopidx)))
			storekit = Dic_StorkitProductInfo [Get_ProductID (shopidx)];

		return storekit;
	}
	


	void refreshReceiptSucceededEvent()
	{
	}


	void refreshReceiptFailedEvent(string error)
	{
	}


	void transactionUpdatedEvent(StoreKitTransaction transaction)
	{
	}


	//상품리스트 성공적으로 들어올떄 콜백함수
	void productListReceivedEvent(List<StoreKitProduct> productList)
	{
		UserEditor.Getsingleton.EditLog("productListReceivedEvent. total products received: " + productList.Count);

		// print the products to the console
		//foreach (StoreKitProduct product in productList)
		//	UserEditor.Getsingleton.EditLog(product.ToString() + "\n");

		//Loadmanager.instance.LoadingUI(false);



		//상품정보(가격, 화폐단위) 받아오기
		for(int i = 0; i < productList.Count; i++)
		{
			Dic_StorkitProductInfo [productList [i].productIdentifier] = productList[i];

			//Debug.Log (productList [i].productIdentifier + " / " + productList [i].price + " / " + productList [i].currencyCode + " / " + productList [i].formattedPrice + 
			//	" / " + productList [i].currencySymbol);
		}




		// 구입 했던 거래 정보
		List<StoreKitTransaction> _storeKitTrns = StoreKitBinding.getAllSavedTransactions();

		UserEditor.Getsingleton.EditLog ("getAllSavedTransactions count : " + _storeKitTrns.Count);

//		for(int i = 0 ; i < _storeKitTrns.Count ; i++)
//		{
//			UserEditor.Getsingleton.EditLog ("getAllSavedTransactions _ Transaction state : " + _storeKitTrns[i].ToString());
//
//		}

		// 구매 후 소모 처리 되지 않은 상 품들 
		List<StoreKitTransaction> _storekitAllCurrent = StoreKitBinding.getAllCurrentTransactions ();

		UserEditor.Getsingleton.EditLog ("getAllCurrentTransactions count : " + _storekitAllCurrent.Count);

		for (int i = 0; i < _storekitAllCurrent.Count; i++) 
		{
			isForceConsume = true;
			UserEditor.Getsingleton.EditLog ("getAllCurrentTransactions _ Transaction state : " + _storekitAllCurrent[i].ToString());

			if (_storekitAllCurrent [i].transactionState == StoreKitTransactionState.Purchased) //거래 대기열에 있음, 사용자에게 청구 됨. 클라이언트는 트랜잭션을 완료해야함. 
			{

				// 다시 소모 처리 시된 상품 저장 => 만약 소모처리중 프로토콜 에러 발 생시 강제 소모 하려고
				storekitTracs.Add (_storekitAllCurrent [i]);

				//거래 상품중에 소모를 하지않은 상품들 소모해준다
				productPurchaseAwaitingConfirmationEvent (_storekitAllCurrent [i]);
				//StoreKitBinding.finishPendingTransaction (_storekitAllCurrent [i].transactionIdentifier);	

			}
			else if (_storekitAllCurrent [i].transactionState == StoreKitTransactionState.Failed) // 거래 실패된 상품 거래 는 그냥 소모 처리 한다
			{ 
				UserEditor.Getsingleton.EditLog ("강제소모 함  : " + _storekitAllCurrent [i].productIdentifier + " / " + _storekitAllCurrent [i].transactionState);
				StoreKitBinding.finishPendingTransaction (_storekitAllCurrent [i].transactionIdentifier);	
			} 


		}	
		if (_storekitAllCurrent.Count > 0)
			isForceConsume = false;


		IsGetQueryInventory = true;

	}

	//상품리스트 못받아올떄 콜백함수
	void productListRequestFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog("productListRequestFailedEvent: " + error);

		IsGetQueryInventory = false;

		// 상품 정보 못가져오면 다시 가져오기 시도 
		//Loadmanager.instance.LoadingUI(true);
		StoreKitManager.productListReceivedEvent += allProducts =>
		{
			UserEditor.Getsingleton.EditLog( "received total products: " + allProducts.Count );
			Products = allProducts;
		};
		UserEditor.Getsingleton.EditLog (" Again requestProductData ~~~~~~~~~~");
		StoreKitBinding.requestProductData(productIDs);

	}

	// 구매 성공 콜백함수
	void purchaseSuccessfulEvent(StoreKitTransaction transaction)
	{
		UserEditor.Getsingleton.EditLog("purchaseSuccessfulEvent : " +transaction.transactionState);
		Utils.logObject(transaction);

		// 구매 진행한 상품 정보 널 시키자 (다음 상품을 위해) 
		transactionProduct = null;


		if (transaction.transactionState == StoreKitTransactionState.Purchased) // 구매완료만 통계보내자
		{
			//구매 했으니 통계에 뿌리 자 ㅎ
			for (int i = 0; i < Products.Count; i++) {
				if (Products [i].productIdentifier == transaction.productIdentifier) {
					//AnalysticManager.Getinstance.Analystic_Purchase (UserDataManager.instance.user.user_Users.NkNm, ""
					//	, Products [i].productIdentifier, Convert.ToDouble(Products [i].price), 1, Products [i].currencyCode, "catIOS");
					AnalysisManager.instance.Anl_Purchace (Products [i].title, Products [i].currencyCode, Convert.ToDouble (Products [i].price));
				}
			}
		}
		if (nextPrcs != null)
			nextPrcs();

		////top 갱신
		//UI_Top.Getsingleton.set_refresh();

		//if (Get_GemShopIdx(buyProductID) == 13) // 월정액 상품 이라면 상점ui 자체를 refresh하자
		//{
		//    UI_Top.Getsingleton.popupStore.Refresh_store();
		//}

		

		// loading UI false
		Loadmanager.instance.LoadingUI(false);

	}

	
	//구매 실패 콜백함수
	void purchaseFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog("purchaseFailedEvent: " + error);

		isPurchasing = false;

		// loading UI false
		Loadmanager.instance.LoadingUI(false);

		UI_Popup_Notice popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice> (UIPOPUP.POPUPNOTICE);
		popup.Set_PopupTitleMessage ("구매실패");
		popup.SetPopupMessage (error);

	}

	// 구매취소 콜백함수
	void purchaseCancelledEvent(string error)
	{
		isPurchasing = false;

		// loading UI false
		Loadmanager.instance.LoadingUI(false);

		UserEditor.Getsingleton.EditLog("purchaseCancelledEvent: " + error);
	}
	
	//구매 에러 콜백함수
	void purchaseErrorEvent(P31Error error)
	{
		isPurchasing = false;

		// loading UI false
		Loadmanager.instance.LoadingUI(false);

		UserEditor.Getsingleton.EditLog("purchaseErrorEvent: " + error);
	}

	//구매후 컨슘처리대기 하는 콜백함수
	void productPurchaseAwaitingConfirmationEvent(StoreKitTransaction transaction)
	{
		UserEditor.Getsingleton.EditLog("productPurchaseAwaitingConfirmationEvent: " + transaction);

		Utils.logObject(transaction);

		// loading UI true
		Loadmanager.instance.LoadingUI(true);


		//서버에 구매 하기 프로토콜을 보낸다
		transactionProduct  = transaction;


		if (isPurchasing || isForceConsume) {
			isPurchasing = false;

			string Based64ReceiptData = GetBased64ReceiptData ();

			UserEditor.Getsingleton.EditLog ("transaction.productIdentifier : " + transaction.productIdentifier + "\n" +
			"transaction.transactionIdentifier : " + transaction.transactionIdentifier);

			if (!Based64ReceiptData.Equals (string.Empty)) {
				if (Dic_productIDs.ContainsValue (transaction.productIdentifier))
					webRequest.ShopBuyItem (Get_GemShopIdx (transaction.productIdentifier), Based64ReceiptData, transaction.transactionIdentifier, callback_Compelete_puchaseIOS);
				else
					callback_Compelete_puchaseIOS ();
			} else {
				// 영 수증 정보 가 없 을때 어떻게 할지 고민해바야한다
			}

		} 


	}

	void callback_Compelete_puchaseIOS()
	{

		// loading UI true
		Loadmanager.instance.LoadingUI(true);

		//소모 처 리를 한다
		StoreKitBinding.finishPendingTransaction(transactionProduct.transactionIdentifier);

	}

	string GetBased64ReceiptData()
	{
		string base64ReceiptData = string.Empty;
		string receiptLocation = StoreKitBinding.getAppStoreReceiptLocation();
		UserEditor.Getsingleton.EditLog ("getAppStoreReceiptLocation : " + StoreKitBinding.getAppStoreReceiptLocation ());


		receiptLocation = receiptLocation.Replace("file://", string.Empty);
		//UserEditor.Getsingleton.EditLog ("getAppStoreReceiptLocation File : " + receiptLocation);
		if (!File.Exists(receiptLocation))
		{
			return base64ReceiptData;
		}
		byte[] receiptBytes = File.ReadAllBytes(receiptLocation);

		UserEditor.Getsingleton.EditLog ("receiptBytes count: " + receiptBytes.Length);

		base64ReceiptData = System.Convert.ToBase64String(receiptBytes);
		//object receip = MiniJSON.Json.Deserialize (base64ReceiptData);
		//Utils.logObject (receip);
		UserEditor.Getsingleton.EditLog ("base64ReceiptData : " + base64ReceiptData);

		return base64ReceiptData;
	}
	


	void restoreTransactionsFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog ("restoreTransactionsFailedEvent");
	}


	void restoreTransactionsFinishedEvent()
	{
		UserEditor.Getsingleton.EditLog ("restoreTransactionsFinishedEvent");
	}


	void paymentQueueUpdatedDownloadsEvent(List<StoreKitDownload> downloads)
	{
		UserEditor.Getsingleton.EditLog("paymentQueueUpdatedDownloadsEvent: ");
		foreach (var dl in downloads)
			UserEditor.Getsingleton.EditLog(dl);
	}


	void fetchStorePromotionVisibilitySucceededEvent(string productIdentifier, SKProductStorePromotionVisibility visibility)
	{
		UserEditor.Getsingleton.EditLog ("fetchStorePromotionVisibilitySucceededEvent");
	}


	void fetchStorePromotionVisibilityFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog ("fetchStorePromotionVisibilityFailedEvent");
	}


	void updateStorePromotionVisibilitySucceededEvent(string productIdentifier)
	{
		UserEditor.Getsingleton.EditLog ("updateStorePromotionVisibilitySucceededEvent");
	}


	void updateStorePromotionVisibilityFailedEvent(string error)
	{
		UserEditor.Getsingleton.EditLog ("updateStorePromotionVisibilityFailedEvent");
	}


#endif

}
