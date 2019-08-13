using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class UI_Friend : UI_Base
{

	private List<UIItem_FriendElement> lst_friendEle = new List<UIItem_FriendElement>();
	private Dictionary<uint, User_Friends> Dic_userFriends = new Dictionary<uint, User_Friends>();

	public RectTransform RectTr_content;

	private UI_Manager ui_Mgr;
	private User user;

	private FRIENDSORTING_TYPE nowSortType =	FRIENDSORTING_TYPE.ADD;

	//UI
	public Text text_FriendNum;



	private static UI_Friend _instance;
	public static UI_Friend Getinstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UI_Friend)) as UI_Friend;

				if (_instance == null)
				{
					GameObject newObj = new GameObject("UI_Friend");
					newObj.AddComponent<UI_Friend>();
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
		ui_Mgr = UI_Manager.Getsingleton;
		user = UserDataManager.instance.user;

		Chk_FriendList();
	}

	public override void set_Close()
	{
		base.set_Close();

		//나갈떄 element 담긴 컬렉션들 다 삭제하자
		ClearElement();

	}

	public override void set_refresh()
	{
		base.set_refresh();

		//다시 친구리스트 받기
		Chk_FriendList();
	}

	void Chk_FriendList()
	{
			webRequest.FriendList(Set_Friends);
	}

	void Set_DicuserFriends()
	{
		Dic_userFriends = user.User_Friends;
	}

	void Set_Friends()
	{
		//이전 element들삭제
		ClearElement();

		//친구들정보 저장
		Set_DicuserFriends();

		//element 생성
		Creat_friendElement();


		//친구 수
		text_FriendNum.text = string.Format("{0}/50", Dic_userFriends.Count);
	}

	// 친구UI안의 element들 생성
	public void Creat_friendElement()
	{
		if (Dic_userFriends.Count > 0)
		{
			foreach (var fr in Dic_userFriends)
			{
				//ui_Mgr.CreatUI(UIITEM.ITEM_FRIENDELEMENT, RectTr_content);
				//UIItem_FriendElement _friendEle = ui_Mgr.Lst_UiItem[ui_Mgr.Lst_UiItem.Count - 1] as UIItem_FriendElement;
				UIItem_FriendElement _friendEle = ui_Mgr.CreatUI(UIITEM.ITEM_FRIENDELEMENT, RectTr_content) as UIItem_FriendElement; 
				_friendEle.Set_friendInfo(fr.Value);

				lst_friendEle.Add(_friendEle);
			}

			//시간정렬 후 접속중 정렬
			Sort_Friends(lst_friendEle, FRIENDSORTING_TYPE.LOGIN);

		}

	}



	public void ClearElement()
	{
		for (int i = 0; i < lst_friendEle.Count; i++ )
		{
			Destroy(lst_friendEle[i].gameObject);
		}
		lst_friendEle.Clear();
		//ui_Mgr.ClearUI_Item(UI.FRIEND);
	}

	void Sort_Friends(List<UIItem_FriendElement> lst_orinFriend , FRIENDSORTING_TYPE sortType)
	{
		List<UIItem_FriendElement> _lst_Orinsort = new List<UIItem_FriendElement>();
		_lst_Orinsort = lst_orinFriend;
		List<UIItem_FriendElement> _lst_newsort = null;

		if (sortType == FRIENDSORTING_TYPE.ADD) // 추가순
		{
			var sortName = from fri in _lst_Orinsort
						   orderby fri.userFriend.FrIdx descending
						   select fri;
			_lst_newsort = sortName.ToList();
		}
		else if (sortType == FRIENDSORTING_TYPE.NAME) //이름순
		{
			var sortName = from fri in _lst_Orinsort
						   orderby fri.userFriend.NkNm descending
						   select fri;
			_lst_newsort = sortName.ToList();
		}
		else if (sortType == FRIENDSORTING_TYPE.LOGIN) // 로그인순
		{
			var sortName = from fri in _lst_Orinsort
						   orderby fri.userFriend.mtime
						   orderby fri.IsLogining
						   select fri;
			_lst_newsort = sortName.ToList();
		}
		else if (sortType == FRIENDSORTING_TYPE.LOGINTIME) // 로그인순
		{
			var sortName = from fri in _lst_Orinsort
						   orderby fri.userFriend.mtime
						   select fri;
			_lst_newsort = sortName.ToList();
		}



			for (int i = 0; i < _lst_newsort.Count; i++)
			{
				if (sortType == FRIENDSORTING_TYPE.LOGIN )
					_lst_newsort[i].transform.SetAsFirstSibling();
				else
				// 정렬을위해 하이라키 순서 마지막으로 보내기
					_lst_newsort[i].transform.SetAsLastSibling();
			}
	}


	// sorting 버튼 누를때 반응 매서드
	public void ResponseButton_sorting(int sortType)
	{

		FRIENDSORTING_TYPE _sortType = (FRIENDSORTING_TYPE)sortType;

		if (nowSortType != _sortType)
		{
			nowSortType = _sortType;
			Sort_Friends(lst_friendEle, nowSortType);

		}
	}


	//친구 찾기
	public void ResposenButton_FindFriend()
	{
		UI_Popup_Find popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Find>(UIPOPUP.POPUPFIND);
		popup.Set_FindPopup(FindID_Type.FRIEND);
	}

#region 친구초대

	public void ResponseButton_FriendInvite()
	{
		webRequest.FriendGetInviteUrl(callback_ShowNativeShare);
	}

	void callback_ShowNativeShare()
	{

	}
#endregion


	public void ResponseButton_Back()
	{
		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);
	}

}
