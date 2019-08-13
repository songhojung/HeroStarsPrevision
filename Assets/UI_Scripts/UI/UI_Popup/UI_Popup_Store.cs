using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public enum STOREMODE_TYPE
{
	NONE = 0,
	Gold = 1,
	Gem = 2,
	Package = 3,
	Special = 4,
}



public enum Package_Type
{
	Month = 1,
	NewUser = 2,
	Gold_1 = 3,
	Gold_2 = 4,
	Substance_1 = 5,
	Substance_2 = 6,
	Substance_3 = 7,
	Special = 8,
}

public class UI_Popup_Store : MonoBehaviour 
{

	private Dictionary<ushort, Infos_Shop> Dic_InfosShop = new Dictionary<ushort, Infos_Shop>();
	private Dictionary<ushort, Infos_EventShopTime> Dic_InfosEventShopTm = new Dictionary<ushort, Infos_EventShopTime>();
	private List<UIItem_StoreElement> Lst_storeElement = new List<UIItem_StoreElement>();
	private List<UIItem_StoreElement> Lst_storePackageElemnet = new List<UIItem_StoreElement>(); // 패키지 상품 element 리스트

	public Transform Tr_Content;
	private STOREMODE_TYPE nowStoreMode = STOREMODE_TYPE.NONE	;

	public List<Toggle> Lst_Toggle;

	private bool isHadInit = false;

	void Start()
	{
		//Init_store();
		//
		//Set_Element(nowStoreMode);
	}

	 void Init_store()
	{
		Dic_InfosEventShopTm = TableDataManager.instance.Infos_EventShopTimes;
		//Dic_InfosShop = TableDataManager.instance.Infos_shops;
		Lst_Toggle[(int)STOREMODE_TYPE.Package -1].isOn = true;
		nowStoreMode = STOREMODE_TYPE.Package;

		isHadInit = true;
	}

	 public void OnEnable()
	 {

		 //스토어 팝업이 재활성화 될떄 store 갱신
		 if (isHadInit)
		 {
			 Refresh_store();
		 }
		 else
		 {
			 Init_store();

			 //토글 true 해서 ResponseButton_tap으로 진행하기 떄문에 set Element 진행안해도됨
			 //Set_Element(nowStoreMode);
		 }
	 }



	public void Refresh_store()
	 {
		

		 if (nowStoreMode == STOREMODE_TYPE.Gem || nowStoreMode == STOREMODE_TYPE.Gold)
		 {
			 if (Lst_storeElement.Count > 0)
			 {
				 for (int i = 0; i < Lst_storeElement.Count; i++)
						 Lst_storeElement[i].ApplyInfo(nowStoreMode);

			 }
		 }
		 else if (nowStoreMode == STOREMODE_TYPE.Package || nowStoreMode == STOREMODE_TYPE.Special)
		 {
			 if (Lst_storePackageElemnet.Count > 0)
			 {
				 for (int i = 0; i < Lst_storePackageElemnet.Count; i++)
					 Lst_storePackageElemnet[i].ApplyInfo(nowStoreMode);
			 }
		 }
		 //패키치상품 예외처리 갱신
		 //Chk_PackageStore(STOREMODE_TYPE.Package);
	 }


	public void Set_Element(STOREMODE_TYPE storeMode)
	{
		//if (Dic_InfosShop.Count <= 0)
		//{
		//	 Dic_InfosShop = TableDataManager.instance.Infos_shops;
		//}

		//상점 이벤트 데이터 따른 실제 상점 데이터 갱신
		Dictionary<ushort, Infos_Shop> _dicInfoShop = TableDataManager.instance.Infos_shops;

		foreach (var sp in _dicInfoShop)
		{
			if (Dic_InfosEventShopTm.ContainsKey(sp.Value.ShopIdx))
			{
				if (TimeManager.Instance.Get_nowTime() < Dic_InfosEventShopTm[sp.Value.ShopIdx].BuyEndTm)
				{
					Dic_InfosShop[sp.Value.ShopIdx] = sp.Value;
				}
			}
			else
				Dic_InfosShop[sp.Value.ShopIdx] = sp.Value;

		}


		Creat_Element(storeMode);
	}



