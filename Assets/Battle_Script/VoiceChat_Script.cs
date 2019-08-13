//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using UnityEngine.Audio;

//public class VoiceChat_Script : MonoBehaviour
//{       
//    Network_Battle_Script Net_Script;

//    AudioSource Voice_AudioSource;
//    AudioClip Voice_Clip;
//    String Microphone_Name = "";

//    bool VoiceChat_Check = false;
//    int Before_Record_Pos = 0;    
//    int Frequency = 0;
//    int Recording_Length = 0;

//    void Start()
//    {
//        Net_Script = GameObject.Find("Network_Script").GetComponent<Network_Battle_Script>();

//        Voice_AudioSource = this.GetComponent<AudioSource>();

//        Frequency = 8000; //44100, 8000, AudioSettings.outputSampleRate
//        Recording_Length = 100;

//        if (Microphone.devices.Length > 0)
//        {
//            VoiceChat_Check = true;

//            Microphone_Name = Microphone.devices[0].ToString();

//            Voice_Clip = Microphone.Start(Microphone_Name, true, Recording_Length, Frequency);

//            Debug.LogWarning("검색된 마이크 갯수 : " + Microphone.devices.Length + "개, 설정된 마이크 이름 : " + Microphone_Name);
//        }
//        else
//        {
//            VoiceChat_Check = false;

//            Debug.LogWarning("검색된 마이크 디바이스가 없다.!!!!!!!!!!");
                        

//        }
//    }

//    float Time_Check = 0.0f;

//    void Update()
//    //void FixedUpdate()
//    {
//        if (VoiceChat_Check == false) return;

//        int Now_Record_Pos = Microphone.GetPosition(Microphone_Name);

//        int Difference_Pos = Now_Record_Pos - Before_Record_Pos;

//        if (Difference_Pos > 0)
//        {
//            //float[] Voice_Buffer = new float[Difference_Pos * Voice_Clip.channels];

//            //Voice_Clip.GetData(Voice_Buffer, Before_Record_Pos);

//            //byte[] Data_Buffer = ToByteArray(Voice_Buffer);
                        
//            //Voice_Chat_Send_Data(Data_Buffer, Voice_Clip.channels);








//            float[] Voice_Buffer = new float[Difference_Pos * Voice_Clip.channels];

//            Voice_Clip.GetData(Voice_Buffer, Before_Record_Pos);

//            byte[] Data_Buffer = ToByteArray(Voice_Buffer);

//            Voice_Chat_Send_Data(Data_Buffer, Voice_Clip.channels);




//            //float[] buffer = new float[audioClip.samples * audioClip.channels];
//            //audioClip.GetData(buffer, 0);




//            //float modifier = (float)audioClip.frequency / (float)newFrequency;

//            //float length = audioClip.length;
//            //int newNumberofSamples = Mathf.FloorToInt(length * newFrequency);
//            //int newBuffLen = newNumberofSamples * audioClip.channels;

//            //float[] newBuffer = new float[newBuffLen];



//            //for (int i = 0; i < newBuffLen; i++)
//            //{
//            //    newBuffer = buffer[Mathf.FloorToInt((float)i * modifier)];
//            //}





//        }

//        Before_Record_Pos = Now_Record_Pos;

        




//        if (Recive_Voice_Data.Length > 0)
//        {
            

//            Voice_AudioSource.clip = AudioClip.Create("Voice_Chat_Clip", Recive_Voice_Data.Length, Recive_Channels, Frequency, false);

//            Voice_AudioSource.clip.SetData(Recive_Voice_Data, 0);

//            if (!Voice_AudioSource.isPlaying) Voice_AudioSource.Play();

//            Recive_Voice_Data = new float[0];
//        }

        
//    }


//    public void Voice_Chat_Send_Data(byte[] Data_Buffer, int Channels)
//    {
//        if (Data_Buffer.Length <= 0) return;

//        //Debug.Log("111 = " + (1 + 4 + 2 + Data_Buffer.Length + 4));

//        ByteData Send_Buffer = new ByteData(1 + 4 + 2 + Data_Buffer.Length + 4, 0);

//        Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.VOICE_CHAT);
//        Send_Buffer.InPutByte(Data_Buffer.Length);
//        Send_Buffer.InPutByte(Data_Buffer);
//        Send_Buffer.InPutByte(Channels);

//        Net_Script.Send_PTP_Data(Send_Buffer);





//        //ByteData Send_Buffer = new ByteData(1024, 0);

//        //Send_Buffer.InPutByte((byte)RELAY_PROTOCOL.VOICE_CHAT);
//        //Send_Buffer.InPutByte(100);
//        //Send_Buffer.InPutByte(100);
//        //Send_Buffer.InPutByte(Channels);

//        //Net_Script.Send_PTP_Data(Send_Buffer);
        


       

//        //Recive_Channels = Channels;
//        //Recive_Voice_Data = ToFloatArray(Data_Buffer);

//    }

//    float[] Recive_Voice_Data = new float[0];
//    int Recive_Channels = 0;

//    //public void Recive_Data(byte[] Data_Buffer, int Channels)
//    public void Recive_Data(ByteData _Receive_data)
//    {

//        Debug.Log("aaaaaaaaaa");


//        int Data_Buffer_Length = 0;
//        _Receive_data.OutPutVariable(ref Data_Buffer_Length);

//        byte[] Data_Buffer = new byte[Data_Buffer_Length];
//        _Receive_data.OutPutVariable(ref Data_Buffer);

//        _Receive_data.OutPutVariable(ref Recive_Channels);

//        Recive_Voice_Data = ToFloatArray(Data_Buffer);

//        //--------------------------------------------------------------------------------

//        ////Voice_AudioSource.clip = AudioClip.Create("Voice_Chat_Clip", Recive_Voice_Data.Length, Channels, Frequency, false);                                            
//        ////Voice_AudioSource.clip.SetData(Recive_Voice_Data, 0);
//        ////if (!Voice_AudioSource.isPlaying) Voice_AudioSource.Play();

//        //--------------------------------------------------------------------------------
//    }

//    public byte[] ToByteArray(float[] floatArray)
//    {
//        int len = floatArray.Length * 4;
//        byte[] byteArray = new byte[len];
//        int pos = 0;
//        foreach (float f in floatArray)
//        {
//            byte[] data = System.BitConverter.GetBytes(f);
//            System.Array.Copy(data, 0, byteArray, pos, 4);
//            pos += 4;
//        }
//        return byteArray;
//    }

//    public float[] ToFloatArray(byte[] byteArray)
//    {
//        int len = byteArray.Length / 4;
//        float[] floatArray = new float[len];
//        for (int i = 0; i < byteArray.Length; i += 4)
//        {
//            floatArray[i / 4] = System.BitConverter.ToSingle(byteArray, i);
//        }
//        return floatArray;
//    }


//}
