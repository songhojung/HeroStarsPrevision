using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.SimpleAndroidNotifications;


public class UI_Top : UI_Base 
{
	private static UI_Top _instance;
	public static UI_Top Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Top)) as UI_Top;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("UI_Top");
					_instance = instanceObj.AddComponent<UI_Top>();
				}
			}

			return _instance;
		}
	}

	public enum AdIconState
	{
		NONE = 99,
		Wait = 0,
		Gem = 1,
		Gold= 2,
		Roulette = 3,
	}

	void Awake()
	{
		_instance = this;
	}

    private User _user;

	public Text text_money;
	public Text text_gem;
	public Text text_FreeGemLeftTime;
    public Text text_UserLv;
    public Text text_Exp;

    public UI_Popup_Store popupStore;

	//광고
	public List<GameObject> Lst_AdStateOBJ; //광고 상태 오브젝트들
	public List<Text> Lst_textAdicon;
	private ITEMTYPE adRewardItem = ITEMTYPE.NONE;

	private Dictionary<string, Coroutine> Dic_routine = new Dictionary<string, Coroutine>();

	public void OnEnable()
	{
		if (Dic_routine.ContainsKey("FreeGem"))
			StartCoroutine(routine_FreeGemLeftTime());
	}


    public override void set_Open()
    {
        base.set_Open();

        _user = UserDataManager.instance.user;



		Set_TopInfo();
    }

	public override void set_Close()
	{
		base.set_Close();

		popupStore.gameObject.SetActive(false);
	}

	public override void set_refresh()
	{
		base.set_refresh();

		_user = UserDataManager.instance.user;

		Set_TopInfo();

	}

	void Set_TopInfo()
	{

		text_money.text = _user.Get_user_goods(ITEMTYPE.GOLD).ToString();
		text_gem.text = _user.Get_user_goods(ITEMTYPE.GEM).ToString();

        //레벨
        int lv = _user.Get_user_goods(ITEMTYPE.LV);
        text_UserLv.text = string.Format("Lv.{0}",lv);

        //경험치 퍼센트
        TableDataManager table = TableDataManager.instance;
        int nowExp = _user.Get_user_goods(ITEMTYPE.EXP);
        int nowLvNeedExp = 0;
        int nextLvNeedExp = 0;
        if(table.Infos_UserLvs.ContainsKey((byte)(lv+1)))
        {
            nextLvNeedExp = (int)table.Infos_UserLvs[(byte)(lv + 1)].UserExp;
        }

        if (table.Infos_UserLvs.ContainsKey((byte)(lv)))
        {
            nowLvNeedExp = (int)table.Infos_UserLvs[(byte)(lv)].UserExp;
        }
        float expPersant = (float)(nowExp - nowLvNeedExp) / (float)(nextLvNeedExp - nowLvNeedExp);
        text_Exp.text = string.Format("{0:f2}%", expPersant * 100f);

        //광고 관련 UI 설정
        Set_AdUI();



	}


	//광고 관련 UI 설정
	void Set_AdUI()
	{
	
	
			if (TimeManager.Instance.Get_nowTime() < _user.User_Ads.AdTm)
			{
				Activate_AdstateObj(AdIconState.Wait);
				//text_FreeGemLeftTime.gameObject.SetActive(true);
				if (!Dic_routine.ContainsKey("FreeGem"))
				{
					Coroutine _routine = StartCoroutine(routine_FreeGemLeftTime());
					Dic_routine["FreeGem"] = _routine;
				}

				//text_FreeGemLeftTime.text = string.Format("{0}:{1}", span.Minutes, span.Seconds);
			}
			else
			{
				//다음 광고 상태오브젝트 표시 
				Dictionary<ushort, Infos_AdReward> adReward = TableDataManager.instance.Infos_AdRewards;
				adRewardItem = _user.User_Ads.RwdItTp;
				AdIconState _adIcnState = AdIconState.NONE;
				ushort All_ItemNum = (ushort)(_user.Get_allWeaponCount() + _user.Get_allDecoCount());

				//아이템 갯수별 보상아이템갯수
				int rwdCount = TableDataManager.instance.Get_AdsRwdNum(All_ItemNum, adRewardItem);
				

				//상태값 저장
				switch (adRewardItem)
				{
					case ITEMTYPE.GEM:_adIcnState = AdIconState.Gem;
						Lst_textAdicon[((int)_adIcnState) - 1].text = string.Format("x{0}", rwdCount);
						break;
					case ITEMTYPE.GOLD:_adIcnState = AdIconState.Gold;
						Lst_textAdicon[((int)_adIcnState) - 1].text = string.Format("x{0}", rwdCount);
						break;
					case ITEMTYPE.ROULETTE: _adIcnState = AdIconState.Roulette; break;
					default: _adIcnState = AdIconState.Roulette; break;
				}

			

				Activate_AdstateObj(_adIcnState);
				//text_FreeGemLeftTime.gameObject.SetActive(false);

				//보상광고로드 하기
				//AdsManager.instance.Reuest_Ads(Ads_TYPE.Reward);
			}

		
		
	}

	IEnumerator routine_FreeGemLeftTime()
	{
		TimeSpan span = new TimeSpan();

		while(true)
		{
			if (_user.User_Ads.AdTm < TimeManager.Instance.Get_nowTime())
			{
				Dic_routine.Remove("FreeGem");
				set_refresh();
				
				break;
			}

			span = _user.User_Ads.AdTm - TimeManager.Instance.Get_nowTime();

			if (span.Hours > 0)
				text_FreeGemLeftTime.text = string.Format("{0}:{1}:{2}", span.Hours, span.Minutes.ToString("D2"), span.Seconds.ToString("D2"));
			else
				text_FreeGemLeftTime.text = string.Format("{0}:{1}", span.Minutes,span.Seconds.ToString("D2"));

			yield return null;
		}
	}



	void Activate_AdstateObj(AdIconState activeState)
	{
		for (int i = 0; i < Lst_AdStateOBJ.Count; i++ )
		{
			if (i == (int)activeState)
				Lst_AdStateOBJ[i].SetActive(true);
			else
				Lst_AdStateOBJ[i].SetActive(false);

		}
	}








	public void ResponseButton_Gold()
	{
		Store_Start_OnTap(STOREMODE_TYPE.Gold);
		
	}

	public void ResponseButton_Gem()
	{

        //StartCoroutine(StaticMethod.routine_waitForInAppProducts(() => Store_Start_OnTap(STOREMODE_TYPE.Gem)));
        Store_Start_OnTap(STOREMODE_TYPE.Gem);

    }


	public void ResponseButton_Store()
	{
        //Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
        //UI_Manager.Getsingleton.CreatUI(UI.STORE, _canvasTr);


        //StartCoroutine(StaticMethod.routine_waitForInAppProducts(Active_Store));
        Active_Store();

    }


	void Active_Store()
	{
		User.isSelectedCharacter = !popupStore.gameObject.activeSelf;
		if (popupStore.gameObject.activeSelf == false)
			popupStore.Start_OnTap(STOREMODE_TYPE.Package);

		popupStore.gameObject.SetActive(!popupStore.gameObject.activeSelf);
	}

	public void Store_Start_OnTap(STOREMODE_TYPE storeMode)
	{
		User.isSelectedCharacter = true;
		popupStore.Start_OnTap(storeMode);

		popupStore.gameObject.SetActive(true);

	}








	#region 광고보기 관련
	public void ResponseButton_FreeGem()
	{

		//User_Times usertime = _user.User_Times[(int)TIMEIDX.ADREWARD];
		if (TimeManager.Instance.Get_nowTime() > _user.User_Ads.AdTm)
		{
#if UNITY_EDITOR
			callback_Complete_WatchAdFreeGem();
#else
				//AdsManager.instance.Show_RewardAds();
				//AdsManager.instance.nextAdsProcess = callback_Complete_WatchAdFreeGem;
#endif


		}
	}


	void callback_Complete_WatchAdFreeGem()
	{
			webRequest.SetAdRwd(callback_Complete_GetAdsReward);
	}

	void callback_Complete_GetAdsReward()
	{

		UserEditor.Getsingleton.EditLog("룰렛인덱스 : " + UserDataManager.instance.user.RltIdx);
		if (adRewardItem != ITEMTYPE.ROULETTE) //아이템타입이 룰렛이아니면
		{


			UI_Popup_GainItem popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_GainItem>(UIPOPUP.POPUPGAINITEM);
			popup.Set_GainPopup();

	
			//top ui 갱신
				UI_Top.Getsingleton.set_refresh();

				//클라푸쉬 
				//if (OptionSetting.instance.Notice_pushMsg)
				//{
				//	 TimeSpan span = _user.User_Ads.AdTm - TimeManager.Instance.Get_nowTime();
				//	//double pushtime = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_limitAdTime].ConsVal * 60; // 600초
				//	 double pushtime = span.TotalSeconds;
				//	NotificationManager.SendPush(TimeSpan.FromSeconds(pushtime), TextDataManager.Dic_TranslateText[304], TextDataManager.Dic_TranslateText[310]);
				//}
		}
		else //룰렛이면
		{
			//룰렛판 돌리기
			UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Roulette>(UIPOPUP.POPUPROULETTE);
		}
	}

	#endregion




	
	
}
