using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManager
{
	public static TimeManager Instance = new TimeManager();


	public DateTime severTime;


	public  DateTime Get_nowTime()
	{
		
		DateTime _nowTime = DateTime.MaxValue;

		if (DateTime.Now > severTime)
		{
			_nowTime = severTime + ( DateTime.Now - severTime);
		}
		else if (DateTime.Now < severTime)
		{
			_nowTime = DateTime.Now + (severTime - DateTime.Now );

		}
		else if (DateTime.Now == severTime)
		{
			_nowTime = DateTime.Now;
		}

		return _nowTime;

	}

	//public DayOfWeek Get_nowWeek()
	//{
	//    int week = 0;

	//    DayOfWeek dayWeek = Get_nowTime().DayOfWeek;

	//}
	

	public DateTime Get_serverTime()
	{
		return severTime;
	}


	public double Get_LeftTime(DateTime requiredTime)
	{
		TimeSpan _timeSpan = Get_nowTime() - requiredTime;
		return _timeSpan.TotalSeconds;
	}

	public DateTime Check_returnTime(DateTime _timeValue)
	{
		string _strValue = _timeValue.ToString();

		if (string.IsNullOrEmpty(_strValue) || string.Equals(_strValue, JsonKey.NullTime))
			return DateTime.MinValue;
		else
			return Convert.ToDateTime(_timeValue);

	}
}
