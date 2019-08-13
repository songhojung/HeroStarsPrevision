using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class User_Units
{
	public uint UserID;			//유저아이디
	public uint Unitidx;			//유닛인덱스
	public ushort UnitRk;			//유닛등급(경험치에 따른 증가)
	public uint UnitExp;			//경험치
	public byte RefLv;			//유닛 레벨(강화도레벨)
	public byte RefFailCnt;		//강화 실패횟수
	public uint MainWpnIdx;		//주무기인덱스
	public uint SubWpnIdx;			//보조무기인덱스
	public int DecoIdx1;			//치장1인덱스
	public int DecoIdx2;			//치장2인덱스
	public int DecoIdx3;			//치장3인덱스
	public byte SubSkill;			//서브스킬
	public DateTime mtime;
	public DateTime ctime;

	public infos_unit infosUnit;


	public void init()
	{
		UserID = 0;
		Unitidx = 0;
		UnitRk = 0;
		UnitExp= 0;
		RefLv = 0;
		RefFailCnt = 0;
		MainWpnIdx = 0;
		SubWpnIdx = 0;
		DecoIdx1 = 0;
		DecoIdx2 = 0;
		DecoIdx3 = 0;
		SubSkill = 0;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}

	

	public bool IsChanged(User_Units otherUnit)
	{
		bool changed = false;
		bool[] chkbolean = new bool[4];

		 chkbolean[0] = (otherUnit.UnitRk == this.UnitRk);
		 chkbolean[1] = (otherUnit.UnitExp == this.UnitExp);
		 chkbolean[2] = (otherUnit.RefLv == this.RefLv);
		 chkbolean[3] = (otherUnit.RefFailCnt == this.RefFailCnt);

		for (int i = 0 ; i <chkbolean.Length; i++)
		{
			if(chkbolean[i] == false)
				changed = true;
		}

		return changed;
	}
}
