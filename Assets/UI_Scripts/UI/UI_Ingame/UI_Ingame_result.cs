using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class UI_Ingame_result : UI_Base 
{
	public enum ResultRwdType
	{
		EXP = 0,
		GOLD,
		GEM,
	}

    private Dictionary<ITEMTYPE, bool> Dic_showRewardFlag = new Dictionary<ITEMTYPE, bool>();
    private GainItem willSndLvGainItem = null;

	public Image Image_result;

	//왼쪽 토탈리워드 UI 및 오브젝트
	public Text text_totalExp;
	public Text text_totalGold;
	public Text text_totalGem;
	public List<GameObject> Lst_BuffOBJ;
	public List<GameObject> Lst_GoodsEffect;						//재화의 이펙트
    public List<GameObject> Lst_RewardBtnObj;

    public GameObject winEffect;




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
		if(winFlag == 0) //승
			Image_result.sprite  = ImageManager.instance.Get_Sprite(DefineKey.text_Win);
		else if(winFlag == 1)//패
			Image_result.sprite  = ImageManager.instance.Get_Sprite(DefineKey.text_Lose);
		else if(winFlag == 2) //무승부
			Image_result.sprite = ImageManager.instance.Get_Sprite(DefineKey.text_Draw);

		Image_result.SetNativeSize();

		//승이면 이펙트 활성
		winEffect.SetActive(winFlag == 0);


		//아이템들 설정
		Apply_Items(webResponse.GetResultInfoList);

	}



	

	//아이템들 설정
	public void Apply_Items(List<GainItem> list_gainItem)
	{
		User _user = UserDataManager.instance.user;

		for (int i = 0; i < list_gainItem.Count; i++ )
		{
			switch (list_gainItem[i].ItTp)
			{
				case ITEMTYPE.GEM:
					////보석
					//text_totalGem.text = list_gainItem[i].num.ToString();
					////재화이펙트 시작
					//Start_GoodsEffect(list_gainItem[i].ItTp);


                   break;
				case ITEMTYPE.GOLD:
					//골드
					text_totalGold.text = list_gainItem[i].num.ToString();
					//재화이펙트 시작
					Start_GoodsEffect(list_gainItem[i].ItTp);

                    break;
				case ITEMTYPE.UNIT:
					
					break;
                case ITEMTYPE.EXP:
                    text_totalExp.text = list_gainItem[i].num.ToString();
                    //경험치이펙트 시작
                    Start_GoodsEffect(ITEMTYPE.EXP);

                    break;
                case ITEMTYPE.LV:
                    //로비로 데이터 보내기위해 할당하자
                    willSndLvGainItem = list_gainItem[i];
                    break;
            }

			
		}


	


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



    void Active_AdRewardBtn(ITEMTYPE type , bool isActive)
    {
        User user = UserDataManager.instance.user;

        //광고보기 보상받기가 처리안될수 잇다 그럼 PlayGoldRwd이  0 아니게 된다 그러면 버튼을 살리자
        if (isActive == false)
        {
            if (type == ITEMTYPE.GOLD && user.User_Ads.PlayGoldRwd != 0) isActive = true;
            else if (type == ITEMTYPE.EXP && user.User_Ads.PlayExpRwd != 0) isActive = true;
        }


        int idx = 0;
        if (type == ITEMTYPE.EXP)
            idx = 0;
        else if (type == ITEMTYPE.GOLD)
            idx = 1;
        else if (type == ITEMTYPE.GEM)
            idx = 2;

        Lst_RewardBtnObj[idx].SetActive(isActive);
    }
	






	void Clear_ResultUIInfo()
	{
		//text_totalGem.text = "0";
		text_totalGold.text = "0";
		text_totalExp.text = "0";

		for (int i = 0; i < Lst_BuffOBJ.Count; i++ )
		{
			Lst_BuffOBJ[i].SetActive(false);
		}


		for (int i = 0; i < Lst_GoodsEffect.Count; i++ )
		{
			Lst_GoodsEffect[i].SetActive(false);
		}

		winEffect.SetActive(false);


        for (int i = 0; i < Lst_RewardBtnObj.Count; i++)
        {
            Lst_RewardBtnObj[i].SetActive(true);
        }

    }





    //광고보고 결과보상 배로 받기
    public void Responsebutton_ShowReward(int rwdTypeIdx)
    {
        AdDoubleRewardType adRwdType = AdDoubleRewardType.None;
        if ((ITEMTYPE)rwdTypeIdx == ITEMTYPE.GOLD) adRwdType = AdDoubleRewardType.GameGold;
        else if ((ITEMTYPE)rwdTypeIdx == ITEMTYPE.EXP) adRwdType = AdDoubleRewardType.GameExp;


        //광고보기가 가능한지 체크
        bool canShowAd = false;
        User user = UserDataManager.instance.user;

        if (adRwdType == AdDoubleRewardType.GameGold && user.User_Ads.PlayGoldRwd != 0) canShowAd = true;
        else if (adRwdType == AdDoubleRewardType.GameExp && user.User_Ads.PlayExpRwd != 0) canShowAd = true;

        if (canShowAd)
        {
            //다시 획득아이템받기위해  이전 획득한 아이템 리스트 클리어
            webResponse.GetResultInfoList.Clear();

            webRequest.GetAdDoubleRwd(adRwdType, () => complete_ShowAdReward((ITEMTYPE)rwdTypeIdx));
        }
        complete_ShowAdReward((ITEMTYPE)rwdTypeIdx);
    }

    void complete_ShowAdReward(ITEMTYPE itemType)
    {
        //광고보왓으니 플레그 변경
        Active_AdRewardBtn(itemType, false);

        int orinNum = 0;
        int rewardNum = 0;
        Text targetText = null;


        for (int i = 0; i < webResponse.GetResultInfoList.Count; i++)
        {
            switch (webResponse.GetResultInfoList[i].ItTp)
            {
                case ITEMTYPE.GEM:
                    break;
                case ITEMTYPE.GOLD:
                    targetText = text_totalGold;
                    rewardNum = webResponse.GetResultInfoList[i].num;

                    break;
                case ITEMTYPE.EXP:
                    targetText = text_totalExp;
                    rewardNum = webResponse.GetResultInfoList[i].num;
                    break;
                case ITEMTYPE.LV:
                    //로비로 데이터 보내기위해 할당하자
                    willSndLvGainItem = webResponse.GetResultInfoList[i];
                    break;
            }




        }



        //if (itemType == ITEMTYPE.EXP)
        //{
        //    targetText = text_totalExp;
        //    rewardNum = UserDataManager.instance.user.User_Ads.PlayExpRwd;
        //}
        //else if (itemType == ITEMTYPE.GOLD)
        //{
        //    targetText = text_totalGold;
        //    rewardNum = UserDataManager.instance.user.User_Ads.PlayGoldRwd;
        //}
        //else if (itemType == ITEMTYPE.GEM)
        //    targetText = text_totalGem;

        if(targetText!=null)
            orinNum = Convert.ToInt32(targetText.text);

        //test
        //targetText = text_totalGold;
        //orinNum = 50;
        //rewardNum = 60;

        //숫자 스무스하게 변하는거 처리
        StartCoroutine(co_smoothChangeNum(targetText, (float)orinNum, (float)rewardNum));
    }



    IEnumerator co_smoothChangeNum(Text targetTxt, float _orinNum, float _targetNum)
    {
        float lerpNum = _orinNum;
        float speed = _targetNum / _orinNum;
        while (true)
        {
            if(lerpNum >= _targetNum)
            {
                break;
            }

            lerpNum =  Mathf.MoveTowards(lerpNum, _targetNum, speed);

            targetTxt.text = lerpNum.ToString("0");

            yield return null;
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

        //할당할데이터
        if(willSndLvGainItem !=null )
        {
            UIData udata = new UIData(new List<object> { willSndLvGainItem });
            UI_Manager.Getsingleton.staticUIData = null; //이전 uidata null
            UI_Manager.Getsingleton.staticUIData = udata;
            willSndLvGainItem = null;
        }

	


		Scene _scene = SceneManager.GetActiveScene();
		if (_scene.name != DefineKey.Main)
		{
			SceneManager.LoadScene(DefineKey.Main);

		}



	}







	

	

	

}

