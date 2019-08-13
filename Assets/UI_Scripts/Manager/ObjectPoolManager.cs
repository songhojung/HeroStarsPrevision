using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour 
{

	private ObjectPool<UI_Base> Op_ObjectUI;
	private ObjectPool<Transform> Op_ObjectLobbyUnit;
    private Dictionary<string, ObjectPool<Transform>> pool_Units = new Dictionary<string, ObjectPool<Transform>>();


    //
    Transform allocateLobbyUnit;

	private static ObjectPoolManager _instance;
	public static ObjectPoolManager Getinstance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(ObjectPoolManager)) as ObjectPoolManager;

				if (_instance == null)
				{
					GameObject newObj = new GameObject("ObjectPoolManager");
					_instance = newObj.AddComponent<ObjectPoolManager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}


	public void Awake()
	{
		Init_Allocate_UIObject();
	}


	void Init_Allocate_UIObject()
	{
		Op_ObjectUI = new ObjectPool<UI_Base>(this.transform);
		string name = UI.TITLE.ToString();
		GameObject obj = Resources.Load(string.Format("Prefebs/UI_{0}", name)) as GameObject;
		Op_ObjectUI.Allocate(name, obj.GetComponent<UI_Base>());
		UserEditor.Getsingleton.EditLog(name + " UI 오브젝트 로드 끝 !@#");

		string name1 = UI.LOADING.ToString();
		GameObject obj1 = Resources.Load(string.Format("Prefebs/{0}", name1)) as GameObject;
		Op_ObjectUI.Allocate(name1, obj1.GetComponent<UI_Base>());
		UserEditor.Getsingleton.EditLog(name1 + " UI 오브젝트 로드 끝 !@#");
	}


	public IEnumerator Allocate_UI_Objects()
	{
		UI[] uis = new UI[] { UI.LOBBY, UI.STORE, UI.NEWS, UI.CLAN, UI.RANKING, UI.FRIEND, UI.SETTING, UI.CHAT, UI.TOP, UI.EQUIPMENT, UI.CUSTOMROOM, UI.INGAME_RESULT };

		UIITEM[] uiItems = new UIITEM[] { UIITEM.ITEM_EQUIPITEM, UIITEM.ITEM_CLANMEMBERELEMENT, UIITEM.ITEM_CLANBOARDELEMENT, UIITEM.ITEM_CLANLISTELEMENT
		,UIITEM.ITEM_FRIENDELEMENT,UIITEM.ITEM_GAMEINVITEELEMENT,UIITEM.ITEM_RANKINGELEMENT,UIITEM.ITEM_NEWSELEMENT,UIITEM.ITEM_STOREELEMENT,UIITEM.ITEM_CHATELEMENT};

		for (int i = 0; i < uis.Length; i++)
		{
            //로비 프리팹 크기가 이제 크지않으니 씬에 안꺼내놓았다
			//if (uis[i] == UI.LOBBY || uis[i] == UI.SETTING)
			//{
			//	string name = uis[i].ToString();
			//	string findName = string.Empty;
			//	if (uis[i] == UI.LOBBY) findName = "UI_Lobby";
			//	else if (uis[i] == UI.SETTING) findName = "UI_Setting";

			//	GameObject obj1 = GameObject.Find(findName);
			//	//obj1.GetComponent<RectTransform>().offsetMin = new Vector2(590f, 350f);
			//	//obj1.GetComponent<RectTransform>().offsetMax = new Vector2(-590f, -350f);
			//	UI_Base lobbyBase = obj1.GetComponent<UI_Base>();
			//	Op_ObjectUI.AllocateDirect(name, lobbyBase);
			//	//obj1.SetActive(false);
			//	UserEditor.Getsingleton.EditLog(name + " UI 오브젝트 로드 끝 !@#");
			//}
			//else
			{

				ResourceRequest Rq = new ResourceRequest();
				string name = uis[i].ToString();
				Rq = Resources.LoadAsync(string.Format("Prefebs/UI_{0}", name));

				yield return Rq;
				GameObject obj = Rq.asset as GameObject;
				Op_ObjectUI.Allocate(name, obj.GetComponent<UI_Base>());
				UserEditor.Getsingleton.EditLog(name + " UI 오브젝트 로드 끝 !@#");

			}
		}

		//for (int i = 0; i < uiItems.Length; i++ )
		//{
		//    ResourceRequest Rq = new ResourceRequest();
		//    string name = uiItems[i].ToString();
		//    Rq = Resources.LoadAsync(string.Format("Prefebs/UI_{0}", name));

		//    yield return Rq;
		//    GameObject obj = Rq.asset as GameObject;
		//    Op_ObjectUI.Allocate(name, obj.GetComponent<UI_Base>());
		//    UserEditor.Getsingleton.EditLog(name + " UIITEM 오브젝트 로드 끝 !@#");
		//}
	}

	// UI 오브젝트 얻기
	public UI_Base Get_ObjectUI(string uiName)
	{
		UI_Base bases = null;
		if (Op_ObjectUI.IsContain(uiName))
		{
			bases = Op_ObjectUI.Get_Object(uiName);
		}
		else
		{
			GameObject obj1 = Resources.Load(string.Format("Prefebs/UI_{0}", uiName)) as GameObject;
			Op_ObjectUI.Allocate(uiName, obj1.GetComponent<UI_Base>());
			bases = Op_ObjectUI.Get_Object(uiName);
		}
		return bases;
	}

	// 안쓰는 UI 오브젝트 잠그기(릴리즈하기)
	public void Release_ObjectUI(string uiName)
	{
		Op_ObjectUI.Release_Object(uiName);
	}

	// UI 오브젝트 캔버스붙일떄 transform 설정하기 
	public void SetTransformParent_ObjectUI(string uiName, Transform tr)
	{
		Op_ObjectUI.SetTransfrom_Object(uiName, tr);
	}


	//public IEnumerator Allocate_LobbyUnitObject()
	//{
	//    //Dictionary<int, infos_unit> dic_infosUnit = TableDataManager.instance.Infos_units;
	//    string[] unitnames = new string[] { "10001", "10002", "10003", "10004", "10005", "10006", "10007" };
	//    Loadmanager.instance.LoadingUI(true);
	//    for (int i = 0; i < unitnames.Length; i++ )
	//    {

	//        if (Op_ObjectLobbyUnit == null)
	//        {
	//            Op_ObjectLobbyUnit = new ObjectPool<Transform>(this.transform);
	//        }

	//        ResourceRequest Rq = new ResourceRequest();
	//        Rq = Resources.LoadAsync(string.Format("Prefebs/Lobbych/Lobbych_{0}", unitnames[i]));

	//        yield return Rq;
	//        GameObject obj = Rq.asset as GameObject;
	//        Op_ObjectLobbyUnit.Allocate(unitnames[i], obj.GetComponent<Transform>());
	//        UserEditor.Getsingleton.EditLog(unitnames[i] + "로비유닛 로드 끝 !@#");


	//    }
	//    Loadmanager.instance.LoadingUI(false);
	//}

	//// 로비 유닛오브젝트 얻기
	//public Transform Get_ObjectLobbyUnit(string name)
	//{
	//    return Op_ObjectLobbyUnit.Get_Object(name);
	//}
	public void Allocate_Character(string name, Transform tr)
	{
		if (Op_ObjectLobbyUnit == null)
			Op_ObjectLobbyUnit = new ObjectPool<Transform>(this.transform);

		Op_ObjectLobbyUnit.Allocate(name, tr.GetComponent<Transform>());
	}

	

	// 로비 유닛오브젝트 얻기
	public Transform Get_ObjectLobbyUnit(string name)
	{
		Transform tr = null;
      

        if(pool_Units.ContainsKey(name))
        {
            tr = pool_Units[name].Get_Object();

            if( tr == null)
            {
                pool_Units[name].Allocate(1);
                tr = pool_Units[name].Get_Object();

            }
        }
        else
        {
            GameObject _obj = Resources.Load("Prefebs/Lobbych/Lobbych_") as GameObject;
            ObjectPool<Transform> op_object = new ObjectPool<Transform>(_obj.transform, this.gameObject.transform, name);

            op_object.Allocate(1);

            pool_Units[name] = op_object;
            tr = pool_Units[name].Get_Object();
        }

        return tr;
	}

	// 안쓰는 로비캐릭터 오브젝트 잠그기(릴리즈하기)
	public void Release_ObjectLobbyUnit(Transform ObjectTr)
	{
		Op_ObjectLobbyUnit.Release_Object(ObjectTr);
	}


	public bool IsContainUI(string name)
	{
		return Op_ObjectUI.IsContain(name);
	}
}
