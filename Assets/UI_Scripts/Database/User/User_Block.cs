using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class User_Block 
{
	public uint UserID;
	public byte BlkTp;
	public DateTime BlkTm;
	public string BlkRsn;
	public DateTime mtime;
	public DateTime ctime;

	public void Init()
	{
		UserID = 0;
		BlkTp = 0;
		BlkTm = DateTime.MinValue;
		BlkRsn = "";
		mtime = DateTime.MinValue;
		ctime = DateTime.MinValue;
	}

}
