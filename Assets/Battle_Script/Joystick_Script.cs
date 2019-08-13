using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick_Script : MonoBehaviour
{

    Image[] Move_Cursor_Img = new Image[2];

    void Awake()
    {

        string[] ObjectName = { "Move_Back_Image", "Move_Image" };
        Transform[] GetTransforms = GetComponentsInChildren<Transform>();

        for (int i = 0; i < Move_Cursor_Img.Length; i++)
        {
            foreach (Transform child in GetTransforms)
            {
                if (child.name.Equals(ObjectName[i]))
                {
                    Move_Cursor_Img[i] = child.GetComponent<Image>();
                    Move_Cursor_Img[i].transform.localPosition = new Vector3(0, 0, 0);
                    Move_Cursor_Img[i].transform.localScale = new Vector3(1, 1, 1);
                    Move_Cursor_Img[i].enabled = false;
                    break;
                }
            }
        }
           
    }


    //커서 중심축에서 최대  벗어나는 거리
    float Cursor_Image_Distance = 60.0f;

    //커서 이미지 움직임 연산
    public void CursorImage_Move(float X_Pos, float Y_Pos)
    {
        if (!Move_Cursor_Img[0].enabled)
        {
            for (int i = 0; i < Move_Cursor_Img.Length; i++)
            {
                Move_Cursor_Img[i].enabled = true;
                Move_Cursor_Img[i].transform.position = new Vector3(X_Pos, Y_Pos, 10);
            }
        }

        //일정 거리 이상 벗어나면 막아준다.
        if (Vector3.Distance(Move_Cursor_Img[0].transform.position, new Vector3(X_Pos, Y_Pos, 10)) < Cursor_Image_Distance)
        {
            Move_Cursor_Img[1].transform.position = new Vector3(X_Pos, Y_Pos, 10);
        }
        else
        {
            Move_Cursor_Img[1].transform.position = Move_Cursor_Img[0].transform.position + ((new Vector3(X_Pos, Y_Pos, 10) - Move_Cursor_Img[0].transform.position).normalized * Cursor_Image_Distance);
        }                    
    }

    public void CursorImage_Enable(bool Check)
    {
        for (int i = 0; i < Move_Cursor_Img.Length; i++)
        {
            Move_Cursor_Img[i].enabled = Check;
        }
    }
}
