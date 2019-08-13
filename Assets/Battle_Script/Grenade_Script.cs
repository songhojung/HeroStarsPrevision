using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grenade_Script : MonoBehaviour
{
    Network_Battle_Script Net_Script;

    GamePlay_Script Game_Script;

    public CHAR_USER_KIND Grenade_User_Kind;

    public uint User_ID = 0;
    public byte User_Team = 0;
    public String OJ_Name = "";
    public GUN_TYPE Gun_Type;
    public Vector3 Start_Pos = new Vector3();
    public Vector3 Target_Pos = new Vector3();
    public float Explosion_Time_MAX = 0.0f;

    Rigidbody Grenade_Rigidbody;
           
    enum EXPLOSION_STATE { IDEL, BOOM_INIT, BOOM, BOOM_END }
    EXPLOSION_STATE Explosion_State;
    float Explosion_Time = 0.0f;
    Vector3 Explosion_Pos = new Vector3();

    bool Move_Check = false;
    float Move_Time = 0.0f;

    bool Launchar_Physics_Check = false;

    RaycastHit Hit_Info;

    Collider First_Hit_OJ_Collider;

    public class ThroughShot_Data_class
    {
        public String OJ_Name;
    }
    public Dictionary<String, ThroughShot_Data_class> ThroughShot_Data_Data = new Dictionary<String, ThroughShot_Data_class>();
    
    //==========================================================================================================================================

    void Awake()
    {
        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

        Game_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();

        //this.transform.GetComponent<Rigidbody>().isKinematic = true;

        Explosion_State = EXPLOSION_STATE.IDEL;

        Move_Check = false;
        Move_Time = 0.0f;

        Launchar_Physics_Check = false;
    }

    void Update()
    {                
        //관통샷 움직임
        ThroughShot_Move_Operation();

        //수류탄 터지는 카운트 연산
        Explosion_Time_Operation();

        //수류탄 이동 좌표 보내기
        Grenade_Pos_Send_Operation();

        //네트웍 데이터 받아서 움직임 처리
        Network_Move_Operation();

        //폭발 충돌 체크
        Explosion_Check_Operation();
    }

    //==========================================================================================================================================

    public void Grenade_Shot(float ForcePower)
    {
        transform.position = Start_Pos;

        Move_Check = true;
        Move_Time = 0.0f;

        First_Hit_OJ_Collider = null;

        //수류탄 움직임 네트웍으로 전송하기
        Grenade_Send_Data();

        Grenade_Rigidbody = GetComponent<Rigidbody>();

        Grenade_Rigidbody.isKinematic = false;

        if (Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)
        {
            //Grenade_Rigidbody.isKinematic = true;
        }
        else
        {
            if (Gun_Type == GUN_TYPE.ROCKET_SKILL)
            {
                Grenade_Rigidbody.useGravity = false;
            }

            Grenade_Rigidbody.AddForce((Target_Pos - Start_Pos).normalized * ForcePower);
        }

        Explosion_Time = 0.0f;
    }

    //==========================================================================================================================================

    //관통샷 움직임
    void ThroughShot_Move_Operation()
    {
        if (Gun_Type != GUN_TYPE.THROUGH_SHOT_SKILL) return;
        
        transform.Translate((Target_Pos - Start_Pos).normalized * (Time.deltaTime * 30.0f), Space.World);        
    }

    //==========================================================================================================================================
    
    //수류탄 이동 좌표 보내기
    void Grenade_Pos_Send_Operation()
    {
        if (!Move_Check) return;

        Move_Time += Time.deltaTime;
        if (Move_Time >= Link_Script.i.Grenade_Data_BPS)
        {
            Move_Time = 0.0f;
            Grenade_Send_Data();
        }
    }

    //수류탄 움직임 네트웍으로 전송하기
    public void Grenade_Send_Data()
    {
        if (Game_Script.GameOver_Check) return;

        if (Explosion_State != EXPLOSION_STATE.IDEL) return;

        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.GRENADE_MOVE);        
        Send_Buffer.InPutByte(OJ_Name);
        Send_Buffer.InPutByte(User_ID);
        Send_Buffer.InPutByte((byte)Gun_Type);
        Send_Buffer.InPutByte(transform.position);
        Send_Buffer.InPutByte(transform.rotation.eulerAngles);        

        Net_Script.Send_PTP_Data(Send_Buffer);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
        
    Vector3 Net_Target_Pos;
    Vector3 Net_EulerAngles;

    float NC_Move_Distance = 0.0f;
    Vector3 NC_Target_Start_Pos = new Vector3();

    //네트웍으로 받은 움직일 좌표
    public void Grenade_Net_Move(ByteData _Receive_data)
    {
        _Receive_data.OutPutVariable(ref Net_Target_Pos);
        _Receive_data.OutPutVariable(ref Net_EulerAngles);

        NC_Move_Distance = 0.0f;
    }

    //최초 네트웍 위치는 바로 셋팅해준다.
    public void Direct_Pos(ByteData _Receive_data)
    {
        Grenade_Net_Move(_Receive_data);

        transform.position = Net_Target_Pos;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Net_EulerAngles), (Time.deltaTime * 10.0f));
    }

    //네트웍 데이터 받아서 움직임 처리
    void Network_Move_Operation()
    {
        if (Grenade_User_Kind != CHAR_USER_KIND.NETWORK) return;

        if (NC_Move_Distance == 0.0f) NC_Target_Start_Pos = transform.position;

        NC_Move_Distance += Time.deltaTime * (4.0f + (0.1f / Link_Script.i.Grenade_Data_BPS));
        
        //수류탄 이동 좌표 연산
        transform.position = Vector3.Lerp(NC_Target_Start_Pos, Net_Target_Pos, NC_Move_Distance);

        //수류탄 각도 연산
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Net_EulerAngles), (Time.deltaTime * 10.0f));       
    }

    //==========================================================================================================================================

    //수류탄 터지는 카운트 연산
    void Explosion_Time_Operation()
    {
        Explosion_Time += Time.deltaTime;

        if (Grenade_User_Kind == CHAR_USER_KIND.PLAYER)
        {
            if (Explosion_Time >= Explosion_Time_MAX)
            {
                Explosion_Time = 0.0f;

                Explosion_State = EXPLOSION_STATE.BOOM_INIT;
                Explosion_Pos = transform.position;
            }  
        }
        else if (Grenade_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            if (Explosion_Time >= 10.0f)
            {
                Explosion_Time = 0.0f;

                //수류탄 오브젝트 삭제
                OJ_Destroy();
            }  
        }    
    }

    //폭발 충돌 체크
    void Explosion_Check_Operation()
    {
        if (Explosion_State != EXPLOSION_STATE.BOOM_INIT) return;

        Explosion_State = EXPLOSION_STATE.BOOM;

        //폭발 임펙트 만들기
        Transform Grenade_Explosion;
        if (Gun_Type == GUN_TYPE.GRENADE || Gun_Type == GUN_TYPE.BOOMER_SKILL)
        {
            Grenade_Explosion = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/Effect_Explosion") as GameObject).GetComponent<Transform>();
        }
        else
        {
            Grenade_Explosion = Instantiate(Resources.Load("Battle_Prefab/3_Effect_Folder/Effect_ExplosionRocket") as GameObject).GetComponent<Transform>();
        }        
        Grenade_Explosion.SetParent(Link_Script.i.Effect_OJ_Set);
        Grenade_Explosion.transform.position = Explosion_Pos;

        if (Grenade_User_Kind == CHAR_USER_KIND.NETWORK)
        {
            //수류탄 오브젝트 삭제
            OJ_Destroy();

            return;
        }

        //폭탄의 일정 거리안에 오브젝트 검사
        Collider[] Char_Colliders = Physics.OverlapSphere(transform.position, Game_Script.Char_Script[User_ID].Grenade_ATK_Range);
        float MY_Total_Damage = UnityEngine.Random.Range(Game_Script.Char_Script[User_ID].Grenade_MIN_ATK, Game_Script.Char_Script[User_ID].Grenade_MAX_ATK);
        bool MY_Critical_Check = false;

        if (Gun_Type == GUN_TYPE.BOOMER_SKILL)
        {
            Char_Colliders = Physics.OverlapSphere(transform.position, Game_Script.Char_Script[User_ID].Boomer_ATK_Range);
            MY_Total_Damage = UnityEngine.Random.Range(Game_Script.Char_Script[User_ID].Boomer_MIN_ATK, Game_Script.Char_Script[User_ID].Boomer_MAX_ATK);
        }
        else if (Gun_Type == GUN_TYPE.LAUNCHER)
        {
            Char_Colliders = Physics.OverlapSphere(transform.position, Game_Script.Char_Script[User_ID].Launcher_ATK_Range);
            MY_Total_Damage = UnityEngine.Random.Range(Game_Script.Char_Script[User_ID].Launcher_MIN_ATK, Game_Script.Char_Script[User_ID].Launcher_MAX_ATK);
        }
        else if (Gun_Type == GUN_TYPE.ROCKET_SKILL)
        {
            Char_Colliders = Physics.OverlapSphere(transform.position, Game_Script.Char_Script[User_ID].Rocket_ATK_Range);
            MY_Total_Damage = UnityEngine.Random.Range(Game_Script.Char_Script[User_ID].Rocket_MIN_ATK, Game_Script.Char_Script[User_ID].Rocket_MAX_ATK);
        }

        if (Game_Script.Char_Script[User_ID].Rage_Time > 0.0f)
        {
            MY_Total_Damage += MY_Total_Damage;
            MY_Critical_Check = true;
        }

        //직사로 맞았을때 처리
        if (First_Hit_OJ_Collider != null)
        {
            //진지 충돌 검사
            if (Link_Script.i.Play_Mode == BattleKind.WAR_OF_POSITION)
            {
                if (First_Hit_OJ_Collider.CompareTag("Destroy_OJ"))
                {
                    //어택 데미지 데이터 보내기
                    Net_Script.Send_Base_Atk(MY_Total_Damage);
                }
            }
        }

        //스플레쉬 맞았을때 처리
        foreach (Collider _Char_Colliders in Char_Colliders)
        {
            //진지 충돌 검사
            if (Link_Script.i.Play_Mode == BattleKind.WAR_OF_POSITION)
            {
                if (_Char_Colliders.CompareTag("Destroy_OJ") && _Char_Colliders != First_Hit_OJ_Collider)
                {
                    if (Physics.Raycast(transform.position, (_Char_Colliders.transform.position - transform.position).normalized, out Hit_Info, Mathf.Infinity))
                    {
                        DestroyMode_Script Destroy_Hitting_Script = Hit_Info.transform.root.GetComponent<DestroyMode_Script>();

                        if (Destroy_Hitting_Script != null && Hit_Info.collider.name.Equals(Destroy_Hitting_Script.Enemy_Base_Name))
                        {
                            //어택 데미지 데이터 보내기
                            Net_Script.Send_Base_Atk(MY_Total_Damage);
                        }
                    }
                }
            }
            else if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글 모드
            {
                //캐릭터가 있는지 검사
                if (_Char_Colliders.gameObject.layer == Game_Script.Char_Script[User_ID].Char_Controller_Layer && _Char_Colliders.name.Equals("Auto_Targeting_OJ") == false)
                {                    
                    BOT_Script Bot_Char_Hitting_Script = _Char_Colliders.transform.root.GetComponent<BOT_Script>();

                    if (Bot_Char_Hitting_Script != null && Bot_Char_Hitting_Script.MY_HP > 0.0f)
                    {
                        //어택 데미지 데이터 보내기
                        Bot_Char_Hitting_Script.Bot_Damage_Operation(MY_Total_Damage, transform.position, Vector3.zero, (byte)Gun_Type, false, MY_Critical_Check);
                    }
                }
            }
            else
            {
                //캐릭터가 있는지 검사
                if (_Char_Colliders.gameObject.layer == Game_Script.Char_Script[User_ID].Char_Controller_Layer && _Char_Colliders.name.Equals("Auto_Targeting_OJ") == false)
                {
                    Player_Script Char_Hitting_Script = _Char_Colliders.transform.root.GetComponent<Player_Script>();

                    if (Char_Hitting_Script == null || Char_Hitting_Script.User_Team == User_Team) continue;

                    if (Char_Hitting_Script.MY_HP > 0.0f && Char_Hitting_Script.Barrier_Check == false)
                    {
                        //어택 데미지 데이터 보내기
                        Game_Script.Char_Script[User_ID].Send_ATK_Data(Char_Hitting_Script.User_ID, MY_Critical_Check, false, MY_Total_Damage, transform.position, Vector3.zero, (byte)Gun_Type);
                    }
                }
            }


            


            ////캐릭터가 있는지 검사
            //if (_Char_Colliders.gameObject.layer == Game_Script.Char_Script[User_ID].Char_Controller_Layer)
            //{
            //    Player_Script Char_Hitting_Script = _Char_Colliders.transform.root.GetComponent<Player_Script>();

            //    if (Char_Hitting_Script == null || Char_Hitting_Script.User_Team == User_Team) continue;

            //    //캐릭터가 있다면 캐릭터 안에 물리 컨포넌트 검사
            //    Transform[] GetTransforms = _Char_Colliders.transform.GetComponentsInChildren<Transform>();

            //    foreach (Transform child in GetTransforms)
            //    {
            //        Rigidbody Rigidbody_Check = child.GetComponent<Rigidbody>();

            //        if (Rigidbody_Check != null)
            //        {
            //            //체크된 캐릭터의 물리 컨포넌트에 전부 레이 쏴서 맞았는지 체크해준다.
            //            if (Physics.Raycast(transform.position, (child.position - transform.position).normalized, out Hit_Info, Mathf.Infinity, Game_Script.Char_Script[User_ID].RayCast_Layer))
            //            {
            //                Player_Script Hitting_Script = Hit_Info.transform.root.GetComponent<Player_Script>();

            //                if (Hitting_Script == null || Hitting_Script.User_ID != Char_Hitting_Script.User_ID || Hit_Info.collider.CompareTag("WALL")) continue;

            //                if (Hitting_Script.MY_HP > 0.0f && Hitting_Script.Barrier_Check == false)
            //                {
            //                    //어택 데미지 데이터 보내기
            //                    Game_Script.Char_Script[User_ID].Send_ATK_Data(Hitting_Script.User_ID, false, false, MY_Total_Damage, transform.position, Vector3.zero, (byte)GUN_TYPE.GRENADE);

            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}



        }

        //폭발 데이터 보내기
        Grenade_Explosion_Send_Data();

        //수류탄 오브젝트 삭제
        OJ_Destroy();
    }
    
    //관통샷 충돌 체크
    void OnTriggerEnter(Collider _Char_Colliders)
    {
        if (Gun_Type != GUN_TYPE.THROUGH_SHOT_SKILL || Grenade_User_Kind == CHAR_USER_KIND.NETWORK) return;
               
        float MY_Total_Damage = UnityEngine.Random.Range(Game_Script.Char_Script[User_ID].ThroughShot_MAX_ATK, Game_Script.Char_Script[User_ID].ThroughShot_MIN_ATK);
        bool MY_Critical_Check = false;
        
        if (Game_Script.Char_Script[User_ID].Rage_Time > 0.0f)
        {
            MY_Total_Damage += MY_Total_Damage;
            MY_Critical_Check = true;
        }
        
        //진지 충돌 검사
        if (Link_Script.i.Play_Mode == BattleKind.WAR_OF_POSITION)
        {
            if (_Char_Colliders.name.Equals("Destroy_OJ"))
            {
                if (ThroughShot_Data_Data.ContainsKey(_Char_Colliders.name) == false)
                {
                    ThroughShot_Data_Data.Add(_Char_Colliders.name, new ThroughShot_Data_class());
                    ThroughShot_Data_Data[_Char_Colliders.name].OJ_Name = _Char_Colliders.name;

                    //어택 데미지 데이터 보내기
                    Net_Script.Send_Base_Atk(MY_Total_Damage);
                }
            }
        }

        if (Link_Script.i.Play_Mode == BattleKind.ALONE_PLAY_BATTLE)//싱글 모드
        {
            //캐릭터 충돌 검사
            if (_Char_Colliders.gameObject.layer == Game_Script.Char_Script[User_ID].Char_Controller_Layer)
            {
                BOT_Script Bot_Char_Hitting_Script = _Char_Colliders.transform.root.GetComponent<BOT_Script>();

                if (Bot_Char_Hitting_Script != null && Bot_Char_Hitting_Script.MY_HP > 0.0f)
                {
                    if (ThroughShot_Data_Data.ContainsKey(_Char_Colliders.name) == false && _Char_Colliders.name.Equals("Auto_Targeting_OJ") == false)
                    {
                        ThroughShot_Data_Data.Add(_Char_Colliders.name, new ThroughShot_Data_class());
                        ThroughShot_Data_Data[_Char_Colliders.name].OJ_Name = _Char_Colliders.name;

                        //어택 데미지 데이터 보내기
                        Bot_Char_Hitting_Script.Bot_Damage_Operation(MY_Total_Damage, transform.position, _Char_Colliders.transform.position + (Vector3.up * 0.5f), (byte)GUN_TYPE.THROUGH_SHOT_SKILL, false, MY_Critical_Check);
                    }
                }
            }
        }
        else
        {
            //캐릭터 충돌 검사
            if (_Char_Colliders.gameObject.layer == Game_Script.Char_Script[User_ID].Char_Controller_Layer)
            {
                Player_Script Char_Hitting_Script = _Char_Colliders.transform.root.GetComponent<Player_Script>();

                if (Char_Hitting_Script == null || Char_Hitting_Script.User_Team == User_Team) return;

                if (Char_Hitting_Script.MY_HP > 0.0f && Char_Hitting_Script.Barrier_Check == false)
                {
                    if (ThroughShot_Data_Data.ContainsKey(_Char_Colliders.name) == false && _Char_Colliders.name.Equals("Auto_Targeting_OJ") == false)
                    {
                        ThroughShot_Data_Data.Add(_Char_Colliders.name, new ThroughShot_Data_class());
                        ThroughShot_Data_Data[_Char_Colliders.name].OJ_Name = _Char_Colliders.name;

                        //어택 데미지 데이터 보내기
                        Game_Script.Char_Script[User_ID].Send_ATK_Data(Char_Hitting_Script.User_ID, MY_Critical_Check, false, MY_Total_Damage, transform.position, _Char_Colliders.transform.position + (Vector3.up * 0.5f), (byte)GUN_TYPE.THROUGH_SHOT_SKILL);
                    }
                }
            }
        }
    }

    //유탄 충돌 체크
    void OnCollisionEnter(Collision collision)
    {
        if (Gun_Type == GUN_TYPE.GRENADE || Gun_Type == GUN_TYPE.BOOMER_SKILL || Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL || Launchar_Physics_Check) return;

        //Player_Script Char_Hitting_Script = collision.transform.root.GetComponent<Player_Script>();

        ////유탄은 같은편 맞으면 안맞은걸로 처리
        //if (Char_Hitting_Script != null && Char_Hitting_Script.User_Team == User_Team) return;

        Launchar_Physics_Check = true;

        Explosion_State = EXPLOSION_STATE.BOOM_INIT;
        Explosion_Pos = transform.position;
                
        First_Hit_OJ_Collider = collision.collider;
    }
    

    //폭발 데이터 보내기
    void Grenade_Explosion_Send_Data()
    {
        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.GRENADE_EXPLOSION);
        Send_Buffer.InPutByte(OJ_Name);

        Send_Buffer.InPutByte((byte)EXPLOSION_STATE.BOOM_INIT);
        Send_Buffer.InPutByte(Explosion_Pos);
                
        Net_Script.Send_PTP_Data(Send_Buffer);
    }


    //폭발 데이터 받기
    public void Grenade_Explosion_Receive_Data(ByteData _Receive_data)
    {
        byte _Explosion_State = 0;

        _Receive_data.OutPutVariable(ref _Explosion_State);
        _Receive_data.OutPutVariable(ref Explosion_Pos);

        Explosion_State = (EXPLOSION_STATE)_Explosion_State;
    }

    //==========================================================================================================================================

    //수류탄 오브젝트 삭제
    public void OJ_Destroy()
    {
        if (Game_Script.Grenade_Data.ContainsKey(OJ_Name)) Game_Script.Grenade_Data.Remove(OJ_Name);

        Destroy(this.gameObject);
    }

    //==========================================================================================================================================   
}
