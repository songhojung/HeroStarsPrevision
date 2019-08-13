using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUnitCost
{

}

public enum BattleContry
{
	NA,		//North America
	//OC,		//Oceania 
	//SA,		//South America
	//AN,		//Antarctica (남극 대륙)
	AS,		//아시아
	EU,		//유럽
	//AF,		//아프리카
}

public class UserEditor : MonoBehaviour 
{

	public ServerKind ConnectServer = ServerKind.Developer;
	public int PlfID = 0;		//플랫폼 아이디 
    public string Lgnkey;				// 로그인 키값 = 구글/페이스북 에서 받은 키값
	public bool isOnRand = true;
	public bool isQuickLoginForEditor = false; // 에디터에서 빠르게 로그인 하려면 on 하시오
	public bool isOpenAllUnits = false;		//유저의 유닛들을 모두 개방 할것인가 ?
	public bool isDoTestTutorial = false;    //튜토리얼 테스트를 할것이냐
	public bool isUseAppgurdLogin = false;			//앱가드 로그인할거냐 
	public BattleContry battleContry = BattleContry.AS;
	
	

	//테스트로 다른 서버 붙을때 (연탁)
	public string Test_MatchIpAdress = string.Empty;		
	public ushort Test_MatchPort = 0;

	public bool isNotUseServerCheckPopup = false;

	private static UserEditor _instance;
	public static UserEditor Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(UserEditor)) as UserEditor;
				if (_instance == null)
				{
					_instance = new GameObject("UserEditor").AddComponent<UserEditor>();
				}
			}

			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			//DontDestroyOnLoad(this);
			//EditUserInfo();
			// 임시 : 로그인 프로토콜호출
			//UI_Login.Getsingleton.ResponseButton_login();
		}
	}



	public void EditUserInfo()
	{
        

		User _user = UserDataManager.instance.user;
		int randKey = Random.Range(1500, 1600);
        
//#if UNITY_EDITOR
		if (isOnRand)
		{
			_user.user_logins.Lgnkey = randKey.ToString();


		}
		else
		{
			_user.user_logins.Lgnkey = Lgnkey;
			_user.user_logins.PlfID = System.Convert.ToByte(PlfID);
		}
		

		//로그인 시도
		//UI_Login.Getsingleton.StartLogin();

	}

	public void EditLog(object message)
	{

#if UNITY_EDITOR
		if(true)
#else
		if (DefineKey.LogKind == ServerKind.Developer)
#endif
			Debug.Log(message);
		else
			return;
	}

}
