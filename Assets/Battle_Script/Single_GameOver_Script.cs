using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Single_GameOver_Script : MonoBehaviour
{
    public Text Kill_Count;
    public GameObject Exit_Button;

    void Awake()
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------

        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;

        //------------------------------------------------------------------------------------------------------------------------------------------------

        Exit_Button.SetActive(false);
    }
        
    public void Panel_Play_Init(int _Kill_Count)
    {
        Kill_Count.text = "" + _Kill_Count;
    }

    public void Panel_View(bool Check)
    {
        this.gameObject.SetActive(Check);
    }

    public void Panel_GameOver_Button_View()
    {
        Exit_Button.SetActive(true);
    }
}
