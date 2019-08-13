using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class UI_News : UI_Base 
{
	

	private NewsToggle_Type nowTapType = NewsToggle_Type.WHOLE;		//현재 tap 
	private UI_Manager ui_mgr;
	private List<UIItem_newsElement> lst_newsElemnet = new List<UIItem_newsElement>(); // 생성된 element들
	private List<UIItem_newsElement> _lst_sortNews = new List<UIItem_newsElement>(); //sorting 에 해당하는 element만 저장
	private Dictionary<uint, User_Posts> Dic_userPosts = new Dictionary<uint, User_Posts>(); // 유저가지고 있는 소식들
	private Dictionary<byte, Infos_Notice> Dic_infosNotice = new Dictionary<byte, Infos_Notice>(); // 공지사항 


	public RectTransform RectTr_content;

	public uint Overlapedpost_UserID = 0;  // 소식게시물 처리 이후 중복으로 남아 있는 게시물의 발신자유저ID
	public User_Posts OverlapedPost;


	private static UI_News _instance;
	public static UI_News Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_News)) as UI_News;

				if (_instance == null)
				{
					GameObject instanceObj = new GameObject("UI_News");
					_instance = instanceObj.AddComponent<UI_News>();
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
		ui_mgr = UI_Manager.Getsingleton;

		webRequest.PostGetList(Set_NewsInfo);

	}

	public override void set_Close()
	{
		base.set_Close();
		Debug.Log("post count: " + UserDataManager.instance.user.User_Posts);

		//new ui 닫을떄 news알림표시 체크		
		CheckNewMarkWhenClose();

		ClearElement();

	}

	public override void set_refresh()
	{
		base.set_refresh();

		//클랜초대 경우 중복된 메세지들 삭제한다.


		ClearElement();

		webRequest.PostGetList(Set_NewsInfo);
	}

	//new ui 닫을떄 news알림표시 체크	
	void CheckNewMarkWhenClose()
	{
		int Diffcount = Dic_userPosts.Count - PlayerPrefs.GetInt(DefineKey.PostCount); // 남아잇어야할 게시물 = 현재 총 게시물 - 이전 게시물(삭제된게시물포함)
		int postcount = Dic_userPosts.Count(); // 클랜초대 제외된 게시물갯수
		PlayerPrefs.SetInt(DefineKey.PostCount, postcount - Diffcount); //  클랜초대 제외된 게시물갯수 - 남아잇어야할 게시물
		PlayerPrefs.Save();
		if (PlayerPrefs.HasKey(DefineKey.PostCount))
		{
			int pcount = PlayerPrefs.GetInt(DefineKey.PostCount);
			if (pcount <= 0)
				UserDataManager.instance.user.MarkChanges[MarkTitleType.NewsCount] = false;
			else
				UserDataManager.instance.user.MarkChanges[MarkTitleType.NewsCount] = true;
		}
		else
		{
			//확인하고 표시 안띄우게 false하자
			UserDataManager.instance.user.MarkChanges[MarkTitleType.NewsCount] = false;
		}
		
	}


	public void Set_NewsInfo()
	{
		Dic_userPosts = UserDataManager.instance.user.User_Posts;
		Dic_infosNotice = TableDataManager.instance.Infos_Notices;

		//우편 UI들어왓으니 우편물 확인한것이다. 확인한 우편물 갯수저장
		PlayerPrefs.SetInt(DefineKey.PostCount, Dic_userPosts.Count);
		//PlayerPrefs.SetInt(DefineKey.PostNoticeCount,Dic_infosNotice.Where(n => n.Value.Noticetime > TimeManager.Instance.Get_nowTime()).Count() );
		PlayerPrefs.Save();

		//소식 element 들 생성 
		Creat_Element();

	

		// 분류하기
		Sorting(nowTapType);
	}
	

	// 모든 elememnt 들 생성하기
	public void Creat_Element()
	{
		

		//소식 생성-> 공지는 공지데이터 따로 있음 / 알림및선물 은 선물아이템있으면 선물 ,없으면 알림임 / 초대는 클랜 초대 데이터로 이용
		//알림및선물 구분
		foreach (var item in Dic_userPosts)
		{
			
			UIItem_newsElement _newsEle = ui_mgr.CreatUI(UIITEM.ITEM_NEWSELEMENT, RectTr_content) as UIItem_newsElement;

				_newsEle.Set_ElementInfo(item.Value);
			lst_newsElemnet.Add(_newsEle);
		}

		//공지 구분

		foreach (var noti in Dic_infosNotice)
		{


			if (noti.Value.LggCd == (byte)OptionSetting.instance.usingLangueage) //공지사항의 국가코드가 현재 국가코드랑 같으면
			{
				if (noti.Value.Noticetime > TimeManager.Instance.Get_nowTime())
				{


					UIItem_newsElement _newsEle = ui_mgr.CreatUI(UIITEM.ITEM_NEWSELEMENT, RectTr_content) as UIItem_newsElement;
					_newsEle.Set_ElementInfo(noti.Value);

					lst_newsElemnet.Add(_newsEle);
				}
			}
		}
	}


	//sorting 하기
	public void Sorting(NewsToggle_Type _type)
	{
		int numNews = 0; // 소식 갯수 

	
		if (lst_newsElemnet.Count > 0)
		{
			var sortElements = from ele in lst_newsElemnet
							   orderby ele.ctime descending //최근 생성 시간별로 분류  
							   select ele; 

			_lst_sortNews = sortElements.ToList();
			
			for (int i = 0; i < _lst_sortNews.Count(); i++)
			{
				_lst_sortNews[i].transform.SetSiblingIndex(i);
			}


			if (_type != NewsToggle_Type.WHOLE) // 전체가 아니면
			{

				for (int i = 0; i < _lst_sortNews.Count; i++)
				{
					if (_lst_sortNews[i].toggleType == _type) // 현재 타입이랑 맞으면 active
					{
						_lst_sortNews[i].gameObject.SetActive(true);
						numNews++;
					}
					else // 현재 타입이랑 맞지 않으면 deactive
						_lst_sortNews[i].gameObject.SetActive(false);

				}

			}
			else // 전체면 모두 active
			{
				for (int i = 0; i < _lst_sortNews.Count; i++)
				{
					_lst_sortNews[i].gameObject.SetActive(true);
					numNews++;
				}


			}
		}



	}



	// element 모두 삭제
	public void ClearElement()
	{
		for (int i = 0; i < lst_newsElemnet.Count; i++ )
		{
			Destroy(lst_newsElemnet[i].gameObject);
		}
		lst_newsElemnet.Clear();
		//ui_mgr.ClearUI_Item(UI.NEWS);
	}
	// element 하나만삭제
	public void ClearElement_One(UIItem_newsElement _newElement)
	{
		//우편물 확인하엿고 수령하였으로. 확인한 우편물 갯수저장
		if (PlayerPrefs.HasKey(DefineKey.PostCount))
		{
			PlayerPrefs.SetInt(DefineKey.PostCount, PlayerPrefs.GetInt(DefineKey.PostCount) - 1);


		}
		PlayerPrefs.Save();

		Destroy(_newElement.gameObject);
		lst_newsElemnet.Remove(_newElement);
	}

	/// <summary>
	/// 게시물 처리후 중복된 게시물들 삭제한다.
	/// </summary>
	public void Clear_OverlapedPosts(NEWSTYPE newType)
	{
		//if (newType == NEWSTYPE.INVITE)
		{
			if (Dic_userPosts.Count > 0)
			{

				var overLpInvitePost = from post in Dic_userPosts
									   where (post.Value.SndUserID == OverlapedPost.SndUserID) 
									   && (post.Value.PstPasIdx == 2) && (post.Value.PostIdx != OverlapedPost.PostIdx)// 발신자가 같고 pstIdx 초대형태(2) 이고 중복게시물과 인덱스같지않는것
									   select post;

				int num = overLpInvitePost.Count();

				if (num <= 0)
				{
					set_refresh();
					return;
				}

				int count = 0;
				foreach (var overLpPost in overLpInvitePost) // 중복된것들도 서버에서도 없어지게 프로토콜 쏘자 
				{

					if (count == num - 1)
						webRequest.PostRecv(overLpPost.Value.PostIdx, (uint)overLpPost.Value.ctime.Month, false, set_refresh);
					else
						webRequest.PostRecv(overLpPost.Value.PostIdx, (uint)overLpPost.Value.ctime.Month, false, null);
					count++;
				}
			}
		}
	}



	public void ResponseButton_Tap(int tapIdx)
	{
		if (nowTapType != (NewsToggle_Type)tapIdx)
		{
			nowTapType = (NewsToggle_Type)tapIdx;

			Sorting(nowTapType);
		}
	}



	// back 버튼 누를때 로비로
	public void ResponseButton_Back()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
	}
}
