using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class User_useUnit 
{
	public uint UserID;		//유저아이디
	public uint UnitIdx;        // 장착유닛 인덱스
    public uint[] UnitIdxs;        // 장착유닛 인덱스들
    public BatchType nowSelectBatch; //lobby 에서 선택한 배치
    public DateTime mtime;		//변경된시간
	public DateTime ctime;		//생성된시간

	public void Init()
	{
		UserID = 0;
		UnitIdx = 0;
        UnitIdxs = new uint[] { 0, 0, 0 };
        mtime = DateTime.MinValue ;
		ctime = DateTime.MinValue;	
	}
}
