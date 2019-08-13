using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Prime31;

public class webResponse
{
	#region const string variation public string
	private const string SvIdx = "SvIdx";
	private const string SvStatus = "SvStatus";
	private const string SvChkMsg = "SvChkMsg";
	private const string SvChkTm = "SvChkTm";
	private const string WebSvIp = "WebSvIp";
	private const string MatchSvIp = "MatchSvIp";
	private const string MatchSvPort = "MatchSvPort";
	private const string ChatSvIp = "ChatSvIp";
	private const string ChatSvPort = "ChatSvPort";
	private const string SubIdx = "SubIdx";
	private const string ServerName = "ServerName";
	private const string PubIp = "PubIp";
	private const string CnPort = "CnPort";
	private const string ConPer = "ConPer";
	private const string game_servers = "game_servers";
	private const string DefaultSvIdx = "DefaultSvIdx";
	private const string market_versions = "market_versions"; 
	private const string UserId = "UserID";
	private const string LgnToken = "LgnToken";
	private const string Nknm = "NkNm";
    private const string UserLv = "UserLv";
    private const string ClanMark = "ClanMark";
	private const string CtrCd = "CtrCd";
	private const string ItTp = "ItTp";
	private const string ItNum = "ItNum";
	private const string UnitIdx = "UnitIdx";
	private const string UnitIdx1 = "UnitIdx1";
	private const string UnitIdx2 = "UnitIdx2";
	private const string UnitIdx3 = "UnitIdx3";
	private const string UnitIdx4 = "UnitIdx4";
	private const string UnitRk = "UnitRk";
	private const string UnitExp = "UnitExp";
	private const string WpnIdx = "WpnIdx";
	private const string DecoIdx = "DecoIdx";
	private const string MainWpnIdx = "MainWpnIdx";
	private const string SubWpnIdx = "SubWpnIdx";
	private const string DecoIdx1 = "DecoIdx1";
	private const string DecoIdx2 = "DecoIdx2";
	private const string DecoIdx3 = "DecoIdx3";
	private const string SubSkill = "SubSkill";
	private const string RefFailCnt = "RefFailCnt";
	private const string UnitHvCnt = "UnitHvCnt";
	private const string RefLv = "RefLv";
	private const string TmIdx = "TmIdx";
	private const string TmSubIdx = "TmSubIdx";
	private const string stime = "Stime";
	private const string etime = "Etime";
	private const string mtime = "mtime";
	private const string ctime = "ctime";
	private const string BtSlotIdx = "BtSlotIdx";
	private const string LgSlotIdx = "LgSlotIdx";
	private const string ItBoxIdx = "ItBoxIdx";
	private const string BtIdx = "BtIdx";
	private const string ServerTime = "ServerTime";
	private const string ClanID = "ClanID";
	private const string RwdIdx = "RwdIdx";
	private const string JoinTmLm = "JoinTmLm";
	private const string ClanName = "ClanName";
	private const string MasterNm = "MasterNm";
	private const string MemberCnt = "MemberCnt";
	private const string PersonsCnt = "PersonsCnt";
	private const string ClanInfoTxt = "ClanInfoTxt";
	private const string GoldBufTm = "GoldBufTm";
	private const string ExpBufTm = "ExpBufTm";
	private const string MemIdx = "MemIdx";
	private const string CnRank = "CnRank";
	private const string CnPoint = "CnPoint";
	private const string CnPoint0 = "CnPoint0";
	private const string CnPoint1 = "CnPoint1";
	private const string CnPointSum0 = "CnPointSum0";
	private const string CnPointSum1 = "CnPointSum1";
	private const string RqUnitIdx = "RqUnitIdx";
	private const string RqStTime = "RqStTime";
	private const string JoinTime = "JoinTime";
	private const string BodIdx = "BodIdx";
	private const string BodKind = "BodKind";
	private const string PostIdx = "PostIdx";
	private const string BodTxt = "BodTxt";
	private const string SndUserID = "SndUserID";
	private const string SndClanID = "SndClanID";
	private const string SndNkNm = "SndNkNm";
	private const string GetFlg = "GetFlg";
	private const string PstPasIdx = "PstPasIdx";
	private const string PostTxt = "PostTxt";
	private const string ExpiryDay = "ExpiryDay";
	private const string ItIdx = "ItIdx";
	private const string LeageIdx = "LeageIdx";
	private const string BattleKind = "BattleKind";
	private const string BtSvIP = "BtSvIP";
	private const string BtSvPort = "BtSvPort";
	private const string GameEndTime = "GameEndTime";
	private const string BtlRoomID = "BtlRoomID";
	private const string MapIdx = "MapIdx";
	private const string RandKey = "RandKey";
	private const string Ranking = "Ranking";
	private const string Exp = "Exp";
	private const string Victories = "Victories";
	private const string KillDeath = "KillDeath";

	private const string ArgIdx = "ArgIdx";
	private const string LeaderIdx = "LeaderIdx";
	private const string FrUserID = "FrUserID";
	private const string Url = "Url";
	private const string QstSlot = "QstSlot";
	private const string QstIdx = "QstIdx";
	private const string QstClrCnt = "QstClrCnt";
	private const string QstRsTime = "QstRsTime";
	private const string continent_code = "continent_code";
	private const string continent_name = "continent_name";
	private const string country_iso_code = "country_iso_code";
	private const string BuyFlgBit = "BuyFlgBit";
	private const string BuyTime = "BuyTime";
	private const string LggCd = "LggCd";
	private const string ClanPush = "ClanPush";
	private const string UseCnt = "UseCnt";
	private const string KillCnt = "KillCnt";
	private const string DeathCnt = "DeathCnt";
	private const string WrdKind = "WrdKind";
	private const string Words = "Words";
	private const string AdTm = "AdTm";
	private const string RwdItTp = "RwdItTp";
	private const string AdShowCnt = "AdShowCnt";
	private const string PlayGoldRwd = "PlayGoldRwd";
	private const string PlayExpRwd = "PlayExpRwd";
    private const string LvRwd = "LvRwd";
    private const string PlayCnt = "PlayCnt";





	#endregion

	public static bool Resp_Error = false;
	public static Dictionary<ITEMTYPE, bool> DicGetState = new Dictionary<ITEMTYPE, bool>();
	public static List<GainItem> GetResultInfoList = new List<GainItem>();
	public static Dictionary<string, object> Dic_data = new Dictionary<string, object>(); // 반환데이터를 확인할 일있을떄 사용


	public static void Init()
	{
		Resp_Error = false;
		GetResultInfoList.Clear();
		Dic_data.Clear();

	}

	// ITEMTYPE 대한 gainItem 에대해 특정 아이템 값 반환
	public static bool GetResultGainItem(ITEMTYPE itemType, ref GainItem _gainItem)
	{
		bool isGet = false;
		if (GetResultInfoList.Count <= 0)
			isGet = false;
		else
		{
			for (int i = 0; i < GetResultInfoList.Count;  i++)
			{
				if (GetResultInfoList[i].ItTp != ITEMTYPE.NONE)
				{
					if (GetResultInfoList[i].ItTp == itemType)
					{
						_gainItem = GetResultInfoList[i];
						isGet =  true;
					}
					
				}
			}
		}

        if(isGet)
        {
            //반환햇으니 리스트에 잇는거 삭제
            GetResultInfoList.Remove(_gainItem);
        }

		return isGet;
		
	}


	public static void Chk_gainUnit(GainUserUnit _gain)
	{
		bool IsHaveType = false;
		for (int i = 0; i < GetResultInfoList.Count; i++)
		{
			if (GetResultInfoList[i].ItTp == ITEMTYPE.UNIT)
			{
				IsHaveType = true;
				if (GetResultInfoList[i].gainUserUnit.Unitidx == _gain.Unitidx)
				{
					GetResultInfoList[i].ReAssign_GainUnit(_gain);
					break;
				}
				else
				{
					IsHaveType = false;
					
				}
			}
			
		}

		if(IsHaveType == false)
			GetResultInfoList.Add(new GainItem(ITEMTYPE.UNIT, _gain));

	}

	#region Response : CheckServer
	public static void ResponseWeb_CheckServer(User _user,Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_dicData = (Dictionary<string, object>)_data[JsonKey.DATA];
		}

		if (_dicData.ContainsKey(SvIdx))
			_user.server_Info.SvIdx = Convert.ToByte(_dicData[SvIdx]);
		if (_dicData.ContainsKey(SvStatus))
			_user.server_Info.SvStatus = Convert.ToByte(_dicData[SvStatus]);
		if (_dicData.ContainsKey(SvChkMsg))
			_user.server_Info.SvChkMsg = Convert.ToString(_dicData[SvChkMsg]);
		if (_dicData.ContainsKey(SvChkTm))
			_user.server_Info.SvChkTm = Check_returnTime(_dicData[SvChkTm]);
		if (_dicData.ContainsKey(WebSvIp))
			_user.server_Info.WebSvIp = Convert.ToString(_dicData[WebSvIp]);
		if (_dicData.ContainsKey(ServerTime))
			_user.server_Info.ServerTime = Check_returnTime(_dicData[ServerTime]);

		
#if UNITY_EDITOR


