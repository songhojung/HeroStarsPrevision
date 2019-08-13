using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Infos_Shop_monthReward 
{
	public ITEMTYPE SellItTp;			//판매아이템타입 BuyItTo이 월정액(15),주정액(14) 인 경우 매일 지급량
	public byte SubIdx;			// 서브인덱스
	public ITEMTYPE RwdItTp;			// 보상 아이템타입
	public uint RwdItIdx;			// 보상 아이템 인덱스
	public ushort RwdItNum;		// 보상 수량
	public DateTime mtime;			//변경된 시간
	public DateTime ctime;			//생성된 시간

}
