using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Text;

public class StaticMethod  
{

	//다음 스펙값 반환 => 1레벨수치+ (1레벨수치*0.05*(현재레벨-1))
	public static float Get_nextSpec(float InitSpec, int nextRefLv, bool isRound , float ratio = 0.05f, int constVal = 1)
	{
		//float ratio = Dic_infosRank[st_UnitInfo.InfosUnit.UnitRk].AddAblty / 1000f;
		float Calspec = 0;
		float _ratio = ratio;
		
		float refv = (float)nextRefLv;
		if (InitSpec != 0)// 다음능력치 = 최초능력치 + (최초능력치 *  비율 * (강화레벨-const))
			Calspec = InitSpec + (InitSpec * (_ratio) * (refv - constVal));
		else// 다음능력치   비율 * (강화레벨-const)
			Calspec =  (_ratio) * (refv - constVal);

		float _nextSpec = 0;
		if (isRound)
		{
			//_nextSpec = (float)Math.Round(Calspec, MidpointRounding.AwayFromZero);
			_nextSpec = StaticMethod.roundFloat(Calspec, 1);
		}
		else
		{
			//float v = (float)Math.Round((Calspec / 0.1f), 2);
			//_nextSpec = (float)Math.Round(v * 0.1f, 2);
			_nextSpec = StaticMethod.roundFloat(Calspec, 2);
		}
		return _nextSpec;

	}


	public static float roundFloat(float value, int digits)
	{
		float roundv = 0f;
		float roundprmt = 0f;
		int digitLocation = Math.Abs(digits);
		float powValue = (float)Math.Pow(10, digitLocation);


		roundprmt = 5f / powValue;// 5, 0.5, 0.05

		float sumValue = value + roundprmt;

		float hValue = sumValue * powValue;

		int gValue = (int)(hValue / 10f);

		if (digits - 1 == 0)
			roundv = gValue;
		else
		{
			float gParam = (float)Math.Pow(10, digits - 1);
			float bValue = gValue / gParam;
			roundv = bValue;
		}

		return roundv;
	}



	


	public static void Chk_TouchSelectArea(bool isSelectedCharacter, ref TouchPhase touchState, ref Vector3 pos_TouchBegan,string ColliderTagName)
	{
		
		//ray = UI_Manager.Getsingleton.canvas.worldCamera.ScreenPointToRay(Input.mousePosition);
#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0) && !isSelectedCharacter)
		{
			Ray ray;
			RaycastHit hit;
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 9000))
			{
				if (hit.collider.tag == ColliderTagName)
				{
					if (touchState == TouchPhase.Began)
						return;
					else
					{
						touchState = TouchPhase.Began;
						pos_TouchBegan = Input.mousePosition;
					}
				}

			}
		}
		else if (Input.GetMouseButtonUp(0) && !isSelectedCharacter)
		{
			if (touchState == TouchPhase.Moved)
			{
				touchState = TouchPhase.Ended;
				pos_TouchBegan = Input.mousePosition;

			}
			else
			{
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit, 9000))
				{
					if (hit.collider.tag == ColliderTagName)
					{

						touchState = TouchPhase.Ended;
					}
				}
			}
		}
#else

			if (Input.touchCount > 0)
			{
				Ray ray;
				RaycastHit hit;
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				//싱글터치
				Touch touch = Input.GetTouch(0);
				Vector3 touchPos;

				if (!isSelectedCharacter)
				{
					switch (touch.phase)
					{
						case TouchPhase.Began:

							Vector3 touchPosVector3 = new Vector3(touch.position.x, touch.position.y, 100);
							touchPos = Camera.main.ScreenToWorldPoint(touchPosVector3);
							ray = Camera.main.ScreenPointToRay(touchPosVector3);
							if (Physics.Raycast(ray, out hit, 9000))
							{
								if (hit.collider.tag == ColliderTagName)
								{
									if (touchState == TouchPhase.Began)
										return;
									else
									{
										touchState = TouchPhase.Began;
										pos_TouchBegan = touchPos;
									}
								}
							}

							break;

						case TouchPhase.Ended:
							touchState = TouchPhase.Ended;
							Vector3 _touchPosVector3 = new Vector3(touch.position.x, touch.position.y, 100);
							touchPos = Camera.main.ScreenToWorldPoint(_touchPosVector3);
							pos_TouchBegan = touchPos;
							break;
					}
				}
			}
