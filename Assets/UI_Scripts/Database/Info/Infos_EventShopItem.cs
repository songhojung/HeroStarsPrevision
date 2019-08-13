using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Infos_EventShopItem 
{
	public ushort ShopIdx;			//상점인덱스
	public byte SubIdx;			//서브인덱스
	public ITEMTYPE SellItTp;		//구입아이템타입
	public uint SellItIdx;			//판매 관려 인덱스
	public uint SellItNum;			//현재 판매개수
	public DateTime ctime;			//생성된 시간
	
}
