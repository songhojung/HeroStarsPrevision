using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loadmanager:MonoBehaviour
{
	public static Loadmanager m_instance = null;
	public static Loadmanager instance
	{
		get
		{
			//if (m_instance == null)
			//{
			//    m_instance = new Loadmanager();
			//}
			//return m_instance;

			if (m_instance == null)
			{
				m_instance = GameObject.FindObjectOfType(typeof(Loadmanager)) as Loadmanager;

				if (m_instance == null)
				{
					GameObject instanceObj = new GameObject("Loadmanager");
					m_instance = instanceObj.AddComponent<Loadmanager>();
					DontDestroyOnLoad(m_instance.gameObject);
				}
			}

			return m_instance;
		}
	}

	public bool activeLoading = false;		//로딩바 활성중이냐 ?

	public static AsyncOperation async = null;

	public del_NextProcess nextResourceLoadProcess;
	public static del_NextProcess nextSceneProcess;

	private static GameObject Obj_load = null;


	private Queue<IEnumerator> Que_LoadDataRoutine = new Queue<IEnumerator>();			//데이터 로딩을 위해 로딩될 코루틴데이터 큐

	// 비동기로 씬 로드 하기
	public static IEnumerator LoadScene(string SceneName, del_NextProcess nextProcess, del_NextProcess nextProcess_2, Slider progressBar)
	{
		
		async = SceneManager.LoadSceneAsync(SceneName);
		async.allowSceneActivation = false;
		//float barValue = 0;

		while (true)
		{
			

			
			if (async.isDone)
			{
				UserEditor.Getsingleton.EditLog("complete load");
				if (progressBar != null)
				{
					if (progressBar.value < 99f)
					{
						progressBar.value = 100f;
						yield return null;
					}

				}

				if (nextProcess != null)
					nextProcess();
				break;
			}
			if (!async.isDone)
			{
				if (progressBar != null)
				{
					if (progressBar.value < 60f)
						progressBar.value += 10f;
				}
				float progress = async.progress * 100.0f;

				UserEditor.Getsingleton.EditLog(string.Format(" {0}%\n", progress));


			}
			if (async.progress == 0.90f )
			{
				if (progressBar != null)
				{
					if (progressBar.value < 100f)
					{
						progressBar.value += 10f;

					}
					else if (progressBar.value >= 100f)
					{
						if (nextProcess_2 != null)
							nextProcess_2();
					}
				}
				else
				{
					if (nextProcess_2 != null)
						nextProcess_2();
				}

				



			}
			
		

			yield return null;

		}
	}








	//로딩띄우기
	public void LoadingUI(bool isNetworking)
	{
		if (isNetworking)
		{
			activeLoading = true;

			Obj_load = ObjectPoolManager.Getinstance.Get_ObjectUI(UI.LOADING.ToString()).gameObject;
			
			Scene _scene = SceneManager.GetActiveScene();
			if (_scene.name == DefineKey.Main)
			{
				Obj_load.transform.SetParent(UI_Manager.Getsingleton.parentToastPopup_Tr, false);
				ObjectPoolManager.Getinstance.SetTransformParent_ObjectUI(UI.LOADING.ToString(), UI_Manager.Getsingleton.parentToastPopup_Tr);
			}
			else
			{
				Obj_load.transform.SetParent(UI_Manager.Getsingleton.CanvasTr, false);
				ObjectPoolManager.Getinstance.SetTransformParent_ObjectUI(UI.LOADING.ToString(),UI_Manager.Getsingleton.CanvasTr);
			}
			Obj_load.transform.SetAsLastSibling();

			//User.isSelectedCharacter = true;		//캐릭회전 잠금

		}
		else
		{
			activeLoading = false;

			ObjectPoolManager.Getinstance.Release_ObjectUI(UI.LOADING.ToString());
			//Destroy(Obj_load);
			Obj_load = null;

			//현재 띄어진 팝업이 없을떄 캐릭회전 해제
			//if(UI_Manager.Getsingleton.UIPopupList.Count <= 0) 
			//User.isSelectedCharacter = false;		

		}
	}



	//리소스 데이터 로드 코루틴정보 초기화
	public void loadResourcesData()
	{
		//UI 오브젝트
		IEnumerator routine_1 = ObjectPoolManager.Getinstance.Allocate_UI_Objects();
		Que_LoadDataRoutine.Enqueue(routine_1);

		// 로비 유닛 오브젝트
		//IEnumerator routine_2 = ObjectPoolManager.Getinstance.Allocate_LobbyUnitObject();
		//Que_LoadDataRoutine.Enqueue(routine_2);

		//이미지 스프라이트
		IEnumerator routine_3 = ImageManager.instance.routine_spriteLoad();
		Que_LoadDataRoutine.Enqueue(routine_3);

		//로딩방 생성
		LoadingUI(true);

		//리소스 데이터 체크 및 로드실행
		Chk_loadResourcesData();
	}

	//리소스 데이터 체크 및 로드실행
	public void Chk_loadResourcesData()
	{
		if (Que_LoadDataRoutine.Count == 0)
		{
			LoadingUI(false);
			if (nextResourceLoadProcess != null)
				nextResourceLoadProcess();
			return;

		}

		IEnumerator nowLoadRoutine = Que_LoadDataRoutine.Dequeue();

		UserEditor.Getsingleton.EditLog("로드 코루틴 시작 : " + nowLoadRoutine.ToString());
		StartCoroutine(Load_ResourcesData(nowLoadRoutine));
	}

	//리소스 데이터 로드실행
	private IEnumerator Load_ResourcesData(IEnumerator LoadRoutine)
	{
		yield return StartCoroutine(LoadRoutine);
		UserEditor.Getsingleton.EditLog("로드 코루틴 끝 !@#");

		Chk_loadResourcesData();
	}

	
}
