using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopupBase : MonoBehaviour 
{

	public del_ResPopup delegate_ResponseOk;

	public del_ResPopup delegate_ResponseNo;

    public virtual void Set_Open()
    {
		//RectTransform _canvasTr = GameObject.Find("Canvas").GetComponent<RectTransform>();
		//GameObject imageObj = new GameObject("Image_backAlpha");
		
		

		//imageObj.AddComponent<Image>().color = new Color(0.2f,0.2f,0.2f,0.8f);
		//imageObj.transform.SetParent(transform,false);
		//imageObj.transform.SetAsFirstSibling();
		//imageObj.GetComponent<RectTransform>().sizeDelta = _canvasTr.sizeDelta;
		//imageObj.GetComponent<RectTransform>().localScale = Vector3.one;
		//imageObj.GetComponent<RectTransform>().localPosition = Vector3.zero;
		
    }

    public virtual void Set_Close()
    {
    }

    public virtual void Set_Refresh()
    {
    }
	public virtual void Set_ErrorMassage()
	{
	}
	public virtual void ResponseButton_Yes()
	{

	}


}