		UserEditor.Getsingleton.EditLog("complete CheckServer");
#endif
	}
	#endregion



	#region Response : GetServerList
	public static void ResponseWeb_GetServerList(User _user, Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_dicData = (Dictionary<string, object>)_data[JsonKey.DATA];
		}

		

			if (_dicData.ContainsKey(game_servers))
			{
				List<object> _lst = (List<object>)_dicData[game_servers];
				for (int i = 0; i < _lst.Count; i++)
				{


					Dictionary<string, object> _dicserver = (Dictionary<string, object>)_lst[i];
					GameServer_Info _gameserver = new GameServer_Info();
					if (_dicserver.ContainsKey(SubIdx))
					{
						_gameserver.SubIdx = Convert.ToByte(_dicserver[SubIdx]);
					}
					if (_dicserver.ContainsKey(ServerName))
						_gameserver.ServerName = Convert.ToString(_dicserver[ServerName]);
					if (_dicserver.ContainsKey(PubIp))
						_gameserver.PubIp = Convert.ToString(_dicserver[PubIp]);
					if (_dicserver.ContainsKey(CnPort))
						_gameserver.CnPort = Convert.ToUInt16(_dicserver[CnPort]);
					if (_dicserver.ContainsKey(ConPer))
						_gameserver.ConPer = Convert.ToByte(_dicserver[ConPer]);

					_user.User_GameServerInfos[_gameserver.SubIdx] = _gameserver;

				UserEditor.Getsingleton.EditLog(" GetServer :" + _gameserver.ServerName  + " / " + _gameserver.PubIp +"/" + _gameserver.CnPort);
				}


			}
			if (_dicData.ContainsKey(DefaultSvIdx))
			{
				_user.DefaultConnectServerIdx = Convert.ToByte(_dicData[DefaultSvIdx]);
			}

		
		

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete GetServerList");
#endif
	}
	#endregion




	#region Response : GetVersion
	public static void ResponseWeb_GetVersion(User _user, Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_dicData = (Dictionary<string, object>)_data[JsonKey.DATA];
		}

		if (_dicData.ContainsKey(market_versions))
		{
			Dictionary<string, object> marketData = (Dictionary<string, object>)_dicData[market_versions];
			foreach (var version in marketData)
			{
				string _key = Convert.ToString(version.Key);
				string _value = Convert.ToString(version.Value);
				_user.Market_versions[_key] = _value;
			}

		}

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete GetVersion");
#endif
	}
	#endregion


	#region Response : serverTime
	public static void ResponseWeb_serverTime(Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_dicData = (Dictionary<string, object>)_data[JsonKey.DATA];
		}

		TimeManager.Instance.severTime = Convert.ToDateTime(_dicData[ServerTime]);

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete server time");
#endif
	}
	#endregion


	#region Response : GetConnectAdress
	public static void ResponseWeb_GetConnectAdress(User _user, Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_dicData = (Dictionary<string, object>)_data[JsonKey.DATA];

			Dictionary<string, object> _dicAdressData = new Dictionary<string, object>();
			if (_dicData.ContainsKey(JsonKey.ADRESS))
			{
				_dicAdressData = (Dictionary<string, object>)_dicData[JsonKey.ADRESS];

				if (_dicAdressData.ContainsKey(continent_code))
					_user.user_Address.continent_Code = Convert.ToString(_dicAdressData[continent_code]);

				if (_dicAdressData.ContainsKey(continent_name))
					_user.user_Address.continent_Name = Convert.ToString(_dicAdressData[continent_name]);

				if (_dicAdressData.ContainsKey(country_iso_code))
					_user.user_Address.continent_iso_Code = Convert.ToString(_dicAdressData[country_iso_code]);
			}
		}

		

	}
	#endregion


	#region Response : MemberChange
	public static void ResponseWeb_MemberChange(Dictionary<string, object> _data)
	{
		int errorIdx = 0;
		if (_data.Count > 1)
		{
			if(_data.ContainsKey(JsonKey.ERRORCODE))
			{
				Resp_Error = true;
				 errorIdx = Convert.ToInt16(_data[JsonKey.ERRORCODE]);
				//popup.SetPopupMessage(TextData.Dic_ErrorCode[errorIdx]);
				
			}
			else if (_data.ContainsKey(JsonKey.RESULTCODE))
			{
				Resp_Error = true;
				errorIdx = Convert.ToInt16(_data[JsonKey.RESULTCODE]);
			}

			BeforeErrorProcess(errorIdx);
		}
		
	}
	#endregion








	#region Response : BuyItemBoxData
	public static void ResponseWeb_BuyItemBoxData(User _user, Dictionary<string, object> _data)
	{
		if (_data.ContainsKey(JsonKey.USER_GOODS))
			Public_UserGoodsData(_user, (Dictionary<string, object>)_data[JsonKey.USER_GOODS]);
		if (_data.ContainsKey(JsonKey.USER_UNITS))
			Public_User_UnitData(_user, (Dictionary<string, object>)_data[JsonKey.USER_UNITS]);


#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete buy Item box data");
#endif
	}
	#endregion

	#region Response : GameStartData
	public static void ResponseWeb_GameStartData(User _user, Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dicUserStartData = (Dictionary<string, object>)_data[JsonKey.USER_GAMES];

		//userGames 데이터 우선 생략 
		//ResponseWeb_Public_Data(_user, _dicUserStartData);
#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete gamestart data");
#endif
	}
	#endregion

	#region Response : GameEndData
	public static void ResponseWeb_GameEndData(User _user, Dictionary<string, object> _data)
	{
		if (_data.ContainsKey(JsonKey.USER_GOODS))
			Public_UserGoodsData(_user, (Dictionary<string, object>)_data[JsonKey.USER_GOODS]);
		if (_data.ContainsKey(JsonKey.USER_GAMES))
			UserEditor.Getsingleton.EditLog("Except User Games data");

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete GameEnd data");
#endif
	}
	#endregion


	#region Public_UserBoxOpenLogData
	public static void Public_UserBoxOpenLogData(User _user, List<object> _data)
	{
		int count = 0;
		for (int i = 0; i < _data.Count; i++ )
		{
			
			Dictionary<string, object> _dic_BoxOpenlog = (Dictionary<string, object>)_data[i];

			List<GainItem> Lst_gainItem = new List<GainItem>();
			
				if (_dic_BoxOpenlog.ContainsKey("BoxRwd"))
				{

					List<object> LstBx = (List<object>)_dic_BoxOpenlog["BoxRwd"];

					for (int k = 0; k < LstBx.Count; k++)
					{
						List<object> Bxinfo = (List<object>)LstBx[k];

						ITEMTYPE _itTp = ITEMTYPE.NONE;
						int _Idx = 0;
						int _num = 0;

						for (int h = 0; h < Bxinfo.Count; h++)
						{
							if (h == 0)
								_itTp = (ITEMTYPE)Enum.Parse(typeof(ITEMTYPE), Bxinfo[h].ToString());
							if (h == 1)
								_Idx = Convert.ToInt32(Bxinfo[h]);
							if (h == 2)
								_num = Convert.ToInt32(Bxinfo[h]);

						}

						
					}
				}

				_user.User_BoxOpenLogs[count] = Lst_gainItem;

				count++;
			}
		

		

	}
	#endregion

	#region Public_UserPostLastTimeData
	public static void Public_UserPostLastTimeData(User _user, Dictionary<string, object> _data)
	{
		Dictionary<string, object> _dic_posttime = (Dictionary<string, object>)Dic_data[JsonKey.POSTLASTTIMES];
		if (!DateTime.Equals(_user.Post_lastTimes, Check_returnTime(_dic_posttime[mtime]))) // 받은 데이터가 lastTime 값과 같이 않으면
		{
			if (_user.Post_lastTimes == DateTime.MinValue) //post_lastTime이 처음 값이면
			{
				_user.Post_lastTimes = Check_returnTime(_dic_posttime[mtime]);
				_user.Post_prevTimes = _user.Post_lastTimes;
			}
			else // 처음값이 아니면
			{
				_user.Post_prevTimes = _user.Post_lastTimes;
				_user.Post_lastTimes = Check_returnTime(_dic_posttime[mtime]);
			}
		}



		TimeSpan _NowSpan = new TimeSpan(_user.Post_lastTimes.Day, _user.Post_lastTimes.Hour, _user.Post_lastTimes.Minute, _user.Post_lastTimes.Second);
		if (!_user.MarkChanges[MarkTitleType.NewsCount])
		{
			bool isChanged = false;
			if (PlayerPrefs.HasKey(DefineKey.PostCount))
			{
			
				int CheckedPostCount = PlayerPrefs.GetInt(DefineKey.PostCount); //확인한 우편물갯수
				if (PlayerPrefs.HasKey("LastPostTimeSeconds"))
				{
					float _lastPostTimes = PlayerPrefs.GetFloat("LastPostTimeSeconds");
					if (CheckedPostCount <= 0) //확인한 우편물이 없다
					{
						if (_lastPostTimes < (float)_NowSpan.TotalSeconds) // 이전우편시간보다 최근우편시간이 클떄
						{
							UserEditor.Getsingleton.EditLog("우편물 없고 최근우편받은시간이 크다");
							isChanged = true;
						}
						else
						{
							UserEditor.Getsingleton.EditLog("우편물 없고 이전우편받은시간이 크다");
							isChanged = false;
						}
						PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)_NowSpan.TotalSeconds);

					}
					else if (CheckedPostCount > 0) // 확인한 우편물이 있다
					{
						//if (_lastPostTimes < (float)_NowSpan.TotalSeconds) 
						{
							UserEditor.Getsingleton.EditLog("우편물이 있다");
							isChanged = true;
							PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)_NowSpan.TotalSeconds);
						}
						//else
						//isChanged = false;
					}
				}
				else
				{
					UserEditor.Getsingleton.EditLog("우편물 키값있고 , 시간키값 없다");
					isChanged = true;
					TimeSpan span = new TimeSpan(_user.Post_lastTimes.Day, _user.Post_lastTimes.Hour, _user.Post_lastTimes.Minute, _user.Post_lastTimes.Second);
					PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)span.TotalSeconds);
				}


			}
			else
			{
				if (PlayerPrefs.HasKey("LastPostTimeSeconds"))
				{
					float _lastPostTimes = PlayerPrefs.GetFloat("LastPostTimeSeconds");
					if (_lastPostTimes < (float)_NowSpan.TotalSeconds) // 이전우편시간보다 최근우편시간이 클떄
					{
						UserEditor.Getsingleton.EditLog("우편갯수 키값없다, 최근우편시간이크다 ");
						isChanged = true;
						PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)_NowSpan.TotalSeconds);
					}
					else
					{
						UserEditor.Getsingleton.EditLog("우편갯수 키값없다, 이전우편시간이크다 ");

						isChanged = false;
					}
				}
				else
				{
					UserEditor.Getsingleton.EditLog("우편갯수 키값없다, 시간 키값없다");
					isChanged = false;
					TimeSpan span = new TimeSpan(_user.Post_lastTimes.Day, _user.Post_lastTimes.Hour, _user.Post_lastTimes.Minute, _user.Post_lastTimes.Second);
					PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)span.TotalSeconds);
				}
			}


			PlayerPrefs.Save();
			_user.MarkChanges[MarkTitleType.NewsCount] = isChanged;

			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
				UI_Lobby.Getsingleton.Set_newsMark();

		} // enf of if(_user.MarkChanges[MarkTitleType.NewsCount])
		else // 마크가 켜져 있을떄
		{
			PlayerPrefs.SetFloat("LastPostTimeSeconds", (float)_NowSpan.TotalSeconds);
		}
	}
	#endregion
	

	#region Public_UsersData
	public static void Public_UsersData(User _user, Dictionary<string, object> _data)
	{
		uint _userId = Convert.ToUInt32(_data[UserId]);
		string _Lgntoken = (string)_data[LgnToken];
		string _nknm = (string)_data[Nknm];

		if (_data.ContainsKey(UserId))
			_user.user_Users.UserID = _userId;
		if (_data.ContainsKey(LgnToken))
			_user.user_Users.LgnToken = _Lgntoken;
		if (_data.ContainsKey(Nknm))
			_user.user_Users.NkNm = _nknm;
		if (_data.ContainsKey(ClanName))
			_user.user_Users.ClanName = Convert.ToString(_data[ClanName]);
		if (_data.ContainsKey(ClanMark))
			_user.user_Users.ClanMark = Convert.ToUInt16(_data[ClanMark]);
		if (_data.ContainsKey(CtrCd))
		{
			_user.user_Users.CtrCd = Convert.ToString(_data[CtrCd]);

		
		}

		//GetResultInfoList.Add(new GainItem(_userId, _Lgntoken, _nknm));
	}
	#endregion

	#region Public_UserLoginData
	public static void Public_UserLoginData(User _user, Dictionary<string, object> _data)
	{
		LanguageCode _LggCd = (LanguageCode)Convert.ToByte(_data[LggCd]);
		byte _ClanPush = Convert.ToByte(_data[ClanPush]);

		if (_data.ContainsKey(LggCd))
			_user.user_logins.LggCd = _LggCd;
		if (_data.ContainsKey(ClanPush))
			_user.user_logins.ClanPush = _ClanPush;



		//GetResultInfoList.Add(new GainItem(_userId, _Lgntoken, _nknm));
	}
	#endregion

	#region Public_GetPostList_Data
	public static void ResponseWeb_GetPostList_Data(User _user, Dictionary<string, object> _data)
	{
		if (_data.ContainsKey(JsonKey.DATA))
		{
			Dictionary<string, object> _dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];
			if (_dic_data.ContainsKey(JsonKey.USER_POSTS_COUNT))
				_user.User_Post_Count = (long)_dic_data[JsonKey.USER_POSTS_COUNT];

			if (_dic_data.ContainsKey(JsonKey.USER_POSTS))
			{
				List<object> _lst = (List<object>)_dic_data[JsonKey.USER_POSTS];

				//갱신되 소식들 받을수 잇으므로 클리어하자GetResultInfoList.Clear();
				_user.User_Posts.Clear();


				for (int i = 0; i < _lst.Count; i++)
				{
					Dictionary<string, object> _dic_posts = (Dictionary<string, object>)_lst[i];
					User_Posts _post = new User_Posts();

					if (_dic_posts.ContainsKey(PostIdx))
						_post.PostIdx = Convert.ToUInt32(_dic_posts[PostIdx]);
					if (_dic_posts.ContainsKey(SndUserID))
						_post.SndUserID = Convert.ToUInt32(_dic_posts[SndUserID]);
					if (_dic_posts.ContainsKey(SndClanID))
						_post.SndClanID = Convert.ToUInt32(_dic_posts[SndClanID]);
					if (_dic_posts.ContainsKey(SndNkNm))
						_post.SndNkNm = Convert.ToString(_dic_posts[SndNkNm]);
					if (_dic_posts.ContainsKey(GetFlg))
						_post.GetFlg = Convert.ToByte(_dic_posts[GetFlg]);
					if (_dic_posts.ContainsKey(PstPasIdx))
						_post.PstPasIdx = Convert.ToByte(_dic_posts[PstPasIdx]);
					if (_dic_posts.ContainsKey(ItTp))
						_post.ItTp = (ITEMTYPE)Enum.Parse(typeof(ITEMTYPE), _dic_posts[ItTp].ToString());
					if (_dic_posts.ContainsKey(ItIdx))
						_post.ItIdx = Convert.ToUInt32(_dic_posts[ItIdx]);
					if (_dic_posts.ContainsKey(PostTxt))
						_post.PostTxt = Convert.ToString(_dic_posts[PostTxt]);
					if (_dic_posts.ContainsKey(ExpiryDay))
						_post.ExpiryDay = Convert.ToByte(_dic_posts[ExpiryDay]);
					if (_dic_posts.ContainsKey(ItNum))
						_post.ItNum = Convert.ToUInt32(_dic_posts[ItNum]);
					if (_dic_posts.ContainsKey(ctime))
						_post.ctime = Check_returnTime(_dic_posts[ctime]);


					_user.User_Posts[_post.PostIdx] = _post;
				}
			}
			else
			{
				_user.User_Posts.Clear();

			}
		

		}
		
	}
	#endregion


	

	#region Public_UserGoodsData
	public static void Public_UserGoodsData(User _user, Dictionary<string, object> _data)
	{
	
		if(_data.Count > 0 )
			GetResultInfoList.Clear();

		foreach (var good in _data)
		{
			Dictionary<string, object> _dic_item = (Dictionary<string, object>)good.Value;

			ITEMTYPE _Ittp = (ITEMTYPE)Enum.Parse(typeof(ITEMTYPE), _dic_item[ItTp].ToString());
			int _ItNum = Convert.ToInt32(_dic_item[ItNum]);

			DateTime _mtime = DateTime.MinValue;
			if (_dic_item.ContainsKey(mtime))
				_mtime = Convert.ToDateTime(_dic_item[mtime]);

			DateTime _ctime = DateTime.MinValue;
			if (_dic_item.ContainsKey(ctime))
				_ctime = Convert.ToDateTime(_dic_item[ctime]);


			User_Good _usergood = new User_Good(_Ittp, _ItNum, _mtime);

			int _haveNum = _user.Get_user_goods(_Ittp);

			_user.User_Goods[_usergood.ItTp] = _usergood;



			//gain아이템 ===========
			if (_Ittp == ITEMTYPE.ATDDAY)
			{
				if (_user.IsChkAttd)
				{
					if (_ItNum != _haveNum)
						GetResultInfoList.Add(new GainItem(_Ittp, 0, _ItNum, _mtime, _ctime));
				}
					//if (_ItNum == _haveNum && _haveNum != 1)
					//{
					//    GetResultInfoList.Add(new GainItem(_Ittp, 0, _ItNum, _mtime, _ctime));
					//}
					//else if (_ItNum != _haveNum && _haveNum != 0)
					//    GetResultInfoList.Add(new GainItem(_Ittp, 0, _ItNum, _mtime, _ctime));


					
			}
			else
			{
				if (_ItNum != _haveNum)
				{
					if (_ItNum - _haveNum > 0)
						GetResultInfoList.Add(new GainItem(_Ittp, 0, _ItNum - _haveNum, _mtime, _ctime));

				}
			}
		}
		
	}
	#endregion

	#region Public_User_UnitData
	public static void Public_User_UnitData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pUnit in _data)
		{
			Dictionary<string, object> _unitIdx = (Dictionary<string, object>)pUnit.Value;
			User_Units _unit = new User_Units();
			bool isRankup = false;

			if (_unitIdx.ContainsKey(UnitIdx))
				_unit.Unitidx = Convert.ToUInt32(_unitIdx[UnitIdx]);
			if (_unitIdx.ContainsKey(UnitRk))
			{
				_unit.UnitRk = Convert.ToUInt16(_unitIdx[UnitRk]);
				if (_unit.UnitRk == 0)
					_unit.UnitRk = 1;
			}
			if (_unitIdx.ContainsKey(UnitExp))
				_unit.UnitExp = Convert.ToUInt32(_unitIdx[UnitExp]);
			if (_unitIdx.ContainsKey(RefLv))
				_unit.RefLv = Convert.ToByte(_unitIdx[RefLv]);
			if (_unitIdx.ContainsKey(RefFailCnt))
				_unit.RefFailCnt = Convert.ToByte(_unitIdx[RefFailCnt]);
			if (_unitIdx.ContainsKey(MainWpnIdx))
				_unit.MainWpnIdx = Convert.ToUInt32(_unitIdx[MainWpnIdx]);
			if (_unitIdx.ContainsKey(SubWpnIdx))
				_unit.SubWpnIdx = Convert.ToUInt32(_unitIdx[SubWpnIdx]);
			if (_unitIdx.ContainsKey(DecoIdx1))
				_unit.DecoIdx1 = Convert.ToInt32(_unitIdx[DecoIdx1]);
			if (_unitIdx.ContainsKey(DecoIdx2))
				_unit.DecoIdx2 = Convert.ToInt32(_unitIdx[DecoIdx2]);
			if (_unitIdx.ContainsKey(DecoIdx3))
				_unit.DecoIdx3 = Convert.ToInt32(_unitIdx[DecoIdx3]);
			if (_unitIdx.ContainsKey(SubSkill))
				_unit.SubSkill = Convert.ToByte(_unitIdx[SubSkill]);


			//gain
			if (_user.User_Units.ContainsKey(_unit.Unitidx))
			{

				User_Units _bfUnit = _user.User_Units[Convert.ToUInt32(_unitIdx[UnitIdx])];

				//isChanged = _unit.IsChanged(_bfUnit);

				if (_unit.UnitRk > _bfUnit.UnitRk)
					isRankup = true;


				//if (isChanged)
				{
					GainUserUnit _gain = new GainUserUnit();
					_gain.Unitidx = _unit.Unitidx;
					_gain.UnitExp = _unit.UnitExp - _bfUnit.UnitExp;
					_gain.UnitRk = _unit.UnitRk;
					_gain.isRankUp = isRankup;
					_gain.SubSkillIdxDiff = _bfUnit.SubSkill - _unit.SubSkill ;
					Chk_gainUnit(_gain);
				}
			}

			_user.User_Units[_unit.Unitidx] = _unit;

		}

	

	}
	#endregion

	


	#region Public_User_WeaponsData
	public static void Public_User_WeaponsData(User _user, Dictionary<string, object> _data)
	{
		
			

			foreach (KeyValuePair<string, object> Wp in _data)
			{
				Dictionary<string, object> _weaponInfo = (Dictionary<string, object>)Wp.Value;
				User_weapon _weapon = new User_weapon();

				//if (_weaponInfo.ContainsKey(UnitIdx))
				//	_weapon.UnitIdx = Convert.ToUInt32(_weaponInfo[UnitIdx]);
				if (_weaponInfo.ContainsKey(WpnIdx))
					_weapon.WpnIdx = Convert.ToUInt32(_weaponInfo[WpnIdx]);
				if (_weaponInfo.ContainsKey(RefLv))
					_weapon.RefLv = Convert.ToByte(_weaponInfo[RefLv]);
				if (_weaponInfo.ContainsKey(RefFailCnt))
					_weapon.RefFailCnt = Convert.ToByte(_weaponInfo[RefFailCnt]);



            _user.User_Weapons[_weapon.WpnIdx] = _weapon;

        }

			
			

		
	}
	#endregion


	#region Public_User_DecosData
	public static void Public_User_DecosData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pUnit in _data)
		{
			uint unitIdx = Convert.ToUInt32(pUnit.Key);
			Dictionary<string, object> _dic_deco = (Dictionary<string, object>)pUnit.Value;
			Dictionary<uint, user_Deco> _dicNewDeco = null;
			if (_user.User_Decos.ContainsKey(unitIdx))
				_dicNewDeco = _user.User_Decos[unitIdx];
			else
				_dicNewDeco = new Dictionary<uint, user_Deco>();
			int idx = 0;
			foreach (KeyValuePair<string, object> dc in _dic_deco)
			{
				idx++;
				Dictionary<string, object> _decoInfo = (Dictionary<string, object>)dc.Value;
				user_Deco _deco = new user_Deco();

				if (_decoInfo.ContainsKey(UnitIdx))
					_deco.Unitidx = Convert.ToUInt32(_decoInfo[UnitIdx]);
				if (_decoInfo.ContainsKey(DecoIdx))
					_deco.DecoIdx = Convert.ToUInt32(_decoInfo[DecoIdx]);

				_dicNewDeco[_deco.DecoIdx] = _deco;
			}
			



			_user.User_Decos[unitIdx] = _dicNewDeco;
		}
	}
	#endregion




	#region Public_User_UseUnitData
	public static void Public_User_UseUnitData(User _user, Dictionary<string, object> _data)
	{
		//foreach (KeyValuePair<string, object> pUnit in _data)
		{
			Dictionary<string, object> _unitIdx = (Dictionary<string, object>)_data;

			if (_unitIdx.ContainsKey(UnitIdx))
				_user.User_useUnit.UnitIdx = Convert.ToUInt32(_unitIdx[UnitIdx]);
            if (_unitIdx.ContainsKey(UnitIdx1))
                _user.User_useUnit.UnitIdxs[0] = Convert.ToUInt32(_unitIdx[UnitIdx1]);
            if (_unitIdx.ContainsKey(UnitIdx2))
                _user.User_useUnit.UnitIdxs[1] = Convert.ToUInt32(_unitIdx[UnitIdx2]);
            if (_unitIdx.ContainsKey(UnitIdx3))
                _user.User_useUnit.UnitIdxs[2] = Convert.ToUInt32(_unitIdx[UnitIdx3]);



        }

	}
	#endregion


	#region Public_User_Unit_UseInfosData
	public static void Public_User_Unit_UseInfosData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pUnit in _data)
		{
			Dictionary<string, object> _dic_useInfos = (Dictionary<string, object>)pUnit.Value;
			User_unit_useInfo unitUseInfos = new User_unit_useInfo();

			if (_dic_useInfos.ContainsKey(UnitIdx))
				unitUseInfos.UnitIdx = Convert.ToUInt32(_dic_useInfos[UnitIdx]);
			if (_dic_useInfos.ContainsKey(UseCnt))
			{
				unitUseInfos.UseCnt = Convert.ToUInt32(_dic_useInfos[UseCnt]);
			}
			if (_dic_useInfos.ContainsKey(KillCnt))
			{
				unitUseInfos.KillCnt = Convert.ToUInt32(_dic_useInfos[KillCnt]);
			}
			if (_dic_useInfos.ContainsKey(DeathCnt))
			{
				unitUseInfos.DeathCnt = Convert.ToUInt32(_dic_useInfos[DeathCnt]);
			}


			if (_user.User_unit_useInfos.ContainsKey(unitUseInfos.UnitIdx))
			{
				User_unit_useInfo _bfUnitUseInfo = _user.User_unit_useInfos[unitUseInfos.UnitIdx];

				GainUserUnit _gain = new GainUserUnit();
				_gain.Unitidx = unitUseInfos.UnitIdx;
				_gain.KillCnt = unitUseInfos.KillCnt - _bfUnitUseInfo.KillCnt;
				_gain.DeathCnt = unitUseInfos.DeathCnt - _bfUnitUseInfo.DeathCnt;
				_gain.UseCnt = unitUseInfos.UseCnt - _bfUnitUseInfo.UseCnt;

				Chk_gainUnit(_gain);
			}
			else
			{
				GainUserUnit _gain = new GainUserUnit();
				_gain.Unitidx = unitUseInfos.UnitIdx;
				_gain.KillCnt = unitUseInfos.KillCnt;
				_gain.DeathCnt = unitUseInfos.DeathCnt;
				_gain.UseCnt = unitUseInfos.UseCnt;

				Chk_gainUnit(_gain);
			}

			_user.User_unit_useInfos[unitUseInfos.UnitIdx] = unitUseInfos;

		}

	}
	#endregion



	#region Public_UserWordsData
	public static void Public_UserWordsData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pUnit in _data)
		{
			Dictionary<string, object> _dic_usewords = (Dictionary<string, object>)pUnit.Value;
			User_Word word = new User_Word();

			if (_dic_usewords.ContainsKey(WrdKind))
				word.WrdKind = (USERWORD_TYPE)Convert.ToByte(_dic_usewords[WrdKind]);
			if (_dic_usewords.ContainsKey(Words))
				word.Words = Convert.ToString(_dic_usewords[Words]);


			_user.User_Words[word.WrdKind] = word;


		}

		

		//자살문구 디폴트 세팅
		User_Word word1 = new User_Word();
		word1.WrdKind = USERWORD_TYPE.WRD_SUICIDE;
		word1.Words = TextDataManager.Dic_TranslateText[438]; //자살문구 
		_user.User_Words[word1.WrdKind] = word1;

	

	}
	#endregion

	#region Public_User_ADsdata
	public static void Public_User_AdsData(User _user, Dictionary<string, object> _data)
	{

		if (_data.ContainsKey(UserId))
			_user.User_Ads.UserID = Convert.ToUInt16(_data[UserId]);
		if (_data.ContainsKey(AdTm))
			_user.User_Ads.AdTm = Check_returnTime(_data[AdTm]);
		if (_data.ContainsKey(RwdItTp))
			_user.User_Ads.RwdItTp = (ITEMTYPE)Convert.ToByte(_data[RwdItTp]);
		if (_data.ContainsKey(AdShowCnt))
			_user.User_Ads.AdShowCnt = Convert.ToUInt32(_data[AdShowCnt]);
        if (_data.ContainsKey(PlayGoldRwd))
            _user.User_Ads.PlayGoldRwd = Convert.ToUInt16(_data[PlayGoldRwd]);
        if (_data.ContainsKey(PlayExpRwd))
            _user.User_Ads.PlayExpRwd = Convert.ToUInt16(_data[PlayExpRwd]);
        if (_data.ContainsKey(LvRwd))
            _user.User_Ads.LvRwd = Convert.ToUInt16(_data[LvRwd]);
        if (_data.ContainsKey(mtime))
			_user.User_Ads.mtime = Check_returnTime(_data[mtime]);
		if (_data.ContainsKey(ctime))
			_user.User_Ads.ctime = Check_returnTime(_data[ctime]);


	}
	#endregion


	

	#region Public_UserTimeData
	public static void Public_UserTimeData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pUsertime in _data)
		{
			Dictionary<string, object> _dicUserTime = (Dictionary<string, object>)pUsertime.Value;
			
			User_Times _userTime = new User_Times();

			if (_dicUserTime.ContainsKey(TmIdx))
			{
				if (Convert.ToInt32(_dicUserTime[TmIdx]) != 0)
				_userTime.TmIdx = Convert.ToInt32(_dicUserTime[TmIdx]);
			}
			if (_dicUserTime.ContainsKey(TmSubIdx))
			{
				if (Convert.ToInt32(_dicUserTime[TmSubIdx]) != 0)
				_userTime.TmSubIdx = Convert.ToInt32(_dicUserTime[TmSubIdx]);
			}
			if (_dicUserTime.ContainsKey(stime))
			{
			    //if (Convert.ToDateTime(_dicUserTime[stime])!= DateTime.MinValue)
			    //_userTime.Stime = Convert.ToDateTime(_dicUserTime[stime]);
				_userTime.Stime = Check_returnTime(_dicUserTime[stime]);
			}
			if (_dicUserTime.ContainsKey(etime))
			{
			    //if (Convert.ToDateTime(_dicUserTime[etime]) != DateTime.MinValue)
			    //_userTime.Etime = Convert.ToDateTime(_dicUserTime[etime]);
				_userTime.Etime = Check_returnTime(_dicUserTime[etime]);

			}


			_user.User_Times[_userTime.TmIdx] = _userTime;

			//안넣어도되지않나?
			//GetResultInfoList.Add(new GainInfo(_userTime.TmIdx, _userTime.TmSubIdx, _userTime.Stime, _userTime.Etime));
		}

	}
	#endregion

	#region Check_returnTime
	public static DateTime Check_returnTime(object _timeValue)
	{
		string _strValue = (string)_timeValue;

		if (string.IsNullOrEmpty(_strValue) || string.Equals(_strValue, JsonKey.NullTime)) 
			return DateTime.MinValue;
		else 
			return Convert.ToDateTime(_timeValue);

	}
	#endregion









	#region Public_UserGamesData
	public static void Public_userGameData(User _user, Dictionary<string, object> _data)
	{

		if (_data.ContainsKey(PlayCnt))
			_user.user_Games.PlayCnt = Convert.ToUInt32(_data[PlayCnt]);
	}
