using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum PROGRESS_STATE { IDEL, PANEL_OPEN, RESPAWN_PANEL_OPEN, PANEL_VIEW, PANEL_CLOSE }

public class Progress_Script : MonoBehaviour
{        
    GamePlay_Script Play_Script;
    
    public GameObject GameOver_OJ;
    public Image GameOver_Img;
    public GameObject Exit_Button;

    public GameObject BG_Img_OJ;

    public Text R_Total_Kill_Num;
    public Text R_Total_Death_Num;
    public Text B_Total_Kill_Num;
    public Text B_Total_Death_Num;

    public struct PLAYER_INFO
    {
        public Image BG_Img;
        public Image ClanMark_Img;
        public Image Country_Img;
        public Text User_Name;
        public Text User_Kill_Num;
        public Text User_Death_Num;
        
    }
    PLAYER_INFO[] Red_Team_Info = new PLAYER_INFO[6];
    PLAYER_INFO[] Blue_Team_Info = new PLAYER_INFO[6];

    public bool Data_Refresh_Check = false;
        

    void Awake()
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------

        Vector2 Size_Delta = transform.GetComponent<RectTransform>().sizeDelta;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.GetComponent<RectTransform>().sizeDelta = Size_Delta;

        Play_Script = GameObject.Find("Script_Object").GetComponent<GamePlay_Script>();
        
        //------------------------------------------------------------------------------------------------------------------------------------------------

        GameOver_OJ.SetActive(false);
        Exit_Button.SetActive(false);

        //------------------------------------------------------------------------------------------------------------------------------------------------

