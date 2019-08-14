using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour 
{
	public List<ParticleSystemElement> Lst_PsElement;





	public void Particle_Play(int idx)
	{
		if (Lst_PsElement.Count > 0)
		{
			for (int i = 0; i < Lst_PsElement.Count; i++)
			{
				if (Lst_PsElement[i].gameObject.activeSelf)
				{
					// 파티클 재생
					Lst_PsElement[i].Particles[idx].Play();
				}
			}

		}
	}




	//public void Update()
	//{
	//    if (Input.GetKeyDown(KeyCode.F1))
	//    {
	//        activeObj(0);
	//    }
	//    else if (Input.GetKeyDown(KeyCode.F2))
	//    {
	//        activeObj(1);
	//    }
	//    else if (Input.GetKeyDown(KeyCode.F3))
	//    {
	//        activeObj(2);
	//    }
	//    else if (Input.GetKeyDown(KeyCode.F4))
	//    {
	//        activeObj(3);
	//    }
	//}

	void activeObj(int idx)
	{
		for (int i = 0; i < Lst_PsElement.Count; i++)
			Lst_PsElement[i].gameObject.SetActive(i == idx);
	}
}
