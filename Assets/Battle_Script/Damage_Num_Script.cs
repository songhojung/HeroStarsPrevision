using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Damage_Num_Script : MonoBehaviour
{
    GamePlay_Script Game_Script;

    CameraControl_Script CameraControlScript;

    bool Operation_Start = false;
    
    uint User_ID = 0;    
    bool Critical_Check = false;
    public Text Damage_Num;
    
    float Ani_Time = 0.0f;
    float Ani_Ypos = 0.0f;
    float Ani_Gravity = 0.0f;
    int Ani_Dir = 0;
    float Ani_Dir_Speed = 0.0f;
    Vector3 Ani_Move_Vec = new Vector3();

    void Awake()
    {
        Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();

        CameraControlScript = GameObject.Find("Camera_Set_Object").GetComponent<CameraControl_Script>();
    }
        
    void Update()
    {
        Move_Operation();
    }

    public void Make_Init(uint _User_ID, bool _Critical_Check, float _Damage_Num, int _Dir)
    {
        Operation_Start = true;
        
        User_ID = _User_ID;
        Critical_Check = _Critical_Check;
        Damage_Num.text = "" + _Damage_Num;

        float Rand_Result = 0.0f;
        Ani_Time = 0.0f;
        if (Critical_Check)
        {
            Rand_Result = UnityEngine.Random.Range(7.0f, 8.0f);

            Ani_Ypos = -Rand_Result;
            Ani_Gravity = UnityEngine.Random.Range(Rand_Result * 2.0f, Rand_Result * 2.5f);

            Damage_Num.color = new Color(ColorResult(255), ColorResult(0), ColorResult(0), 1);
            Damage_Num.fontSize = 60;
        }
        else
        {
            Rand_Result = UnityEngine.Random.Range(4.0f, 6.0f);

            Ani_Ypos = -Rand_Result;
            Ani_Gravity = UnityEngine.Random.Range(Rand_Result * 2.0f, Rand_Result * 2.5f);

            Damage_Num.color = new Color(ColorResult(255), ColorResult(255), ColorResult(255), 1);
            Damage_Num.fontSize = 40;
        }
        Ani_Dir = _Dir;
        Ani_Dir_Speed = UnityEngine.Random.Range(10.0f, 100.0f);

        Ani_Move_Vec = new Vector3(0, 0, 0);
    }

    Vector3 Char_Pos = new Vector3();

    void Move_Operation()
    {
        if (!Operation_Start) return;

        if (Link_Script.i.Play_Mode != BattleKind.ALONE_PLAY_BATTLE)//싱글모드가 아닐때
        {
            if (Game_Script.Char_Script.ContainsKey(User_ID) == false)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        
                
        Ani_Move_Vec += Vector3.down * Ani_Ypos;
        Ani_Ypos += (Time.deltaTime * Ani_Gravity);

        if (Ani_Dir == 0)
        {
            Ani_Move_Vec += Vector3.left * (Ani_Dir_Speed * Time.deltaTime);
        }
        else if (Ani_Dir == 1)
        {
            Ani_Move_Vec += Vector3.right * (Ani_Dir_Speed * Time.deltaTime);
        }

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글모드
        {
            Char_Pos = Game_Script.Bot_Script[User_ID].transform.position + (Vector3.up * 1.15f);
        }
        else
        {
            Char_Pos = Game_Script.Char_Script[User_ID].transform.position + (Vector3.up * 1.15f);
        }

        transform.position = CameraControlScript.Main_Camera.WorldToScreenPoint(Char_Pos) + Ani_Move_Vec;

        
        Ani_Time += Time.deltaTime;
        if (Ani_Time >= 1.0f)
        {
            Ani_Time = 0.0f;
            Destroy(this.gameObject);
        }
    }

    float ColorResult(int _Color)
    {
        return (float)(_Color / 255.0f);
    }
}
