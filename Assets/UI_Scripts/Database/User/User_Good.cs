using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Good 
{
	public ITEMTYPE ItTp = ITEMTYPE.NONE;
    public int ItNum = 0;
	public DateTime mtime;
	public DateTime ctime;

	public User_Good(ITEMTYPE _ittp ,int _ItNum,DateTime _mtime)
	{
		ItTp = _ittp;
		ItNum = _ItNum;
		mtime = _mtime;
	}

	public User_Good(ITEMTYPE _ittp)
	{
		ItTp = _ittp;

	}

	public User_Good()
	{

	}
}
