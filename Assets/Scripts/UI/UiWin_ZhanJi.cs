using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using ProtoBuf;
using System;

[System.Serializable]
public class UiWin_ZhanJi : UiWin_Parent
{
    ProtoBuf.QueryInfoRsp rueryInfo_;


    struct ZhanJi_1Layer_Num
    {
        public int i_Index_Max;
        public int i_PageC_;
        public int i_PageMax;
        public ZhanJi_1Layer_Num(int i_Max)
        {
            i_Index_Max = i_Max;
            i_PageC_ = 1;
            i_PageMax = ((i_Max / 5) + ((i_Max % 5) > 0 ? 1 : 0));
        }
    }
    [SerializeField]
    private ZJ13Z_Result zhanJi_13Z;//13张单局结算界面查看

    [SerializeField]
    private ZhanJi_DataUI zhanJi_1L_DataUI;//战绩第一层级
    [SerializeField]
    private ZhanJi_DataUI zhanJi_2L_DataUI;//战绩第二层级

    private string zj1Layer_Time;
    private int zhanJi_1_Data_Index;///用来记录战绩第一层，翻到了第几页
    private const int zhanJi_1_Max = 5;//第一层最大战绩数
    [SerializeField]
    private Transform window06_T;

    private RectTransform img_All_Bg;//作用于1层级 与2层级的切换
    private GameObject btn_Zj1L_Select_Bg_0;//用作模板   
    private ScrollRect sRect_Bg;//用来滑动战绩第二层级
    //private Text t_Zj_Page_;
    //public static Transform layout_YinCang_AllZhanJi;//用来隐藏战绩,  
    private int i_Zj_Id;
    private ZhanJi_1Layer_Num zj_Index;
    private int layer_Value;
    private int i_C_1Index;

    [SerializeField]
    public string zj_Data;
    public const int zhanJi_MAX = 16;
    //public List<ProtoBuf.EachRoundPlayer> players_;

    public int I_Zj_Id
    {
        get
        {
            return i_Zj_Id;
        }
    }

