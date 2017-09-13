using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ProtoBuf;

public class UiWin_QJJS : MonoBehaviour {

    [SerializeField]
    private List<Text[]> all_P_UI = new List<Text[]>();
    [SerializeField]
    private List<Image> all_P_TouXiang = new List<Image>();
    private RectTransform rectT_Btn_MjEnd_Start_Game;// rectT_Btn_MjEnd_Close;
    public Button btn_RetrunHome, btn_Share_Data;
    private int i_ZiMo, i_DianPao, i_JiePao, i_MingGang, i_AnGang, i_ZongFen;
    private Text t_Time;
    void Start()
    {
        if (all_P_UI.Count==0)
        {
            Init(transform);
        }
    }
    
    public void Init(Transform window_19)
    {
        Transform bg_ = window_19.FindChild("Img_Bg0/Img_End_All_Sum");
        for (int i = 0; i < bg_.childCount; i++)
        {
            all_P_UI.Add(bg_.GetChild(i).GetComponentsInChildren<Text>());
            all_P_TouXiang.Add(bg_.GetChild(i).GetComponentInChildren<Button>().image);
        }

        New_Sum();
        //rectT_Btn_MjEnd_Start_Game = window_19.FindChild("Img_End_All_Sum").GetComponent<RectTransform>();
        //rectT_Btn_MjEnd_Close = window_19.FindChild("Btn_MjEnd_Close").GetComponent<RectTransform>(); ;
        btn_RetrunHome = transform.FindChild("Img_Bg0/Btn_RetrunHome").GetComponent<Button>();
        btn_Share_Data = transform.FindChild("Img_Bg0/Btn_Share_Data").GetComponent<Button>();

        btn_Share_Data.onClick.AddListener(delegate() {
            OnClick_Btn_Share_Data();
        });
        btn_RetrunHome.onClick.AddListener(delegate () {
            OnClick_Btn_MjEnd_Close();
        });

        t_Time = transform.FindChild("Img_Bg0/T_Time").GetComponent<Text>();
        t_Time.text = "";
        Set_Time();
    }

    public void Set_Time()
    {

        System.DateTime moment = System.DateTime.Now;
        // Year gets 1999.
        int year = moment.Year;
        // Month gets 1 (January).
        int month = moment.Month;
        // Day gets 13.
        int day = moment.Day;
        
        if (DataManage.Instance.isGameEnter_1)
        {
            string strGuiZe = DataManage.Instance.RoomInfoNxStr.Length > 0 ? "\t\t规则：" : "\t\t";
            t_Time.text = "房间号：" + DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString() + "\t\t局数：" + DataManage.Instance.roomJuShu_Max + strGuiZe + DataManage.Instance.RoomInfoNxStr.Replace("\n", "\t\t");
        }
        t_Time.text += year + "年" + month + "月" + day + "日" + "\t" + moment.Hour + ":" + moment.Minute;
        Debug.Log(year + "年" + month + "月" + day + "日" + "\t" + moment.Hour + ":" + moment.Minute);

    }

    void OnClick_Btn_MjEnd_Close()
    {
        //Model3dManage.Instance.DestroyObjectModel("MJGameController");
        //UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_InGame_MJ);
        //UIManager.Instance.DestroyObjectUI( AllPrefabName.UiWin_MjEnd_1);
        //UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_QuanJuJieSuan);
        GameManager.GM.Game_RetrueHome();
    }

    void OnClick_Btn_Share_Data()
    {
        Debug.Log("分享~");
        SdkEvent.Instance.ShareResult();
    }

