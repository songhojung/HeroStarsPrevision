using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class User_unit_useInfo 
{
	public uint UserID;		//유저아이디
	public uint UnitIdx;		//유닛인덱스
	public uint UseCnt;		//사용횟수
	public uint KillCnt;		//킬횟수
	public uint DeathCnt;		//죽인횟수
	public DateTime mtime;		//변경된시간
	public DateTime ctime;		//생성된시간

	public void Init()
	{
		UserID = 0;
		UnitIdx = 0;
		UseCnt = 0;
		KillCnt = 0;
		DeathCnt = 0;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}

}
