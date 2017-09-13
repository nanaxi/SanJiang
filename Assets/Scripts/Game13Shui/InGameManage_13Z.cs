using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class InGameManage_13Z : MonoBehaviour
{
    /// <summary>缓存池——>扑克池，装扑克
    /// </summary>
    public PublicPathPool poolPoker;
    public Button
        btn_PType_DuiZi,
        btn_PType_ShunZi,
        btn_PType_2Dui,
        btn_PType_3Tiao,
        btn_PType_TongHua,
        btn_PType_HuLu,
        btn_PType_ZhaDan,
        btn_PType_THS,
        btn_PType_TSP,

        btn_ClearALL,
        btn_ConfirmBP
        ;

    /// <summary>本地玩家手牌Layout
    /// </summary>
    public Transform locaHandPoker_Parent;
    

    /// <summary>倒计时
    /// </summary>
    public Countdown countDownTime;
    /// <summary>摆牌 父物体
    /// </summary>
    public GameObject bgBaiPaiParent;

    /// <summary>标题，显示开始比牌
    /// </summary>
    public GameObject title_StartBiPai;

    [SerializeField]
    private RectTransform fangZhuStyle;

    /// <summary>中间的UI元素父物体背景——
    /// 1、准备按钮。2、邀请按钮
    /// </summary>
    public GameObject center_Bg0;

    [SerializeField]
    private PokerFaPaiAnimator animaFaPai;

    public UiWin_PlayerInfoX1 lookPlayerInfoX1;//查看玩家信息界面
    public Transform vLayout_MaPai;//用来摆放马牌的位置
    public RectTransform maPai_Tag;//马牌标签，如果某玩家的牌是码牌，就给他一个马牌标签
    public GameObject bpLocaChangeGold;//比牌的时候显示本地分数
    [SerializeField]private Text[] bpLocaTexts;

    public PhizAnima phizAnima_M;//关于表情的播放
    /// <summary>单局结算
    /// </summary>
    [SerializeField]
    private END13Z_1Ju end13Z_1Ju;
    
    /// <summary>是否开启单局结算
    /// </summary>
    public bool isEnd1Show {
        get { return end13Z_1Ju.gameObject.activeInHierarchy; }
    }

    /// <summary>全局结算
    /// </summary>
    [SerializeField]
    private END13Z_ALL end13Z_ALL;

    /// <summary>是否开启单局结算
    /// </summary>
    public bool isEndAllShow
    {
        get { return end13Z_ALL.gameObject.activeInHierarchy; }
    }

    /// <summary>代表一个玩家，进行游戏
    /// </summary>
    public InGame_Player13Z[] player13ZAry;

    /// <summary>显示玩家信息  的UI
    /// </summary>
    public INGamePlayer_Ui[] allPlayerInfoUis;

    /// <summary>摆牌中：三道GLayout Parent
    /// </summary>
    public Transform[] baiPai_ParentAry;

    /// <summary>本地手牌
    /// </summary>
    public PokerInfos[] locaHandPokerAry;

    /// <summary>装载扑克分类
    /// </summary>
    public Poker13TypeList pokerFenLei = new Poker13TypeList();
    //public List<Poker_X> allPoker = new List<Poker_X>();

    /// <summary>当前分类选中的扑克
    /// </summary>
    public GameObject[] listCCAry;// = new List<GameObject>();

    public List<Poker_X> listTestBaiHao = new List<Poker_X>();


    [SerializeField]
    private int iBiPai_Index;
    /// <summary>是否提示过同一IP
    /// </summary>
    private bool isPromptIP = false;

    /// <summary>是否鼠标正按下
    /// </summary>
    private bool isMouseDown = false;
    /// <summary>是否设置网络接收
    /// </summary>
    private bool isSetNet = false;
    /// <summary>是否重新连接
    /// </summary>
    private bool isRelink = false;

    /// <summary>是否投票成功
    /// </summary>
    public bool isTPCG = false;

    [SerializeField]
    private InGameRuleInfo ruleInfoUI = new InGameRuleInfo();

    public GameData_13Z gm13ZData = new GameData_13Z();
    // Use this for initialization
    void Awake()
    {
        //for (uint i = 1; i < 62; i++)
        //{
        //    allPoker.Add(i.ToPoker_X());
        //}
        if (DataManage.Instance != null)
        {
            DataManage.Instance.onChangePlayerData += this.UpdatePlayerUi;
        }
        List<int> listTest = new List<int>();

    }

    void Start()
    {
        DataManage.Instance.Chat_Init();

        //animaFaPai.gameObject.SetActive(false);
        end13Z_1Ju.gameObject.SetActive(false);
        end13Z_ALL.gameObject.SetActive(false);
        title_StartBiPai.SetActive(false);
        end13Z_1Ju.btnCloseEnd1Ju.onClick.AddListener(delegate ()
        {
            End1Ju_CloseInit();
        });
        end13Z_ALL.btn_CloseALL.onClick.AddListener(delegate ()
        {
            EndALL_CloseInit();
        });

        //Test_AddHandPoker();
        //Init_SetFenLei();
        //Init_BtnPTYPE();
        animaFaPai.gameObject.SetActive(true);

        Debug.Log("长度？" + pokerFenLei.list_DuiZi.Count);
        //countDownTime.onChangeTime = OnChange_CountDownTime;
        countDownTime.onTimeZero = OnChange_CountDownEND;
        //countDownTime.CountDown_Start(60);

        Net_SetEvent();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isMouseDown = false;
        }
    }

    public void Net_SetEvent()
    {
        if (isSetNet)
        {
            Debug.Log("【NET】已经设置过网络了");
            return;
        }
        PublicEvent.GetINS.Event_recivePlayerReady += UpdatePrepOK;//更新准备
        PublicEvent.GetINS.Event_reciveGetFirstCards += UpdateStartHandCards;//更新开始手牌
        PublicEvent.GetINS.Event_KeYiHu += Open_KeYiHu;//打开可以胡
        PublicEvent.GetINS.Event_Recive13ZBiPai += UpdateBiPai;//更新比牌
        PublicEvent.GetINS.Event_KeYiBao += Open_KeYiTing;//打开可以听
        PublicEvent.GetINS.Event_reciveSelectQue += UpdateHuleResult;//更新谁已经摆牌完毕

        PublicEvent.GetINS.Event_ReciveOtherCanPlay += IPengGangHu;  //服务器广播谁碰杠胡了

        PublicEvent.GetINS.Event_ReciveRoundOverResult += UpdateEndInning;//单局结算
        PublicEvent.GetINS.Event_ReciveRound_AllOverResult += UpdateEndAll;//全局结算

        PublicEvent.GetINS.Event_Recive13ZDaQiang += UpdateDaQiang;//打枪
        PublicEvent.GetINS.Event_reciveGetGuiCards += UpdateMaPai;//马牌

        PublicEvent.GetINS.Event_ReUpdateMj += ReUpdateMJ;//重连恢复牌面

        PublicEvent.GetINS.Event_RecivePlayer_VChat += Chat_V_P;//有人说话的声音闪烁图标
        isSetNet = true;
    }
    public void Net_RemoveEvent()
    {
        PublicEvent.GetINS.Event_recivePlayerReady -= UpdatePrepOK;
        PublicEvent.GetINS.Event_reciveGetFirstCards -= UpdateStartHandCards;
        PublicEvent.GetINS.Event_KeYiHu -= Open_KeYiHu;
        PublicEvent.GetINS.Event_Recive13ZBiPai -= UpdateBiPai;
        PublicEvent.GetINS.Event_KeYiBao -= Open_KeYiTing;
        PublicEvent.GetINS.Event_reciveSelectQue -= UpdateHuleResult;

        PublicEvent.GetINS.Event_ReciveOtherCanPlay -= IPengGangHu;  //服务器广播谁碰杠胡了

        PublicEvent.GetINS.Event_ReciveRoundOverResult -= UpdateEndInning;
        PublicEvent.GetINS.Event_ReciveRound_AllOverResult -= UpdateEndAll;

        PublicEvent.GetINS.Event_Recive13ZDaQiang -= UpdateDaQiang;
        PublicEvent.GetINS.Event_reciveGetGuiCards -= UpdateMaPai;

        PublicEvent.GetINS.Event_ReUpdateMj -= ReUpdateMJ;//重连恢复牌面

        PublicEvent.GetINS.Event_RecivePlayer_VChat -= Chat_V_P;
    }

    void OnDestroy()
    {
        Net_RemoveEvent();
        DataManage.Instance.onChangePlayerData -= this.UpdatePlayerUi;
    }

    void OnGUI()
    {
        //if (DataManage.Instance._roomEnterRsp != null
        //    && DataManage.Instance._roomEnterRsp.mjRoom != null
        //    )
        //{
        //    GUI.Box(new Rect(Screen.width * 0.5f, 32, 128, 32), "房间号" + DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString());
        //}
    }

    void OnChange_CountDownTime(int iTime)
    {
        //countDownTime.tShowTime.text = iTime.ToString();
    }

    void OnChange_CountDownEND()
    {
        Debug.Log("倒计时结束？？？？");
    }

    /// <summary>对手牌进行分类
    /// </summary>
    void Init_SetFenLei()
    {
        PokerInfos[] pokerAryXXX = locaHandPoker_Parent.GetComponentsInChildren<PokerInfos>();

        pokerFenLei.list_DuiZi = Poker13Type.PokerType_DuiZi(pokerAryXXX);// (locaHandPokerAry);
        pokerFenLei.list_2Dui = Poker13Type.PokerType_DuiZiX2(pokerAryXXX);
        pokerFenLei.list_3Tiao = Poker13Type.PokerType_3Tiao(pokerAryXXX);//(locaHandPokerAry);
        pokerFenLei.list_ShunZi = Poker13Type.PokerType_ShunZi(pokerAryXXX);//(locaHandPokerAry);
        pokerFenLei.list_TongHua = Poker13Type.PokerType_TongHua(pokerAryXXX);//(locaHandPokerAry);
        pokerFenLei.list_HuLu = Poker13Type.PokerType_HuLu(pokerAryXXX);
        pokerFenLei.list_ZhaDan = Poker13Type.PokerType_ZhaDan(pokerAryXXX);//(locaHandPokerAry);
        pokerFenLei.list_TongHuaShun = Poker13Type.PokerType_TongHuaShun(pokerAryXXX);//(locaHandPokerAry);

    }

    /// <summary>初始化 扑克分类按钮是否可点击
    /// </summary>
    void Init_BtnPTYPE()
    {
        btn_PType_DuiZi.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_DuiZi) > 0;
        btn_PType_ShunZi.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_ShunZi) > 0;
        btn_PType_2Dui.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_2Dui) > 0;
        btn_PType_3Tiao.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_3Tiao) > 0;
        btn_PType_TongHua.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_TongHua) > 0;
        btn_PType_HuLu.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_HuLu) > 0;
        btn_PType_ZhaDan.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_ZhaDan) > 0;
        btn_PType_THS.interactable = pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_TongHuaShun) > 0;
        btn_PType_TSP.interactable = gm13ZData.isTeShuPai; //pokerFenLei.GetTypeAryCount(Poker13Type_XXX.T_TeShuPai) > 0;
    }

    public void Test_AddHandPoker()
    {
        uint[] handAry = new uint[] { 1, 3, 3, 3, 12, 6, 9, 18, 55, 55, 55, 55, 57 };
        for (int i = 0; i < handAry.Length; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(handAry[i].ToPoker_X().CardName, locaHandPoker_Parent);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = handAry[i].ToPoker_X();
            pokerIns.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClick_Poker(pokerIns.GetComponent<PokerInfos>());
            });
        }
        locaHandPokerAry = locaHandPoker_Parent.GetComponentsInChildren<PokerInfos>();
    }

    #region/*———按钮事件区域———*/
    /// <summary>//【拖拽赋值】按钮准备
    /// </summary>
    public void OnClick_Btn_Prep()
    {
        PublicEvent.GetINS.Fun_SentClientPre();
        Debug.Log("OnClick_Btn_Prep");
        animaFaPai.gameObject.SetActive(true);
        bgBaiPaiParent.SetActive(false);
        BaiHao_CloseALL();
        DaQiang_CloseALL();

        for (int i = 0; i < bpLocaTexts.Length; i++)
        {
            bpLocaTexts[i].transform.parent.gameObject.SetActive(false);
        }
        gm13ZData.isHuPai = false;
    }
    /// <summary>//【拖拽赋值】按钮邀请
    /// </summary>
    public void OnClick_Btn_YaoQing()
    {
        Debug.Log("OnClick_Btn_Prep");
    }
    /// <summary>//【拖拽赋值】按钮退出
    /// </summary>
    public void OnClick_Btn_RequestQuitRoom()
    {
        If_OpenWindowQuitRoom();
    }
    /// <summary>//【拖拽赋值】按钮设置
    /// </summary>
    public void OnClick_Btn_OpenSetting()
    {
        Debug.Log("打开设置");
        UIManager.Instance.ShowUI(AllPrefabName.uiWin_Setting, UIManager.Instance.canvas_T);
    }
    /// <summary>//【拖拽赋值】按钮聊天
    /// </summary>
    public void OnClick_Btn_OpenChatPhiz()
    {

    }

    /// <summary>【拖拽赋值】点击手牌分类
    /// </summary>
    public void OnClick_Btn_PokerType(GameObject btn)
    {
        Debug.Log("Poker分类" + btn.gameObject.name);
        PokerInitClick();

        switch (btn.gameObject.name)
        {
            case "Btn_PType_DuiZi":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_DuiZi).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_2Dui":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_2Dui).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_3Tiao":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_3Tiao).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_ShunZi":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_ShunZi).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_TongHua":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_TongHua).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_HuLu":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_HuLu).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_ZhaDan":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_ZhaDan).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_THS":
                listCCAry = pokerFenLei.GetTypeAry(Poker13Type_XXX.T_TongHuaShun).ToArray();
                SelectAry_Poker(listCCAry);
                break;
            case "Btn_PType_TSP":
                //特殊牌
                PublicEvent.GetINS.Fun_SentTeShuPai(0);
                break;

            default:
                break;
        }
    }

    /// <summary>当点击了扑克
    /// </summary>
    public void OnClick_Poker(PokerInfos btnGm)
    {
        if (btnGm.IsSelect == false)
        {
            btnGm.Init_ClickHight();
        }
        btnGm.SelectPoker(!btnGm.IsSelect);
    }

    /// <summary>当按下扑克
    /// </summary>
    void OnHandDown_Poker(PokerInfos pokerInf)
    {
        if (pokerInf.transform.parent == locaHandPoker_Parent)
        {
            isMouseDown = true;
            OnHandEnter_Poker(pokerInf);
        }
    }

    /// <summary>当按下扑克的 时候 滑动选择
    /// </summary>
    void OnHandEnter_Poker(PokerInfos pokerInf)
    {
        if (pokerInf.transform.parent == locaHandPoker_Parent
            && isMouseDown)
        {
            if (pokerInf.IsSelect == false)
            {
                pokerInf.Init_ClickHight();
            }
            pokerInf.SelectPoker(!pokerInf.IsSelect);
        }
    }

    void PokerInitClick()
    {
        //StartCoroutine(SetPokerGLayout());
        locaHandPoker_Parent.GetComponent<GridLayoutGroup>().SetLayoutHorizontal(); //.CalculateLayoutInputHorizontal();
        locaHandPoker_Parent.GetComponent<GridLayoutGroup>().SetLayoutVertical();
        PokerInfos[] allHandCards = locaHandPoker_Parent.GetComponentsInChildren<PokerInfos>();
        //yield return new WaitForEndOfFrame();
        for (int i = 0; i < allHandCards.Length; i++)
        {
            if (allHandCards[i].transform.parent == locaHandPoker_Parent)
            {
                //if (allHandCards[i].IsSelect == true)
                //{
                //    allHandCards[i].SelectPoker(false);
                //}
                allHandCards[i].IsSelect = false;
                allHandCards[i].Init_ClickHight();
            }
        }
    }

    IEnumerator SetPokerGLayout()
    {
        locaHandPoker_Parent.GetComponent<GridLayoutGroup>().SetLayoutHorizontal(); //.CalculateLayoutInputHorizontal();
        locaHandPoker_Parent.GetComponent<GridLayoutGroup>().SetLayoutVertical();
        PokerInfos[] allHandCards = locaHandPoker_Parent.GetComponentsInChildren<PokerInfos>();
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < allHandCards.Length; i++)
        {
            if (allHandCards[i].transform.parent == locaHandPoker_Parent)
            {
                //if (allHandCards[i].IsSelect == true)
                //{
                //    allHandCards[i].SelectPoker(false);
                //}
                allHandCards[i].IsSelect = false;
                allHandCards[i].Init_ClickHight();
            }
        }
        yield return null;
    }

    void SelectAry_Poker(GameObject[] listPoker)
    {
        for (int i = 0; i < listPoker.Length; i++)
        {
            if (listPoker[i].transform.parent == locaHandPoker_Parent)
            {
                OnClick_Poker(listPoker[i].GetComponent<PokerInfos>());
            }
        }
    }

    /// <summary>摆牌,拖拽赋值
    /// </summary>
    public void OnClick_Btn_BaiPai(int iIndex)
    {
        if (iIndex < 0 || iIndex >= baiPai_ParentAry.Length)
        {
            Debug.Log("<color=red> </color>");
            return;
        }
        Transform clickParent = baiPai_ParentAry[iIndex];
        List<PokerInfos> listAry = new List<PokerInfos>();

        for (int i = 0; i < locaHandPokerAry.Length; i++)
        {
            if (locaHandPokerAry[i].IsSelect)
            {
                //Poker_BaiPai_Up(locaHandPokerAry[i], clickParent);
                listAry.Add(locaHandPokerAry[i]);
            }
        }
        switch (iIndex)
        {
            case 0:
                if (clickParent.childCount + listAry.Count > 3)
                {
                    Debug.Log("超过空位不能摆");
                    return;
                }
                break;
            case 1:
                if (clickParent.childCount + listAry.Count > 5)
                {
                    Debug.Log("超过空位不能摆");
                    return;
                }
                break;
            case 2:
                if (clickParent.childCount + listAry.Count > 5)
                {
                    Debug.Log("超过空位不能摆");
                    return;
                }
                break;
            default:
                break;
        }

        for (int i = 0; i < listAry.Count; i++)
        {
            Poker_BaiPai_Up(listAry[i], clickParent);
        }

        Init_SetFenLei();
        Init_BtnPTYPE();
        listAry = null;
    }

    /// <summary>取消道,拖拽赋值
    /// </summary>
    public void OnClick_QuXiao(int iIndex)
    {
        if (iIndex < 0 || iIndex >= baiPai_ParentAry.Length)
        {
            Debug.Log("<color=red> </color>");
            return;
        }
        Transform clickParent = baiPai_ParentAry[iIndex];
        PokerInfos[] pokersBaiPai = clickParent.GetComponentsInChildren<PokerInfos>();

        for (int i = 0; i < pokersBaiPai.Length; i++)
        {
            if (pokersBaiPai[i].IsSelect == false)
            {
                Poker_BaiPai_Down(pokersBaiPai[i], locaHandPoker_Parent);
            }
        }

        //Poker_SortLoca();

        Init_SetFenLei();
        Init_BtnPTYPE();
    }

    /// <summary>取消所有道
    /// </summary>
    public void OnClick_Btn_ClearALL()
    {
        for (int i = 0; i < baiPai_ParentAry.Length; i++)
        {
            OnClick_QuXiao(i);
        }
    }

    /// <summary>摆牌确定
    /// </summary>
    public void OnClick_Btn_ConfirmBP()
    {
        List<uint> cards = new List<uint>();
        PokerInfos[] pokerARY = baiPai_ParentAry[0].parent.GetComponentsInChildren<PokerInfos>();
        if (pokerARY.Length != 13)
        {
            Debug.LogError("没有摆完13张？");
            return;
        }
        cards = (from pokerINS in pokerARY
                 select pokerINS.pokerInfo.CardId).ToList();

        listTestBaiHao = (from pokerINS in pokerARY
                          select pokerINS.pokerInfo).ToList();

        string strResult = "";
        for (int i = 0; i < pokerARY.Length; i++)
        {
            strResult += pokerARY[i].pokerInfo.CardName + "—";
        }

        //poolPoker.PoolRecycleAll();
        //pokerFenLei.ClearAllList();
        //Init_BtnPTYPE();
        PublicEvent.GetINS.Fun_SentHu13Z_BPOK(cards);
        Debug.Log("摆牌确定OnClick_Btn_ConfirmBP\t" + strResult);
    }

    #endregion

    /// <summary>摆牌
    /// </summary>
    void Poker_BaiPai_Up(PokerInfos pokerX, Transform parentLayout)
    {
        Poker_X pokerSx = pokerX.pokerInfo;
        pokerX.GetComponent<Button>().onClick.RemoveAllListeners();
        pokerX.IsSelect = false;
        poolPoker.PoolRecycle(pokerX.gameObject);
        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerSx.CardName, parentLayout);
        pokerIns.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Poker_BaiPai_Down(pokerIns.GetComponent<PokerInfos>(), locaHandPoker_Parent);

            Init_SetFenLei();
            Init_BtnPTYPE();
        });

        if (locaHandPoker_Parent.childCount < 1)
        {
            btn_ConfirmBP.gameObject.SetActive(true);
            btn_ClearALL.gameObject.SetActive(true);
        }

        Poker_SortLoca(parentLayout);
    }

    /// <summary>摆牌
    /// </summary>
    void Poker_BaiPai_Up(PokerInfos[] pokerAry, Transform parentLayout)
    {

    }

    /// <summary>取消摆牌
    /// </summary>
    void Poker_BaiPai_Down(PokerInfos pokerX, Transform parentLayout)
    {
        Poker_X pokerSx = pokerX.pokerInfo;
        pokerX.GetComponent<Button>().onClick.RemoveAllListeners();
        poolPoker.PoolRecycle(pokerX.gameObject);
        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerSx.CardName, locaHandPoker_Parent);
        pokerIns.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClick_Poker(pokerIns.GetComponent<PokerInfos>());
        });

        Poker_SortLoca();
        PokerInitClick();


        btn_ConfirmBP.gameObject.SetActive(false);
        btn_ClearALL.gameObject.SetActive(false);
    }

    /// <summary>取消摆牌
    /// </summary>
    void Poker_BaiPai_Down(PokerInfos[] pokerAry, Transform parentLayout)
    {

    }

    /// <summary>本地手牌排序 ：1、当去下摆牌的时候
    /// </summary>
    public void Poker_SortLoca()
    {
        locaHandPokerAry = (from pokerIns in locaHandPokerAry
                            orderby pokerIns.pokerInfo.CardValue descending
                            select pokerIns).ToArray();

        for (int i = 0; i < locaHandPokerAry.Length; i++)
        {
            if (locaHandPokerAry[i] != null)
            {
                if (locaHandPokerAry[i].pokerInfo.CardValue > 1)
                {
                    locaHandPokerAry[i].transform.SetSiblingIndex(i);
                }
                else if (locaHandPokerAry[i].pokerInfo.CardValue == 1)
                {
                    locaHandPokerAry[i].transform.SetAsFirstSibling();
                }
                locaHandPokerAry[i].Init_ClickHight();
            }
        }
    }

    /// <summary>本地摆牌位置手牌排序 ：
    /// </summary>
    public void Poker_SortLoca(Transform tParent)
    {
        PokerInfos[] bpPokers = tParent.GetComponentsInChildren<PokerInfos>();

        bpPokers = (from pokerIns in bpPokers
                            orderby pokerIns.pokerInfo.CardValue descending
                            select pokerIns).ToArray();

        for (int i = 0; i < bpPokers.Length; i++)
        {
            if (bpPokers[i] != null)
            {
                if (bpPokers[i].pokerInfo.CardValue > 1)
                {
                    bpPokers[i].transform.SetSiblingIndex(i);
                }
                else if (bpPokers[i].pokerInfo.CardValue == 1)
                {
                    bpPokers[i].transform.SetAsFirstSibling();
                }
                bpPokers[i].Init_ClickHight();
            }
        }
    }

    void Prep_CloseALL()
    {
        for (int i = 0; i < player13ZAry.Length; i++)
        {
            player13ZAry[i].PrepOK_OpenOrClose(false);
        }
    }

    void BaiHao_CloseALL()
    {
        for (int i = 0; i < player13ZAry.Length; i++)
        {
            player13ZAry[i].BaiHao_OpenOrClose(false);
        }
    }

    /// <summary>关闭所有关于打枪
    /// </summary>
    void DaQiang_CloseALL()
    {
        for (int i = 0; i < player13ZAry.Length; i++)
        {
            player13ZAry[i].DaQiang_OpenOrClose(false);
        }
        for (int i = 0; i < player13ZAry.Length; i++)
        {
            player13ZAry[i].BeiDaQiang_OpenOrClose(false);
        }
    }

    void JoinRoom_Init()
    {
        bgBaiPaiParent.SetActive(false);
    }

    /// <summary>比牌开始
    /// </summary>
    public void BiPai_Start()
    {
        locaHandPokerAry = null;
        PokerInfos[] pokerAry = baiPai_ParentAry[0].parent.GetComponentsInChildren<PokerInfos>(true);
        poolPoker.PoolRecycle<PokerInfos>(pokerAry);//回收所有摆好的牌，准备比牌

        animaFaPai.gameObject.SetActive(false);
        bgBaiPaiParent.SetActive(false);
        StartCoroutine(AnimaTestBaiPai());
    }

    IEnumerator AnimaTestBaiPai()
    {
        Debug.Log("测试比牌 开始");
        List<PokerInfos> pokerAry = new List<PokerInfos>();
        title_StartBiPai.SetActive(true);
        yield return new WaitForSeconds(2);
        title_StartBiPai.SetActive(false);

        for (int i = 0; i < listTestBaiHao.Count && i < 3; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(listTestBaiHao[i].CardName);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = listTestBaiHao[i];
            pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
            pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
        }
        Debug.Log("测试比牌 AAA" + pokerAry.Count);
        player13ZAry[0].BiPai_ADD(0, pokerAry.ToArray(), 66);
        pokerAry.Clear();

        yield return new WaitForSeconds(1);

        for (int i = 3; i < listTestBaiHao.Count && i < 8; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(listTestBaiHao[i].CardName);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = listTestBaiHao[i];
            pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
            pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
        }
        Debug.Log("测试比牌 AAA" + pokerAry.Count);
        player13ZAry[0].BiPai_ADD(1, pokerAry.ToArray(), -66);
        pokerAry.Clear();

        yield return new WaitForSeconds(1);

        for (int i = 8; i < listTestBaiHao.Count; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(listTestBaiHao[i].CardName);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = listTestBaiHao[i];
            pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
            pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
        }
        Debug.Log("测试比牌 CCC" + pokerAry.Count);
        player13ZAry[0].BiPai_ADD(2, pokerAry.ToArray(), 123);
        pokerAry.Clear();
        yield return new WaitForSeconds(1);
        Debug.Log("测试比牌 结束");
        pokerAry = null;
        yield return null;
    }

    #region/*———Player Ui Show———*/

    /// <summary>更新房主
    /// </summary>
    void UpdateFangZhuStyle()
    {
        Debug.Log("Set FangZhu UI???" + DataManage.Instance.roomBoosId);
        int i_Index = DataManage.Instance.PData_GetIndex(DataManage.Instance.roomBoosId);// Get_Player_Index(zhuangJia_Id);
        //fangZhuStyle.transform.SetParent(allPlayerInfoUis[i_Index].btnHead.transform.parent.parent);
        //fangZhuStyle.gameObject.SetActive(true);
        //fangZhuStyle.anchoredPosition = Vector2.one * -15;
        allPlayerInfoUis[i_Index].transform.FindChild("FangZhu_Style").gameObject.SetActive(true);
    }

    public void UpdatePlayerUi(Player_Data[] list_RoomPData)
    {
        if (list_RoomPData.Length > allPlayerInfoUis.Length)
        {
            Debug.LogError("<color=red>这里可能出现错误！</color>");
        }
        SetLeftTop_UI();//设置左上角房间信息UI
        UpdateFangZhuStyle();
        for (int i_1 = 0; i_1 < list_RoomPData.Length; i_1++)
        {
            if (list_RoomPData[i_1] == null || list_RoomPData[i_1].p_ID == 0)
            {
                allPlayerInfoUis[i_1].Init_();
            }
            else
            {
                if (allPlayerInfoUis[i_1].playerCharID != list_RoomPData[i_1].p_ID)
                {
                    allPlayerInfoUis[i_1].gameObject.SetActive(true);
                    allPlayerInfoUis[i_1].btnHead.image.sprite = null;
                    if (allPlayerInfoUis[i_1].isInitVar == false)
                    {
                        allPlayerInfoUis[i_1].Init_Var();
                    }
                    Debug.Log("__12_____" + allPlayerInfoUis[i_1].gameObject.activeInHierarchy);
                    allPlayerInfoUis[i_1].Set_PlayerInfoUI(list_RoomPData[i_1]);
                }
            }
        }
    }

    /// <summary>获取空位的索引
    /// </summary>
    int GetPlayerIsNull()
    {
        for (int i = 0; i < allPlayerInfoUis.Length; i++)
        {
            if (allPlayerInfoUis[i].gameObject.activeInHierarchy == false)
            {
                return i;
            }
        }
        return 3;
    }

    #endregion

    void SetLeftTop_UI()
    {
        ruleInfoUI.tRoomNumber.text = "房间号:"+ DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString();
        ruleInfoUI.tRoomRuleInfo.text = "规则:\n" + DataManage.Instance.RoomInfoNxStr;

    }

    /// <summary>可以特殊牌
    /// </summary>
    void KeYi_TeShuPai(uint charid)
    {
        Debug.Log("可以特殊牌");
        if (charid == 0)
        {
            btn_PType_TSP.interactable = true;
        }
    }

    /// <summary>更新玩家已准备
    /// </summary>
    void UpdatePrepOK(uint charid)
    {
        Debug.Log("Prep   Charid :" + charid);
        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            
        }
        player13ZAry[DataManage.Instance.PData_GetIndex(charid)].PrepOK_OpenOrClose(true);
        
    }

    //碰杠胡委托总接口
    void IPengGangHu(ProtoBuf.MJGameOP MjOp, uint charid, uint card, uint oriCharid = 0)
    {
        Debug.Log("调用了么");
        switch (MjOp)
        {

            case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:

                break;

            case ProtoBuf.MJGameOP.MJ_OP_GUO:

                break;

            case ProtoBuf.MJGameOP.MJ_OP_HU:

                break;
            case ProtoBuf.MJGameOP.MJ_OP_TING:
                //UpdateTing(charid);
                break;

            default:
                break;

        }
    }

    /// <summary>更新玩家初始手牌
    /// </summary>
    void UpdateStartHandCards(List<uint> cards, int saet)
    {
        Prep_CloseALL();
        DataManage.Instance.MyPlayer_Data.isgaming = true;
        UpdateMaPai(new List<uint>(new uint[] { gm13ZData.maPaiCard }));
        gm13ZData.isTeShuPai = false;

        btn_ConfirmBP.gameObject.SetActive(false);
        btn_ClearALL.gameObject.SetActive(false);
        //cards.Sort();
        center_Bg0.SetActive(false);
        iBiPai_Index = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(cards[i].ToPoker_X().CardName, locaHandPoker_Parent);
            PokerInfos pokerInf = pokerIns.GetComponent<PokerInfos>();
            pokerInf.pokerInfo = cards[i].ToPoker_X();
            pokerInf.Init_ClickHight();
            pokerInf.IsSelect = false;
            pokerIns.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                OnClick_Poker(pokerInf);
            });

            if (pokerInf.pokerInfo.CardId == this.gm13ZData.maPaiCard)
            {
                MaPaiTag_IfAdd(pokerInf);
            }

            /*添加滑动批量选择*/
            if (pokerIns.GetComponent<DragPoker13Z>() == null)
            {
                pokerIns.AddComponent<DragPoker13Z>();
            }
            DragPoker13Z dragPoker = pokerIns.GetComponent<DragPoker13Z>();
            dragPoker.onHandDown.RemoveAllListeners();
            dragPoker.onHandEnter.RemoveAllListeners();
            dragPoker.onHandDown.AddListener(delegate ()
            {
                OnHandDown_Poker(pokerInf);
            });
            dragPoker.onHandEnter.AddListener(delegate ()
            {
                OnHandEnter_Poker(pokerInf);
            });
        }
        locaHandPokerAry = locaHandPoker_Parent.GetComponentsInChildren<PokerInfos>();
        Poker_SortLoca();

        Init_SetFenLei();
        Init_BtnPTYPE();

        if (gm13ZData.isHuPai == false)
        {
            animaFaPai.Anima_Start(GetPlayerIsNull());
            StartCoroutine(WaitTimeEvent(3,delegate() {
                bgBaiPaiParent.SetActive(true);
                countDownTime.CountDown_Start(60);
            }));
        }
        

        string strIpPrompt = "";
        Player_Data[] all_Player_Data = DataManage.Instance.PData_GetDataAry();
        for (int i = 0; i < all_Player_Data.Length; i++)
        {
            for (int i_1 = 0; i_1 < all_Player_Data.Length; i_1++)
            {
                if (i_1 != i)
                {
                    if (all_Player_Data[i].p_Ip == all_Player_Data[i_1].p_Ip)
                    {
                        strIpPrompt += "[<color=yellow>" + all_Player_Data[i].P_Name + "</color>]和[<color=yellow>" + all_Player_Data[i_1].P_Name + "</color>]玩家在同一IP！！！\n";
                        break;
                    }
                }
            }
        }
        if (strIpPrompt.Length > 0 && isPromptIP == false)
        {
            //PromptManage.AddPrompt(new PromptModel("同一IP提醒：\n" + strIpPrompt, delegate () { }));
            GameObject uiWin = UIManager.Instance.ShowPerfabUI(AllPrefabName.UiWin_PromptIp, UIManager.Instance.canvas_T);
            uiWin.GetComponent<UiWin_Prompt>().IF_Index(uiWin.transform);
            uiWin.GetComponentInChildren<Text>().text = "同一IP提醒：\n" + strIpPrompt;

            isPromptIP = true;
            Debug.Log("【IP：】" + strIpPrompt);
        }
    }

    /// <summary>更新玩家比牌
    /// </summary>
    void UpdateBiPai(List<ProtoBuf.ComparedCard> cardDatas)
    {
        List<PokerInfos> pokerAry = new List<PokerInfos>();
        iBiPai_Index++;
        BiPai_Sort(ref cardDatas);

        if (isRelink)
        {
            poolPoker.PoolRecycleAll();
            pokerFenLei.ClearAllList();
            Init_BtnPTYPE();
            animaFaPai.gameObject.SetActive(false);
            bgBaiPaiParent.SetActive(false);

            isRelink = false;
        }
        
        switch (iBiPai_Index)
        {
            case 1:
                
                BaiHao_CloseALL();
                //title_StartBiPai.SetActive(true);
                
                break;
            case 2:
                title_StartBiPai.SetActive(true);//(false);
                StartCoroutine(WaitTimeEvent(1.5f, delegate ()
                {
                    if (System.Math.Round(title_StartBiPai.transform.localScale.x, 1) == 1.0f)
                    {
                        title_StartBiPai.SetActive(false);
                    }
                }));

                //yield return new WaitForSeconds(2);
                for (int i_C = 0; i_C < cardDatas.Count; i_C++)
                {
                    List<Poker_X> pokerXAry = new List<Poker_X>();
                    for (int i = 0; i < cardDatas[i_C].card.Count; i++)
                    {
                        pokerXAry.Add(cardDatas[i_C].card[i].ToPoker_X());
                    }
                    for (int i = 0; i < pokerXAry.Count && i < 3; i++)
                    {
                        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
                        pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
                        pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
                        pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
                        if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
                        {
                            MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
                        }
                    }
                    uint charidXX= cardDatas[i_C].charid;
                    BiPaiJiLu_X bpjlX = new BiPaiJiLu_X();
                    bpjlX.pokerAry = pokerAry.ToArray();
                    bpjlX.gold = cardDatas[i_C].soce;
                    bpjlX.pokerType = BiPai_GetName(cardDatas[i_C].type);
                    bpjlX.pokerType = bpjlX.pokerType.IndexOf("三条")>=0 ? "豹子" : bpjlX.pokerType;
                    Debug.Log("测试比牌 AAA" + pokerAry.Count);
                    StartCoroutine(WaitTimeEvent(i_C,delegate()
                    {
                        AudioPlay(bpjlX.pokerType, charidXX);
                        player13ZAry[DataManage.Instance.PData_GetIndex(charidXX)].BiPai_ADD(0, bpjlX.pokerAry, bpjlX.gold, bpjlX.pokerType);
                    }));

                    if (charidXX == DataManage.Instance.MyPlayer_Data.p_ID)
                    {//如果是本地玩家， 单独显示一次分数
                        bpLocaTexts[0].text = "头道：" + cardDatas[i_C].soce;
                        bpLocaTexts[0].transform.parent.gameObject.SetActive(true);
                    }
                    pokerAry.Clear();
                    //player13ZAry[DataManage.Instance.PData_GetIndex(cardDatas[i_C].charid)].BiPai_ADD(0, pokerAry.ToArray(), cardDatas[i_C].soce, BiPai_GetName(cardDatas[i_C].type));
                    //pokerAry.Clear();
                }

                break;
            case 3:
                //title_StartBiPai.SetActive(false);
                for (int i_C = 0; i_C < cardDatas.Count; i_C++)
                {
                    List<Poker_X> pokerXAry = new List<Poker_X>();
                    for (int i = 0; i < cardDatas[i_C].card.Count; i++)
                    {
                        pokerXAry.Add(cardDatas[i_C].card[i].ToPoker_X());
                    }
                    for (int i = 0; i < pokerXAry.Count && i < 5; i++)
                    {
                        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
                        pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
                        pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
                        pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
                        if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
                        {
                            MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
                        }
                    }
                    uint charidXX = cardDatas[i_C].charid;
                    BiPaiJiLu_X bpjlX = new BiPaiJiLu_X();
                    bpjlX.pokerAry = pokerAry.ToArray();
                    bpjlX.gold = cardDatas[i_C].soce;
                    bpjlX.pokerType = BiPai_GetName(cardDatas[i_C].type);
                    bpjlX.pokerType = bpjlX.pokerType.IndexOf("葫芦") >= 0 ? "中墩葫芦" : bpjlX.pokerType;
                    bpjlX.pokerType = bpjlX.pokerType.IndexOf("炸弹") >= 0 ? "中墩炸弹" : bpjlX.pokerType;

                    Debug.Log("测试比牌 AAA" + pokerAry.Count);
                    StartCoroutine(WaitTimeEvent(i_C, delegate ()
                    {
                        AudioPlay(bpjlX.pokerType, charidXX);
                        player13ZAry[DataManage.Instance.PData_GetIndex(charidXX)].BiPai_ADD(1, bpjlX.pokerAry, bpjlX.gold, bpjlX.pokerType);
                    }));

                    if (charidXX == DataManage.Instance.MyPlayer_Data.p_ID)
                    {//如果是本地玩家， 单独显示一次分数
                        bpLocaTexts[1].text = "中道：" + cardDatas[i_C].soce;
                        bpLocaTexts[1].transform.parent.gameObject.SetActive(true);
                    }
                    pokerAry.Clear();
                    Debug.Log("测试比牌 BBB" + pokerAry.Count);
                    //player13ZAry[DataManage.Instance.PData_GetIndex(cardDatas[i_C].charid)].BiPai_ADD(1, pokerAry.ToArray(), cardDatas[i_C].soce, BiPai_GetName(cardDatas[i_C].type));
                    //pokerAry.Clear();
                }
                break;
            case 4:
                //title_StartBiPai.SetActive(false);
                for (int i_C = 0; i_C < cardDatas.Count; i_C++)
                {
                    List<Poker_X> pokerXAry = new List<Poker_X>();
                    for (int i = 0; i < cardDatas[i_C].card.Count; i++)
                    {
                        pokerXAry.Add(cardDatas[i_C].card[i].ToPoker_X());
                    }
                    for (int i = 0; i < pokerXAry.Count && i < 5; i++)
                    {
                        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
                        pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
                        pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
                        pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
                        if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
                        {
                            MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
                        }
                    }

                    uint charidXX = cardDatas[i_C].charid;
                    BiPaiJiLu_X bpjlX = new BiPaiJiLu_X();
                    bpjlX.pokerAry = pokerAry.ToArray();
                    bpjlX.gold = cardDatas[i_C].soce;
                    bpjlX.pokerType = BiPai_GetName(cardDatas[i_C].type);
                    Debug.Log("测试比牌 AAA" + pokerAry.Count);
                    StartCoroutine(WaitTimeEvent(i_C, delegate ()
                    {
                        AudioPlay(bpjlX.pokerType, charidXX);
                        player13ZAry[DataManage.Instance.PData_GetIndex(charidXX)].BiPai_ADD(2, bpjlX.pokerAry, bpjlX.gold, bpjlX.pokerType);
                    }));

                    if (charidXX == DataManage.Instance.MyPlayer_Data.p_ID)
                    {//如果是本地玩家， 单独显示一次分数
                        bpLocaTexts[2].text = "尾道：" + cardDatas[i_C].soce;
                        bpLocaTexts[2].transform.parent.gameObject.SetActive(true);
                    }
                    pokerAry.Clear();
                    Debug.Log("测试比牌 CCC" + pokerAry.Count);
                    //player13ZAry[DataManage.Instance.PData_GetIndex(cardDatas[i_C].charid)].BiPai_ADD(2, pokerAry.ToArray(), cardDatas[i_C].soce, BiPai_GetName(cardDatas[i_C].type));
                    //pokerAry.Clear();
                }
                break;
            case 5:
                //title_StartBiPai.SetActive(false);
                //特殊牌
                for (int i_C = 0; i_C < cardDatas.Count; i_C++)
                {
                    List<Poker_X> pokerXAry = new List<Poker_X>();
                    for (int i = 0; i < cardDatas[i_C].card.Count; i++)
                    {
                        pokerXAry.Add(cardDatas[i_C].card[i].ToPoker_X());
                    }
                    for (int i = 0; i < pokerXAry.Count; i++)
                    {
                        GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
                        pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
                        pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
                        pokerAry.Add(pokerIns.GetComponent<PokerInfos>());
                        if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
                        {
                            MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
                        }
                    }
                    Debug.Log("测试比牌 DDD" + pokerAry.Count);
                    player13ZAry[DataManage.Instance.PData_GetIndex(cardDatas[i_C].charid)].BiPai_ADD(2, pokerAry.ToArray(), cardDatas[i_C].soce);
                    pokerAry.Clear();
                }
                break;
            default:
                break;
        }
        Debug.Log("开始比牌");
    }

    /// <summary>比牌排序,分数最低先比
    /// </summary>
    void BiPai_Sort(ref List<ProtoBuf.ComparedCard> cardDatas)
    {
        cardDatas = (from cardIns in cardDatas
                     orderby cardIns.soce ascending
                     select cardIns).ToList();
    }

    IEnumerator WaitTimeEvent(float waitTime,UnityEngine.Events.UnityAction onEvent)
    {
        yield return new WaitForSeconds(waitTime);
        if (onEvent!=null)
        {
            onEvent.Invoke();
        }
    }

    /// <summary>更新玩家打枪
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="charidAry"></param>
    void UpdateDaQiang(uint charid, List<uint> charidAry)
    {
        Debug.Log("UpdateDaQiang:" + charid + "Count:" + charidAry.Count);
        player13ZAry[DataManage.Instance.PData_GetIndex(charid)].DaQiang_OpenOrClose(true);
        for (int i = 0; i < charidAry.Count; i++)
        {
            player13ZAry[DataManage.Instance.PData_GetIndex(charidAry[i])].BeiDaQiang_OpenOrClose(true);
        }
    }

    /// <summary>更新马牌
    /// </summary>
    void UpdateMaPai(List<uint> cards, int seat = 0)
    {
        if (cards.Count < 1)
        {
            Debug.Log("<color=red>Error MaPai Cards.Count == 0</color>");
            return;
        }

        if (cards[0] == 0)
        {
            Debug.Log("<color=red>Error MaPai Card == 0</color>");
            return;
        }
        gm13ZData.maPaiCard = cards[0];

        if (vLayout_MaPai.childCount > 0)
        {
            //poolPoker.PoolRecycle(vLayout_MaPai.GetChild(0).gameObject);
            return;
        }

        GameObject pokerIns = poolPoker.PoolGetGameObject(gm13ZData.maPaiCard.ToPoker_X().CardName);
        GameObject gmIns = Instantiate(pokerIns);
        gmIns.transform.SetParent(vLayout_MaPai);
        gmIns.transform.localScale = Vector3.one;
        poolPoker.PoolRecycle(pokerIns);
    }

    /// <summary>马牌标签—》判断手牌是不是马牌，如果是就加一个标签
    /// </summary>
    void MaPaiTag_IfAdd(PokerInfos pokerIns)
    {
        maPai_Tag.SetParent(pokerIns.transform);
        maPai_Tag.localScale = Vector3.one;
        maPai_Tag.anchoredPosition = Vector2.zero;
    }

    void MaPaiTag_Init()
    {
        maPai_Tag.SetParent(transform);
        maPai_Tag.anchoredPosition = Vector2.one * -128;
    }

    void Open_KeYiHu(uint charid, uint oricharid)
    {
        Debug.Log("Player KeYi Hu" + charid);
    }

    void Open_KeYiTing(uint charid, List<uint> cards)
    {
        Debug.Log("Player KeYi TeShuPai" + charid);
        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            gm13ZData.isTeShuPai = true;
            btn_PType_TSP.interactable = true;
        }
    }

    /// <summary>更新玩家摆牌成功，或玩家摆牌失败
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="cardType"></param>
    void UpdateHuleResult(uint charid, uint cardType)
    {
        Debug.Log("UpdateHule" + charid + "___是否404失败：" + cardType);
        if (cardType == 404 && charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {//摆牌失败
            PromptManage.AddPrompt(new PromptModel("摆牌失败！\n请检查您的牌是否达成条件:\n尾道>中道>头道", delegate ()
            {
                Debug.Log("Close Prompt UpdateHuleResult");
            }));
            OnClick_Btn_ClearALL();

            btn_ConfirmBP.gameObject.SetActive(false);
            btn_ClearALL.gameObject.SetActive(false);

            return;
        }
        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            poolPoker.PoolRecycleAll();
            pokerFenLei.ClearAllList();
            Init_BtnPTYPE();

            //animaFaPai.gameObject.SetActive(false);
            bgBaiPaiParent.SetActive(false);
            gm13ZData.isHuPai = true;
        }
        player13ZAry[DataManage.Instance.PData_GetIndex(charid)].BaiHao_OpenOrClose(true);
    }


    void UpdateTing(uint charid, uint cardType)
    {
        Debug.Log("UpdateTing OK" + charid);
    }

    #region/*———关于重连———*/

    /// <summary>重连恢复扑克
    /// </summary>
    /// <param name="mjRoom"></param>
    void ReUpdateMJ(ProtoBuf.MJRoomInfo mjRoom)
    {
        Debug.Log("13Z 重连更新牌面~");
        Prep_CloseALL();
        DataManage.Instance.MyPlayer_Data.isgaming = true;

        if (mjRoom.charStates.Count > 0 && mjRoom.charStates[0].xiazhu > 0)
        {
            UpdateMaPai(new List<uint>(new uint[] { (uint)mjRoom.charStates[0].xiazhu }));
        }
        isRelink = true;
        //更新四方向的牌
        for (int i = 0; i < mjRoom.charIds.Count; i++)
        {
            if (mjRoom.charIds[i] == DataManage.Instance.MyPlayer_Data.p_ID)
            {
                
                for (int i_2 = 0; i_2 < mjRoom.cardsInfos[i].x3zOut.Count; i_2++)
                {//有人已经胡了牌
                    gm13ZData.isHuPai = mjRoom.cardsInfos[i].x3zOut[i_2] > 0;
                }

                UpdateStartHandCards(mjRoom.cardsInfos[i].handCards, 0);
            }
            else
            {//不是本地玩家

            }
        }

        for (int x = 0; x < mjRoom.roomCache.charList.Count; x++)
        {
            if (DataManage.Instance.PData_GetIndex(mjRoom.roomCache.charList[x].charId) != -1)
            {
                int seat = DataManage.Instance.PData_GetIndex(mjRoom.roomCache.charList[x].charId);
                //if (mjRoom.roomCache.charList[x].opList.Count>0&& !mjRoom.roomCache.charList[x].opList.Contains(6)) {
                for (int i = 0; i < mjRoom.roomCache.charList[x].opList.Count && seat == 0; i++)
                {
                    Debug.Log(mjRoom.roomCache.charList.Count + "  _" + x + "_  " + (mjRoom.roomCache.charList[x].opList[i] - 1).ToString());
                    ProtoBuf.MJGameOP[] mjGmOps = (ProtoBuf.MJGameOP[])Enum.GetValues(typeof(ProtoBuf.MJGameOP));
                    ProtoBuf.MJGameOP c_MjGmOps = mjGmOps[mjRoom.roomCache.charList[x].opList[i] - 1];
                    //Debug.Log(Debug_Mj_GameC_.ChongLianType___.ToString() + "EnumType:" + c_MjGmOps.ToString());
                    switch (c_MjGmOps)
                    {
                        case ProtoBuf.MJGameOP.MJ_OP_TING:
                            PublicEvent.GetINS.Fun_KeYiBao(mjRoom.roomCache.charList[x].charId, mjRoom.cardsInfos[x].tingCards);

                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_HU:

                            PublicEvent.GetINS.Fun_reciveCanPlay(mjRoom.roomCache.charList[x].charId, ProtoBuf.MJGameOP.MJ_OP_HU, mjRoom.roomCache.charList[x].cardList, mjRoom.roomCache.charList[x].oriCharId);

                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                Debug.Log("<color=red> 重连后，没有在所有玩家Id里面找到该玩家ID？+" + "</color>");
            }
        }
    }

    #endregion

    #region /*———关于结束———*/

    /// <summary>更新单局结算
    /// </summary>
    void UpdateEndInning(ProtoBuf.MJGameOver end_MJGmOver)
    {
        Debug.Log("13Shui END 1 ");
        //DaQiang_CloseALL();

        End1Ju_OpenInit();
        end13Z_1Ju.gameObject.SetActive(true);
        end13Z_1Ju.UpdatePlayerUI(DataManage.Instance.PData_GetDataAry());
        for (int i = 0; i < end_MJGmOver.players.Count; i++)
        {
            
            //////mjOv1.end_Mj_P_UI[i].Set_PlayerUIGai(rsp.players[i], DataManage.Instance.PData_GetData(rsp.players[i].charId).P_Name, zhuangJia_Id.ToString());
            end13Z_1Ju.UpdatePlayerUI_Gold(end_MJGmOver.players[i].charId, end_MJGmOver.players[i].changeGold);
            List<uint> handCards = end_MJGmOver.players[i].restCards;// Poker_Sort(end_MJGmOver.players[i].restCards);
            List<GameObject> listResult = GetPokerObjList(handCards);// end_MJGmOver.players[i].restCards);
            end13Z_1Ju.UpdatePlayerUI_Poker(end_MJGmOver.players[i].charId, listResult.ToArray(), end_MJGmOver.players[i].hasTingPai > 0);
            end13Z_1Ju.UpdatePlayerUI_TSPTxt(end_MJGmOver.players[i].charId, TSP_GetName(end_MJGmOver.players[i].hasTingPai));

            Debug.Log(end_MJGmOver.players[i].charId + "End Poker Count " + end_MJGmOver.players[i].restCards.Count);
        }
    }

    /// <summary>根据cardAry 获取扑克Ary
    /// </summary>
    List<GameObject> GetPokerObjList(List<uint> cards)
    {
        List<GameObject> listResult = new List<GameObject>();
        List<Poker_X> pokerXAry = new List<Poker_X>();
        for (int i = 0; i < cards.Count; i++)
        {
            pokerXAry.Add(cards[i].ToPoker_X());
        }
        for (int i = 0; i < pokerXAry.Count; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
            pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
            listResult.Add(pokerIns);

            if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
            {
                MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
            }
        }
        return listResult;
    }

    /// <summary>单局结算打开 ——>初始化
    /// </summary>
    void End1Ju_OpenInit()
    {
        //poolPoker.PoolRecycleAll();
        //pokerFenLei.ClearAllList();
        //Init_BtnPTYPE();
    }

    /// <summary>单局结算关闭 ——>初始化
    /// </summary>
    void End1Ju_CloseInit()
    {
        if (isTPCG && end13Z_ALL.gameObject.activeInHierarchy == false)
        {
            GameManager.GM.Game_RetrueHome();
            return;
        }
        poolPoker.PoolRecycleAll();
        pokerFenLei.ClearAllList();
        Init_BtnPTYPE();

        poolPoker.PoolRecycleAll();
        for (int i = 0; i < player13ZAry.Length; i++)
        {//清除桌面上显示的所有分数
            player13ZAry[i].ShowGold_ClearAll();
        }

        center_Bg0.SetActive(true);
        end13Z_1Ju.gameObject.SetActive(false);
    }

    /// <summary>全局结算
    /// </summary>
    /// <param name="end_All_S"></param>
    void UpdateEndAll(List<ProtoBuf.MJGameOver> end_All_S)
    {
        Debug.Log("UpdateEndAll" + end_All_S.Count);
        end13Z_ALL.gameObject.SetActive(true);
        end13Z_ALL.UpdatePlayerUI(DataManage.Instance.PData_GetDataAry());
        int endIndex = end_All_S.Count - 1;
        int iDYJ = 0;
        for (int i = 0; i < end_All_S[endIndex].players.Count; i++)
        {
            int i_ShengLi = 0;
            int i_Shu = 0;
            int i_PingJu = 0;
            for (int i_1 = 0; i_1 < end_All_S.Count; i_1++)
            {
                if (end_All_S[i_1].players[i].changeGold<0)
                {
                    i_Shu++;
                }else if (end_All_S[i_1].players[i].changeGold == 0)
                {
                    i_PingJu++;
                }
                else if (end_All_S[i_1].players[i].changeGold > 0)
                {
                    i_ShengLi++;
                }
            }
            iDYJ = end_All_S[endIndex].players[i].restGold > iDYJ ? end_All_S[endIndex].players[i].restGold : iDYJ;
            end13Z_ALL.UpdatePlayerUI_TextAry(
                end_All_S[endIndex].players[i].charId,
                end_All_S[endIndex].players[i].restGold,
                i_ShengLi,
                i_Shu,
                i_PingJu,
                (int)end_All_S[endIndex].costDiamond);
        }
        end13Z_ALL.UpdatePlayerUI_DaYingJia(iDYJ);
    }

    void EndALL_OpenInit()
    {

    }

    void EndALL_CloseInit()
    {
        GameManager.GM.Game_RetrueHome();
    }

    #endregion

    #region/*———关于退出房间———*/

    void If_OpenWindowQuitRoom()
    {
        GameObject uiQuitPrompt = UIManager.Instance.ShowUI(AllPrefabName.uiWin_Prompt_QuitRoom, UIManager.Instance.canvas_T);

        uiQuitPrompt.transform.FindChild("Img_Bg0/Btn_CloseWin").GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
        });

        Button btn_ = uiQuitPrompt.transform.FindChild("Img_Bg0/Btn_Confirm").GetComponent<Button>();

        //if (c_GameScene == C_Game_Scene.麻将游戏界面)
        //{

        if (DataManage.Instance.MyPlayer_Data.isgaming)
        {
            uiQuitPrompt.GetComponentInChildren<Text>().text = "已经开始了游戏，确认退出并发起<color=red>解散</color>房间投票吗？";
            btn_.onClick.AddListener(delegate ()
            {
                this.Btn_DetermineOpenVote(3);
                UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
            });
            //Debug.Log("ZhiXing  NotGame_QuitRoom");
        }
        else
        {
            //Debug.Log("ZhiXing  NotGame_QuitRoom");
            if (DataManage.Instance.roomBoosId == DataManage.Instance.MyPlayer_Data.p_ID)
            {//是房主，
                uiQuitPrompt.GetComponentInChildren<Text>().text = "您是房主,还没有开始游戏，\n确认<color=red>解散</color>房间吗？";
                btn_.onClick.AddListener(delegate ()
                {
                    this.Btn_DetermineOpenVote(3);
                    UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
                });
            }
            else
            {//不是房主
                uiQuitPrompt.GetComponentInChildren<Text>().text = "您不是房主确认<color=red>退出</color>房间吗？";
                btn_.onClick.AddListener(delegate ()
                {
                    this.Btn_DetermineOpenVote(5);
                    UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);

                });
            }
        }

    }

    /// <summary>退出房间，确定按钮事件 
    /// 3:发起投票退出确定——4:发起投票退出拒绝——5：直接退出
    /// </summary>
    public void Btn_DetermineOpenVote(int i_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        switch (i_)
        {
            case 3:
                PublicEvent.GetINS.VoteRequest_Game13Z(true);
                break;
            case 4:
                PublicEvent.GetINS.VoteRequest_Game13Z(false);
                break;
            case 5:
                PublicEvent.GetINS.OnExitRoom();
                break;
            default:
                break;
        }
        

    }
    #endregion

    public void Chat_V_P(uint p_Id)
    {
        allPlayerInfoUis[DataManage.Instance.PData_GetIndex(p_Id)].transform.FindChild("VChatStyleBg").gameObject.SetActive(true);
        StartCoroutine(WaitTimeEvent(6, delegate ()
        {
            if (gameObject.activeInHierarchy)
            {
                allPlayerInfoUis[DataManage.Instance.PData_GetIndex(p_Id)].transform.FindChild("VChatStyleBg").gameObject.SetActive(false);
            }
        }));
    }

    void AudioPlay(string cnSTR, uint charid)
    {
        string strPath1 = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan_GM13Z + AudioPath.cardType : AudioPath.sexWoMan_GM13Z + AudioPath.cardType;

        switch (cnSTR)
        {
            case "豹子":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.BaoZi.ToString());
                break;
            case "豹子六对半":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.BaoZiLiuDuiBan.ToString());
                break;
            case "豹子炸弹六对半":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.BzZdLiuDuiBan.ToString());
                break;
            case "打枪":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.DaQiang.ToString());
                break;
            case "对子":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.DuiZi.ToString());
                break;
            case "葫芦":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.HuLu.ToString());
                break;
            case "开始比牌":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.KaiShiBiPai.ToString());
                break;
            case "两对":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.LiangDui.ToString());
                break;
            case "六对半":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.LiuDuiBan.ToString());
                break;
            case "全大":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.QuanDa.ToString());
                break;
            case "全黑":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.QuanHei.ToString());
                break;
            case "全红":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.QuanHong.ToString());
                break;
            case "全垒打":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.QuanLeiDa.ToString());
                break;
            case "全小":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.QuanXiao.ToString());
                break;
            case "三分天下":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.SanFenTianXia.ToString());
                break;
            case "三顺子":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.SanShunZi.ToString());
                break;
            case "三条":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.SanTiao.ToString());
                break;
            case "三同花":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.SanTongHua.ToString());
                break;
            case "双炸六对半":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ShuangZhaLiuDuiBan.ToString());
                break;
            case "四套三条":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.SiTaoSanTiao.ToString());
                break;
            case "乌龙":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.WuLong.ToString());
                break;
            case "一条龙":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.YiTiaoLong.ToString());
                break;
            case "一条青龙":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.YiTiaoQingLong.ToString());
                break;
            case "炸弹":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhaDan.ToString());
                break;
            case "炸弹六对半":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhaDanLiuDuiBan.ToString());
                break;
            case "中墩葫芦":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhongDunHuLu.ToString());
                break;
            case "中墩同花顺":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhongDunTongHuaShun.ToString());
                break;
            case "中墩炸弹":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhongDunZhaDan.ToString());
                break;
            case "中原一点黑":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhongYuan1DHei.ToString());
                break;
            case "中原一点红":
                Audio_Manage.Instance.Play_Audio(strPath1 + AudioPath_13Z.ZhongYuan1DHong.ToString());
                break;
            default:
                break;
        }
    }


    /// <summary>根据ID 获取特殊牌名字
    /// </summary>
    string TSP_GetName(int cardType)
    {
        TSPCardType tspType = (TSPCardType)cardType;
        string strResult = "";
        switch (tspType)
        {
            case TSPCardType.Type_Null:
                strResult = "";
                break;
            case TSPCardType.ZHI_ZUN_QING_LONG:
                strResult = "至尊青龙";
                break;
            case TSPCardType.YI_TIAO_LONG:
                strResult = "一条龙";
                break;
            case TSPCardType.ALL_BACK:
                strResult = "全黑";
                break;
            case TSPCardType.ALL_HET:
                strResult = "全红";
                break;
            case TSPCardType.ONE_BACK:
                strResult = "中原一点黑";
                break;
            case TSPCardType.ONE_HET:
                strResult = "中原一点红";
                break;
            case TSPCardType.LIU_DUI_BAN:
                strResult = "六对半";
                break;
            case TSPCardType.FOUR_OR_THREE:
                strResult = "4拖3";
                break;
            case TSPCardType.ALL_BIG:
                strResult = "全大";
                break;
            case TSPCardType.ALL_SMALL:
                strResult = "全小";
                break;
            case TSPCardType.SAN_QING:
                strResult = "三清";
                break;
            case TSPCardType.SAN_SANSUN:
                strResult = "三顺";
                break;
            default:
                strResult = "未知牌型";
                break;
        }
        return strResult;
    }

    /// <summary>根据ID 获取比牌每道的名字
    /// </summary>
    string BiPai_GetName(uint cardType)
    {
        OrdinaryCardType biPaiType = (OrdinaryCardType)cardType;
        string strResult = "";
        switch (biPaiType)
        {
            case OrdinaryCardType.San_Card:
                strResult = "乌龙";
                break;
            case OrdinaryCardType.One_Pair:
                strResult = "一对";
                break;
            case OrdinaryCardType.Twe_Pair:
                strResult = "两对";
                break;
            case OrdinaryCardType.Three_Strip:
                strResult = "三条";
                break;
            case OrdinaryCardType.Shun_Zi:
                strResult = "顺子";
                break;
            case OrdinaryCardType.Tong_Hua:
                strResult = "同花";
                break;
            case OrdinaryCardType.Hu_Lu:
                strResult = "葫芦";
                break;
            case OrdinaryCardType.Tie_Zhi:
                strResult = "炸弹";
                break;
            case OrdinaryCardType.Tong_Hua_Shun:
                strResult = "同花顺";
                break;
            case OrdinaryCardType.Ordinary_Type_Null:
                strResult = "普通牌型";
                break;
            default:
                break;
        }
        return strResult;
    }
}