        Transform[] GetTransforms = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in GetTransforms)
        {
            for (int i = 0; i < Red_Team_Info.Length; i++)
            {
                if (child.name.Equals("1_" + i))
                {
                    Transform[] Child_Transforms = child.transform.GetComponentsInChildren<Transform>();
                    foreach (Transform _Child_Transforms in Child_Transforms)
                    {
                        if (_Child_Transforms.name.Equals("back"))
                        {
                            Red_Team_Info[i].BG_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("image_clanmark"))
                        {
                            Red_Team_Info[i].ClanMark_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("image_flag"))
                        {
                            //Red_Team_Info[i].Country_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("username"))
                        {
                            Red_Team_Info[i].User_Name = _Child_Transforms.GetComponent<Text>();
                        }
                        else if (_Child_Transforms.name.Equals("Text (7)"))
                        {
                            Red_Team_Info[i].User_Kill_Num = _Child_Transforms.GetComponent<Text>();
                        }
                        else if (_Child_Transforms.name.Equals("Text (8)"))
                        {
                            Red_Team_Info[i].User_Death_Num = _Child_Transforms.GetComponent<Text>();
                        }
                    }                    
                }
            }

            for (int i = 0; i < Blue_Team_Info.Length; i++)
            {
                if (child.name.Equals("2_" + i))
                {
                    Transform[] Child_Transforms = child.transform.GetComponentsInChildren<Transform>();
                    foreach (Transform _Child_Transforms in Child_Transforms)
                    {
                        if (_Child_Transforms.name.Equals("back"))
                        {
                            Blue_Team_Info[i].BG_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("image_clanmark"))
                        {
                            Blue_Team_Info[i].ClanMark_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("image_flag"))
                        {
                            //Blue_Team_Info[i].Country_Img = _Child_Transforms.GetComponent<Image>();
                        }
                        else if (_Child_Transforms.name.Equals("username"))
                        {
                            Blue_Team_Info[i].User_Name = _Child_Transforms.GetComponent<Text>();
                        }
                        else if (_Child_Transforms.name.Equals("Text (7)"))
                        {
                            Blue_Team_Info[i].User_Kill_Num = _Child_Transforms.GetComponent<Text>();
                        }
                        else if (_Child_Transforms.name.Equals("Text (8)"))
                        {
                            Blue_Team_Info[i].User_Death_Num = _Child_Transforms.GetComponent<Text>();
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------
    }

    void Update()
    {
        if (Data_Refresh_Check)
        {
            Data_Refresh_Check = false;

            Panel_Play_Init();
        }
    }

    public void Panel_Play_Init()
    {

        for (int i = 0; i < Red_Team_Info.Length; i++)
        {
            Red_Team_Info[i].BG_Img.color = new Color(1, 1, 1, 1);
            Red_Team_Info[i].BG_Img.gameObject.SetActive(false);
            Red_Team_Info[i].ClanMark_Img.gameObject.SetActive(false);
            //Red_Team_Info[i].Country_Img.gameObject.SetActive(false);
            Red_Team_Info[i].User_Name.text = "";
            Red_Team_Info[i].User_Kill_Num.text = "";
            Red_Team_Info[i].User_Death_Num.text = "";

            Blue_Team_Info[i].BG_Img.color = new Color(1, 1, 1, 1);
            Blue_Team_Info[i].BG_Img.gameObject.SetActive(false);
            Blue_Team_Info[i].ClanMark_Img.gameObject.SetActive(false);
            //Blue_Team_Info[i].Country_Img.gameObject.SetActive(false);
            Blue_Team_Info[i].User_Name.text = "";
            Blue_Team_Info[i].User_Kill_Num.text = "";
            Blue_Team_Info[i].User_Death_Num.text = "";
        }


        //----------------------------------------------------------------------------------------------------------------

        R_Total_Kill_Num.text = "" + Play_Script.R_Total_Kill;
        R_Total_Death_Num.text = "" + Play_Script.R_Total_Death;
        B_Total_Kill_Num.text = "" + Play_Script.B_Total_Kill;
        B_Total_Death_Num.text = "" + Play_Script.B_Total_Death;

        //----------------------------------------------------------------------------------------------------------------

        Progress_ArrayList.Clear();
        for (int i = 0; i < Play_Script.R_Progress_Info.Length; i++)
        {
            Progress_ArrayList.Add(Play_Script.R_Progress_Info[i]);
        }
        Progress_ArrayList.Sort(Progress_Sort);

        for (int i = 0; i < Progress_ArrayList.Count; i++)
        {
            if (((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).User_ID == 0) continue;

            Red_Team_Info[i].BG_Img.gameObject.SetActive(true);

            if (((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).User_ID == Link_Script.i.User_ID) Red_Team_Info[i].BG_Img.color = new Color(1, 1, 0, 1);


            Red_Team_Info[i].ClanMark_Img.gameObject.SetActive(true);
            //Red_Team_Info[i].Country_Img.gameObject.SetActive(true);
            Red_Team_Info[i].ClanMark_Img.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).ClanMark_Index));
            //Red_Team_Info[i].Country_Img.sprite = ImageManager.instance.Get_FlagSprite(((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Country_Index);

            Red_Team_Info[i].User_Name.text = ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).NickName;
            Red_Team_Info[i].User_Kill_Num.text = "" + ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Kill_Num;
            Red_Team_Info[i].User_Death_Num.text = "" + ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Death_Num;
        }

        //----------------------------------------------------------------------------------------------------------------

        Progress_ArrayList.Clear();
        for (int i = 0; i < Play_Script.B_Progress_Info.Length; i++)
        {
            Progress_ArrayList.Add(Play_Script.B_Progress_Info[i]);
        }
        Progress_ArrayList.Sort(Progress_Sort);

        for (int i = 0; i < Progress_ArrayList.Count; i++)
        {            
            if (((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).User_ID == 0) continue;

            Blue_Team_Info[i].BG_Img.gameObject.SetActive(true);

            if (((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).User_ID == Link_Script.i.User_ID) Blue_Team_Info[i].BG_Img.color = new Color(1, 1, 0, 1);         

            Blue_Team_Info[i].ClanMark_Img.gameObject.SetActive(true);
            //Blue_Team_Info[i].Country_Img.gameObject.SetActive(true);
            Blue_Team_Info[i].ClanMark_Img.sprite = ImageManager.instance.Get_Sprite(string.Format("{0}{1}", DefineKey.clanmark, ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).ClanMark_Index));
            //Blue_Team_Info[i].Country_Img.sprite = ImageManager.instance.Get_FlagSprite(((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Country_Index);

            Blue_Team_Info[i].User_Name.text = ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).NickName;
            Blue_Team_Info[i].User_Kill_Num.text = "" + ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Kill_Num;
            Blue_Team_Info[i].User_Death_Num.text = "" + ((GamePlay_Script.PROGRESS_PANEL_INFO)Progress_ArrayList[i]).Death_Num;
        }

        //----------------------------------------------------------------------------------------------------------------
    }

    public void Panel_GameOver_Text_View(String Result_Text)
    {
        GameOver_OJ.SetActive(true);

        GameOver_Img.sprite = ImageManager.instance.Get_Sprite(Result_Text);
        GameOver_Img.SetNativeSize();
    }

    public void Panel_GameOver_Button_View()
    {
        Exit_Button.SetActive(true);
    }

    public void Panel_View(bool Check)
    {
        this.gameObject.SetActive(Check);
    }

    public void BG_Img_OJ_View(bool Check)
    {
        BG_Img_OJ.SetActive(Check);
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------------

    class ProgressComparer : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            GamePlay_Script.PROGRESS_PANEL_INFO A_Data = (GamePlay_Script.PROGRESS_PANEL_INFO)a;
            GamePlay_Script.PROGRESS_PANEL_INFO B_Data = (GamePlay_Script.PROGRESS_PANEL_INFO)b;

            if (A_Data.User_ID == 0) return 1;

            if (B_Data.User_ID == 0) return -1;

            if (A_Data.Kill_Num == B_Data.Kill_Num)
            {
                if (A_Data.Death_Num < B_Data.Death_Num)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            else if (A_Data.Kill_Num > B_Data.Kill_Num)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

    ProgressComparer Progress_Sort = new ProgressComparer();
    ArrayList Progress_ArrayList = new ArrayList();

    //--------------------------------------------------------------------------------------------------------------------------------------------------------

}
