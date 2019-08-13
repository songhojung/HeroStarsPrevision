using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class UI_Ingame_resultOld : UI_Base
{
    public enum ResultRwdType
    {
        EXP = 0,
        GOLD,
        GEM,
    }

    private List<UIItem_ResultUnitSlot> Lst_ResultSlotElement = new List<UIItem_ResultUnitSlot>();
    private Dictionary<uint, infos_unit> Dic_infosUnit = new Dictionary<uint, infos_unit>();

    public Transform Tr_content;

    public Image Image_result;

    //왼쪽 토탈리워드 UI 및 오브젝트
    public Text text_totalExp;
    public Text text_totalGold;
    public Text text_totalGem;
    public List<GameObject> Lst_BuffOBJ;
    public List<GameObject> Lst_GoodsEffect;                        //재화의 이펙트
    public GameObject winEffect;

    private int resultSlotIdx = 0;



    public override void set_Open()
    {
        base.set_Open();


    }

    public override void set_Close()
    {
        base.set_Close();

        Clear_ResultUIInfo();

    }


    public override void set_refresh()
    {
        base.set_refresh();
    }

    // 결과 보여주기
    public void showResult(byte winFlag)
    {
        //결과표시 
        if (winFlag == 0) //승
            Image_result.sprite = ImageManager.instance.Get_Sprite(DefineKey.text_Win);
        else if (winFlag == 1)//패
            Image_result.sprite = ImageManager.instance.Get_Sprite(DefineKey.text_Lose);
        else if (winFlag == 2) //무승부
            Image_result.sprite = ImageManager.instance.Get_Sprite(DefineKey.text_Draw);

        Image_result.SetNativeSize();

        //승이면 이펙트 활성
        winEffect.SetActive(winFlag == 0);

        resultSlotIdx = 0;

        //이전 element 삭제
        Clear_resultElement();

        //슬롯생성
        Create_ResultSlotElement();


        //슬롯 해당된것들 데이터 적용
        Apply_SlotElementsInfo();

        //아이템들 설정
        Apply_Items(webResponse.GetResultInfoList);


        //슬롯 정렬
        Sort_resultSlotElement();
    }

    void Create_ResultSlotElement()
    {

        //유닛구성정보 체크
        Dictionary<uint, Infos_EventItemTime> dic_eventUnit = TableDataManager.instance.Infos_EventItemTimes;
        Dictionary<uint, infos_unit> _dicInfosUnit = TableDataManager.instance.Infos_units;
        Dictionary<uint, infos_unit> _DicInfosUnitTemp = new Dictionary<uint, infos_unit>();

        foreach (var infoUnit in _dicInfosUnit)
        {
            //이벤트 유닛 인데 가지고 있으면 구성정보에 추가 없으면 뺸다
            if (dic_eventUnit.ContainsKey(infoUnit.Value.UnitIdx))
            {
                if (TimeManager.Instance.Get_nowTime() > dic_eventUnit[infoUnit.Value.UnitIdx].BuyEndTm)
                {
                    _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
                }
                else
                {
                    if (UserDataManager.instance.user.User_Units.ContainsKey(infoUnit.Value.UnitIdx))
                        _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
                }
            }
            else
                _DicInfosUnitTemp[infoUnit.Value.UnitIdx] = infoUnit.Value;
        }


        //유닛구성정보 할당
        Dic_infosUnit.Clear();
        Dic_infosUnit = _DicInfosUnitTemp;


        //생성
        foreach (var item in Dic_infosUnit)
        {
            UIItem_ResultUnitSlot resultEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_RESULTUNITSLOT, Tr_content) as UIItem_ResultUnitSlot;
            Lst_ResultSlotElement.Add(resultEle);
        }
    }

    void Apply_SlotElementsInfo()
    {
        User _user = UserDataManager.instance.user;

        Dictionary<uint, infos_unit> _dicInfosUnit = TableDataManager.instance.Infos_units;

        if (Lst_ResultSlotElement.Count > 0)
        {


            int idx = 0;
            foreach (var infounit in Dic_infosUnit)
            {

                if (_user.User_Units.ContainsKey(infounit.Value.UnitIdx))           // 가지고 잇는 유닛 슬롯 설정
                {
                    User_Units unit = _user.User_Units[infounit.Value.UnitIdx];
                    Lst_ResultSlotElement[idx].IsActiveSlot = true;
                    Lst_ResultSlotElement[idx].Set_unitIcon(infounit.Value.UnitIdx);
                    idx++;
                }
            }


            foreach (var infounit in Dic_infosUnit)
            {
                if (!_user.User_Units.ContainsKey(infounit.Value.UnitIdx))
                {
                    Lst_ResultSlotElement[idx].IsActiveSlot = false;// 가지고 있지 않으니 유닛슬롯 미설정
                    Lst_ResultSlotElement[idx].Set_unitIcon(infounit.Value.UnitIdx);
                    idx++;
                }
            }



        }
    }

    //아이템들 설정
    public void Apply_Items(List<GainItem> list_gainItem)
    {
        User _user = UserDataManager.instance.user;
        int sumExp = 0;

        for (int i = 0; i < list_gainItem.Count; i++)
        {
            switch (list_gainItem[i].ItTp)
            {
                case ITEMTYPE.GEM:
                    //보석
                    text_totalGem.text = list_gainItem[i].num.ToString();
                    //재화이펙트 시작
                    Start_GoodsEffect(list_gainItem[i].ItTp);
                    break;
                case ITEMTYPE.GOLD:
                    //골드
                    text_totalGold.text = list_gainItem[i].num.ToString();
                    //재화이펙트 시작
                    Start_GoodsEffect(list_gainItem[i].ItTp);
                    break;
                case ITEMTYPE.UNIT:
                    Get_slotElement(list_gainItem[i]).Set_resultUnitSlot(list_gainItem[i]);
                    sumExp += (int)list_gainItem[i].gainUserUnit.UnitExp;
                    break;
            }


        }



        if (sumExp > 0)
            //경험치이펙트 시작
            Start_GoodsEffect(ITEMTYPE.EXP);
        //경험치
        text_totalExp.text = sumExp.ToString();

        //버프
        //클랜골드버프
        if (_user.clan_Clans.GoldBufTm > TimeManager.Instance.Get_nowTime())
            Lst_BuffOBJ[(int)BUFF_TYPE.CLANGOLD].SetActive(true);
        else
            Lst_BuffOBJ[(int)BUFF_TYPE.CLANGOLD].SetActive(false);

        //클랜exp버프
        if (_user.clan_Clans.ExpBufTm > TimeManager.Instance.Get_nowTime())
            Lst_BuffOBJ[(int)BUFF_TYPE.CLANEXP].SetActive(true);
        else
            Lst_BuffOBJ[(int)BUFF_TYPE.CLANEXP].SetActive(false);


        //이벤트exp버프
        bool expEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.EXP);
        Lst_BuffOBJ[(int)BUFF_TYPE.EVENTEXP].SetActive(expEvent);

        //이벤트 gold버프
        bool goldEvent = StaticMethod.Chk_EventBuff(EVENT_KIND.GOLD);
        Lst_BuffOBJ[(int)BUFF_TYPE.EVENTGOLD].SetActive(goldEvent);



    }

    UIItem_ResultUnitSlot Get_slotElement(GainItem _gainItem)
    {
        UIItem_ResultUnitSlot slot = null;

        if (Lst_ResultSlotElement[resultSlotIdx].IsActiveSlot)
        {
            slot = Lst_ResultSlotElement[resultSlotIdx];
            resultSlotIdx++;
        }


        return slot;
    }


    //해당 제화 이펙트 시작
    void Start_GoodsEffect(ITEMTYPE itemType)
    {
        int idx = 0;
        if (itemType == ITEMTYPE.EXP)
            idx = 0;
        else if (itemType == ITEMTYPE.GOLD)
            idx = 1;
        else if (itemType == ITEMTYPE.GEM)
            idx = 2;
        else
            return;

        if (Lst_GoodsEffect[idx].activeSelf == false)
            Lst_GoodsEffect[idx].SetActive(true);

    }



    //결과 슬롯정렬
    void Sort_resultSlotElement()
    {
        //경험치 별로 정렬
        var sortedLst = from lst in Lst_ResultSlotElement
                        orderby lst.Get_gainItem() == null
                        where lst.Get_gainItem() != null
                        orderby lst.Get_gainItem().gainUserUnit.UnitExp descending
                        select lst;

        List<UIItem_ResultUnitSlot> lst_slot = sortedLst.ToList();

        //하이라키 정렬
        for (int i = 0; i < lst_slot.Count; i++)
        {
            lst_slot[i].transform.SetSiblingIndex(i);
        }

    }






    void Clear_resultElement()
    {
        for (int i = 0; i < Lst_ResultSlotElement.Count; i++)
        {
            Destroy(Lst_ResultSlotElement[i].gameObject);
        }
        Lst_ResultSlotElement.Clear();
    }

    void Clear_ResultUIInfo()
    {
        text_totalGem.text = "0";
        text_totalGold.text = "0";
        text_totalExp.text = "0";

        for (int i = 0; i < Lst_BuffOBJ.Count; i++)
        {
            Lst_BuffOBJ[i].SetActive(false);
        }


        for (int i = 0; i < Lst_GoodsEffect.Count; i++)
        {
            Lst_GoodsEffect[i].SetActive(false);
        }

        winEffect.SetActive(false);
    }







    // 게임결과시에 관련내용 통계내리기
    void Analyze_GameEnd()
    {
        //게임판수 통계
        AnalyzePlayCount();

        //유닛별 킬데스 통계
        AnalyzeUnitKD();
    }


    void AnalyzePlayCount()
    {
        //게임판수
        bool isDo = false;
        int playCount = UserDataManager.instance.user.Get_ChkPlayCount();

        if (playCount <= 5)// 1,2,3,4,5
            isDo = true;
        else if (playCount <= 50 && (playCount % 10 == 0)) // 10,20,30,40,50
            isDo = true;
        else if (playCount == 100) // 100
            isDo = true;

        //if (isDo)
        //	AnalysisManager.instance.Anl_CustomEvt(DefineKey.Game, DefineKey.PlayCount, string.Format("{0}_{1}", DefineKey.PlayCount, playCount), null, 1);
    }


    //유닛별 킬데스 통계
    void AnalyzeUnitKD()
    {
        User _user = UserDataManager.instance.user;
        Dictionary<uint, User_Units> dic_userUnit = _user.User_Units;

        if (_user.User_unit_useInfos.ContainsKey(_user.User_useUnit.UnitIdx))
        {
            string stnd = string.Empty;
            float killStnd = 0;

            uint death = _user.User_unit_useInfos[_user.User_useUnit.UnitIdx].DeathCnt;
            uint kill = _user.User_unit_useInfos[_user.User_useUnit.UnitIdx].KillCnt;
            if (death == 0) death = 1;
            float kdRate = (float)(kill) / (float)(death);


            if (kdRate >= 5f)
            {
                killStnd = 5f;
                stnd = DefineKey.Over1;
            }
            else if (kdRate >= 3f)
            {
                killStnd = 3f;
                stnd = DefineKey.Over1;
            }
            else if (kdRate >= 2f)
            {
                killStnd = 1f;
                stnd = DefineKey.Over1;
            }
            else if (kdRate > 1f)
            {
                killStnd = 1f;
                stnd = DefineKey.Over1;
            }
            else if (kdRate <= 0.5f)
            {
                killStnd = 0.5f;
                stnd = DefineKey.LessThan;
            }
            else if (kdRate <= 1f)
            {
                killStnd = 1f;
                stnd = DefineKey.LessThan;
            }
            //AnalysisManager.instance.Anl_CustomEvt(DefineKey.Unit, DefineKey.UnitKD, string.Format("{0}{1}", killStnd, stnd), null, 1);
        }
    }



    public void ResponseButton_OK()
    {

        //획득한 아이템 리스트 클리어
        webResponse.GetResultInfoList.Clear();

        Link_Script.i.GameOver_Init();

        //모든팝업 클리어하기
        UI_Manager.Getsingleton.ClearPopupUI();

        UI_Manager.Getsingleton.ClearALL_UI();

        UI_Manager.Getsingleton._UI = UI.LOBBY;

        //게임끝냇으니 광고보기on
        //AdsManager.instance.IsShowAdsGameEnd = true;

        //분석하기 
        Analyze_GameEnd();


        Scene _scene = SceneManager.GetActiveScene();
        if (_scene.name != DefineKey.Main)
        {
            SceneManager.LoadScene(DefineKey.Main);

        }



    }
}
