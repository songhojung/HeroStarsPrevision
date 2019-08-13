using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;	

public class User_weapon 
{
	public uint UserID;			//유저아이디
	public uint UnitIdx;			//유닛인덱스
	public uint WpnIdx;			//무기인덱스
	public byte RefLv;			//강화 레벨
	public byte RefFailCnt;		//강화실패횟수
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된시간
	

	public void Init()
	{
		UserID = 0;
		UnitIdx = 0;
		WpnIdx = 0;
		RefLv = 0;
		RefFailCnt = 0;
		mtime = DateTime.MaxValue;
		ctime = DateTime.MaxValue;
	}
}
