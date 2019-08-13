using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public partial class UI_Manager : MonoBehaviour
{

	public List<UI_PopupBase> UIPopupList = new List<UI_PopupBase>();

	// :: popup 삭제
	public void ClearUI(UIPOPUP _popup)
	{
		if(UIPopupList.Count == 0)	return;

		Type __type = GetUIpopupType(_popup);

		UI_PopupBase _Pop =UIPopupList.Find(n => n.GetType() == __type);
		if (_Pop == null)
			return;
		_Pop.Set_Close();
		Destroy(_Pop.gameObject);
		UIPopupList.Remove(_Pop);

	

		Open_NextPopup();		//다음 팝업창

	}

	Type GetUIpopupType(UIPOPUP _popup)
	{
		Type _type = null;
		switch (_popup)
		{
			case UIPOPUP.POPUPNOTICE: _type = typeof(UI_Popup_Notice); break;
			case UIPOPUP.POPUPSELECTIVE: _type = typeof(UI_Popup_Selective);break;
			case UIPOPUP.POPUPUSERINFO: _type = typeof(UI_Popup_UserInfo); break;
			case UIPOPUP.POPUPSHOWINGBOX: _type = typeof(UI_Popup_Showing); break;
			case UIPOPUP.POPUPSHOWINGLEVEL: _type = typeof(UI_Popup_Showing); break;
			case UIPOPUP.POPUPTOAST: _type = typeof(UI_Popup_Toast); break;
			case UIPOPUP.POPUPMAKENAME: _type = typeof(UI_Popup_MakeName); break;
			case UIPOPUP.POPUPBUYITEM: _type = typeof(UI_Popup_BuyItem); break;
			case UIPOPUP.POPUPNETWORKERROR: _type = typeof(UI_Popup_NetworkError); break;
			case UIPOPUP.POPUPCLANINFO: _type = typeof(UI_Popup_ClanInfo); break;
			case UIPOPUP.POPUPFIND: _type = typeof(UI_Popup_Find); break;
			case UIPOPUP.POPUPCHATMESSAGE: _type = typeof(UI_Popup_ChatMessage); break;
			case UIPOPUP.POPUPPLATFORM: _type = typeof(UI_Popup_Platform); break;
			case UIPOPUP.POPUPREINFORCE:_type = typeof(UI_Popup_Reinforce); break;
			case UIPOPUP.POPUPRFPROCESS:_type = typeof(UI_Popup_RfProcess); break;
			case UIPOPUP.POPUPSTARTGAME:_type = typeof(UI_Popup_StartGame); break;
			case UIPOPUP.POPUPINVITE:_type = typeof(UI_Popup_Invite); break;
			case UIPOPUP.POPUPATTENDANCE: _type = typeof(UI_Popup_Attendace); break;
			case UIPOPUP.POPUPSERVERSELECT: _type = typeof(UI_Popup_ServerSelect); break;
			case UIPOPUP.POPUPWRITEBOARD: _type = typeof(UI_Popup_WriteBoard); break;
			case UIPOPUP.POPUPGAINITEM: _type = typeof(UI_Popup_GainItem); break;
			case UIPOPUP.POPUPROULETTE: _type = typeof(UI_Popup_Roulette); break;
			case UIPOPUP.POPUPSHOPPURCHASE: _type = typeof(UI_Popup_ShopPurchase); break;

		}
		return _type;
	}
	public void ClearUI(UI_PopupBase popUp)
	{
		if (UIPopupList.Count == 0) return;

		//int idx = UIPopupList.IndexOf(popUp);
		//UI_PopupBase _pop = UIPopupList[idx];
		popUp.Set_Close();
		Destroy(popUp.gameObject);
		UIPopupList.Remove(popUp);

		Open_NextPopup();		//다음 팝업창

	}

	public void ClearPopupUI()
	{
		for (int i = 0; i < UIPopupList.Count; i++ )
		{
			UIPopupList[i].Set_Close();
			Destroy(UIPopupList[i].gameObject);
			UIPopupList.Remove(UIPopupList[i]);
		}
	}

	/// <summary>
	/// popup inst 
	/// </summary>
	public UI_PopupBase InstantiateUI(UIPOPUP _popup, Transform _parentTr)
	{
		GameObject NewUI;
		GameObject CloneUI;

		NewUI = Resources.Load(string.Format("Prefebs/UI_{0}", _popup.ToString())) as GameObject;
		CloneUI = Instantiate(NewUI);
		UserEditor.Getsingleton.EditLog(string.Format("Creat Prefebs/UI_{0}", _popup.ToString()));


#if UNITY_EDITOR
		CloneUI.name = NewUI.name;
#endif
		CloneUI.transform.SetParent(_parentTr);

		RectTransform _cloneUIRectTr = CloneUI.GetComponent<RectTransform>();
		RectTransform _newUIRectTr = NewUI.GetComponent<RectTransform>();

		_cloneUIRectTr.sizeDelta = _newUIRectTr.sizeDelta;
		_cloneUIRectTr.anchoredPosition = _newUIRectTr.anchoredPosition;
		_cloneUIRectTr.localScale = _newUIRectTr.localScale;

		UI_PopupBase _uiPopupBase = CloneUI.GetComponent<UI_PopupBase>();

		UIPopupList.Add(_uiPopupBase);

		//if (UIPopupList.Count == 1)
			Open_NextPopup();
		//else
		//	_uiPopupBase.gameObject.SetActive(false);

		//팝업올라 왓으니 캐릭회전 잠그자
			//User.isSelectedCharacter = true;


		return _uiPopupBase;

	}// end of function



	public T CreatAndGetPopup<T>(UIPOPUP _popup)
	{
		T popUp = default(T);
		if (_popup == UIPOPUP.POPUPTOAST || _popup == UIPOPUP.POPUPCHATMESSAGE)
		{
			if (!ChkGetPopup(out popUp, _popup))
			{
				UI_PopupBase _uiPopupBase = null;
				Scene _scene = SceneManager.GetActiveScene();
				if (_scene.name == DefineKey.Main) // Tr을 분리하였기때문에  메인씬쪽은 따로 팝업을 생성처리한다
					_uiPopupBase = InstantiateUI(_popup, parentToastPopup_Tr);
				else
					_uiPopupBase = InstantiateUI(_popup, CanvasTr);
				popUp = _uiPopupBase.GetComponent<T>();
			}
		}
		else
		{
			UI_PopupBase _uiPopupBase = null;
			Scene _scene = SceneManager.GetActiveScene();
			if (_scene.name == DefineKey.Main) // Tr을 분리하였기때문에  메인씬쪽은 따로 팝업을 생성처리한다
				_uiPopupBase = InstantiateUI(_popup, parentPopup_Tr);
			else
				_uiPopupBase = InstantiateUI(_popup, CanvasTr);
			popUp = _uiPopupBase.GetComponent<T>();
		}

			return popUp;
	}

	bool ChkGetPopup<T>(out T p, UIPOPUP _popup)
	{
		bool isExist = false;
		p = default(T);

			for (int i = 0; i < UIPopupList.Count; i++)
			{
				if (UIPopupList[i].GetType() == typeof(UI_Popup_Toast))
				{
					
					ClearUI(UIPopupList[i]);
					UI_PopupBase _uiPopupBase = null;
					Scene _scene = SceneManager.GetActiveScene();
					if (_scene.name == DefineKey.Main)
					{
						_uiPopupBase = InstantiateUI(_popup, parentToastPopup_Tr);
					}
					else
						_uiPopupBase = InstantiateUI(_popup, CanvasTr);

					p = _uiPopupBase.GetComponent<T>();
					isExist = true;
					break;
				}
				//else if (UIPopupList[i].GetType() == typeof(UI_Popup_ChatMessage))
				//{
				//    p = UIPopupList[i].GetComponent<T>();
				//    //UI_Popup_Toast t = p as UI_Popup_Toast;
				//    //if (t.toastType == ToastPopUpType.Notice)
				//    isExist = true;
				//    //else
				//    //	isExist = false;
				//    break;
				//}
			}

		return isExist;
	}


	bool ExistPopup(UIPOPUP _popup)
	{
		bool isExist = false;
		Type _poptype = GetUIpopupType(_popup);
		for (int i = 0; i < UIPopupList.Count; i++)
		{
			if (UIPopupList[i].GetType() == _poptype)
			{
				isExist = true;
				break;
			}
			
		}

		return isExist;
	}


	void Open_NextPopup()
	{



		if (UIPopupList.Count == 0)
		{
			
			//User.isSelectedCharacter = false;
			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TOP))
			{
				//if(UI_Top.Getsingleton.popupStore.gameObject.activeSelf)
				//	User.isSelectedCharacter = true;
			}
			return;
		}

		
		int nowIdx = UIPopupList.Count - 1;
		
		//토스트 팝업생성으로 들어왓다면 openNext 로직은 안들어간다
		if (UIPopupList[nowIdx].GetType() == typeof(UI_Popup_Toast))
		{
			UIPopupList[nowIdx].gameObject.SetActive(true);

			UIPopupList[nowIdx].Set_Open();
		}
		else //openNext 로직
		{


			for (int i = 0; i < UIPopupList.Count; i++)
			{
				if (nowIdx == i)
				{
					
					//if (UIPopupList[i].GetType() != typeof(UI_Popup_Toast))
					UIPopupList[i].gameObject.SetActive(true);

					UIPopupList[i].Set_Open();

				
				}
				else
				{
					//토스트 팝업이 띄워져 있으면 비활성하지말자 => 그대로띄어지게 하자
					if (UIPopupList[i].GetType() != typeof(UI_Popup_Toast))
						UIPopupList[i].gameObject.SetActive(false);
				}
			}
		}
		//UIPopupList[0].gameObject.SetActive(true);
		//UIPopupList[UIPopupList.Count - 1].Set_Open();
	}

}