enum OrdinaryCardType
{
    San_Card = 0,              // 十三水 ：：散牌（乌龙）
    One_Pair,                  // 十三水 ：：一对
    Twe_Pair,                  // 十三水 ：：两对
    Three_Strip,               // 十三水 ：：三条
    Shun_Zi,                   // 十三水 ：：顺子
    Tong_Hua,                  // 十三水 ：：同花
    Hu_Lu,                     // 十三水 ：：葫芦
    Tie_Zhi,                   // 十三水 ：：铁枝
    Tong_Hua_Shun,             // 十三水 ：：同花顺
    Ordinary_Type_Null,        // 十三水 ：：普通牌的null牌型
}

enum TSPCardType
{
    Type_Null = 0,             //十三水 ：：无特殊牌型
    ZHI_ZUN_QING_LONG,         //十三水 ：：至尊青龙
    YI_TIAO_LONG,              //十三水 ：：一条龙
    ALL_BACK,                  //十三水 ：：全黑
    ALL_HET,                   //十三水 ：：全红
    ONE_BACK,                  //十三水 ：：中原一点黑
    ONE_HET,                   //十三水 ：：中原一点红
    LIU_DUI_BAN,               //十三水 ：：六对半  
    FOUR_OR_THREE,             //十三水 ：：4拖3 
    ALL_BIG,                   //十三水 ：：全大
    ALL_SMALL,                 //十三水 ：：全小
    SAN_QING,                  //十三水 ：：三清
    SAN_SANSUN,                //十三水 ：：三顺
};

