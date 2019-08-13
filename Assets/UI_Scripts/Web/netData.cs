using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;




public class netData
{

	public WWWForm form = new WWWForm();

	public ProtocolName protocolName = ProtocolName.NONE;

	public Dictionary<string, object> SendData = new Dictionary<string, object>();

	public del_webResp_0 del_result;

	const string Params = "Params";
	

	//프로토콜 호출 여러종류의 의한 에러로 반환값이 들어 오지 않을떄 다시 프로토콜을 쏴야될경우
	//이전 값그대로 프로토콜을 다시 쏴야한다. 그떄 시퀀시로 이용해서 다시 쏘자
	static uint Seq = 0;

	public netData()
	{
		Seq++;
		SendData.Add("Seq", Seq);

		if (!SendData.ContainsKey(DefineKey.UserID))
			SendData.Add(DefineKey.UserID, UserDataManager.instance.user.user_Users.UserID);
		if (!SendData.ContainsKey(DefineKey.LgnToken))
			SendData.Add(DefineKey.LgnToken, UserDataManager.instance.user.user_Users.LgnToken);

	}

	public netData(ProtocolName _protocolName)
	{
		Seq++;
		SendData.Add("Seq", Seq);

	}

    [Obsolete]
 //   public WWW get_WebSend(string _url)
	//{
	//	WWWForm _form = new WWWForm();

	//	string _jsonStr = MiniJSON.Json.Serialize(SendData);



	//	UserEditor.Getsingleton.EditLog(string.Format("SendURL : <b><size=16><color=#28ff65>{0}</color></size></b>", _url));
	//	UserEditor.Getsingleton.EditLog(string.Format("json str : <b><color=#28ff65>{0}</color></b>", _jsonStr));


	//	_form.AddField(Params, _jsonStr);

	//	WWW www = new WWW(_url, _form);
	//	return www;
	//}

    public UnityWebRequest get_WebSend(string _url)
    {
        WWWForm _form = new WWWForm();

        string _jsonStr = MiniJSON.Json.Serialize(SendData);



        UserEditor.Getsingleton.EditLog(string.Format("SendURL : <b><size=16><color=#28ff65>{0}</color></size></b>", _url));
        UserEditor.Getsingleton.EditLog(string.Format("json str : <b><color=#28ff65>{0}</color></b>", _jsonStr));


        _form.AddField(Params, _jsonStr);

        UnityWebRequest www = UnityWebRequest.Post(_url, _form);
        return www;
    }

    public void set_SendData(string _key, object _value)
	{
		SendData[_key] = _value;
	}
}
