using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GainUserUnit
{
	public uint Unitidx;			//유닛인덱스
	public ushort UnitRk;			//유닛등급(경험치에 따른 증가)
	public uint UnitExp;			//경험치
	public uint UseCnt;			//사용횟수
	public uint KillCnt;			//킬횟수
	public uint DeathCnt;			//죽인횟수
	public bool isRankUp;			//등급상승하엿나?
	public int SubSkillIdxDiff;	//현재서브인덱스 - 이전서브인덱스 차이값
}

public class GainItem
{
	//users
	public uint UserID = 0;				// 유저아이디
	public string NkNm = "";				// 닉네임
	public string LgnToken = "";			// 로그인 토큰값

	//goods
	public ITEMTYPE ItTp = ITEMTYPE.NONE;
	public int ItIdx = 0;
	public int num = 0;
	public DateTime mtime;
	public DateTime ctime;


	public GainUserUnit gainUserUnit;

	// user 생성자
	//public GainItem(GainUserUnit _gainUnit)
	//{
	//	if(Dic_gainUnit.ContainsKey(_gainUnit.Unitidx))
	//}
		

	// user 생성자
	public GainItem(uint _UserID, string _NkNm, string _LgnToken)
	{
		UserID = _UserID;
		NkNm = _NkNm;
		LgnToken = _LgnToken;
	}

	// goods 생성자
	public GainItem(ITEMTYPE _ItTp, int _ItIdx, int _num)
	{
		ItTp = _ItTp;
		ItIdx = _ItIdx;
		num = _num;
	}

	// goods 생성자
	public GainItem(ITEMTYPE _ItTp, int _ItIdx, int _num,DateTime _mtime)
	{
		ItTp = _ItTp;
		ItIdx = _ItIdx;
		num = _num;
		mtime = _mtime;
	}

	// goods 생성자
	public GainItem(ITEMTYPE _ItTp, int _ItIdx, int _num, DateTime _mtime, DateTime _ctime)
	{
		ItTp = _ItTp;
		ItIdx = _ItIdx;
		num = _num;
		mtime = _mtime;
		ctime = _ctime;
	}

	// goods 생성자
	public GainItem(ITEMTYPE _ItTp, int _num)
	{
		ItTp = _ItTp;
		num = _num;
	}





	public GainItem(ITEMTYPE _ItTp, GainUserUnit _gainUserunit)
	{
		ItTp = _ItTp;
		gainUserUnit = _gainUserunit;
	}

	public void ReAssign_GainUnit(GainUserUnit _gainUserunit)
	{
		if(this.gainUserUnit.Unitidx == 0)
			this.gainUserUnit.Unitidx = _gainUserunit.Unitidx;
		if (this.gainUserUnit.UnitRk == 0)
			this.gainUserUnit.UnitRk = _gainUserunit.UnitRk;
		if (this.gainUserUnit.UnitExp == 0)
			this.gainUserUnit.UnitExp = _gainUserunit.UnitExp;
		if (this.gainUserUnit.UseCnt == 0)
			this.gainUserUnit.UseCnt = _gainUserunit.UseCnt;
		if (this.gainUserUnit.KillCnt == 0)
			this.gainUserUnit.KillCnt = _gainUserunit.KillCnt;
		if (this.gainUserUnit.DeathCnt == 0)
			this.gainUserUnit.DeathCnt = _gainUserunit.DeathCnt;
		if (this.gainUserUnit.isRankUp == false)
			this.gainUserUnit.isRankUp = _gainUserunit.isRankUp;
	}



	//public GainInfo(int _BtIdx, int _RandKey)
	//{
	//    BtIdx = _BtIdx;
	//    RandKey = _RandKey;
	//}
}
