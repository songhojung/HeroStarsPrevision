using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using System;


/// <summary>
/// 인게임에서  UI 정보 (유저정보 등등) 호출 할때 SendManager 에서 처리하자 
/// </summary>
public class SendManager : MonoBehaviour 
{

	//현재 사용중인 게임서버 정보 반환(IP, 포트)
	public GameServer_Info Get_GameServerInfo()
	{
		return UserDataManager.instance.user.User_userGameServer;
	}

	/// <summary>
	/// 버전 반환
	/// </summary>
	public ushort Get_version()
	{
#if UNITY_EDITOR
		return DefineKey.Release_AOS_Version;
#elif UNITY_ANDROID
		return DefineKey.Release_AOS_Version;
#elif UNITY_IOS
		return DefineKey.Release_IOS_Version;
#endif
	}


	/// <summary>
	///유저 로그인정보 반환
	/// </summary>
	public User_Users Get_userInfo()
	{
		User_Users _userInfo = UserDataManager.instance.user.user_Users;
	
		return _userInfo;
	}



	//유저 재화 정보 반환
	public int Get_userGoods(ITEMTYPE goodsType)
	{
		return UserDataManager.instance.user.Get_user_goods(goodsType);
	}


	//유저 재화 정보 갱신
	public void Set_userGoods(ITEMTYPE goodType, int value)
	{
		User _user = UserDataManager.instance.user;

		if (_user.User_Goods.ContainsKey(goodType))
			_user.User_Goods[goodType].ItNum = value;

	}



	//유저의 게임관련 데이터 반환
	public User_Games Get_UserGames()
	{
		return UserDataManager.instance.user.user_Games;

	
	}


	/// <summary>
	/// 유저 국가코드 반환
	/// </summary>
	public byte[] Get_UserCountryCode()
	{
		UserEditor.Getsingleton.EditLog("Get_UserCountryCode " + UserDataManager.instance.user.user_Users.CtrCd);
		return Encoding.UTF8.GetBytes(UserDataManager.instance.user.user_Users.CtrCd);
	}


	/// <summary>
	/// 유저 문구메세지 반환
	/// </summary>
	public string Get_KillingMessage(USERWORD_TYPE wordType)
	{
		User _user = UserDataManager.instance.user;
		string killMsg = string.Empty;
		if (_user.User_Words.ContainsKey(wordType))
			killMsg = _user.User_Words[wordType].Words;
		else
			killMsg = "";

		return killMsg;
	}

	/// <summary>
	/// 유져 현재 사용중인 캐릭터 인덱스반환
	/// </summary>
	public uint[] Get_useCharacterIdx()
	{
		return UserDataManager.instance.user.User_useUnit.UnitIdxs;
	}


    /// <summary>
    /// 현재 소유한 모든캐릭터들 정보 반환
    /// </summary>
    public Dictionary<uint, User_Units> Get_userCharacters()
	{
		return UserDataManager.instance.user.User_Units;

	}



	/// <summary>
	/// 한 캐릭터가 소유한 모든 무기 데이터반환
	/// </summary>
	//public Dictionary<uint, User_weapon> Get_userWeapons(int unitIdx)
	//{
	//	User _user = UserDataManager.instance.user;

	//	if (_user.User_Weapons.ContainsKey((uint)unitIdx))
	//	{
 //           return null;/*_user.User_Weapons[(uint)unitIdx];*/
	//	}
	//	else
	//	{
	//		Debug.LogError("Please check user Weapon,  this character doesn't have Data");
	//		return null;
	//	}
	//}


    public Dictionary<uint, User_weapon> Get_userWeapons()
    {
        User _user = UserDataManager.instance.user;

        return _user.User_Weapons;
    }




    /// <summary>
    /// 무기의 타입 정보 반환
    /// </summary>
    public WEAPONE_TYPE Get_WeaponType(int weaponIdx)
	{
		WEAPONE_TYPE _wpnType;
		if (TableDataManager.instance.Infos_weapons.ContainsKey((uint)weaponIdx))
			_wpnType = TableDataManager.instance.Infos_weapons[(uint)weaponIdx].WpnType;
		else
			_wpnType = WEAPONE_TYPE.NONE;
		return _wpnType;
	}

	/// <summary>
	/// 모든무기 인덱스를 배열로 반환
	/// </summary>
	public int[] GetAll_WeaponIdx()
	{
		int count = TableDataManager.instance.Infos_weapons.Count;
		int idx = 0;
		int[] itemIdx = new int[count];
		foreach (var item in TableDataManager.instance.Infos_weapons)
		{
			itemIdx[idx] = (int)item.Value.WpnIdx;
			idx++;
		} 

		return itemIdx;
	}