[System.Serializable]
public class Poker13TypeList
{
    public int iIndex_DuiZi, iIndex_ShunZi, iIndex_2Dui,
                iIndex_3Tiao, iIndex_TongHua, iIndex_HuLu,
                iIndex_ZhaDan, iIndex_TongHuaShun, iIndex_TeShuPai;

    public List<List<GameObject>> list_DuiZi = new List<List<GameObject>>();
    public List<List<GameObject>> list_ShunZi = new List<List<GameObject>>();
    public List<List<GameObject>> list_2Dui = new List<List<GameObject>>();
    public List<List<GameObject>> list_3Tiao = new List<List<GameObject>>();
    public List<List<GameObject>> list_TongHua = new List<List<GameObject>>();
    public List<List<GameObject>> list_HuLu = new List<List<GameObject>>();
    public List<List<GameObject>> list_ZhaDan = new List<List<GameObject>>();
    public List<List<GameObject>> list_TongHuaShun = new List<List<GameObject>>();
    public List<List<GameObject>> list_TeShuPai = new List<List<GameObject>>();
    public Poker13TypeList()
    {
        iIndex_DuiZi = 0;
        iIndex_ShunZi = 0;
        iIndex_2Dui = 0;
        iIndex_3Tiao = 0;
        iIndex_TongHua = 0;
        iIndex_HuLu = 0;
        iIndex_ZhaDan = 0;
        iIndex_TongHuaShun = 0;
        iIndex_TeShuPai = 0; ;
    }

