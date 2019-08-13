using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine;
#endif

public class ByteData
{
    public static byte[] NullData = new byte[4];

    public byte[] data;         //데이터 전달
    public int DataIndex = 0;               //데이터 인덱스

    public ByteData(int _size, int _addLenth = 4)
    {
        DataIndex = _addLenth;
        data = new byte[_size + _addLenth];
    }

    public ByteData(byte[] _data)
    {
        data = _data;
    }

    /************************************************************************/
    /* private                                                              */
    /************************************************************************/

    public void InPutByte(bool InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(byte[] InPutData)
    {
        InPutByte(data, ref DataIndex, (short)InPutData.Length);
        InPutByte(data, ref DataIndex, InPutData, (short)InPutData.Length);
    }
    public void InPutByte(byte InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(short InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(int InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(uint InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(float InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(long InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(double InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(string InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(ushort InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData);
    }
    public void InPutByte(Vector3 InPutData)
    {
        InPutByte(data, ref DataIndex, InPutData.x);
        InPutByte(data, ref DataIndex, InPutData.y);
        InPutByte(data, ref DataIndex, InPutData.z);
    }

    public bool Getbool() { bool _value = false; OutPutVariable(ref _value); return _value; }
    public int Getint() { int _value = 0; OutPutVariable(ref _value); return _value; }
    public uint Getuint() { uint _value = 0; OutPutVariable(ref _value); return _value; }
    public short Getshort() { short _value = 0; OutPutVariable(ref _value); return _value; }
    public ushort Getushort() { ushort _value = 0; OutPutVariable(ref _value); return _value; }
    public long Getlong() { long _value = 0; OutPutVariable(ref _value); return _value; }
    public string Getstring() { string _value = string.Empty; ; OutPutVariable(ref _value); return _value; }
    public float Getfloat() { float _value = 0; OutPutVariable(ref _value); return _value; }
    public byte Getbyte() { byte _value = 0; OutPutVariable(ref _value); return _value; }
    public double Getdouble() { double _value = 0; OutPutVariable(ref _value); return _value; }
    public byte[] GetBytes() { byte[] _value = null; OutPutVariable(ref _value); return _value; }
    public ByteData GetByteArray()
    {
        short _len = 0;
        OutPutVariable(ref _len);     //문자열 길이 획득
        byte[] InPutData = new byte[_len];
        //길이 만큼 문자열 짜르자
        Array.Copy(data, DataIndex, InPutData, 0, _len);
        DataIndex += _len;

        return new ByteData(InPutData);

    }

    public Vector3 GetVector3()
    {
        float _x = 0; float _y = 0; float _z = 0;
        OutPutVariable(ref _x);
        OutPutVariable(ref _y);
        OutPutVariable(ref _z);
        return new Vector3(_x, _y, _z);
    }

    public void OutPutVariable(ref byte InPutData)
    {
        InPutData = data[DataIndex];
        DataIndex += 1;
    }
    public void OutPutVariable(ref short InPutData)
    {
        InPutData = BitConverter.ToInt16(data, DataIndex);
        DataIndex += 2;
    }
    public void OutPutVariable(ref ushort InPutData)
    {
        InPutData = BitConverter.ToUInt16(data, DataIndex);
        DataIndex += 2;
    }
    public void OutPutVariable(ref int InPutData)
    {
        InPutData = BitConverter.ToInt32(data, DataIndex);
        DataIndex += 4;
    }
    public void OutPutVariable(ref uint InPutData)
    {
        InPutData = BitConverter.ToUInt32(data, DataIndex);
        DataIndex += 4;
    }
    public void OutPutVariable(ref float InPutData)
    {
        InPutData = BitConverter.ToSingle(data, DataIndex);
        DataIndex += 4;
    }
    public void OutPutVariable(ref long InPutData)
    {
        InPutData = BitConverter.ToInt64(data, DataIndex);
        DataIndex += 8;
    }
    public void OutPutVariable(ref bool InPutData)
    {
        InPutData = BitConverter.ToBoolean(data, DataIndex);
        DataIndex += 1;
    }
    public void OutPutVariable(ref double InPutData)
    {
        InPutData = BitConverter.ToDouble(data, DataIndex);
        DataIndex += 8;
    }

    public void OutPutVariable(int OutPutDataSize, ref string InPutData)
    {
        InPutData = Encoding.UTF8.GetString(data, DataIndex, OutPutDataSize);
        DataIndex += OutPutDataSize;
    }

    public void OutPutVariable(ref string InPutData)
    {
        int _len = 0;
        OutPutVariable(data, ref DataIndex, ref _len);     //문자열 길이 획득
        //길이 만큼 문자열 짜르자
        InPutData = Encoding.UTF8.GetString(data, DataIndex, _len);
        DataIndex += _len;
    }

    public void OutPutVariable(ref byte[] InPutData)
    {
        short _len = 0;
        OutPutVariable(ref _len);     //문자열 길이 획득

        if (InPutData == null)
            InPutData = new byte[_len];

        //길이 만큼 문자열 짜르자
        Array.Copy(data, DataIndex, InPutData, 0, _len);
        DataIndex += _len;
    }

    public void OutPutVariable(ref Vector3 InPutData)
    {
        OutPutVariable(ref InPutData.x);
        OutPutVariable(ref InPutData.y);
        OutPutVariable(ref InPutData.z);
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, bool InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += 1;
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, byte[] InPutData)
    {
        Array.Copy(InPutData, 0, OutPutData, OutPutStartIndex, InPutData.Length);
        OutPutStartIndex += InPutData.Length;
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, byte[] InPutData, short _lenth)
    {
        Array.Copy(InPutData, 0, OutPutData, OutPutStartIndex, _lenth);
        OutPutStartIndex += _lenth;
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, byte InPutData)
    {
        OutPutData[OutPutStartIndex] = InPutData;
        OutPutStartIndex += 1;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, short InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, int InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, uint InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, float InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, long InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, double InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }

    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, string InPutData)
    {
        byte[] DataArray = Encoding.UTF8.GetBytes(InPutData);

        //문자열 길이 입력
        InPutByte(OutPutData, ref OutPutStartIndex, DataArray.Length);

        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, ushort InPutData)
    {
        byte[] DataArray = BitConverter.GetBytes(InPutData);
        Array.Copy(DataArray, 0, OutPutData, OutPutStartIndex, DataArray.Length);
        OutPutStartIndex += DataArray.Length;
    }
    public static void InPutByte(byte[] OutPutData, ref int OutPutStartIndex, Vector3 InPutData)
    {
        InPutByte(OutPutData, ref OutPutStartIndex, InPutData.x);
        InPutByte(OutPutData, ref OutPutStartIndex, InPutData.y);
        InPutByte(OutPutData, ref OutPutStartIndex, InPutData.z);
    }

    /************************************************************************/
    /*                                                                      */
    /************************************************************************/


    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref byte InPutData)
    {
        InPutData = OutPutData[OutPutStartIndex];
        OutPutStartIndex += 1;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref short InPutData)
    {
        InPutData = BitConverter.ToInt16(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 2;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref ushort InPutData)
    {
        InPutData = BitConverter.ToUInt16(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 2;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref int InPutData)
    {
        InPutData = BitConverter.ToInt32(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 4;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref float InPutData)
    {
        InPutData = BitConverter.ToSingle(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 4;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref long InPutData)
    {
        InPutData = BitConverter.ToInt64(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 8;
    }
    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref bool InPutData)
    {
        InPutData = BitConverter.ToBoolean(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 1;
    }

    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref double InPutData)
    {
        InPutData = BitConverter.ToDouble(OutPutData, OutPutStartIndex);
        OutPutStartIndex += 8;
    }

    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref Vector3 InPutData)
    {
        InPutData.x = BitConverter.ToSingle(OutPutData, OutPutStartIndex); OutPutStartIndex += 4;
        InPutData.y = BitConverter.ToSingle(OutPutData, OutPutStartIndex); OutPutStartIndex += 4;
        InPutData.z = BitConverter.ToSingle(OutPutData, OutPutStartIndex); OutPutStartIndex += 4;
    }

    public static void OutPutVariable(byte[] OutPutData, ref int OutPutStartIndex, ref string InPutData)
    {
        int _len = 0;
        OutPutVariable(OutPutData, ref OutPutStartIndex, ref _len);     //문자열 길이 획득

        //길이 만큼 문자열 짜르자
        InPutData = Encoding.UTF8.GetString(OutPutData, OutPutStartIndex, _len);
        OutPutStartIndex += _len;
    }

    //배열 길이 만큼 복사
    public static byte[] ArrayCopy(byte[] _byte, int _dataIndex)
    {

        byte[] _data = new byte[_dataIndex];

        Array.Copy(_byte, 0, _data, 0, _dataIndex); ;

        return _data;
    }


    //배열 길이 만큼 복사
    public static byte[] ArrayCopyLenthAdd(byte[] _byte, int _dataIndex)
    {

        byte[] _data = new byte[_dataIndex + 4];

        byte[] DataArray = BitConverter.GetBytes(_dataIndex);
        Array.Copy(DataArray, 0, _data, 0, DataArray.Length);

        Array.Copy(_byte, 0, _data, 4, _dataIndex); ;

        return _data;
    }


    public byte[] GetTrimByteData()
    {
        return ByteData.ArrayCopy(data, DataIndex);
    }
}
