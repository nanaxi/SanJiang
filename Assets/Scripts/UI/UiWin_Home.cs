using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiWin_Home : UiWin_Parent
{
    public GameObject gmSEMeiZi;
    public UiWin_HomePlayerUI homePlayerUI;//Home玩家数据
    GameObject gm_CameraMeiZi;//妹子摄像机
    public Text t_SystemNotice;//拖拽赋值
    private bool isInit = false;

    // Use this for initialization
    void Start()
    {
        homePlayerUI = homePlayerUI == null ? GetComponentInChildren<UiWin_HomePlayerUI>() : homePlayerUI;
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());
        //t_SystemNotice = GetComponentInChildren<RectT_Marquee>().GetComponent<Text>();
        Init_();
        gm_CameraMeiZi = GameObject.Find("Camera_MeiZi");
           
    }

    void OnEnable()
    {
        if (gm_CameraMeiZi == null)
        {
            gm_CameraMeiZi = GameObject.Find("Camera_MeiZi");
        }
        Init_();//
    }

    void OnDisable()
    {
        
        if (gm_CameraMeiZi != null)
        {
            gm_CameraMeiZi.transform.FindChild("Canvas_MeiZi").gameObject.SetActive(false);//妹子关闭显示
            gmSEMeiZi.SetActive(false);
        }
    }

    public void SetHome_Diamond()
    {
        if (homePlayerUI!=null)
        {
            homePlayerUI.SetPlayerGold();
        }
    }

    void Init_()
    {
        Debug.Log("执行了1次？？？");

        if (homePlayerUI != null)
        {
            if (DataManage.Instance.MyPlayer_Data.onChangeDia == null)
            {
                DataManage.Instance.MyPlayer_Data.onChangeDia += homePlayerUI.SetPlayerGold;
            }
            homePlayerUI.Set_PlayerInfoUI(DataManage.Instance.MyPlayer_Data);
            StartCoroutine(DataManage.Instance.W3_Request_Tx(BaseProto.playerInfo.m_id, GlobalSettings.avatarUrl, SetHead, 1));
        }

        GameObject gm_CameraMeiZi = GameObject.Find("Camera_MeiZi");
        gmSEMeiZi = gm_CameraMeiZi.transform.FindChild("SE_MeiZi").gameObject;
        if (gm_CameraMeiZi != null && gm_CameraMeiZi.transform.childCount > 0)
        {
            gm_CameraMeiZi.transform.FindChild("Canvas_MeiZi").gameObject.SetActive(true);
            gmSEMeiZi.SetActive(true);
        }

        t_SystemNotice.text = "请设置公告！请设置公告！请设置公告！请设置公告！请设置公告！";// DataManage.Instance.StrSystemNotice;


        //防止重连后的UI层级问题
        if (UIManager.Instance.FindUI( AllPrefabName.uiWin_ZhanJi)!=null && UIManager.Instance.FindUI(AllPrefabName.uiWin_ZhanJi).activeInHierarchy)
        {
            UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_ZhanJi); 
        }

        if (UIManager.Instance.FindUI(AllPrefabName.uiWin_Setting) != null && UIManager.Instance.FindUI(AllPrefabName.uiWin_Setting).activeInHierarchy)
        {
            UIManager.Instance.FindUI(AllPrefabName.uiWin_Setting).transform.SetAsLastSibling();
        }

        if (UIManager.Instance.FindUI(AllPrefabName.uiWin_CreateRoom) != null && UIManager.Instance.FindUI(AllPrefabName.uiWin_CreateRoom).activeInHierarchy)
        {
            UIManager.Instance.FindUI(AllPrefabName.uiWin_CreateRoom).transform.SetAsLastSibling();
        }

        if (UIManager.Instance.FindUI(AllPrefabName.uiWin_JoinRoom) != null && UIManager.Instance.FindUI(AllPrefabName.uiWin_JoinRoom).activeInHierarchy)
        {
            UIManager.Instance.FindUI(AllPrefabName.uiWin_JoinRoom).transform.SetAsLastSibling();
        }

        if (UIManager.Instance.FindUI(AllPrefabName.uiWin_Prompt_QuitGame) != null && UIManager.Instance.FindUI(AllPrefabName.uiWin_Prompt_QuitGame).activeInHierarchy)
        {
            UIManager.Instance.FindUI(AllPrefabName.uiWin_Prompt_QuitGame).transform.SetAsLastSibling();
        }
    }

    void SetHead(Sprite sprite_, int i_Index)
    {
        homePlayerUI.btnHead.image.sprite = sprite_;
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
                    case "Btn_OpenPlayerInfo":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("查看玩家信息");
                            UiWin_PlayerInfo looKInfo = UIManager.Instance.ShowUI(AllPrefabName.uiWin_PlayerInfo, UIManager.Instance.canvas_T).GetComponent<UiWin_PlayerInfo>();
                            looKInfo.Set_PlayerInfoUI(DataManage.Instance.MyPlayer_Data);
                        };
                        break;
                    case "Btn_Share":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("分享");
                            //StartCoroutine(BtnClick_Anima(btn_.image.rectTransform));
                            UIManager.Instance.ShowUI(AllPrefabName.UiWin_ShareUI, UIManager.Instance.canvas_T);
                            //SdkEvent.Instance.OnClick_Home_GameYaoQing();
                        };
                        break;
                    case "Btn_OpenCerateRoom":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("打开创建房间");
                            UIManager.Instance.ShowUI(AllPrefabName.uiWin_CreateRoom, UIManager.Instance.canvas_T);
                        };
                        break;
                    case "Btn_OpenJoinRoom":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("打开加入房间");
                            UIManager.Instance.ShowUI(AllPrefabName.uiWin_JoinRoom, UIManager.Instance.canvas_T);
                        };
                        break;
                    case "Btn_OpenSetting":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("打开设置");
                            UIManager.Instance.ShowUI(AllPrefabName.uiWin_Setting, UIManager.Instance.canvas_T);
                        };
                        break;
                    case "Btn_OpenZhanJi":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("打开战绩");
                            //StartCoroutine(BtnClick_Anima(btn_.image.rectTransform));
                            UIManager.Instance.ShowUI(AllPrefabName.uiWin_ZhanJi, UIManager.Instance.canvas_T);
                            PublicEvent.GetINS.ZhanjiHuiFangRequst();
                        };
                        break;
                    case "Btn_OpenQuit":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("打开退出游戏提示");
                            //StartCoroutine(BtnClick_Anima(btn_.image.rectTransform));
                            UIManager.Instance.ShowUI(AllPrefabName.uiWin_Prompt_QuitGame, UIManager.Instance.canvas_T);
                        };
                        break;
                    case "Btn_OpenBuyPrompt":
                        btnOnClck_ = delegate ()
                        {
                            OnClick_Btn_OpenBuyPrompt();
                        };
                        break;
                        
                        break;

                    case "Btn_UpdateDia":
                        btnOnClck_ = delegate ()
                        {
                            Debug.Log("刷新~");
                            btn_.interactable = false;
                            PublicEvent.GetINS.DiamondRequst();
                            StartCoroutine(InvekeR_(60, delegate () {
                                btn_.interactable = true;
                            }));
                        };
                        break;
                    default:
                        break;
                }

                btn_.onClick.AddListener(delegate ()
                {
                    Debug.Log("Click Btn:" + btn_.gameObject.name);
                    Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);

                    if (btnOnClck_ != null)
                    {
                        btnOnClck_.Invoke();
                    }
                });
            }
        }
    }

    public void OnClick_Btn_OpenBuyPrompt()
    {
        Debug.Log("打开购买联系方式提示");
        GameObject gmPrompt1 = UIManager.Instance.ShowUI(AllPrefabName.uiWin_BuyDiamondPrompt, UIManager.Instance.canvas_T);
        gmPrompt1.GetComponentInChildren<Text>().text = "充值提示！充值提示！充值提示！请联系XXX";//DataManage.Instance.StrSystemBuyDiaMsg;
        gmPrompt1.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
        {
            Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_BuyDiamondPrompt);
        });
    }

    IEnumerator InvekeR_(float time_, UnityAction stay_Event)
    {
        yield return new WaitForSeconds(time_);
        stay_Event.Invoke();
        yield return null;
    }

    private void OnClick_(GameObject btn_)
    {
        Debug.Log("Click Btn:" + btn_.name);
        if (btn_.gameObject.name.IndexOf("Btn_Number_") >= 0)
        {

        }
        else if (btn_.gameObject.name.IndexOf("Btn_CloseWin") >= 0)
        {
        }
    }

    IEnumerator BtnClick_Anima(RectTransform rect_)
    {
        Vector2 v2_Jl = rect_.anchoredPosition3D;
        Vector2 v2_J2 = rect_.anchoredPosition3D;
        float f_AnimaTime = 0.1f;
        bool b_LeftOrRight = true;

        while (f_AnimaTime > 0.0f)
        {
            v2_Jl.x = v2_Jl.x + (b_LeftOrRight ? 16 : -16);
            rect_.anchoredPosition3D = v2_Jl;
            f_AnimaTime -= 0.02f;
            b_LeftOrRight = !b_LeftOrRight;
            yield return new WaitForSeconds(0.02f);
        }
        rect_.anchoredPosition3D = v2_J2;
        yield return null;
    }
    
}