	/// <summary>
	/// 모든치장 인덱스를 배열로 반환
	/// </summary>
	public int[] GetAll_DecoIdx()
	{
		int count = TableDataManager.instance.Infos_Decos.Count;
		int idx = 0;
		int[] decoIdx = new int[count];
		foreach (var item in TableDataManager.instance.Infos_Decos)
		{
			decoIdx[idx] = (int)item.Value.DecoIdx;
			idx++; 
		}

		return decoIdx;
	}


	/// <summary>
	/// 특정 캐릭터인덱스의 코스튬 구성정보 반환
	/// </summary>
	public Infos_Deco Get_DecoInfo(int unitidx)
	{

		if (TableDataManager.instance.Infos_Decos.ContainsKey((uint)unitidx))
			return TableDataManager.instance.Infos_Decos[(uint)unitidx];
		else
		{
			Debug.LogError(" this character doesn't have Deco data, Please Check again");
			return null;
		}

	}



	/// <summary>
	/// 특정 치장(코스튬) 능력치들 반환
	/// </summary>
	public float Get_DecoAbillity(int decoIdx , DECOABL_TYPE _Abillity)
	{

		Array typeArray = Enum.GetValues(typeof(DECOABL_TYPE));

		byte[] abls = new byte[typeArray.Length];

		for (int i = 0; i < typeArray.Length; i++)
		{
			abls[i] = 0;
		}

		if (TableDataManager.instance.Infos_Decos.ContainsKey((uint)decoIdx))
		{
			Infos_Deco _deco = TableDataManager.instance.Infos_Decos[(uint)decoIdx];
			abls[0] = _deco.GrdPwr;
			abls[1] = _deco.RspSpd;
			abls[2] = _deco.SklChgTm;

		}

		return (float)abls[(int)_Abillity];
	}




	/// <summary>
	///구성정보 TableData 반환
	/// </summary>
	public TableDataManager Get_TableData()
	{

		return TableDataManager.instance;
	}



	/// <summary>
	/// 특정캐릭터의 현재강화레벨 대한 능력치 반환
	/// </summary>
	public infos_unit Get_ReinfCharacter(uint unitIdx, byte RefLv)
	{

		infos_unit _unit = new infos_unit(TableDataManager.instance.Infos_units[unitIdx]);
		_unit.Hp = StaticMethod.Get_nextSpec(_unit.Hp, RefLv, true);

		if (RefLv >= 5)
			_unit.MvSpeed = StaticMethod.Get_nextSpec(_unit.MvSpeed, RefLv, false, 0.05f, 4);

		if (RefLv >= 10)
			_unit.RldSpeed = (ushort)StaticMethod.Get_nextSpec(_unit.RldSpeed, RefLv, true, 5f, 9);

		return _unit;
	}



	/// <summary>
	/// 특정무기의 현재강화레벨 대한 능력치 반환
	/// </summary>
	public Infos_Weapon Get_ReinfWeapon(uint weaponIdx, byte RefLv)
	{

		Infos_Weapon _weapon = new Infos_Weapon(TableDataManager.instance.Infos_weapons[weaponIdx]);
		_weapon.AtkMin = (int)StaticMethod.Get_nextSpec(_weapon.AtkMin, RefLv, true);
		_weapon.AtkMax = (int)StaticMethod.Get_nextSpec(_weapon.AtkMax, RefLv, true);
		_weapon.Critical = (float)StaticMethod.Get_nextSpec(_weapon.Critical, RefLv, false);

		return _weapon;
	}






	/// <summary>
	/// 유저정보 프로토콜 호출 (인자값: 유저, 콜백함수)
	/// </summary>
	public void Protocol_GetUserInfo(User_Users user, del_webResp_0 callback)
	{
		webRequest.GetUserInfos(user.UserID, callback);

	}

	/// <summary>
	/// 게임종료시 유저정보 데이터갱신을위한 프로토콜 (인자값: 콜백함수)
	/// </summary>
	public void Protocol_GameEndRfsData(del_webResp_0 callback)
	{
		//이전 gain 정보 클리어
		webResponse.GetResultInfoList.Clear();

		webRequest.GameEndRfsData(callback);

	}



