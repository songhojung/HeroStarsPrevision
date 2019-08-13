using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Infos_Deco 
{
	public uint DecoIdx;				//치장인덱스
	public string DecoName;			//치창이름
	public DECOPART_TYPE DecoPart;		//치장부위
	public byte GrdPwr;				//수류탄공격력
	public byte RspSpd;				//부활시간
	public byte SklChgTm;				//스킬 쿨타임
	public ITEMTYPE SellItTp;			//판매아이템 타입
	public uint SellItNum;				//판매아이템 갯수
	public byte SortIdx;				//정렬순서
	public byte AttmntActive;			//부착물 활성,비활성
	public byte NewFlg;				//새로운 데코아이콘표시
	public DateTime mtime;				//변경된시간
	public DateTime ctime;				//생성된 시간

}

	
