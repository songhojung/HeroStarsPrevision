using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LobbyCharacterPositioning : MonoBehaviour 
{
	public List<characterInfo> characters = new List<characterInfo>();
	public Transform Tr_Look;

	//위치선정 관련
	public  GameObject center;														//중	심체 오브젝트
	private float sumDegree = 180;													//회	전된 각도 총합
	private List<Vector3> Lst_LocationPos = new List<Vector3>();						// 위치할 좌표 리스트
	private List<float> Lst_Degrees = new List<float>();								// 회전 각도들 리스트
	private Dictionary<uint, float> Dic_ChrctDegree = new Dictionary<uint, float>();	// 회전체의 회전의 대한 캐릭터별 회전각도


	//회전관련
	public Transform CharRotate_Tr;							//회전체 Tr
	//public bool isSelectedCharacter = false;
	private TouchPhase TouchType = TouchPhase.Ended;			//현재터치 타입
	private Vector3 Pos_whenTouchArea = Vector3.zero;		// 처음터치하엿던 위치
	private Vector3 nowMousePos = Vector3.zero;				//현재 터치한 위치
	private float RotateSensitive_value = 0f;				//부러운회전을위한 보정값
	private Touch touch;									
	float Dis = 0;										//회전량
	float bfDis = 0;										//이전회전량


	//모드 관련
	public bool IsRandPosioning = false;
	public uint firstSelectUnitIdx = 0;

	public void OnDisable()
	{
		

		//TouchType = TouchPhase.Canceled;
	}


	// Use this for initialization
	void Start () 
	{
		CharRotate_Tr = this.transform;

#if UNITY_EDITOR
		RotateSensitive_value = 0.1f;
#else
		RotateSensitive_value = 15f;		
#endif
	}

	public void AddList_Character(characterInfo unitObj)
	{
		characters.Add(unitObj);
	}

	//캐릭터들 위치 잡아준다 
	public void makePosition()
	{
		//makePositionCnt = posCount;

		float perDegree = 360f / characters.Count;
		Transform bodyTr = center.transform;

		for (int i = 0; i < characters.Count; i++)
		{



			sumDegree += perDegree;
			//if (sumDegree >= 360)
			//{
			//	sumDegree = 0;
			//}

			
			bodyTr.localRotation = Quaternion.Euler(new Vector3(0, sumDegree, 0));
			Vector3 frontpos = bodyTr.transform.forward *1.5f;
			frontpos.y = 0;
			Vector3 Makepos = frontpos;
			//GameObject objj = new GameObject("newobj");
			//objj.transform.parent = CharRotate_Tr;
			//objj.transform.localPosition = Makepos;

			Lst_LocationPos.Add(Makepos);
			Lst_Degrees.Add(sumDegree - 180f);
		}



		// 위치잡자
		List<int> randPosition = null;
		if (IsRandPosioning)
			randPosition = make_randIndxArray(characters.Count);

		for (int i = 0; i < characters.Count; i++)
		{
			if (!IsRandPosioning) // 랜덤포지션아님
			{
				characters[i].transform.localPosition = Lst_LocationPos[i];
				//if (characters[i].userUnit != null)
					Dic_ChrctDegree[characters[i].unitInfos.UnitIdx] = Lst_Degrees[i] * (-1f);
			}
			else //랜덤포지션 설정
			{
				characters[i].transform.localPosition = Lst_LocationPos[randPosition[i]];
				//if (characters[i].userUnit != null)
				Dic_ChrctDegree[characters[i].unitInfos.UnitIdx] = Lst_Degrees[randPosition[i]] * (-1f);

				//램덤 유닛인덱스 부여를위해 
				if (randPosition[i] == characters.Count - 1)
				firstSelectUnitIdx = characters[i].unitInfos.UnitIdx;
			}
		}

		Apply_LookAtFrontChrct();

	}

	//특정 캐릭터 나오게 회전시켜준다
	public void SetRotate_TargetCharacter(uint _unitIdx)
	{
		if (CharRotate_Tr == null)
			CharRotate_Tr = this.transform;

		if (Dic_ChrctDegree.ContainsKey(_unitIdx))
		{
			//유닛 있는곳으로 회전
			CharRotate_Tr.localRotation = Quaternion.Euler(new Vector3(0, Dic_ChrctDegree[_unitIdx], 0));

			//bfDis = 0;
			//bfDis -= Dic_ChrctDegree[_unitIdx];

			Dis = 0;
			Dis -= Dic_ChrctDegree[_unitIdx];

			//유닛 정면바라보기 
			Apply_LookAtFrontChrct();
		}
	}


	public void Clear_makedPosition()
	{
		characters.Clear();
		sumDegree = 180;
		Lst_LocationPos.Clear();
		Lst_Degrees.Clear();
		Dic_ChrctDegree.Clear();
		CharRotate_Tr.localRotation = Quaternion.Euler(Vector3.zero);
	}


	void Update()
	{
		//캐릭터 터치영역 들어왓는지 확인
		//Chk_TouchSelectArea();
		StaticMethod.Chk_TouchSelectArea(User.isSelectedCharacter, ref TouchType, ref Pos_whenTouchArea, "CharSelectArea");
		//캐릭터 선택하기 (회전시키기)
		CharacterRotating();

	

	}



	
	//캐릭터 선택하기 (회전시키기)
	void CharacterRotating()
	{
		//터치영역에 들어옴
		//Debug.Log("TouchType : " + TouchType);

		if (TouchType == TouchPhase.Began)
			TouchType = TouchPhase.Moved;
		else if (TouchType == TouchPhase.Moved)
		{

#if UNITY_EDITOR
			if (nowMousePos == Input.mousePosition)
				return;

			nowMousePos = Input.mousePosition;
			//Debug.Log("nowMousePos : " + nowMousePos);
#else
				touch = Input.GetTouch(0);
				Vector3 touchPosVector3 = new Vector3(touch.position.x, touch.position.y, 100);
				Vector3 touchPos = Camera.main.ScreenToWorldPoint(touchPosVector3);
				//Debug.Log("touchPos : " + touchPos);

				if(nowMousePos == touchPos)
				{

					return;
				}
				nowMousePos = touchPos;
#endif

			 float xDiff = nowMousePos.x - Pos_whenTouchArea.x;

			 xDiff = xDiff * RotateSensitive_value;

			 Dis = bfDis + xDiff;
			 //Debug.Log("bfDis : " + bfDis + "\n" + "xDiff : " + xDiff);



			 float _y = CharRotate_Tr.localRotation.y - Dis;

			 //Debug.Log("CharRotate_Tr.localRotation.y - Dis : " + _y);
			 //_y = RotateSensitive_value * _y;

			 CharRotate_Tr.localRotation = Quaternion.Euler(new Vector3(0, _y, 0));

			 //Debug.Log("CharRotate_Tr.localRotation : " + CharRotate_Tr.localRotation);

			//캐릭터들 앞에 보게하기
			Apply_LookAtFrontChrct();
		}
		else if (TouchType == TouchPhase.Ended)
		{
			bfDis = Dis;

			DoCharacterAni();
		}
		else if (TouchType == TouchPhase.Canceled)
		{

		}
	}

	//캐릭터들 앞에 보게하기
	void Apply_LookAtFrontChrct()
	{
		for (int i = 0; i < characters.Count; i++)
		{

			characters[i].transform.rotation = new Quaternion(0, 1f, 0, 0);
			//Vector3 pos = Tr_Look.localPosition;
			//pos.y = 0;
			//characters[i].transform.localRotation = Quaternion.LookRotation(pos);
			//characters[i].transform.LookAt(Camera.main.transform);
			//characters[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 20, 0));
		}
	}

	public void DoCharacterAni()
	{

	}

	List<int> make_randIndxArray(int maxIdx)
	{
		List<int> lst_randIdx = new List<int>();

		while (lst_randIdx.Count < maxIdx)
		{
			int x = Random.Range(0, maxIdx);

			if (!lst_randIdx.Contains(x))
				lst_randIdx.Add(x);
			
		}

		return lst_randIdx;
	}

}
