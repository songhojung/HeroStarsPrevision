using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Infos_Shop
{
	public ushort ShopIdx;			//상점인덱스
	public byte SellItTp;			//판매아이템타입
	public byte SellItIdx;			//판매아이템인덱스
	public uint SellItNum;			//판매아이템개수
	public byte AdDelFlg;			//광고제거
	public byte BnsItTp;			//보너스아이템타입
	public uint BnsIdx;			//보너스아이템인덱스
	public uint BnsItNum;			//보너스아이템갯수
	public byte BuyItTp;			//구매아이템타입
	public uint BuyOriItNum;		//기존갯수
	public uint BuyItNum;			//현재판매갯수
	public string OriginPrice;		//IOS 원래가격 배율
	public byte SortIdx;			//정렬순서
	public byte OpenFlg;			//오픈여부
	public string ShopTxt;			// 가격텍스트 등 (IOS 구매시 IOS 가격이 뜬다)
	public byte PurLimit;			//구매제한 있는 상품 (0 : 무제한 구매가능 , 1: 한번구매가능)
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된시간

	public Infos_Shop()
	{
	}
	/// <summary>
	/// 상점인덱스, 판매아이템타입, 판매아이템개수, ,보너스아이템타입 ,보너스아이템인덱스 ,보너스아이템갯수, 구매아이템타입, 기존갯수,
	/// 현재판매갯수,정렬순서,오픈여부,가격텍스트
	/// </summary>
	public Infos_Shop(byte _ShopIdx = 0, byte _SellItTp = 0, byte _SellItIdx = 0, uint _SellItNum = 0, byte _BnsItTp = 0, uint _BnsIdx = 0, uint _BnsItNum = 0
					   , byte _BuyItTp = 0, uint _BuyOriItNum = 0, uint _BuyItNum = 0, byte _SortIdx = 0, byte _OpenFlg = 0, string _ShopTxt ="")
	{
		ShopIdx = _ShopIdx;
		SellItTp = _SellItTp;
		SellItIdx = _SellItIdx;
		SellItNum = _SellItNum;
		BnsItTp = _BnsItTp;
		BnsIdx = _BnsIdx;
		BnsItNum = _BnsItNum;
		BuyItTp = _BuyItTp;
		BuyOriItNum = _BuyOriItNum;
		BuyItNum = _BuyItNum;
		SortIdx = _SortIdx;
		OpenFlg = _OpenFlg;
		ShopTxt = _ShopTxt;
	}


}