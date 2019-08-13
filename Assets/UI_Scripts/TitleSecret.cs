using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleSecret : MonoBehaviour ,IBeginDragHandler,IDragHandler, IEndDragHandler{

	public InputField input_secret;

	private Vector2 DefaultPosiotion;
	private bool isTouchColli = false;

	void Start()
	{
		Debug.Log("creat secret");
	}

	 public void OnBeginDrag(PointerEventData eventData)
	 {
		 DefaultPosiotion = this.transform.position;
		 isTouchColli = false;
	 }
	 public void OnDrag(PointerEventData eventData)
	 {
		 
#if UNITY_EDITOR
		 this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#else
		 this.transform.position = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
#endif
	 }

	 public void OnEndDrag(PointerEventData eventData)
	 {
//#if UNITY_EDITOR
//         Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//#else
//          Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
//#endif

		 this.transform.position = DefaultPosiotion;

		 if (isTouchColli)
		 {
			 Debug.Log("Complete !!!!!!!!!!");
			 activeInputField();
		 }
	 }



	 void OnTriggerStay2D(Collider2D coll)
	 {


		 if (coll.transform.tag == "TitleSecret")
		 {
			 isTouchColli = true;
			 Debug.Log("title stay !!!!!!!!!!");
		 }
	 }


	void activeInputField()
	 {

		 if (input_secret.isFocused == false)
		 {
			 EventSystem.current.SetSelectedGameObject(input_secret.gameObject, null);
			 input_secret.OnPointerClick(new PointerEventData(EventSystem.current));
		 }
	 }

	public void ResponseInput_Secret()
	{
		if (input_secret.text == "dyg1018")
		{
			Debug.Log("SAME");
			Destroy(GameObject.Find("TestSecret").gameObject);
			if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.TITLE))
			{

				UI_Manager.Getsingleton.ClearPopupUI();
				UI_Title.Getsingleton.Start_CheckVersion();

				
			}
		}
		else
		{
			Debug.Log("NOT SAME");
			input_secret.text = "";
		}
	}

}
