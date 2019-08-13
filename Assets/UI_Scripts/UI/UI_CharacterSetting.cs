using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_CharacterSetting : UI_Base
{
    #region singleton
    private static UI_CharacterSetting _instance;
    public static UI_CharacterSetting Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UI_CharacterSetting)) as UI_CharacterSetting;

                if (_instance == null)
                {
                    GameObject instanceObj = new GameObject("UI_CharacterSetting");
                    _instance = instanceObj.AddComponent<UI_CharacterSetting>();
                }
            }

            return _instance;
        }
    }
    #endregion

    public enum CharSettingTapType { None = 99, Info = 0,Weapon = 1,}
    public enum TapConfigUIObjectKind { Unlock = 0, UnlockBg, Weapon, Btn_Deco,Btn_BuyChar,TapToggleGroup }

    public enum RefFillImageType { Init = 0, Upgraded,}
    //==========멤버변수==============
    private User user;
    public characterInfo viewCharacter;
    public uint nowSelectUnitIdx;
    private int nowSelectCharToggleIdx;
    private CharSettingTapType nowTapType = CharSettingTapType.Info;


    private Dictionary<uint, Infos_Weapon> Dic_weaponInfos = new Dictionary<uint, Infos_Weapon>();


    private List<UIItem_EquipItem> Lst_mainWeaponItems = new List<UIItem_EquipItem>();
    private UIItem_EquipItem Selected_Item;					//선택한아이템



    // =====================ui======================

    //Drag 영역
    public RoatateSector CharRotateSector;


    //탭토글
    public List<Toggle> Lst_ToggleTap;

    //탭구성쪽 전체 오브젝트들
    public List<GameObject> Lst_tapConfigUIObj;

    //뷰캐릭터
    public Transform viewCharacterParentTr;


    //캐릭정보
    public Text Text_UnitName;
    public Image Image_MainSkillTumnail;
    public Text Text_Hp;
    public Text Text_MoveSpeed;
    public Text Text_ReloadSpeed;
    public Text Text_SkillDescrip;
    public Text Text_RefLv;


    //캐릭구매
    public Image Image_CharBuyType;
    public Text Text_CharBuyNum;

    //캐릭사용버튼
    public GameObject DisableUseCharObj;
    


    //무기정보
    public Text Text_WeaponName;
    public Image Image_WpnType;
    public List<Image> Lst_graphPower;               //cur공격력 그래프
    public List<Image> Lst_graphCritical;                //cur크리 그래프
    public Image Image_graphAccuracy;                //cur정확도 그래프
    public Image Image_graphRecoil;              //cur반동제어 그래프
    public Image Image_graphRateOfFire;          //cur공속 그래프
    public Image Image_graphReloadSpeed;         //cur장전속도 그래프
    public Image Image_graphMagazine;                //cur탄창용량 그래프
    public Image Image_graphZoom;				//cur줌배율 그래프
    public Text Text_Power;
    public Text Text_Cri;
    public Text Text_Accuracy;
    public Text Text_Recoil;
    public Text Text_RateofFire;
    public Text Text_ReloadSpeeding;
    public Text Text_Magazine;
    public Text Text_Zoom;


    public Text Text_WpnUpgrade;
    public Text Text_WpnRefLv;
    public Text Text_WpnBuyPrice;
    public Image Image_WpnBuyType;

    //무기관련버튼
    public GameObject UpgradeBtnObj;
    public GameObject BuyWeapnBtnObj;
    public GameObject EquipBtnObj;
    public GameObject DisableEquipObj;

    //무기리스트
    public Transform weaponLstTr;


    //캐릭터선택
    public List<Toggle> Lst_ToggleSelectCharacter;

    //배치넘버
    public Text Text_BatchNum;




    public override void set_Open()
    {
        base.set_Open();

        nowSelectCharToggleIdx = 99;
        nowTapType = CharSettingTapType.Info;
        Set_CharSetting();
    }


    public override void set_Close()
    {
        base.set_Close();
    }

    public override void set_refresh()
    {
        base.set_refresh();

        Set_CharSetting();
    }





    void Set_CharSetting()
    {
        user = UserDataManager.instance.user;

        //무기데이터 예외할당처리
        Sign_WeaponDataInfo();

        uint recievedSelectUnitidx = 0;

        //이전UI에서부터 넘어온 캐릭터 인덱스!!!!
        if (BaseData.m_Datas != null && BaseData.m_Datas.Count > 0)
            recievedSelectUnitidx = (uint)BaseData.m_Datas[0];


        //외부에서부터 선택한 유닛이랑 현재 유닛 인덱스랑 다르면 토글켜서 진행
        if (nowSelectUnitIdx != recievedSelectUnitidx)
        {

            //해당 유닛 토글 켜기
            int toggleOnIdx = GetToggIdeFromUnitIdx(recievedSelectUnitidx);

            Lst_ToggleSelectCharacter[toggleOnIdx].isOn = true;
        }
        else
        {
            //유닛 같으니 갱신을 위해 진행

            //뷰캐릭터 설정
            Set_ViewCharacter();

            //캐릭터 config 설정
            Set_TapInfomation(nowTapType);

        }
    }







    void Sign_WeaponDataInfo()
    {
        Dictionary<uint, Infos_EventItemTime> _dicEventShop = TableDataManager.instance.Infos_EventItemTimes;
        Dictionary<uint, Infos_Weapon> _dicInfosweapn = TableDataManager.instance.Infos_weapons;

        //할당될 장착컬렉션들 클리어 시켜주자 => refesh되기 떄문에
        Dic_weaponInfos.Clear();

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
                    if (user.User_Weapons.ContainsKey(wpn.Value.WpnIdx))
                        Dic_weaponInfos[wpn.Value.WpnIdx] = wpn.Value;
                }
            }
            else
            {
                if(wpn.Value.WpnPart == EQUIPPART_TYPE.PRIMARY) //메인무기만 필터링
                 Dic_weaponInfos[wpn.Value.WpnIdx] = wpn.Value;
            }
        }


        
    }




    void Set_ViewCharacter()
    {
        User_Units _userUnit = null;
        infos_unit _infoUnit = null;

        if (user.User_Units.ContainsKey(nowSelectUnitIdx))
            _userUnit = user.User_Units[nowSelectUnitIdx];

        if (TableDataManager.instance.Infos_units.ContainsKey(nowSelectUnitIdx))
            _infoUnit = TableDataManager.instance.Infos_units[nowSelectUnitIdx];

        if (viewCharacter == null)
        {
           

            if (_userUnit != null)
            {
                //선택한 캐릭터 생성
                Transform unitTr = ObjectPoolManager.Getinstance.Get_ObjectLobbyUnit(nowSelectUnitIdx.ToString());
                unitTr.transform.parent = viewCharacterParentTr;
                unitTr.transform.localPosition = new Vector3(0,0,500f);
                unitTr.transform.localRotation = Quaternion.Euler(new Vector3(0, 180,0 ));
                unitTr.transform.localScale = new Vector3(225.0f, 225.0f, 225.0f); // 크기 225로 잡기

                characterInfo lc = unitTr.GetComponent<characterInfo>();

                //캐릭 초기화
                lc.Init_chrctBasicInfo(_userUnit, _infoUnit);

                //회전관련 값 할당
                CharRotateSector.Init_RotateSector(unitTr);

                //생성된캐릭 할당
                viewCharacter = lc;
            }
        }
        else
        {
            //소유중인 유닛이면
            //if (_userUnit != null)
            {
                // characterInfo 갱신위한 파라미터 재할당
                viewCharacter.Refresh_ChrctInfo(_userUnit, _infoUnit);
            }
        }


        //배치값 
        Text_BatchNum.text = user.GetBatchTxt(user.User_useUnit.nowSelectBatch);
    }






    void Set_TapInfomation(CharSettingTapType tapType)
    {

        //tap종류에따른 object 활성하기
        CharSettingTapType getTapType = Activation_TapInfomation(tapType, nowSelectUnitIdx);

        if (getTapType == CharSettingTapType.Info)
        {
            Set_TapCharacterInfomation();
        }
        else if (getTapType == CharSettingTapType.Weapon)
        {
            Set_TapWeaponInfomation();
        }

    }







    //tap종류에따른 object 활성하기
    CharSettingTapType Activation_TapInfomation(CharSettingTapType _nowTapType, uint unitidx)
    {
        bool isOnBtnBuyChar = false;
        bool isOnBtnDeco = false;
        bool isOnTapToggleGroup = false;
        bool isOnUnlockBg = false;
        bool isOnUnlock = false;
        bool isOnWeapon = false;

        //캐릭을 소유중
        if (user.User_Units.ContainsKey(unitidx))
        {
            isOnBtnBuyChar = false; // 구매버튼 비활성
            isOnBtnDeco = true; // 꾸미기버튼 활성
            isOnUnlockBg = true; // unlock 뒷배경활성
            isOnTapToggleGroup = true; //tap토글 활성
        }
        else
        {
            //소유하지 않고 잇으니 info 로
            _nowTapType = CharSettingTapType.Info;
            isOnBtnBuyChar = true; // 구매버튼 활성
            isOnBtnDeco = false;  // 꾸미기버튼 비활성
            isOnUnlockBg = false;  // unlock 뒷배경 비활성
            isOnTapToggleGroup = false; //탭 토글 비활성


            //구매버튼활성 하엿으니 구매가격및 구매재화
            infos_unit _infoUnit = null;
            if (TableDataManager.instance.Infos_units.ContainsKey(nowSelectUnitIdx))
                _infoUnit = TableDataManager.instance.Infos_units[nowSelectUnitIdx];

            if (_infoUnit != null)
            {
                //구매가격
                Text_CharBuyNum.text = _infoUnit.SellItNum.ToString();

                //구매재화이미지
                if (_infoUnit.SellItTp == ITEMTYPE.GEM)
                    Image_CharBuyType.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
                else if (_infoUnit.SellItTp == ITEMTYPE.GOLD)
                    Image_CharBuyType.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);

            }

        }



        if (_nowTapType == CharSettingTapType.Info)
        {
            isOnWeapon = false;
            isOnUnlock = true;
        }
        else if (_nowTapType == CharSettingTapType.Weapon)
        {
            isOnWeapon = true;
            isOnUnlock = false;
        }

        Active_tapConfigUIObj(TapConfigUIObjectKind.UnlockBg, isOnUnlockBg);
        Active_tapConfigUIObj(TapConfigUIObjectKind.Btn_BuyChar, isOnBtnBuyChar);
        Active_tapConfigUIObj(TapConfigUIObjectKind.TapToggleGroup, isOnTapToggleGroup);
        Active_tapConfigUIObj(TapConfigUIObjectKind.Btn_Deco, isOnBtnDeco);
        Active_tapConfigUIObj(TapConfigUIObjectKind.Unlock, isOnUnlock);
        Active_tapConfigUIObj(TapConfigUIObjectKind.Weapon, isOnWeapon);

        return _nowTapType;

    }







    //캐릭터 정보
    void Set_TapCharacterInfomation()
    {
        infos_unit _infoUnit = null;
        User_Units _userUnit = null;

        if (user.User_Units.ContainsKey(nowSelectUnitIdx))
            _userUnit = user.User_Units[nowSelectUnitIdx];

        if (TableDataManager.instance.Infos_units.ContainsKey(nowSelectUnitIdx))
            _infoUnit = TableDataManager.instance.Infos_units[nowSelectUnitIdx];

        if (_infoUnit != null)
        {
            //스킬이미지
            Image_MainSkillTumnail.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.lobbySkill, (int)_infoUnit.SkillKind));

            //이름
            Text_UnitName.text = _infoUnit.UnitName;

            //스킬설명
            Text_SkillDescrip.text = TextDataManager.Dic_TranslateText[(int)_infoUnit.UnitIdx];

            //레벨
            int lv = _userUnit != null ? _userUnit.RefLv : 1;
            Text_RefLv.text = string.Format("Lv.{0}", lv);



            float initHp = _infoUnit.Hp;
            float initMv = _infoUnit.MvSpeed;
            float initRld = _infoUnit.RldSpeed;
            int reflv = _userUnit != null ? _userUnit.RefLv : 1;

            //hp
            Text_Hp.text = ((int)StaticMethod.Get_nextSpec(initHp, reflv, true)).ToString();

            //movespeed
            Text_MoveSpeed.text = reflv > 4
            ? (StaticMethod.Get_nextSpec(initMv, reflv, false, 0.05f, 4)).ToString() //5렙이상시
            : initMv.ToString(); // 5렙이하시 기본값

            //reloadSpeed
            Text_ReloadSpeed.text = reflv > 9 
            ? "+" + Mathf.Abs((int)StaticMethod.Get_nextSpec(initRld, reflv, true, 5f, 9)).ToString() //9렙이상시
            : "+" + initRld.ToString(); // 9렙이하시 기본값





            //선택 캐릭와 현재배치의캐릭 과 같은지 체크
            bool isUseChar = nowSelectUnitIdx == user.User_useUnit.UnitIdxs[(int)user.User_useUnit.nowSelectBatch]
                ? true : false;
            //같으면 disable 활성
            DisableUseCharObj.SetActive(isUseChar);
        }



    }











    //무기 정보 관련
    void Set_TapWeaponInfomation()
    {
        if(Lst_mainWeaponItems != null)
        {
            if(Lst_mainWeaponItems.Count <=0)
            {
                StartCoroutine(Creat_WeaponEquipItem());
            }
            else
            {
                //현재 착용한 무기
                Selected_Item = Get_EquipWeaponItem();
                Process_EquipItemInfo(Selected_Item);
            }
        }
    }


    //아아템생성
    IEnumerator Creat_WeaponEquipItem()
    {
        Loadmanager.instance.LoadingUI(true);
        yield return StartCoroutine(routine_CreatItem());
        //현재 착용한 무기
        Selected_Item = Get_EquipWeaponItem();
        Process_EquipItemInfo(Selected_Item);
        Loadmanager.instance.LoadingUI(false);
    }

    IEnumerator routine_CreatItem()
    {



        //무기 아이템 생성
        {
            foreach (var weapon in Dic_weaponInfos.OrderBy(n => n.Value.SortIdx))
            {
                //타입
                int partIdx = (int)weapon.Value.WpnPart;
                    //생성및 리스트 할당
                    Sign_CreateItem( weapon.Value.WpnIdx, partIdx, weapon.Value.SortIdx);
                yield return null;
            }


            //리스트 정렬하기 itemidx 순
            var sortList = Lst_mainWeaponItems.OrderBy(n => n.SortIdx);
            for (int i = 0; i < sortList.ToList().Count; i++)
                sortList.ToList()[i].transform.SetSiblingIndex(i);
        }
        



    }


    void Sign_CreateItem( uint Itemidx, int partIdx, int sortIdx)
    {
        //생성
        UIItem_EquipItem item = UI_Manager.Getsingleton.CreatUI(
            UIITEM.ITEM_EQUIPITEM, weaponLstTr) as UIItem_EquipItem;
        item.Set_ItemInfo(Itemidx, partIdx, sortIdx);

        //리스트할당
        Lst_mainWeaponItems.Add(item);

    }




    void Process_EquipItemInfo(UIItem_EquipItem targetSelectItem)
    {
       

        //무기아이템 생성및 삭제 관련 체크
        Chk_WeaponItem_Created();

        
        //아이템 상태 설정하기
        Set_WeaponItemState(targetSelectItem);

        //무기 정보 설정
        Set_WeaponInfomation(targetSelectItem);
    }




    //해당 무기 정보 설정
    void Set_WeaponInfomation(UIItem_EquipItem targetSelectItem)
    {

        if (targetSelectItem != null)
        {
            //아이템 이름
            Text_WeaponName.text = Dic_weaponInfos[targetSelectItem.ItemIdx].WpnName;

            //무기 타입이미지
            Image_WpnType.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.WpnType, (int)Dic_weaponInfos[Selected_Item.ItemIdx].WpnType));


            //최대치
            float AttValueMax = 100f;
            float CriticalValueMax = 20f;
            float AccuracyValueMax = 300f;
            float RecoilValueMax = 100f;
            float RateofFireValueMax = 600f;
            float ReloadSpeedValueMax = 10f;
            float MagazineValueMax = 100f;
            float ZoomValueMax = 10f;




            float initAttMin = (float)Dic_weaponInfos[targetSelectItem.ItemIdx].AtkMin;
            float initAttMax = (float)Dic_weaponInfos[targetSelectItem.ItemIdx].AtkMax;
            float initCri = Dic_weaponInfos[targetSelectItem.ItemIdx].Critical;
            int acurracy = Dic_weaponInfos[targetSelectItem.ItemIdx].AimInit;
            int recoil = Dic_weaponInfos[targetSelectItem.ItemIdx].AimCtrl;
            int rateOfFire = Dic_weaponInfos[targetSelectItem.ItemIdx].AtkSpeed;
            int reloadSpeed = Dic_weaponInfos[targetSelectItem.ItemIdx].GunReload;


            if (targetSelectItem.IsHave)
            {
                int reflv = user.User_Weapons[targetSelectItem.ItemIdx].RefLv;

                //공격력
                float att = StaticMethod.Get_nextSpec(initAttMax, reflv, true);
                Text_Power.text = att.ToString();
                Lst_graphPower[(int)RefFillImageType.Init].fillAmount = initAttMax / AttValueMax;
                Lst_graphPower[(int)RefFillImageType.Upgraded].fillAmount = att / AttValueMax;


                //크리
                float cri = StaticMethod.Get_nextSpec(initCri, reflv, false);
                Text_Cri.text = cri.ToString();
                Lst_graphPower[(int)RefFillImageType.Init].fillAmount = initCri / CriticalValueMax;
                Lst_graphPower[(int)RefFillImageType.Upgraded].fillAmount = cri / CriticalValueMax;

                //lv
                if (user.User_Weapons.ContainsKey(targetSelectItem.ItemIdx))
                {
                    int lv = user.User_Weapons[targetSelectItem.ItemIdx].RefLv;
                    Text_WpnRefLv.text = string.Format("Lv.{0}", lv);

                    if (lv >= 15)
                        Text_WpnUpgrade.text = TextDataManager.Dic_TranslateText[410]; //MAX
                    else
                        Text_WpnUpgrade.text = TextDataManager.Dic_TranslateText[413]; // 강화

                }


            }
            else
            {
                //공격력
                float att = initAttMax;
                Text_Power.text = att.ToString();
                Lst_graphPower[(int)RefFillImageType.Init].fillAmount = initAttMax / AttValueMax;
                Lst_graphPower[(int)RefFillImageType.Upgraded].fillAmount = 0 / AttValueMax;


                //크리
                float cri = initCri;
                Text_Cri.text = cri.ToString();
                Lst_graphPower[(int)RefFillImageType.Init].fillAmount = initCri / CriticalValueMax;
                Lst_graphPower[(int)RefFillImageType.Upgraded].fillAmount = 0 / CriticalValueMax;

                //lv
                Text_WpnRefLv.text = string.Format("Lv.{0}", 1);

                //가격
                string price = Dic_weaponInfos[targetSelectItem.ItemIdx].SellItNum.ToString();

                //제화타입
                string buyItemType = string.Empty;
                if (Dic_weaponInfos[targetSelectItem.ItemIdx].SellItTp == ITEMTYPE.GEM)
                    buyItemType = DefineKey.Gem;
                else if (Dic_weaponInfos[targetSelectItem.ItemIdx].SellItTp == ITEMTYPE.GOLD)
                    buyItemType = DefineKey.Gold;

                Text_WpnBuyPrice.text = price;
                Image_WpnBuyType.sprite = ImageManager.instance.Get_Sprite(buyItemType);
            }

            //정확도
            Text_Accuracy.text = acurracy.ToString();
            Image_graphAccuracy.fillAmount = (float)acurracy / AccuracyValueMax;
            //반동제어
            Text_Recoil.text = recoil.ToString();
            Image_graphRecoil.fillAmount = (float)recoil / RecoilValueMax;
            //발사속도
            Text_RateofFire.text = rateOfFire.ToString();
            Image_graphRateOfFire.fillAmount = (float)rateOfFire / RateofFireValueMax;
            //장전속도
            Text_ReloadSpeeding.text = ((600f / (float)reloadSpeed)).ToString("N1");
            Image_graphReloadSpeed.fillAmount = (600f / (float)reloadSpeed) / ReloadSpeedValueMax;
            //탄창수
            Text_Magazine.text = Dic_weaponInfos[targetSelectItem.ItemIdx].Magazine.ToString();
            Image_graphMagazine.fillAmount = (float)Dic_weaponInfos[targetSelectItem.ItemIdx].Magazine / MagazineValueMax;
            //줌스케일
            Text_Zoom.text = string.Format("x{0}", Dic_weaponInfos[Selected_Item.ItemIdx].ZoomScale);
            Image_graphZoom.fillAmount = (float)Dic_weaponInfos[targetSelectItem.ItemIdx].ZoomScale / ZoomValueMax;




            //버튼들 처리
            bool upgradeBtnActive = Selected_Item.ItemIdx == 20100 ? false : targetSelectItem.IsHave;
            UpgradeBtnObj.SetActive(upgradeBtnActive);
            bool buyBtnActive = Selected_Item.ItemIdx == 20100 ? false : !targetSelectItem.IsHave;
            BuyWeapnBtnObj.SetActive(buyBtnActive);
            EquipBtnObj.SetActive(targetSelectItem.IsHave);
            DisableEquipObj.SetActive(targetSelectItem.IsHave && targetSelectItem.IsEquip);
        }
    }









    // 아이템이 생성이 되엇는지 체크 (없어야 할 아이템이면 비활성, 있어야할 아이템이면 생성)
    void Chk_WeaponItem_Created()
    {
        List<UIItem_EquipItem> _lst_Items = Lst_mainWeaponItems;

        //생성 된것중에 비활성시켜야할 아이템체크
        for (int i = 0; i < _lst_Items.Count; i++)
        {
            if (!_lst_Items[i].isUnEquipBtn) // 아이템오브젝트가 해제버튼이 아니라면
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
        }



        //생성 안된것중 생성시켜야할 아이템체크
        var notCreatedItemWpn = Dic_weaponInfos.Where(n => (int)n.Value.WpnPart == (int)(EquipType.Main + 1))
            .Where(n => !_lst_Items.ToDictionary(g => g.ItemIdx, g => g.ItemIdx).ContainsKey(n.Value.WpnIdx));

        if (notCreatedItemWpn.Count() > 0)
        {

            foreach (var ncItem in notCreatedItemWpn)
            {
                //생성및 리스트 할당
                Sign_CreateItem(ncItem.Value.WpnIdx, (int)ncItem.Value.WpnPart, ncItem.Value.SortIdx);
            }

            //리스트 정렬하기 itemidx 순
            var sortList = Lst_mainWeaponItems.OrderBy(n => n.SortIdx);
            for (int i = 0; i < sortList.ToList().Count; i++)
                sortList.ToList()[i].transform.SetSiblingIndex(i);

        }




    }


    //아이템 상태 설정하기 (소유중,착용중,선택함 등등)
    void Set_WeaponItemState(UIItem_EquipItem targetSelectItem)
    {
        


        
        for (int i = 0; i < Lst_mainWeaponItems.Count; i++)
        {
            //아이템 백이미지 처리
            if (Lst_mainWeaponItems[i].ItemIdx != targetSelectItem.ItemIdx)
            {
                Lst_mainWeaponItems[i].ButtonState = ButtonState_Type.Normal;
            }
            else
            {
                Lst_mainWeaponItems[i].ButtonState = ButtonState_Type.Selected;

            }


            //소유햇는지 체크
            if(user.User_Weapons.ContainsKey(Lst_mainWeaponItems[i].ItemIdx))
            {
                Lst_mainWeaponItems[i].IsHave = true;
            }
            else
            {
                Lst_mainWeaponItems[i].IsHave = false;
            }


            if(user.User_Units.ContainsKey(nowSelectUnitIdx))
            {
                //장착해는지 체크
                if (user.User_Units[nowSelectUnitIdx].MainWpnIdx == Lst_mainWeaponItems[i].ItemIdx)
                    Lst_mainWeaponItems[i].IsEquip = true;
                else
                    Lst_mainWeaponItems[i].IsEquip = false;
            }


            //소유중인 유닛이 무기 장착 체크 (AK 기본무기는 항상 false)
            if (user.GetEquipMainWpnIdxsOfHaveUnits(nowSelectUnitIdx).Contains(Lst_mainWeaponItems[i].ItemIdx))
            {
                Lst_mainWeaponItems[i].IsEquipedOtherUnit = true;

                if(Lst_mainWeaponItems[i].ItemIdx == 20100)
                    Lst_mainWeaponItems[i].IsEquipedOtherUnit = false;
            }
            else
                Lst_mainWeaponItems[i].IsEquipedOtherUnit = false;

        }
    }






    //장비 착용 적용하기
    void Apply_EquipItem()
    {
        if(viewCharacter!= null)
        {
            viewCharacter.Apply_SelectItemCharacterEquip(EquipType.Main, Selected_Item.ItemIdx);
        }
    }













    //선택한 아이템 처리 
    public void Doprocess_SelectedEquipItem(UIItem_EquipItem nowSelectItem)
    {

        Selected_Item = nowSelectItem;
        //아이템 상태 설정하기 (소유중,착용중,선택함 등등)
        Process_EquipItemInfo(nowSelectItem);




        //장비 착용 적용하기
        Apply_EquipItem();


        //더블클릭 해서 장착하기 => 더블클릭 && 소유중 &&  비장착중이면 
        if (Selected_Item.isDoubleClick && !Selected_Item.IsEquip && Selected_Item.IsHave)
            ResponseButton_Equip(0);

        //더블클릭 &&  장착해제버튼이라면 장착해제
        if (Selected_Item.isUnEquipBtn && Selected_Item.isDoubleClick)
            ResponseButton_Equip(1);

    }










    void Active_tapConfigUIObj(TapConfigUIObjectKind tapConfigKnd, bool isActive)
    {
        Lst_tapConfigUIObj[(int)tapConfigKnd].SetActive(isActive);
    }


    
    int GetToggIdeFromUnitIdx(uint _unitIdx)
    {
        int idx = ((int)_unitIdx % 10000) -1;

        return idx;
    }

    uint GetUnitidxFromToggleIdx(int idx)
    {
        uint _unitIdx = (uint)(10000 + idx);

        return _unitIdx;
    }


    UIItem_EquipItem Get_EquipWeaponItem()
    {
        User _user = UserDataManager.instance.user;
        uint userItemIdx = 0;
        userItemIdx = _user.User_Units[nowSelectUnitIdx].MainWpnIdx;

        if (userItemIdx == 0)
            userItemIdx = 20100;

        UIItem_EquipItem equipWpn = null;

        for (int i = 0; i < Lst_mainWeaponItems.Count; i++)
        {
            if(userItemIdx == Lst_mainWeaponItems[i].ItemIdx)
            {
                equipWpn = Lst_mainWeaponItems[i];
                break;
            }
        }

        return equipWpn;
    }




    //캐릭터 선택 토글
    public void ResponseToggle_SelectCharacter(int idx)
    {
        if(nowSelectCharToggleIdx!= idx)
        {
            nowSelectCharToggleIdx = idx;
            nowSelectUnitIdx = GetUnitidxFromToggleIdx(idx);

            //뷰캐릭터 설정
            Set_ViewCharacter();

            //캐릭터 config 설정
            Set_TapInfomation(nowTapType);
        }
    }



    //탭정보창 선택 토글
    public void ResponseToggle_TapInfoSelect(int idx)
    {
        if(nowTapType != (CharSettingTapType)idx)
        {
            nowTapType = (CharSettingTapType)idx;

            Set_TapInfomation(nowTapType);
        }
    }











    //캐릭 구매

    public void ResponseButton_BuyCharacter()
    {

        Dictionary<uint, infos_unit> _dic_infosUnit = TableDataManager.instance.Infos_units;


        webRequest.ShopBuyUnit(nowSelectUnitIdx, _dic_infosUnit[nowSelectUnitIdx].SellItNum, () => callback_complete_buyCharacter());
    }




    void callback_complete_buyCharacter()
    {

        //뷰캐릭터 설정
        Set_ViewCharacter();

        //캐릭터 config 설정
        Set_TapInfomation(nowTapType);

        //top 재화갱신
        UI_Top.Getsingleton.set_refresh();

      
    }






    //캐릭 사용하기
    public void ResponseButton_UseCharacter()
    {
        BatchType batch = user.User_useUnit.nowSelectBatch;

        uint[] useUnitIdxs= user.User_useUnit.UnitIdxs;



        //사용중인지 확인하기
        int SameIndex = 0;
        bool isUsingOneOfSlots = false;
        for (int i = 0; i < useUnitIdxs.Length; i++)
        {
            if(batch != (BatchType)i && nowSelectUnitIdx == useUnitIdxs[i])
            {
                SameIndex = i;
                isUsingOneOfSlots = true;
                
            }
        }


        //다른 슬롯에서 선택한캐릭을 사용중이다
        if(isUsingOneOfSlots)
        {
            string unitName = TableDataManager.instance.Infos_units[nowSelectUnitIdx].UnitName;

            UI_Popup_Toast popuptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
            popuptoast.SetPopupMessage(string.Format("{0} 캐릭터를 {1}로 사용합니다",unitName,user.GetBatchTxt(batch)));

            uint curBatchUnitidx = useUnitIdxs[(int)batch];
            useUnitIdxs[(int)batch] = nowSelectUnitIdx;
            useUnitIdxs[SameIndex] = curBatchUnitidx;

            Debug.Log("배치 스의치 : " + useUnitIdxs[0] + " / " + useUnitIdxs[1] + " / " + useUnitIdxs[2]);
        }
        else
        {
            useUnitIdxs[(int)batch] = nowSelectUnitIdx;
        }

        webRequest.SetUseUnitIdx(useUnitIdxs[0], useUnitIdxs[1], useUnitIdxs[2], callback_complete_useCharacter);

    }

    void callback_complete_useCharacter()
    {
        user = UserDataManager.instance.user;
        Set_TapCharacterInfomation();
    }










    //캐릭터 레벨업(강화)
    public void ResponseButton_CharacterLvUp()
    {


        UI_Popup_Reinforce popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Reinforce>(UIPOPUP.POPUPREINFORCE);
        popup.Set_ReinforceInfo(REINFORCE_TYPE.UNIT, nowSelectUnitIdx, nowSelectUnitIdx);
        popup.Set_addEventYESButton(() => callback_Try_Reinforce(popup.reinforceType));

    }



    void callback_Try_Reinforce(REINFORCE_TYPE rfType)
    {
        UI_Popup_RfProcess popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_RfProcess>(UIPOPUP.POPUPRFPROCESS);
        popup.Set_addEventYESButton(() => callback_complete_Rfprocess(nowSelectUnitIdx));

        //실패 수가 1이상이면 실패!
        if (user.User_Units[nowSelectUnitIdx].RefFailCnt >= 1)
            popup.Set_RfProcess(rfType, false, nowSelectUnitIdx, nowSelectUnitIdx);
        else // 아니면 강화성공
            popup.Set_RfProcess(rfType, true, nowSelectUnitIdx, nowSelectUnitIdx);




    }




    //유닛 강화 완료햇으니 정보 갱신 
    void callback_complete_Rfprocess(uint _unitIdx)
    {
        user = UserDataManager.instance.user;
        Set_TapCharacterInfomation();

        //top 재화갱신
        UI_Top.Getsingleton.set_refresh();

        //다시 강화하기 팝업띄우기
        ResponseButton_CharacterLvUp();

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
                Selected_Item.isDoubleClick = true;

                if (Selected_Item.IsHave)
                {
                    willBeEquipIdx = Selected_Item.ItemIdx;

                    uint alreadyEquipedWeaponUnitidx = user.GetUnitidxsWithEquipedThisWeapon(willBeEquipIdx);
                    //다른유닛이 무기를 착용하고 잇는지 체크
                    if (alreadyEquipedWeaponUnitidx != 0)
                    {

                        UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
                        popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]); // 친구삭제
                        popup.SetPopupMessage("다른 히어로가 장착하고 있는 무기 입니다 \n장착하시겠습니까?");
                        popup.Set_addEventButton(()=> complete_ChkWeaponEquipment(nowSelectUnitIdx,willBeEquipIdx,nowUseUnit));
                        return;
                    }

                }
                else if (Selected_Item.isUnEquipBtn)
                {
                    //Apply_EquipItem();
                    willBeEquipIdx = 0;
                }
                else
                    return;
            }
            else if (equipState == 1)
            {
                uint chkEquipIdx = 0;
              chkEquipIdx = nowUseUnit.MainWpnIdx;
                

                if (chkEquipIdx == 0) return;

                willBeEquipIdx = 0;

            }


                webRequest.SetEquipItem(nowSelectUnitIdx, willBeEquipIdx, nowUseUnit.SubWpnIdx, (uint)nowUseUnit.DecoIdx1, (uint)nowUseUnit.DecoIdx2, (uint)nowUseUnit.DecoIdx3, callback_complete_Equip);

        }
    }

    void complete_ChkWeaponEquipment(uint unitidx, uint changingEquipIdx,User_Units _UseUnit)
    {
        webRequest.SetEquipItem(unitidx, changingEquipIdx, _UseUnit.SubWpnIdx, 
            (uint)_UseUnit.DecoIdx1, (uint)_UseUnit.DecoIdx2, (uint)_UseUnit.DecoIdx3, callback_complete_Equip);


        //연탁형이 다른유닛 무기스위칭 처리해서 데이터보내줄것임
       
    }

    void callback_complete_Equip()
    {
        user = UserDataManager.instance.user;


        Process_EquipItemInfo(Selected_Item);
        //장비 착용 적용하기
        //Apply_EquipItem();

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

            price = Dic_weaponInfos[ItemIdx].SellItNum;
            webRequest.ShopBuyweapon(nowSelectUnitIdx, ItemIdx, price, callback_complete_BuyItem);
        }
    }

    void callback_complete_BuyItem()
    {
        user = UserDataManager.instance.user;
        Process_EquipItemInfo(Selected_Item);

        //top 갱신
        if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
            UI_Top.Getsingleton.set_refresh();

    }







    //선택한 아이템 레벨업 
    public void ResponseButton_LevelUp()
    {
        User _user = UserDataManager.instance.user;


        int maxRefLv = TableDataManager.instance.Get_MaxRefLv();
        byte reflv = 0;

            reflv = _user.User_Weapons[Selected_Item.ItemIdx].RefLv;


        // 선택한 아이템이 있고 보유중이면 레벨업한다 && 강화레벨 15 이하이면 한다
        if (Selected_Item != null && reflv < maxRefLv)
        {


            UI_Popup_Reinforce popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Reinforce>(UIPOPUP.POPUPREINFORCE);
            popup.Set_ReinforceInfo(REINFORCE_TYPE.WEAPON, nowSelectUnitIdx, Selected_Item.ItemIdx);
            popup.Set_addEventYESButton(() => callback_Try_RfEquipItem(popup.reinforceType));


        }
    }


    void callback_Try_RfEquipItem(REINFORCE_TYPE rfType)
    {
        User _user = UserDataManager.instance.user;
        UI_Popup_RfProcess popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_RfProcess>(UIPOPUP.POPUPRFPROCESS);
        popup.Set_addEventYESButton(callback_complete_ReinforceItem);

        if (_user.User_Weapons[Selected_Item.ItemIdx].RefFailCnt >= 1)
            popup.Set_RfProcess(rfType, false, nowSelectUnitIdx, Selected_Item.ItemIdx);
        else
            popup.Set_RfProcess(rfType, true, nowSelectUnitIdx, Selected_Item.ItemIdx);

    }

    void callback_complete_ReinforceItem()
    {
        user = UserDataManager.instance.user;

        Process_EquipItemInfo(Selected_Item);

        //top 갱신
        if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
            UI_Top.Getsingleton.set_refresh();

        //다시 강화 창 띄우기 
        ResponseButton_LevelUp();
    }





    public void ResponseButton_Equipment()
    {
        UIData udata = new UIData(new List<object> { nowSelectUnitIdx });
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.EQUIPMENT, _canvasTr, udata);

    }




    public void ResponseButton_Back()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
    }
}
