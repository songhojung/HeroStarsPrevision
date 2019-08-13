using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelecter : MonoBehaviour {

	private Vector3 SelectingScale = new Vector3(1.5f, 1.5f, 1.5f);
	private Vector3 OrinScale = Vector3.zero;

	private Queue<Collider> Lst_selectedChar = new Queue<Collider>();

	private Queue<Coroutine> Que_routineSize = new Queue<Coroutine>();
	void Start () 
	{
		
	}
	
	

	public void OnTriggerEnter(Collider coll)
	{

		if (coll.tag == "ChacterSelecting")
		{
		//이전선택 캐릭터 크기 줄이자
			if (Lst_selectedChar.Count > 0)
				SetScale_WhenUnselected(Lst_selectedChar.Dequeue());

			// 현재 선택캐릭터 큐담기
			Lst_selectedChar.Enqueue(coll);

			if (OrinScale == Vector3.zero)
				OrinScale = coll.transform.localScale;

		

			if (Que_routineSize.Count > 0)
				StopCoroutine(Que_routineSize.Dequeue());

			//현재 선택된캐릭터 크기키우기
			Coroutine routine =	StartCoroutine(routine_LerpScaleUpObject(coll, SelectingScale, 4f));
			Que_routineSize.Enqueue(routine);

			


			//선택 되었다 그다음행동~
			characterInfo cInfo = coll.GetComponent<characterInfo>();


            UI_LobbyOld.Getsingleton.Apply_LobbyInfoUsingSelectedChar(cInfo);

            //선택된 캐릭터가 세트이펙트 활성여부
            cInfo.Chk_EquipSetBuf(true);

            //소유캐릭정보UI 재 활성화 ..에니메이션 재발동을위해 
            UI_LobbyOld.Getsingleton.On_reActiveUnlockInfo();
		}
	}



	void SetScale_WhenUnselected(Collider coll)
	{
		if (Que_routineSize.Count > 0)
			StopCoroutine(Que_routineSize.Dequeue());
		//현재 선택된캐릭터 크기줄이기
		Coroutine routine = StartCoroutine(routine_LerpScaleDownObject(coll, OrinScale, 4f));
		Que_routineSize.Enqueue(routine);

		//선택안되엇으니 세트이펙트 끔
		coll.GetComponent<characterInfo>().Chk_EquipSetBuf(false);

		//coll.transform.localScale = OrinScale;
	}


	IEnumerator routine_LerpScaleUpObject(Collider coll, Vector3 _targetScale, float _speed)
	{
		Vector3 nowS = coll.transform.localScale;
		Vector3 targetS = _targetScale;
		Vector3 lerpS = Vector3.zero ;
		float time = 0f;
		

	

		while (true)
		{

			
			
			if (Vector3.Equals(lerpS, targetS))
			{
				Que_routineSize.Dequeue();
				break;
			}
			time += Time.deltaTime * _speed;

			
				lerpS = Vector3.Lerp(nowS, targetS, time);

			coll.transform.localScale = lerpS;

			

			yield return null;
		}
	}


	IEnumerator routine_LerpScaleDownObject(Collider coll, Vector3 _targetScale, float _speed)
	{
		Vector3 nowS = coll.transform.localScale;
		Vector3 targetS = _targetScale;
		Vector3 lerpS = Vector3.zero;
		float time = 0f;
		

	

		

		while (true)
		{

		
			if (Vector3.Equals(lerpS, targetS))
			{
				Que_routineSize.Dequeue();
				break;
			}
			time += Time.deltaTime * _speed;

		
				lerpS = Vector3.Lerp(targetS, nowS, time);
			

			coll.transform.localScale = lerpS;



			yield return null;
		}
	}
}
