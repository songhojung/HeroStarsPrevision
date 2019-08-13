using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Toast : UI_PopupBase 
{
	
	public Text text_massage;
	public Image Image_back;
	
	private float animationTime = 0f;
	private Animator animator;

	public override void Set_Open()
	{
		base.Set_Open();
		animator = GetComponent<Animator>();
		animationTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

		//오픈시 시작
		startToast();
	}

	public override void Set_Close()
	{
		base.Set_Close();
	}

	//일반 토스트팝업 띄움
	public void SetPopupMessage(string msg)
	{
		text_massage.text = msg;
		
	}
	public void Set_addEventButton(del_ResPopup _Action)
	{
		delegate_ResponseOk = _Action;
	}
	

	//팝업 띄움 시작
	public void startToast()
	{

		StartCoroutine(coroutine_ReduceTime());
	}


	//팝업 업애기
	void endToast()
	{
		if (delegate_ResponseOk != null)
			delegate_ResponseOk();

		StopCoroutine(coroutine_ReduceTime());
		UI_Manager.Getsingleton.ClearUI(this);
	}

	IEnumerator coroutine_ReduceTime()
	{
		 float _time = animationTime;
		while (_time > 0)
		{
			_time -= Time.deltaTime;
			if (_time <= 0)
			{
				endToast();
			}
			yield return null;
		}
	}






	
	
}