    void New_Sum()
    {
        this.i_ZiMo = 0;
        this.i_DianPao = 0;
        this.i_JiePao = 0;
        this.i_MingGang = 0;
        this.i_AnGang = 0;
        this.i_ZongFen = 0;
    }
    public void Fun_ReciveAllGameOverResult(List<MJGameOver> end_All_S)
    {
        List<uint> listDYJ = new List<uint>();
        int i_DYJGold = 0;
        int i_ShuLeGold = 0;
        for (int i = 0; i < end_All_S[0].players.Count; i++)
        {
            //end_All_S[0].in
            for (int J = 0; J < end_All_S.Count; J++)
            {
                //前面打过几局
                //1.胡
                for (int z = 0; z < end_All_S[J].players[i].huInfos.Count; z++)
                {
                    if (end_All_S[J].players[i].huInfos[0].oricharId != end_All_S[J].players[i].charId && end_All_S[J].players[i].huInfos[0].oricharId != 0)//catag属性未名
                    {
                        //ItemList[""].GetComponent<Text>().text = "" +
                        i_JiePao++;

                    }
                    else
                    {
                        i_ZiMo++;

                    }
                }
                //2.放炮
                for (int z = 0; z < end_All_S[J].players[i].fpInfos.Count; z++)
                {
                    i_DianPao++;
                }
                //杠
                for (int z = 0; z < end_All_S[J].players[i].gangInfos.Count; z++)
                {
                    if (end_All_S[J].players[i].gangInfos[z].catag == 2)//catag属性未名 1//碰上加杠（明杠） 2//暗杠 3//明杠
                    {
                        //明杠
                        i_AnGang++;

                    }
                    else
                    {
                        i_MingGang++;

                    }
                }

                //i_ZongFen += end_All_S[J].players[i].restGold;//restGold属性未名
                //Debug.Log(J+"号"+RoundOverResult.befores[J].befores[i].gold);//restGold属性未名
            }
            i_ZongFen = end_All_S[end_All_S.Count - 1].players[i].restGold;
            //Player_Start_Gm_UI p_UI = Get_P_Data(end_All_S[0].players[i].charId.ToString());
            all_P_TouXiang[i].sprite = DataManage.Instance.Head_GetSprite(end_All_S[0].players[i].charId); // p_UI.p_Img_TouXiang.sprite;// Resources.Load<Sprite>("tx_1");
            //[0]名字 、[1]底分、[2]Hu、[3]ZiMo、[4]DianPao、[5]ZongFen
            all_P_UI[i][0].text = "名字：" + DataManage.Instance.PData_GetData(end_All_S[0].players[i].charId).P_Name+ /*p_UI.p_T_Name.text + */"\nID:" + end_All_S[0].players[i].charId.ToString();
            all_P_UI[i][1].text = "接炮：" + i_JiePao + "\n自摸：" + i_ZiMo + "\n点炮：" + i_DianPao;  //"底分：" + i_ZiMo.ToString();
            //all_P_UI[i][2].text = "" + i_JiePao;
            //all_P_UI[i][3].text = "" + i_ZiMo;
            //all_P_UI[i][4].text = "" + i_DianPao;
            all_P_UI[i][2].text = "<color=red><size=40>总分：" + i_ZongFen.ToString()+"</size></color>";

            //all_P_UI[i][6].text = "总分为：" + i_ZongFen;
            if (i_ZongFen !=0 && i_ZongFen >= i_DYJGold)
            {
                i_DYJGold = i_ZongFen;
            }

            if (i_ZongFen != 0 && i_ZongFen <= i_ShuLeGold)
            {
                i_ShuLeGold = i_ZongFen;
            }
            this.New_Sum();
        }

        for (int i = 0; i < all_P_UI.Count; i++)
        {
            if (all_P_UI[i][2].text.Length>0)
            {
                all_P_UI[i][0].transform.parent.FindChild("Img_DYJ").gameObject.SetActive(i_DYJGold > 0 && all_P_UI[i][2].text == i_DYJGold.ToString());
            }
        }

        for (int i = 0; i < all_P_UI.Count; i++)
        {
            if (all_P_UI[i][2].text.Length > 0)
            {
                Image imgDSJ = all_P_UI[i][0].transform.parent.FindChild("Img_DYJ").GetComponent<Image>();
                imgDSJ.gameObject.SetActive(i_ShuLeGold < 0 && all_P_UI[i][2].text == i_ShuLeGold.ToString() || i_DYJGold > 0 && all_P_UI[i][2].text == i_DYJGold.ToString());
                if (imgDSJ.gameObject.activeInHierarchy)
                {
                    imgDSJ.sprite = all_P_UI[i][2].text == i_ShuLeGold.ToString() ? Resources.Load<Sprite>(ResPath_Assets.sprite_END_ShuLe):imgDSJ.sprite;
                }
            }
        }
    }

    //public Player_Start_Gm_UI Get_P_Data(string p_ID)
    //{
    //    Player_Start_Gm_UI[] roomPlayerInfo = Gm_Manager.G_M.Start_Gm_Ui.Getall_Player_UIgm;
    //    for (int i = 0; i < roomPlayerInfo.Length; i++)
    //    {
    //        if (roomPlayerInfo[i].p_T_Id.text.IndexOf(p_ID) >= 0)
    //        {
    //            return roomPlayerInfo[i];
    //        }
    //    }
    //    return new Player_Start_Gm_UI();
    //}
}