	/// <summary>
	/// 이전 UI 삭제 후 씬 로드 (매개변수 :씬이름)
	/// </summary>
	public void Load_Scene(string sceneName)
	{

		GameObject orinObj = Resources.Load(string.Format("Prefebs/{0}", "GameLoading")) as GameObject;
		GameObject CloneObj = Instantiate(orinObj) as GameObject;

		CloneObj.transform.SetParent(UI_Manager.Getsingleton.CanvasTr);

		RectTransform _cloneRectTr = CloneObj.GetComponent<RectTransform>();
		RectTransform _OrinIObjRectTr = orinObj.GetComponent<RectTransform>();

		_cloneRectTr.sizeDelta = _OrinIObjRectTr.sizeDelta;
		_cloneRectTr.anchoredPosition = _OrinIObjRectTr.anchoredPosition;
		_cloneRectTr.localScale = _OrinIObjRectTr.localScale;

		StartCoroutine(Loadmanager.LoadScene(sceneName, callback_Complete_LoadScene, callback_nextProcess_loadScene, CloneObj.GetComponent<GameLoadProcess>().slider));

		//동기방식



		//SceneManager.LoadScene(sceneName);
		//UI_Manager.Getsingleton.ClearPopupUI();
		//UI_Manager.Getsingleton.ClearALL_UI();
		//UI_Manager.Getsingleton.Find_UICanvasTr();
	}

	void callback_nextProcess_loadScene()
	{
		SoundExecuter.Getsingleton.ChkStartMusic(MUSIC_TYPE.LOBBY, false); // 로비배경음 끄기;

		UI_Manager.Getsingleton.ClearALL_UI();

		//로테이트 변수 false 
		User.isSelectedCharacter = false;

		Loadmanager.async.allowSceneActivation = true;

		UserEditor.Getsingleton.EditLog("allowActivation");
		
	}

