using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using Assets.SimpleAndroidNotifications;
public class UI_LobbyOld : UI_Base
{
    public enum LobbyCharTouchType
    {
        NONE = 99,
        Began = 0,
        Stay = 1,
        End = 2,
    }

    public enum EumLobbyObj
    {
        Event = 0,
        HelperFirstSelect = 1,
        InviteMsg = 2,
        BeginnerGameBtn = 3,
        EventBanner = 4,

    }

    //멤버변수
    public Dictionary<uint, infos_unit> Dic_infosUnit = new Dictionary<uint, infos_unit>();
    public Dictionary<uint, User_Units> Dic_UserUnit = new Dictionary<uint, User_Units>();

    private bool isMode = false;
    public bool isSelectMode // 신규유저 캐릭터 선택모드이냐?
    {
        set
        {
            isMode = value;
            LbChrctPositioning.IsRandPosioning = isMode;
        }
        get { return isMode; }
    }


    //상단 UI
    public Image image_clanMark;
    public Image image_Flag;
    public Text text_UserName;
    public Text text_UserID;

    //캐릭터 정보 UI
    //공통
    public List<Text> Lst_textUnitName;
    public List<Image> Lst_imageMainSkill;
    public List<Text> Lst_textHp;
    public List<Text> Lst_textmoveSpeed;
    public List<Text> Lst_textReloadSpeed;
    public List<Text> Lst_textSubSkill;

    //미소유
    public Text text_Descrip;
    public Text text_buyPrice;
    public Image image_buyType;
    //소유
    public Image image_UnitRank;
    public Image image_UnitWeapon;
    public Text text_UnitKD;
    public Text text_UnitExp;
    public Text text_UnitLv;
    public Text text_LvInfo;
    public Slider slider_unitExp;

    //캐릭터 회전 및 선택 관련
    public List<characterInfo> Lst_LobbyCharacters;
    public Dictionary<uint, characterInfo> Dic_lobbyCharacter = new Dictionary<uint, characterInfo>(); // 유닛인덱스에 따른 characterinfo 정보 컬렉션
    public Transform CharRotate_Tr;
    public LobbyCharacterPositioning LbChrctPositioning;                    //로비캐릭터 위치지정 스크립트
    public bool isSelectedCharacter = false;
    public bool isChangedLobbyChrtCount = false;                            // 로비 활성될캐릭터 수가 변햇다.

    //공통 오브젝트
    public List<GameObject> Lst_publicLobbyObj;

    //이벤트 정보
    public List<GameObject> Lst_EventObj;


    //캐릭터 정보관련
    public List<GameObject> Lst_ObjCharacterInfo;

    //출첵관련
    public Text text_AtdLeftTime;
    private Coroutine AttdRoutine;


    //핑관련
    public Text text_ping;

    //초대메시지 관련
    private uint inviterUserID = 0;
    public Text text_Sender;

    //버튼 마크 관련
    public List<GameObject> Lst_Mark;


    //서브스킬 체인지 관련
    private bool isOpeningSubskillPopup = false;

