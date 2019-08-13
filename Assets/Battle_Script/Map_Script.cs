using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map_Script : MonoBehaviour
{
    public Vector3[] Map_Start_Pos = new Vector3[12];
    public Quaternion[] Map_Start_Dir = new Quaternion[12];
    public Vector3[] Map_Bot_Point_Pos;

    public void Map_Init(byte _Map_Index)
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------

        Transform Map = GameObject.Find("MAP_SEATOWN_" + _Map_Index).GetComponent<Transform>();
        Transform[] GetTransforms = Map.GetComponentsInChildren<Transform>();
        foreach (Transform child in GetTransforms)
        {
            if (child.name.Equals("OutSide_Cube"))
            {
                child.tag = "OutSide_Cube";
                child.gameObject.layer = LayerMask.NameToLayer("OutSide_Cube");
            }
            else
            {
                child.tag = "WALL";
            }
        }

        //바닥에 닿으면 죽는 큐브는 제일 마지막에 태크 셋팅해준다.
        foreach (Transform child in GetTransforms)
        {
            if (child.name.Equals("DeadZone"))
            {
                child.tag = "DeadZone";
                break;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------

        String OJ_Name = "Map_Start_Point_Set_" + _Map_Index;

        Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/" + OJ_Name) as GameObject).name = OJ_Name;
        GameObject.Find(OJ_Name).transform.position = new Vector3(0, 0, 0);
        GameObject.Find(OJ_Name).transform.localScale = new Vector3(1, 1, 1);        

        for (int i = 0; i < Map_Start_Pos.Length; i++)
        {
            Map_Start_Pos[i] = GameObject.Find("StartPoint (" + i + ")").transform.position;
            Map_Start_Dir[i] = GameObject.Find("StartPoint (" + i + ")").transform.rotation;
        }

        Destroy(GameObject.Find(OJ_Name));

        //------------------------------------------------------------------------------------------------------------------------------------------------
    }

    //맵의 봇이 생성될 위치값 셋팅
    public void Map_Bot_Init(int _Map_Index)
    {
        String OJ_Name = "Map_Bot_Point_Set_" + _Map_Index;

        Instantiate(Resources.Load("Battle_Prefab/1_Game_OJ_Folder/" + OJ_Name) as GameObject).name = OJ_Name;
        GameObject.Find(OJ_Name).transform.localPosition = new Vector3(0, 0, 0);
        GameObject.Find(OJ_Name).transform.localScale = new Vector3(1, 1, 1);

        //맵의 봇 생성 포인트 갯수 체크 하기
        for (int i = 0; i < 50; i++)
        {
            if (GameObject.Find("Map_Bot_Point (" + i + ")") == null)
            {
                Map_Bot_Point_Pos = new Vector3[i];
                break;
            }
        }

        for (int i = 0; i < Map_Bot_Point_Pos.Length; i++)
        {
            Map_Bot_Point_Pos[i] = GameObject.Find("Map_Bot_Point (" + i + ")").transform.position;
        }

        Destroy(GameObject.Find(OJ_Name));
    }
}