    public void ClearAllList()
    {
        list_DuiZi.Clear();
        list_ShunZi.Clear();
        list_2Dui.Clear();
        list_3Tiao.Clear();
        list_TongHua.Clear();
        list_HuLu.Clear();
        list_ZhaDan.Clear();
        list_TongHuaShun.Clear();
        list_TeShuPai.Clear();
    }

    public List<GameObject> GetTypeAry(Poker13Type_XXX pType)
    {
        List<GameObject> listRsult = new List<GameObject>();
        List<List<GameObject>> listMM = new List<List<GameObject>>();
        int iRIndex = 0;
        switch (pType)
        {
            case Poker13Type_XXX.T_DuiZi:
                listMM = list_DuiZi;
                iRIndex = iIndex_DuiZi++;
                if (iIndex_DuiZi >= list_DuiZi.Count)
                {
                    iIndex_DuiZi = 0;
                }
                break;
            case Poker13Type_XXX.T_ShunZi:
                listMM = list_ShunZi;
                iRIndex = iIndex_ShunZi++;
                if (iIndex_ShunZi >= list_ShunZi.Count)
                {
                    iIndex_ShunZi = 0;
                }
                break;
            case Poker13Type_XXX.T_2Dui:
                listMM = list_2Dui;
                iRIndex = iIndex_2Dui++;
                if (iIndex_2Dui >= list_2Dui.Count)
                {
                    iIndex_2Dui = 0;
                }
                break;
            case Poker13Type_XXX.T_3Tiao:
                listMM = list_3Tiao;
                iRIndex = iIndex_3Tiao++;
                if (iIndex_3Tiao >= list_3Tiao.Count)
                {
                    iIndex_3Tiao = 0;
                }
                break;
            case Poker13Type_XXX.T_TongHua:
                listMM = list_TongHua;
                iRIndex = iIndex_TongHua++;
                if (iIndex_TongHua >= list_TongHua.Count)
                {
                    iIndex_TongHua = 0;
                }
                break;
            case Poker13Type_XXX.T_HuLu:
                listMM = list_HuLu;
                iRIndex = iIndex_HuLu++;
                if (iIndex_HuLu >= list_HuLu.Count)
                {
                    iIndex_HuLu = 0;
                }
                break;
            case Poker13Type_XXX.T_ZhaDan:
                listMM = list_ZhaDan;
                iRIndex = iIndex_ZhaDan++;
                if (iIndex_ZhaDan >= list_ZhaDan.Count)
                {
                    iIndex_ZhaDan = 0;
                }
                break;
            case Poker13Type_XXX.T_TongHuaShun:
                listMM = list_TongHuaShun;
                iRIndex = iIndex_TongHuaShun++;
                if (iIndex_TongHuaShun >= list_TongHuaShun.Count)
                {
                    iIndex_TongHuaShun = 0;
                }
                break;
            case Poker13Type_XXX.T_TeShuPai:
                listMM = list_TeShuPai;
                iRIndex = iIndex_TeShuPai++;
                if (iIndex_TeShuPai >= list_TeShuPai.Count)
                {
                    iIndex_TeShuPai = 0;
                }
                break;
            default:
                break;
        }
        if (iRIndex >= listMM.Count)
        {
            iRIndex = 0;
        }
        if (listMM.Count < 1)
        {
            return listRsult;
        }
        else
        {
            listRsult = listMM[iRIndex];
            //return listMM[iRIndex];
        }

        return listRsult;
    }

