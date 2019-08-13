using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetWork_Script
{
    private Socket Socket_Client;
    private byte[] BeginReceive_Packet;    
    private ByteData Receive_ByteData;    
    private int EndReceive_Length = 0;
    private const int LENGTH = 1024;
    private const int TOTAL_LENGTH = 2048;
    
    static byte[] HeaderData = new byte[] { (byte)'S', (byte)'F' };		//헤더
    
    SocketError SocketErrorCode;

    public delegate void ReceiveDelegate(byte[] Packet_Data);
    ReceiveDelegate Receive_Delegate;
    public delegate void ErrorDelegate();
    ErrorDelegate Error_Delegate;

    bool BeginReceive_Start_Check = false;
    
    public bool IsConnected
    {
        get
        {
            if (Socket_Client == null) return false;
            return Socket_Client.Connected;
        }
        set { }
    }

    public void Delegate_Init(ReceiveDelegate _Receive_Delegate, ErrorDelegate _Error_Delegate)
    {
        Receive_Delegate = _Receive_Delegate;
        Error_Delegate = _Error_Delegate;
    }

    //================================================================================================================================================================================

    public void Connect_Start(string IP_Address, int Port)
    {
        Total_Recv_Buffer = new byte[TOTAL_LENGTH];
        Total_Buffer_Max_Index = 0;
        BeginReceive_Start_Check = false;

        try
        {
            //---------------------------------------------------------------------------------------------------
            //Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Socket_Client.BeginConnect(IP_Address, Port, Connection_Accept, null);            
            //---------------------------------------------------------------------------------------------------


            //---------------------------------------------------------------------------------------------------

            IPAddress curAdd = null;

            if (IPAddress.TryParse(IP_Address, out curAdd))
            {
                if (curAdd.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // v6인 경우
					//Debug.Log("v6  curAdd :" + curAdd);
                    Socket_Client = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                }
                else
                {
                    // v4인 경우
					//Debug.Log("v4  curAdd :" + curAdd);
                    Socket_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }

                Socket_Client.BeginConnect(IP_Address, Port, Connection_Accept, null);
            }
            else
            {
                IPHostEntry heserver = Dns.GetHostEntry(IP_Address);
				//Debug.Log(" heserver :" + heserver);
                foreach (IPAddress _curAdd in heserver.AddressList)
                {
                    Socket_Client = new Socket(_curAdd.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    Socket_Client.BeginConnect(_curAdd, Port, Connection_Accept, null);
                }
            }

            //---------------------------------------------------------------------------------------------------

        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    private void Connection_Accept(IAsyncResult result)
    {
        try
        {
            Socket_Client.EndConnect(result);

            BeginReceive_Init();
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    //================================================================================================================================================================================

    
    void BeginReceive_Init()
    {
        try
        {
            if (Socket_Client != null)
            {
                BeginReceive_Start_Check = true;

                BeginReceive_Packet = new byte[LENGTH];
                Socket_Client.BeginReceive(BeginReceive_Packet, 0, BeginReceive_Packet.Length, SocketFlags.None, Received_Callback, BeginReceive_Packet);                
            }
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }
    
    private void Received_Callback(IAsyncResult result)
    {
        if (Socket_Client == null || IsConnected == false) return;

        //콜백 함수를 한번 등록했는데 2번 연속 호출되는 경우의 폰들이 있기때문에 체크후 막아준다.
        //2번째 호출되는 콜백함수의 들어온 값은 이전과 동일하다고 믿는 전제하에 막는것임.
        //연속되게 들어온 데이터가 이전과 동일한지는 모른다
        if (BeginReceive_Start_Check == false) return;
        BeginReceive_Start_Check = false;

        try
        {
            EndReceive_Length = Socket_Client.EndReceive(result, out SocketErrorCode);

            if (SocketErrorCode != SocketError.Success)
            {
                //Debug.Log("EndReceive_Length = " + EndReceive_Length + ", LENGTH = " + LENGTH);

                Error_DisConnect(SocketErrorCode.ToString());

                return;
            }

            if (EndReceive_Length <= 0)
            {
                //Debug.LogError("받은 데이터가 0 값이다!!! Received_Callback() EndReceive_Length = " + EndReceive_Length);

                BeginReceive_Init();

                return;
            }

            //토탈 버퍼에 담아서 받은 패킷 나눠주는 처리 한다.
            Total_Recv_Operation(EndReceive_Length, (byte[])result.AsyncState);

            BeginReceive_Init();
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }


    byte[] Total_Recv_Buffer = new byte[TOTAL_LENGTH];
    int Total_Buffer_Max_Index = 0;

    //토탈 버퍼에 담아서 받은 패킷 나눠주는 처리 한다.
    void Total_Recv_Operation(int _Recv_Buffer_Size, byte[] _Recv_Buffer)
    {
        try
        {
            //토탈 버퍼에 현재 받은 데이터 담기			
            Array.Copy(_Recv_Buffer, 0, Total_Recv_Buffer, Total_Buffer_Max_Index, _Recv_Buffer_Size);
            Total_Buffer_Max_Index += _Recv_Buffer_Size;
            ByteData _Buffer_Data = new ByteData(Total_Recv_Buffer);

            while (true)
            {
                //헤더검사를 할수 있을만큼 남았는지 체크
                if (Total_Buffer_Max_Index <= (_Buffer_Data.DataIndex + 4)) break;

                //헤더 문자열 일치 체크
                if (_Buffer_Data.Getbyte() != HeaderData[0]) continue;
                if (_Buffer_Data.Getbyte() != HeaderData[1]) { _Buffer_Data.DataIndex--; continue; }

                //헤더 길이 만큼 전체 데이터 받았는지 체크
                short _Data_Length = _Buffer_Data.Getshort();

                if (Total_Buffer_Max_Index < _Buffer_Data.DataIndex + _Data_Length)
                {
                    _Buffer_Data.DataIndex -= 4;
                    break;
                }

                //길이 읽은거 다시 감소
                _Buffer_Data.DataIndex -= 2;

                Receive_Delegate(_Buffer_Data.GetBytes());
            }
            			
            //처리한 버퍼 지우고 뒤에 남은 버퍼 앞으로 땡기자
            Total_Buffer_Max_Index -= _Buffer_Data.DataIndex;

            byte[] Temp_Array = new byte[Total_Buffer_Max_Index];
            Array.Copy(Total_Recv_Buffer, _Buffer_Data.DataIndex, Temp_Array, 0, Temp_Array.Length);
            Array.Copy(Temp_Array, 0, Total_Recv_Buffer, 0, Temp_Array.Length);            
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    //================================================================================================================================================================================
            
    private void Send_Packet(byte[] data)
    {
        if (Socket_Client == null || IsConnected == false) return;

        try
        {
            Socket_Client.BeginSend(data, 0, data.Length, SocketFlags.None, Send_Callback, null);
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    private void Send_Callback(IAsyncResult result)
    {
        if (Socket_Client == null || IsConnected == false) return;

        try
        {
            Socket_Client.EndSend(result, out SocketErrorCode);

            if (SocketErrorCode != SocketError.Success)
            {
                Error_DisConnect(SocketErrorCode.ToString());
            }
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    //================================================================================================================================================================================

    void Error_DisConnect(Exception ex)
    {
        Error_DisConnect(ex.ToString());
    }

    public void Error_DisConnect(String Error_Note)
    {   
        Debug.LogError("Error_Note : " + Error_Note);

		//if (Error_Delegate == null)
		//	Debug.Log("Error_Delegate  NULL " + "Socket_Client STATE  :" +Socket_Client);
		//else
		{
		//	Debug.Log("Error_Delegate LIVE");

			Error_Delegate();

		}


        Disconnect();
    }

    public void Disconnect()
    {
        if (Socket_Client == null) return;

        if (Socket_Client.Connected) Socket_Client.Shutdown(SocketShutdown.Both);
        Socket_Client.Close();
        Socket_Client = null;
    }

    //================================================================================================================================================================================

    public readonly object Total_Send_Lock = new object();
    byte[] Total_Send_Packet;
    int Total_Send_Index = 0;
    int Total_Send_Packet_Length = 0;

    public void Sum_Send_Paket_Init()
    {
        Total_Send_Packet_Length = LENGTH;
        Total_Send_Packet = new byte[Total_Send_Packet_Length];
        Total_Send_Index = 0;
    }

    //패킷 일정시간동안 모아서 보낼때 사용
    public void Now_Send_Packet()
    {
        lock (Total_Send_Lock)
        {
            if (Total_Send_Index <= 0) return;

            byte[] _Total_Send_Packet = new byte[Total_Send_Index];

            Array.Copy(Total_Send_Packet, 0, _Total_Send_Packet, 0, Total_Send_Index);

            Send_Packet(_Total_Send_Packet);

            Sum_Send_Paket_Init();
        }        
    }

    //================================================================================================================================================================================

    ByteData Send_Data_Buffer;
    int Buffer_Length = 3;
    
    //프로토콜 맞춰서 보내기
    public void Send_Data(NETKIND _Protocol, byte[] _Data, bool _Sum_Check = false)
    {
        if (Socket_Client == null || IsConnected == false) return;

        if (_Data == null) _Data = new byte[0];

        try
        {
            Send_Data_Buffer = new ByteData(HeaderData.Length + Buffer_Length + _Data.Length, 0);

            for (int i = 0; i < HeaderData.Length; i++)
            {
                Send_Data_Buffer.InPutByte(HeaderData[i]);
            }

            Send_Data_Buffer.InPutByte((short)(Buffer_Length + _Data.Length - sizeof(short)));//길이 ("길이" 데이터 제외한 크기)
            Send_Data_Buffer.InPutByte((byte)_Protocol);
            
            Array.Copy(_Data, 0, Send_Data_Buffer.data, HeaderData.Length + Buffer_Length, _Data.Length);

            Send_Data_Buffer.DataIndex += _Data.Length;


            if (_Sum_Check == false)//바로 패킷 보내기
            {
                Send_Packet(Send_Data_Buffer.GetTrimByteData());
            }
            else//패킷 모아서 보내기
            {
                lock (Total_Send_Lock)
                {
                    Array.Copy(Send_Data_Buffer.data, 0, Total_Send_Packet, Total_Send_Index, Send_Data_Buffer.DataIndex);

                    Total_Send_Index += Send_Data_Buffer.DataIndex;
                }

                //일정양 이상 쌓였으면 꽉차기전에 미리 보내버린다.
                if (Total_Send_Index + 128 >= Total_Send_Packet_Length) Now_Send_Packet();
            }
        }
        catch (Exception ex)
        {
            Error_DisConnect(ex);
        }
    }

    //================================================================================================================================================================================

}
