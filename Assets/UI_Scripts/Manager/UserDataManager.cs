using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager
{
	public static UserDataManager instance = new UserDataManager();

	/// <summary>
	/// 나의 유저정보
	/// </summary>
	public User user = new User();
	
	/// <summary>
	/// 다른유저의 유저정보
	/// </summary>
	public User OtherUser = new User();

	public bool isOtherUser = false;

	public Queue<object> Que_OtherUserIDs = new Queue<object>(); // 검색할 유저 아이디를 큐에담자

	public UserDataManager()
	{

	}

	public User GetOtherUser
	{
		get
		{
			OtherUser.Init();
			return OtherUser;
		}

	}



	

	//public static UserDataManager m_instance = null;
	//public static UserDataManager instance
	//{
	//    get
	//    {
	//        if (m_instance == null)
	//        {
	//            m_instance = new UserDataManager();
	//        }
	//        return m_instance;
	//    }
	//}

	//public void Init()
	//{
	//}

}
