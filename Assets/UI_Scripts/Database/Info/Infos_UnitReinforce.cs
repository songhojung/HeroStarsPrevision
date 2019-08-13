﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Infos_UnitReinforce 
{
	public byte RefLv;					//강화레벨
	public byte GoldSucPer;				//골드강화성공 확률
	public uint NeedGold;					//골드 필요 개수
	public uint NeedGem;					//보석필요개수
	public DateTime mtime;					//변경된 시간
	public DateTime ctime;					//생성된 시간
}
