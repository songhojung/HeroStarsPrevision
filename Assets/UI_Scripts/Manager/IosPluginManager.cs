using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections;

/// <summary>
/// Activates and Deactivates (shows and hides) the native UI via native code in UIBinding.m
/// </summary>

public class IosPluginManager :MonoBehaviour
{
	private static IosPluginManager _instance;
	public static IosPluginManager Getsingleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(IosPluginManager)) as IosPluginManager;
				if (_instance == null)
				{
					_instance = new GameObject("IosPluginManager").AddComponent<IosPluginManager>();
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			return _instance;
		}
	}



	void Awake()
	{
		Init ();
	}

	public void Init()
	{
		
	}


    #if UNITY_IOS
	[DllImport("__Internal")]
	public static extern string getUUID();

	[DllImport("__Internal")]
	public static extern string getCountryCode ();

	[DllImport("__Internal")]
	public static extern void showWebView (string url);

#endif
    public string getGUID()
    {
//#if UNITY_EDITOR
//        string GUID = string.Empty;
//        for (int i = 0; i < 18; i++)
//            GUID += System.Convert.ToString(UnityEngine.Random.Range(1, 10));
//        return GUID;
#if UNITY_IPHONE
        return getUUID();
#elif UNITY_ANDROID
		return SystemInfo.deviceUniqueIdentifier;
#endif
    }





	//public string get_CountryCode()
	//{
	//	return getCountryCode ();
	//}



	//public void StartWebView(string url)
	//{
	//	showWebView (url);
	//}
}

