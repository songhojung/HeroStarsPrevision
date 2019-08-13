using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum OPTION_STATE { IDEL, PANEL_OPEN, PANEL_VIEW, PANEL_CLOSE }

public class Option_Script : MonoBehaviour
{
    GamePlay_Script Play_Script;

    public Toggle Auto_ATK;
    public Toggle Hand_ATK;
    public Slider Sensitive_Speed;
    public Slider Sensitive_Zoom_Speed;
    public Slider BGM_Volum;
    public Slider EFF_Volum;

    OptionSetting Setting = null;

    void Awake()
    {
        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;
        transform.SetAsLastSibling();//가장 나중에 그려진 레이어로 맞추기

        Play_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();
    }

    public void Option_View(bool View_Check)
    {
        if (View_Check)
        {
            this.gameObject.SetActive(View_Check);

            Setting = SendManager.Instance.Get_OptionSettingInfo();

            if (Setting.AttackType)
            {
                Auto_ATK.isOn = true;
                Hand_ATK.isOn = false;
            }
            else
            {
                Auto_ATK.isOn = false;
                Hand_ATK.isOn = true;
            }

            Sensitive_Speed.value = Setting.Sensitive * 0.01f;
            Sensitive_Zoom_Speed.value = Setting.SensitiveZoomIn * 0.01f;
            BGM_Volum.value = Setting.VolumBGM * 0.01f;
            EFF_Volum.value = Setting.VolumEffect * 0.01f;
        }
        else
        {
            Play_Script.Option_Auto_Shot = Setting.AttackType;
            Play_Script.Option_Sensitive = Setting.Sensitive * 0.01f;

            if (Auto_ATK.isOn)
            {
                Setting.AttackType = true;
            }
            else
            {
                Setting.AttackType = false;
            }

            Setting.Sensitive = (int)(Sensitive_Speed.value * 100);
            Setting.SensitiveZoomIn = (int)(Sensitive_Zoom_Speed.value * 100);
            Setting.VolumBGM = (int)(BGM_Volum.value * 100);
            Setting.VolumEffect = (int)(EFF_Volum.value * 100);
                        
            //저장하기
            if (Setting != null) SendManager.Instance.Save_OptionSettingInfo(Setting);

            this.gameObject.SetActive(View_Check);
        }
    }

    //효과음 슬라이드 터치 이벤트시 호출
    public void Sound_Change_Event()
    {
        Setting.VolumEffect = (int)(EFF_Volum.value * 100);
    }
}