public struct AudioPath
{
    public const string sexMan = "AudioClips/Man/";
    public const string sexWoMan = "AudioClips/WoMan/";

    public const string sexMan_GM13Z = "AudioClips/Audio13Z/Man/";
    public const string sexWoMan_GM13Z = "AudioClips/Audio13Z/WoMan/";
    public const string cardType = "CardType/";

    public const string btn_Click1 = "AudioClips/Button_Click";
}

public enum AudioPathOp_TYPE
{
    Op_AnGang,Op_DuiDuiHu, Op_Gang, Op_GangKai, Op_Hu, Op_Peng, Op_ZiKou
}

/// <summary>声音路径名称。——》13水
/// </summary>
public enum AudioPath_13Z
{
    BaoZi, BaoZiLiuDuiBan, BzZdLiuDuiBan, DaQiang, DuiZi,
    HuLu, KaiShiBiPai, LiangDui, LiuDuiBan, QuanDa, QuanHei,
    QuanHong, QuanLeiDa, QuanXiao, SanFenTianXia, SanShunZi,
    SanTiao, SanTongHua, ShuangZhaLiuDuiBan, SiTaoSanTiao,
    WuLong, YiTiaoLong, YiTiaoQingLong, ZhaDan, ZhaDanLiuDuiBan,
    ZhongDunHuLu, ZhongDunTongHuaShun, ZhongDunZhaDan, ZhongYuan1DHei, ZhongYuan1DHong
}