    void Start()
    {
        layer_Value = 0;
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());
        Init_Var(transform);
        //OpenAllZhanJi_Ui(false);
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        //base.Set_OnEventList<Component_>(all_);
        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                UnityEngine.Events.UnityAction btnOnClck_ = null;
                switch (btn_.gameObject.name)
                {
                    case "Btn_CloseWin":
                        btnOnClck_ = delegate () {
                            Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                            if (layer_Value == 2)
                            {//如果在战绩第2层，则返回第1层。
                                Set_ImgAllBg_RectTransform(1);
                            }
                            else
                            {//如果在战绩第1层，关闭战绩（隐藏显示）
                                transform.SetAsFirstSibling();
                                UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_ZhanJi);
                            }
                        };
                        break;
                    default:
                        break;
                }
                if (btnOnClck_ != null)
                {
                    btn_.onClick.AddListener(delegate ()
                    {
                        Debug.Log("Click Btn:" + btn_.gameObject.name);
                        btnOnClck_.Invoke();

                    });
                }
            }
        }

    }

    void Set_rueryInfo_(QueryInfoRsp value_)
    {
        this.rueryInfo_ = value_;
        //zj_Index = new ZhanJi_1Layer_Num(rueryInfo_.records.Count);
        Debug.Log("接收到了战绩，战绩数量："+value_.records.Count);
        SetZhanJi_1Layer();
    }

    public void Init_Var(Transform window06)
    {
        if (PublicEvent.GetINS.Fun_ReciveZhanJiHuiFang == null)
        {
            PublicEvent.GetINS.Fun_ReciveZhanJiHuiFang = Set_rueryInfo_;
        }

        window06_T = window06;
        //img_All_Bg = window06_T.FindChild("Img_All_Bg").GetComponent<RectTransform>();
        sRect_Bg = window06.FindChild("Img_Bg0/SV_Bg0").GetComponent<ScrollRect>();
        zhanJi_1L_DataUI = new ZhanJi_DataUI(sRect_Bg.content.GetChild(0));
        //img_Layout_Bg_RectT
        zhanJi_2L_DataUI = new ZhanJi_DataUI(sRect_Bg.content.GetChild(1));
        
        isInitVar = true;
    }

    public void Update_Zj_1L(bool upOrDown)
    {
        if (upOrDown)
        {

            zhanJi_1_Data_Index++;
        }
        else
        {

            zhanJi_1_Data_Index--;
        }
    }


    private void Set_ImgAllBg_RectTransform(int i_index)
    {
        sRect_Bg.content.GetChild(0).gameObject.SetActive(i_index == 1);
        sRect_Bg.content.GetChild(1).gameObject.SetActive(i_index == 2);
        Debug.Log("执行？"+i_index);
        layer_Value = i_index;
         //   img_All_Bg.anchoredPosition = new Vector2(rectT_PointX, img_All_Bg.anchoredPosition.y);
    }

    #region /* —————————战绩界面按钮事件———————————————*/
    public void OnClick_Btn_Zj1L_Select_Bg(GameObject btn_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        Debug.Log("选择了战绩界面 第一层级 的 " + btn_.gameObject.name + "___" + btn_.transform.GetSiblingIndex());
        //1、先设置好第二层级的数据， 
        OpenAll_ZhanJi2(false);
        int i_ = 0;
        try
        {
            i_ = int.Parse(btn_.name.Replace("ZJ1Layer", ""));
        }
        catch (Exception e)
        {
            Debug.LogError("int.Parse Error!");
            throw;
        }
        //zj1Layer_Time = btn_.transform.FindChild("Img_Layout/T_RoomNumber").GetComponent<Text>().text;
        SetZhanJi_2Layer(i_);// btn_.transform.GetSiblingIndex());
        //2、跳转到第二层级。
        this.Set_ImgAllBg_RectTransform(2);
    }

    public void OnClick__Zj_2Layer_Return()
    {
        this.Set_ImgAllBg_RectTransform(1);
    }


    public void OpenAllZhanJi_Ui(bool b_)
    {
        for (int i = 0; i < zhanJi_1L_DataUI.t_Layout_T.childCount/*zhanJi_All_Text.Count*/; i++)
        {
            //zhanJi_1L_DataUI.zhanJi_All_Text[i][0].GetComponentInParent<LayoutElement>().gameObject.SetActive(b_);   //.transform.parent.gameObject.SetActive(b_);
            zhanJi_1L_DataUI.t_Layout_T.GetChild(i).gameObject.SetActive(b_);
        }

        for (int i = 0; i < zhanJi_2L_DataUI.t_Layout_T.childCount/*zhanJi_All_Text.Count*/; i++)
        {
            //zhanJi_2L_DataUI.zhanJi_All_Text[i][0].GetComponentInParent<LayoutElement>().gameObject.SetActive(b_);   //.transform.parent.gameObject.SetActive(b_);
            zhanJi_2L_DataUI.t_Layout_T.GetChild(i).gameObject.SetActive(b_);
        }
    }

    void OpenAll_ZhanJi2(bool b_)
    {
        for (int i = 0; i < zhanJi_2L_DataUI.t_Layout_T.childCount; i++)
        {
            zhanJi_2L_DataUI.t_Layout_T.GetChild(i).gameObject.SetActive(b_);
            //zhanJi_2L_DataUI.zhanJi_All_Text[i][0].GetComponentInParent<LayoutElement>().gameObject.SetActive(b_);   //.transform.parent.parent.gameObject.SetActive(b_);
        }
    }
    public void Btn_Zj1L_Up_OnClick()
    {
        //1、先设置好战绩。
        Debug.Log("UpPack");
        if (zj_Index.i_PageMax > 0)
        {
            zj_Index.i_PageC_--;
            zj_Index.i_PageC_ = Mathf.Clamp(zj_Index.i_PageC_, 1, zj_Index.i_PageMax);
            OpenAllZhanJi_Ui(false);
            SetZhanJi_1Layer();
        }
    }
    public void Btn_Zj1L_Down_OnClick()
    {
        Debug.Log("DownPack");
        if (zj_Index.i_PageMax > 0)
        {
            zj_Index.i_PageC_++;
            zj_Index.i_PageC_ = Mathf.Clamp(zj_Index.i_PageC_, 1, zj_Index.i_PageMax);
            OpenAllZhanJi_Ui(false);
            SetZhanJi_1Layer();
        }
    }

    /// <summary>【拖拽赋值】 战绩分类按钮
    /// </summary>
    public void OnClick_Btn_ZJClassify(int tIndex)
    {
        ProtoBuf.GameType gameType = GameType.GT_NULL;
        switch (tIndex)
        {
            case 11:
                //13张
                gameType = GameType.GT_THIRt;
                break;
            case 21:
                //兴文麻将
                gameType = GameType.GT_MJ;
                break;
            case 31:
                //兴文大贰
                gameType = GameType.GT_DE;
                break;
            default:

                return;
        }
        Debug.Log(""+gameType);
        ZhanJi_Info[] zjInfoAry = zhanJi_1L_DataUI.t_Layout_T.GetComponentsInChildren<ZhanJi_Info>(true);
        for (int i = 0; i < zjInfoAry.Length; i++)
        {
            zjInfoAry[i].gameObject.SetActive(zjInfoAry[i].gameType == gameType);
        }

    }

    public void OnClick_Btn_Zj2L_Play_HuiFang(GameObject btn_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        i_Zj_Id = int.Parse(btn_.name.Replace("ZJ2Layer",""));
        Debug.Log("点击了 回放按钮" + I_Zj_Id.ToString());

        if (rueryInfo_.records[i_C_1Index].gameType == GameType.GT_MJ)
        {
            ZJHF_Nx3D zjhF_Mj = Model3dManage.Instance.ShowModel("MJ_ZJHF", null).GetComponent<ZJHF_Nx3D>();
            zjhF_Mj.Init_();
            zjhF_Mj.mjRoundR_ = rueryInfo_.records[i_C_1Index].rounds[i_Zj_Id];

            UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_Home);
            UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_ZhanJi);
        }
        else if (rueryInfo_.records[i_C_1Index].gameType == GameType.GT_THIRt)
        {
            Debug.Log("战绩 十三水");
            //rueryInfo_.records[i_C_1Index].rounds[i_Zj_Id];
            zhanJi_13Z.gameObject.SetActive(true);
            zhanJi_13Z.Init_ShowZJ(rueryInfo_.records[i_C_1Index].rounds[i_Zj_Id]);
            zhanJi_13Z.transform.SetAsLastSibling();
        }

    }

    #endregion

    /// <summary>设置战绩大局 第一层
    /// </summary>
    void SetZhanJi_1Layer()
    {
        Debug.Log("执行设置第一层战绩了吗？？？AAA");
        //t_Zj_Page_.text = (zj_Index.i_PageC_) + "/" + zj_Index.i_PageMax;
        int i_Sum_ = 0;
        string strGold = "";
        for (int i = rueryInfo_.records.Count - 1; i >=0; i--)
        {
            Debug.Log("执行设置第一层战绩了吗？？？BBB");
            zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.gameObject.SetActive(true);

            //给战绩第一层加一个 GameType  作用于分类
            if (zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.GetComponent<ZhanJi_Info>()==null)
            {
                zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.gameObject.AddComponent<ZhanJi_Info>();
            }
            ZhanJi_Info zjINFO = zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.GetComponent<ZhanJi_Info>();
            zjINFO.gameType = rueryInfo_.records[i].gameType;

            zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].text = (i + 1).ToString();
            
            Button btn_ZJ1 = zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.GetComponentInChildren<Button>();
            btn_ZJ1.gameObject.name = "ZJ1Layer" + i;
            btn_ZJ1.onClick.RemoveAllListeners();
            btn_ZJ1.onClick.AddListener(delegate () {
                OnClick_Btn_Zj1L_Select_Bg(btn_ZJ1.gameObject);
            });
            //房号
            //    st[1].text = "房间号:" + RRR.mjRecords[i].roomId;
            //zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][1].text = "房间号:" + rueryInfo_.records[i].roomId.ToString();
            zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][0].transform.parent.parent.gameObject.name = "Btn_Zj1L_Select_Bg_" + (i);
            //    //时间 
            UInt32 timeLong = (UInt32)rueryInfo_.records[i].rounds[rueryInfo_.records[i].rounds.Count - 1].roundOverTime;
            
            string time = timeLong.ToString();

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000000");

            TimeSpan toNow = new TimeSpan(lTime);
            var TimeOut = dtStart.Add(toNow);
            zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][1].text = GameTypeToString(rueryInfo_.records[i].gameType) + "\n房间号:" + rueryInfo_.records[i].roomId.ToString()+"\n"+ TimeOut;

            //    //玩家战绩
            zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][2].text = "";
            for (int j = 0; j < rueryInfo_.records[i].rounds[0].players.Count; j++)
            {
                int xxxx = 0;
                for (int z = 0; z < rueryInfo_.records[i].rounds.Count; z++)
                {

                    xxxx += rueryInfo_.records[i].rounds[z].players[j].gold;

                }
                strGold = xxxx<0? "<color=brown>"+xxxx+"</color>" : "<color=yellow>+" + xxxx + "</color>";

                //st[j + 3].text = RRR.mjRecords[i].rounds[0].players[j].name + ":       " + xxxx;"<color=#00000000>占位符</color>"
                zhanJi_1L_DataUI.zhanJi_All_Text[i_Sum_][/*j + 3*/2].text += rueryInfo_.records[i].rounds[0].players[j].name.ToName() + ": <size=35>" + strGold + "</size><color=#00000000>占位占位</color>";
            }
            i_Sum_++;
        }
    }

    /// <summary>设置战绩小局—》大局点击后的所属小局 第二层
    /// </summary>
    void SetZhanJi_2Layer(int i)
    {
        i_C_1Index = i;
        for (int j = 0; j < rueryInfo_.records[i].rounds.Count; j++)
        {
            //小局组块[j].SetActive(true);
            //Text[] str = 小局组块[j].GetComponentsInChildren<Text>();
            //str[0].text = "" + j;
            zhanJi_2L_DataUI.zhanJi_All_Text[j][0].transform.parent.parent.gameObject.SetActive(true);
            zhanJi_2L_DataUI.zhanJi_All_Text[j][0].text = j.ToString();
            zhanJi_2L_DataUI.zhanJi_All_Text[j][0].transform.parent.parent.gameObject.name += j;
            zhanJi_2L_DataUI.zhanJi_All_Text[j][0].transform.parent.gameObject.SetActive(true);
            Button btn_ZJ1 = zhanJi_2L_DataUI.zhanJi_All_Text[j][0].transform.parent.GetComponentInChildren<Button>();
            btn_ZJ1.gameObject.name = "ZJ2Layer"+ j;
            btn_ZJ1.onClick.RemoveAllListeners();
            btn_ZJ1.onClick.AddListener(delegate() {
                //Debug.Log("点击了战绩第二次播放");
                OnClick_Btn_Zj2L_Play_HuiFang(btn_ZJ1.gameObject);
            });
            //时间 
            UInt32 timeLong = (UInt32)rueryInfo_.records[i].rounds[rueryInfo_.records[i].rounds.Count - 1].roundOverTime;
            string time = timeLong.ToString();
            
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000000");

            TimeSpan toNow = new TimeSpan(lTime);
            var TimeOut = dtStart.Add(toNow);

            time = "" + TimeOut;
            time = time.Remove(0, 10);
            //str[1].text = "" + time;
            zhanJi_2L_DataUI.zhanJi_All_Text[j][1].text = time;
            //zhanJi_2L_DataUI.zhanJi_All_Text[j][2].text = zj1Layer_Time;
            zhanJi_2L_DataUI.zhanJi_All_Text[j][2].text = "";
            for (int k = 0; k < rueryInfo_.records[i].rounds[0].players.Count; k++)
            {
                //str[k + 2].text = RRR.mjRecords[i].rounds[j].players[k].gold + "";
                zhanJi_2L_DataUI.zhanJi_All_Text[j][2].text += rueryInfo_.records[i].rounds[j].players[k].name.ToName() + ": <color=red>" + rueryInfo_.records[i].rounds[j].players[k].gold + "</color><color=#00000000>占位占位</color>";// rueryInfo_.records[i].rounds[j].players[k].gold.ToString();
                //+= rueryInfo_.records[i].rounds[j].players[k].name.ToName() + ": " + xxxx+ "<color=#00000000>占位符占位符</color>";
            }

        }

        zj1Layer_Time = "";

        

    }

    string GameTypeToString(ProtoBuf.GameType gmType)
    {
        switch (gmType)
        {
            case GameType.GT_NULL:
                break;
            case GameType.GT_GC:
                break;
            case GameType.GT_MJ:
                return "【兴文麻将】";
                break;
            case GameType.GT_DE:
                break;
            case GameType.GT_XWMJ:
                return "【兴文麻将】";
                break;
            case GameType.GT_THIRt:
                return "【13水】";
                break;
            default:
                break;
        }
        return "";
    }

    [System.Serializable]
    private class ZhanJi_DataUI
    {
        /// <summary>战绩界面 数据UI
        /// </summary>
        public List<Text[]> zhanJi_All_Text;

        public Transform t_Layout_T;


        public ZhanJi_DataUI(Transform layout_T)
        {
            zhanJi_All_Text = new List<Text[]>();
            //List<Transform> t_Select_List = new List<Transform>();
            t_Layout_T = layout_T;
            //Image[] all_Img = layout_T.GetComponentsInChildren<Image>();
            for (int i = 0; i < layout_T.childCount; i++)
            {
                zhanJi_All_Text.Add(layout_T.GetChild(i).GetComponentsInChildren<Text>());
                zhanJi_All_Text[zhanJi_All_Text.Count - 1][0].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    
    public class ZhanJi_Info : MonoBehaviour
    {
        public ProtoBuf.GameType gameType = GameType.GT_NULL;
    }
}




