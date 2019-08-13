using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class UI_Equipment : UI_Base 
{
	public List<GameObject> Lst_EquipInvenOBJ;
	public List<RectTransform> Lst_EquipIvenContentTr;

	public EquipType nowEquipInven = EquipType.Dress_HEAD;
    public uint nowSelectUnitIdx;

    private Dictionary<uint, infos_unit> Dic_unitInfos;
	private Dictionary<uint, Infos_Weapon> Dic_weaponInfos = new	Dictionary<uint, Infos_Weapon>();
	private Dictionary<uint, Infos_Deco> Dic_decoInfos = new Dictionary<uint, Infos_Deco>();

	private List<UIItem_EquipItem> Lst_mainWeaponItems = new List<UIItem_EquipItem>();
	private List<UIItem_EquipItem> Lst_subWeaponItems = new List<UIItem_EquipItem>();
	private List<UIItem_EquipItem> Lst_Dress1Items = new List<UIItem_EquipItem>();
	private List<UIItem_EquipItem> Lst_Dress2Items = new List<UIItem_EquipItem>();
	private List<UIItem_EquipItem> Lst_Dress3Items = new List<UIItem_EquipItem>();
	public UIItem_EquipItem Selected_Item;					//선택한아이템

	public Button button_Equip;
    public GameObject DisableEquipBtn;

    public List<GameObject> Lst_EquipButtonOBJ;

	//캐릭터

	private Dictionary<uint, characterInfo> Dic_EquiqCharacter = new Dictionary<uint, characterInfo>();
	private Vector3 OrinCharacterRotateOBJPos;					//CharacterRotate 오브젝트 원래 위치
	public Transform Tr_CharacterRotate;
	public characterInfo equipCharacter;
	private Vector3 TouchBegan_Pos = Vector3.zero;
	private Vector3 TouchNow_Pos = Vector3.zero;
    //Drag 영역
    public RoatateSector CharRotateSector;

    //UI
    public List<Text> Lst_textItemName;
	public List<Image> Lst_imageWpnType;
	public List<Text> Lst_textPower;				//공격력
	public List<Text> Lst_textCritical;				//크리티컬
	public List<Text> Lst_textAccuracy;				//초기정확도
	public List<Text> Lst_textRecoil;				//반동제어
	public List<Text> Lst_textRateOfFire;			//공속
	public List<Text> Lst_textReloadSpeed;			//장전속도
	public List<Text> Lst_textMagazine;				//탄창용량
	public List<Text> Lst_textZoom;					//줌배율
	public List<Image> Lst_graphCurPower;				//cur공격력 그래프
	public List<Image> Lst_graphCurCritical;				//cur크리 그래프
	public List<Image> Lst_graphCurAccuracy;				//cur정확도 그래프
	public List<Image> Lst_graphCurRecoil;				//cur반동제어 그래프
	public List<Image> Lst_graphCurRateOfFire;			//cur공속 그래프
	public List<Image> Lst_graphCurReloadSpeed;			//cur장전속도 그래프
	public List<Image> Lst_graphCurMagazine;				//cur탄창용량 그래프
	public List<Image> Lst_graphCurZoom;				//cur줌배율 그래프

	public List<Image> Lst_graphNxtPower;				//Next공격력 그래프
	public List<Image> Lst_graphNxtCritical;				//Next크리 그래프


	public List<Text> Lst_textUpgrade;
	public List<Text> Lst_textItemLv;
	public List<Text> Lst_textBuyPrice;
	public List<Image> Lst_ImageBuyGoodsType;			
	public List<Text> Lst_textDecoGDP;				// 치장 폭탄능력치텍스트
	public List<Text> Lst_textDecoRSPS;				// 치장 리스폰능력치텍스트
	public List<Text> Lst_textDecoCHRS;				// 치장 쿨타임능력치텍스트
	public List<GameObject> Lst_DecoHeadSpecOBJ;		// 치장1 스펙텍스트 오브젝트 
	public List<GameObject> Lst_DecoBodySpecOBJ;		// 치장2 스펙텍스트 오브젝트
	public List<GameObject> Lst_DecoFaceSpecOBJ;		// 치장3 스펙텍스트 오브젝트
	public List<GameObject> Lst_InfoBuyOBJ;			// 정보란에 있는 구매버튼 오브젝트
	public List<GameObject> Lst_InfoNormalOBJ;		// 정보란에 있는 레벨,soldout 오브젝트

	//세트효과
	public Text text_notiSet;                       //세트효과 이름표시
    public GameObject SetIntroPopup;
    public UI_Popup_SetItemInfo popupSetItemInfo;

    private static UI_Equipment _instance;
	public static UI_Equipment Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Equipment)) as UI_Equipment;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("UI_Equipment");
					_instance = instanceObj.AddComponent<UI_Equipment>();
				}
			}

			return _instance;
		}
	}

	public override void set_Open()
	{
		base.set_Open();

		Init_EquipmentUI();
	}

	public override void set_Close()
	{
		base.set_Close();


		//세트설명 팝업 닫자
		Activate_SetIntroPopup(false);
	}

	public override void set_refresh()
	{
		base.set_refresh();

		// 표시되엇던 UI 정보 초기화
		Clear_UIInfo();

		//모든아이템 정보 초기화
		ClearAll_EquipItemInfo();

		//장착 구성데이터 할당
		Sign_EquipDataInfo();

		// 사용하는 캐릭터생성 및 정보 설정
		Apply_UseCharacter();


		//장착 정보 설정하기 및 장착아이템생성및 설정
		Set_EquipmentInfo(nowEquipInven);

		//rotateOBJ 이동, 캐릭터 뒤쪽보기 기능
		Apply_characterRotateBehaivour(nowEquipInven);
	}

	void Update()
	{
		//StaticMethod.Chk_TouchSelectArea(User.isSelectedCharacter, ref touchState, ref TouchBegan_Pos, "CharSelectArea");

		//if (equipCharacter != null)
		//	StaticMethod.TouchRotateObject(ref touchState, ref TouchBegan_Pos, ref TouchNow_Pos, touch, equipCharacter.transform, value_Rotatesensitive, 
		//		null, Chk_TouchCharacter);


	}

	void Init_EquipmentUI()
	{
		Dic_unitInfos = TableDataManager.instance.Infos_units;
		//Dic_weaponInfos = TableDataManager.instance.Infos_weapons;
		//Dic_decoInfos = TableDataManager.instance.Infos_Decos;

		//장착 구성데이터 할당
		Sign_EquipDataInfo();


		//아이템선택
		Selected_Item = null;

		//캐릭터 회전 시 필요한 정보 설정
		Set_rotateInfo();

		// 사용하는 캐릭터생성 및 정보 설정
		Apply_UseCharacter();

		//장착 정보 설정하기 및 장착아이템생성및 설정
		Set_EquipmentInfo(nowEquipInven);
	}

     

	//장착 구성데이터 할당
	void Sign_EquipDataInfo()
	{
		User _user = UserDataManager.instance.user;
		Dictionary<uint, Infos_EventItemTime> _dicEventShop = TableDataManager.instance.Infos_EventItemTimes;
		Dictionary<uint, Infos_Weapon> _dicInfosweapn = TableDataManager.instance.Infos_weapons;
		Dictionary<uint, Infos_Deco> _dicInfosDeco = TableDataManager.instance.Infos_Decos;

		//할당될 장착컬렉션들 클리어 시켜주자 => refesh되기 떄문에
		Dic_weaponInfos.Clear();
		Dic_decoInfos.Clear();


        //캐릭터 세팅으로부터 넘어온 캐릭터 인덱스!!!!
        if(BaseData.m_Datas != null && BaseData.m_Datas.Count >0)
            nowSelectUnitIdx = (uint)BaseData.m_Datas[0];



        //무기쪽 이벤트 아이템 및 소지아이템 필터링
        foreach (var wpn in _dicInfosweapn)
		{
			if (_dicEventShop.ContainsKey(wpn.Value.WpnIdx))
			{
				if (TimeManager.Instance.Get_nowTime() < _dicEventShop[wpn.Value.WpnIdx].BuyEndTm)
				{
					Dic_weaponInfos[wpn.Value.WpnIdx] = wpn.Value;
				}
				else
				{
					if (_user.User_Weapons.ContainsKey(wpn.Value.WpnIdx))
						Dic_weaponInfos[wpn.Value.WpnIdx] = wpn.Value;
				}
			}
			else
				Dic_weaponInfos[wpn.Value.WpnIdx] = wpn.Value;
		}


		//치장쪽 이벤트 아이템 및 소지아이템 필터링
		foreach (var deco in _dicInfosDeco)
		{
			if (_dicEventShop.ContainsKey(deco.Value.DecoIdx))
			{
				if (TimeManager.Instance.Get_nowTime() < _dicEventShop[deco.Value.DecoIdx].BuyEndTm)
				{
					Dic_decoInfos[deco.Value.DecoIdx] = deco.Value;
				}
				else
				{
					if (_user.User_Decos.ContainsKey(nowSelectUnitIdx))
					{
						if (_user.User_Decos[nowSelectUnitIdx].ContainsKey(deco.Value.DecoIdx))
							Dic_decoInfos[deco.Value.DecoIdx] = deco.Value;
					}
				}
			}
			else
				Dic_decoInfos[deco.Value.DecoIdx] = deco.Value;
		}

      

    }




	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			선택한 캐릭터 정보			================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////

	// 사용하는 캐릭터생성 및 정보 설정
	void Apply_UseCharacter()
	{
		User _user = UserDataManager.instance.user;
        uint _unitIdx = nowSelectUnitIdx;


        if (!Dic_EquiqCharacter.ContainsKey(_unitIdx))
		{
			//선택한 캐릭터 생성
			Transform unitOrinObj = ObjectPoolManager.Getinstance.Get_ObjectLobbyUnit(_unitIdx.ToString());
			unitOrinObj.gameObject.SetActive(false);

			GameObject unitObj = Instantiate(unitOrinObj.gameObject);
			unitObj.SetActive(true);

			unitObj.transform.parent = Tr_CharacterRotate;
			unitObj.transform.localPosition = Vector3.zero;
			unitObj.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			unitObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // 크기 1.0로 잡기

			characterInfo lc = unitObj.GetComponent<characterInfo>();
			lc.unitIdx = (int)_unitIdx;
			//아이템관련 오브젝트 가져오기
			lc.Init_chrctBasicInfo();

			if (Dic_unitInfos.ContainsKey(_unitIdx))
				lc.unitInfos = Dic_unitInfos[_unitIdx];
			if (_user.User_Units.ContainsKey(_unitIdx))
				lc.userUnit = _user.User_Units[_unitIdx];
			lc.isHave = true;

			

			// 캐릭터 만든 정보 저장
			Dic_EquiqCharacter[_unitIdx] = lc;
		}
		else //캐릭터가 있다면 정보 갱신
		{

			foreach (var userUnit in _user.User_Units)
			{
				if (Dic_EquiqCharacter.ContainsKey(userUnit.Value.Unitidx))
				{
					Dic_EquiqCharacter[userUnit.Value.Unitidx].userUnit = userUnit.Value; 
				}
				
			}

		}

		//선택한 캐릭터 아니면 오브젝트 비활성
		foreach (var equipUnit in Dic_EquiqCharacter)
		{
			if (equipUnit.Value.userUnit.Unitidx == _unitIdx)
			{
				equipUnit.Value.gameObject.SetActive(true);
			
			}
			else
				equipUnit.Value.gameObject.SetActive(false);
		}


		//회전하기 위한 캐릭터 정보 할당
		equipCharacter = Dic_EquiqCharacter[nowSelectUnitIdx];

        CharRotateSector.Init_RotateSector(equipCharacter.transform);

    }


	

	void Chk_TouchCharacter()
	{
		float dis = TouchNow_Pos.x - TouchBegan_Pos.x;
		if (Mathf.Abs(dis) <= 0.5f)
		{
			equipCharacter.Do_toughBehavior();
		}
	}


	//rotateOBJ 이동, 캐릭터 뒤쪽보기 기능
	void Apply_characterRotateBehaivour(EquipType equipType)
	{
		Move_CharcterRotateOBJ(equipType);

		Turning_Character(equipType);
	}


	//해당 장비ui시 rotateobj 이동
	void Move_CharcterRotateOBJ(EquipType equipType)
	{
		Vector3 moveCenterPos = new Vector3(-325f,-224f,500f);
		if (equipType == EquipType.Main || equipType == EquipType.Sub)
		{
			if (Tr_CharacterRotate.localPosition == OrinCharacterRotateOBJPos)
				return;
			else
			{
				//가운데로 이동보간
				StartCoroutine(moveObj(Tr_CharacterRotate.localPosition,OrinCharacterRotateOBJPos));
				
			}
		}
		else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
		{
			if (Tr_CharacterRotate.localPosition == moveCenterPos)
				return;
			else
			{
				//월래자리로 이동 보간
				StartCoroutine(moveObj(Tr_CharacterRotate.localPosition, moveCenterPos));
			}
		}
	}


	IEnumerator moveObj(Vector3 nowPos, Vector3 targetPos)
	{
		float time = 0f;
		Vector3 trPos = Tr_CharacterRotate.localPosition;
		while (true)
		{
			if (trPos == targetPos)
				break;

			time += Time.deltaTime;


			trPos = Vector3.Lerp(nowPos, targetPos, time * 3);
			 Tr_CharacterRotate.localPosition = trPos;

			yield return null;
		}
	}

	//해당 장비ui시 캐릭터 돌기(가방 : 뒤로돌기 , 나머지 앞으로돌기)
	void Turning_Character(EquipType equipType)
	{
		

		if (equipType == EquipType.Main || equipType == EquipType.Sub ||
			equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_FACE)
		{
			Vector3 front = new Vector3(0,180f,0);
			//캐릭터 앞으로 돌기
			StartCoroutine(characterTurning(equipCharacter.transform.localEulerAngles, front));
		}
		else if ( equipType == EquipType.Dress_BODY)
		{
			Vector3 Back = new Vector3(0,-30f,0);
			//캐릭터 뒤로돌기
			StartCoroutine(characterTurning(equipCharacter.transform.localEulerAngles, Back));
		}
	}



	IEnumerator characterTurning(Vector3 nowRt, Vector3 targetRt)
	{
		float time = 0f;
		Vector3 bodyTr = equipCharacter.transform.localEulerAngles;
		while (true)
		{
            if (bodyTr == targetRt)
            {
                //최종회전값 전달
                CharRotateSector.Change_InitYDegree(equipCharacter.transform.eulerAngles.y);
                break;
            }
			time += Time.deltaTime;

			bodyTr = Vector3.Lerp(nowRt, targetRt, time * 3);

			equipCharacter.transform.localEulerAngles = bodyTr;
			yield return null;
		}

	}

	///////////////////////===============================================================================================//////////////////////////////////
	///////////////////////===========================			장착 정보			================================//////////////////////////////////
	///////////////////////===============================================================================================//////////////////////////////////

	//장착 정보 설정하기 
	void Set_EquipmentInfo(EquipType equipType)
	{

		//토글탭 오브젝트 활성
		Active_EquipObject(equipType);

		//아이템들 생성및 체크
		Set_EquipItem(equipType);
	}

	//캐릭터 회전 시 필요한 정보 설정
	void Set_rotateInfo()
	{

		OrinCharacterRotateOBJPos = new Vector3(Tr_CharacterRotate.localPosition.x, Tr_CharacterRotate.localPosition.y, Tr_CharacterRotate.localPosition.z);


	}




	//장착타입에 따른 아이템 생성및 체크
	void Set_EquipItem(EquipType equipType)
	{
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(equipType);

		if (_lst_Items != null)
		{
			if (_lst_Items.Count <= 0)
				//생성
				StartCoroutine(Creat_EquipItem(equipType));
			else
				//체크
				Chk_EquipItemInfo(equipType);
		}
	}


	//아아템생성
	IEnumerator Creat_EquipItem(EquipType equipType)
	{
		Loadmanager.instance.LoadingUI(true);
		yield return StartCoroutine(routine_CreatItem(equipType));
		Chk_EquipItemInfo(equipType);
		Loadmanager.instance.LoadingUI(false);
	}

	IEnumerator routine_CreatItem(EquipType equipType)
	{


		//무기 아이템 생성
		if (equipType == EquipType.Main || equipType == EquipType.Sub)
		{
			foreach (var weapon in Dic_weaponInfos.OrderBy(n=>n.Value.SortIdx))
			{
				//타입
				int partIdx = (int)weapon.Value.WpnPart;
				if (partIdx == (int)equipType +1)
				{
					//생성및 리스트 할당
					Sign_CreateItem(equipType, weapon.Value.WpnIdx, partIdx, weapon.Value.SortIdx);
				}
				yield return null;
			}

			
			//리스트 정렬하기 itemidx 순
			Sort_EquipItemList(equipType);
		}
		// 치장아이템 생성
		else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
		{

			foreach (KeyValuePair<uint, Infos_Deco> dc in Dic_decoInfos.OrderBy(n=> n.Value.SortIdx))
			{
				int partIdx = (int)dc.Value.DecoPart;
				if (partIdx == (int)(equipType - 1))
				{
					//생성및 리스트 할당
					Sign_CreateItem(equipType, dc.Value.DecoIdx, partIdx, dc.Value.SortIdx);
					
				}

				yield return null;
			}

			Sign_CreateItem(equipType, 1, 0, 0);
			
			//리스트 정렬하기 itemidx 순
			Sort_EquipItemList(equipType);
			yield return null;
		}



	}


	void Sign_CreateItem(EquipType equipType, uint Itemidx, int partIdx , int sortIdx)
	{
		//생성
		UIItem_EquipItem item = UI_Manager.Getsingleton.CreatUI(
			UIITEM.ITEM_EQUIPITEM, Lst_EquipIvenContentTr[(int)equipType]) as UIItem_EquipItem;
		item.Set_ItemInfo(Itemidx, partIdx,sortIdx);

		//리스트할당
		if (equipType == EquipType.Main) Lst_mainWeaponItems.Add(item);
		else if (equipType == EquipType.Sub) Lst_subWeaponItems.Add(item);
		else if (equipType == EquipType.Dress_HEAD) Lst_Dress1Items.Add(item);
		else if (equipType == EquipType.Dress_BODY) Lst_Dress2Items.Add(item);
		else if (equipType == EquipType.Dress_FACE) Lst_Dress3Items.Add(item);
	}



	//해당유닛에대한 장착하고잇는 장비 아이템체크 및 구매한 아이템들을 체크하자
	void Chk_EquipItemInfo(EquipType equipType)
	{
		Chk_EquipItem_Created(equipType);
		Chk_EquipItemInfo_HAVEItem(equipType);
		Chk_EquipItemInfo_EQUIPItem(equipType);
	}

	// 아이템이 생성이 되엇는지 체크 (없어야 할 아이템이면 비활성, 있어야할 아이템이면 생성)
	void Chk_EquipItem_Created(EquipType equipType)
	{
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(equipType);

		//생성 된것중에 비활성시켜야할 아이템체크
		for (int i = 0; i < _lst_Items.Count; i++ )
		{
			if (!_lst_Items[i].isUnEquipBtn) // 아이템오브젝트가 해제버튼이 아니라면
			{

				if (equipType == EquipType.Main || equipType == EquipType.Sub)
				{
					if (!Dic_weaponInfos.ContainsKey(_lst_Items[i].ItemIdx))
					{
						_lst_Items[i].gameObject.SetActive(false);
					}
					else // 생성되었지만 비활성인 아이템 활성
					{
						if (_lst_Items[i].gameObject.activeSelf == false)
							_lst_Items[i].gameObject.SetActive(true);
					}
				}
				else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
				{
					if (!Dic_decoInfos.ContainsKey(_lst_Items[i].ItemIdx))
					{
						_lst_Items[i].gameObject.SetActive(false);
					}
					else // 생성되었지만 비활성인 아이템 활성
					{
						if (_lst_Items[i].gameObject.activeSelf == false)
							_lst_Items[i].gameObject.SetActive(true);
					}
				}
			}
		}



		//생성 안된것중 생성시켜야할 아이템체크
		if (equipType == EquipType.Main || equipType == EquipType.Sub)
		{
			var notCreatedItemWpn = Dic_weaponInfos.Where(n=> (int)n.Value.WpnPart == (int)(equipType +1))
				.Where(n => !_lst_Items.ToDictionary(g => g.ItemIdx, g => g.ItemIdx).ContainsKey(n.Value.WpnIdx));

			if (notCreatedItemWpn.Count() > 0)
			{

				foreach (var ncItem in notCreatedItemWpn)
				{
					//생성및 리스트 할당
					Sign_CreateItem(equipType, ncItem.Value.WpnIdx, (int)ncItem.Value.WpnPart, ncItem.Value.SortIdx);
				}

				//리스트 정렬하기 itemidx 순
				Sort_EquipItemList(equipType);

			}
		}
		else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
		{
			var notCreatedItemDeco = Dic_decoInfos.Where(n=>(int)n.Value.DecoPart == (int)(equipType-1))
				.Where(n => !_lst_Items.ToDictionary(g => g.ItemIdx, g => g.ItemIdx).ContainsKey(n.Value.DecoIdx));

			if (notCreatedItemDeco.Count() > 0)
			{

				foreach (var ncItem in notCreatedItemDeco)
				{
					//생성및 리스트 할당
					Sign_CreateItem(equipType, ncItem.Value.DecoIdx, (int)ncItem.Value.DecoPart, ncItem.Value.SortIdx);
				}

				//리스트 정렬하기 itemidx 순
				Sort_EquipItemList(equipType);

			}
		}
		

		
	}

	//소유아이템 인지 아닌지 체크하기
	void Chk_EquipItemInfo_HAVEItem(EquipType equipType)
	{
		User _user = UserDataManager.instance.user;
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(equipType);
		List<UIItem_EquipItem> _lst_HaveItems = new List<UIItem_EquipItem>();
		uint useritemIdx = 0;

		if (equipType == EquipType.Main || equipType == EquipType.Sub)
		{
			//현재 구입한 무기아이템 모으기
			if (_user.User_Weapons.ContainsKey(nowSelectUnitIdx))
			{
				foreach (var item in _user.User_Weapons)
				{
					useritemIdx = item.Value.WpnIdx;

					var hvItem = _lst_Items.Where(n => n.ItemIdx == useritemIdx);

					foreach (var hv in hvItem) _lst_HaveItems.Add(hv);

				}
			}
		}
		else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
		{
			//현재 구입한 치장아이템 모으기
			if (_user.User_Decos.ContainsKey(nowSelectUnitIdx))
			{
				Dictionary<uint, user_Deco> _dic_unitDeco = _user.User_Decos[nowSelectUnitIdx];

				foreach (var dc in _dic_unitDeco)
				{
					useritemIdx = dc.Value.DecoIdx;

					var hvItem = _lst_Items.Where(n => n.ItemIdx == useritemIdx);

					foreach (var hv in hvItem) _lst_HaveItems.Add(hv);
				}
			}
		}

		//가지고 있는것들 list ishave를 true 할당
		for (int i = 0; i < _lst_HaveItems.Count; i++ )
		{
			_lst_HaveItems[i].IsHave = true;
		}

		
	}


	// 해당 유닛의 장착하고잇는 장비에대해 아이템체크
	void Chk_EquipItemInfo_EQUIPItem(EquipType equipType)
	{
		User _user = UserDataManager.instance.user;
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(equipType);
		uint useItemIdx = 0;

		//장착중인 아이템들 인덱스 받기
		if (_user.User_Units.ContainsKey(nowSelectUnitIdx))
		{
			useItemIdx = Get_EquipItemIndex(equipType);
		}

		//장착중인게 없다면 첫번쨰 아이템Index 부여
		if (useItemIdx == 0)
		{
			if (equipType == EquipType.Main) 
			{
				foreach (var item in Dic_weaponInfos)
				{
					if (item.Value.WpnPart == EQUIPPART_TYPE.PRIMARY)
					{
						useItemIdx = item.Value.WpnIdx;
						break;
					}
				}
				
			}
			else if (equipType == EquipType.Sub)
			{
				foreach (var item in Dic_weaponInfos)
				{
					if (item.Value.WpnPart == EQUIPPART_TYPE.SECONDARY)
					{
						useItemIdx = item.Value.WpnIdx;
						break;
					}
				}
			}
			else if (equipType == EquipType.Dress_HEAD)
			{
				//foreach (var item in Dic_decoInfos)
				//{
				//    if (item.Value.DecoPart == DECOPART_TYPE.HEAD)
				//    {
				//        useItemIdx = item.Value.DecoIdx;
				//        break;
				//    }
				//}
				useItemIdx = 1;
			}
			else if (equipType == EquipType.Dress_BODY)
			{
				//foreach (var item in Dic_decoInfos)
				//{
				//    if (item.Value.DecoPart == DECOPART_TYPE.BODY)
				//    {
				//        useItemIdx = item.Value.DecoIdx;
				//        break;
				//    }
				//}
				useItemIdx = 1;
			}
			else if (equipType == EquipType.Dress_FACE)
			{
				//foreach (var item in Dic_decoInfos)
				//{
				//    if (item.Value.DecoPart == DECOPART_TYPE.FACE)
				//    {
				//        useItemIdx = item.Value.DecoIdx;
				//        break;
				//    }
				//}
				useItemIdx = 1;
			}

		}

		//아이템장착
		var selectLstItem = _lst_Items.Where(n => n.ItemIdx == useItemIdx);
		if (selectLstItem.Count() > 0)
		{
			//아이템 ishave 할당하고 선택으로 설정
			foreach (var useitem in selectLstItem)
			{
				//useitem.IsHave = true;
				Selected_Item = useitem;
				Apply_EquipItem();
				Set_SelectedItem(Selected_Item);
				
			}
		}
	}


	
	

	
	//선택 아이템 이름,스펙, 레벨 , 구매 등 정보 설정
	public void Set_SelectedItem_Infomation(EquipType equipType)
	{
		if (Selected_Item.isUnEquipBtn)
		{
			Clear_UIInfo();

			Lst_InfoNormalOBJ[(int)equipType].SetActive(false);
			Lst_InfoBuyOBJ[(int)equipType].SetActive(false);

			return;
		}

		if (Selected_Item != null)
		{
			User _user = UserDataManager.instance.user;
			string ItemName = string.Empty;
			string price = string.Empty;
			string buyItemType = string.Empty;


			if (equipType == EquipType.Main || equipType == EquipType.Sub)
			{
				ItemName = Dic_weaponInfos[Selected_Item.ItemIdx].WpnName;

				//최대치
				float AttValueMax = 100f;
				float CriticalValueMax = 20f;
				float AccuracyValueMax = 300f;
				float RecoilValueMax = 100f;
				float RateofFireValueMax = 600f;
				float ReloadSpeedValueMax = 10f;
				float MagazineValueMax = 100f;
				float ZoomValueMax = 10f;




				float initAttMin = (float)Dic_weaponInfos[Selected_Item.ItemIdx].AtkMin;
				float initAttMax = (float)Dic_weaponInfos[Selected_Item.ItemIdx].AtkMax;
				float initCri = Dic_weaponInfos[Selected_Item.ItemIdx].Critical;
				int acurracy = Dic_weaponInfos[Selected_Item.ItemIdx].AimInit;
				int recoil = Dic_weaponInfos[Selected_Item.ItemIdx].AimCtrl;
				int rateOfFire = Dic_weaponInfos[Selected_Item.ItemIdx].AtkSpeed;
				int reloadSpeed = Dic_weaponInfos[Selected_Item.ItemIdx].GunReload; 
				

				if (Selected_Item.IsHave)
				{
					
 
					int reflv = _user.User_Weapons[Selected_Item.ItemIdx].RefLv;
					//공격력
					float att = StaticMethod.Get_nextSpec(initAttMax, reflv, true);
					Lst_textPower[(int)equipType].text = string.Format("{0}",att);
					Lst_graphCurPower[(int)equipType].fillAmount = initAttMax / AttValueMax;
					if(reflv <= 15)
						Lst_graphNxtPower[(int)equipType].fillAmount = att / AttValueMax;

					//크리티컬
					float cri = StaticMethod.Get_nextSpec(initCri, reflv, false);
					Lst_textCritical[(int)equipType].text = string.Format("{0}%", StaticMethod.Get_nextSpec(initCri, reflv, false));
					Lst_graphCurCritical[(int)equipType].fillAmount = initCri / CriticalValueMax;
					if (reflv <= 15)
						Lst_graphNxtCritical[(int)equipType].fillAmount = cri / CriticalValueMax;

					

					//lv
					if (_user.User_Weapons.ContainsKey(Selected_Item.ItemIdx))
					{
						int lv = _user.User_Weapons[Selected_Item.ItemIdx].RefLv;
						Lst_textItemLv[(int)equipType].text = string.Format("Lv.{0}",lv);

						if (lv >= 15)
							Lst_textUpgrade[(int)equipType].text = TextDataManager.Dic_TranslateText[410]; //MAX
						else
							Lst_textUpgrade[(int)equipType].text = TextDataManager.Dic_TranslateText[413]; // 강화

					}
				}
				else
				{
					//공격력
					float att = Dic_weaponInfos[Selected_Item.ItemIdx].AtkMax;
					Lst_textPower[(int)equipType].text = string.Format("{0}", att);
					Lst_graphCurPower[(int)equipType].fillAmount = att / AttValueMax;
					Lst_graphNxtPower[(int)equipType].fillAmount = 0;

					//크리티컬
					float cri =  Dic_weaponInfos[Selected_Item.ItemIdx].Critical;
					Lst_textCritical[(int)equipType].text = string.Format("{0}%",cri);
					Lst_graphCurCritical[(int)equipType].fillAmount = cri / CriticalValueMax;
					Lst_graphNxtCritical[(int)equipType].fillAmount = 0;



					//가격
					price = Dic_weaponInfos[Selected_Item.ItemIdx].SellItNum.ToString();

					//제화타입
					if(Dic_weaponInfos[Selected_Item.ItemIdx].SellItTp == ITEMTYPE.GEM)
						buyItemType = DefineKey.Gem;
					else if(Dic_weaponInfos[Selected_Item.ItemIdx].SellItTp == ITEMTYPE.GOLD)
						buyItemType = DefineKey.Gold;
					
				}


				//정확도
				Lst_textAccuracy[(int)equipType].text = acurracy.ToString();
				Lst_graphCurAccuracy[(int)equipType].fillAmount = (float)acurracy / AccuracyValueMax;
				//반동제어
				Lst_textRecoil[(int)equipType].text = recoil.ToString();
				Lst_graphCurRecoil[(int)equipType].fillAmount = (float)recoil / RecoilValueMax;
				//발사속도
				Lst_textRateOfFire[(int)equipType].text = rateOfFire.ToString();
				Lst_graphCurRateOfFire[(int)equipType].fillAmount = (float)rateOfFire / RateofFireValueMax;
				//장전속도
				Lst_textReloadSpeed[(int)equipType].text = ((600f / (float)reloadSpeed)).ToString("N1");
				Lst_graphCurReloadSpeed[(int)equipType].fillAmount = (600f/(float)reloadSpeed) / ReloadSpeedValueMax;
				//탄창수
				Lst_textMagazine[(int)equipType].text = Dic_weaponInfos[Selected_Item.ItemIdx].Magazine.ToString();
				Lst_graphCurMagazine[(int)equipType].fillAmount = (float)Dic_weaponInfos[Selected_Item.ItemIdx].Magazine / MagazineValueMax;
				//줌스케일
				Lst_textZoom[(int)equipType].text = string.Format("x{0}", Dic_weaponInfos[Selected_Item.ItemIdx].ZoomScale);
				Lst_graphCurZoom[(int)equipType].fillAmount = (float)Dic_weaponInfos[Selected_Item.ItemIdx].ZoomScale / ZoomValueMax;


				//무기타입아이콘
				Lst_imageWpnType[(int)equipType].sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}",DefineKey.WpnType,(int)Dic_weaponInfos[Selected_Item.ItemIdx].WpnType));


			}
			else if (equipType == EquipType.Dress_HEAD || equipType == EquipType.Dress_BODY || equipType == EquipType.Dress_FACE)
			{
				ItemName = Dic_decoInfos[Selected_Item.ItemIdx].DecoName;

				byte gdp = Dic_decoInfos[Selected_Item.ItemIdx].GrdPwr;
				byte rspd = Dic_decoInfos[Selected_Item.ItemIdx].RspSpd;
				byte chrg = Dic_decoInfos[Selected_Item.ItemIdx].SklChgTm;

				List<GameObject> lstObj = null;
				if (equipType == EquipType.Dress_HEAD)
					lstObj = Lst_DecoHeadSpecOBJ;
				else if (equipType == EquipType.Dress_BODY)
					lstObj = Lst_DecoBodySpecOBJ;
				else if (equipType == EquipType.Dress_FACE)
					lstObj = Lst_DecoFaceSpecOBJ;

				
					lstObj[0].SetActive(gdp != 0);
					lstObj[1].SetActive(rspd != 0);
					lstObj[2].SetActive(chrg != 0);

				//스펙
				Lst_textDecoGDP[(int)equipType -2].text = string.Format("{0}%",gdp);
				Lst_textDecoRSPS[(int)equipType - 2].text = string.Format("{0}%", rspd);
				Lst_textDecoCHRS[(int)equipType - 2].text = string.Format("{0}%", chrg);


				
				if (!Selected_Item.IsHave)
				{
					//가격
					price = Dic_decoInfos[Selected_Item.ItemIdx].SellItNum.ToString();

					//제화타입
					if (Dic_decoInfos[Selected_Item.ItemIdx].SellItTp == ITEMTYPE.GEM)
						buyItemType = DefineKey.Gem;
					else if (Dic_decoInfos[Selected_Item.ItemIdx].SellItTp == ITEMTYPE.GOLD)
						buyItemType = DefineKey.Gold;
				}

			
			}

			//아이템이름
			Lst_textItemName[(int)equipType].text = ItemName;

			//구매 제화 이미지
			if (!string.IsNullOrEmpty(buyItemType))
			{
				Lst_ImageBuyGoodsType[(int)equipType].sprite = ImageManager.instance.Get_Sprite(buyItemType);
				Lst_ImageBuyGoodsType[(int)equipType].SetNativeSize();
			}

			//구매 가격
			Lst_textBuyPrice[(int)equipType].text = price;


			
			
				//정보란에 있는 버튼활성  (구매 or levelup버튼)
				Active_InfoButton(equipType, Selected_Item.IsHave);


			
		}
	}





	//아이템 선택시  설정 (아이템 백이미지, 버튼생성 ,아이템정보 표시)
	public void Set_SelectedItem(UIItem_EquipItem nowSelectItem)
	{
		Selected_Item = nowSelectItem;
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(nowEquipInven);

		//나머지 백이미지 노말로
		for (int i = 0; i < _lst_Items.Count; i++ )
		{
			if (_lst_Items[i] != nowSelectItem)
				_lst_Items[i].ButtonState = ButtonState_Type.Normal;
			else
			{
				//더블클릭 해서 장착하기 => 버튼이선택되어있고 && 소유중 &&  비장착중이면 
				//if (_lst_Items[i].ButtonState == ButtonState_Type.Selected)
				//{
				//    if (!_lst_Items[i].IsEquip && _lst_Items[i].IsHave)
				//    {
				//        ResponseButton_Equip();
				//    }
				//}
				
				_lst_Items[i].ButtonState = ButtonState_Type.Selected;
			}

		}


		////선택한아이템이 장착하지 않앗고 획득한 아이템이면 장착버튼생성
		//if (nowSelectItem.IsEquip || !nowSelectItem.IsHave)
		//    button_Equip.gameObject.SetActive(false);
		//else
		//    button_Equip.gameObject.SetActive(true);

		

		//아이템정보 표시
		//if (!Selected_Item.isUnEquipBtn)
			Set_SelectedItem_Infomation(nowEquipInven);

	}



	//장비 착용 적용하기
	public void Apply_EquipItem()
	{
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(nowEquipInven);
		uint userItemIdx = Get_EquipItemIndex(nowEquipInven);

		
		// 해당아이템 장착마크표시 , 나머지 장착마크 미표시
		for (int i = 0; i < _lst_Items.Count; i++)
		{

			//해제버튼일떄
			if (Selected_Item.ItemIdx == 1)
			{
				if (Selected_Item == _lst_Items[i] && Selected_Item.isDoubleClick)
				{
					_lst_Items[i].IsEquip = true;

				}
				else
				{
					if (_lst_Items[i].ItemIdx != userItemIdx)
					{
						if (_lst_Items[i].IsEquip)
							_lst_Items[i].IsEquip = true;
						else
							_lst_Items[i].IsEquip = false;
					}
					else
					{
						if (!Selected_Item.isDoubleClick)
							_lst_Items[i].IsEquip = true;
						else
							_lst_Items[i].IsEquip = false;

					}
				}
			}
            //유저가 착용중인 아이템과 같지 않을때
			else if (_lst_Items[i].ItemIdx != userItemIdx)
			{
                //=> 모두 장착해제처리한다 (왜 isDoubleClick처리 한것일까...)
                //if (Selected_Item.isDoubleClick)
                _lst_Items[i].IsEquip = false;
				//else
				//{
				//	if (_lst_Items[i].ItemIdx == 1 && _lst_Items[i].IsEquip)
				//		_lst_Items[i].IsEquip = true;
				//}
			}
			else //유저 착용중인 아이템과 같을때
				_lst_Items[i].IsEquip = true;
		}




		if (Selected_Item.ItemIdx == 1)
		{
			//장착하엿을떄는 장착버튼 비활성
			if (Selected_Item.IsEquip)
			{
				//button_Equip.gameObject.SetActive(false);
                Active_EquipButton(false,true);

            }
			else //선택한아이템이 장착하지 않앗고 획득한 아이템이면 장착버튼 활성
			{
                if (userItemIdx != 0)
                {
                    //button_Equip.gameObject.SetActive(true);
                    Active_EquipButton(true,false);
                }
                else
                {
                    Active_EquipButton(true, true);
                }
			}
		}
		else
		{
            if(Selected_Item.IsHave && !Selected_Item.IsEquip)
            {
                Active_EquipButton(true,false);
            }
            else if(!Selected_Item.IsHave)
            {
                Active_EquipButton(false,true);
            }
            else if(Selected_Item.IsEquip)
            {
                Active_EquipButton(false, false);

            }
            //소유하지 않거나, 장착하엿을떄는 장착버튼 비활성
   //         if (!Selected_Item.IsHave || Selected_Item.IsEquip)
			//{
			//	//button_Equip.gameObject.SetActive(false);
   //             Active_EquipButton(false);

   //         }
			//else //선택한아이템이 장착하지 않앗고 획득한 아이템이면 장착버튼 활성
			//{
   //             //button_Equip.gameObject.SetActive(true);
   //             //Active_EquipButton(true);

   //         }
		}

		//if (Selected_Item.IsEquip)
		//{
		//    if (nowEquipInven == EquipType.Main || nowEquipInven == EquipType.Sub)
		//        beEquip = 3;
		//    else
		//        beEquip = 1;
		//}
		//else if (!Selected_Item.IsEquip && !Selected_Item.IsHave)
		//{
		//    beEquip = 3;
		//}
		//else //선택한아이템이 장착하지 않앗고 획득한 아이템이면 장착버튼 활성
		//{
		//    //button_Equip.gameObject.SetActive(true);
		//    beEquip = 0;
		//}

		//for (int i = 0; i < Lst_EquipButtonOBJ.Count; i++) Lst_EquipButtonOBJ[i].SetActive(beEquip == i);


		
		if (Selected_Item.ItemIdx == 1)
		{
			//해제버튼 이 아니고 더블클릭상태가 아니면 
			if (Selected_Item.isUnEquipBtn /*&& Selected_Item.isDoubleClick*/)
				// 캐릭터 아이템 장착 or 캐릭터모델에 아이템적용(미리보기)
				Apply_useCharacter_EquipItem();
		}
		else
		{
			// 캐릭터 아이템 장착 or 캐릭터모델에 아이템적용(미리보기)
			Apply_useCharacter_EquipItem();
		}
	}


	//사용 캐릭터 구입한 장착아이템 적용
	void Apply_useCharacter_EquipItem()
	{
		equipCharacter.Apply_SelectItemCharacterEquip(nowEquipInven, Selected_Item.ItemIdx);

		//세트 이펙트활성 체크 하고(SetBufKnd반환) 맞으면 이펙트 활성
		SetBufKnd _setKind = equipCharacter.Chk_EquipSetBuf(true);
		if (_setKind != SetBufKnd.NONE)
		{
			//텍스트표시
			int txtIdx = 2000 + ((int)_setKind - 1);
			text_notiSet.text = TextDataManager.Dic_TranslateText[txtIdx]; // xx세트
			text_notiSet.color = DefineKey.SoldYellow;
		}
		else
		{
			//텍스트미표시
			text_notiSet.text = TextDataManager.Dic_TranslateText[516]; //세트효과 없음
			text_notiSet.color = DefineKey.White;
		}
	}







	//선택한 아이템 처리 
	public void Doprocess_SelectedEquipItem(UIItem_EquipItem nowSelectItem)
	{
		

		//아이템 선택시  설정 (아이템 백이미지, 버튼생성 ,아이템정보 표시)
		Set_SelectedItem(nowSelectItem);


		

		//장비 착용 적용하기
		Apply_EquipItem();


		//더블클릭 해서 장착하기 => 더블클릭 && 소유중 &&  비장착중이면 
		if (Selected_Item.isDoubleClick && !Selected_Item.IsEquip && Selected_Item.IsHave)
			ResponseButton_Equip(0);

		//더블클릭 &&  장착해제버튼이라면 장착해제
		if (Selected_Item.isUnEquipBtn && Selected_Item.isDoubleClick)
			ResponseButton_Equip(1);
		
	}






	//구매후 아이템 have 할당
	public void Set_Item_Have(EquipType _EquipType)
	{
		User _user = UserDataManager.instance.user;

		uint userItemIdx = Get_EquipItemIndex(_EquipType);
		
		Selected_Item.IsHave = true;


		//EuqipCharacter 데이터 갱신
		foreach (var userUnit in _user.User_Units)
		{
			if (Dic_EquiqCharacter.ContainsKey(userUnit.Value.Unitidx))
			{
				Dic_EquiqCharacter[userUnit.Value.Unitidx].userUnit = userUnit.Value;
			}
		}

		equipCharacter = Dic_EquiqCharacter[nowSelectUnitIdx];

		//장착하기
		//ResponseButton_Equip(0); -> 서버에서 해줌

		//아이템 선택시  설정 (아이템 백이미지, 버튼생성 )
		Set_SelectedItem(Selected_Item);

		//장비 착용 적용하기
		Apply_EquipItem();


		
	}



	//아이템리스트 정렬하기
	void Sort_EquipItemList(EquipType equipType)
	{

		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(equipType);
		
			var sortList = _lst_Items.OrderBy(n => n.SortIdx);
			for (int i = 0; i < sortList.ToList().Count; i++)
		{
			sortList.ToList()[i].transform.SetSiblingIndex(i);
		}
	}




	//장비 타입에 따른 장비아이템리스트 반환
	public List<UIItem_EquipItem> Get_nowEquipItemList(EquipType equipType)
	{
		if (equipType == EquipType.Main) return Lst_mainWeaponItems;
		else if (equipType == EquipType.Sub) return Lst_subWeaponItems;
		else if (equipType == EquipType.Dress_HEAD) return Lst_Dress1Items;
		else if (equipType == EquipType.Dress_BODY) return Lst_Dress2Items;
		else if (equipType == EquipType.Dress_FACE) return Lst_Dress3Items;
		else return null;
	}



	// 현재 착용중인 아이템인덱스 반환
	uint Get_EquipItemIndex(EquipType equipType)
	{
		User _user = UserDataManager.instance.user;
		uint userItemIdx = 0;
		if (equipType == EquipType.Main) userItemIdx = _user.User_Units[nowSelectUnitIdx].MainWpnIdx;
		else if (equipType == EquipType.Sub) userItemIdx = _user.User_Units[nowSelectUnitIdx].SubWpnIdx;
		else if (equipType == EquipType.Dress_HEAD) userItemIdx = (uint)_user.User_Units[nowSelectUnitIdx].DecoIdx1;
		else if (equipType == EquipType.Dress_BODY) userItemIdx = (uint)_user.User_Units[nowSelectUnitIdx].DecoIdx2;
		else if (equipType == EquipType.Dress_FACE) userItemIdx = (uint)_user.User_Units[nowSelectUnitIdx].DecoIdx3;

		return userItemIdx;
	}



    void Active_EquipButton(bool isActive, bool isAllDisable)
    {
        bool equipActive = false;
        bool disableEquipActive = false;

        //equipActive = isActive;
        //disableEquipActive = !isActive;

        equipActive = isAllDisable == true ? false : isActive;
        disableEquipActive = isAllDisable == true ? false : !isActive;

        button_Equip.gameObject.SetActive(equipActive);
        DisableEquipBtn.SetActive(disableEquipActive);
        
    }


	//활성화 시킬 장착오브젝트 (토글탭)
	void Active_EquipObject(EquipType equipType)
	{
		for (int i = 0; i < Lst_EquipInvenOBJ.Count; i++)
		{
			if (i == (int)equipType)
				Lst_EquipInvenOBJ[i].SetActive(true);
			else
				Lst_EquipInvenOBJ[i].SetActive(false);
		}
	}


	//정보란에 있는 버튼 활성/비활성
	void Active_InfoButton(EquipType equipType, bool _isHave)
	{
        //치장에 soldout 오브젝트 안나오게 하기 위해 main,sub 에서만 작동
        if(equipType == EquipType.Main || equipType== EquipType.Sub)
		    Lst_InfoNormalOBJ[(int)equipType].SetActive(_isHave);

		Lst_InfoBuyOBJ[(int)equipType].SetActive(!_isHave);
	}


	


	void Activate_SetIntroPopup(bool isactive)
	{
        //if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
        //    UI_Top.Getsingleton.gameObject.SetActive(!isactive);

        //SetIntroPopup.SetActive(isactive);

        if (isactive)
            popupSetItemInfo.Open_SetInfo(nowSelectUnitIdx);
        else
            popupSetItemInfo.Close_SetInfo();

    }




    //아이템 정보 초기화 (정보 = 버튼상태, 언락상태 , 소유상태)
    void Clear_EquipItemInfo(EquipType equipType)
	{
		List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(nowEquipInven);

		for (int i = 0; i < _lst_Items.Count; i++) _lst_Items[i].Clear_ItemInfo();
	}

	//모든아이템 정보 초기화 (정보 = 버튼상태, 언락상태 , 소유상태)
	void ClearAll_EquipItemInfo()
	{

		for (int i = 0; i < Lst_mainWeaponItems.Count; i++) Lst_mainWeaponItems[i].Clear_ItemInfo();
		for (int i = 0; i < Lst_subWeaponItems.Count; i++) Lst_subWeaponItems[i].Clear_ItemInfo();
		for (int i = 0; i < Lst_Dress1Items.Count; i++) Lst_Dress1Items[i].Clear_ItemInfo();
		for (int i = 0; i < Lst_Dress2Items.Count; i++) Lst_Dress2Items[i].Clear_ItemInfo();
		for (int i = 0; i < Lst_Dress3Items.Count; i++) Lst_Dress3Items[i].Clear_ItemInfo();
	}


	// UI 정보초기화
	void Clear_UIInfo()
	{
		for (int i = 0; i < Lst_textItemName.Count; i++) Lst_textItemName[i].text ="-";
		for (int i = 0; i < Lst_textPower.Count; i++) Lst_textPower[i].text = "-";
		for (int i = 0; i < Lst_textCritical.Count; i++) Lst_textCritical[i].text = "-";
		for (int i = 0; i < Lst_textRecoil.Count; i++) Lst_textRecoil[i].text = "-";
		for (int i = 0; i < Lst_textAccuracy.Count; i++) Lst_textAccuracy[i].text = "-";
		for (int i = 0; i < Lst_textRateOfFire.Count; i++) Lst_textRateOfFire[i].text = "-";
		for (int i = 0; i < Lst_textReloadSpeed.Count; i++) Lst_textReloadSpeed[i].text = "-";

		for (int i = 0; i < Lst_graphCurPower.Count; i++) Lst_graphCurPower[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphNxtCritical.Count; i++) Lst_graphNxtCritical[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurRecoil.Count; i++) Lst_graphCurRecoil[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurAccuracy.Count; i++) Lst_graphCurAccuracy[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurRateOfFire.Count; i++) Lst_graphCurRateOfFire[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurReloadSpeed.Count; i++) Lst_graphCurReloadSpeed[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurMagazine.Count; i++) Lst_graphCurMagazine[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphCurZoom.Count; i++) Lst_graphCurZoom[i].fillAmount = 0;

		for (int i = 0; i < Lst_graphNxtPower.Count; i++) Lst_graphNxtPower[i].fillAmount = 0;
		for (int i = 0; i < Lst_graphNxtCritical.Count; i++) Lst_graphNxtCritical[i].fillAmount = 0;




		for (int i = 0; i < Lst_textItemLv.Count; i++) Lst_textItemLv[i].text = "Lv.1";
		for (int i = 0; i < Lst_textBuyPrice.Count; i++) Lst_textBuyPrice[i].text = "0";
		for (int i = 0; i < Lst_DecoHeadSpecOBJ.Count; i++) Lst_DecoHeadSpecOBJ[i].SetActive(false);
		for (int i = 0; i < Lst_DecoBodySpecOBJ.Count; i++) Lst_DecoBodySpecOBJ[i].SetActive(false);
		for (int i = 0; i < Lst_DecoFaceSpecOBJ.Count; i++) Lst_DecoFaceSpecOBJ[i].SetActive(false);
		for (int i = 0; i < Lst_InfoBuyOBJ.Count; i++) Lst_InfoBuyOBJ[i].SetActive(false);
		for (int i = 0; i < Lst_InfoNormalOBJ.Count; i++) Lst_InfoNormalOBJ[i].SetActive(false);
	}









	
	// 장비UI 토글 선택시 
	public void ResposeToggle_Equipment(int equipIdx)
	{
		if (nowEquipInven != (EquipType)equipIdx)
		{
			nowEquipInven = (EquipType)equipIdx;

            CharRotateSector.Clear_Rotate();

            //장착버튼 다끄기
            Active_EquipButton(false, true);

            //장착 정보 설정하기 
            Set_EquipmentInfo(nowEquipInven);

			//rotateOBJ 이동, 캐릭터 뒤쪽보기 기능
			Apply_characterRotateBehaivour(nowEquipInven);
		}
	}





	//아이템 장착하기
	public void ResponseButton_Equip(int equipState)
	{
		if (Selected_Item != null)
		{
			User _user = UserDataManager.instance.user;
			User_Units nowUseUnit = _user.User_Units[nowSelectUnitIdx];
			uint willBeEquipIdx = 0;
			if (equipState == 0)
			{
				

				if (Selected_Item.IsHave)
					willBeEquipIdx = Selected_Item.ItemIdx;
				else if (Selected_Item.isUnEquipBtn)
				{
					Apply_EquipItem();
					willBeEquipIdx = 0;
				}
				else
					return;
			}
			else if (equipState == 1)
			{
				uint chkEquipIdx = 0;
				if (nowEquipInven == EquipType.Main) chkEquipIdx = nowUseUnit.MainWpnIdx;
				else if (nowEquipInven == EquipType.Sub)  chkEquipIdx = nowUseUnit.SubWpnIdx;
				else if (nowEquipInven == EquipType.Dress_HEAD) chkEquipIdx = (uint)nowUseUnit.DecoIdx1;
				else if (nowEquipInven == EquipType.Dress_BODY) chkEquipIdx = (uint)nowUseUnit.DecoIdx2;
				else if (nowEquipInven == EquipType.Dress_FACE) chkEquipIdx = (uint)nowUseUnit.DecoIdx3;

				if (chkEquipIdx == 0) return;

				willBeEquipIdx = 0;

			}
			

			if (nowEquipInven == EquipType.Main)
				webRequest.SetEquipItem(nowSelectUnitIdx, willBeEquipIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
			else if (nowEquipInven == EquipType.Sub)
				webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, willBeEquipIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
			else if (nowEquipInven == EquipType.Dress_HEAD)
				webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx, willBeEquipIdx, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
			else if (nowEquipInven == EquipType.Dress_BODY)
				webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, willBeEquipIdx, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
			else if (nowEquipInven == EquipType.Dress_FACE)
			{
				webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, willBeEquipIdx, callback_complete_Equip);
			}
			
		}
	}

	public void callback_complete_Equip()
	{
		User _user = UserDataManager.instance.user;

		//EuqipCharacter 데이터 갱신
		foreach (var userUnit in _user.User_Units)
		{
			if (Dic_EquiqCharacter.ContainsKey(userUnit.Value.Unitidx))
			{
				Dic_EquiqCharacter[userUnit.Value.Unitidx].userUnit = userUnit.Value;
			}
		}

		equipCharacter = Dic_EquiqCharacter[nowSelectUnitIdx];

        List<UIItem_EquipItem> _lst_Items = Get_nowEquipItemList(nowEquipInven);


        //아이템 선택시  설정 (아이템 백이미지, 버튼생성 ,아이템정보 표시)
        Selected_Item.isDoubleClick = true;
        Set_SelectedItem(Selected_Item);

		//장비 착용 적용하기
		Apply_EquipItem();
		
		//Refresh_EquipItem(nowEquipInven);
	}



	// 장착해제
	void ResponseButton_UnEquip()
	{
		if (Selected_Item != null)
		{
			if (Selected_Item.IsHave)
			{
				User _user = UserDataManager.instance.user;
				User_Units nowUseUnit = _user.User_Units[nowSelectUnitIdx];

				if (nowEquipInven == EquipType.Main)
					webRequest.SetEquipItem(nowSelectUnitIdx, 0, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
				else if (nowEquipInven == EquipType.Sub)
					webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx,0, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
				else if (nowEquipInven == EquipType.Dress_HEAD)
					webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx,0, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
				else if (nowEquipInven == EquipType.Dress_BODY)
					webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1,0, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);
				else if (nowEquipInven == EquipType.Dress_FACE)
					webRequest.SetEquipItem(nowSelectUnitIdx, nowUseUnit.MainWpnIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, 0, callback_complete_Equip);
			}

		}
	}




	//선택한 아이템 레벨업 
	public void ResponseButton_LevelUp()
	{
		User _user = UserDataManager.instance.user;

		
		int maxRefLv = TableDataManager.instance.Get_MaxRefLv();
		byte reflv = 0;

		if (nowEquipInven == EquipType.Main || nowEquipInven == EquipType.Sub)
			reflv = _user.User_Weapons[Selected_Item.ItemIdx].RefLv;


		// 선택한 아이템이 있고 보유중이면 레벨업한다 && 강화레벨 15 이하이면 한다
		if (Selected_Item != null && reflv < maxRefLv)
		{
			//로비캐릭회전 잠금
			User.isSelectedCharacter = true;


			UI_Popup_Reinforce popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Reinforce>(UIPOPUP.POPUPREINFORCE);
			popup.Set_ReinforceInfo(REINFORCE_TYPE.WEAPON, nowSelectUnitIdx, Selected_Item.ItemIdx);
			popup.Set_addEventYESButton(()=>callback_Try_RfEquipItem(popup.reinforceType));


		}
	}


	void callback_Try_RfEquipItem(REINFORCE_TYPE rfType)
	{
		User _user = UserDataManager.instance.user;
		UI_Popup_RfProcess popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_RfProcess>(UIPOPUP.POPUPRFPROCESS);
		popup.Set_addEventYESButton(callback_complete_ReinforceItem);

		if (_user.User_Weapons[Selected_Item.ItemIdx].RefFailCnt >=1)
			popup.Set_RfProcess(rfType, false, nowSelectUnitIdx, Selected_Item.ItemIdx);
		else
			popup.Set_RfProcess(rfType, true, nowSelectUnitIdx, Selected_Item.ItemIdx);

	}

	void callback_complete_ReinforceItem()
	{
		//구매후 아이템 have 할당
		//Set_Item_Have(nowEquipInven);

		//top 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();

		//다시 강화 창 띄우기 
		ResponseButton_LevelUp();
	}





	//선택한 아이템 구매한다
	public void ResponseButton_BuyItem()
	{
		if (Selected_Item != null)
		{
			User _user = UserDataManager.instance.user;
			uint ItemIdx = 0;
			uint price = 0;

			ItemIdx = Selected_Item.ItemIdx;

			if (nowEquipInven == EquipType.Main || nowEquipInven == EquipType.Sub)
			{
				price = Dic_weaponInfos[ItemIdx].SellItNum;
				webRequest.ShopBuyweapon(nowSelectUnitIdx, ItemIdx, price, callback_complete_BuyItem);
			}
			else if (nowEquipInven == EquipType.Dress_HEAD || nowEquipInven == EquipType.Dress_BODY || nowEquipInven == EquipType.Dress_FACE)
			{
				price = Dic_decoInfos[ItemIdx].SellItNum;
				webRequest.ShopBuyDeco(nowSelectUnitIdx, ItemIdx, price, callback_complete_BuyItem);

			}
		}
	}

	public void callback_complete_BuyItem()
	{
		//구매후 아이템 have 할당
		Set_Item_Have(nowEquipInven);

		//top 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();

	}







	public void ResponsButton_setIntroPopup()
	{
		Activate_SetIntroPopup(!popupSetItemInfo.gameObject.activeSelf);
	}





	public void ResponseButton_Back()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UIData udata = new UIData(new List<object> { nowSelectUnitIdx });
        UI_Manager.Getsingleton.CreatUI(UI.CHARACTERSETTING, _canvasTr, udata);
	}
}