    private static UI_LobbyOld _instance;
    public static UI_LobbyOld Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UI_LobbyOld)) as UI_LobbyOld;

                if (_instance == null)
                {
                    GameObject instanceObj = new GameObject("UI_LobbyOld");
                    _instance = instanceObj.AddComponent<UI_LobbyOld>();
                }
            }

            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;

    }


    public override void set_Open()
    {
        base.set_Open();


        Set_LobbyInfo();
    }

    public override void set_Close()
    {
        base.set_Close();


    }



    public override void set_refresh()
    {
        base.set_refresh();

        //Set_LobbyInfo();

        RefreshDataInfo();

    }


    void CloseProcess()
    {
        //ui나갈떄 출첵표시 코루틴 정지
        if (AttdRoutine != null)
            StopCoroutine(AttdRoutine);
    }


    //프로토콜완료후, 특정행위완료후 등등 데이터가 변경되면서 UI의정보를 다시적용해야될떄 쓰이는 함수
    public void RefreshDataInfo()
    {

        //기본적 필요 데이터정보 저장
        Chk_basicalInfo();

        //상단유저정보 설정
        Set_TopLobbyUserInfo();

        // 변화됫는지 (캐릭터 갯수,및 정보) 로비캐릭터 체크
        Chk_ApplyChangedLobbyChrct();

        //캐릭터모델 착용아이템 갱신
        Set_HaveLobbyCharacterInfo();

        //특정 캐릭터 나오게 회전시켜준다
        LbChrctPositioning.SetRotate_TargetCharacter(UserDataManager.instance.user.User_useUnit.UnitIdx);

        //기타 UI 설정
        Set_OtherInfo();

        //캐릭터 정보 UI 갱신
        //foreach (var Lobbychar in Dic_lobbyCharacter)
        {
            Apply_LobbyInfoUsingSelectedChar(Dic_lobbyCharacter[UserDataManager.instance.user.User_useUnit.UnitIdx]);
        }

    }

    void Set_LobbyInfo()
    {

        //첫번쨰 캐릭선택 햇는지 체크
        //if (UserDataManager.instance.user.User_Units.Count <= 0)
        //	isSelectMode = true;

        //기본적 필요 데이터정보 저장
        Chk_basicalInfo();

        //상단유저정보 설정
        Set_TopLobbyUserInfo();


        //로비 캐릭터 정보 설정
        Set_LobbyCharacterInfo();

        //최초 캐릭터 선택햇는지 체크
        Chk_FirstSelectCharacter();

        //기타 UI 설정
        Set_OtherInfo();




    }

    void Chk_basicalInfo()
    {

        //유닛정보 할당

        Dic_UserUnit = UserDataManager.instance.user.User_Units;

        //유닛구성정보 체크
        Dictionary<uint, Infos_EventItemTime> dic_eventUnit = TableDataManager.instance.Infos_EventItemTimes;
        Dictionary<uint, infos_unit> _dicInfosUnit = TableDataManager.instance.Infos_units;
        Dictionary<uint, infos_unit> _DicInfosUnitTemp = new Dictionary<uint, infos_unit>();

        foreach (var infoUnit in _dicInfosUnit)
        {
            if (dic_eventUnit.ContainsKey(infoUnit.Value.UnitIdx))
            {
                if (TimeManager.Instance.Get_nowTime() > dic_eventUnit[infoUnit.Value.UnitIdx].BuyEndTm)
                {
                    _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
                }
                else
                {
                    if (Dic_UserUnit.ContainsKey(infoUnit.Value.UnitIdx))
                        _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
                }
            }
            else
                _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
        }

        //구성된 유닛 이 달라졋는지 체크
        if (Dic_infosUnit.Count > 0 && Dic_infosUnit.Count != _DicInfosUnitTemp.Count)
            isChangedLobbyChrtCount = true;
        else
            isChangedLobbyChrtCount = false;


        //유닛구성정보 할당
        Dic_infosUnit.Clear();
        Dic_infosUnit = _DicInfosUnitTemp;


    }


    //상단유저정보 설정
    void Set_TopLobbyUserInfo()
    {
        User _user = UserDataManager.instance.user;

        //클랜마크
        if (_user.user_Clans.ClanID != 0)
            image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, _user.clan_Clans.ClanMark));
        else
            image_clanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
        //국가마크
        image_Flag.sprite = ImageManager.instance.Get_FlagSprite(_user.user_Users.CtrCd);
        //이름
        text_UserName.text = _user.user_Users.NkNm;
        //아이디
        text_UserID.text = _user.user_Users.UserID.ToString();


        //버프
        //클랜골드버프
        if (_user.clan_Clans.GoldBufTm > TimeManager.Instance.Get_nowTime())
            Lst_EventObj[(int)BUFF_TYPE.CLANGOLD - 1].SetActive(true);
        else
            Lst_EventObj[(int)BUFF_TYPE.CLANGOLD - 1].SetActive(false);

        //클랜exp버프
        if (_user.clan_Clans.ExpBufTm > TimeManager.Instance.Get_nowTime())
            Lst_EventObj[(int)BUFF_TYPE.CLANEXP].SetActive(true);
        else
            Lst_EventObj[(int)BUFF_TYPE.CLANEXP].SetActive(false);


        //이벤트exp버프
        bool expEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.EXP);
        Lst_EventObj[(int)BUFF_TYPE.EVENTEXP + 1].SetActive(expEvent);

        //이벤트 gold버프
        bool goldEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.GOLD);
        Lst_EventObj[(int)BUFF_TYPE.EVENTGOLD].SetActive(goldEvent);




    }





    //로비 캐릭터 정보 설정
    public void Set_LobbyCharacterInfo()
    {


        //캐릭터 오브젝트 가져 오기
        characterInfo[] chars = CharRotate_Tr.GetComponentsInChildren<characterInfo>();
        Lst_LobbyCharacters.Clear();
        for (int i = 0; i < chars.Length; i++)
        {
            Lst_LobbyCharacters.Add(chars[i]);
        }

        //// 구성정보 할당
        //foreach (var unit in Dic_infosUnit)
        //{
        //    for (int i = 0; i < Lst_LobbyCharacters.Count; i++)
        //    {
        //        if (unit.Value.UnitIdx == Lst_LobbyCharacters[i].unitIdx) //캐릭터 구성 정보와 세워진 로비캐릭터 idx 가맡으면 값할당
        //        {
        //            Lst_LobbyCharacters[i].Get_Items();
        //            Lst_LobbyCharacters[i].unitInfos = unit.Value;
        //            Dic_lobbyCharacter[unit.Value.UnitIdx] = Lst_LobbyCharacters[i];

        //            //포지션닝하기위해 list에 추가
        //            LbChrctPositioning.AddList_Character(Lst_LobbyCharacters[i]);
        //        }

        //    }
        //}

        Dictionary<uint, infos_unit> dic_allinfoUnit = TableDataManager.instance.Infos_units;



        for (int i = 0; i < Lst_LobbyCharacters.Count; i++)
        {

            //characterInfo 에 기본적인 정보 할당
            Lst_LobbyCharacters[i].Init_chrctBasicInfo();

            if (dic_allinfoUnit.ContainsKey((uint)Lst_LobbyCharacters[i].unitIdx))
                Lst_LobbyCharacters[i].unitInfos = dic_allinfoUnit[(uint)Lst_LobbyCharacters[i].unitIdx];

            Dic_lobbyCharacter[(uint)Lst_LobbyCharacters[i].unitIdx] = Lst_LobbyCharacters[i];

            //캐릭터 구성 정보와 세워진 로비캐릭터 idx 가맡으면 
            if (Dic_infosUnit.ContainsKey((uint)Lst_LobbyCharacters[i].unitIdx))
            {
                //포지션닝하기위해 list에 추가
                LbChrctPositioning.AddList_Character(Lst_LobbyCharacters[i]);
            }
            else  //캐릭터 구성 정보와 세워진 로비캐릭터 idx 가 맞지않으면 비활성
            {
                Lst_LobbyCharacters[i].gameObject.SetActive(false);
            }

        }



        //구입한 유닛 정보 설정
        Set_HaveLobbyCharacterInfo();

        //위치 잡아주기
        LbChrctPositioning.makePosition();



        //특정 캐릭터 나오게 회전시켜준다
        LbChrctPositioning.SetRotate_TargetCharacter(UserDataManager.instance.user.User_useUnit.UnitIdx);

    }


    void Set_HaveLobbyCharacterInfo()
    {

        foreach (var InfoUnit in Dic_infosUnit)
        {

            if (Dic_UserUnit.ContainsKey(InfoUnit.Value.UnitIdx))
            {
                if (Dic_lobbyCharacter.ContainsKey(InfoUnit.Value.UnitIdx))
                {
                    Dic_lobbyCharacter[InfoUnit.Value.UnitIdx].userUnit = Dic_UserUnit[InfoUnit.Value.UnitIdx];
                    Dic_lobbyCharacter[InfoUnit.Value.UnitIdx].isHave = true;
                }
            }
            else
            {
                if (Dic_lobbyCharacter.ContainsKey(InfoUnit.Value.UnitIdx))
                {
                    Dic_lobbyCharacter[InfoUnit.Value.UnitIdx].isHave = false;
                }
            }

        }

    }




    // 변화됫는지 (캐릭터 갯수,및 정보) 로비캐릭터 체크
    void Chk_ApplyChangedLobbyChrct()
    {
        //갯수 변화 됫으니 위치 다시 캐릭터 활성/비활성하자
        if (isChangedLobbyChrtCount)
        {

            //포지션잡기 관련 데이터 클리어
            LbChrctPositioning.Clear_makedPosition();

            //캐릭터 활성/비활성 
            for (int i = 0; i < Lst_LobbyCharacters.Count; i++)
            {
                //캐릭터 구성 정보와 세워진 로비캐릭터 idx 가맞으면 
                if (Dic_infosUnit.ContainsKey((uint)Lst_LobbyCharacters[i].unitIdx))
                {
                    Lst_LobbyCharacters[i].gameObject.SetActive(true);
                    //포지션닝하기위해 list에 추가
                    LbChrctPositioning.AddList_Character(Lst_LobbyCharacters[i]);
                }
                else  //캐릭터 구성 정보와 세워진 로비캐릭터 idx 가 맞지않으면 비활성
                {
                    Lst_LobbyCharacters[i].gameObject.SetActive(false);
                }
            }


            //위치 다시잡기
            LbChrctPositioning.makePosition();


            //특정 캐릭터 나오게 회전시켜준다
            LbChrctPositioning.SetRotate_TargetCharacter(UserDataManager.instance.user.User_useUnit.UnitIdx);
        }
    }





    //최초 캐릭터 선택햇는지 체크
    public void Chk_FirstSelectCharacter()
    {
        //처음 캐릭터 선택이면 아래 로직진행
        //if (isSelectMode)
        //{
        //
        //	//초기 유닛 구매를위해 unitidx 랜덤 부여
        //	UserDataManager.instance.user.User_useUnit.UnitIdx = LbChrctPositioning.firstSelectUnitIdx;
        //	ResponseButton_BuyCharacter();
        //	
        //}
        //else
        {

            // 로그인시 각종 팝업들 띄움,표시 하기
            Show_PopupsWhenLogin();

        }
        // 선택모드 작동
        //Active_FirstSelectMode(isSelectMode);

    }




    // 선택모드 작동/미작동
    void Active_FirstSelectMode(bool isActive)
    {
        //헬퍼 작동
        Activate_publicLobbyObj(EumLobbyObj.HelperFirstSelect, isActive);
        Activate_publicLobbyObj(EumLobbyObj.Event, !isActive);
        //Activate_publicLobbyObj(LobbyObj.InviteMsg, !isActive);

        //topui 잠시 끄자
        if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
            UI_Top.Getsingleton.gameObject.SetActive(!isActive);
    }



    // 로그인시 ㅔ 각종 팝업들 띄움,표시 하는 함수
    void Show_PopupsWhenLogin()
    {
        //일일출첵관련 
        Chk_AttendDay();

        //알림판 띄우기
        Chk_Announcement();
    }


    //알림판 띄우기 
    void Chk_Announcement()
    {
        //TabjoyTool.instance.Show_TjContent("notice");
    }



    //출첵보상요청
    void Chk_AttendDay()
    {
        UserDataManager.instance.user.IsChkAttd = true;

        //로비캐릭회전 잠금
        User.isSelectedCharacter = true;

        webRequest.GetAttendDayRwd(ShowPopup_AttendDay);

        Chk_AttendDayPushNoti();
    }




    //일일출첵관련 팝업
    void ShowPopup_AttendDay()
    {
        // 서브스킬선택 팝업창이 뜨지 않으면 로비캐릭회전 잠금해제
        if (!isOpeningSubskillPopup)
            User.isSelectedCharacter = false;

        User _user = UserDataManager.instance.user;
        GainItem AttendGainItem = null;
        if (webResponse.GetResultGainItem(ITEMTYPE.ATDDAY, ref AttendGainItem))
        {
            DateTime nowTime = TimeManager.Instance.Get_nowTime();
            DateTime _mtime = AttendGainItem.mtime;
            _mtime.AddDays(1);

            if (_mtime.Year <= nowTime.Year)
            {
                if (_mtime.Month <= nowTime.Month)
                {
                    if (_mtime.Day <= nowTime.Day)
                    {
                        // 출첵팝업
                        UI_Popup_Attendace popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Attendace>(UIPOPUP.POPUPATTENDANCE);
                        popup.Set_RwdInfo(AttendGainItem.num, UI_Popup_Attendace.AttendRwd_Type.AttdToday);


                    }
                }
            }
        }

        //소식관련 마크 처리
        if (_user.MarkChanges.ContainsKey(MarkTitleType.NewsCount))
        {
            Lst_Mark[(int)MarkTitleType.NewsCount].SetActive(_user.MarkChanges[MarkTitleType.NewsCount]);
        }

    }



    void Chk_AttendDayPushNoti()
    {
        //#if UNITY_ANDROID
        //		User _user = UserDataManager.instance.user;

        //		int AttdPushCount = 4;
        //		for (int i = 0; i < AttdPushCount; i++)
        //		{
        //			// 이전 예약된푸쉬 삭제
        //			int id = DefineKey.PUSH_ID_ATTDAY + i;
        //			NotificationManager.Cancel(id);
        //		}


        //		//푸쉬 예약하자
        //		for (int i = 0; i < AttdPushCount; i++)
        //		{
        //			DateTime nowTime = TimeManager.Instance.Get_nowTime();

        //			int nextHour = ((i + 1) * 24) + (12 - nowTime.Hour); // 다음날 오후12시쯤에 예약
        //			int id = DefineKey.PUSH_ID_ATTDAY + i;
        //			Debug.Log("nextHour : " + nextHour + " id : " + id);

        //			NotificationManager.SendPush(TimeSpan.FromHours(nextHour), 
        //				TextDataManager.Dic_TranslateText[287], //출석보상
        //				TextDataManager.Dic_TranslateText[506],  //접속하셔서 출석체크 보상받으세요 !
        //				id);
        //		}

        //#endif

    }









    //출첵보상시간표시
    void Set_AttendDayLeftTime()
    {

        AttdRoutine = StartCoroutine(routine_AttendTime(Chk_AttendDay));
    }



    IEnumerator routine_AttendTime(del_NextProcess nextProcess)
    {
        DateTime nowTime = TimeManager.Instance.Get_nowTime();
        TimeSpan nowSpan = new TimeSpan(0, nowTime.Hour, nowTime.Minute, nowTime.Second);
        TimeSpan DailySpan = new TimeSpan(24, 0, 0);
        double totalTime = (DailySpan - nowSpan).TotalSeconds;

        int hour;
        int min;
        int sec;

        while (true)
        {
            if (totalTime <= 0)
            {
                if (nextProcess != null)
                    nextProcess();
                break;
            }
            hour = (int)totalTime / 3600;
            min = (int)((totalTime % 3600) / 60);
            sec = (int)totalTime % 60;

            text_AtdLeftTime.text = string.Format("{0}:{1}:{2}", hour.ToString("D2"), min.ToString("D2"), sec.ToString("D2"));

            totalTime -= Time.deltaTime;

            yield return null;
        }
    }






    //선택한 캐릭터의한 로비정보 적용
    public void Apply_LobbyInfoUsingSelectedChar(characterInfo selectChar)
    {
        //캐릭터 선택 ...캐릭인덱스저장
        User _user = UserDataManager.instance.user;
        _user.User_useUnit.UnitIdx = selectChar.unitInfos.UnitIdx;

        if (selectChar.isHave)
        {
            // 소유캐릭터정보 오브젝트 활성화
            Active_unlockInfo(true);

            //계급으로 초보자버튼 활성/비활성
            if (selectChar.userUnit.UnitRk < 3) //상병 미만이면 활성
                Activate_publicLobbyObj(EumLobbyObj.BeginnerGameBtn, true);
            else
                Activate_publicLobbyObj(EumLobbyObj.BeginnerGameBtn, false);

            // 선택된 소유캐릭터 정보 넣기

            Dictionary<byte, Infos_UnitRank> _dicUnitRank = TableDataManager.instance.Infos_unitRanks;

            //계급
            image_UnitRank.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.unitrank, selectChar.userUnit.UnitRk));

            //현재무기
            image_UnitWeapon.sprite = ImageManager.instance.Get_Sprite(_user.User_Units[_user.User_useUnit.UnitIdx].MainWpnIdx.ToString());

            //경험치
            byte curRk = (byte)selectChar.userUnit.UnitRk;
            byte nextRk = (byte)(selectChar.userUnit.UnitRk + 1);
            if (_dicUnitRank.ContainsKey(nextRk))
            {
                float nowExp = 0;
                float nextNeedExp = 0;

                //지금획득경험치
                if (curRk == 1)
                    nowExp = (float)selectChar.userUnit.UnitExp;
                else
                    nowExp = (float)(selectChar.userUnit.UnitExp - _dicUnitRank[curRk].NeedExp);

                //다음필요경험치
                nextNeedExp = _dicUnitRank[nextRk].NeedExp - _dicUnitRank[curRk].NeedExp;

                text_UnitExp.text = string.Format("{0}/{1}", selectChar.userUnit.UnitExp, _dicUnitRank[nextRk].NeedExp);
                slider_unitExp.maxValue = nextNeedExp;
                slider_unitExp.value = nowExp;

            }

            //레벨
            text_UnitLv.text = string.Format("LV.{0}", selectChar.userUnit.RefLv);
            if (selectChar.userUnit.RefLv < 15)
            {
                text_LvInfo.text = "UPGRADE";
            }
            else
            {
                text_LvInfo.text = "MAX";
            }


            //킬데스
            if (_user.User_unit_useInfos.ContainsKey(_user.User_useUnit.UnitIdx))
            {
                uint death = _user.User_unit_useInfos[_user.User_useUnit.UnitIdx].DeathCnt;
                if (death == 0) death = 1;
                float kdRate = (float)(_user.User_unit_useInfos[_user.User_useUnit.UnitIdx].KillCnt) / (float)(death);
                text_UnitKD.text = string.Format("{0}", kdRate.ToString("N3"));
            }
            else
            {
                text_UnitKD.text = string.Format("0.000");
            }

            //능력치
            float initHp = selectChar.unitInfos.Hp;
            float initMv = selectChar.unitInfos.MvSpeed;
            int reflv = selectChar.userUnit.RefLv;
            Lst_textUnitName[0].text = selectChar.unitInfos.UnitName;

            Lst_imageMainSkill[0].sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.lobbySkill, (int)selectChar.unitInfos.SkillKind));

            Lst_textHp[0].text = ((int)StaticMethod.Get_nextSpec(initHp, reflv, true)).ToString();

            if (reflv > 4) //강화렙 5이상 시
                Lst_textmoveSpeed[0].text = (StaticMethod.Get_nextSpec(initMv, reflv, false, 0.05f, 4)).ToString();
            else
                Lst_textmoveSpeed[0].text = selectChar.unitInfos.MvSpeed.ToString();

            if (reflv > 9) //강화렙 10 이상시
                Lst_textReloadSpeed[0].text = "+" + Mathf.Abs((int)StaticMethod.Get_nextSpec(0f, reflv, true, 5f, 9)).ToString();
            else
                Lst_textReloadSpeed[0].text = "+" + selectChar.unitInfos.RldSpeed.ToString();


            if (selectChar.userUnit.SubSkill != 0)
                Lst_textSubSkill[0].text = TableDataManager.instance.Infos_Skills[selectChar.userUnit.SubSkill].SkillName;
            else
                Lst_textSubSkill[0].text = "-----";

        }
        else
        {
            // 비소유캐릭터정보 오브젝트 비활성화
            Active_unlockInfo(false);



            //선택된 미소유캐릭터 정보 넣기

            //설명
            text_Descrip.text = TextDataManager.Dic_TranslateText[(int)selectChar.unitInfos.UnitIdx];

            //가격
            if (isSelectMode)
                text_buyPrice.text = "0";
            else
                text_buyPrice.text = selectChar.unitInfos.SellItNum.ToString();

            //구매이미지
            if (selectChar.unitInfos.SellItTp == ITEMTYPE.GEM)
                image_buyType.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
            else if (selectChar.unitInfos.SellItTp == ITEMTYPE.GOLD)
                image_buyType.sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);

            //비소유시 능력치
            Lst_textUnitName[1].text = selectChar.unitInfos.UnitName;
            //스킬이미지
            Lst_imageMainSkill[1].sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.lobbySkill, (int)selectChar.unitInfos.SkillKind));
            Lst_textHp[1].text = selectChar.unitInfos.Hp.ToString();
            Lst_textmoveSpeed[1].text = selectChar.unitInfos.MvSpeed.ToString();
            Lst_textReloadSpeed[1].text = "+" + selectChar.unitInfos.RldSpeed.ToString();
            Lst_textSubSkill[1].text = "-----";

        }





        //for (int i = 0; i < Lst_LobbyCharacters.Count; i++)
        //{
        //    if (Lst_LobbyCharacters[i] != selectChar)
        //        Lst_LobbyCharacters[i].Set_lockObj(false);
        //}





    }


    public void Set_Ping(int ping)
    {
        //핑정보
        text_ping.text = string.Format("{0}ms", ping);
    }


    //기타 UI 설정
    public void Set_OtherInfo()
    {
        //출첵보상시간표시
        Set_AttendDayLeftTime();

        //초대메세지 설정 (초대메세지 있으면 팝업보이기)
        Set_InviteMessage();

        //리뷰작성 부탁 팝업
        Chk_review();

        //알림 표시 설정 관련
        Set_newsMark();

        //광고관련 체크
        Chk_Ads();

        //오류등으로 이전에 서브스킬 못바꾼거 바꾸기 체크
        StartCoroutine(Chk_changedSubSkill());
    }



    //리뷰작성 부탁 
    void Chk_review()
    {
        if (!PlayerPrefs.HasKey(DefineKey.ReviewLater))
        {
            if (UserDataManager.instance.user.Get_user_goods(ITEMTYPE.GAMEPLAY) >= 3) //3판이상이냐
            {
                UI_Popup_Review popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Review>(UIPOPUP.POPUPREVIEW);
                popup.Set_addEventYESButton(callback_review);

                PlayerPrefs.SetInt(DefineKey.ReviewLater, 1);
                PlayerPrefs.Save();
            }
        }
    }

    void callback_review()
    {
        string marketUrl = string.Empty;

#if UNITY_EDITOR
        marketUrl = "market://details?id=com.cle.dy.Suddenground";
#elif UNITY_ANDROID
		marketUrl = "market://details?id=com.cle.dy.Suddenground";
#elif UNITY_IOS
		marketUrl = "https://itunes.apple.com/app/id1395707286";
#endif
        Application.OpenURL(marketUrl);
    }



    //알림 표시 설정 관련
    public void Set_newsMark()
    {
        User _User = UserDataManager.instance.user;

        //마크 표시 혹은 미표시 이면 표시 또는 미표시 해주자
        foreach (var mark in _User.MarkChanges)
        {
            Lst_Mark[(int)mark.Key].SetActive(mark.Value);
        }
    }


    void Chk_Ads()
    {

        // 판마다 광고보기 체크
        Chk_GameEndShowAds();
    }


    void Chk_GameEndShowAds()
    {
        User _user = UserDataManager.instance.user;

        //광고표시
        if (_user.Get_user_goods(ITEMTYPE.AD_REMOVE) > 0)
        {

            UserEditor.Getsingleton.EditLog("광고 제거 햇다");
        }
        else
        {
            //겜완료 해서 광고봐야하냐
            //if (AdsManager.instance.IsShowAdsGameEnd)
            //{
            //	UserEditor.Getsingleton.EditLog("판수 : " + _user.Get_user_goods(ITEMTYPE.GAMEPLAY));
            //	if (_user.Get_user_goods(ITEMTYPE.GAMEPLAY) % 
            //		TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_AfterGameShowAd].ConsVal == 0) // 2판마다 광고보기
            //	{
            //		UserEditor.Getsingleton.EditLog("광고 제거 안햇다, 2판마다 광고보기");

            //		AdsManager.instance.Show_interstitalAds();
            //		AdsManager.instance.nextAdsProcess = callback_complete_watchAds;
            //	}
            //}
        }


    }

    void callback_complete_watchAds()
    {
        //AdsManager.instance.IsShowAdsGameEnd = false;
    }






    bool isProcessingSelectingSubskill = false;
    //오류등으로 이전에 서브스킬 못바꾼거 바꾸기 체크
    IEnumerator Chk_changedSubSkill()
    {
        User _user = UserDataManager.instance.user;
        uint needChangedSubskillUnitidx = 0;

        while (true)
        {

            if (_user.User_DrawSkills.Count <= 0)
                break;



            foreach (var Ds in _user.User_DrawSkills)
            {
                if (Ds.Value.SubSkill != 0)
                {
                    if (isProcessingSelectingSubskill == false)
                    {

                        isProcessingSelectingSubskill = true;

                        needChangedSubskillUnitidx = Ds.Value.UnitIdx;
                        PopupSelectingSubSkill(Ds.Value.UnitIdx);
                        yield return null;
                    }
                }
            }

            if (isProcessingSelectingSubskill == false)
                break;

            yield return null;

        }
    }

    //스킬 바꾸기 알림팝업 => 스킬바꿀꺼냐 ?
    void PopupSelectingSubSkill(uint UnitIdx)
    {
        User _user = UserDataManager.instance.user;

        //서브스킬 체인지으로 팝업오픈중
        isOpeningSubskillPopup = true;

        string bfSkill = TableDataManager.instance.Infos_Skills[_user.User_Units[UnitIdx].SubSkill].SkillName;
        byte subSkill = _user.User_DrawSkills[UnitIdx].SubSkill;
        string NewSkill = TableDataManager.instance.Infos_Skills[subSkill].SkillName;

        UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
        popup.Set_addEventButton(() => callback_DoChange_Subskill(UnitIdx));
        popup.Set_addNoEventButton(() => callback_DontChange_Subskill(UnitIdx));
        popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);//알림
        popup.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[146], bfSkill, NewSkill));//서브스킬을 변경 하시겠습니까?
    }

    void callback_DoChange_Subskill(uint UnitIdx)
    {
        webRequest.BuySkillCommit(UnitIdx,
            UserDataManager.instance.user.User_DrawSkills[UnitIdx].SubSkill,
            true,
            () => Callback_Complete_PopupSelectingSubskill(UnitIdx));

        //서브스킬 체인지팝업 닫혓다
        isOpeningSubskillPopup = false;
    }

    void callback_DontChange_Subskill(uint UnitIdx)
    {
        webRequest.BuySkillCommit(UnitIdx,
            UserDataManager.instance.user.User_DrawSkills[UnitIdx].SubSkill,
            false,
            () => Callback_Complete_PopupSelectingSubskill(UnitIdx));

        //서브스킬 체인지팝업 닫혓다
        isOpeningSubskillPopup = false;
    }


    void Callback_Complete_PopupSelectingSubskill(uint UnitIdx)
    {
        isProcessingSelectingSubskill = false;


        //갱신
        Dic_lobbyCharacter[UnitIdx].userUnit = UserDataManager.instance.user.User_Units[UnitIdx];
        Apply_LobbyInfoUsingSelectedChar(Dic_lobbyCharacter[UnitIdx]);
    }









    // 소유캐릭터정보 오브젝트 활성화 대한 설정
    void Active_unlockInfo(bool isActive)
    {

        Lst_ObjCharacterInfo[0].SetActive(isActive);
        Lst_ObjCharacterInfo[1].SetActive(!isActive);
    }

    //소유캐릭정보UI 재 활성화
    public void On_reActiveUnlockInfo()
    {
        //에니메이션 발동을위해 오브젝트가 켜져있다가 꺼주고 다시 켜자
        if (Lst_ObjCharacterInfo[0].activeSelf == true)
        {
            Lst_ObjCharacterInfo[0].SetActive(false);
            Lst_ObjCharacterInfo[0].SetActive(true);
        }

        if (Lst_ObjCharacterInfo[1].activeSelf == true)
        {
            Lst_ObjCharacterInfo[1].SetActive(false);
            Lst_ObjCharacterInfo[1].SetActive(true);
        }
    }


    void Activate_publicLobbyObj(EumLobbyObj obj, bool isActive)
    {
        for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
            if (i == (int)obj)
                Lst_publicLobbyObj[i].SetActive(isActive);
    }

    bool isActive_publicLobbyObj(EumLobbyObj obj)
    {
        bool isactive = false;
        for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
            if (i == (int)obj)
                if (Lst_publicLobbyObj[i].activeSelf)
                    isactive = Lst_publicLobbyObj[i].activeSelf;

        return isactive;
    }

    void Activate_EventObj(int idx, bool isActive)
    {
        for (int i = 0; i < Lst_publicLobbyObj.Count; i++)
            if (i == idx)
                Lst_publicLobbyObj[i].SetActive(isActive);
    }



    //데이터 클리어
    public void ClearAllData()
    {
        //캐릭터관련 클리어
        Dic_lobbyCharacter.Clear();
        Lst_LobbyCharacters.Clear();
    }





    //캐릭터 레벨업(강화)
    public void ResponseButton_CharacterLvUp()
    {
        User _user = UserDataManager.instance.user;

        //로비캐릭회전 잠금
        User.isSelectedCharacter = true;

        UI_Popup_Reinforce popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Reinforce>(UIPOPUP.POPUPREINFORCE);
        popup.Set_ReinforceInfo(REINFORCE_TYPE.UNIT, _user.User_useUnit.UnitIdx, _user.User_useUnit.UnitIdx);
        popup.Set_addEventYESButton(() => callback_Try_Reinforce(popup.reinforceType));

    }



    void callback_Try_Reinforce(REINFORCE_TYPE rfType)
    {
        User _user = UserDataManager.instance.user;
        uint _unitIdx = _user.User_useUnit.UnitIdx;
        UI_Popup_RfProcess popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_RfProcess>(UIPOPUP.POPUPRFPROCESS);
        popup.Set_addEventYESButton(() => callback_complete_Rfprocess(_user.User_useUnit.UnitIdx));

        //실패 수가 1이상이면 실패!
        if (_user.User_Units[_user.User_useUnit.UnitIdx].RefFailCnt >= 1)
            popup.Set_RfProcess(rfType, false, _unitIdx, _unitIdx);
        else // 아니면 강화성공
            popup.Set_RfProcess(rfType, true, _unitIdx, _unitIdx);




    }




    //유닛 강화 완료햇으니 정보 갱신 
    void callback_complete_Rfprocess(uint _unitIdx)
    {

        //if (UserDataManager.instance.user.User_Units.ContainsKey(_unitIdx))
        //{
        //    Dic_lobbyCharacter[_unitIdx].userUnit = UserDataManager.instance.user.User_Units[_unitIdx];
        //    Apply_LobbyInfoUsingSelectedChar(Dic_lobbyCharacter[_unitIdx]);
        //}

        //top 재화갱신
        UI_Top.Getsingleton.set_refresh();

        //다시 강화하기 팝업띄우기
        ResponseButton_CharacterLvUp();

    }



    //캐릭터 구매
    public void ResponseButton_BuyCharacter()
    {
        uint useUnitIdx = UserDataManager.instance.user.User_useUnit.UnitIdx;
        //최초 유닛구입
        if (isSelectMode)
        {

            webRequest.SetChoiceUnit(useUnitIdx, () => callback_complete_buyCharacter(useUnitIdx));

        }
        else // 추후 유닛구입
        {
            Dictionary<uint, infos_unit> _dic_infosUnit = TableDataManager.instance.Infos_units;


            webRequest.ShopBuyUnit(useUnitIdx, _dic_infosUnit[useUnitIdx].SellItNum, () => callback_complete_buyCharacter(useUnitIdx));
        }
    }

    //캐릭구매 완료 콜백 , 최초유닛구매 완료 콜백
    public void callback_complete_buyCharacter(uint _unitIdx)
    {
        // 최초유닛구매 완료 시 모드 off 
        if (isSelectMode)
        {
            isSelectMode = false;
            Chk_FirstSelectCharacter();
            RefreshDataInfo();
            //Chk_FirstSelectCharacter();
        }

        if (UserDataManager.instance.user.User_Units.ContainsKey(_unitIdx))
        {
            Set_HaveLobbyCharacterInfo();
            //Dic_lobbyCharacter[_unitIdx].userUnit = UserDataManager.instance.user.User_Units[_unitIdx];
            Apply_LobbyInfoUsingSelectedChar(Dic_lobbyCharacter[_unitIdx]);

            //구매 캐릭터이펙트 발동
            Dic_lobbyCharacter[_unitIdx].playBuyEffect(0);

            //유저 보유 유닛갯수 통계내리기
            Analyze_UserHaveUnitCount();
        }


        //top ui 재화 갱신
        if (UI_Manager.Getsingleton.Dic_UILst[UI.TOP])
            UI_Top.Getsingleton.set_refresh();
    }


    //유저 보유 유닛갯수 통계내리기
    void Analyze_UserHaveUnitCount()
    {
        //if (UserDataManager.instance.user.User_Units.Count > 1)
        //{
        //	UserEditor.Getsingleton.EditLog("유닛갯수 통계내림=> " + DefineKey.HaveUnitCount + " : " + UserDataManager.instance.user.User_Units.Count);
        //	AnalysisManager.instance.Anl_CustomEvt(DefineKey.User, DefineKey.HaveUnitCount,
        //		string.Format("{0}{1}", UserDataManager.instance.user.User_Units.Count, DefineKey.LessThan), null, 1);
        //}
    }



    public void ResponseButton_Attend()
    {
        // 출첵팝업
        UI_Popup_Attendace popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Attendace>(UIPOPUP.POPUPATTENDANCE);
        popup.Set_RwdInfo(UserDataManager.instance.user.Get_user_goods(ITEMTYPE.ATDDAY));

    }



    //게임 시작
    public void ResponseButton_StartGame(int battleIdx)
    {

        if (battleIdx == 0) //일반 퀵조인
        {
            //로비캐릭 회전 잠금
            User.isSelectedCharacter = true;

           // webRequest.SetUseUnitIdx(UserDataManager.instance.user.User_useUnit.UnitIdx, callback_complete_StartGamePopup);
        }
        else if (battleIdx == 1) //초보자 퀵조인
        {
            //로비캐릭 회전 잠금
            User.isSelectedCharacter = true;

           // webRequest.SetUseUnitIdx(UserDataManager.instance.user.User_useUnit.UnitIdx, callback_complete_StartGameBeginner);
        }
    }

    void callback_complete_StartGamePopup()
    {
        //로비캐릭 회전 잠금해제
        User.isSelectedCharacter = false;

        UI_Popup_StartGame popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_StartGame>(UIPOPUP.POPUPSTARTGAME);

    }

    void callback_complete_StartGameBeginner()
    {
        User _user = UserDataManager.instance.user;

        //로딩바
        Loadmanager.instance.LoadingUI(true);

        Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.TRY_QUICKJOIN;

        //퀵조인 요청 보내기
        _user.user_Games.battleGameKind = BattleKind.BEGINNER_TEAM_BATTLE;
        Link_Script.i.GamePlay_Send_Quick_Join(_user.user_Games.battleGameKind);
    }





    //이벤트배너
    public void ResponseButton_EventBanner()
    {

        //StartCoroutine(StaticMethod.routine_waitForInAppProducts(showEventBanner));
        showEventBanner();

    }


    void showEventBanner()
    {
        //캐릭로비 로테이트 비활성
        User.isSelectedCharacter = true;

        //스폐셜상품 인덱스
        ushort SpecialShopProductIdx = 308;

        Infos_Shop specialShopInfo = null;
        if (TableDataManager.instance.Infos_shops.ContainsKey(SpecialShopProductIdx))
            specialShopInfo = TableDataManager.instance.Infos_shops[SpecialShopProductIdx];

        if (specialShopInfo != null)
        {
            UI_Popup_ShopPurchase popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_ShopPurchase>(UIPOPUP.POPUPSHOPPURCHASE);
            popup.Set_ShopPuchase(specialShopInfo, STOREMODE_TYPE.Package);
        }
    }






    #region 초대메세지 관련

    //초대메세지 설정
    void Set_InviteMessage()
    {
        User _user = UserDataManager.instance.user;

        //초대메세지가 1개이상이고 , 메세지 오브젝트가 비활성이면 메세지보이기
        if (!isActive_publicLobbyObj(EumLobbyObj.InviteMsg) && _user.User_LstRcvRoomInvites.Count > 0)
        {
            Show_InviteMessage();
        }
    }


    //초대 메세지 보이기
    public void Show_InviteMessage()
    {
        User _user = UserDataManager.instance.user;


        if (!isActive_publicLobbyObj(EumLobbyObj.InviteMsg))
        {
            Activate_publicLobbyObj(EumLobbyObj.InviteMsg, true);


            text_Sender.text = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserNkNm;

            //초대자 아이디 저장
            inviterUserID = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserID;

        }
        else
        {
            Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);
            Activate_publicLobbyObj(EumLobbyObj.InviteMsg, true);

            text_Sender.text = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserNkNm;

            //초대자 아이디 저장
            inviterUserID = _user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1].InviterUserID;

        }
    }

    //초대수락
    public void ResponseButton_AcceptInvite()
    {
        User _user = UserDataManager.instance.user;
        User_Units unit = null;
        Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);

        //유닛 소유하고 있다면
        if (_user.User_Units.ContainsKey(_user.User_useUnit.UnitIdx))
        {
            unit = _user.User_Units[_user.User_useUnit.UnitIdx];
        }
        else //유닛 소유 않고 있다면 가지고 있는것중 아무거나 할당
        {
            foreach (var hUnit in _user.User_Units)
            {
                unit = hUnit.Value;
                _user.User_useUnit.UnitIdx = unit.Unitidx; //배틀 에 값정보 전달하기위해 User_useUnit 값변경해야한다.
                break;
            }

        }


        Network_MainMenuSoketManager.Getsingleton.Send_CTS_Answer_IntiveRoom(inviterUserID, true, (int)unit.Unitidx, unit.RefLv,
            (int)unit.MainWpnIdx, (int)unit.SubWpnIdx, unit.DecoIdx1, unit.DecoIdx2);

        inviterUserID = 0;

        //초대 수락했으니  초대 리스브 정보들 클리어
        _user.User_LstRcvRoomInvites.Clear();
    }

    //초대메시지 닫기
    public void ResponseButton_CloseInviteMsg()
    {
        User _user = UserDataManager.instance.user;
        //Activate_publicLobbyObj(LobbyObj.InviteMsg, false);
        Network_MainMenuSoketManager.Getsingleton.Send_CTS_Answer_IntiveRoom(inviterUserID, false);


        //초대 닫기햇으니 다음 초대메세지 보이게
        if (_user.User_LstRcvRoomInvites.Count > 0)
        {
            var lst_same = _user.User_LstRcvRoomInvites.RemoveAll(n => n.InviterUserID == inviterUserID);

            if (lst_same == 0)
                _user.User_LstRcvRoomInvites.Clear();

            //for (int i = 0; i < _user.User_LstRcvRoomInvites.Count; i++)
            //{
            //	if (inviterUserID == _user.User_LstRcvRoomInvites[i].InviterUserID)
            //		_user.User_LstRcvRoomInvites.Remove(_user.User_LstRcvRoomInvites[i]);
            //}
            //_user.User_LstRcvRoomInvites.Remove(_user.User_LstRcvRoomInvites[_user.User_LstRcvRoomInvites.Count - 1]);

            inviterUserID = 0;


            if (_user.User_LstRcvRoomInvites.Count != 0)
                Show_InviteMessage();
            else
                Activate_publicLobbyObj(EumLobbyObj.InviteMsg, false);

        }

    }


    #endregion





    public void ResponseButton_Equipment()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.EQUIPMENT, _canvasTr);

    }

    public void ResponseButton_setting()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.SETTING, _canvasTr);

    }

    public void ResponseButton_Clan()
    {
        //로비캐릭회전 잠금
        User.isSelectedCharacter = true;
        webRequest.ClanInfo(callback_complete_ClanInfo);
    }



    void callback_complete_ClanInfo()
    {
        //로비캐릭회전 잠금해제
        User.isSelectedCharacter = false;

        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.CLAN, _canvasTr);
    }



    public void ResponseButton_Ranking()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.RANKING, _canvasTr);

    }
    public void ResponseButton_Friends()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.FRIEND, _canvasTr);
    }
    public void ResponseButton_News()
    {
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.NEWS, _canvasTr);
    }


    public void ReseponseButton_gotoSUGfacebookPage()
    {
        Application.OpenURL("https://www.facebook.com/HiClegames/");
    }


    //뒤로가기 (서버선택으로)
    public void ResponseButton_Back()
    {
        User _user = UserDataManager.instance.user;

        //if (_user.User_GameServerInfos.Count > 1)
        {
            //로그인상태 변경 -> 서버선택으로
            _user.LogInState = Login_State.LogSelectServer;
        }
        //else
        //	_user.LogInState = Login_State.LogOut;


        //메인메뉴 소켓서버 끊기 
        Network_MainMenuSoketManager.Getsingleton.Disconnect(DISCONNECT_STATE.NORMALITY, "서버선택로그아웃 인한 Disconnect");

        //타이틀 UI생성
        Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        UI_Manager.Getsingleton.CreatUI(UI.TITLE, _canvasTr);
    }




}
