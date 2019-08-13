using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.SimpleAndroidNotifications;


public class Main : MonoBehaviour 
{
	//public UI_Lobby lobby;
	//public UI_Setting setting;

    private static Main _instance;
    public static Main Getsingleton
    {
        get
        {
            if (_instance == null)
            {
               _instance = FindObjectOfType(typeof(Main)) as Main;
                if (_instance == null)
                    _instance = new GameObject("Main").AddComponent<Main>();
            }

            return _instance;
        }
    }



    void Awake()
    {
        _instance = this;
		 
		


		//씬이 메인메뉴로 넘어갈때 전투씬에 있던 오브젝트 전부 삭제 하기
		//Link_Script.ins.Battle_Effect_Destroy();

		

		//광고
		//AdsManager ad = AdsManager.instance;

		OptionSetting _optionSetting = OptionSetting.instance;
		LanguageManager.Getsingleton.SetLanguage((LanguageCode)_optionSetting.usingLangueage);

		//사운드매니저 오브젝생성
		SoundExecuter soundExc = SoundExecuter.Getsingleton;
		soundExc.ChkStartMusic(MUSIC_TYPE.LOBBY,true); // 로비배경음 시작;
		ObjectPoolManager op = ObjectPoolManager.Getinstance;

	
		//linkScript 객체 생성하기
		Link_Script.i.LinkScript_Start();

	
		StartMain();

 
    }

	public void StartMain()
	{

		//UI 오브젝트 체크
		//if (ObjectPoolManager.Getinstance.IsContainUI(UI.LOBBY.ToString()))
		//{
		//	Destroy(lobby.gameObject);
		//}
		//if (ObjectPoolManager.Getinstance.IsContainUI(UI.SETTING.ToString()))
		//{
		//	Destroy(setting.gameObject);
		//}
		

		UI_Manager.Getsingleton.StartUI();



	}

	

}
