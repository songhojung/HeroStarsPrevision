using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TableDataManager 
{

	public static TableDataManager instance = new TableDataManager();
	/// <summary>
	///유닛 계급 구성정보: 키값 = 레벨(유닛계급)
	/// </summary>
	public Dictionary<byte, Infos_UnitRank> Infos_unitRanks = new Dictionary<byte, Infos_UnitRank>();


	/// <summary>
	///유닛 정보에 대한 구성정보 : 키값 = 유닛인덱스 UnitIdx
	/// </summary>
	public Dictionary<uint, infos_unit> Infos_units = new Dictionary<uint, infos_unit>();

	/// <summary>
	/// 무기정보 대한 구성정보 : 키값 무기인덱스
	/// </summary>
	public Dictionary<uint, Infos_Weapon> Infos_weapons = new Dictionary<uint, Infos_Weapon>();


	/// <summary>
	/// 치장정보에 대한 구성정보 : 키값 = 치장인덱스
	/// </summary>
	public Dictionary<uint, Infos_Deco> Infos_Decos = new Dictionary<uint, Infos_Deco>();


	/// <summary>
	/// 강화에 필요한 정보대한 구성정보 : 키값 = 강화레벨
	/// </summary>
	public Dictionary<byte, Infos_reinforce_price> Infos_reinforcePrices = new Dictionary<byte, Infos_reinforce_price>();


	/// <summary>
	/// 클랜 케릭터 지원요청시 필요한 구성정보 : 키값 = 유닛등급 UnitRk
	/// </summary>
	public Dictionary<ushort, Infos_Shop> Infos_shops = new Dictionary<ushort, Infos_Shop>();


	/// <summary>
	/// 공지사항 에대한 구성 정보 : 키값 = 공지사항인덱스 NotiIdx
	/// </summary>
	public Dictionary<byte, Infos_Notice> Infos_Notices = new Dictionary<byte, Infos_Notice>();

	/// <summary>
	/// 출석보상 에 대한 구성정보 : 키값  = 연속출석일수
	/// </summary>
	public Dictionary<byte, Infos_AttendReward> Infos_AttendRewards = new Dictionary<byte, Infos_AttendReward>();

	/// <summary>
	/// IP 별 국가 데이터 구성정보 : 키값 = 상수인덱스
	/// </summary>
	public Dictionary<UInt64, Infos_IpConcode> Infos_IpConcodes = new Dictionary<UInt64, Infos_IpConcode>();

	/// <summary>
	/// 상수 데이터 대한 구성정보 ex)킬 카운터 대한 추가보상 상수데이터
	/// </summary>
	public Dictionary<byte, Infos_constValue> Infos_ConstValues = new Dictionary<byte, Infos_constValue>();

	/// <summary>
	/// 상점 주/월정액 지급량 대한 데이터
	/// </summary>
	public Dictionary<ITEMTYPE, Infos_Shop_monthReward> Infos_Shop_monthRewards = new Dictionary<ITEMTYPE, Infos_Shop_monthReward>();


	/// <summary>
	/// 2배 이벤트 시간 대한 구성정보 : 키값 = 이벤트[요일][종류][시작시간]
	/// </summary>
	public Dictionary<DayOfWeek, Dictionary<EVENT_KIND, Dictionary<byte, Infos_doubleEventsReward>>> Infos_DoubleEventReward =
		new Dictionary<DayOfWeek, Dictionary<EVENT_KIND, Dictionary<byte, Infos_doubleEventsReward>>>();


	/// <summary>
	/// 스킬 구성정보 : 키값 = 스킬인덱스
	/// </summary>
	public Dictionary<byte, Infos_Skill> Infos_Skills = new Dictionary<byte, Infos_Skill>();


	/// <summary>
	/// 무기 강화에 필요한 정보대한 구성정보 : 키값 = 강화레벨
	/// </summary>
	public Dictionary<byte, Infos_WeaponReinforce> Infos_WeaponReinforces = new Dictionary<byte, Infos_WeaponReinforce>();

	/// <summary>
	/// 캐릭 강화에 필요한 정보대한 구성정보 : 키값 = 강화레벨
	/// </summary>
	public Dictionary<byte, Infos_UnitReinforce> Infos_UnitReinforces = new Dictionary<byte, Infos_UnitReinforce>();



	/// <summary>
	/// 이벤트 상점 과금 대한 구성정보 : 키값 = 상점인덱스
	/// </summary>
	public Dictionary<uint, Infos_EventItemTime> Infos_EventItemTimes = new Dictionary<uint, Infos_EventItemTime>();


	/// <summary>
	/// 룰렛 보상 대한 구성정보 : 키값 = 룰렛인덱스
	/// </summary>
	public Dictionary<byte, Infos_RouletteReward> Infos_RouletteRewards = new Dictionary<byte, Infos_RouletteReward>();


	/// <summary>
	/// 광고보상 대한 구성정보 : 키값 = 광고보기횟수
	/// </summary>
	public Dictionary<ushort, Infos_AdReward> Infos_AdRewards = new Dictionary<ushort, Infos_AdReward>();


	/// <summary>
	/// 이벤트 상점 기간 관련 구성정보 : 키값  = 상점인덱스
	/// </summary>
	public Dictionary<ushort, Infos_EventShopTime> Infos_EventShopTimes = new Dictionary<ushort, Infos_EventShopTime>();



	/// <summary>
	/// 이벤트 상점의 아이템관련 구성정보 : Infos_EventShopItems[상점인덱스][서브인덱스]
	/// </summary>
	public Dictionary<ushort, Dictionary<byte, Infos_EventShopItem>> Infos_EventShopItems = new Dictionary<ushort, Dictionary<byte, Infos_EventShopItem>>();

	
	/// <summary>
	/// 세트 버프 관련 구성정보 :키값 = [세트인덱스]
	/// </summary>
	public Dictionary<ushort, Infos_SetBuf> Infos_SetBuffs = new Dictionary<ushort, Infos_SetBuf>();


    /// <summary>
    /// 유저레벨 관련 구성정보 : 키값 = [레벨]
    /// </summary>
    public Dictionary<byte, Infos_UserLv> Infos_UserLvs = new Dictionary<byte, Infos_UserLv>();


	string Get_TableKey(TABLEDATE_TYPE type)
	{
		return ((int)type).ToString();
	}

	
	public  DateTime Check_returnTime(object _timeValue)
	{
		string _strValue = (string)_timeValue;

		if (string.IsNullOrEmpty(_strValue) || string.Equals(_strValue, JsonKey.NullTime))
			return DateTime.MinValue;
		else
			return Convert.ToDateTime(_timeValue);

	}
	

	public void Set_tableData(Dictionary<string, object> _data)
	{
		Dictionary<string, object> _tableData = new Dictionary<string, object>();
		if (_data.ContainsKey(JsonKey.DATA))
		{
			_tableData = (Dictionary<string, object>)_data[JsonKey.DATA];

			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.unit_ranks)))
				Applydata_unitRanks((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.unit_ranks)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.unit_infos)))
				Applydata_infosUnits((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.unit_infos)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.reinforce_prices)))
				Applydata_reinforce_prices((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.reinforce_prices)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.weapon_infos)))
				Applydata_weaponInfos((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.weapon_infos)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.notices)))
				Applydata_Notices((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.notices)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.const_values)))
				Applydata_Const_Values((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.const_values)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.attend_rewards)))
				Applydata_AttendRwd((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.attend_rewards)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.deco_infos)))
				Applydata_decoInfos((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.deco_infos)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.reward_double_events)))
				Applydata_DoubleEventReward((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.reward_double_events)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.shops)))
				Applydata_Shop((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.shops)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.skill_infos)))
				Applydata_Skill((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.skill_infos)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.weapon_reinforces)))
				Applydata_WeaponReinforce((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.weapon_reinforces)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.unit_reinforces)))
				Applydata_UnitReinforce((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.unit_reinforces)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.event_shops)))
				Applydata_EventShop((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.event_shops)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.roulette_rewards)))
				Applydata_RouletteReward((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.roulette_rewards)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.ad_rewards)))
				Applydata_AdsReward((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.ad_rewards)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.shop_month_rewards)))
				Applydata_Shop_monthReward((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.shop_month_rewards)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.shop_items)))
				Applydata_EvntShop_Item((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.shop_items)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.event_shop_times)))
				Applydata_EventShop_Time((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.event_shop_times)]);
			if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.set_bufs)))
				Applydata_SetBuff((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.set_bufs)]);
            if (_tableData.ContainsKey(Get_TableKey(TABLEDATE_TYPE.user_levels)))
                Applydata_UserLv((List<object>)_tableData[Get_TableKey(TABLEDATE_TYPE.user_levels)]);

        }

		
		

