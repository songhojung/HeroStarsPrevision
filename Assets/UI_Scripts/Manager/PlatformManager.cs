using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using GooglePlayGames.BasicApi.SavedGame;
//using GooglePlayGames.BasicApi;
//using GooglePlayGames;
//using UnityEngine.SocialPlatforms;
//using Facebook.Unity;
//#if UNITY_IOS
//using UnityEngine.SocialPlatforms.GameCenter;
//#endif

public class PlatformManager : MonoBehaviour
{
    public string Platform_UserID;
    public string Platform_UserEmail;
    public del_NextProcess1 nextPorcess;


    private static PlatformManager _instance;
    public static PlatformManager Getsingleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(PlatformManager)) as PlatformManager;
                if (_instance == null)
                {
                    _instance = new GameObject("PlatformManager").AddComponent<PlatformManager>();
                }
            }
            return _instance;
        }
    }



    void Awake()
    {
        Init_Platform();
    }

    //플랫폼 초기화 하기
    public void Init_Platform()
    {
        //안드로이드
#if UNITY_ANDROID

        //안드로이드 빌더 초기화
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestEmail().Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //UserEditor.Getsingleton.EditLog("구글 플레이 빌더 초기화");

        ////구글플레이 활성화
        //PlayGamesPlatform.Activate();
        //UserEditor.Getsingleton.EditLog("구글플레이 활성화");
#endif




    }



    public bool Authenticated
    {
//#if UNITY_EDITOR
        get { return false; }
        //#elif UNITY_ANDROID
        //		//get { return PlayGamesPlatform.Instance.IsAuthenticated();}
        //#elif UNITY_IOS
        //         //get { return Social.Active.localUser.authenticated; }
        //#endif
    }

    public void FB_Initialize()
    {
        //페북 초기화
        //if (!FB.IsInitialized)
        //{
        //    UserEditor.Getsingleton.EditLog("페북 init");
        //    FB.Init(FB_CompleteInitialize, OnHideUnity);

        //}
        //else
        //{
        //    UserEditor.Getsingleton.EditLog("already FB initialized");
        //    Facebook_SignIn();
        //}
    }


    void FB_CompleteInitialize()
    {
        UserEditor.Getsingleton.EditLog("페북 init 콜백처리");
        //if (FB.IsInitialized)
        //{
        //    UserEditor.Getsingleton.EditLog("FB initialized start FB ActivateApp");
        //    FB.ActivateApp();
        //    Facebook_SignIn();
        //}
        //else
        //{
        //    UserEditor.Getsingleton.EditLog("Failed to Init the Facebook SDK");
        //}
    }

    void OnHideUnity(bool isGameShown)
    {
        UserEditor.Getsingleton.EditLog("OnHideUnity : isGameShown => " + isGameShown);
        if (!isGameShown)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    // 이전 계정으로 사용햇던 플랫폼 체크하기
    public int ChkGet_BeforeUsePlatformIndex()
    {
        int platformValue = 0;

        if (PlayerPrefs.HasKey(DefineKey.BeforeUserPlatformAccount))
        {
#if UNITY_EDITOR
            if (UserEditor.Getsingleton.isQuickLoginForEditor) // 개발을위해 에디터에서 바로 게스트로그인하기
                platformValue = 1;
            else
            {
                //platformValue = 0;
                platformValue = PlayerPrefs.GetInt(DefineKey.BeforeUserPlatformAccount);
            }
#elif UNITY_ANDROID || UNITY_IOS
			platformValue = PlayerPrefs.GetInt(DefineKey.BeforeUserPlatformAccount);
			//디버깅용으로 0값줌 나중에 삭제
			//platformValue = 0;
#endif
        }
        else
        {
            platformValue = 0;
            Save_NowUsePlatformIndex(platformValue);
        }

        return platformValue;
    }

    public void Save_NowUsePlatformIndex(int _platformValue)
    {
        PlayerPrefs.SetInt(DefineKey.BeforeUserPlatformAccount, _platformValue);
        PlayerPrefs.Save();
        UserEditor.Getsingleton.EditLog("플랫폼인덱스 저장 : " + _platformValue);
    }






    //구글 로그인
    public void Google_SignIn()
    {
#if UNITY_ANDROID
        User _user = UserDataManager.instance.user;


        //if (PlayGamesPlatform.Instance.IsAuthenticated() == true)
        //{
        //    if (_user.user_logins.PlfID == 2)
        //        UserEditor.Getsingleton.EditLog("이미 계정 연결되었음");
        //}
        //else if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        //{
        //    // 이전 구글 로그인으로 GoogleID 가 저장 되어있다면 저장된값 불러오기
        //    if (PlayerPrefs.HasKey("UserPlatformID"))
        //    {
        //        Platform_UserID = PlayerPrefs.GetString(DefineKey.UserPlatformID);
        //        Platform_UserEmail = PlayerPrefs.GetString(DefineKey.UserPlatformEmail);

        //        nextPorcess(true);
        //    }
        //    else // 이전 구글 로그인으로 GoogleID 가 저장 안되어 있다면 로그인 시도하여 id와 이메일얻어 저장
        //    {

        //        PlayGamesPlatform.Instance.Authenticate((bool sucess) =>
        //            {
        //                if (sucess)
        //                {
        //                    UserEditor.Getsingleton.EditLog("google sucess");

        //                    Platform_UserID = PlayGamesPlatform.Instance.GetUserId();
        //                    Platform_UserEmail = PlayGamesPlatform.Instance.GetUserEmail();

        //                    //로컬에 저장
        //                    PlayerPrefs.SetString(DefineKey.UserPlatformID, Platform_UserID);
        //                    PlayerPrefs.SetString(DefineKey.UserPlatformEmail, Platform_UserEmail);
        //                    PlayerPrefs.Save();

        //                    UserEditor.Getsingleton.EditLog(string.Format("google id : {0}, goole Email : {1}", Platform_UserID, Platform_UserEmail));

        //                }
        //                else
        //                {
        //                    UserEditor.Getsingleton.EditLog("google fail	");
        //                }

        //                nextPorcess(sucess);
        //            });
        //    }
        //}
        //Social.localUser.Authenticate(Google_SignIn);
#endif
    }




    public void Google_SignOut()
    {
#if UNITY_ANDROID
        // 로그인 상태면 호출
        //if (PlayGamesPlatform.Instance.IsAuthenticated() == true)
        //{
        //    PlayGamesPlatform.Instance.SignOut();
        //}
#endif
    }





    public void Gamecenter_SignIn()
    {
        // 이전 겜센터 로그인으로 ID 가 저장 되어있다면 저장된값 불러오기
        if (PlayerPrefs.HasKey("UserPlatformID"))
        {
            UserEditor.Getsingleton.EditLog(" Have Gamecenter uid");
            Platform_UserID = PlayerPrefs.GetString(DefineKey.UserPlatformID);
            Platform_UserEmail = PlayerPrefs.GetString(DefineKey.UserPlatformEmail);

            nextPorcess(true);
        }
        else // 이전 겜센터 로그인으로 ID 가 저장 안되어 있다면 로그인 시도하여 id와 이메일얻어 저장
        {
#if UNITY_IOS


			//GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

			//Social.localUser.Authenticate((bool success) =>
			//{
			//	if (success)
			//	{
			//		UserEditor.Getsingleton.EditLog("Gamecenter success");
			//		Platform_UserID = Social.localUser.id;
			//		UserEditor.Getsingleton.EditLog("Gamecenter id : " + Platform_UserID);
			//		Platform_UserEmail ="";

			//		//로컬에 저장
			//		PlayerPrefs.SetString(DefineKey.UserPlatformID, Platform_UserID);
			//		PlayerPrefs.SetString(DefineKey.UserPlatformEmail, Platform_UserEmail);
			//		PlayerPrefs.Save();
			//	}
			//	else
			//	{
			//		UserEditor.Getsingleton.EditLog("gamecenter fail	");
			//	}

			//	nextPorcess(success);

			//});
#endif
        }


    }






    public void Facebook_SignIn()
    {

        // 이전 페이스북 로그인으로 FID 가 저장 되어있다면 저장된값 불러오기
        //if (PlayerPrefs.HasKey(DefineKey.UserPlatformID))
        //{
        //    Platform_UserID = PlayerPrefs.GetString(DefineKey.UserPlatformID);
        //    Platform_UserEmail = PlayerPrefs.GetString(DefineKey.UserPlatformEmail);

        //    // 다음프로세스로
        //    nextPorcess(true);
        //}
        //else // 이전 페이스북 로그인 으로 FID  저장된게없으면 
        //{
        //    List<string> permissions = new List<string>();
        //    permissions.Add("public_profile");
        //    permissions.Add("email");
        //    permissions.Add("user_friends");

        //    //FB 로그인 하는 부분
        //    FB.LogInWithReadPermissions(callback: Facebook_AuthCallback);
        //}
    }

    //void Facebook_AuthCallback(IResult result)
    //{
    //    bool _isLogInAndGetInfo = false;

    //    if (result != null)
    //    {
    //        UserEditor.Getsingleton.EditLog("FB authCallback 들어왔다");
    //        if (FB.IsLoggedIn)
    //        {
    //            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
    //            UserEditor.Getsingleton.EditLog("FB_Token_userID : " + aToken.UserId);

    //            foreach (string perm in aToken.Permissions)
    //            {
    //                UserEditor.Getsingleton.EditLog(perm);
    //            }




    //            //FB api에서 이메일과 Id 값 받아오기 
    //            FB.API("/me?fields=email", HttpMethod.GET, (sucess) =>
    //            {
    //                if (sucess.Error == null)
    //                {
    //                    //api 반환값 키중에 email이 잇으면
    //                    if (sucess.ResultDictionary.ContainsKey("email"))
    //                    {
    //                        UserEditor.Getsingleton.EditLog("email : " + sucess.ResultDictionary["email"]);
    //                        Platform_UserEmail = (string)sucess.ResultDictionary["email"];

    //                        //페이스북로그인 이메일값 저장
    //                        PlayerPrefs.SetString(DefineKey.UserPlatformEmail, Platform_UserEmail);
    //                        PlayerPrefs.Save();
    //                    }
    //                    else
    //                    {
    //                        UserEditor.Getsingleton.EditLog("email : empty");
    //                        Platform_UserEmail = "";
    //                    }

    //                    //api 반환값 키중에 id가 잇으면
    //                    if (sucess.ResultDictionary.ContainsKey("id"))
    //                    {
    //                        UserEditor.Getsingleton.EditLog("id : " + sucess.ResultDictionary["id"]);
    //                        Platform_UserID = (string)sucess.ResultDictionary["id"];

    //                        //페이스북로그인 FID 저장
    //                        PlayerPrefs.SetString(DefineKey.UserPlatformID, Platform_UserID);
    //                        PlayerPrefs.Save();
    //                    }
    //                    else
    //                    {
    //                        UserEditor.Getsingleton.EditLog("id : empty");
    //                        Platform_UserID = "";
    //                    }

    //                    _isLogInAndGetInfo = true;
    //                }
    //                else
    //                {
    //                    UserEditor.Getsingleton.EditLog(sucess.Error);
    //                    _isLogInAndGetInfo = false;
    //                }

    //                // 다음프로세스로
    //                nextPorcess(_isLogInAndGetInfo);
    //            });


    //        }
    //        else
    //        {
    //            UserEditor.Getsingleton.EditLog("FB 로그인 되지 않음");
    //            _isLogInAndGetInfo = false;
    //            // 다음프로세스로
    //            nextPorcess(_isLogInAndGetInfo);
    //        }


    //    }
    //    else
    //    {
    //        UserEditor.Getsingleton.EditLog(result.Error);
    //    }


    //}


    public void Guest_Signin()
    {
        UserEditor.Getsingleton.EditLog("로그인 게스트 계정");

        User _user = UserDataManager.instance.user;
        _user.user_Users.Init(); //

#if UNITY_EDITOR
        UserEditor.Getsingleton.EditUserInfo();
        nextPorcess(true);

#elif UNITY_ANDROID || UNITY_IOS

		//Uid = SystemInfo.deviceUniqueIdentifier;
		string Uid = IosPluginManager.Getsingleton.getGUID();
		UserEditor.Getsingleton.EditLog("uid : " + Uid);
		_user.user_logins.Lgnkey = Uid;

		UserEditor.Getsingleton.EditLog("_user.user_logins.Lgnkey : " + _user.user_logins.Lgnkey);
		//로그인시도
		nextPorcess(true);
#endif
    }



    //연동된 계정 로그아웃
    public void Platform_LogOut()
    {
        UserEditor.Getsingleton.EditLog("계정 로그아웃");
#if UNITY_ANDROID
        //구글
        Google_SignOut();
#elif UNITY_IOS
		//게임센터 

#endif
        //패북
        //if (FB.IsLoggedIn) // 패북은 로그인 될떄 로그아웃 해주자.
        //{
        //    FB.LogOut();
        //}

        //값 초기화
        Platform_UserID = "";
        Platform_UserEmail = "";

        //저장된 id키,email키, platform타입키 삭제
        PlayerPrefs.DeleteKey(DefineKey.UserPlatformID);
        PlayerPrefs.DeleteKey(DefineKey.UserPlatformEmail);
        PlayerPrefs.DeleteKey(DefineKey.BeforeUserPlatformAccount);

    }





    //====================================================   업적/리더보드   =======================================================================================

    //업적키 가져오기
    public string Get_AchievementKey(int Lv)
    {
        string str = string.Empty;
        switch (Lv)
        {
            //case 2: str = GPGSIds.achievement_user_level_2_achievement; break;
            //case 3: str = GPGSIds.achievement_user_level_3_achievement; break;
            //case 4: str = GPGSIds.achievement_user_level_4_achievement; break;
            //case 5: str = GPGSIds.achievement_user_level_5_achievement; break;
            //case 20: str = GPGSIds.achievement_user_level_20_achievement; break;
            //case 30: str = GPGSIds.achievement_user_level_30_achievement; break;
            //case 40: str = GPGSIds.achievement_user_level_40_achievement; break;
            //case 50: str = GPGSIds.achievement_user_level_50_achievement; break;
            default: str = ""; break;
        }

        return str;
    }

    // 리더보드 올리기
    public void ReportLeaderboard(string _leaderId, long _score, System.Action<bool> _callback)
    {
        if (!Authenticated || string.IsNullOrEmpty(_leaderId))
        {
            if (_callback != null)
                _callback(false);
            return;
        }
        else
        {

            Social.ReportScore(_score, _leaderId, (_success) =>
            {
                if (_success)

                    if (_callback != null)
                        _callback(_success);
            });
        }
    }

    //업적 풀기
    public void UnlockAchievement(string _achId, System.Action<bool> _callback)
    {
        UserEditor.Getsingleton.EditLog("try UnlockAchievement");
        if (!Authenticated || string.IsNullOrEmpty(_achId))
        {
            UserEditor.Getsingleton.EditLog("achive false");
            if (_callback != null)
                _callback(false);
            return;
        }
        else
        {
            Social.ReportProgress(_achId, 100.0f, (_success) =>
            {
                UserEditor.Getsingleton.EditLog("achive true");
                if (_callback != null)
                    _callback(_success);
            });
        }
    }

    //업적들 보여주기
    public void ShowAchievementsUI()
    {
        if (Authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }

    //리더보드 보여주기
    public void ShowLeaderboardUI()
    {
        if (Authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }



}