#endregion



	#region Public_userDrawSkillData
	public static void Public_userDrawSkillData(User _user, Dictionary<string, object> _data)
	{
		foreach (KeyValuePair<string, object> pds in _data)
		{
			Dictionary<string, object> _dic_drawskill = (Dictionary<string, object>)pds.Value;
			User_drawSkill _userdrawskill = new User_drawSkill();

			if (_dic_drawskill.ContainsKey(UnitIdx))
				_userdrawskill.UnitIdx = Convert.ToUInt32(_dic_drawskill[UnitIdx]);
			if (_dic_drawskill.ContainsKey(SubSkill))
				_userdrawskill.SubSkill = Convert.ToByte(_dic_drawskill[SubSkill]);

			_user.User_DrawSkills[_userdrawskill.UnitIdx] = _userdrawskill;
		}
	}
	#endregion



	#region Public_User_ClansData
	public static void Public_User_ClansData(User _user, Dictionary<string, object> _data)
	{
		User_Clans _userclan = new User_Clans();


		_userclan.ClanID = Convert.ToUInt16(_data[ClanID]);
		_userclan.RwdIdx = Convert.ToByte(_data[RwdIdx]);
		_userclan.JoinTmLm = Check_returnTime(_data[JoinTmLm]);


		

		_user.user_Clans = _userclan;

	}
	#endregion

	#region Public_Clan_ClansData
	public static void Public_Clan_ClansData(User _user, Dictionary<string, object> _data)
	{
		Clans _clans = new Clans();

		if (_data.ContainsKey(ClanID))
			_clans.ClanID = Convert.ToUInt16(_data[ClanID]);
		if (_data.ContainsKey(ClanName))
			_clans.ClanName = Convert.ToString(_data[ClanName]);
		if (_data.ContainsKey(ClanMark))
			_clans.ClanMark = Convert.ToUInt16(_data[ClanMark]);
		if (_data.ContainsKey(ClanInfoTxt))
			_clans.ClanInfoTxt = Convert.ToString(_data[ClanInfoTxt]);
		if (_data.ContainsKey(PersonsCnt))
			_clans.PersonCnt = Convert.ToByte(_data[PersonsCnt]);
		if (_data.ContainsKey(GoldBufTm))
			_clans.GoldBufTm = Check_returnTime(_data[GoldBufTm]);
		if (_data.ContainsKey(ExpBufTm))
			_clans.ExpBufTm = Check_returnTime(_data[ExpBufTm]);


		_user.clan_Clans = _clans;

	}
	#endregion

	#region Public_Clan_ClansMemberData (클랜의 멤버들 정보)
	public static void Public_Clan_ClansMemberData(User _user, Dictionary<string, object> _data)
	{
		foreach (var clanmember in _data)
		{
			Dictionary<string, object> _dic_member = (Dictionary<string, object>)clanmember.Value;

			Clan_members _clan_member = new Clan_members();

			if (_dic_member.ContainsKey(MemIdx))
				_clan_member.MemIdx = Convert.ToByte(_dic_member[MemIdx]);
			if (_dic_member.ContainsKey(CnRank))
				_clan_member.CnRank = Convert.ToByte(_dic_member[CnRank]);
			if (_dic_member.ContainsKey(UserId))
				_clan_member.UserID = Convert.ToUInt32(_dic_member[UserId]);
			if (_dic_member.ContainsKey(Nknm))
				_clan_member.NkNm = Convert.ToString(_dic_member[Nknm]);
			if (_dic_member.ContainsKey(JoinTime))
				_clan_member.JoinTime = Check_returnTime(_dic_member[JoinTime]);
			if (_dic_member.ContainsKey(mtime))
				_clan_member.mtime = Check_returnTime(_dic_member[mtime]);
			if (_dic_member.ContainsKey(SvIdx))
				_clan_member.SvIdx = Convert.ToByte(_dic_member[SvIdx]);
			if (_dic_member.ContainsKey(CtrCd))
				_clan_member.CtrCd = Convert.ToString(_dic_member[CtrCd]);
            if (_dic_member.ContainsKey(UserLv))
            {
                _clan_member.UserLv = Convert.ToUInt16(_dic_member[UserLv]);
                _clan_member.UserLv = _clan_member.UserLv == 0 ? (ushort)1 : _clan_member.UserLv;
            }
            _user.Clan_members[_clan_member.MemIdx] = _clan_member;

		}

		
		//clan_members 가 갱신이 되면 clan_member(본유저의 clan멤버정보)를 갱신 해주자. 안그러면 번거롭게 getuserIfo 프로토콜호출해야한다
		if (_data.Count > 0 && _user.Clan_members.Count >0)
		{
			var myClanMember = from member in _user.Clan_members
							   where member.Value.UserID == _user.user_Users.UserID
							   select member;

		

			foreach (var mem in myClanMember)
			{
				_user.MyUserClan_member.MemIdx = mem.Value.MemIdx;
				_user.MyUserClan_member.CnRank = mem.Value.CnRank;
				_user.MyUserClan_member.UserID = mem.Value.UserID;
				_user.MyUserClan_member.JoinTime = mem.Value.JoinTime;
				_user.MyUserClan_member.mtime = mem.Value.mtime;
			}


			//클랜전 완료 될때 얻은 클랜 점수를 위해 gainitem 화 해야한다.
	

		

		}
	}
	#endregion


	#region Public_Clan_UserClanMemberData (유저의 멤버 정보)
	public static void Public_Clan_UserClanMemberData(User _user, Dictionary<string, object> _data)
	{
		
			Clan_members _clan_member = new Clan_members();

			if(_data.ContainsKey(ClanID))
			_clan_member.ClanID = Convert.ToUInt32(_data[ClanID]);
			if (_data.ContainsKey(MemIdx))
			_clan_member.MemIdx = Convert.ToByte(_data[MemIdx]);
			if (_data.ContainsKey(CnRank))
			_clan_member.CnRank = Convert.ToByte(_data[CnRank]);
			if (_data.ContainsKey(UserId))
			_clan_member.UserID = Convert.ToUInt32(_data[UserId]);
			if (_data.ContainsKey(Nknm))
			_clan_member.NkNm = Convert.ToString(_data[Nknm]);
			if (_data.ContainsKey(JoinTime))
			_clan_member.JoinTime = Check_returnTime(_data[JoinTime]);
			if (_data.ContainsKey(mtime))
			_clan_member.mtime = Check_returnTime(_data[mtime]);
			if (_data.ContainsKey(ctime))
			_clan_member.ctime = Check_returnTime(_data[ctime]);

			if (UserDataManager.instance.isOtherUser == false)
			{
				_user.MyUserClan_member = _clan_member;
			}
			else
			{
				_user.OtherUserClan_member = _clan_member;
			}


	}
	#endregion

	#region Public_Clan_ClanBoardsData
	public static void Public_Clan_ClanBoardsData(User _user, Dictionary<string, object> _data)
	{
		foreach (var clanBoard in _data)
		{
			Dictionary<string, object> _dic_board = (Dictionary<string, object>)clanBoard.Value;

			Clan_Boards _clan_board = new Clan_Boards();

			if(_dic_board.ContainsKey(BodIdx))
			_clan_board.BodIdx= Convert.ToUInt16(_dic_board[BodIdx]);
			if (_dic_board.ContainsKey(BodKind))
			_clan_board.BodKind = Convert.ToByte(_dic_board[BodKind]);
			if (_dic_board.ContainsKey(UserId))
			_clan_board.UsetID = Convert.ToUInt32(_dic_board[UserId]);
			if (_dic_board.ContainsKey(Nknm))
			_clan_board.NkNm = Convert.ToString(_dic_board[Nknm]);
			if (_dic_board.ContainsKey(BodTxt))
			_clan_board.BodTxt = Convert.ToString(_dic_board[BodTxt]);
			if (_dic_board.ContainsKey(mtime))
			_clan_board.mtime = Check_returnTime(_dic_board[mtime]);

			_user.Clan_boards[_clan_board.BodIdx] = _clan_board;

		}

	}
	#endregion

	#region Response : GetUnitExpRanking_Data
	public static void ResponseWeb_GetUnitExpRanking_Data(User _user, Dictionary<string, object> _data)
	{
		if (_data.ContainsKey(JsonKey.DATA))
		{
			Dictionary<string, object> _dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];
			if (_dic_data.ContainsKey(JsonKey.MYRANKING))
			{
				int _ItNum = Convert.ToInt32(_dic_data[JsonKey.MYRANKING]);
				User_Good _usergood = new User_Good(ITEMTYPE.RANK, _ItNum,DateTime.MinValue);
				_user.User_Goods[ITEMTYPE.RANK] = _usergood;

			}

			//전에 담은거 업애기위해 클리어하자
			_user.Rank_UnitExps.Clear();

			if (_dic_data.ContainsKey(JsonKey.Rank_UnitExp))
			{
				List<object> _lst = (List<object>)_dic_data[JsonKey.Rank_UnitExp];

				for (int i = 0; i < _lst.Count; i++)
				{
					Dictionary<string, object> _dic_rank = (Dictionary<string, object>)_lst[i];

					//webRequest.nowSearchRankingUnitIdx = 0 (전체랭캉) 이고 반환값의 Unitidx 가 0 이면 담지 않는다
					//if (webRequest.nowSearchRankingUnitIdx == 0 && _dic_rank.ContainsKey(UnitIdx) && Convert.ToUInt32(_dic_rank[UnitIdx]) == 0)
					//	continue;

					Rank_UnitExp _rank = new Rank_UnitExp();

					if (_dic_rank.ContainsKey(UserId))
						_rank.UserID = Convert.ToUInt32(_dic_rank[UserId]);
					if (_dic_rank.ContainsKey(CtrCd))
						_rank.CtrCd = Convert.ToString(_dic_rank[CtrCd]);
					if (_dic_rank.ContainsKey(Nknm))
						_rank.NkNm = Convert.ToString(_dic_rank[Nknm]);
                    if (_dic_rank.ContainsKey(UserLv))
                    {
                        _rank.UserLv = Convert.ToUInt16(_dic_rank[UserLv]);
                        _rank.UserLv = _rank.UserLv == 0 ? (ushort)1 : _rank.UserLv;
                    }
                    if (_dic_rank.ContainsKey(ClanMark))
						_rank.ClanMark = Convert.ToUInt16(_dic_rank[ClanMark]);
					if (_dic_rank.ContainsKey(ClanName))
						_rank.ClanName = Convert.ToString(_dic_rank[ClanName]);
					if (_dic_rank.ContainsKey(Ranking))
						_rank.Ranking = Convert.ToUInt32(_dic_rank[Ranking]);
					if (_dic_rank.ContainsKey(Exp))
						_rank.Exp = Convert.ToUInt32(_dic_rank[Exp]);
					if (_dic_rank.ContainsKey(KillDeath))
						_rank.KillDeath = Convert.ToString(_dic_rank[KillDeath]);
					if (_dic_rank.ContainsKey(UnitIdx))
						_rank.Unitidx = Convert.ToUInt32(_dic_rank[UnitIdx]);


					_user.Rank_UnitExps[_rank.UserID] = _rank;
				}
				_user.Rank_DicUnitExps[webRequest.nowSearchRankingUnitIdx] = _user.Rank_UnitExps;
			}
		}

		else if (_data.ContainsKey(JsonKey.ERRORCODE))
		{

			int errorIdx = Convert.ToInt16(_data[JsonKey.ERRORCODE]);
			BeforeErrorProcess(errorIdx);
		}


		//현재 유저의 유닛의 랭킹을 저장하자
		if (_user.Rank_UnitExps.ContainsKey(_user.user_Users.UserID))
		{
			Rank_UnitExp _rank = new Rank_UnitExp();
			_rank = _user.Rank_UnitExps[_user.user_Users.UserID];

			_user.User_UnitRanking[webRequest.nowSearchRankingUnitIdx] = _rank;
		}
		
	}
	#endregion


	

	#region Response : FriendsList_Data
	public static void ResponseWeb_FriendsList_Data(User _user, Dictionary<string, object> _data)
	{
		_user.User_Friends.Clear();

		if (_data.ContainsKey(JsonKey.DATA))
		{
			Dictionary<string, object> _dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];


			if (_dic_data.ContainsKey(JsonKey.USER_FRIENDS))
			{
				Dictionary<string, object> _dic_friends = (Dictionary<string, object>)_dic_data[JsonKey.USER_FRIENDS];

				foreach (var friend in _dic_friends)
				{
					Dictionary<string, object> _friendData = (Dictionary<string, object>)friend.Value;

					User_Friends _friend = new User_Friends();

					if (_friendData.ContainsKey(FrUserID))
						_friend.FrUserID = Convert.ToUInt32(_friendData[FrUserID]);
					if (_friendData.ContainsKey(Nknm))
						_friend.NkNm = Convert.ToString(_friendData[Nknm]);
					if (_friendData.ContainsKey(CtrCd))
						_friend.CtrCd = Convert.ToString(_friendData[CtrCd]);
					if (_friendData.ContainsKey(ClanMark))
						_friend.ClanMark = Convert.ToUInt16(_friendData[ClanMark]);
					if (_friendData.ContainsKey(ClanName))
						_friend.ClanName = Convert.ToString(_friendData[ClanName]);
					if (_friendData.ContainsKey(SvIdx))
						_friend.SvIdx = Convert.ToByte(_friendData[SvIdx]);
					if (_friendData.ContainsKey(mtime))
						_friend.mtime = Check_returnTime(_friendData[mtime]);

					_friend.FrIdx = Convert.ToByte(friend.Key);

					_user.User_Friends[_friend.FrUserID] = _friend;

				}
			}


		}

	}
	#endregion

	#region Response : FriendInviteUrl_Data
	public static void ResponseWeb_FriendInviteUrl_Data(User _user, Dictionary<string, object> _data, string url)
	{
		if (_data.ContainsKey(JsonKey.DATA))
		{
			Dictionary<string, object> _dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];


			if (_dic_data.ContainsKey(Url))
				_user.InviteURL = string.Format("{0}{1}", url, Convert.ToString(_dic_data[Url]));

		}

	}
	#endregion


	#region Response : ClanList_Data
	public static void ResponseWebClanList_Data(User _user, Dictionary<string, object> _data)
	{
		if (_data.ContainsKey(JsonKey.DATA))
		{
			Dictionary<string, object> _dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];
			
			if (_dic_data.ContainsKey(JsonKey.CLAN_LIST))
			{
				List<object> _lst = (List<object>)_dic_data[JsonKey.CLAN_LIST];

				for (int i = 0; i < _lst.Count; i++)
				{
					Dictionary<string, object> _dic_Clanlst = (Dictionary<string, object>)_lst[i];
					Clan_Lists _rmClan = new Clan_Lists();

					if (_dic_Clanlst.ContainsKey(ClanID))
						_rmClan.ClanID = Convert.ToUInt32(_dic_Clanlst[ClanID]);
					if (_dic_Clanlst.ContainsKey(ClanMark))
						_rmClan.ClanMark = Convert.ToUInt16(_dic_Clanlst[ClanMark]);
					if (_dic_Clanlst.ContainsKey(ClanName))
						_rmClan.ClanName = Convert.ToString(_dic_Clanlst[ClanName]);
					if (_dic_Clanlst.ContainsKey(Nknm))
						_rmClan.NkNm = Convert.ToString(_dic_Clanlst[Nknm]);
					if (_dic_Clanlst.ContainsKey(CtrCd))
						_rmClan.CtrCd = Convert.ToString(_dic_Clanlst[CtrCd]);
					if (_dic_Clanlst.ContainsKey(MemberCnt))
						_rmClan.MemberCnt = Convert.ToByte(_dic_Clanlst[MemberCnt]);
					if (_dic_Clanlst.ContainsKey(PersonsCnt))
						_rmClan.PersonCnt = Convert.ToByte(_dic_Clanlst[PersonsCnt]);
					if (_dic_Clanlst.ContainsKey(CnPointSum0))
						_rmClan.CnPointSum0 = Convert.ToUInt16(_dic_Clanlst[CnPointSum0]);
					if (_dic_Clanlst.ContainsKey(CnPointSum1))
						_rmClan.CnPointSum1 = Convert.ToUInt16(_dic_Clanlst[CnPointSum1]);

					_user.Clan_clanLists[_rmClan.ClanID] = _rmClan;
				}
			}
		


		}

	}
	#endregion


	#region Get List GainiIfo
	
	public static void Get_ListGainitem(Dictionary<string, object> _data)
	{
		Resp_Error = false;
		Dic_data.Clear();

		if(_data.ContainsKey(JsonKey.DATA))
			Dic_data = (Dictionary<string, object>)_data[JsonKey.DATA];

		else if (_data.ContainsKey(JsonKey.ERRORCODE))
		{
			Resp_Error = true;


			int errorIdx = Convert.ToInt16(_data[JsonKey.ERRORCODE]);
			BeforeErrorProcess(errorIdx);


#if UNITY_EDITOR
			UserEditor.Getsingleton.EditLog(string.Format("ERRORCODE : {0}", TextDataManager.Dic_ErrorCode[errorIdx]));
#endif
				
		}
		else if (_data.ContainsKey(JsonKey.RESULTCODE))
		{
			Resp_Error = true;
			int errorIdx = Convert.ToInt16(_data[JsonKey.RESULTCODE]);
			BeforeErrorProcess(errorIdx);
		}

		if (Dic_data.Count == 0)
		{
			GetResultInfoList.Clear();
			return;
		}
		User _user = UserDataManager.instance.user;

		if (_user.user_Users.LgnToken != "")
		{

			if (UserDataManager.instance.Que_OtherUserIDs.Count > 0)
			{
				//확인할 유저아이디 순차적으로 디큐및 삭제 
				object _otherUserID = UserDataManager.instance.Que_OtherUserIDs.Dequeue();
				User _otherUser = UserDataManager.instance.GetOtherUser;
				Public_response(_otherUser);
			}
			else
			{
				Public_response(_user);
			}

		}
		else
		{
			Public_response(_user);

		}
		
		

		


	}
	#endregion

	static void Public_response(User user)
	{
	

		if (Dic_data.ContainsKey(JsonKey.AuthenticationKey))
		{
			string _AuthKey = Convert.ToString(Dic_data[JsonKey.AuthenticationKey]);
			user.AuthenticationKEY = _AuthKey;
		}
		if (Dic_data.ContainsKey(JsonKey.BOXOPENLOG))
		{
			Public_UserBoxOpenLogData(user, (List<object>)Dic_data[JsonKey.BOXOPENLOG]);
		}
		if (Dic_data.ContainsKey(JsonKey.POSTLASTTIMES))
		{
			Public_UserPostLastTimeData(user, (Dictionary<string, object>)Dic_data[JsonKey.POSTLASTTIMES]);
		}
		if (Dic_data.ContainsKey(JsonKey.MYRANKING))
		{
			int _ItNum = Convert.ToInt32(Dic_data[JsonKey.MYRANKING]);
			User_Good _usergood = new User_Good(ITEMTYPE.RANK, _ItNum,DateTime.MinValue);
			user.User_Goods[ITEMTYPE.RANK] = _usergood;

		}
		if (Dic_data.ContainsKey(JsonKey.RltIdx))
		{
			user.RltIdx = Convert.ToByte(Dic_data[JsonKey.RltIdx]);
		}
		if (Dic_data.ContainsKey(JsonKey.USERS))
		{
			Public_UsersData(user, (Dictionary<string, object>)Dic_data[JsonKey.USERS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_LOGINS))
		{
			Public_UserLoginData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_LOGINS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_GOODS))
		{
			Public_UserGoodsData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_GOODS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_UNITS))
		{
			Public_User_UnitData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_UNITS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_WEAPONS))
		{
			Public_User_WeaponsData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_WEAPONS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_DECOS))
		{
			Public_User_DecosData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_DECOS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_USEUNITS))
		{
			Public_User_UseUnitData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_USEUNITS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_UNIT_USEINFOS))
		{
			Public_User_Unit_UseInfosData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_UNIT_USEINFOS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_ADS))
		{
			Public_User_AdsData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_ADS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_WORDS))
		{
			Public_UserWordsData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_WORDS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USERS_TIMES))
		{
			Public_UserTimeData(user, (Dictionary<string, object>)Dic_data[JsonKey.USERS_TIMES]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_GAMES))
		{
			Public_userGameData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_GAMES]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_DRAW_SKILLS))
		{
			Public_userDrawSkillData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_DRAW_SKILLS]);
		}
		if (Dic_data.ContainsKey(JsonKey.USER_CLANS))
		{
			Public_User_ClansData(user, (Dictionary<string, object>)Dic_data[JsonKey.USER_CLANS]);
		}
		if (Dic_data.ContainsKey(JsonKey.CLANS))
		{
			Public_Clan_ClansData(user, (Dictionary<string, object>)Dic_data[JsonKey.CLANS]);
		}
		if (Dic_data.ContainsKey(JsonKey.CLAN_MEMBERS))
		{
			Public_Clan_ClansMemberData(user, (Dictionary<string, object>)Dic_data[JsonKey.CLAN_MEMBERS]);
		}
		if (Dic_data.ContainsKey(JsonKey.CLAN_MEMBER))
		{
			Public_Clan_UserClanMemberData(user, (Dictionary<string, object>)Dic_data[JsonKey.CLAN_MEMBER]);
		}
		if (Dic_data.ContainsKey(JsonKey.CLAN_BOARDS))
		{
			Public_Clan_ClanBoardsData(user, (Dictionary<string, object>)Dic_data[JsonKey.CLAN_BOARDS]);
		}
	}

	/// <summary>
	/// 오류발생 팝업띄우기 전 처리
	/// </summary>
	public static void BeforeErrorProcess(int error_Idx)
	{
		UI_Popup_Notice _popup = null;
		switch (error_Idx)
		{
			case 2: //로그인토큰불일치
				 _popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				 _popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				 _popup.SetPopupMessage(TextDataManager.Dic_TranslateText[248]);
				 _popup.Set_addEventButton(Application.Quit);
				break;
			case 101: //이미 사용중인 계정입니다
				 _popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				 _popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				_popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[error_Idx]);

				//계정연동 실패시 user_login값 초기화
				User _user = UserDataManager.instance.user;
				_user.user_logins.Lgnkey = "";
				_user.user_logins.Email = "";
				_user.user_logins.PlfID = (byte)PlatformManager.Getsingleton.ChkGet_BeforeUsePlatformIndex(); //현재 로그인중인 플랫폼으로
				break;
			case 103: //이미 사용중인 닉네임
			case 111: // 사용불가능한문자열 포함되어있습니다
				 _popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				 _popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				_popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[error_Idx]);
				// 다시 닉네임 등록 팝업창을 띄운다.
				break;
			case 306: //클랜가입되지 않앗다
				//Resp_Error = false;

				//클랜가입되지않은 에러 인데 클랜값이 있다면 강퇴 당한것이다. 그래서 유저정보프로토콜쏘자
				if (UserDataManager.instance.user.user_Clans.ClanID != 0)
				{
					//클랜강퇴되었으니 클랜랭킹및 클랜리스트들도 갱신 필요하다 rank UI들어갔을떄 갱신하기위해 데이터들 clear하자
				
					webRequest.GetUserInfos(UserDataManager.instance.user.user_Users.UserID, () => AfterErrorProcess(error_Idx, null));
				}
				else
				{
					AfterErrorProcess(error_Idx, null);
				}
				
				break;
			case 304: //클랜탈퇴후 24이후에 가입가능
				 _popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				 _popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				_popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[error_Idx]);
				break;
			case 201:// 제화가 부족합니다.
			case 202:// 강화재료부족.
				UI_Popup_Selective popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Selective>(UIPOPUP.POPUPSELECTIVE);
				popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				popup.SetPopupMessage(string.Format("{0}\n{1}", TextDataManager.Dic_ErrorCode[error_Idx], TextDataManager.Dic_TranslateText[249]));
				AfterErrorProcess(error_Idx, popup);

				//이전으로 돌아갈 UI 설정
				//if (UI_Manager.Getsingleton._UI != UI.STORE)
				//UI_Manager.Getsingleton.goBackUI = UI_Manager.Getsingleton._UI;
				break;
		
		case 501: // 결제 검 증 오류
		case 502: //결제 내역이 존재하지 않습니다   
		case 503: //이미 구매한 과금 영수증입니다
			_popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice> (UIPOPUP.POPUPNOTICE);
			_popup.Set_PopupTitleMessage (TextDataManager.Dic_TranslateText [145]);
			_popup.SetPopupMessage (TextDataManager.Dic_ErrorCode [error_Idx]);

