using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DestroyMode_Script : MonoBehaviour
{
    CameraControl_Script CameraControlScript;

    Transform[] Destroy_OJ = new Transform[2];
    Transform[] Destroy_HP_OJ = new Transform[2];
    Image[] Destroy_HP_Image = new Image[2];

    public String Enemy_Base_Name = "";

    bool Play_Check = false;
        
    void Awake()
    {
        Transform[] GetTransforms = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in GetTransforms)
        {
            for (int i = 0; i < Destroy_OJ.Length; i++)
            {
                if (child.name.Equals("Destroy_OJ_Pos (" + i + ")"))
                {
                    Destroy_OJ[i] = child.gameObject.GetComponent<Transform>();
                }
            }
        }

        CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();
    }

    void Update()
    {
        HP_Pos_Operation();
    }

    public void HP_Init(byte User_Team)
    {
        Play_Check = true;

        if (User_Team == 0)
        {
            Enemy_Base_Name = "Destroy_OJ_Pos (1)";
        }
        else
        {
            Enemy_Base_Name = "Destroy_OJ_Pos (0)";
        }

        for (int i = 0; i < Destroy_OJ.Length; i++)
        {
            String OJ_Name = "Destroy_OJ_" + i;

            Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/Destroy_HP_Bar") as GameObject).name = OJ_Name;
            Destroy_HP_OJ[i] = GameObject.Find(OJ_Name).GetComponent<Transform>();
            Destroy_HP_OJ[i].SetParent(GameObject.Find("Canvas").transform);
            Destroy_HP_OJ[i].localScale = new Vector3(1, 1, 1);
            Destroy_HP_OJ[i].localPosition = new Vector3(0, 0, 0);
            Destroy_HP_OJ[i].SetAsFirstSibling();//가장 첫번째 레이어로 맞춰서 켄버스 안에 있는 플레이 UI 밑으로 가려지게 만든다.

            Transform[] GetTransforms = Destroy_HP_OJ[i].GetComponentsInChildren<Transform>();

            foreach (Transform child in GetTransforms)
            {
                if (child.name.Equals("Move_Hp_Image"))
                {
                    Destroy_HP_Image[i] = child.gameObject.GetComponent<Image>();
                }
            }
        }
    }

    void HP_Pos_Operation()
    {
        if (!Play_Check) return;

        for (int i = 0; i < Destroy_OJ.Length; i++)
        {
            float Scale_Result = 1.0f - ((CameraControlScript.Camera_Pos - Destroy_OJ[i].position).magnitude * 0.015f);
            Scale_Result = Mathf.Max(Mathf.Min(Scale_Result, 1.0f), 0.4f);
            Destroy_HP_OJ[i].localScale = new Vector3(Scale_Result, Scale_Result, 1.0f);


            Vector3 HP_Pos = Destroy_OJ[i].position + (Vector3.up * 2.0f);

            Destroy_HP_OJ[i].position = CameraControlScript.Main_Camera.WorldToScreenPoint(HP_Pos);

            Destroy_HP_OJ[i].gameObject.SetActive(true);

            if (!IsInView(Destroy_HP_OJ[i].position, HP_Pos, "Destroy_OJ_Pos (" + i + ")", true)) Destroy_HP_OJ[i].gameObject.SetActive(false);
            
            
            Destroy_HP_Image[i].fillAmount = (float)(Link_Script.i.Base_OJ_Now_HP[i] / Link_Script.i.Base_OJ_Max_HP);
        }
    }


    RaycastHit FrontCheck_Hit;

    //카메라 앞에 오브젝트가 있는지 체크하기
    private bool IsInView(Vector3 FrontCheck_PointOnScreen, Vector3 Target_Vec, String Target_Name, bool Raycast_Check)
    {
        //타겟이 앞에 없다면
        if (FrontCheck_PointOnScreen.z < 0) return false;

        //타겟이 화면상에 없다면
        if ((FrontCheck_PointOnScreen.x < 0) || (FrontCheck_PointOnScreen.x > Screen.width) || (FrontCheck_PointOnScreen.y < 0) || (FrontCheck_PointOnScreen.y > Screen.height)) return false;

        //타켓이 장애물에 가려졌다면
        if (Raycast_Check && Physics.Linecast(CameraControlScript.Camera_Pos, Target_Vec, out FrontCheck_Hit))
        {
            if (!FrontCheck_Hit.transform.name.Equals(Target_Name)) return false;
        }

        return true;
    }
}