#if UNITY_EDITOR
		UserEditor.Getsingleton.EditLog("complete ReferenceDB");
#endif
	}



	void Applydata_unitRanks(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _unit = (List<object>)lstData[i];
			Infos_UnitRank _unitRanks = new Infos_UnitRank();
			for (int j = 0; j < _unit.Count; j++)
			{
				switch (j)
				{
					case 0: _unitRanks.UnitRk = Convert.ToByte(_unit[j]); break;
					case 1: _unitRanks.NeedExp = Convert.ToUInt32(_unit[j]); break;
					case 2: _unitRanks.RwdItNum= Convert.ToUInt16(_unit[j]); break;

				}
			}
			Infos_unitRanks[_unitRanks.UnitRk] = _unitRanks;

		}
	}

	void Applydata_infosUnits(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _unit = (List<object>)lstData[i];
			infos_unit _infos_unit = new infos_unit();
			for (int j = 0; j < _unit.Count; j++)
			{
				switch (j)
				{
					case 0: _infos_unit.UnitIdx = Convert.ToUInt32(_unit[j]); break;
					case 1: _infos_unit.UnitName = (_unit[j].ToString()); break;
					case 2: _infos_unit.SkillKind = Convert.ToByte(_unit[j]); break;
					case 3: _infos_unit.Hp = Convert.ToInt32(_unit[j]); break;
					case 4: _infos_unit.MvSpeed = Convert.ToInt32(_unit[j]); break;
					case 5: _infos_unit.SellItTp = (ITEMTYPE)Convert.ToByte(_unit[j]); break;
					case 6: _infos_unit.SellItNum = Convert.ToUInt32(_unit[j]); break;
					case 7: _infos_unit.RldSpeed = Convert.ToUInt16(_unit[j]); break;
				}
			}
			Infos_units[_infos_unit.UnitIdx]  = _infos_unit;

		}
	}



	void Applydata_weaponInfos(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _weapon = (List<object>)lstData[i];
			Infos_Weapon _infosWeapon = new Infos_Weapon();
			for (int j = 0; j < _weapon.Count; j++)
			{
				switch (j)
				{
					case 0: _infosWeapon.WpnIdx = Convert.ToUInt32(_weapon[j]); break;
					case 1: _infosWeapon.WpnName= (_weapon[j].ToString()); break;
					case 2: _infosWeapon.WpnType = (WEAPONE_TYPE)Convert.ToByte(_weapon[j]); break;
					case 3: _infosWeapon.WpnPart = (EQUIPPART_TYPE)Convert.ToByte(_weapon[j]); break;
					case 4: _infosWeapon.AtkMin= Convert.ToInt32(_weapon[j]); break;
					case 5: _infosWeapon.AtkMax = Convert.ToInt32(_weapon[j]); break;
					case 6: _infosWeapon.AtkRange = Convert.ToInt32(_weapon[j]); break;
					case 7: _infosWeapon.AtkSpeed = Convert.ToInt32(_weapon[j]); break;
					case 8: _infosWeapon.AimInit = Convert.ToInt32(_weapon[j]); break;
					case 9: _infosWeapon.AimCtrl = Convert.ToInt32(_weapon[j]); break;
					case 10: _infosWeapon.Magazine = Convert.ToInt32(_weapon[j]); break;
					case 11: _infosWeapon.Critical = Convert.ToSingle(_weapon[j]); break;
					case 12: _infosWeapon.GunReload = Convert.ToUInt16(_weapon[j]); break;
					case 13: _infosWeapon.SellItTp = (ITEMTYPE)Convert.ToByte(_weapon[j]); break;
					case 14: _infosWeapon.SellItNum = Convert.ToUInt32(_weapon[j]); break;
					case 15: _infosWeapon.ZoomScale = Convert.ToByte(_weapon[j]); break;
					case 16: _infosWeapon.SortIdx = Convert.ToByte(_weapon[j]); break;
					case 17: _infosWeapon.NewFlg = Convert.ToByte(_weapon[j]); break;

				}
			}
			Infos_weapons[_infosWeapon.WpnIdx] = _infosWeapon;

		}
	}



	void Applydata_decoInfos(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _deco = (List<object>)lstData[i];
			Infos_Deco _infosDeco = new Infos_Deco();
			for (int j = 0; j < _deco.Count; j++)
			{
				switch (j)
				{
					case 0: _infosDeco.DecoIdx = Convert.ToUInt32(_deco[j]); break;
					case 1: _infosDeco.DecoName= (_deco[j].ToString()); break;
					case 2: _infosDeco.DecoPart = (DECOPART_TYPE)Convert.ToByte(_deco[j]); break;
					case 3: _infosDeco.GrdPwr = Convert.ToByte(_deco[j]); break;
					case 4: _infosDeco.RspSpd = Convert.ToByte(_deco[j]); break;
					case 5: _infosDeco.SklChgTm = Convert.ToByte(_deco[j]); break;
					case 6: _infosDeco.SellItTp = (ITEMTYPE)Convert.ToByte(_deco[j]); break;
					case 7: _infosDeco.SellItNum = Convert.ToUInt32(_deco[j]); break;
					case 8: _infosDeco.SortIdx = Convert.ToByte(_deco[j]); break;
					case 9: _infosDeco.AttmntActive = Convert.ToByte(_deco[j]); break;
					case 10: _infosDeco.NewFlg = Convert.ToByte(_deco[j]); break;

				}
			}
			Infos_Decos[_infosDeco.DecoIdx] = _infosDeco;

		}
	}



	void Applydata_DoubleEventReward(List<object> lstData)
	{

	

		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _eventdata = (List<object>)lstData[i];
			Infos_doubleEventsReward _infos_doubleEvt = new Infos_doubleEventsReward();
			for (int j = 0; j < _eventdata.Count; j++)
			{

				switch (j)
				{
					case 0: _infos_doubleEvt.EvtWeek = (DayOfWeek)Convert.ToByte(_eventdata[j]); break;
					case 1: _infos_doubleEvt.EvtKind = (EVENT_KIND)Convert.ToByte(_eventdata[j]); break;
					case 2: _infos_doubleEvt.SHour = Convert.ToByte(_eventdata[j]); break;
					case 3: _infos_doubleEvt.EHour = Convert.ToByte(_eventdata[j]); break;

				}
			}

			if (Infos_DoubleEventReward.ContainsKey(_infos_doubleEvt.EvtWeek))
			{

				if (Infos_DoubleEventReward[_infos_doubleEvt.EvtWeek].ContainsKey(_infos_doubleEvt.EvtKind))
				{
					Infos_DoubleEventReward[_infos_doubleEvt.EvtWeek][_infos_doubleEvt.EvtKind][_infos_doubleEvt.SHour] = _infos_doubleEvt;
				}
				else
				{
					Dictionary<byte, Infos_doubleEventsReward> _dic_EvtHour = new Dictionary<byte, Infos_doubleEventsReward>();
					_dic_EvtHour[_infos_doubleEvt.SHour] = _infos_doubleEvt;
					Infos_DoubleEventReward[_infos_doubleEvt.EvtWeek][_infos_doubleEvt.EvtKind] = _dic_EvtHour;
				}

			}
			else
			{
				Dictionary<EVENT_KIND, Dictionary<byte, Infos_doubleEventsReward>> _dic_EvtKind = new Dictionary<EVENT_KIND, Dictionary<byte, Infos_doubleEventsReward>>();
				Dictionary<byte, Infos_doubleEventsReward> _dic_EvtHour = new Dictionary<byte, Infos_doubleEventsReward>();
				_dic_EvtHour[_infos_doubleEvt.SHour] = _infos_doubleEvt;
				_dic_EvtKind[_infos_doubleEvt.EvtKind] = _dic_EvtHour;
				Infos_DoubleEventReward[_infos_doubleEvt.EvtWeek] = _dic_EvtKind;
			}
		}
	}




	void Applydata_reinforce_prices(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _price = (List<object>)lstData[i];

			Infos_reinforce_price _reinf_p = new Infos_reinforce_price();

			for (int j = 0; j < _price.Count; j++)
			{
				if (j == 0)
					_reinf_p.RefLv = Convert.ToByte(_price[j]);
				else if (j == 1)
					_reinf_p.GoldSucPer = Convert.ToByte(_price[j]);
				else if (j == 2)
					_reinf_p.NeedGold = Convert.ToUInt32(_price[j]);
				else if (j == 3)
					_reinf_p.NeedGem= Convert.ToUInt32(_price[j]);
			}

			Infos_reinforcePrices[_reinf_p.RefLv] = _reinf_p;
		

		}
	}

	public byte Get_MaxRefLv()
	{
		int count = Infos_reinforcePrices.Count;
		return (byte)count;
	}







	void Applydata_Shop(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> lstShop = (List<object>)lstData[i];
			Infos_Shop _infos_shop = new Infos_Shop();
			for (int j = 0; j < lstShop.Count; j++)
			{
				if (j == 0)
					_infos_shop.ShopIdx = Convert.ToUInt16(lstShop[j]);
				else if (j == 1)
					_infos_shop.SellItTp =  Convert.ToByte(lstShop[j]);
				else if (j == 2)
					_infos_shop.SellItNum = Convert.ToUInt32(lstShop[j]);
				else if (j == 3)
					_infos_shop.BnsItTp = Convert.ToByte(lstShop[j]);
				else if (j == 4)
					_infos_shop.BnsIdx = Convert.ToUInt32(lstShop[j]);
				else if (j == 5)
					_infos_shop.BnsItNum = Convert.ToUInt32(lstShop[j]);
				else if (j == 6)
					_infos_shop.BuyItTp = Convert.ToByte(lstShop[j]);
				else if (j == 7)
					_infos_shop.BuyOriItNum = Convert.ToUInt32(lstShop[j]);
				else if (j == 8)
					_infos_shop.BuyItNum = Convert.ToUInt32(lstShop[j]);
				else if (j == 9)
					_infos_shop.ShopTxt = Convert.ToString(lstShop[j]);
				else if (j == 10)
					_infos_shop.SortIdx = Convert.ToByte(lstShop[j]);
				else if (j == 11)
					_infos_shop.OpenFlg = Convert.ToByte(lstShop[j]);
				else if (j == 12)
					_infos_shop.PurLimit = Convert.ToByte(lstShop[j]);
				else if (j == 13)
					_infos_shop.AdDelFlg = Convert.ToByte(lstShop[j]);
				else if (j == 14)
					_infos_shop.OriginPrice= Convert.ToString(lstShop[j]);
			}

			Infos_shops[_infos_shop.ShopIdx] =  _infos_shop;

		}
	}







	

	void Applydata_Notices(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstNotice = (List<object>)lstData[i];
			Infos_Notice _infos_notice = new Infos_Notice();
			for (int j = 0; j < _lstNotice.Count; j++)
			{
				if (j == 0)
					_infos_notice.NotiIdx = Convert.ToByte(_lstNotice[j]);
				else if (j == 1)
					_infos_notice.NotiTitle = Convert.ToString(_lstNotice[j]);
				else if (j == 2)
					_infos_notice.NotiText = Convert.ToString(_lstNotice[j]);
				else if (j == 3)
					_infos_notice.ctime = Check_returnTime(_lstNotice[j]);
				else if (j == 4)
					_infos_notice.Noticetime = Check_returnTime(_lstNotice[j]);
				else if (j == 5)
					_infos_notice.LggCd = Convert.ToByte(_lstNotice[j]);
				
			}

			Infos_Notices[_infos_notice.NotiIdx] =  _infos_notice;
		}
	}



	void Applydata_Const_Values(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstconstValues = (List<object>)lstData[i];
			Infos_constValue _infos_constValue = new Infos_constValue();
			for (int j = 0; j < _lstconstValues.Count; j++)
			{
				if (j == 0)
					_infos_constValue.ConsIdx = Convert.ToByte(_lstconstValues[j]);
				else if (j == 1)
					_infos_constValue.ConsVal = Convert.ToUInt32(_lstconstValues[j]);


			}

			Infos_ConstValues[_infos_constValue.ConsIdx] = _infos_constValue;

		}
	}


	void Applydata_AttendRwd(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstconstValues = (List<object>)lstData[i];
			Infos_AttendReward _infos_AttendRwd = new Infos_AttendReward();
			for (int j = 0; j < _lstconstValues.Count; j++)
			{
				if (j == 0)
					_infos_AttendRwd.AtdDay = Convert.ToByte(_lstconstValues[j]);
				else if (j == 1)
					_infos_AttendRwd.RwdItNum = Convert.ToUInt16(_lstconstValues[j]);
				else if (j == 2)
					_infos_AttendRwd.RwdItTp= (ITEMTYPE)Convert.ToByte(_lstconstValues[j]);

			}

			Infos_AttendRewards[_infos_AttendRwd.AtdDay] = _infos_AttendRwd;

		}
	}

	void Applydata_Shop_monthReward(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstShopMonthRwd = (List<object>)lstData[i];
			Infos_Shop_monthReward _infos_shopMonthRwd = new Infos_Shop_monthReward();
			for (int j = 0; j < _lstShopMonthRwd.Count; j++)
			{
				if (j == 0)
					_infos_shopMonthRwd.SellItTp = (ITEMTYPE)Convert.ToByte(_lstShopMonthRwd[j]);
				else if (j == 1)
					_infos_shopMonthRwd.SubIdx = Convert.ToByte(_lstShopMonthRwd[j]);
				else if (j == 2)
					_infos_shopMonthRwd.RwdItTp = (ITEMTYPE)Convert.ToByte(_lstShopMonthRwd[j]);
				else if (j == 3)
					_infos_shopMonthRwd.RwdItIdx = Convert.ToUInt32(_lstShopMonthRwd[j]);
				else if (j == 4)
					_infos_shopMonthRwd.RwdItNum = Convert.ToUInt16(_lstShopMonthRwd[j]);


			}

			Infos_Shop_monthRewards[_infos_shopMonthRwd.SellItTp] = _infos_shopMonthRwd;
		}
	}




	void Applydata_EvntShop_Item(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstEvntShopItem = (List<object>)lstData[i];
			Infos_EventShopItem _infos_evntShopItem = new Infos_EventShopItem();
			for (int j = 0; j < _lstEvntShopItem.Count; j++)
			{
				if (j == 0)
					_infos_evntShopItem.ShopIdx = Convert.ToUInt16(_lstEvntShopItem[j]);
				else if (j == 1)
					_infos_evntShopItem.SubIdx = Convert.ToByte(_lstEvntShopItem[j]);
				else if (j == 2)
					_infos_evntShopItem.SellItTp = (ITEMTYPE)Convert.ToByte(_lstEvntShopItem[j]);
				else if (j == 3)
					_infos_evntShopItem.SellItIdx= Convert.ToUInt32(_lstEvntShopItem[j]);
				else if (j == 4)
					_infos_evntShopItem.SellItNum = Convert.ToUInt32(_lstEvntShopItem[j]);


			}


			if (Infos_EventShopItems.ContainsKey(_infos_evntShopItem.ShopIdx))
			{
				
				Infos_EventShopItems[_infos_evntShopItem.ShopIdx][_infos_evntShopItem.SubIdx] = _infos_evntShopItem;
				
			}
			else
			{
				Dictionary<byte, Infos_EventShopItem> _dicInfosEvntshopItems = new Dictionary<byte, Infos_EventShopItem>();
				_dicInfosEvntshopItems[_infos_evntShopItem.SubIdx] = _infos_evntShopItem;

				Infos_EventShopItems[_infos_evntShopItem.ShopIdx] = _dicInfosEvntshopItems;
			}
			
		}
	}







	void Applydata_EventShop_Time(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstEventShopTime = (List<object>)lstData[i];
			Infos_EventShopTime _infos_EventshopTime = new Infos_EventShopTime();
			for (int j = 0; j < _lstEventShopTime.Count; j++)
			{
				if (j == 0)
					_infos_EventshopTime.ShopIdx = Convert.ToUInt16(_lstEventShopTime[j]);
				else if (j == 1)
					_infos_EventshopTime.BuyStartTm = Check_returnTime(_lstEventShopTime[j]);
				else if (j == 2)
					_infos_EventshopTime.BuyEndTm = Check_returnTime(_lstEventShopTime[j]);
	


			}

			Infos_EventShopTimes[_infos_EventshopTime.ShopIdx] = _infos_EventshopTime;
		}
	}


	void Applydata_SetBuff(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstSetBuf = (List<object>)lstData[i];
			Infos_SetBuf _infos_SetBuf = new Infos_SetBuf();
			for (int j = 0; j < _lstSetBuf.Count; j++)
			{
				if (j == 0)
					_infos_SetBuf.SetIdx = Convert.ToUInt16(_lstSetBuf[j]);
				else if (j == 1)
					_infos_SetBuf.BufKind = (SetBufKnd)Convert.ToByte(_lstSetBuf[j]);
				else if (j == 2)
				{
					if ((int)_infos_SetBuf.BufKind != 0)
					{
						uint val = Convert.ToUInt32(_lstSetBuf[j]);
						float fval = ((float)val / (1000f));
						_infos_SetBuf.BufVal = fval;
					}
					else
						_infos_SetBuf.BufVal = 0;
				}
				else if (j == 3)
					_infos_SetBuf.MainWpnIdx = Convert.ToUInt32(_lstSetBuf[j]);
				else if (j == 4)
					_infos_SetBuf.SubWpnIdx = Convert.ToUInt32(_lstSetBuf[j]);
				else if (j == 5)
					_infos_SetBuf.DecoIdx1 = Convert.ToUInt32(_lstSetBuf[j]);
				else if (j == 6)
					_infos_SetBuf.DecoIdx2 = Convert.ToUInt32(_lstSetBuf[j]);
				else if (j == 7)
					_infos_SetBuf.DecoIdx3 = Convert.ToUInt32(_lstSetBuf[j]);



			}

			Infos_SetBuffs[_infos_SetBuf.SetIdx] = _infos_SetBuf;
		}
	}







    void Applydata_UserLv(List<object> lstData)
    {
        for (int i = 0; i < lstData.Count; i++)
        {
            List<object> _lstUserLv = (List<object>)lstData[i];
            Infos_UserLv _infoUserLv = new Infos_UserLv();
            for (int j = 0; j < _lstUserLv.Count; j++)
            {
                if (j == 0)
                    _infoUserLv.UserLv= Convert.ToByte(_lstUserLv[j]);
                else if (j == 1)
                    _infoUserLv.UserExp = Convert.ToUInt32(_lstUserLv[j]);
                else if (j == 2)
                    _infoUserLv.RwdItNum = Convert.ToUInt32(_lstUserLv[j]);


            }

            Infos_UserLvs[_infoUserLv.UserLv] = _infoUserLv;
        }
    }








    void Applydata_Skill(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _lstSkill = (List<object>)lstData[i];
			Infos_Skill _infosskill = new Infos_Skill();
			for (int j = 0; j < _lstSkill.Count; j++)
			{
				if (j == 0)
					_infosskill.UnitSkill = Convert.ToByte(_lstSkill[j]);
				else if (j == 1)
					_infosskill.SkillKnd = (SKILL_TYPE)Convert.ToByte(_lstSkill[j]);
				else if (j == 2)
					_infosskill.SkillVal = Convert.ToUInt16(_lstSkill[j]);
				else if (j == 3)
					_infosskill.SkillName= Convert.ToString(_lstSkill[j]);
				else if (j == 4)
					_infosskill.CoolTime = (float)Convert.ToDouble(_lstSkill[j]);
				else if (j == 5)
					_infosskill.RunningTime = (float)Convert.ToDouble(_lstSkill[j]);


			}

			Infos_Skills[_infosskill.UnitSkill] = _infosskill;
		}
	}



	void Applydata_WeaponReinforce(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _price = (List<object>)lstData[i];

			Infos_WeaponReinforce _reinf_w = new Infos_WeaponReinforce();

			for (int j = 0; j < _price.Count; j++)
			{
				if (j == 0)
					_reinf_w.RefLv = Convert.ToByte(_price[j]);
				else if (j == 1)
					_reinf_w.GoldSucPer = Convert.ToByte(_price[j]);
				else if (j == 2)
					_reinf_w.NeedGold = Convert.ToUInt32(_price[j]);
				else if (j == 3)
					_reinf_w.NeedGem = Convert.ToUInt32(_price[j]);
			}

			Infos_WeaponReinforces[_reinf_w.RefLv] = _reinf_w;


		}
	}



	void Applydata_UnitReinforce(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> _price = (List<object>)lstData[i];

			Infos_UnitReinforce _reinf_u = new Infos_UnitReinforce();

			for (int j = 0; j < _price.Count; j++)
			{
				if (j == 0)
					_reinf_u.RefLv = Convert.ToByte(_price[j]);
				else if (j == 1)
					_reinf_u.GoldSucPer = Convert.ToByte(_price[j]);
				else if (j == 2)
					_reinf_u.NeedGold = Convert.ToUInt32(_price[j]);
				else if (j == 3)
					_reinf_u.NeedGem = Convert.ToUInt32(_price[j]);
			}

			Infos_UnitReinforces[_reinf_u.RefLv] = _reinf_u;


		}
	}




	void Applydata_EventShop(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> lstShop = (List<object>)lstData[i];
			Infos_EventItemTime _infos_evnShop = new Infos_EventItemTime();
			for (int j = 0; j < lstShop.Count; j++)
			{
				if (j == 0)
					_infos_evnShop.ItemIdx = Convert.ToUInt32(lstShop[j]);
				else if (j == 1)
					_infos_evnShop.BuyStartTm = Check_returnTime(lstShop[j]);
				else if (j == 2)
					_infos_evnShop.BuyEndTm = Check_returnTime(lstShop[j]);
			}

			Infos_EventItemTimes[_infos_evnShop.ItemIdx] = _infos_evnShop;

		}
	}


	void Applydata_RouletteReward(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> lstroulletRwd = (List<object>)lstData[i];
			Infos_RouletteReward _infosRouletteRwd = new Infos_RouletteReward();
			for (int j = 0; j < lstroulletRwd.Count; j++)
			{
				if (j == 0)
					_infosRouletteRwd.RltIdx = Convert.ToByte(lstroulletRwd[j]);
				else if (j == 1)
					_infosRouletteRwd.RwdItTp = (ITEMTYPE)Convert.ToByte(lstroulletRwd[j]);
				else if (j == 2)
					_infosRouletteRwd.RwdItIdx= Convert.ToUInt32(lstroulletRwd[j]);
				else if (j == 3)
					_infosRouletteRwd.RwdItNum = Convert.ToUInt16(lstroulletRwd[j]);
				else if (j == 4)
					_infosRouletteRwd.RndPer = Convert.ToUInt16(lstroulletRwd[j]);
			}

			Infos_RouletteRewards[_infosRouletteRwd.RltIdx] = _infosRouletteRwd;

		}
	}


	void Applydata_AdsReward(List<object> lstData)
	{
		for (int i = 0; i < lstData.Count; i++)
		{
			List<object> lstAdsRwd = (List<object>)lstData[i];
			Infos_AdReward _infosAdsRwd = new Infos_AdReward();
			for (int j = 0; j < lstAdsRwd.Count; j++)
			{
				if (j == 0)
					_infosAdsRwd.AdCnt = Convert.ToUInt16(lstAdsRwd[j]);
				else if (j == 1)
					_infosAdsRwd.GoldRwdItNum = Convert.ToUInt16(lstAdsRwd[j]);
				else if (j == 2)
					_infosAdsRwd.GemRwdItNum = Convert.ToUInt16(lstAdsRwd[j]);
			}

			Infos_AdRewards[_infosAdsRwd.AdCnt] = _infosAdsRwd;

		}
	}


	public int Get_AdsRwdNum(ushort cnt, ITEMTYPE itemtype)
	{
		int itemNum = 0;

	

		bool isRight = false;
		while (!isRight)
		{
			if (Infos_AdRewards.ContainsKey(cnt))
				isRight = true;
			else
				cnt--;
		}

		if (itemtype == ITEMTYPE.GEM)
			itemNum = Infos_AdRewards[cnt].GemRwdItNum;
		else if (itemtype == ITEMTYPE.GOLD)
			itemNum = Infos_AdRewards[cnt].GoldRwdItNum;

		return itemNum;
	}





	/////////////////////////////////////////////


	public byte Get_UnitRank(uint exp)
	{
		byte rank = 0;
		byte bfRank = 0;
		if (Infos_unitRanks[2].NeedExp > exp)
			rank = 1;
		else
		{

			foreach (var rk in Infos_unitRanks)
			{
				if (rk.Value.NeedExp <= exp)
				{
					bfRank = rk.Value.UnitRk;
				}
				else if (rk.Value.NeedExp >= exp)
				{
					rank = bfRank;
					break;
				}

			}

		}

		return rank;
	}

    public string GetSetName(SetBufKnd knd)
    {
        string name = string.Empty;
        switch (knd)
        {
            
            case SetBufKnd.ATK_UP:
                name = "메이드 셋트";
                break;
            case SetBufKnd.HP_UP:
                name = "루키 셋트";

                break;
            case SetBufKnd.ATK_SKILL_DMG_UP:
                name = "어택 스킬 셋트";
                break;
            case SetBufKnd.DMG_DECLINE:
                name = "방어 셋트";
                break;
            default:
                break;
        }

        return name;
    }

}
