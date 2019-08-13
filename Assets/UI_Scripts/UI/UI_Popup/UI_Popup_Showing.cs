using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct ShowingInfo
{
	
}
public enum ShowingKind { UserlvUp=0,UnitLvUp, }

public class UI_Popup_Showing : UI_PopupBase
{
	

	public enum specType { Lv = 0, Hp = 1, Att = 2, Critical = 3 };
    private ShowingKind nowShowingKnd;
    private bool isCanTouchBackground;

	public List<GameObject> Lst_LvShowingObj;					//유닛레벨오브젝트,유저레벨오브젝트,리그오브젝트 의 리스트

	private Dictionary<int, Infos_rank> Dic_infosRank = new Dictionary<int, Infos_rank>(); // 다음 spec을 알기위한 유닛강화정보 컬렉션



//유닛레벨 상승 연출
	public Image image_unit;
	public Text text_unitName;
	public List<Text> lst_textBeforeValue;
	public List<Text> lst_textCurValue;

//유저레벨 상승 연출
	public Text text_CurUserlv;
	public Text text_rwdNum;
    public Text text_AdRwdRate;
    public GameObject userLvRewardBtnObj;

    private int basicRwdNum;        //기본 보상량






	public override void Set_Open()
	{
		base.Set_Open();
        isCanTouchBackground = false;




    }
	public override void Set_Close()
	{
		base.Set_Close();
	}

	

	/// <summary>
	/// 레벨관련 연출 위한 셋팅함수
	/// </summary>
	public void set_showingInfo(ShowingInfo _st_showingInfo, ShowingKind _knd)
	{
        nowShowingKnd = _knd;

        Activation_ShowingObject(_knd);

        TriggerBackTouchForMoment(1f);

        userLvRewardBtnObj.SetActive(true);

        switch (_knd)
        {
            case ShowingKind.UserlvUp:
                Process_userLvShowing();
                break;
            case ShowingKind.UnitLvUp:
                break;
        }


    }


    #region 유저레벨연출

    void Process_userLvShowing()
    {
        User user = UserDataManager.instance.user;
        //레벨
        int nowLv = user.Get_user_goods(ITEMTYPE.LV);
        text_CurUserlv.text = nowLv.ToString();

        TableDataManager table = TableDataManager.instance;
        
        if(table.Infos_UserLvs.ContainsKey((byte)nowLv))
            basicRwdNum = (int)table.Infos_UserLvs[(byte)nowLv].RwdItNum;

        //보상량
        text_rwdNum.text = basicRwdNum.ToString();

        //광고보기시 더받는 배율
    }






    bool isShowAd = false;
    //광고보고 보상더받기
    public void ResponseButton_ShowReward()
    {
        webRequest.GetAdDoubleRwd(AdDoubleRewardType.LvUpRwd, complete_showAdReward);
        //test
        //complete_showAdReward();


    }

    void complete_showAdReward()
    {
        userLvRewardBtnObj.SetActive(false);

        isShowAd = true;

        int rewardNum = 0;

        for (int i = 0; i < webResponse.GetResultInfoList.Count; i++)
        {
            switch (webResponse.GetResultInfoList[i].ItTp)
            {
                case ITEMTYPE.GEM:
                    rewardNum = webResponse.GetResultInfoList[i].num;
                    break;
                
            }




        }

        //test
        StartCoroutine(co_smoothChangeNum(text_rwdNum, (float)basicRwdNum, rewardNum));
    }



    IEnumerator co_smoothChangeNum(Text targetTxt, float _orinNum, float _targetNum)
    {
        float lerpNum = _orinNum;

        while (true)
        {
            if (lerpNum >= _targetNum)
            {
                break;
            }

            lerpNum = Mathf.MoveTowards(lerpNum, _targetNum, 2f);

            targetTxt.text = lerpNum.ToString("0");

            yield return null;
        }
    }


    #endregion







    //일정시간 백그라운드터치 잠그기
    void TriggerBackTouchForMoment(float time)
    {
        StartCoroutine(co_unlockBackTouch(time));
    }

    IEnumerator co_unlockBackTouch(float time )
    {
        yield return new WaitForSeconds(time);
            isCanTouchBackground = true;
    }






    public void Activation_ShowingObject(ShowingKind knd)
	{
		for (int i = 0; i < Lst_LvShowingObj.Count; i++ )
		{
			Lst_LvShowingObj[i].SetActive(i == (int)knd);
		}
	}


	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}
	

	

	public void ResponseButton_Confirm()
	{
        if (isCanTouchBackground)
        {

            //광고봣으면 GetResultInfoList클리어
            if ( isShowAd)
                webResponse.GetResultInfoList.Clear();

            if (delegate_ResponseOk != null)
                delegate_ResponseOk();


            UI_Manager.Getsingleton.ClearUI(this);
        }
	}



	
}
