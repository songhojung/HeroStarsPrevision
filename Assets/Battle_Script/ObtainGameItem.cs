using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObtainGameItem : MonoBehaviour
{
    private CHAR_USER_KIND ObtainItemUserKnd;
    private Network_Battle_Script Net_Script;
    private GamePlay_Script Game_Script;
    private Transform Tr;
    private Vector3 Rcv_Position;
    private Vector3 Rcv_EulerAngles;

    public int CreatedIndex = 0;
    private float Move_Time = 0;

    private bool hasMoved = false;
    private void Awake()
    {
        Net_Script = Network_Battle_Script.Getsingleton;
        Game_Script = GamePlay_Script.Getsingleton;
        Tr = GetComponent<Transform>();
        hasMoved = false;

    }




    public void Init_Item(CHAR_USER_KIND _UserKnd, byte _CreatedIndex)
    {
        ObtainItemUserKnd = _UserKnd;
        CreatedIndex = _CreatedIndex;
    }




    private void Update()
    {
        Operation_Move();
        Operation_ObtainItem_PosDataSend();
    }

    void Operation_Move()
    {
        if (!hasMoved || ObtainItemUserKnd == CHAR_USER_KIND.PLAYER) return;

        Tr.position = Vector3.Lerp(Tr.position, Rcv_Position, Time.deltaTime);
        Tr.rotation = Quaternion.Slerp(Tr.rotation, Quaternion.Euler(Rcv_EulerAngles), (Time.deltaTime * 10.0f));
    }


    void Operation_ObtainItem_PosDataSend()
    {
        if (!hasMoved || ObtainItemUserKnd == CHAR_USER_KIND.NETWORK) return;

        Move_Time += Time.deltaTime;
        if (Move_Time >= Link_Script.i.Grenade_Data_BPS)
        {
            Move_Time = 0.0f;
            Send_ObtainGameItem_Data();
        }
    }

    public void Send_ObtainGameItem_Data()
    {
        if (Game_Script.GameOver_Check) return;


        ByteData Send_Buffer = new ByteData(128, 0);

        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.OBTAINITEM_MOVE);
        Send_Buffer.InPutByte((byte)CreatedIndex);
        Send_Buffer.InPutByte(Tr.position);
        Send_Buffer.InPutByte(Tr.rotation.eulerAngles);

        Net_Script.Send_PTP_Data(Send_Buffer);
    }


   



    public void Recv_ObtainGameItem_Data(ByteData _Receive_data)
    {
        Rcv_Position = _Receive_data.GetVector3();
        Rcv_EulerAngles = _Receive_data.GetVector3();

       
    }



    public void Init_LocationPos(ByteData _Receive_data)
    {
        hasMoved = true;

        Recv_ObtainGameItem_Data(_Receive_data);
        Tr.position = Rcv_Position;
        Tr.rotation = Quaternion.Slerp(Tr.rotation, Quaternion.Euler(Rcv_EulerAngles), (Time.deltaTime * 10.0f));
    }
    public void Init_LocationPos(Vector3 pos, Vector3 eulerAngles)
    {
        hasMoved = true;

        Tr.position = Rcv_Position  = pos;

        Rcv_EulerAngles = eulerAngles;
        Tr.rotation = Quaternion.Slerp(Tr.rotation, Quaternion.Euler(eulerAngles), (Time.deltaTime * 10.0f));
    }

  

}
