using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Games  
{
	//배틀게임 타입
	public BattleKind battleGameKind = BattleKind.NONE;

	//전체플레이횟수
	public uint PlayCnt;

	//시작맵 인덱스
	public byte MapIdx; 

	// 변경된 시간
	public DateTime mtime;

	// 생성된 시간
	public DateTime ctime;


	public void Init()
	{
		battleGameKind = BattleKind.NONE;
		MapIdx = 0;
		PlayCnt = 0;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}
}
