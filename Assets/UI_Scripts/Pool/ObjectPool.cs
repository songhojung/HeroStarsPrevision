using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
	//한종류 오브젝트 일떄 쓰일 멤버 변수
	private Stack<T> Stk_ObjectPools;				//오브젝트 담을 stack컬렉션
	private T OriObject;							//Original 오브젝트
	private int AllocateCount;						//할당할 갯수
	private string objName;						//오브젝트 이름

	//한종류 이상 오브젝트풀 일떄 쓰일 변수
	private Dictionary<string, T> Dic_ObejctPools;		//오브젝트 담을 Dictionary컬렉션
	private Dictionary<string, T> Dic_OrinObjectPools;	//Original 오브젝트 담을 Dic컬렉션


	private Transform parentTr;


	public ObjectPool()
	{}

	public ObjectPool(T _oriObject , Transform _parentTr, string _Name)
	{
		OriObject = _oriObject;
		parentTr = _parentTr;
		objName = _Name;
		Stk_ObjectPools = new Stack<T>();
	}

	public ObjectPool( Transform _parentTr)
	{
		parentTr = _parentTr;

		Dic_ObejctPools = new Dictionary<string, T>();
		Dic_OrinObjectPools = new Dictionary<string, T>();
	}


	public void Allocate(int AllocCount)
	{
		for (int i = 0; i < AllocCount; i ++ )
		{
			T obj = GameObject.Instantiate<T>(OriObject);
			obj.transform.SetParent(parentTr);
			obj.name = objName + i.ToString();
			obj.gameObject.SetActive(false);

			Stk_ObjectPools.Push(obj);
		}
	}

	public void Allocate(string DicKey ,T _oriObject)
	{
		if (!Dic_OrinObjectPools.ContainsKey(DicKey))
			Dic_OrinObjectPools[DicKey] = _oriObject; //ori 오브젝트를 담자

		T obj = GameObject.Instantiate<T>(_oriObject);
		obj.transform.SetParent(parentTr);
		obj.name = _oriObject.name;
		obj.gameObject.SetActive(false);

		Dic_ObejctPools[DicKey] = obj;
	}


	public void AllocateDirect(string DicKey ,T _oriObject)
	{
		if (!Dic_OrinObjectPools.ContainsKey(DicKey))
		{
			Dic_OrinObjectPools[DicKey] = _oriObject;
		}
		_oriObject.gameObject.transform.SetParent(parentTr);
		_oriObject.gameObject.name = _oriObject.name;
		_oriObject.gameObject.SetActive(false);
		Dic_ObejctPools[DicKey] = _oriObject;
	}


	public T Get_Object()
	{
		T poolObj = default(T);

		if (Stk_ObjectPools.Count > 0)
		{
			poolObj = Stk_ObjectPools.Pop();
			poolObj.gameObject.SetActive(true);
		}

		return poolObj;
	}

	public T Get_Object(string DicKey)
	{
		T poolObj = default(T);


		if (Dic_ObejctPools.Count > 0)
		{
			poolObj = Dic_ObejctPools[DicKey];
			poolObj.gameObject.SetActive(true);
		}

		return poolObj;
	}


	public void Release_Object(T obj)
	{
        obj.transform.SetParent(parentTr);
        obj.gameObject.SetActive(false);
		Stk_ObjectPools.Push(obj);
	}

	public void Release_Object(string dicKey)
	{
		if (Dic_ObejctPools.ContainsKey(dicKey))
		{
			Dic_ObejctPools[dicKey].transform.SetParent(parentTr);
			Dic_ObejctPools[dicKey].gameObject.SetActive(false);

		}
		else
			Debug.LogError("Not exist objects in the ObjectPool");
	}

	public bool IsContain(string name)
	{
		return Dic_ObejctPools.ContainsKey(name);
	}

	// 오브젝트의 트랜스폼이 ori의 transform과 같게 하기
	public void SetTransfrom_Object(string dicKey, Transform tr)
	{

		if (Dic_ObejctPools.ContainsKey(dicKey))
		{
			GameObject CloneObj = Dic_ObejctPools[dicKey].gameObject;
			GameObject orinObj = Dic_OrinObjectPools[dicKey].gameObject;

			CloneObj.transform.SetParent(tr);
				
			RectTransform _cloneRectTr = CloneObj.GetComponent<RectTransform>();
			RectTransform _OrinIObjRectTr = orinObj.GetComponent<RectTransform>();

			_cloneRectTr.sizeDelta = _OrinIObjRectTr.sizeDelta;
            _cloneRectTr.anchoredPosition = _OrinIObjRectTr.anchoredPosition;
            //_cloneRectTr.anchoredPosition = Vector2.zero ;

            _cloneRectTr.anchoredPosition3D = _OrinIObjRectTr.anchoredPosition3D;
            //_cloneRectTr.anchoredPosition3D = Vector3.zero;

            //_cloneRectTr.localScale = _OrinIObjRectTr.localScale;
            _cloneRectTr.localScale = Vector3.one;

		}
		else
		{
			Debug.LogError("Not allocated Object In the Obejct pools");
			return;
		}
	}
}

	