#endif

	}


	public static void TouchRotateObject(ref TouchPhase touchState, ref Vector3 pos_TouchBegan, ref Vector3 nowPointPos, Touch touch, Transform rotateObj 
		,float rotataeSensitive, del_NextProcess nxtPrcMoved, del_NextProcess nxtPrcEnded)
	{
		//터치영역에 들어옴
		//Debug.Log("TouchType : " + TouchType);

		if (touchState == TouchPhase.Began)
			touchState = TouchPhase.Moved;
		else if (touchState == TouchPhase.Moved)
		{

#if UNITY_EDITOR
			if (nowPointPos == Input.mousePosition)
				return;

			nowPointPos = Input.mousePosition;
			//Debug.Log("nowMousePos : " + nowMousePos);
#else
				touch = Input.GetTouch(0);
				Vector3 touchPosVector3 = new Vector3(touch.position.x, touch.position.y, 100);
				Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchPosVector3);
				//Debug.Log("touchPos : " + touchPos);

				if (nowPointPos == touchPos)
				{
					Debug.Log("same nowPos");

					return;
				}
				nowPointPos = touchPos;
#endif

			float dis = nowPointPos.x - pos_TouchBegan.x;
			//Debug.Log("dis : " + dis);
			dis = dis / rotataeSensitive;
			rotateObj.eulerAngles = new Vector3(rotateObj.eulerAngles.x,
				rotateObj.eulerAngles.y - dis, rotateObj.eulerAngles.z);


			if (nxtPrcMoved != null)
				nxtPrcMoved();

		}
		else if (touchState == TouchPhase.Ended)
		{
			if (nxtPrcEnded != null)
				nxtPrcEnded();

			touchState = TouchPhase.Canceled;
		}
	}





	public static IEnumerator routine_GetLeftTime(DateTime lastestTime, Text text_leftTime , int showTxtType)
	{
		DateTime _lastTime = lastestTime;
		//Debug.Log("최종로그인시간 :" + lastestTime);


		while (true)
		{
			DateTime nowtime = TimeManager.Instance.Get_nowTime();

			TimeSpan _timespan = _lastTime - nowtime;

			int sec = _timespan.Seconds;
			int totalmin = (int)_timespan.TotalMinutes;
			int hour = totalmin / 60;
			int day = hour / 24;
			int min = totalmin % 60;

			if (showTxtType == 0) // 일,시간,분,초 따로 표시
			{
				if (day > 0)
					text_leftTime.text = string.Format("{0} {1}", day, TextDataManager.Dic_TranslateText[181]); // ~일
				else if (hour > 0)
					text_leftTime.text = string.Format("{0} {1}", hour, TextDataManager.Dic_TranslateText[180]); // ~시간
				else if (min > 0)
					text_leftTime.text = string.Format("{0} {1}", min, TextDataManager.Dic_TranslateText[179]); // ~분
				else
					text_leftTime.text = string.Format("{0} {1}", sec, TextDataManager.Dic_TranslateText[245]); // ~초
			}
			else if (showTxtType == 1) // 시간 + 분 표시 
			{
				//if (day > 0)
				//	text_leftTime.text = string.Format("{0}{1} {2}{3} {4}{5}", day, TextDataManager.Dic_TranslateText[181]
				//		, hour%24, TextDataManager.Dic_TranslateText[180], min, TextDataManager.Dic_TranslateText[179]); // ~일 ~시간 ~분
				if (hour > 0)
					text_leftTime.text = string.Format("{0} {1} {2}{3}", hour, TextDataManager.Dic_TranslateText[180]
						, min, TextDataManager.Dic_TranslateText[179]); // ~시간 ~분
				else if (min > 0)
					text_leftTime.text = string.Format("{0} {1}", min, TextDataManager.Dic_TranslateText[179]); // ~분
			}


			yield return null;
		}
	}


	public static byte[] Get_CountryCodeByteData()
	{
		byte[] _code = new byte[2];

#if UNITY_EDITOR
		string str = "kr";
		_code = Encoding.UTF8.GetBytes(str);
#elif UNITY_ANDROID
		_code = Encoding.UTF8.GetBytes(AndroidPluginManager.Getsingleton.countryCode);
#elif UNITY_IOS
		_code = Encoding.UTF8.GetBytes(IosPluginManager.Getsingleton.get_CountryCode());
#endif
		return _code;

	}


	public static string Get_CountryCodeStringData()
	{
		string _code = string.Empty;
		if (PlayerPrefs.HasKey(DefineKey.UserCountryCode))
			_code = PlayerPrefs.GetString(DefineKey.UserCountryCode);

		if (string.IsNullOrEmpty(_code))
		{
#if UNITY_EDITOR
		_code = "kr";
#elif UNITY_ANDROID
		//_code = AndroidPluginManager.Getsingleton.countryCode;
				_code = "kr";


#elif UNITY_IOS
		//_code = IosPluginManager.Getsingleton.get_CountryCode();
           		_code = "kr";

#endif
        }
        UserEditor.Getsingleton.EditLog("국가코드 받기 : " + _code);
		return _code;

	}


	public static string Get_OrinCountryCodeString()
	{
		string _code = string.Empty;
		

		if (string.IsNullOrEmpty(_code))
		{
#if UNITY_EDITOR
			_code = "kr";
#elif UNITY_ANDROID
		//_code = AndroidPluginManager.Getsingleton.countryCode;
		_code = "kr";

#elif UNITY_IOS
		//_code = IosPluginManager.Getsingleton.get_CountryCode();
            _code = "kr";
#endif
        }
        UserEditor.Getsingleton.EditLog("진짜 로컬국가코드 받기 : " + _code);
		return _code;

	}


	public static bool Chk_EventBuff(EVENT_KIND eventKind)
	{
		bool isDoingEvnet = false;
		DayOfWeek dweek = TimeManager.Instance.Get_nowTime().DayOfWeek;
		byte nowHour = (byte)TimeManager.Instance.Get_nowTime().Hour;

		if (TableDataManager.instance.Infos_DoubleEventReward.ContainsKey(dweek))
		{

			Dictionary<EVENT_KIND, Dictionary<byte, Infos_doubleEventsReward>> _dic_evnet = TableDataManager.instance.Infos_DoubleEventReward[dweek];

			if (_dic_evnet.ContainsKey(eventKind))
			{
				Dictionary<byte, Infos_doubleEventsReward> _dic_EvtTime = _dic_evnet[eventKind];
				bool isEvent = false;

				foreach (var evtHour in _dic_EvtTime)
				{
					if (evtHour.Value.SHour <= nowHour && evtHour.Value.EHour > nowHour)
					{
						isEvent = true;
					}
				}

				if (isEvent)
					isDoingEvnet = true;
				else
					isDoingEvnet = false;

			}
			else
			{
				isDoingEvnet = false;
			}

		}
		else
		{
			isDoingEvnet = false;
		}

		return isDoingEvnet;
	}




	public static bool TimeCount(ref float _Time, float _Max_Time)
	{
		_Time += Time.deltaTime;
		if (_Time >= _Max_Time)
		{
			_Time = 0.0f;

			return true;
		}

		return false;
	}





	public static IEnumerator routine_waitForInAppProducts(del_NextProcess nextProcess)
	{
		if (InAppPurchaseManager.instance.IsGetQueryInventory == false)
		{
			Loadmanager.instance.LoadingUI(true);
			User.isSelectedCharacter = true;

			float time = 0;
			while (true)
			{
				time += Time.deltaTime;

				if (InAppPurchaseManager.instance.IsGetQueryInventory == true)
				{
					Loadmanager.instance.LoadingUI(false);
					User.isSelectedCharacter = false;

					if (nextProcess != null) nextProcess();
					break;
				}

				if (time > 5)
				{
					Loadmanager.instance.LoadingUI(false);
					User.isSelectedCharacter = false;
					UI_Popup_Notice pop = UI_Manager.Getsingleton.CreatAndGetPopup<UI_Popup_Notice>(UIPOPUP.POPUPNOTICE);
					pop.Set_PopupTitleMessage("알림");
					pop.SetPopupMessage("상품 정보를 받아오지 못하였습니다. \n 다시 시도 바랍니다.");
					break;
				}

				yield return null;
			}
		}
		else
		{
			if (nextProcess != null) nextProcess();
			yield return null;
		}

	}
}

