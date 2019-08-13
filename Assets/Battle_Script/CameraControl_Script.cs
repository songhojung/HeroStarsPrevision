
using System.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraControl_Script : MonoBehaviour
{
    CharacterController Camera_Controller;

    public Camera Main_Camera = null;
    public float Camera_PixelWidth = 0.0f;
    public float Camera_PixelHeight = 0.0f;

    Transform Main_Camera_Transform;
    Transform Camera_Center_OJ;//카메라 회전각 조절 오브젝트
    Transform Camera_Move_OJ;//카메라 움직일 위치 오브젝트
    Transform Camera_Center_Aim_OJ;//카메라가 쳐다보게되는 기준 조준점 오브젝트
    Transform Camera_Shake_Aim_OJ;//카메라가 흔들리는 조준점 오브젝트
    Transform Camera_Move_Aim_OJ;//카메라가 실제 조준되는 조준점 오브젝트
    Transform Camera_Grenade_Aim_OJ;//수류탄 조준점 오브젝트
    Transform Camera_Launcher_Start_OJ;//유탄 시작점 오브젝트
    
    Vector3 Camera_Move_Vec;

    Vector3 MoveRay_Camera_Left;//카메라 왼쪽 충돌체크 좌표
    Vector3 MoveRay_Camera_Right;//카메라 오른쪽 충돌체크 좌표

    Vector3 MoveRay_Char_Center;//캐릭터 충돌체크 중심 좌표
    Vector3 MoveRay_Char_Left;//캐릭터 왼쪽 충돌케크 좌표
    Vector3 MoveRay_Char_Right;//케릭터 오른쪽 충돌체크 좌표
    float MoveRay_Distance = 0.5f;//좌,우 충돌체크 길이/
    
    Transform MY_Char_Transform;

    float Camera_Shake_Force = 0.0f;
    float Camera_Shake_Speed = 0.0f;

    public MotionBlur MotionBlur_Script;
    public int MotionBlur_Effect_State = 0;    

    public bool Direct_Move = false;

    

    void Awake()
    {
        //카메라 움직임 큐브 객체 찾기
        string[] ObjectName = { "Main_Camera", "Camera_Center_OJ", "Camera_Move_OJ", "Camera_Center_Aim_OJ", "Camera_Shake_Aim_OJ", "Camera_Move_Aim_OJ", "Camera_Grenade_Aim_OJ", "Camera_Launcher_Start_OJ" };
        Transform[] GetTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform child in GetTransforms)
        {
            if (child.name.Equals(ObjectName[0]))
            {
                Main_Camera = child.GetComponent<Camera>();
                Main_Camera_Transform = child;

                Camera_PixelWidth = Main_Camera.pixelWidth;
                Camera_PixelHeight = Main_Camera.pixelHeight;

                MotionBlur_Script = Main_Camera.GetComponent<MotionBlur>();
                MotionBlur_Script.enabled = false;
            }
            else if (child.name.Equals(ObjectName[1]))
            {
                Camera_Center_OJ = child;

            }
            else if (child.name.Equals(ObjectName[2]))
            {
                Camera_Move_OJ = child;

            }
            else if (child.name.Equals(ObjectName[3]))
            {
                Camera_Center_Aim_OJ = child;

            }
            else if (child.name.Equals(ObjectName[4]))
            {
                Camera_Shake_Aim_OJ = child;

            }
            else if (child.name.Equals(ObjectName[5]))
            {
                Camera_Move_Aim_OJ = child;

            }
            else if (child.name.Equals(ObjectName[6]))
            {
                Camera_Grenade_Aim_OJ = child;

            }
            else if (child.name.Equals(ObjectName[7]))
            {
                Camera_Launcher_Start_OJ = child;

            }
        }

        Camera_Move_Vec = new Vector3(0.6f, 1.4f, -2.5f);
        

        MoveRay_Camera_Left = new Vector3(-MoveRay_Distance, 0.0f, 0.0f);
        MoveRay_Camera_Right = new Vector3(MoveRay_Distance, 0.0f, 0.0f);

        MoveRay_Char_Center = new Vector3(0.0f, 1.3f, -0.2f);
        MoveRay_Char_Left = new Vector3(-MoveRay_Distance, 1.3f, -0.2f);
        MoveRay_Char_Right = new Vector3(MoveRay_Distance, 1.3f, -0.2f);




        //Camera_Move_Vec = new Vector3(0.6f, 1.4f, -2.5f);
        //Camera_Move_Vec = new Vector3(0.0f, 0.0f, -0.0f);
                
    }

    
    //카메라 움직임 관련 큐브 초기화
    public void Control_Cube_Init(Transform Char_Tranform)
    {
        MY_Char_Transform = Char_Tranform;

        //캐릭터위치와 동일
        transform.position = MY_Char_Transform.position;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        Camera_Center_OJ.position = transform.position;

        //캐릭터 정면 바라보는 각도와 동일
        Camera_Center_OJ.localRotation = Quaternion.Euler(new Vector3(0, Char_Tranform.localRotation.eulerAngles.y, 0));

        //카메라 위치잡는 큐브자리 셋팅
        Camera_Move_OJ.localPosition = Camera_Move_Vec;

        //카메라 위치 셋팅
        Main_Camera_Transform.position = Camera_Move_OJ.position;
        Main_Camera_Transform.LookAt(Camera_Center_Aim_OJ);

        Camera_Shake_Force = 0.0f;
        Camera_Shake_Speed = 0.0f;

        MotionBlur_Script.enabled = false;
        MotionBlur_Effect_State = 0;        

        Direct_Move = false;
    }

    //카메라 움직임 큐브 좌표 셋팅
    public void TransformPos(Vector3 _MovePos)
    {
        //카메라 첫번째 기준 큐브 움직임
        Camera_Center_OJ.position = _MovePos;
    }

    //타켓 큐브 오브젝트 리턴
    public Transform Camera_Center_Aim_OJ_Transform()
    {
        return Camera_Center_Aim_OJ;
    }

    //타켓 큐브 오브젝트 리턴
    public Transform Camera_Move_Aim_OJ_Transform()
    {
        return Camera_Move_Aim_OJ;
    }
       

    //카메라 오브젝트 리턴
    public Transform Camera_Transform
    {
        get { return Main_Camera_Transform; }
        //set { Main_Camera_Transform = value; }
    }
    
    //카메라 현재 좌표 리턴
    public Vector3 Camera_Pos
    {
        get { return Main_Camera_Transform.position; }
        set { Main_Camera_Transform.position = value; }
    }

    //수류탄 조준점
    public Vector3 Camera_Grenade_Aim_World_Pos
    {
        get { return Camera_Grenade_Aim_OJ.position; }
        set { Camera_Grenade_Aim_OJ.position = value; }
    }

    //수류탄 조준점
    public Vector3 Camera_Grenade_Aim_Local_Pos
    {
        get { return Camera_Grenade_Aim_OJ.localPosition; }
        set { Camera_Grenade_Aim_OJ.localPosition = value; }
    }

    //유탄 시작점 좌표
    public Vector3 Camera_Launcher_Start_World_Pos
    {
        get { return Camera_Launcher_Start_OJ.position; }
        set { Camera_Launcher_Start_OJ.position = value; }
    }

    //유탄 시작점 오브젝트
    public Transform Camera_Launcher_Start_Transform
    {
        get { return Camera_Launcher_Start_OJ; }
        set { Camera_Launcher_Start_OJ = value; }
    }

    //카메라 움직임,각도 큐브 오브젝트 리턴
    public Transform Camera_Center_OJ_Transform()
    {
        return Camera_Center_OJ;
    }

    //카메라 움직임,각도 큐브 오브젝트 리턴
    public Vector3 Camera_Center_OJ_EulerAngles()
    {
        return Camera_Center_OJ.rotation.eulerAngles;
    }


    //카메라 각도 큐브 연산
    public void Camera_Center_OJ_Rotation(Quaternion _rotation)
    {
        Camera_Center_OJ.rotation = Quaternion.Slerp(Camera_Center_OJ.rotation, _rotation, 0.3f);        
    }


    //카메라 줌 상태 셋팅
    public float Camera_FieldOfView
    {
        get { return Main_Camera.fieldOfView; }
        set { Main_Camera.fieldOfView = value; }
    }

    //리로드시 카메라 위치 이동시키기
    public void Camera_Move_Pos_Init(int Kind)
    {
        if (Kind == 0)
        {
            //카메라 위치잡는 큐브자리 셋팅
            Camera_Move_OJ.localPosition = Camera_Move_Vec;            
        }
        else
        {
            //카메라 위치잡는 큐브자리 셋팅
            Camera_Move_OJ.localPosition = new Vector3(Camera_Move_Vec.x + 0.2f, Camera_Move_Vec.y - 0.5f, Camera_Move_Vec.z + 1.0f);
        }
    }

    public Vector3 Test_Pos;


    RaycastHit Result_RaycastHit;
    Vector3 Result_Vector = new Vector3();
    //Vector3 Camera_Move_OJ_Dir = new Vector3();

    float Distance = 0.0f;
    Vector3 Start_Vec = new Vector3();
    Vector3 Target_Vec = new Vector3();
    
    //카메라 이동 큐브 레이케스트 연산//카메라 이동 큐브 레이케스트 연산
    public void Camera_Move_Raycast_Operation()
    {        
        if (MY_Char_Transform == null) return;

        //카메라가 이동할 좌표
        Result_Vector = Camera_Move_OJ.position;

        //캐릭터 각도에 따른 센터 좌표 셋팅
        Start_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(MoveRay_Char_Center);
        Target_Vec = Camera_Move_OJ.position;
        Distance = (Target_Vec - Start_Vec).magnitude;

        //Debug.DrawRay(Start_Vec, (Target_Vec - Start_Vec).normalized * Distance, Color.green);
        if (Physics.Raycast(Start_Vec, (Target_Vec - Start_Vec).normalized, out Result_RaycastHit, Distance))
        {
            Result_Vector = Result_RaycastHit.point + ((Start_Vec - Result_RaycastHit.point).normalized * 0.4f);
        }
        else
        {            
            Start_Vec = Camera_Move_OJ.position + ((Camera_Center_Aim_OJ.position - Camera_Move_OJ.position).normalized * Distance);
            Target_Vec = Camera_Move_OJ.position;

            //Debug.DrawRay(Start_Vec, (Target_Vec - Start_Vec).normalized * Distance, Color.red);
            if (Physics.Raycast(Start_Vec, (Target_Vec - Start_Vec).normalized, out Result_RaycastHit, Distance))
            {
                Result_Vector = Result_RaycastHit.point + ((Start_Vec - Result_RaycastHit.point).normalized * 0.4f);
            }
        }
        

        //---------------------------------------------------------------------------------------------------------------------------------------------------------




        //if (MY_Char_Transform == null) return;
        
        ////카메라가 이동할 좌표
        //Result_Vector = Camera_Move_OJ.position;

        ////캐릭터 각도에 따른 센터,좌,우 좌표 셋팅
        //Vector3 MoveRay_Center_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(MoveRay_Char_Center);
        //Vector3 MoveRay_Left_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(MoveRay_Char_Left);
        //Vector3 MoveRay_Right_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(MoveRay_Char_Right);

        ////Debug.DrawRay(MoveRay_Center_Vec, (MoveRay_Left_Vec - MoveRay_Center_Vec).normalized * MoveRay_Distance, Color.red);
        ////Debug.DrawRay(MoveRay_Center_Vec, (MoveRay_Right_Vec - MoveRay_Center_Vec).normalized * MoveRay_Distance, Color.blue);

        ////캐릭터 충돌 센터 좌표와 카메라 이동 큐브간 방향성 
        //Camera_Move_OJ_Dir = Camera_Move_OJ.position - MoveRay_Center_Vec;

        ////Debug.DrawRay(MoveRay_Center_Vec, Camera_Move_OJ_Dir.normalized * Camera_Move_OJ_Dir.magnitude, Color.green);
        
        ////센터큐브와 카메라 이동큐브간에 충돌체크
        //if (Physics.Raycast(MoveRay_Center_Vec, Camera_Move_OJ_Dir.normalized, out Result_RaycastHit, Camera_Move_OJ_Dir.magnitude))
        //{
        //    Result_Vector = Result_RaycastHit.point + ((MoveRay_Center_Vec - Result_RaycastHit.point).normalized * 0.58f);
        //}


        ////캐릭터기준 좌,우 충돌체크(항상 오른쪽 먼저 체크하고 아니면 왼쪽으로 넘어간다.)
        //if (Physics.Raycast(MoveRay_Center_Vec, (MoveRay_Right_Vec - MoveRay_Center_Vec).normalized, out Result_RaycastHit, MoveRay_Distance))
        //{
        //    //오른쪽이 닿았다면 캐릭터기준 뒤쪽으로 좌표 맞춘다.
        //    Vector3 Temp_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(Vector3.back * (Result_Vector - MoveRay_Center_Vec).magnitude);

        //    Result_Vector = new Vector3(Temp_Vec.x, Result_Vector.y, Temp_Vec.z);
        //}
        //else if (Physics.Raycast(MoveRay_Center_Vec, (MoveRay_Left_Vec - MoveRay_Center_Vec).normalized, out Result_RaycastHit, MoveRay_Distance))
        //{
        //    //왼쪽이 닿았다면 캐릭터 오른쪽 충돌지점 기준으로 좌표 맞춘다.
        //    Vector3 Temp_Vec = MoveRay_Right_Vec + MY_Char_Transform.TransformDirection(Vector3.back * (Result_Vector - MoveRay_Center_Vec).magnitude);

        //    Result_Vector = new Vector3(Temp_Vec.x, Result_Vector.y, Temp_Vec.z);
        //}

        ////기본 이동 큐브와, 캐릭터 좌,우 충돌체크가 없을때만 카메라 좌,우 충돌체크 해준다.
        //if (Result_Vector == Camera_Move_OJ.position)
        //{
        //    Vector3 MoveRay_MoveCube_Center_Vec = Camera_Move_OJ.position;
        //    Vector3 MoveRay_MoveCube_Left_Vec = MoveRay_MoveCube_Center_Vec + Main_Camera_Transform.TransformDirection(MoveRay_Camera_Left);
        //    Vector3 MoveRay_MoveCube_Right_Vec = MoveRay_MoveCube_Center_Vec + Main_Camera_Transform.TransformDirection(MoveRay_Camera_Right);
            
        //    //Debug.DrawRay(MoveRay_MoveCube_Center_Vec, (MoveRay_MoveCube_Left_Vec - MoveRay_MoveCube_Center_Vec).normalized * MoveRay_Distance, Color.red);
        //    //Debug.DrawRay(MoveRay_MoveCube_Center_Vec, (MoveRay_MoveCube_Right_Vec - MoveRay_MoveCube_Center_Vec).normalized * MoveRay_Distance, Color.blue);

        //    //카메라의 오른쪽으로 충돌체크 됐다면 케릭터기준 뒤쪽으로 맞춘다.(항상 오른쪽 먼저 체크하고 왼쪽으로 넘어간다.)
        //    if (Physics.Raycast(MoveRay_MoveCube_Center_Vec, (MoveRay_MoveCube_Right_Vec - MoveRay_MoveCube_Center_Vec).normalized, out Result_RaycastHit, MoveRay_Distance))
        //    {
        //        Vector3 Temp_Vec = MY_Char_Transform.position + MY_Char_Transform.TransformDirection(Vector3.back * (Result_Vector - MoveRay_Center_Vec).magnitude);

        //        Result_Vector = new Vector3(Temp_Vec.x, Result_Vector.y, Temp_Vec.z);
        //    }
        //    else if (Physics.Raycast(MoveRay_MoveCube_Center_Vec, (MoveRay_MoveCube_Left_Vec - MoveRay_MoveCube_Center_Vec).normalized, out Result_RaycastHit, MoveRay_Distance))
        //    {
        //        Vector3 Temp_Vec = MoveRay_Right_Vec + MY_Char_Transform.TransformDirection(Vector3.back * (Result_Vector - MoveRay_Right_Vec).magnitude);

        //        Result_Vector = new Vector3(Temp_Vec.x, Result_Vector.y, Temp_Vec.z);
        //    }
        //}


        //---------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    ////타켓 큐브 움직임 연산
    //public void Aim_Cube_Move_Operation()
    //{
    //    if (MY_Char_Transform == null) return;

    //    //카메라와 캐릭터 사이 거리 구하기
    //    float Char_Distance = (new Vector3(MY_Char_Transform.position.x, MY_Char_Transform.position.y + 0.5f, MY_Char_Transform.position.z) - Main_Camera_Transform.position).magnitude;
        
    //    RaycastHit[] Raycastall = Physics.RaycastAll(Main_Camera_Transform.position, (Camera_Center_Aim_OJ.position - Main_Camera_Transform.position).normalized, Mathf.Infinity).OrderBy(h => h.distance).ToArray();

    //    for (int i = 0; i < Raycastall.Length; i++)
    //    {
    //        if (Raycastall[i].collider.name.Equals(MY_Char_Transform.name)) continue;//자기 자신이면 패스

    //        if (Char_Distance > Raycastall[i].distance) continue;//카메라와 캐릭터 사이에 있는 오브젝으면 패스

    //        //Debug.DrawRay(Main_Camera_Transform.position, (Camera_Move_Aim_OJ.position - Main_Camera_Transform.position).normalized * (Camera_Move_Aim_OJ.position - Main_Camera_Transform.position).magnitude, Color.black);

    //        //Camera_Move_Aim_OJ.position = Vector3.Lerp(Camera_Move_Aim_OJ.position, Raycastall[i].point, 0.1f);
    //        Camera_Move_Aim_OJ.position = Vector3.Lerp(Camera_Move_Aim_OJ.position, Raycastall[i].point, 5.0f * Time.deltaTime);
            

    //        break;
    //    }
    //}
   
    //카메라 움직임 연산 
    public void Camera_Move_Operation()
    {
        //흔들림 없는 카메라 쳐다보기 연산
        //Main_Camera_Transform.position = Vector3.Lerp(Main_Camera_Transform.position, Result_Vector, 0.1f);
        //Main_Camera_Transform.LookAt(Camera_Center_Aim_OJ);

        //카메라 움직임 좌표 연산
        if (Direct_Move)//보정연산 없이 바로 카메라 좌표 셋팅
        {
            Main_Camera_Transform.position = Camera_Move_OJ.position;            
        }
        else
        {
            Main_Camera_Transform.position = Vector3.Lerp(Main_Camera_Transform.position, Result_Vector, 20.0f * Time.deltaTime);
        }

        if (Gun_Type == GUN_TYPE.SNIPER)
        {
            //최종 흔들림 큐브 위치
            Camera_Shake_Aim_OJ.position = Vector3.Lerp(Camera_Shake_Aim_OJ.position, Camera_Center_Aim_OJ.position, Time.deltaTime * 4.0f);
        }
        else
        {
            //카메라 흔들림 큐브 연산
            if (Camera_Shake_Force > 0.0f)
            {
                Camera_Shake_Aim_OJ.position = Camera_Center_Aim_OJ.position + Camera_Center_Aim_OJ.TransformDirection(new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(0.0f, 0.1f), 0).normalized * Camera_Shake_Force);

                Camera_Shake_Force -= (Time.deltaTime * (1.0f + Camera_Shake_Speed));                
            }

            //모션 블로 효과 주기
            MotionBlur_Effect_Operation();
            
            //최종 흔들림 큐브 위치
            Camera_Shake_Aim_OJ.position = Vector3.Lerp(Camera_Shake_Aim_OJ.position, Camera_Center_Aim_OJ.position, 0.5f);
        }
                        
        //카메라가 최종 흔들림 큐브 쳐다보게 하기
        Main_Camera_Transform.LookAt(Camera_Shake_Aim_OJ);                
    }
    
    //-------------------------------------------------------------------------------------------------------------------------

    void MotionBlur_Effect_Init()
    {
        MotionBlur_Effect_State = 1;        
    }

    //모션 블로 효과 주기
    void MotionBlur_Effect_Operation()
    {
        switch (MotionBlur_Effect_State)
        {
            case 0:
                break;
            case 1:

                MotionBlur_Script.enabled = true;

                Camera_FieldOfView += Time.deltaTime * 40.0f;

                if (Camera_FieldOfView >= 50.0f + 10.0f)
                {
                    MotionBlur_Effect_State = 2;
                    MotionBlur_Script.enabled = false;
                }

                break;
            case 2:

                Camera_FieldOfView -= Time.deltaTime * 40.0f;

                if (Camera_FieldOfView <= 50.0f)
                {
                    MotionBlur_Effect_State = 0;
                    MotionBlur_Script.enabled = false;
                }

                break;
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------

    void Camera_Shake(float _Force, float _Speed)
    {
        Camera_Shake_Force = _Force;
        Camera_Shake_Speed = _Speed;
    }

    GUN_TYPE Gun_Type;
    
    public void Gun_Camera_Shake(GUN_TYPE _Gun_Type)
    {
        Gun_Type = _Gun_Type;

        if (_Gun_Type == GUN_TYPE.RIFLE || _Gun_Type == GUN_TYPE.SUB_MACHINGUN || _Gun_Type == GUN_TYPE.HEAVY_MACHINGUN)
        {
            Camera_Shake(0.3f, 0.0f);
        }
        else if (_Gun_Type == GUN_TYPE.SNIPER)
        {
            Camera_Shake_Aim_OJ.position += Vector3.up * 0.5f;
        }
        else if (_Gun_Type == GUN_TYPE.PUMP_SHOTGUN)
        {
            Camera_Shake(1.3f, 3.0f);
        }
        else if (_Gun_Type == GUN_TYPE.DOUBLE_HANDGUN)
        {
            Camera_Shake(0.3f, 0.0f);
        }
        else if (_Gun_Type == GUN_TYPE.LAUNCHER || _Gun_Type == GUN_TYPE.ROCKET_SKILL)
        {
            Camera_Shake(1.9f, 3.0f);
        }
        else if (_Gun_Type == GUN_TYPE.THROUGH_SHOT_SKILL)
        {
            Camera_Shake(1.0f, 2.0f);
            MotionBlur_Effect_Init();
        }
        else
        {
            Camera_Shake(0.15f, 0.0f);
        }
    }
}
