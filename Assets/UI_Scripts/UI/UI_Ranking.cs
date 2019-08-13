using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UI_Ranking : UI_Base
{
	

	private RankingType nowRankingType = RankingType.WHOLE;
    private List<UIItem_rankingElement> Lst_RankingElement = new List<UIItem_rankingElement>();
    private Dictionary<uint, Rank_UnitExp> Dic_rankLeague = new Dictionary<uint, Rank_UnitExp>();
    private uint requestRankingUnitidx = 0; //랭킹리스트 요청할 유닛인덱스 ( 0은 전체)
    private int maxViewElemetCnt = 5;
    private int nowPage = 1;
    private int MaxPage = 0;

    public Transform ContentTr;
	//UI
	public Text text_Ranking;
	public Image image_ClanMark;
	public Text text_userName;
	public Text text_Exp;
	public Text text_KD;
    public Text text_Lv;
    public Text text_Page;

    public override void set_Open()
	{
		base.set_Open();


        Try_unitRankingList();

	}

	public override void set_Close()
	{ 
		base.set_Close();

		ClearElement(); //UI 닫을때도 다 삭제하자 

		//hadInit = false;
		
	}

	public override void set_refresh()
	{
		base.set_refresh();

		Set_Ranking();
	}



	void Set_Ranking()
	{

		Try_unitRankingList();
	}



	void Try_unitRankingList()
	{

		//if (nowRankingType != RankingType.WHOLE)
		//{
		//	string name = nowRankingType.ToString();
		//	foreach (var unit in TableDataManager.instance.Infos_units)
		//	{
		//		if (string.Equals(unit.Value.UnitName, name))
		//		{
		//			webRequest.UnitExpRanking(unit.Value.UnitIdx, Apply_Ranking);
		//		}

		//	}
		//}
		//else
		{
            requestRankingUnitidx = 0;

            webRequest.UnitExpRanking(requestRankingUnitidx, Apply_Ranking);
		}
	}








	


	//랭킹 element 생성
	public void Apply_Ranking()
	{
		RankingType rankType = RankingType.None;

			rankType = nowRankingType;

		

		// 나의 랭킹설정
		Set_MyRanking(rankType);


        //rank element 갱신및 생성
        Set_RankElemets();

        //페이지수
        Set_RankingPage();

    }






	// 내랭킹 설정
	private void Set_MyRanking(RankingType _rankingType)
	{
		User _user = UserDataManager.instance.user;

		string name = nowRankingType.ToString();
		uint _Unitidx = 0;
		if (_rankingType != RankingType.WHOLE)
			_Unitidx = 10000 + (uint)_rankingType + 1;
		//foreach (var unit in TableDataManager.instance.Infos_units)
		{
			//if (string.Equals(unit.Value.UnitName, name) || string.Equals("WHOLE", name))
			{
				Rank_UnitExp uRank = null;

				if (_user.User_UnitRanking.ContainsKey(_Unitidx))
				{
					uRank = _user.User_UnitRanking[_Unitidx];

					//랭크
					text_Ranking.text = uRank.Ranking.ToString();


					//클랜마크
					if (_user.user_Clans.ClanID != 0)
						image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, uRank.ClanMark));
					//이름
					text_userName.text = uRank.NkNm;
					//exp
					text_Exp.text = uRank.Exp.ToString();
					//킬뎃
					text_KD.text = uRank.KillDeath.ToString();

                    //lv
                    text_Lv.text = uRank.UserLv.ToString();
			
				}
				else
				{
					//내랭킹 정보 없는걸로 표시
					ClearMyRankingUI();
				}

				
			}
			
		}
	}


    void Set_RankingPage()
    {
        //총페이지 갯수
        MaxPage =(int) Mathf.Ceil( (float)Dic_rankLeague.Count / (float)maxViewElemetCnt);

        text_Page.text = string.Format("{0}/{1}", nowPage, MaxPage);
    }



    
    void Set_RankElemets()
    {
        Dic_rankLeague = UserDataManager.instance.user.Rank_DicUnitExps[requestRankingUnitidx];
        Dic_rankLeague = Arrange_ChkSameRank(Dic_rankLeague);  //등수 다시 정렬하기


        if (Lst_RankingElement.Count < maxViewElemetCnt)
        {
            //생성
            Create_RankElement(Dic_rankLeague, nowPage);
        }
        else
        {
            //갱신
            Refresh_RankElement(Dic_rankLeague, nowPage);
        }
    }


    //등수 다시 정렬하기
    Dictionary<uint, Rank_UnitExp> Arrange_ChkSameRank(Dictionary<uint, Rank_UnitExp> _dic_rank)
    {
        //같은 순위 처리 ex)30점이 20등이면 30점가진유저들 전부 20등으로만들기
        uint nowScore = 0;
        uint tempScore = 0;
        uint sameRank = 0;
        double kd = 0;
        //Dictionary<uint, Rank_Leage> _dic_sameRank = Dic_rankLeague;
        foreach (var rank in _dic_rank)
        {

            if (nowScore == 0)
            {
                nowScore = rank.Value.Exp;
                tempScore = nowScore;
                sameRank = rank.Value.Ranking;
                kd = Convert.ToDouble(rank.Value.KillDeath);
            }
            else
            {
                nowScore = rank.Value.Exp;


                if (nowScore == tempScore)
                {
                    if (kd <= Convert.ToDouble(rank.Value.KillDeath))
                        rank.Value.Ranking = sameRank;
                    else
                    {
                        sameRank = rank.Value.Ranking;
                        kd = Convert.ToDouble(rank.Value.KillDeath);
                    }

                }
                else
                {
                    sameRank = rank.Value.Ranking;
                    kd = Convert.ToDouble(rank.Value.KillDeath);
                }
                tempScore = nowScore;
            }

        }
        return _dic_rank;
    }



    void Create_RankElement(Dictionary<uint, Rank_UnitExp> _dic_rank,int page)
    {
        var LstRankData = _dic_rank.ToList();
        int startIndex = (page - 1) * maxViewElemetCnt;
        int endIndex = LstRankData.Count < maxViewElemetCnt ? LstRankData.Count : page * maxViewElemetCnt;
        int createdElementCnt = Lst_RankingElement.Count;

        for (int i = startIndex; i < endIndex; i++)
        {
            if(createdElementCnt > 0 &&  (createdElementCnt - 1) - i >= 0)
            {
                //갱신
                Lst_RankingElement[i].Set_elementInfo(LstRankData[i].Value, RankingType.WHOLE);
            }
            else
            {
                //생성

                UIItem_rankingElement _rEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_RANKINGELEMENT, ContentTr) as UIItem_rankingElement;
                _rEle.Set_elementInfo(LstRankData[i].Value, RankingType.WHOLE);

                Lst_RankingElement.Add(_rEle);
            }

        }

       
    }

    void Refresh_RankElement(Dictionary<uint, Rank_UnitExp> _dic_rank, int page)
    {
        var LstRankData = _dic_rank.ToList();
        int startIndex = (page - 1) * maxViewElemetCnt;
        int endIndex = page * maxViewElemetCnt;
        endIndex = LstRankData.Count < endIndex ? LstRankData.Count : endIndex;

        for (int i = 0; i < Lst_RankingElement.Count; i++)
        {
            int dataIdx = startIndex + i;
            if (dataIdx < endIndex)
            {
                Lst_RankingElement[i].gameObject.SetActive(true);
                Lst_RankingElement[i].Set_elementInfo(LstRankData[dataIdx].Value, RankingType.WHOLE);
            }
            else
            {
                Lst_RankingElement[i].gameObject.SetActive(false);
            }
        }

        //for (int i = startIndex ; i < endIndex; i++)
        //{
        //    Lst_RankingElement[i].Set_elementInfo(LstRankData[i].Value, RankingType.WHOLE);

        //}
    
       
    }

 //       IEnumerator coroutine_CreatElement(RankingType rankType)
	//{
	//	Dictionary<uint, Rank_UnitExp> dic_rank = new Dictionary<uint, Rank_UnitExp>();
	//	uint unitIdx = 0; 
	//	if(rankType != RankingType.WHOLE)
	//		unitIdx = 10000 + (uint)rankType + 1;

	//	dic_rank = UserDataManager.instance.user.Rank_DicUnitExps[unitIdx];

	//	if (dic_rank.Count > 0)
	//		{

	//			List<UIItem_rankingElement> _lstElement = null;

	//			if (Dic_rankingEleLst.ContainsKey(rankType))
	//				_lstElement = Dic_rankingEleLst[rankType];
	//			else
	//				_lstElement = new List<UIItem_rankingElement>();

	//			bool isDoRefresh = Chk_CreatedElementLstPerfect(_lstElement);

	//			if(isDoRefresh == false)
	//				Loadmanager.instance.LoadingUI(true);

	//			int _lstIdx = 0;

	//			//같은 순위 처리 ex)30점이 20등이면 30점가진유저들 전부 20등으로만들기
	//			uint nowScore = 0;
	//			uint tempScore = 0;
	//			uint sameRank = 0;
	//			double kd = 0;
	//			//Dictionary<uint, Rank_Leage> _dic_sameRank = Dic_rankLeague;
	//			foreach (var rank in dic_rank)
	//			{

	//				if (nowScore == 0)
	//				{
	//					nowScore = rank.Value.Exp;
	//					tempScore = nowScore;
	//					sameRank = rank.Value.Ranking;
	//					kd = Convert.ToDouble(rank.Value.KillDeath);
	//				}
	//				else
	//				{
	//					nowScore = rank.Value.Exp;


	//					if (nowScore == tempScore)
	//					{
	//						if (kd <= Convert.ToDouble(rank.Value.KillDeath))
	//							rank.Value.Ranking = sameRank;
	//						else
	//						{
	//							sameRank = rank.Value.Ranking;
	//							kd = Convert.ToDouble(rank.Value.KillDeath);
	//						}

	//					}
	//					else
	//					{
	//						sameRank = rank.Value.Ranking;
	//						kd = Convert.ToDouble(rank.Value.KillDeath);
	//					}
	//					tempScore = nowScore;
	//				}

					

	//				//생성
	//				if (isDoRefresh == false)
	//				{
	//					UIItem_rankingElement _rEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_RANKINGELEMENT, Get_UnitLstContentTr(rankType)) as UIItem_rankingElement;
	//					_rEle.Set_elementInfo(rank.Value,rankType);

	//					_lstElement.Add(_rEle);
	//					_lstIdx++;
	//				}
	//				else//갱신
	//				{
	//					if (_lstElement[_lstIdx] != null)
	//					{
	//						_lstElement[_lstIdx].Set_elementInfo(rank.Value, rankType);
	//						_lstIdx++;
	//					}
						
	//				}

	//				//Debug.Log("COUNT :" + _lstIdx);

	//				//UIItem_rankingElement _rankingEle = UI_Manager.Getsingleton.CreatUI(UIITEM.ITEM_RANKINGELEMENT, Get_UnitLstContentTr(rankType)) as UIItem_rankingElement;

	//				//_rankingEle.Set_elementInfo(rank.Value);

	//				//lst_RankingEle.Add(_rankingEle);

	//				yield return null;
	//			}


	//			//최종적으로 랭킹리스트 dic에 담기
	//			Dic_rankingEleLst[rankType] = _lstElement;

	//			Loadmanager.instance.LoadingUI(false);
	//		}
		
	
	//}

	//element리스트 완벽히 채워졋냐 확인
	bool Chk_CreatedElementLstPerfect(List<UIItem_rankingElement> lstElement)
	{
		if (lstElement == null || lstElement.Count <= 100)
			return false;
		else
			return true;
	}






	

	

	// 모든 element 삭제하자
	private void ClearElement()
	{
		
	
	}


	// 모든 element 삭제하자
	private void ClearElement(List<UIItem_rankingElement> _lst)
	{
		if (Chk_CreatedElementLstPerfect(_lst) == false)
		{

			for (int i = 0; i < _lst.Count; i++)
			{
				Destroy(_lst[i].gameObject);
			}

			_lst.Clear();
		}
	}



	//내정보 없는걸로 표시 
	void ClearMyRankingUI()
	{
		//랭크
		text_Ranking.text = "-";
		//클랜마크
		image_ClanMark.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, 0));
		//이름
		text_userName.text = "-";
		//exp
		text_Exp.text = "-";
		//킬뎃
		text_KD.text = "-";
        //lv
        text_Lv.text = "-";
	}



    public void ResponseButton_Page(int operIdx)
    {
        if (operIdx == 0) //내림
            nowPage--;
        else if (operIdx == 1) //올림
            nowPage++;

        if (nowPage < 1)
            nowPage = 1;
        else if (nowPage > MaxPage)
            nowPage =MaxPage;

       


        Set_RankElemets();

        Set_RankingPage();

    }
	



	public void ResponseButton_Back()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
	}

	

}



