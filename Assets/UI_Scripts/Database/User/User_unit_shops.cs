using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class User_unit_shop 
{
	public uint UserID;			//유저아이디
	public byte BuyFlgBit;			//구매했는지처리Bit
	public uint UnitIdx1;			//캐릭터인덱스1
	public uint UnitIdx2;			//캐릭터인덱스2
	public uint UnitIdx3;			//캐릭터인덱스3
	public uint UnitIdx4;			//캐릭터인덱스4
	public DateTime BuyTime;		//구매가능시간
	public DateTime mtime;			//변경된시간
	public DateTime ctime;			//생성된 시간



	public void Init()
	{
		UserID = 0;
		BuyFlgBit = 0;
		UnitIdx1 = 0;
		UnitIdx2= 0;
		UnitIdx3= 0;
		UnitIdx4= 0;
		BuyTime = DateTime.MinValue;
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;		
	}

	public int GetUnitArrayIdx(uint _unitIdx)
	{
		if (_unitIdx == UnitIdx1)
			return 1;
		else if (_unitIdx == UnitIdx2)
			return 2;
		else if (_unitIdx == UnitIdx3)
			return 3;
		else if (_unitIdx == UnitIdx4)
			return 4;
		else
			return 0;
	}
}