    public int GetTypeAryCount(Poker13Type_XXX pType)
    {
        List<List<GameObject>> listMM = new List<List<GameObject>>();
        //int iRIndex = 0;
        switch (pType)
        {
            case Poker13Type_XXX.T_DuiZi:
                listMM = list_DuiZi;

                break;
            case Poker13Type_XXX.T_ShunZi:
                listMM = list_ShunZi;

                break;
            case Poker13Type_XXX.T_2Dui:
                listMM = list_2Dui;

                break;
            case Poker13Type_XXX.T_3Tiao:
                listMM = list_3Tiao;

                break;
            case Poker13Type_XXX.T_TongHua:
                listMM = list_TongHua;

                break;
            case Poker13Type_XXX.T_HuLu:
                listMM = list_HuLu;

                break;
            case Poker13Type_XXX.T_ZhaDan:
                listMM = list_ZhaDan;

                break;
            case Poker13Type_XXX.T_TongHuaShun:
                listMM = list_TongHuaShun;

                break;
            case Poker13Type_XXX.T_TeShuPai:
                listMM = list_TeShuPai;

                break;
            default:
                break;
        }
        return listMM.Count;
    }

}

public class GameData_13Z
{
    public uint maPaiCard;
    public bool isHuPai = false;

    public bool isTeShuPai = false;
    public GameData_13Z()
    { }
}

public enum Poker13Type_XXX
{
    T_DuiZi, T_ShunZi, T_2Dui,
    T_3Tiao, T_TongHua, T_HuLu,
    T_ZhaDan, T_TongHuaShun, T_TeShuPai
}

/// <summary>游戏中规则信息
/// </summary>
[System.Serializable]
public class InGameRuleInfo
{
    /// <summary>【拖拽赋值】显示房间号
    /// </summary>
    public Text tRoomNumber;

    /// <summary>【拖拽赋值】显示房间规则
    /// </summary>
    public Text tRoomRuleInfo;

    public InGameRuleInfo()
    { }
}