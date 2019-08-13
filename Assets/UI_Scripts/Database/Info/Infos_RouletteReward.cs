using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Infos_RouletteReward 
{
	public byte RltIdx;				//룰렛인덱스
	public ITEMTYPE RwdItTp;			//보상 아이템타입
	public uint RwdItIdx;				//보상아이템 인덱스
	public ushort RwdItNum;			//보상수량
	public ushort RndPer;				//뽑기랜덤확률
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된시간
	
}
