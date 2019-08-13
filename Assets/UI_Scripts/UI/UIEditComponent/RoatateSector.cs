using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoatateSector : MonoBehaviour
{

    private Transform RotateTargetTr;

    private Vector3 beginPointPos = new Vector3();

    private float initRotatorYdegree = 0f;
    private float bfDis = 0;
    private float Dis = 0;
    private float nowDis = 0;



    public void Init_RotateSector(Transform targetTr)
    {
        RotateTargetTr = targetTr;

        initRotatorYdegree = RotateTargetTr.eulerAngles.y;
    }






    public void OnRotateDragBegin()
    {


        beginPointPos = Input.mousePosition;




    }


    public void OnRotateDrag()
    {



        Vector3 endPos = Input.mousePosition;


        nowDis = (endPos.x - beginPointPos.x) / 5f;
        Dis = (bfDis - nowDis);

        if (RotateTargetTr != null)
            RotateTargetTr.rotation = Quaternion.Euler(0, initRotatorYdegree + Dis, 0);


    }
    public void OnRotateDragEnd()
    {
        
                bfDis = Dis;

    }


    public void OnRotatePointDown()
    {
        if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.CHARACTERSETTING))
            UI_CharacterSetting.Getsingleton.viewCharacter.Do_toughBehavior();
        if (UI_Manager.Getsingleton.Dic_UILst.ContainsKey(UI.EQUIPMENT))
            UI_Equipment.Getsingleton.equipCharacter.Do_toughBehavior();
    }


    public void Change_InitYDegree(float ydegree)
    {
        initRotatorYdegree = ydegree;
    }

    public void Clear_Rotate()
    {
        bfDis = 0;
        Dis = 0;
        nowDis = 0;
    }
}
