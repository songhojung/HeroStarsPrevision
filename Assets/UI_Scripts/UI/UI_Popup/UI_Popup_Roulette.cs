using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.SimpleAndroidNotifications;

public class UI_Popup_Roulette : UI_PopupBase 
{
	public enum RouletteState
	{
		NONE = 99,
		GetStart= 0,
		RotatingInStart = 1,
		RotatingforSelect = 2,
		Done = 3,
	}

	public List<GameObject> Lst_RoulttBtnOBJ;
	public List<Image> Lst_imageItem;
	public List<Text> Lst_textItemNum;


	private Animator ani;

	private RouletteState rouletteState = RouletteState.NONE;
	private float speed = 5f;
	private float targetSelectSpeed = 0f;
	private float minSpeed = 0.5f;
	private float speedValue = 0f;

	public override void Set_Open()
	{
		base.Set_Open();

		//로비 회전 잠금
		User.isSelectedCharacter = true;
		// 룰렛 설정
		Set_Roulette();
		
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	public override void Set_Refresh()
	{
		base.Set_Refresh();
	}



	// 룰렛 설정
	void Set_Roulette()
	{
		User _user = UserDataManager.instance.user;


		ani = GetComponent<Animator>();

		//targetStopSpeed  값설정
		//if (_user.RltIdx <= 3)
		//    targetSelectSpeed = 0.6f;
		//else if (_user.RltIdx > 3 && _user.RltIdx <= 6)
		//    targetSelectSpeed = 0.8f;
		//else if (_user.RltIdx > 6 && _user.RltIdx <= 8)
		//    targetSelectSpeed = 0.9f;

		speedValue = Time.deltaTime;

		targetSelectSpeed = 1f + (0.20f * (Math.Abs(8 - _user.RltIdx)));
		//start 버튼 활성
		Activate_RoulttBtnOBJ(RouletteState.GetStart);

		//ui설정
		Set_RouletteInfoUI();
	}


	//룰렛 ui설정
	void Set_RouletteInfoUI()
	{
		Dictionary<byte, Infos_RouletteReward> dic_roulttRwd = TableDataManager.instance.Infos_RouletteRewards;
		int idx = 0;
		foreach (var rlttRwd in dic_roulttRwd)
		{
			if (rlttRwd.Value.RwdItTp == ITEMTYPE.GEM)
				Lst_imageItem[idx].sprite = ImageManager.instance.Get_Sprite(DefineKey.Gem);
			else if (rlttRwd.Value.RwdItTp == ITEMTYPE.GOLD)
				Lst_imageItem[idx].sprite = ImageManager.instance.Get_Sprite(DefineKey.Gold);

			Lst_imageItem[idx].SetNativeSize();

			Lst_textItemNum[idx].text = string.Format("x{0}", rlttRwd.Value.RwdItNum);

			idx++;
		}
	}



	//시작 버튼 
	public void Start_rouletteRotate()
	{
		ani.SetFloat(DefineKey.Ani_RouletteSpeed, speed);

		//룰렛 스타트
		rouletteState = RouletteState.GetStart;

		StartCoroutine(Set_SpeedRoulette());
	}


	IEnumerator Set_SpeedRoulette()
	{
		//돌아가는중 상태
		rouletteState = RouletteState.RotatingInStart;
		Activate_RoulttBtnOBJ(RouletteState.RotatingInStart);

		while (speed >= minSpeed)
		{
			
			speed -= speedValue;
		
			ani.SetFloat(DefineKey.Ani_RouletteSpeed, speed);

			
			yield return null;
		}
	}



	//룰렛이 진행중일떄 발생할 이벤트
	public void event_RouletteComplete(int matchingIndex)
	{
		if (rouletteState == RouletteState.RotatingInStart)
		{
			
			if (speed <= minSpeed + 1.5f)
			{//중간속도에 도달햇으면 해당룰렛인덱스의 4개전 인덱스를 에서 속도 완전줄이자
				User _user = UserDataManager.instance.user;
				int idx = 0;
				if (_user.RltIdx < 5)
					idx = _user.RltIdx + 4;
				else
					idx = _user.RltIdx - 4;

				if (idx == matchingIndex)
				{
					rouletteState = RouletteState.RotatingforSelect;
					speedValue = Time.deltaTime * 3f;
					
				}
			}
			
		}
		if (rouletteState == RouletteState.RotatingforSelect)
		{
			User _user = UserDataManager.instance.user;
			//if (speed <= minSpeed)
			{
				if (_user.RltIdx == (byte)matchingIndex)
				{
					rouletteState = RouletteState.Done;

					//end 애니메이션 호출
					ani.SetTrigger(string.Format("roulttEnd{0}", _user.RltIdx));

					//회전 애니메이션 멈춤
					speed = 0f;
					ani.SetFloat(DefineKey.Ani_RouletteSpeed, speed);
				
				}
			}
		}
		else if (rouletteState == RouletteState.Done)
		{
			
			//완료시 행할것들
			
			
		}
	}




	//end 애니메이션 끝날떄 발생할 이벤트
	public void event_RouletteEnd()
	{
		//로비 회전 잠금해제
		User.isSelectedCharacter = false;

		//보상아이템 팝업
		UI_Popup_GainItem popup = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_GainItem>(UIPOPUP.POPUPGAINITEM);
		popup.Set_GainPopup();


		//top ui 갱신
		if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			UI_Top.Getsingleton.set_refresh();

		//클라푸쉬 
		//if (OptionSetting.instance.Notice_pushMsg)
		//{
		//	TimeSpan span = UserDataManager.instance.user.User_Ads.AdTm - TimeManager.Instance.Get_nowTime();
		//	double pushtime = span.TotalSeconds;
		//	//double pushtime = TableDataManager.instance.Infos_ConstValues[(int)ConstValue_TYPE.Const_limitAdTime].ConsVal * 60;
		//	NotificationManager.SendPush(TimeSpan.FromSeconds(pushtime), TextDataManager.Dic_TranslateText[304], TextDataManager.Dic_TranslateText[310]);
		//}

		//팝업닫기
		UI_Manager.Getsingleton.ClearUI(this);
	}


	void Activate_RoulttBtnOBJ(RouletteState state)
	{
		for (int i = 0; i < Lst_RoulttBtnOBJ.Count; i++ )
		{
			if (i == (int)state)
				Lst_RoulttBtnOBJ[i].SetActive(true);
			else
				Lst_RoulttBtnOBJ[i].SetActive(false);
		}
	}
}