	 /// <summary>
	 /// 각 탭별로 element생성
	 /// </summary>
	 public void Creat_Element(STOREMODE_TYPE storeMode)
	 {

		 foreach (var itemshop in Dic_InfosShop)
		 {
			 if (ChkShopMode(itemshop.Value.ShopIdx, storeMode))
			 {
				 //생성할 element 생성및 정보설정
				 CreateEleInfo(itemshop.Value, storeMode);
			 }
		 }




		 //sortIdx 값에 따른 아이템들정렬하기 
		 if (storeMode == STOREMODE_TYPE.Gem || storeMode == STOREMODE_TYPE.Gold)
		 {
			 Set_SortOrderByValue(Lst_storeElement);

		 }
		 else if (storeMode == STOREMODE_TYPE.Package || storeMode == STOREMODE_TYPE.Special)
		 {
			 Set_SortOrderByValue(Lst_storePackageElemnet);
		 }

	 }

	 //생성할 element 생성및 정보설정
	 void CreateEleInfo(Infos_Shop shopinfo, STOREMODE_TYPE _storemode)
	 {
		 UIItem_StoreElement storeEle = null;
		 //패키지 ,스폐셜 상품 예외처리
		 if (_storemode == STOREMODE_TYPE.Package || _storemode == STOREMODE_TYPE.Special)
		 {
			  storeEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_STOREPACKAGEELEMENT, Tr_Content) as UIItem_StoreElement;

			 BonusInfo bouns = new BonusInfo(0, 0, (int)shopinfo.BnsItNum, (ITEMTYPE)shopinfo.BnsItTp, (int)shopinfo.BnsIdx);
			 storeEle.Set_info(shopinfo, bouns, _storemode);
			 Lst_storePackageElemnet.Add(storeEle);

		 }
		 else
		 {
			  storeEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_STOREELEMENT, Tr_Content) as UIItem_StoreElement;

