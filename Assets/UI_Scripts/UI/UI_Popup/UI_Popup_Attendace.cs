using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_Popup_Attendace : UI_PopupBase 
{
	public enum AttendRwd_Type
	{
		AttdNext = 0,
		AttdToday = 1,
	}

	public List<GameObject> Lst_textAtd;
	public List<GameObject> Lst_AttandanceRwdObj;
	public List<GameObject> Lst_RwdActive;
	public List<GameObject> Lst_RwdDeactive;
	public List<Image> Lst_RwdImage;
	public List<Text> Lst_RwdNum;
	public List<Text> Lst_Day;
	public Text text_leftAtdTime;

	public Button button_Close;



	public override void Set_Open()
	{
		base.Set_Open();


		//캐릭 로비 회전 잠금
		User.isSelectedCharacter = true;
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public void Set_RwdInfo(int Attday,AttendRwd_Type attType = AttendRwd_Type.AttdNext)
	{
		Dictionary<byte, Infos_AttendReward> _dic_AttendRwd = TableDataManager.instance.Infos_AttendRewards;


		//보상오브젝트 활성및 비활성
		if (_dic_AttendRwd.ContainsKey((byte)Attday))
		{
			Active_Rwd(Attday - 1, true);
		}
		else
		{
			Active_Rwd(4, true);
		}

	

		for (int i = 0; i < _dic_AttendRwd.Count; i++ )
		{
			//보상이미지
			if(_dic_AttendRwd[(byte)i].RwdItTp == ITEMTYPE.GEM)
				Lst_RwdImage[i].sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
			else if(_dic_AttendRwd[(byte)i].RwdItTp == ITEMTYPE.GOLD)
				Lst_RwdImage[i].sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);

			Lst_RwdImage[i].SetNativeSize();
		

			
			int day  = _dic_AttendRwd[(byte)i].AtdDay + 1;
			{
				//출석일수
				if(day > 4)
					Lst_Day[i].text = string.Format("{0}D", day);
				else
					Lst_Day[i].text = string.Format("{0}D", day);

				//보상갯수
				Lst_RwdNum[i].text = string.Format("X{0}", _dic_AttendRwd[(byte)(i)].RwdItNum);
			}

		}

		if (attType == AttendRwd_Type.AttdToday) // 출첵완료
		{
			Active_RwdText(AttendRwd_Type.AttdToday);
		}
		else // 출첵미완료
		{
			Active_RwdText(AttendRwd_Type.AttdNext);
			//다음출첵까지 남은시간
			DateTime nowTime = TimeManager.Instance.Get_nowTime();
			TimeSpan nowSpan = new TimeSpan(nowTime.Hour, nowTime.Minute, nowTime.Second);
			TimeSpan DailySpan = new TimeSpan(24, 0, 0);
			TimeSpan leftSpan = DailySpan - nowSpan;

			text_leftAtdTime.text = string.Format("{0}:{1}:{2}", leftSpan.Hours, leftSpan.Minutes, leftSpan.Seconds);
		}
		//0초뒤에 닫기버튼 활성화
		StartCoroutine(routine_CloseButton());

	}

	void Active_Rwd(int idx , bool isActive)
	{
		Lst_RwdActive[idx].SetActive(isActive);
		Lst_RwdDeactive[idx].SetActive(!isActive);
	}

	// 출첵 따른 보여줄 텍스트 활성,비활성
	void Active_RwdText(AttendRwd_Type Activetype)
	{
		for (int i = 0; i < Lst_textAtd.Count; i++)
			Lst_textAtd[i].gameObject.SetActive(i == (int)Activetype);
	}


	public void ResponseButton_close()
	{

		//캐릭 로비 회전 잠금해제
		User.isSelectedCharacter = false;

		UI_Manager.Getsingleton.ClearUI(this);
	}
	

	IEnumerator routine_CloseButton()
	{

		yield return new WaitForSeconds(0.0f);
		button_Close.onClick.AddListener(ResponseButton_close);
		
	}
}
