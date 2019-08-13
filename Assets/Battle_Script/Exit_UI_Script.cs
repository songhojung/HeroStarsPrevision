using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EXIT_UI_STATE { IDEL, PANEL_OPEN, PANEL_VIEW, PANEL_CLOSE }

public class Exit_UI_Script : MonoBehaviour
{
    void Awake()
    {
        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;
    }

    public void Exit_UI_View(bool View_Check)
    {
        this.gameObject.SetActive(View_Check);
    }
}