			 BonusInfo bouns = new BonusInfo(0, 0, (int)shopinfo.BnsItNum, (ITEMTYPE)shopinfo.BnsItTp, (int)shopinfo.BnsIdx);
			 storeEle.Set_info(shopinfo, bouns, _storemode);
			 Lst_storeElement.Add(storeEle);
		 }
		
		
	 }


	 //상점인덱스가 현재 상점모드랑 맞는지 체크
	 bool ChkShopMode(ushort _shopIdx, STOREMODE_TYPE _storemode)
	 {
		 bool isRight = false;
		 int quota = 0;
		 if (_storemode == STOREMODE_TYPE.Gem) quota = 2;
		 else if (_storemode == STOREMODE_TYPE.Gold) quota = 1;
		 else if (_storemode == STOREMODE_TYPE.Package) quota = 3;
		 else if (_storemode == STOREMODE_TYPE.Special) quota = 4;

		 isRight = (_shopIdx / 100 == quota) ? true : false;

		 return isRight;
	 }


	public void Chk_PackageStore(STOREMODE_TYPE storeMode)
	 {
		 //패키지 상품 예외처리
		 if (storeMode == STOREMODE_TYPE.Package)
		 {
			 int activePackageNum = 0;

			 for (int i = 0; i < Lst_storePackageElemnet.Count; i++)
			 {
				 //우선 정보 갱신
				 Lst_storePackageElemnet[i].ApplyInfo(storeMode);

				 if (Lst_storePackageElemnet[i].infos_shop.PurLimit != 0) //단일 소모 패키지 상품만
				 {

					 if (UserDataManager.instance.user.chk_PurchaseLimit(Lst_storePackageElemnet[i].infos_shop.PurLimit))
					 {
						 Lst_storePackageElemnet[i].gameObject.SetActive(false);//비활성화

					 }
					 else
					 {
						 //패키지상품 2개만 활성 
						 if (activePackageNum >= 2)
						 {
							 Lst_storePackageElemnet[i].gameObject.SetActive(false);//비활성화
							 continue;
						 }

						 Lst_storePackageElemnet[i].gameObject.SetActive(true);//활성화
						 activePackageNum++;
					 }
				 }
			 }

		 }

	 }


	 void Clear_Element()
	 {
		 for (int i = 0; i < Lst_storeElement.Count; i++)
		 {
			 Destroy(Lst_storeElement[i].gameObject);
		 }
		 Lst_storeElement.Clear();


		 for (int i = 0; i < Lst_storePackageElemnet.Count; i++)
		 {
			 Destroy(Lst_storePackageElemnet[i].gameObject);
		 }
		 Lst_storePackageElemnet.Clear();
	 }


	 //sortIdx 값에 따른 아이템들정렬하기 
	 void Set_SortOrderByValue(List<UIItem_StoreElement> _UIItem_StoreElement)
	 {
		 var sortLst = _UIItem_StoreElement.OrderBy(n => n.infos_shop.SortIdx);

		 List<UIItem_StoreElement> sortLstElement = sortLst.ToList();

		 for (int i = 0; i < sortLstElement.Count; i++)
		 {
			 //_UIItem_StoreElement[i].transform.SetSiblingIndex((int)_UIItem_StoreElement[i].infos_shop.SortIdx);
			 sortLstElement[i].transform.SetAsLastSibling();
		 }
	 }




	 public void ResponseButton_tap(int tapIdx)
	 {
		 STOREMODE_TYPE type = (STOREMODE_TYPE)tapIdx;

		 if (Lst_Toggle[tapIdx-1].isOn == true)
		 //if (nowStoreMode != (STOREMODE_TYPE)tapIdx)
		 {

			 nowStoreMode = (STOREMODE_TYPE)tapIdx;

			 Clear_Element();

			 Set_Element(nowStoreMode);
		 }


	 }


	 public void ResponseButton_Close()
	 {
		 gameObject.SetActive(false);

		 User.isSelectedCharacter = false;
	 }


	 public void Start_OnTap(STOREMODE_TYPE storeMode)
	 {


		 if (storeMode == STOREMODE_TYPE.Gold)
		 {
			 //Lst_Toggle[(int)ITEMTYPE.GOLD - 1].isOn = true;
			 for (int i = 0; i < Lst_Toggle.Count; i++ )
			 {
				 if (i == ((int)STOREMODE_TYPE.Gold - 1))
					Lst_Toggle[i].isOn = true;
				 else
					 Lst_Toggle[i].isOn = false;

			 }

			 if (isHadInit == false)
				 isHadInit = true;
			
		 }
		 else if (storeMode == STOREMODE_TYPE.Gem)
		 {
			// Lst_Toggle[(int)ITEMTYPE.GEM - 1].isOn = true;
			 for (int i = 0; i < Lst_Toggle.Count; i++)
			 {
				 if (i == ((int)STOREMODE_TYPE.Gem - 1))
					 Lst_Toggle[i].isOn = true;
				 else
					 Lst_Toggle[i].isOn = false;

			 }

			 if (isHadInit == false)
				 isHadInit = true;
		 }
		 else if (storeMode == STOREMODE_TYPE.Package)
		 {
			 // Lst_Toggle[(int)ITEMTYPE.GEM - 1].isOn = true;
			 for (int i = 0; i < Lst_Toggle.Count; i++)
			 {
				 if (i == ((int)STOREMODE_TYPE.Package - 1))
					 Lst_Toggle[i].isOn = true;
				 else
					 Lst_Toggle[i].isOn = false;

			 }

			 if (isHadInit == false)
				 isHadInit = true;
		 }
	 }
}
