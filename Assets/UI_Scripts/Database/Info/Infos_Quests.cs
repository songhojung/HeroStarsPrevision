using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Infos_Quests
{
	public byte QstIdx;				//퀘스트 인덱스
	public byte QstTp;				//퀘스트 종류
	public ITEMTYPE RwdItTp;			//보상 아이템 타입
	public uint RwdItIdx;				//보상아이템 인덱스
	public ushort RwdItNum;			// 보상 수량
	public byte QstClrCnt;				// 클리어 횟수
	public byte QstRsHour;				//초기화 시간(시)
	public byte OpenFlg;				//오픈 여부
	public DateTime mtime;				//변경된 시간
	public DateTime ctime;				// 생성된 시간

}
