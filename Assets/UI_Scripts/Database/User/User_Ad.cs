using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Ad  
{
	public uint UserID;		//유저아이디
	public DateTime AdTm;		//광고보기가능시간
	public ITEMTYPE RwdItTp;	//아이템타입
	public uint AdShowCnt;		//광고본횟수
    public byte DailyShowCnt;   //일일 광고본횟수
    public ushort PlayGoldRwd;  //게임 마지막 골드 보상
    public ushort PlayExpRwd;   //게임 마지막 경험치 보상
    public ushort LvRwd;        //레벨 보상 어디까지 받았는지
	public DateTime mtime;		//변경된시간
	public DateTime ctime;		//생성된시간
}