#if UNITY_IOS
				//========>강제 소모처리
				if (InAppPurchaseManager.instance.storekitTracs.Count > 0) 
				{
					StoreKitTransaction _tracsaction = null;
					List<StoreKitTransaction> _storeTrac = InAppPurchaseManager.instance.storekitTracs;
					for (int i = 0; i < _storeTrac.Count; i++) 
					{
						_tracsaction = _storeTrac [i];
						StoreKitBinding.finishPendingTransaction (_storeTrac [i].transactionIdentifier);
						
						break;
					}
				InAppPurchaseManager.instance.storekitTracs.Remove (_tracsaction);


				}

#elif UNITY_ANDROID
				//====>강제소모처리 
			UserEditor.Getsingleton.EditLog("강제 소모 처리 시도 => lst_NoneConsumedProduct.Count :" + InAppPurchaseManager.instance.lst_NoneConsumedProduct.Count);
			if (InAppPurchaseManager.instance.isForceConsume == false)
			{
				InAppPurchaseManager.instance.isForceConsume = true;
				InAppPurchaseManager.instance.ForceConsumePurchase();
			}


#endif
				break;

			default:
				 _popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
				 _popup.Set_PopupTitleMessage(TextDataManager.Dic_TranslateText[145]);
				_popup.SetPopupMessage(TextDataManager.Dic_ErrorCode[error_Idx]);
				break;
		}

		

	}

	/// <summary>
	/// 오류발생 팝업띄운 이후의 콜백 매서드처리
	/// </summary>
	public static void AfterErrorProcess(int error_Idx ,UI_PopupBase Popup)
	{
		if (error_Idx == 306) // 클랜가입되지 않았습니다.
		{
			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.LOBBY))
			{
				callback_Error306();
			}
			else
			{
				

				User _user = UserDataManager.instance.user;

				bool isBannedClan = false;
				//클랜 강퇴당한 유저경우 예외처리로 토스트날리고 로비보내자
				if (_user.clan_Clans.ClanID != 0)
				{

					UI_Popup_Toast poptoast = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Toast>(UIPOPUP.POPUPTOAST);
					poptoast.SetPopupMessage(string.Format(TextDataManager.Dic_TranslateText[204], UserDataManager.instance.user.user_Users.NkNm)); // ~가 클랜에 강퇴되엇습니다

					Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
					UI_Manager.Getsingleton.CreatUI(UI.LOBBY, _canvasTr);

					isBannedClan = true;

				}

				//어차피 여기 에러들어온것은 클랜이 없는것 or 클랜 강퇴이므로  user_clan, clan_clans, clan_members , clan_boards값을 무조건 초기화 시켜주자
				_user.user_Clans.Init();
				_user.clan_Clans.Init();
				_user.Clan_members.Clear();
				_user.Clan_boards.Clear();


				if (isBannedClan)
				{
					//이름변경하엿으니 소켓로그인 다시쏘자
					Network_MainMenuSoketManager.Getsingleton.Send_CTS_Login();
				}
		
			}
		}
		else if (error_Idx == 201) // 제화가 부족합니다.
		{
			UI_Popup_Selective _popup = Popup as UI_Popup_Selective;
			_popup.Set_addEventButton(callback_Error201);
		}
		else if (error_Idx == 202) // 강화재료부족.
		{
			UI_Popup_Selective _popup = Popup as UI_Popup_Selective;
			_popup.Set_addEventButton(callback_Error202);
			
		}
	}


	static void callback_Error306()
	{

		//로비캐릭회전 잠금해제
		User.isSelectedCharacter = false;

		Transform _canvasTr = UI_Manager.Getsingleton.CanvasTr;
		UI_Manager.Getsingleton.CreatUI(UI.CLAN, _canvasTr);
	}


	static void callback_Error201()
	{
		UI_Manager _ui_Mgr = UI_Manager.Getsingleton;

		if (!_ui_Mgr.Dic_UILst.ContainsKey(UI.TOP))
			_ui_Mgr.CreatUI(UI.TOP, _ui_Mgr.CanvasTr);

		
		if (webRequest.nowUseGoods == ITEMTYPE.GEM)
			UI_Top.Getsingleton.Store_Start_OnTap(STOREMODE_TYPE.Gem);
		else if (webRequest.nowUseGoods == ITEMTYPE.GOLD)
			UI_Top.Getsingleton.Store_Start_OnTap(STOREMODE_TYPE.Gold);
	}

	static void callback_Error202()
	{
		UI_Manager _ui_Mgr = UI_Manager.Getsingleton;
		if (!_ui_Mgr.Dic_UILst.ContainsKey(UI.STORE))
			_ui_Mgr.CreatUI(UI.STORE, _ui_Mgr.CanvasTr);

		UI_Store _store = _ui_Mgr.Dic_UILst[UI.STORE] as UI_Store;
		_store.Start_OnTap(ITEMTYPE.GEM);
	}

}
