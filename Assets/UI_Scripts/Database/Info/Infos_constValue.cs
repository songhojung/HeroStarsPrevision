using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Infos_constValue 
{
	public byte ConsIdx;			//상수인덱스 1 : n킬당 2번 상수값만큼 메달 획득
								//			2: 승리시 1번 상수 개수당 메달 n개 획득
								//			3: 승리시 기본메달 획득 갯수
								//			4: 승리시 골드 획득량
	public uint ConsVal;
	public DateTime mtime;
	public DateTime ctime;
	
}