	void callback_Complete_LoadScene()
	{
		
		UI_Manager.Getsingleton.UIPopupList.Clear();
		UI_Manager.Getsingleton.Find_UICanvasTr();


		//광고표시를 위한 광고 로드
		User _user = UserDataManager.instance.user;

		if (_user.Get_user_goods(ITEMTYPE.AD_REMOVE) > 0)
		{
			UserEditor.Getsingleton.EditLog("광고 제거 햇다, 광고 로드 안함");
		}
		else
		{
			if (TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_AfterGameShowAd].ConsVal == 1)
			{
				UserEditor.Getsingleton.EditLog("광고 제거 안햇다, 1판마다 전이므로 광고로드하자");
				//AdsManager.instance.Reuest_Ads(Ads_TYPE.Interstitial_Image);
			}
			else if (_user.Get_user_goods(ITEMTYPE.GAMEPLAY) %
				TableDataManager.instance.Infos_ConstValues[(byte)ConstValue_TYPE.Const_AfterGameShowAd].ConsVal != 0) // 2판마다 광고보기
			{
				UserEditor.Getsingleton.EditLog("광고 제거 안햇다, n판마다 전이므로 광고로드하자");
				//AdsManager.instance.Reuest_Ads(Ads_TYPE.Interstitial_Image);
			}

		}
		
	}

	void callback_nextprocess3_loadscene()
	{
	

	}


	/// <summary>
	/// 결과화면 UI 보여주기 (매개변수 : 승리flag => 승  = 0 , 패 = 1 ,무승부 = 2) 
	/// </summary>
	public void Create_ResultUI(byte winFlag)
	{
		UI_Manager _ui_mgr = UI_Manager.Getsingleton;

		//인게임 canvas 찾기
		_ui_mgr.Find_UICanvasTr();

		//결과창 UI 생성
		//_ui_mgr.Dic_UILst.Clear();
		_ui_mgr.CreatUI(UI.INGAME_RESULT, _ui_mgr.CanvasTr);

		// 결과창 UI의 컴포넌트가져오기
		UI_Ingame_result ingame_result = _ui_mgr.Dic_UILst[UI.INGAME_RESULT] as UI_Ingame_result;

		ingame_result.showResult(winFlag);

		ingame_result.transform.SetAsFirstSibling();

	}




	//채팅UI 띄우기
	public void Create_ChatUI()
	{
		UI_Manager _ui_mgr = UI_Manager.Getsingleton;

		//인게임 canvas 찾기
		_ui_mgr.Find_UICanvasTr();

		//결과창 UI 생성
		//_ui_mgr.Dic_UILst.Clear();
		_ui_mgr.CreatUI(UI.CHAT, _ui_mgr.CanvasTr);
	}


	/// <summary>
	/// 채팅 UI 삭제하기
	/// </summary>
	public void Clear_ChatUI()
	{
		UI_Manager _ui_mgr = UI_Manager.Getsingleton;

		//인게임 canvas 찾기
		_ui_mgr.Find_UICanvasTr();

		_ui_mgr.ClearUI(UI.CHAT);

	}

	
	/// <summary>
	/// 채팅메세지 보내기
	/// </summary>
	public void Send_ChatMessage(string msg)
	{
		//욕설인지 체크 : 욕설은 *로 바껴서 나옴
		TextDataManager.Chk_BannedLetter(ref msg);
		Network_MainMenuSoketManager.Getsingleton.Send_CTS_ChatMessage(1, msg);

	
	}

	/// <summary>
	/// 채팅창에 알림메세지 띄우기
	/// </summary>
	public void Send_NoticeMessage(string msg)
	{
		UI_Chat.Getsingleton.RecieveMessage_BattleChatNotice(msg);

	}



	
	//////================================================= 네트워크 부분 ================================================

	// 소켓서버 데이터보내기
	public void Send_NetData(NETKIND _Protocol, byte[] _Data)
	{
		Network_MainMenuSoketManager.Getsingleton.NetSendData(_Protocol, _Data);
	}


	//소켓 연결되었는지 체크
	public bool IsConnectNet()
	{
		return Network_MainMenuSoketManager.Getsingleton.IsConnect();
	}


	// 퀵조인 성공시 호출 함수
	public void Complete_QuickJoin(byte mapIdx)
	{
		
		Network_MainMenuSoketManager.Getsingleton.MapIdx = mapIdx;
		Network_MainMenuSoketManager.Getsingleton.Operation_State = MMSERVER_STATE.COMPLETE_QUICKJOIN;

		

	}



	/// <summary>
	/// 전투씬 끝나고 로비로 이동 함수
	/// </summary>
	public void Set_GoToLobby()
	{
		//네트워크 끊기
		//Link_Script.ins.MainMenu_Move_Init();

		//모든팝업 클리어하기
		UI_Manager.Getsingleton.ClearPopupUI();

		UI_Manager.Getsingleton.ClearALL_UI();

		UI_Manager.Getsingleton._UI = UI.LOBBY;

		Scene _scene = SceneManager.GetActiveScene();
		if (_scene.name != DefineKey.Main)
		{
			//SceneManager.LoadScene(DefineKey.Main);
			//SceneManager.LoadSceneAsync(DefineKey.Main);
			StartCoroutine(Loadmanager.LoadScene(DefineKey.Main, callback_1, callback_2, null));
			//StartCoroutine(Loadmanager.LoadScene("temp", callback_1, callback_2, null));
			//SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

		}

		

	}

	void callback_1()
	{
		//Debug.Log("come in callback 1");
	}

	void callback_2()
	{
		Loadmanager.async.allowSceneActivation = true;
		//Debug.Log("come in callback 2");
	}


	// ===================================== Etc ================================

	//현재씬 이름 반환
	public string Get_currentScene()
	{
		Scene _scene = SceneManager.GetActiveScene();
		return _scene.name;
	}


	/// <summary>
	/// 옵션세팅(시야감도, 표현, 자동공격등) 정보들을 로컬에 저장 합니다.
	/// </summary>
	public void Save_OptionSettingInfo(OptionSetting _setting)
	{
		_setting.Save_SettingValues();
	}

	/// <summary>
	///  옵션세팅(시야감도, 표현, 자동공격등) 정보들을 반환합니다.
	/// </summary>
	public OptionSetting Get_OptionSettingInfo()
	{
		OptionSetting _optionSetting = OptionSetting.instance;
		return _optionSetting;

	
		
	}


	public void PlayGameSound(AUDIOSOURCE_TYPE Audio_Type, int AudioIdx, AudioSource AudioSource3d =null)
	{
		if(Audio_Type == AUDIOSOURCE_TYPE.AUDIO_2D)
		{
			SoundExecuter.Getsingleton.PlayEffect(AudioIdx);
		}
		else if(Audio_Type == AUDIOSOURCE_TYPE.AUDIO_3D)
		{
			if (AudioSource3d == null)
			{
				AudioSource3d = GetComponent<AudioSource>(); // 캐릭터에 붙어있는는 audiosource 가져오기
				if (AudioSource3d == null)
					Debug.LogError("please find audioSource for 3d");
			}

			SoundExecuter.Getsingleton.PlaySound_3D(AudioIdx, AudioSource3d);
		}

		
	}

	


	






















	private static SendManager _instance;
	public static SendManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(SendManager)) as SendManager;

				if (_instance == null)
				{
					GameObject obj = new GameObject("SendManager");
					_instance = obj.AddComponent(typeof(SendManager)) as SendManager;

				}
			}
			return _instance;
		}
	}

	void Awake()
	{
		_instance = this;
		 DontDestroyOnLoad(this.gameObject);
	}

	public string Release_BattleVersion;
	public string Develop_BattleVersion;
}
