using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.EventSystems;
using System;
public partial class UI_Manager : MonoBehaviour
{

    private static UI_Manager _instance;
    public static UI_Manager Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(UI_Manager)) as UI_Manager;
				if (_instance == null)
				{
					_instance = new GameObject("UI_Manager").AddComponent<UI_Manager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
            }
            return _instance;
        }
    }

	private EventSystem evntSystem;
    // : UI에붙일 transform
    public Canvas canvas;
    public Transform CanvasTr;
	public Transform parentPopup_Tr;
	public Transform parentToastPopup_Tr;


    public UI _UI = UI.TITLE;
	public UI goBackUI = UI.NONE;		//이전 UI로 돌아갈떄 필요한 변수
	public Dictionary<UI, GameObject> Dic_UI_SaveObject = new Dictionary<UI, GameObject>();
	public Dictionary<UI, GameObject> Dic_UI_Object = new Dictionary<UI, GameObject>();
	public Dictionary<UI, UI_Base> Dic_UILst = new Dictionary<UI, UI_Base>();
	public List<UI_Base> Lst_UiItem = new List<UI_Base>();


    public UIData staticUIData = null;      //외부에서 할당된 UI data, 보통 씬전환시에 사용함 (ex. result ui => lobby)
	public bool isIPhoneXDesplay = false;					// 아이폰 X를 위한 UI냐 ?



	
	void Awake()
	{
		// 초기화
		Init();

		
	}

	public void Init()
	{
		_UI = UI.TITLE;
		goBackUI = UI.NONE;
		List<UI> _lst_UI = new List<UI>(); ;
		foreach (var Ui in Dic_UILst)
		{
			_lst_UI.Add(Ui.Key);
		}

		for (int i = 0; i < _lst_UI.Count; i++)
			ClearUI(_lst_UI[i]);

		Dic_UILst.Clear();
		ClearPopupUI();
	}


 
	public void StartUI()
	{
		UserEditor.Getsingleton.EditLog("UI_manager start UI");

		if (CanvasTr == null)
		{
			Find_UICanvasTr();
		}


		if (_UI != UI.TITLE && !Dic_UILst.ContainsKey(UI.TOP))
		{
			CreatUI(UI.TOP, CanvasTr);
		}

		CreatUI(_UI, CanvasTr, staticUIData);



	}

	

    // : UI 생성
    public void CreatUI(UI _uiScene,Transform _parentTr,UIData uData = null)
    {
		if (_uiScene != UI.TOP)
		{


			//if (_uiScene == UI.STORE || _uiScene == UI.CHAT)
				goBackUI = _UI;

			_UI = _uiScene;

		}
        switch(_uiScene)
        {


			case UI.TITLE:
				if (Dic_UILst.ContainsKey(UI.SETTING))
					ClearUI(UI.SETTING);
				if (Dic_UILst.ContainsKey(UI.TOP))
					ClearUI(UI.TOP);
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				InstantiateUI(_uiScene, _parentTr, uData);
				break;

            case UI.TOP:
                InstantiateUI(_uiScene, _parentTr, uData);
                break;

            case UI.LOBBY:
				if (Dic_UILst.ContainsKey(UI.TITLE))
					ClearUI(UI.TITLE);
				if (Dic_UILst.ContainsKey(UI.EQUIPMENT))
					ClearUI(UI.EQUIPMENT);
				if (Dic_UILst.ContainsKey(UI.CUSTOMROOM))
					ClearUI(UI.CUSTOMROOM);
					//DeActiveUI(UI.FORMATION);
				if (Dic_UILst.ContainsKey(UI.CLAN))
					ClearUI(UI.CLAN);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				if (Dic_UILst.ContainsKey(UI.SETTING))
					ClearUI(UI.SETTING);
				if (Dic_UILst.ContainsKey(UI.RANKING))
					ClearUI(UI.RANKING);
				if (Dic_UILst.ContainsKey(UI.FRIEND))
					ClearUI(UI.FRIEND);
				if (Dic_UILst.ContainsKey(UI.NEWS))
					ClearUI(UI.NEWS);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
                if (Dic_UILst.ContainsKey(UI.CHARACTERSETTING))
                    ClearUI(UI.CHARACTERSETTING);
                InstantiateUI(_uiScene, _parentTr, uData);
				//ActiveUI(UI.LOBBY);
                break;

			case UI.CUSTOMROOM:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.TOP))
					ClearUI(UI.TOP);
				if (Dic_UILst.ContainsKey(UI.FRIEND))
					ClearUI(UI.FRIEND);
				if (Dic_UILst.ContainsKey(UI.CLAN))
					ClearUI(UI.CLAN);
				InstantiateUI(_uiScene, _parentTr, uData);
				break;

			case UI.MATCHING:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.TOP))
					ClearUI(UI.TOP);
					//DeActiveUI(UI.FORMATION);
				if (Dic_UILst.ContainsKey(UI.CLAN))
					ClearUI(UI.CLAN);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				if (Dic_UILst.ContainsKey(UI.SETTING))
					ClearUI(UI.SETTING);
				if (Dic_UILst.ContainsKey(UI.RANKING))
					ClearUI(UI.RANKING);
				if (Dic_UILst.ContainsKey(UI.FRIEND))
					ClearUI(UI.FRIEND);
				if (Dic_UILst.ContainsKey(UI.NEWS))
					ClearUI(UI.NEWS);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				InstantiateUI(_uiScene, _parentTr, uData);
				break;


			case UI.CLAN:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			case UI.STORE:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.CLAN))
					ClearUI(UI.CLAN);
				if (Dic_UILst.ContainsKey(UI.RANKING))
					ClearUI(UI.RANKING);
				if (Dic_UILst.ContainsKey(UI.FRIEND))
					ClearUI(UI.FRIEND);
				if (Dic_UILst.ContainsKey(UI.NEWS))
					ClearUI(UI.NEWS);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				InstantiateUI(_uiScene, _parentTr, uData);
					break;

			case UI.SETTING:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			case UI.RANKING:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			case UI.FRIEND:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			case UI.NEWS:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.CHAT))
					ClearUI(UI.CHAT);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			


			case UI.CHAT:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
				ClearUI(UI.LOBBY);
				if (Dic_UILst.ContainsKey(UI.TOP))
					ClearUI(UI.TOP);
				if (Dic_UILst.ContainsKey(UI.CLAN))
					ClearUI(UI.CLAN);
				if (Dic_UILst.ContainsKey(UI.STORE))
					ClearUI(UI.STORE);
				if (Dic_UILst.ContainsKey(UI.SETTING))
					ClearUI(UI.SETTING);
				if (Dic_UILst.ContainsKey(UI.RANKING))
					ClearUI(UI.RANKING);
				if (Dic_UILst.ContainsKey(UI.FRIEND))
					ClearUI(UI.FRIEND);
				if (Dic_UILst.ContainsKey(UI.NEWS))
					ClearUI(UI.NEWS);
				if (Dic_UILst.ContainsKey(UI.MATCHING))
					ClearUI(UI.MATCHING);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;
			case UI.EQUIPMENT:
				if (Dic_UILst.ContainsKey(UI.LOBBY))
					ClearUI(UI.LOBBY);
                if (Dic_UILst.ContainsKey(UI.CHARACTERSETTING))
                    ClearUI(UI.CHARACTERSETTING);
                InstantiateUI(_uiScene, _parentTr, uData);
				break;
            case UI.CHARACTERSETTING:
                if (Dic_UILst.ContainsKey(UI.LOBBY))
                    ClearUI(UI.LOBBY);
                if (Dic_UILst.ContainsKey(UI.EQUIPMENT))
                    ClearUI(UI.EQUIPMENT);
                InstantiateUI(_uiScene, _parentTr, uData);
                break;

            case UI.INGAME_MAIN:
				ClearUI(UI.LOBBY);
				ClearUI(UI.TOP);
				InstantiateUI(_uiScene, _parentTr, uData);

				break;

			case UI.INGAME_RESULT:
				InstantiateUI(_uiScene, _parentTr, uData);
				
				break;

        }

		//하이라키에 UI 순서 정하기
		UI_Odering();
 

        //씬바꼇으니 Language 설정 해주기
       // LanguageManager _languager = LanguageManager.Getsingleton;
       // _languager.SetLanguage(_languager.language);

    }




	//: UI uinit 생성
	public UI_Base CreatUI(UIITEM _uiItem, Transform _parentTr)
	{
		UI_Base _uibase = null ;
		switch (_uiItem)
		{
			
			case UIITEM.ITEM_COMBATINFO:
			case UIITEM.ITEM_LEAGUEUNIT:
			case UIITEM.ITEM_UNIT:
			case UIITEM.ITEM_CLANMEMBERELEMENT:
			case UIITEM.ITEM_CLANBOARDELEMENT:
			case UIITEM.ITEM_RANKINGELEMENT:
			case UIITEM.ITEM_FRIENDELEMENT:
			case UIITEM.ITEM_NEWSELEMENT:
			case UIITEM.ITEM_GAINITEM:
			case UIITEM.ITEM_STOREELEMENT:
			case UIITEM.ITEM_STOREPACKAGEELEMENT:
			case UIITEM.ITEM_CLANMARKLIST:
			case UIITEM.ITEM_QUESTELEMENT:
			case UIITEM.ITEM_CHATELEMENT:
			case UIITEM.ITEM_EQUIPITEM:
			case UIITEM.ITEM_CLANLISTELEMENT:
			case UIITEM.ITEM_GAMEINVITEELEMENT:
			case UIITEM.ITEM_RESULTUNITSLOT:
				_uibase =InstantiateUI(_uiItem, _parentTr);
				break;

			
		}

		return _uibase;
	}


    //  : UI 삭제
    public void ClearUI(UI _ui)
    {
        
#if UNITY_EDITOR
            if (!Dic_UILst.ContainsKey(_ui))
            {
                Debug.LogError("ERROR ClearUI IS NULL UI : " + _ui);
            }
#endif
            Dic_UILst[_ui].set_Close();

			Dic_UILst.Remove(_ui);
			ObjectPoolManager.Getinstance.Release_ObjectUI(_ui.ToString());

			UserEditor.Getsingleton.EditLog("Clear UI " + _ui);
        
    }

	public void ClearALL_UI()
	{
		foreach (var ui in Dic_UILst)
		{
			ui.Value.set_Close();

			ObjectPoolManager.Getinstance.Release_ObjectUI(ui.Key.ToString());

			UserEditor.Getsingleton.EditLog("Clear UI " + ui.Key);
		}

		Dic_UILst.Clear();

	}




	// uiitem 전체 삭제
	public void ClearUI_Item(UI _ui)
	{
		

		int itemCount = Lst_UiItem.Count;
		for (int i = 0; i < itemCount; i++)
		{
			Lst_UiItem[i].set_Close();

			Destroy(Lst_UiItem[i].gameObject);
		}
		Lst_UiItem.Clear();
		
	}

	// uiitem 하나만 삭제
	public void RemoveUI_Item(UI_Base uiItem)
	{
		int idx = Lst_UiItem.IndexOf(uiItem);
		

		Destroy(Lst_UiItem[idx].gameObject);

		Lst_UiItem.Remove(uiItem);

	}


    public void InstantiateUI(UI _ui, Transform _parentTr, UIData uData = null)
    {
			GameObject CloneUI = ObjectPoolManager.Getinstance.Get_ObjectUI(_ui.ToString()).gameObject;
	
			ObjectPoolManager.Getinstance.SetTransformParent_ObjectUI(_ui.ToString(),CanvasTr);

			UI_Base _uibase = CloneUI.GetComponent<UI_Base>();

        if (uData != null)
            _uibase.Set_BaseData(uData);

        if (!Dic_UI_SaveObject.ContainsKey(_ui))
			{
				Dic_UI_SaveObject[_ui] = CloneUI;

				Dic_UILst.Add(_ui, _uibase);

				Dic_UILst[_ui].set_Open();
			}
			else
			{
				Dic_UILst.Add(_ui, _uibase);
				//Dic_UILst[_ui].set_Open();
				Dic_UILst[_ui].set_refresh();
			}
				UserEditor.Getsingleton.EditLog("complete creat " + _ui);
			

    }// end of function


	public UI_Base InstantiateUI(UIITEM _uiItem, Transform _parentTr)
	{


		GameObject orinUI = ObjectPoolManager.Getinstance.Get_ObjectUI(_uiItem.ToString()).gameObject;
		GameObject CloneUI = Instantiate(orinUI);

		CloneUI.transform.SetParent(_parentTr);

		RectTransform _cloneUIRectTr = CloneUI.GetComponent<RectTransform>();
		RectTransform _newUIRectTr = orinUI.GetComponent<RectTransform>();

		_cloneUIRectTr.sizeDelta = _newUIRectTr.sizeDelta;
		_cloneUIRectTr.anchoredPosition = _newUIRectTr.anchoredPosition;
		_cloneUIRectTr.localScale = _newUIRectTr.localScale;

		UI_Base _uibase = CloneUI.GetComponent<UI_Base>();

		//Lst_UiItem.Add(_uibase);

		_uibase.set_Open();


		return _uibase;
		//Dic_UIunit.Add(_uiunit, _uibase);

		//Dic_UIunit[_uiunit].set_Open();

	}// end of function



	void UI_Odering()
	{

		if (Dic_UILst.ContainsKey(UI.TOP))
		{
			Dic_UILst[UI.TOP].transform.SetAsLastSibling();
		}
	}






	//public void Load_UIScene(string sceneName)
	//{
	//	StartCoroutine(Loadmanager.instance.LoadScene(sceneName));
	//}

	public void Find_UICanvasTr()
	{
		//버튼 드레그 값 조정
		evntSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
		evntSystem.pixelDragThreshold = (int)(0.5f * Screen.dpi / 2.54f);

        GameObject CanvasObj = GameObject.Find("Canvas");
        //canvas = FindObjectOfType(typeof(Canvas)) as Canvas;
        canvas = CanvasObj.GetComponent<Canvas>();

        Scene _scene = SceneManager.GetActiveScene();
		if (_scene.name == DefineKey.Main)
		{
			CanvasTr = GameObject.Find("1_UI").GetComponent<Transform>();
			parentPopup_Tr = GameObject.Find("2_Popup").GetComponent<Transform>();
			parentToastPopup_Tr = GameObject.Find("3_ToastPopup").GetComponent<Transform>();


			//IPhone X 화면 대응
			UserEditor.Getsingleton.EditLog("Screen.width : " + Screen.width + "Screen.height : " + Screen.height);
			if ((Screen.width == 2436 && Screen.height == 1125))
			{
				float _aspect = ((float)Screen.height / (float)Screen.width);
				//_aspect = 0.45f;
				UserEditor.Getsingleton.EditLog("_aspect : " + _aspect);
				if (_aspect < 0.5f)
				{
					UserEditor.Getsingleton.EditLog ("아이폰X 화면이다 ");
					//아이폰X 화면이다 
					isIPhoneXDesplay = true;

					CanvasTr.transform.localScale = new Vector3(0.95f, 0.95f, 1);
					parentPopup_Tr.transform.localScale = new Vector3(0.95f, 0.95f, 1);
					parentToastPopup_Tr.transform.localScale = new Vector3(0.95f, 0.95f, 1);
				}
				else
				{
					//아이폰X 화면 아니다 
					isIPhoneXDesplay = false;
				}
			}
			else
			{
				//아이폰X 화면 아니다 
				isIPhoneXDesplay = false;	
			}


		
		}
		else
		{
			//Canvas _canvas = FindObjectOfType(typeof(Canvas)) as Canvas;
            GameObject CanvasObj1 = GameObject.Find("Canvas");
            //canvas = FindObjectOfType(typeof(Canvas)) as Canvas;
            Canvas _canvas = CanvasObj1.GetComponent<Canvas>();
            CanvasTr = _canvas.transform;



			//IPhone X 화면 대응
			UserEditor.Getsingleton.EditLog("Screen.width : " + Screen.width + "Screen.height : " + Screen.height);
			if ((Screen.width == 2436 && Screen.height == 1125))
			{
				float _aspect = ((float)Screen.height / (float)Screen.width);
				UserEditor.Getsingleton.EditLog("_aspect : " + _aspect);
				if (_aspect < 0.5f)
				{
					//아이폰X 화면이다 
					isIPhoneXDesplay = true;

					GameObject _IngamePlayUIobj = GameObject.Find("GamePlay_UI_Set");
					if (_IngamePlayUIobj != null)
						_IngamePlayUIobj.transform.localScale = new Vector3(0.95f, 0.95f, 1);
				}
				else
				{
					//아이폰X 화면 아니다 
					isIPhoneXDesplay = false;
				}
			}
			else
			{
				//아이폰X 화면 아니다 
				isIPhoneXDesplay = false;
			}


		}

		
	
	}

	public void destroyUI(GameObject obj)
	{
		Destroy(obj);

	}

	public void StartloadingRoutine(IEnumerator name)
	{
		StartCoroutine(name);
	}
	public void StoploadingRoutine()
	{
		//StopCoroutine(name);
		StopAllCoroutines();
	}



	/// <summary>
	/// 보상,보급등으로 얻은 아이템들을 UI로 생성시켜준다.
	/// </summary>
	public UIItem_GainItem GetCreat_GainItemUI(Transform targetTr, int gainItemIdx)
	{
		UIItem_GainItem gainItem;

		 List<GainItem> _lst_gainItem = webResponse.GetResultInfoList;
		 GainItem _gain = _lst_gainItem[gainItemIdx];
		 if (_gain.ItTp == ITEMTYPE.GEM)
		 {
			 return null;
		 }




		 gainItem = CreatUI(UIITEM.ITEM_GAINITEM, targetTr) as UIItem_GainItem;
		 gainItem.Set_GainItemInfo(_gain);
		

		 return gainItem;
	}

	bool findgainItem(UI_Base a)
	{
		if (a != null)
			return true;
		else
			return false;
	}



	void Update()
	{

		//백키 에대한 업데이트 처리
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Scene _scene = SceneManager.GetActiveScene();
			if (_scene.name == DefineKey.Main) //메인씬일때만
			{
				if (UIPopupList.Count > 0 && UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Toast)
				&& UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Showing)
				&& UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Platform)
					&& UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Roulette))
				{

					if (!Dic_UILst.ContainsKey(UI.TITLE)) //(타이틀) 쪽에 나타나는 모든 팝업창은 백키 기능 작동안함
					{
						//// 선택팝업창인데 로비UI 이면 리더선택기능할수 있게 false 하자
						//if(UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_Selective) ||
						//    UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_Notice)||
						//    UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_Attendace)||
						//    UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_Reinforce)||
						//    UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_RfProcess))
						//{
						//    if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
						//    {
						//        User.isSelectedCharacter = false;
						//    }
						//    ClearUI(UIPopupList[UIPopupList.Count - 1]);
						//}
						//else
						//    ClearUI(UIPopupList[UIPopupList.Count - 1]);

						process_BackPopupUI(UIPopupList[UIPopupList.Count - 1]);
					}
				
				}
				else
				{
					if (Loadmanager.instance.activeLoading) //로딩바 무시
						return;		
					else if (UI_Top.Getsingleton.popupStore.gameObject.activeSelf) // 스토어팝업 체크
						UI_Top.Getsingleton.popupStore.ResponseButton_Close();
					else if ( Dic_UILst.ContainsKey(UI.SETTING)&&UI_Setting.Getinstance.popupCoopon.gameObject.activeSelf) // 쿠폰팝업 체크
						UI_Setting.Getinstance.popupCoopon.gameObject.SetActive(false);
					else if (UIPopupList.Count <= 0)
					{
						process_BackUI(_UI);
					}
					
				}
			}
			else
			{

				if (UIPopupList.Count > 0 && UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Toast)
				&& UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Showing)
				&& UIPopupList[UIPopupList.Count - 1].GetType() != typeof(UI_Popup_Platform))
				{
					if (UIPopupList[UIPopupList.Count - 1].GetType() == typeof(UI_Popup_Selective))
					{
						//UI_Popup_Selective selectpopup = UIPopupList[UIPopupList.Count - 1] as UI_Popup_Selective;
						//selectpopup.ResponseButton_No();
						//선택팝업창을 닫습니다
					}
				}
				else
				{
					// 배틀씬 일떄 백키 기능
					process_BackUI(_UI);
				}
				
			}
			
		}


	}

	//백 버튼 할떄 행해질 것들
	void process_BackUI(UI _nowUI)
	{
		switch (_nowUI)
		{
			case UI.LOBBY:
				UI_Popup_Selective popup = CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[289]);
				popup.SetPopupMessage(TextDataManager.Dic_TranslateText[290]);
				popup.Set_addEventButton(Application.Quit);
				break;
			case UI.CLAN:
				Dic_UILst[UI.CLAN].GetComponent<UI_Clan>().ResponseButton_Back();
				break;
			case UI.RANKING:
				Dic_UILst[UI.RANKING].GetComponent<UI_Ranking>().ResponseButton_Back();
				break;
			case UI.FRIEND:
				Dic_UILst[UI.FRIEND].GetComponent<UI_Friend>().ResponseButton_Back();
				break;
			case UI.NEWS:
				Dic_UILst[UI.NEWS].GetComponent<UI_News>().ResponseButton_Back();
				break;
			case UI.SETTING:
				Dic_UILst[UI.SETTING].GetComponent<UI_Setting>().ResponseButton_Back();
				break;
			case	UI.EQUIPMENT:
				Dic_UILst[UI.EQUIPMENT].GetComponent<UI_Equipment>().ResponseButton_Back();
				break;
			case UI.CUSTOMROOM:
				Dic_UILst[UI.CUSTOMROOM].GetComponent<UI_CustomRoom>().ResponseButton_Back();
				break;
            case UI.CHARACTERSETTING:
                Dic_UILst[UI.CHARACTERSETTING].GetComponent<UI_CharacterSetting>().ResponseButton_Back();
                break;
			case UI.INGAME_RESULT:
				Dic_UILst[UI.INGAME_RESULT].GetComponent<UI_Ingame_result>().ResponseButton_OK();
				break;
			
		}
	}


	void process_BackPopupUI(UI_PopupBase popupBase)
	{
		Type popupType = popupBase.GetType();

		if (popupType == typeof(UI_Popup_Selective))
		{
			popupBase.GetComponent<UI_Popup_Selective>().ResponseButton_No() ;
			
		}
		else if (popupType == typeof(UI_Popup_Notice))
		{
			popupBase.GetComponent<UI_Popup_Notice>().ResponseButton_Yes();

		}
		else if (popupType == typeof(UI_Popup_Attendace))
		{
			popupBase.GetComponent<UI_Popup_Attendace>().ResponseButton_close();

		}
		else if (popupType == typeof(UI_Popup_Reinforce))
		{
			popupBase.GetComponent<UI_Popup_Reinforce>().ResponseButton_Close();

		}
		else if (popupType == typeof(UI_Popup_RfProcess))
		{
			popupBase.GetComponent<UI_Popup_RfProcess>().ResponseButton_ReinforceEnd();

		}
		else if (popupType == typeof(UI_Popup_StartGame))
		{
			popupBase.GetComponent<UI_Popup_StartGame>().ResponseButton_Close();

		}
		else
		{
			ClearUI(popupBase);
		}
	}
}
