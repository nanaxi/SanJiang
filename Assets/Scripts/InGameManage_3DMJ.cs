using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InGameManage_3DMJ : MonoBehaviour, IFCMjGameNet_OpenMayOperation, IFCMjGameNet_Update
{

    public Camera _camera;
    public Transform ray_MjSelect;

    public Transform t_ShouPaiParent;
    public InGame_PlayerMJ[] allPlayerMJAry;//代表一个玩家在操作麻将    
    private List<uint> listStartHandCards = new List<uint>();//用来记录服务器发送过来的手牌
    [SerializeField]
    private int paiQ_Count;
    private List<GameObject> listPaiQ_GmAry = new List<GameObject>();//用来获取所有的牌墙墩，方便删除
    private TouZi_Rotate[] touZiAry;
    [SerializeField]
    private Mj_ClickHight mjClickM;//关于麻将点击用到的变量
    public const float clickMjHight_Y = 0.04f;//麻将点击的高度
    [SerializeField]
    public MJGameC_Info mjGameInfo_M;//记录可以碰杠胡的值，以及OriCharId;
    [SerializeField]
    private bool yn_KeYiChuPai = false;//是否可以出牌
    public bool isInitSetVar = false;
    public GameObject[] dirLight = new GameObject[4];//方向指针
    public int i_IsInitDIR = 0;
    private bool isPromptIP = false;
    public Transform t_ChuPaiZhiZhen;
    public Transform t_BorderGm, t_BorderPosition1, t_BorderPosition2;//麻将牌墙的边界,拖拽赋值   
    [SerializeField]
    private InputMouse_ inputMD;//关于获取鼠标开始点着移动
    /// <summary>关于麻将拖拽排序
    /// </summary>
    private MjDragRank mjDragRank_C = new MjDragRank();

    [SerializeField]
    private BaoTingCCC baotingC = new BaoTingCCC();

    private bool isAlign;
    /// <summary>是否自动对齐
    /// </summary>
    public bool IsAlign
    {
        set
        {
            if (value)
            {
                LocaHandCardsSort();
            }

            isAlign = value;
            
        }
        get { return isAlign; }
    }

    private bool isFeiPeng;
    /// <summary>是否飞碰
    /// </summary>
    public bool IsFeiPeng
    {
        set { isFeiPeng = value; }
        get
        {
            if (inGm_UI != null)
            {
                isFeiPeng = inGm_UI.tog_FeiPeng.isOn;
            }
            return isFeiPeng;
        }
    }

    int ZhuangJiaid = -1;          //庄家 0是自己
    bool FirstChupai = true;     //第一个出牌 要等服务器询问过后 开启出牌功能
    uint zhuangJia_Id = 0;
    private IFCMjGameNet_OpenMayOperation _iFC_OpenUI;
    private IFCMjGameNet_Update _iFC_Update;

    #region/*———特效———*/
    [SerializeField]
    private Transform t_PSPosition;// = new Transform[4];
    //GameObject light_Environmental;
    #endregion
    #region/*———材质球———*/
    public Material fxLight_An;
    public Material fxLight_Liang;
    public Material locaHandCard_Que;
    #endregion
    #region/*———麻将UI———*/
    private InGameManage_3DMjUI inGm_UI;

    #endregion


    // Use this for initialization
    void Awake()
    {
        if (!isInitSetVar)
        {
            Init_Game();
        }

    }

    public void Init_Game()
    {
        isInitSetVar = false;
        _iFC_OpenUI = this;
        _iFC_Update = this;
        //mj_Distance = Vector3.Distance(t_ShouPaiParent.GetChild(0).position, t_ShouPaiParent.GetChild(1).position);
        inGm_UI = UIManager.Instance.ShowUI(AllPrefabName.uiWin_InGame_MJ, UIManager.Instance.canvas_T).GetComponent<InGameManage_3DMjUI>();
        inGm_UI.transform.SetAsLastSibling();
        inGm_UI._inGameM_3DMJ = this;
        inGm_UI.gameObject.SetActive(true);
        inGm_UI.StartSetRoomTime();
        isAlign = inGm_UI.tog_ZDDQ.isOn;
        isFeiPeng = inGm_UI.tog_FeiPeng.isOn;
        inGm_UI.uiWin_End1_MJ.btn_End1Setout.onClick.AddListener(delegate () { OnClick_Btn_End1Setout(); });
        uint hzCardId = 53;
        DataManage.Instance.MJSX_Add(hzCardId.ToCard());
        //inGm_UI.svMJDrag.onDragStart = null;
        //inGm_UI.svMJDrag.onDragStart = delegate () { this.OnDrag_MJStart(); };
        //inGm_UI.svMJDrag.onDragEnd = null;
        //inGm_UI.svMJDrag.onDragEnd = delegate () { this.OnDrag_MJEnd(); };

        Net_SetEvent();
        DataManage.Instance.onChangePlayerData += this.UpdatePlayerUi;

        mjClickM = new Mj_ClickHight();
        touZiAry = transform.FindChild("TouZi_Ary").GetComponentsInChildren<TouZi_Rotate>();
        t_ChuPaiZhiZhen = transform.FindChild("YouBiao");
        t_ChuPaiZhiZhen.gameObject.SetActive(false);
        allPlayerMJAry = transform.FindChild("PlayerAry").GetComponentsInChildren<InGame_PlayerMJ>();


        StartCoroutine(VarIsInitOver());
        isInitSetVar = true;
    }

    void OnDestroy()
    {
        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_InGame_MJ);
        Net_RemoveEvent();
        DataManage.Instance.onChangePlayerData -= this.UpdatePlayerUi;
    }

    IEnumerator VarIsInitOver()
    {
        //yield return new WaitForEndOfFrame();
        //light_Environmental.SetActive(true);

        int i_Count1 = 0;
        while (i_Count1 != 4)
        {//IF Need Script Init Over
            for (int i = 0; i < 4; i++)
            {
                i_Count1 += allPlayerMJAry[i].isInit ? 1 : 0;
            }

            if (i_Count1 != 4)
            {
                i_Count1 = 0;
                yield return new WaitForEndOfFrame();
            }
        }

        //Test Create Start MJ
        //Test_StartSp();
        //Test_MopaiAdd();

        string[] testCardName = new string[] {
            "1W","2W","3W","4W","5W","6W","7W","8W","9W",
            "1T","2T","3T","4T","5T","6T","7T","8T","9T",
            "1B","2B","3B","4B","5B","6B","7B","8B","9B"
        };

        //for (int i = 0; i < testCardName.Length; i++)
        //{//Test Create ChuPai
        //    for (int i1 = 0; i1 < allPlayerMJAry.Length; i1++)
        //    {
        //        allPlayerMJAry[i1].ChuPai_ADD(testCardName[i]);
        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}

        yield return null;
    }

    public void Test_StartSp()
    {
        List<uint> testSpList = new List<uint>();
        for (int i = 0; i < 14; i++)
        {
            testSpList.Add(DataManage.Instance.mjCardId_Ary[UnityEngine.Random.Range(0, 27)]);
        }
        _iFC_Update.UpdateStartHandCards(testSpList, 0);
    }

    void Test_MopaiAdd()
    {
        //allPlayerMJAry[0].MoPai_Add_MY(DataManage.Instance.MJSX_Get(6));
        //allPlayerMJAry[1].MoPai_Add_OtherPlayer(DataManage.Instance.MJSX_Get(6));
        //allPlayerMJAry[2].MoPai_Add_OtherPlayer(DataManage.Instance.MJSX_Get(6));
        //allPlayerMJAry[3].MoPai_Add_OtherPlayer(DataManage.Instance.MJSX_Get(6));//测试向摸牌添加牌面
        allPlayerMJAry[1].MJ_Op_Other(MJ_OpType.PengPai, new Mj_Sx_(1, 1, "1W"));
        allPlayerMJAry[2].MJ_Op_Other(MJ_OpType.PengPai, new Mj_Sx_(1, 1, "1W"));
        allPlayerMJAry[3].MJ_Op_Other(MJ_OpType.PengPai, new Mj_Sx_(1, 1, "1W"));

        allPlayerMJAry[1].MJ_Op_Other(MJ_OpType.MingGang, new Mj_Sx_(1, 2, "2W"));
        allPlayerMJAry[2].MJ_Op_Other(MJ_OpType.MingGang, new Mj_Sx_(1, 2, "2W"));
        allPlayerMJAry[3].MJ_Op_Other(MJ_OpType.MingGang, new Mj_Sx_(1, 2, "2W"));

        allPlayerMJAry[1].MJ_Op_Other(MJ_OpType.AnGang, new Mj_Sx_(1, 3, "3W"));
        allPlayerMJAry[2].MJ_Op_Other(MJ_OpType.AnGang, new Mj_Sx_(1, 3, "3W"));
        allPlayerMJAry[3].MJ_Op_Other(MJ_OpType.AnGang, new Mj_Sx_(1, 3, "3W"));

        allPlayerMJAry[1].ShouPai_Destroy_Other(8, false);
        allPlayerMJAry[1].ShouPai_Rank_OtherPlayer();

    }
    public void Net_SetEvent()
    {
        PublicEvent.GetINS.Event_recivePlayerReady += inGm_UI.Open_Player_ZhunBei;
        PublicEvent.GetINS.Event_reciveGetFirstCards += _iFC_Update.UpdateStartHandCards;
        PublicEvent.GetINS.Event_ReciveOtherPopCard += _iFC_Update.UpdateChuPai;
        PublicEvent.GetINS.Event_DirLight += Update_ZhiZhen_R;
        PublicEvent.GetINS.Event_reciveGetCard += _iFC_Update.UpdateMopai;
        PublicEvent.GetINS.Event_ZhuangChuDiYiZhang = ZhuangFirstChupai;
        PublicEvent.GetINS.Event_reciveZhuang += ReserveZhuangJia;
        PublicEvent.GetINS.Event_KeYiPeng += _iFC_OpenUI.Mj_OpenMay_Peng;// Mj_Open_Peng;
        PublicEvent.GetINS.Event_KeYiGang += _iFC_OpenUI.Mj_OpenMay_Gang;// Mj_Open_Gang;
        PublicEvent.GetINS.Event_KeYiHu += _iFC_OpenUI.Mj_OpenMay_Hu;// Mj_Open_Hu;
        PublicEvent.GetINS.Event_ReciveRoundOverResult += End_YiJu;

        PublicEvent.GetINS.Fun_UpdatePaishu += inGm_UI.Set_ShengYu_PaiShu;
        PublicEvent.GetINS.Event_ReUpdateMj += this.ReUpdateMJ;
        //Debug.Log("<color=red>执行设置委托了吗？？？？？</color>");
        PublicEvent.GetINS.Event_ReciveOtherCanPlay += IPengGangHu;  //服务器广播谁碰杠胡了

        PublicEvent.GetINS.Event_RecivePlayer_VChat += Chat_V_P;

        PublicEvent.GetINS.Event_reciveGetIsOnLine += inGm_UI.SetIsOnLine;

        PublicEvent.GetINS.Event_KeYiBao += Mj_OpenMay_BaoPai;
        PublicEvent.GetINS.Event_KeYiFei += Mj_OpenMay_FeiPeng;
        PublicEvent.GetINS.Event_KeYiTiPai += Mj_OpenMay_TiPai;

        PublicEvent.GetINS.Event_reciveSelectQue += UpdateXuanQue;
    }
    public void Net_RemoveEvent()
    {
        PublicEvent.GetINS.Event_recivePlayerReady -= inGm_UI.Open_Player_ZhunBei;
        PublicEvent.GetINS.Event_reciveGetFirstCards -= _iFC_Update.UpdateStartHandCards;
        PublicEvent.GetINS.Event_ReciveOtherPopCard -= _iFC_Update.UpdateChuPai;
        PublicEvent.GetINS.Event_DirLight -= Update_ZhiZhen_R;
        PublicEvent.GetINS.Event_reciveGetCard -= _iFC_Update.UpdateMopai;
        PublicEvent.GetINS.Event_ZhuangChuDiYiZhang = null;// ZhuangFirstChupai;
        PublicEvent.GetINS.Event_reciveZhuang -= ReserveZhuangJia;
        PublicEvent.GetINS.Event_KeYiPeng -= _iFC_OpenUI.Mj_OpenMay_Peng;// Mj_Open_Peng;
        PublicEvent.GetINS.Event_KeYiGang -= _iFC_OpenUI.Mj_OpenMay_Gang;// Mj_Open_Gang;
        PublicEvent.GetINS.Event_KeYiHu -= _iFC_OpenUI.Mj_OpenMay_Hu;// Mj_Open_Hu;
        PublicEvent.GetINS.Event_ReciveRoundOverResult -= End_YiJu;
        PublicEvent.GetINS.Fun_UpdatePaishu -= inGm_UI.Set_ShengYu_PaiShu;
        PublicEvent.GetINS.Event_ReUpdateMj -= this.ReUpdateMJ;
        //Debug.Log("<color=red>执行设置委托了吗？？？？？</color>");
        PublicEvent.GetINS.Event_ReciveOtherCanPlay -= IPengGangHu;  //服务器广播谁碰杠胡了

        PublicEvent.GetINS.Event_RecivePlayer_VChat -= Chat_V_P;

        PublicEvent.GetINS.Event_KeYiBao -= Mj_OpenMay_BaoPai;
        PublicEvent.GetINS.Event_KeYiFei -= Mj_OpenMay_FeiPeng;
        PublicEvent.GetINS.Event_KeYiTiPai -= Mj_OpenMay_TiPai;

        PublicEvent.GetINS.Event_reciveSelectQue -= UpdateXuanQue;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRayClick();

    }

    /// <summary>关于Ray射线检测点击麻将
    /// </summary>
    void UpdateRayClick()
    {
        #region/* Ray*/
        if (Input.GetMouseButtonDown(0))
        {//测试麻将拖拽
            inputMD = new InputMouse_(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            if (inputMD.isDragStart == false && Vector2.Distance(Input.mousePosition, inputMD.v2_DownPosition) >= InputMouse_.i_MayDragDistance)
            {
                Debug.Log("可以开始滑动了？");
                inputMD.isDragStart = true;
                Ray rayDown = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
                RaycastHit hitInfoDown;
                if (Physics.Raycast(rayDown, out hitInfoDown))
                {
                    GameObject gameObj = hitInfoDown.collider.gameObject;
                    //Debug.Log("click object name is " + gameObj.name);
                    Transform ray_MjDown = gameObj.transform;
                    if (ray_MjDown.parent == t_ShouPaiParent && ray_MjDown != null
                        && ray_MjDown.GetComponent<MJInfos>() != null
                        && ray_MjDown.transform.childCount > 0
                        )
                    {
                        mjClickM.mj_DownDrag_Object = ray_MjDown.GetChild(0);
                        Debug.Log("click object name is " + gameObj.name + "MeshName" + mjClickM.mj_DownDrag_Object.GetComponent<MeshRenderer>().material.name);
                        if (allPlayerMJAry[0].DragRank_IFEqualMoPai(ray_MjDown.gameObject) == false && yn_KeYiChuPai == false)
                        {
                            //inGm_UI.svMJDrag.image_My.raycastTarget = false;
                            mjDragRank_C.DragObj = ray_MjDown.gameObject;
                            mjDragRank_C.DragObj_Index = allPlayerMJAry[0].DragRank_GetIndex(ray_MjDown.gameObject);
                            mjClickM.mj_DownDrag_Object = ray_MjDown.GetChild(0);
                            mjClickM.mj_DownDrag_Object.parent = ray_MjDown.parent;
                        }
                        else if (yn_KeYiChuPai)
                        {
                            //inGm_UI.svMJDrag.image_My.raycastTarget = true;
                            mjClickM.mj_DownDrag_Object = ray_MjDown.GetChild(0);
                        }

                    }
                    else
                    {
                        mjClickM.mj_DownDrag_Object = null;
                    }
                }
                else
                {
                    mjClickM.mj_DownDrag_Object = null;
                }
            }

            if (inputMD.isDragStart && mjClickM.mj_DownDrag_Object != null)
            {//测试麻将拖拽
                Ray rayDown = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
                RaycastHit hitInfoDown;
                if (Physics.Raycast(rayDown, out hitInfoDown))
                {
                    GameObject gameObj = hitInfoDown.collider.gameObject;
                    //Debug.Log("click object name is " + gameObj.name);
                    Transform ray_MjDown = gameObj.transform;
                    if (yn_KeYiChuPai == false)
                    {
                        mjClickM.mj_DownDrag_Object.position = hitInfoDown.point;
                        if (ray_MjDown.parent == t_ShouPaiParent && ray_MjDown != null
                        && ray_MjDown.GetComponent<MJInfos>() != null
                        && ray_MjDown.transform.childCount > 0)
                        {
                            if (allPlayerMJAry[0].DragRank_IFEqualMoPai(ray_MjDown.gameObject) == false
                            && mjClickM.mj_DownDrag_Object != null
                            && !IsAlign
                            )
                            {
                                mjDragRank_C.DragTargetObj = ray_MjDown.gameObject;
                                mjDragRank_C.DragTargetObj_Index = allPlayerMJAry[0].DragRank_GetIndex(ray_MjDown.gameObject);
                                if (mjDragRank_C.DragObj_Index != mjDragRank_C.DragTargetObj_Index)
                                {
                                    mjDragRank_C.DragObj_Index = allPlayerMJAry[0].DragRank_MoveOtherMJ(mjDragRank_C.DragObj_Index, mjDragRank_C.DragTargetObj_Index);
                                }
                            }
                        }
                    }

                    if (mjClickM.mj_DownDrag_Object != null && yn_KeYiChuPai == true)
                    {
                        mjClickM.mj_DownDrag_Object.position = hitInfoDown.point;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (yn_KeYiChuPai)
            {//关于麻将拖动出牌 测试
                if (mjClickM.mj_DownDrag_Object != null)
                {
                    if (mjClickM.mj_DownDrag_Object.GetComponentInParent<MJInfos>().MjINFO_.mJCardID != 53 &&
                        Input.mousePosition.y > Screen.height / 3
                        && Que_IsThere(mjClickM.mj_DownDrag_Object.gameObject))
                    {
                        if (inGm_UI.needMJ_Ui.IsShowBtn) // !inGm_UI.needMJ_Ui.jl_PointTog[1] && !inGm_UI.needMJ_Ui.jl_PointTog[2] && !inGm_UI.needMJ_Ui.jl_PointTog[3] && !inGm_UI.needMJ_Ui.jl_PointTog[4])
                        {
                            if (mjClickM.mj_DownDrag_Object.GetComponentInParent<MJInfos>().MjINFO_.mJCardID == 0)
                            {//防止报0错误
                                mjClickM.mj_DownDrag_Object.GetComponentInParent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(mjClickM.mj_DownDrag_Object.transform.GetChild(0).gameObject.name);
                            }
                            Mj_Sx_ mjSx = mjClickM.mj_DownDrag_Object.GetComponentInParent<MJInfos>().MjINFO_;
                            if (mjClickM.mj_Select_Object != null)
                            {
                                mjClickM.Mj_Click_Move(false);
                            }
                            MJ_Drag3D_InitPosition();
                            //Loca_UpdateChuPai(DataManage.Instance.MyPlayer_Data.p_ID, mjSx.mJCardID);
                            PublicEvent.GetINS.Fun_SentPopCard(mjSx.mJCardID);//gameObj.GetComponent<MJInfos>().MjINFO_.mJCardID);

                            mjClickM.mj_Select_Object = null;
                            yn_KeYiChuPai = false;
                        }
                        else {
                            MJ_Drag3D_Init();
                        }
                    }
                    else
                    {
                        MJ_Drag3D_Init();
                    }
                }
            }
            else
            {
                //if (mjClickM.mj_DownDrag_Object != null)
                //{
                //    MJ_Drag3D_Init();
                //}
                if (mjClickM.mj_DownDrag_Object != null && yn_KeYiChuPai == false)
                {
                    allPlayerMJAry[0].DragRank_EndSetParent(mjDragRank_C.DragObj_Index, mjClickM.mj_DownDrag_Object.gameObject);
                    mjDragRank_C.Init_();
                    mjClickM.mj_DownDrag_Object = null;
                }
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到
                GameObject gameObj = hitInfo.collider.gameObject;
                Debug.Log("click object name is " + gameObj.name);
                ray_MjSelect = gameObj.transform;
                if (ray_MjSelect.parent == t_ShouPaiParent && ray_MjSelect != null
                    && ray_MjSelect.GetComponent<MJInfos>() != null
                    && ray_MjSelect.transform.childCount > 0
                    )
                {
                    /***/
                    //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Card_Click.ToString()));
                    if (yn_KeYiChuPai)
                    {
                        if (mjClickM.mj_Select_Object != null)
                        {
                            if (mjClickM.mj_Select_Object.gameObject != gameObj)
                            {
                                mjClickM.Mj_Click_Move(false);
                                mjClickM = new Mj_ClickHight(ray_MjSelect, clickMjHight_Y);
                                mjClickM.Mj_Click_Move(true);
                            }
                            else
                            {
                                if (gameObj.GetComponentInParent<MJInfos>().MjINFO_.mJCardID != 53
                                    && Que_IsThere(gameObj.transform.GetChild(0).gameObject)
                                    && inGm_UI.needMJ_Ui.IsShowBtn )//!inGm_UI.needMJ_Ui.jl_PointTog[1] && !inGm_UI.needMJ_Ui.jl_PointTog[2] && !inGm_UI.needMJ_Ui.jl_PointTog[3] && !inGm_UI.needMJ_Ui.jl_PointTog[4])
                                {
                                    if (gameObj.GetComponent<MJInfos>().MjINFO_.mJCardID == 0)
                                    {//防止报0错误
                                        gameObj.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(gameObj.transform.GetChild(0).gameObject.name);
                                    }
                                    Mj_Sx_ mjSx = gameObj.GetComponent<MJInfos>().MjINFO_;
                                    mjClickM.Mj_Click_Move(false);
                                    MJ_Drag3D_InitPosition();
                                    //Loca_UpdateChuPai(DataManage.Instance.MyPlayer_Data.p_ID, mjSx.mJCardID);
                                    PublicEvent.GetINS.Fun_SentPopCard(mjSx.mJCardID);//gameObj.GetComponent<MJInfos>().MjINFO_.mJCardID);

                                    mjClickM.mj_Select_Object = null;
                                    yn_KeYiChuPai = false;
                                }
                            }
                        }
                        if (mjClickM.mj_Select_Object == null)
                        {
                            mjClickM = new Mj_ClickHight(gameObj.transform, clickMjHight_Y);
                            mjClickM.Mj_Click_Move(true);
                        }
                    }
                    else
                    {
                        if (mjClickM.mj_Select_Object != null)
                        {
                            if (mjClickM.mj_Select_Object.gameObject != gameObj)
                            {
                                mjClickM.Mj_Click_Move(false);
                                mjClickM = new Mj_ClickHight(gameObj.transform, clickMjHight_Y);
                                mjClickM.Mj_Click_Move(true);
                            }
                            else
                            {
                                //mjClickM.Mj_Click_Move(false);
                                mjClickM.Mj_Click_Move(!mjClickM.isSelect);
                            }
                        }
                        if (mjClickM.mj_Select_Object == null)
                        {
                            mjClickM = new Mj_ClickHight(gameObj.transform, clickMjHight_Y);
                            mjClickM.Mj_Click_Move(true);
                        }
                    }
                }
            }
        }
        #endregion
    }

    void MJ_Drag3D_Init()
    {
        if (mjClickM.mj_DownDrag_Object != null)
        {
            mjClickM.mj_DownDrag_Object.transform.position = mjClickM.mj_DownDrag_Object.transform.parent.position;
            mjClickM.mj_DownDrag_Object = null;
        }
    }
    void MJ_Drag3D_InitPosition()
    {
        if (mjClickM.mj_DownDrag_Object != null)
        {
            mjClickM.mj_DownDrag_Object.transform.position = mjClickM.mj_DownDrag_Object.transform.parent.position;
        }
    }
    /// <summary>麻将拖动开始
    /// </summary>
    void OnDrag_MJStart()
    {
        //if (mjClickM.mj_DownDrag_Object!=null)
        //{
        //    //inGm_UI.svMJDrag.sVRect.content.anchoredPosition = Input.mousePosition;
        //    inGm_UI.svMJDrag.img_MJ.sprite = Resources.Load<Sprite>("Sprites/NingXiaHuaShui/MJUI/paimian/"+mjClickM.mj_DownDrag_Object.gameObject.name);
        //    if (inGm_UI.svMJDrag.img_MJ.sprite!=null)
        //    {
        //        inGm_UI.svMJDrag.sVRect.content.gameObject.SetActive(true);
        //        mjClickM.mj_DownDrag_Object.GetComponentInChildren<MeshRenderer>().enabled = false;
        //        //inGm_UI.svMJDrag.sVRect.content.anchoredPosition = Input.mousePosition;
        //    }
        //}
    }
    /// <summary>麻将拖动结束
    /// </summary>
    void OnDrag_MJEnd()
    {
        //if (inGm_UI.svMJDrag.sVRect.content.gameObject.activeInHierarchy)
        //{
        //    inGm_UI.svMJDrag.img_MJ.sprite = null;
        //    inGm_UI.svMJDrag.sVRect.content.gameObject.SetActive(false);
        //    if (mjClickM.mj_DownDrag_Object != null && mjClickM.mj_DownDrag_Object.GetComponentInChildren<MeshRenderer>()!=null)
        //    {
        //        mjClickM.mj_DownDrag_Object.GetComponentInChildren<MeshRenderer>().enabled = true;
        //    }

        //    if (inGm_UI.svMJDrag.sVRect.content.localScale.x <= 1.5f+0.1f &&
        //        inGm_UI.svMJDrag.sVRect.content.localScale.x >= 1.5f - 0.1f)
        //    {
        //        Debug.Log("你想出牌？？？" + mjClickM.mj_DownDrag_Object.gameObject.name);
        //        if (mjClickM.mj_DownDrag_Object.GetComponent<MJInfos>().MjINFO_.mJCardID != 53 && yn_KeYiChuPai && mjClickM.mj_DownDrag_Object != null && mjClickM.mj_DownDrag_Object.GetComponent<MJInfos>()!= null)
        //        {
        //            if (!inGm_UI.needMJ_Ui.jl_PointTog[1] && !inGm_UI.needMJ_Ui.jl_PointTog[2] && !inGm_UI.needMJ_Ui.jl_PointTog[3] && !inGm_UI.needMJ_Ui.jl_PointTog[4])
        //            {
        //                if (mjClickM.mj_DownDrag_Object.GetComponent<MJInfos>().MjINFO_.mJCardID == 0)
        //                {//防止报0错误
        //                    mjClickM.mj_DownDrag_Object.GetComponent<MJInfos>().MjINFO_ = DataManage.Instance.MJSX_Get(mjClickM.mj_DownDrag_Object.transform.GetChild(0).gameObject.name);
        //                }

        //                PublicEvent.GetINS.Fun_SentPopCard(mjClickM.mj_DownDrag_Object.GetComponent<MJInfos>().MjINFO_.mJCardID);
        //                if (mjClickM.mj_Select_Object != null)
        //                {
        //                    mjClickM.Mj_Click_Move(false);
        //                    mjClickM.mj_Select_Object = null;
        //                }
        //                mjClickM.mj_DownDrag_Object = null;
        //                yn_KeYiChuPai = false;
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>当程序焦点
    /// </summary>
    void OnApplicationFocus()
    {
        if (mjClickM.mj_DownDrag_Object != null)
        {
            allPlayerMJAry[0].DragRank_EndSetParent(mjDragRank_C.DragObj_Index, mjClickM.mj_DownDrag_Object.gameObject);
            mjDragRank_C.Init_();
            mjClickM.mj_DownDrag_Object = null;
        }
        OnDrag_MJEnd();
    }

    #region/*———杂项———*/


    public void OpenOrClose_ChuPai_ZhiZhe(Transform t_ = null)
    {
        if (t_ != null)
        {
            //needMJ_Ui.t_ChuPai_ZhiZhen.SetParent(t_);
            t_ChuPaiZhiZhen.SetParent(t_);

            t_ChuPaiZhiZhen.localPosition = Vector3.zero;
            t_ChuPaiZhiZhen.localPosition = Vector3.zero;
            Vector3 v3Position_1 = t_ChuPaiZhiZhen.position;
            v3Position_1.y += 0.02f;//指针变高一点
            t_ChuPaiZhiZhen.position = v3Position_1;

            t_ChuPaiZhiZhen.localScale = Vector3.one;
            t_ChuPaiZhiZhen.gameObject.SetActive(true);
            //waitTime_1 = Time.deltaTime;
            StartCoroutine(WaitTimeInveke_Event(
                delegate ()
                {
                    t_ChuPaiZhiZhen.SetParent(transform);
                    t_ChuPaiZhiZhen.localScale = Vector3.one;
                    //t_ChuPaiZhiZhen.transform.SetSiblingIndex(transform.childCount / 2);
                }, Time.deltaTime
                ));
        }
        else
        {
            t_ChuPaiZhiZhen.SetParent(transform);
            t_ChuPaiZhiZhen.gameObject.SetActive(false);
        }
    }

    //float waitTime_1 = 0;
    IEnumerator WaitTimeInveke_Event(UnityEngine.Events.UnityAction event_, float waitTime_1 = 0)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(waitTime_1);
        event_.Invoke();
    }

    #endregion
    #region /*———麻将具体操作具体———*/

    IEnumerator Game_StartInit()
    {
        //t_BorderMoveGoal = t_BorderPosition1;
        float f_MoveTime = 1.0f;//最大移动时间， 防止卡住
        while (Vector3.Distance(t_BorderGm.position, t_BorderPosition1.position) > 0.005f && f_MoveTime > 0.0f)
        {//麻将桌边界移向桌子底下
            t_BorderGm.position = Vector3.MoveTowards(t_BorderGm.position, t_BorderPosition1.position, (Time.deltaTime * 3));
            yield return new WaitForSeconds(0.1f);
            f_MoveTime -= 0.1f;
        }
        f_MoveTime = 1.0f;//下面以上桌子还需要最大移动时间
        t_BorderGm.position = t_BorderPosition1.position;
        Debug.Log("Invoke_1");
        bool isFengPai = DataManage.Instance.RoomInfoNxStr.IndexOf("不带风牌") >= 0;

        if (touZiAry.Length == 2 && DataManage.Instance.GetRoomTouZi.Length == 2)
        {
            touZiAry[0].TouZiStartRotate(DataManage.Instance.GetRoomTouZi[0]);
            touZiAry[1].TouZiStartRotate(DataManage.Instance.GetRoomTouZi[1]);
        }

        yield return new WaitForSeconds(0.1f);

        List<List<GameObject>> listAllPaiQ = new List<List<GameObject>>();
        //生成牌墙
        if (isFengPai)
        {//不带风牌,牌墙108张牌公式 ：((13+13)* 2)+ ((14+14)* 2) = 108
            int i_Position = (int)DataManage.Instance.MyPlayer_Data.position;
            //i_Position = i_Position % 2 == 1 ? 26 : 28 ;
            listAllPaiQ.Add(allPlayerMJAry[0].PaiQ_InitShow(i_Position % 2 == 1 ? 26 : 28));
            listAllPaiQ.Add(allPlayerMJAry[1].PaiQ_InitShow(i_Position % 2 != 1 ? 26 : 28));
            listAllPaiQ.Add(allPlayerMJAry[2].PaiQ_InitShow(i_Position % 2 == 1 ? 26 : 28));
            listAllPaiQ.Add(allPlayerMJAry[3].PaiQ_InitShow(i_Position % 2 != 1 ? 26 : 28));
        }
        else
        {//带风牌,牌墙136张牌公式 ：(17+17) * 2 = 108

            listAllPaiQ.Add(allPlayerMJAry[0].PaiQ_InitShow(34));
            listAllPaiQ.Add(allPlayerMJAry[1].PaiQ_InitShow(34));
            listAllPaiQ.Add(allPlayerMJAry[2].PaiQ_InitShow(34));
            listAllPaiQ.Add(allPlayerMJAry[3].PaiQ_InitShow(34));
        }
        //PaiQ_SetList(listAllPaiQ);//设置牌墙列表
        //paiQ_Count = listPaiQ_GmAry.Count;//设置牌墙列表个数

        yield return new WaitForSeconds(0.1f);

        Debug.Log("Invoke_2");
        while (Vector3.Distance(t_BorderGm.position, t_BorderPosition2.position) > 0.005f && f_MoveTime > 0.0f)
        {//麻将桌边界移向桌子上面
            t_BorderGm.position = Vector3.MoveTowards(t_BorderGm.position, t_BorderPosition2.position, (Time.deltaTime * 3));
            yield return new WaitForSeconds(0.1f);
            f_MoveTime -= 0.1f;
        }
        t_BorderGm.position = t_BorderPosition2.position;
        Debug.Log("Invoke_3");
        MJ_Operation_StartHandCards(listStartHandCards, 0);

        DataManage.Instance.roomCardNumCount = (isFengPai ? 108 : 136) - (13 * 4 + 1);
        yield return null;
    }


    void MJ_Operation_StartHandCards(List<uint> all_SP, int seat)
    {
        //if (DataManage.Instance.RoomInfoNxStr.IndexOf("缺")>=0)
        //{
        inGm_UI.gmXuanQue_Bg.SetActive(DataManage.Instance.RoomInfoNxStr.IndexOf("缺") >= 0);
        //}
        paiQ_Count = listPaiQ_GmAry.Count;
        inGm_UI.needMJ_Ui.Prep_CloseOKStyleAll();
        FirstChupai = true;
        DataManage.Instance.MyPlayer_Data.isgaming = true;
        mjGameInfo_M.yn_EndReducePaiQ = false;
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
                        strIpPrompt += "[" + all_Player_Data[i].P_Name + "]和[" + all_Player_Data[i_1].P_Name + "]玩家在同一IP！！！\n";
                        break;
                    }
                }
            }
        }
        if (strIpPrompt.Length > 0 && isPromptIP == false)
        {
            UiWin_Prompt.OpenPrompt("同一IP提醒：\n" + strIpPrompt);
            isPromptIP = true;
            Debug.Log("【IP：】" + strIpPrompt);
        }

        //PaiQ_Reduce(13);
        //PaiQ_Reduce(13);
        //PaiQ_Reduce(14);
        //Gm_Manager.G_M.Open_Prompt(
        if (seat == 0)
        {
            all_SP.Sort();

            for (int i = 0; i < all_SP.Count; i++)
            {
                DataManage.Instance.MJSX_Add(all_SP[i].ToCard());
                if (i == 13)
                {
                    //string mjName1 = NeedDL.DicValueGetKey<string, int>(DataManage.Instance.mjCardId_Ary[Random.Range(0, 34)], DataManage.Instance.dicMJSize);
                    allPlayerMJAry[0].MoPai_Add_MY(DataManage.Instance.MJSX_Get(all_SP[i]));
                    inGm_UI.tog_ZDDQ.isOn = true;
                    inGm_UI.tog_ZDDQ.interactable = false;
                }
                else
                {
                    allPlayerMJAry[0].ShouPai_Add_My(DataManage.Instance.MJSX_Get(all_SP[i]));
                    PaiQ_Reduce(1);
                }

            }

            for (int i = 1; i < allPlayerMJAry.Length; i++)
            {//本地显示其他玩家的手牌
                for (int i_1 = 0; i_1 < 13; i_1++)
                {
                    PaiQ_Reduce(1);
                    allPlayerMJAry[i].ShouPai_Add_OtherPlayer(new Mj_Sx_());
                }
                if (DataManage.Instance.PData_GetSeatID(i) == zhuangJia_Id)
                {
                    allPlayerMJAry[i].MoPai_Add_OtherPlayer(new Mj_Sx_(1, 1, "1W"));
                }

            }
            Debug.Log("StartMJGame ShouPai添加了手牌？？？" + all_SP.Count);
            //this.Close_ZhunBeiFont();
            PaiQ_Reduce(1);
        }

    }

    public void PaiQ_Reduce(int i_Count)
    {
        return;
        int i_C1 = 0;//已经销毁的多少
        int i_CSY = i_Count;//剩余销毁的牌墙
        if (mjGameInfo_M.yn_EndReducePaiQ)
        {//从末尾减牌(适用于杠牌后的摸牌)

            //for (int i = allPlayerMJAry.Length - 1; i >= 0; i--)
            //{
            //    i_C1 += allPlayerMJAry[i].PaiQ_Reduce_EndStart(i_CSY);
            //    if (i_C1 == i_Count)
            //    {
            //        mjGameInfo_M.yn_EndReducePaiQ = false;
            //        break;
            //    }
            //    else if (i_C1 < i_Count)
            //    {
            //        i_CSY = i_Count - i_C1;
            //    }
            //}
            PaiQ_Reduce_EndStart(i_Count);
            mjGameInfo_M.yn_EndReducePaiQ = false;
        }
        else
        {//从开始减牌(适用于普通摸牌)
            //for (int i = 0; i < allPlayerMJAry.Length; i++)
            //{
            //    i_C1 += allPlayerMJAry[i].PaiQ_Reduce_StartEnd(i_CSY);
            //    if (i_C1 == i_Count)
            //    {
            //        break;
            //    }
            //    else if (i_C1 < i_Count)
            //    {
            //        i_CSY = i_Count - i_C1;
            //    }
            //}
            PaiQ_Reduce_StartEnd(i_Count);
        }

    }

    /// <summary>用List从开始到结束的地方清除牌墙
    /// </summary>
    private int PaiQ_Reduce_StartEnd(int i_Count)
    {
        int i_C1 = 0;
        bool isUp = paiQ_Count % 2 == 0;

        for (int i = 0; i < listPaiQ_GmAry.Count; i++)
        {
            if (i % 2 == (isUp ? 1 : 0))
            {
                if (listPaiQ_GmAry[i].gameObject.activeInHierarchy)
                {
                    listPaiQ_GmAry[i].gameObject.SetActive(false);
                    paiQ_Count--;
                    isUp = paiQ_Count % 2 == 0;
                    i_C1++;

                    if (i_C1 == i_Count)
                    {
                        break;
                    }
                }
            }
        }
        return i_C1;
    }

    /// <summary>用List从结束到开始的地方清除牌墙
    /// </summary>
    public int PaiQ_Reduce_EndStart(int i_Count)
    {
        int i_C1 = 0;
        for (int i = listPaiQ_GmAry.Count - 1; i >= 0; i--)
        {
            if (listPaiQ_GmAry[i].gameObject.activeInHierarchy)
            {
                listPaiQ_GmAry[i].gameObject.SetActive(false);
                i_C1++;
                if (i_C1 == i_Count)
                {
                    break;
                }
            }
        }
        return i_C1;
    }

    public void PaiQ_SetList(List<List<GameObject>> listAllPQ)
    {
        int i_StartIndex = PaiQ_StartIndex();
        int i_Index_TZ2 = (int)(DataManage.Instance.GetRoomTouZi[0] < DataManage.Instance.GetRoomTouZi[1] ? DataManage.Instance.GetRoomTouZi[0] : DataManage.Instance.GetRoomTouZi[1]);

        i_Index_TZ2 = Mathf.Clamp((i_Index_TZ2 * 2), 0, listAllPQ[i_StartIndex].Count - 1);//限制Index

        for (int i = i_StartIndex; i < listAllPQ.Count; i++)
        {
            if (i == i_StartIndex)
            {
                for (int i_1 = i_Index_TZ2; i_1 < listAllPQ[i].Count; i_1++)
                {
                    if (listAllPQ[i][i_1] != null)
                    {
                        GameObject gm_PQ = listAllPQ[i][i_1];
                        listPaiQ_GmAry.Add(gm_PQ);
                        listAllPQ[i][i_1] = null;
                    }
                }
            }
            else
            {
                for (int i_1 = 0; i_1 < listAllPQ[i].Count; i_1++)
                {
                    if (listAllPQ[i][i_1] != null)
                    {
                        GameObject gm_PQ = listAllPQ[i][i_1];
                        listPaiQ_GmAry.Add(gm_PQ);
                        listAllPQ[i][i_1] = null;
                    }
                }
            }
        }

        for (int i = 0; i <= i_StartIndex; i++)
        {
            for (int i_1 = 0; i_1 < listAllPQ[i].Count; i_1++)
            {
                if (listAllPQ[i][i_1] != null)
                {
                    GameObject gm_PQ = listAllPQ[i][i_1];
                    listPaiQ_GmAry.Add(gm_PQ);
                    listAllPQ[i][i_1] = null;
                }
            }
        }
    }

    public int PaiQ_StartIndex()
    {
        int iZJ_Idex = DataManage.Instance.PData_GetIndex(zhuangJia_Id);
        int i_StartPaiQIndex = (int)(DataManage.Instance.GetRoomTouZi[0] + DataManage.Instance.GetRoomTouZi[1]);
        i_StartPaiQIndex = i_StartPaiQIndex % 4;
        int i_Index_XX = iZJ_Idex + i_StartPaiQIndex;
        i_Index_XX--;//从庄家开始数着走
        i_Index_XX = i_Index_XX % 4;
        i_Index_XX = Mathf.Clamp(i_Index_XX, 0, 3);
        return i_Index_XX;
    }

    public void MJ_Operation_Peng(uint charId, Mj_Sx_ mjSx)
    {
        if (mjGameInfo_M.mj_Cp_Transform != null /*&& mjGameInfo_M.mj_Cp_Transform.GetChild(0).GetComponent<Image>().sprite.name == mjSx.mj_SpriteName*/)
        {//删除出牌。
            //mjGameInfo_M.mj_Cp_Transform.gameObject.SetActive(false);
            MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
            mjGameInfo_M.mj_Cp_Transform = null;
            OpenOrClose_ChuPai_ZhiZhe();
        }

        if (DataManage.Instance.PData_GetIndex(charId) == 0)
        {//本地玩家
            allPlayerMJAry[0].ShouPai_Destroy_My(2, mjSx, false);
            allPlayerMJAry[0].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
            yn_KeYiChuPai = true;
        }
        else
        {//非本地玩家
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charId)].ShouPai_Destroy_Other(2, false);
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charId)].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
        }

        MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Peng, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charId)));//特效
    }

    void MJ_Operation_Gang(uint p_Id, Mj_Sx_ mjSx, uint oriCardid)
    {
        if (DataManage.Instance.PData_GetIndex(p_Id) == 0)
        {
            yn_KeYiChuPai = true;
        }
        MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Gang, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(p_Id))); //播放特效
        bool isMingAnGang = false;
        //string audiopath_ = Get_Player_Data(p_Id).sex == 1 ? AudioPath_.audioOperPath_Mans_ + AudioPathO_.Gang_.ToString() : AudioPath_.audioOperPath_Grils_ + AudioPathO_.Gang_.ToString();//声音
        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(audiopath_));//播放声音
        mjGameInfo_M.yn_EndReducePaiQ = true;
        if (p_Id == DataManage.Instance.MyPlayer_Data.p_ID)
        {//本地玩家

            if (p_Id == oriCardid)
            {//玩家自己杠自己的牌
                int i_PengCount = allPlayerMJAry[0].Peng_IsExist(mjSx);//
                if (i_PengCount == 1)
                {//八杠 发现了想胡牌玩家的碰牌里面有过想碰的牌， 所以为明杠
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(1, mjSx, false);//回收手牌

                    Debug.Log(p_Id + "GangPai_Type(0)" + mjSx.mj_SpriteName);
                    goto AudioPlay;//return;
                }
                else
                {////执行到这里，那么就是 暗杠。
                    Debug.Log(p_Id + "GangPai_Type(1)" + mjSx.mj_SpriteName);
                    int i_HuiShou_Sp = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(4, mjSx, false);

                    if (i_HuiShou_Sp == 3 || i_HuiShou_Sp == 4)
                    {
                        allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.AnGang, mjSx);//显示暗杠麻将
                        Debug.Log("AnGang OK");
                    }
                    else
                    {
                        //Debug.Log("<color=red>AnGang?|" + p_Id + "|" + Debug_Mj_GameC_.GangPai_Errror___.ToString() + mj_Hs_Name + "|" + "</color>");
                    }
                    isMingAnGang = true;
                    //zj_HuiFang += Zj_HuiFang_O_Type.o_AnGang.ToString() + "|Id" + p_Id + "|+CardId" + mj_Hs_Name + "\n";
                    goto AudioPlay;//return;
                }
            }
            else
            {//明杠

                if (mjGameInfo_M.mj_Cp_Transform != null /*&& mjGameInfo_M.mj_Cp_Transform.GetChild(0).GetComponent<Image>().sprite.name == mjSx.mj_SpriteName*/)
                {//删除出牌。
                    //mjGameInfo_M.mj_Cp_Transform.gameObject.SetActive(false);
                    MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
                    mjGameInfo_M.mj_Cp_Transform = null;
                    OpenOrClose_ChuPai_ZhiZhe();
                }

                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_My(3, mjSx, false);//回收手牌/*int i_HuiShou_Sp = */
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.MingGang, mjSx);//显示杠牌

                //zj_HuiFang += Zj_HuiFang_O_Type.o_MingGang.ToString() + "|Id" + p_Id + "|+CardId" + mj_Hs_Name + "\n";
                Debug.Log(p_Id + "GangPai_Type" + mjSx.mj_SpriteName);
                goto AudioPlay;//return;

            }
        }
        else
        {//非本地玩家

            if (p_Id == oriCardid)
            {//玩家自己杠自己的牌
                int i_PengCount = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].Peng_IsExist(mjSx);//是否存在碰牌
                if (i_PengCount == 1)
                {//发现了想胡牌玩家的碰牌里面有过想碰的牌， 所以为明杠
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_Other(1, false);//回收手牌
                    //zj_HuiFang += Zj_HuiFang_O_Type.o_MingGang.ToString() + "|Id" + p_Id + "|+CardId" + mj_Hs_Name + "\n";

                    Debug.Log(p_Id + "GangPai_Type(0)" + mjSx.mj_SpriteName);
                    goto AudioPlay;//return;
                }
                else
                {////执行到这里，那么就是 暗杠。
                    Debug.Log(p_Id + "GangPai_Type(1)" + mjSx.mj_SpriteName);
                    int i_HuiShou_Sp = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_Other(4, false);//回收手牌

                    if (i_HuiShou_Sp == 3 || i_HuiShou_Sp == 4)
                    {
                        allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.AnGang, mjSx);//显示暗杠
                        Debug.Log("AnGang OK");
                    }
                    else
                    {
                        //Debug.Log("<color=red>AnGang?|" + p_Id + "|" + Debug_Mj_GameC_.GangPai_Errror___.ToString() + mj_Hs_Name + "|" + "</color>");
                    }
                    isMingAnGang = true;
                    //zj_HuiFang += Zj_HuiFang_O_Type.o_AnGang.ToString() + "|Id" + p_Id + "|+CardId" + mj_Hs_Name + "\n";
                    goto AudioPlay;//return;
                }
            }
            else
            {//明杠

                if (mjGameInfo_M.mj_Cp_Transform != null /*&& mjGameInfo_M.mj_Cp_Transform.GetChild(0).GetComponent<Image>().sprite.name == mjSx.mj_SpriteName*/)
                {//删除出牌。
                    //mjGameInfo_M.mj_Cp_Transform.gameObject.SetActive(false);
                    MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
                    mjGameInfo_M.mj_Cp_Transform = null;
                    OpenOrClose_ChuPai_ZhiZhe();
                }

                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_Destroy_Other(3, false);//回收手牌/*int i_HuiShou_Sp =*/
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].MJ_Op_Other(MJ_OpType.MingGang, mjSx);//显示杠牌

                Debug.Log(p_Id + "GangPai_Type" + mjSx.mj_SpriteName);
                goto AudioPlay;//return;

            }
        }

        AudioPlay: AudioPlay_Gang(isMingAnGang, p_Id);

    }

    void AudioPlay_Gang(bool isMingAn, uint charId)
    {
        string strPath = DataManage.Instance.PData_GetData(charId).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        strPath = strPath + "" + (isMingAn ? AudioPathOp_TYPE.Op_AnGang.ToString() : AudioPathOp_TYPE.Op_Gang.ToString());
        Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charId), strPath);
    }

    public void MJOperation_ChuPai(uint p_Id, Mj_Sx_ mjInfo)
    {
        int i_Index = DataManage.Instance.PData_GetIndex(p_Id);
        if (i_Index == 0 && mjClickM.mj_Select_Object != null)
        {
            mjClickM.Mj_Click_Move(false);
        }

        if (mjClickM.mj_DownDrag_Object != null && yn_KeYiChuPai == false)
        {//先防止有对象没在麻将位置
            allPlayerMJAry[0].DragRank_EndSetParent(mjDragRank_C.DragObj_Index, mjClickM.mj_DownDrag_Object.gameObject);
            mjDragRank_C.Init_();
            mjClickM.mj_DownDrag_Object = null;
        }
        //if (inGm_UI.svMJDrag.sVRect.content.gameObject.activeInHierarchy)
        //{
        //    //inGm_UI.svMJDrag.image_My.raycastTarget = false;
        //    inGm_UI.svMJDrag.img_MJ.sprite = null;
        //    inGm_UI.svMJDrag.sVRect.content.gameObject.SetActive(false);
        //    if (mjClickM.mj_DownDrag_Object != null && mjClickM.mj_DownDrag_Object.GetComponentInChildren<MeshRenderer>() != null)
        //    {
        //        mjClickM.mj_DownDrag_Object.GetComponentInChildren<MeshRenderer>().enabled = true;
        //    }
        //}

        GameObject rectChuPai_;
        if (i_Index == 0)
        {//本地玩家出牌
            rectChuPai_ = allPlayerMJAry[0].ShouPai_ChuPai_My(mjInfo);
        }
        else
        {//非本地玩家出牌
            rectChuPai_ = allPlayerMJAry[DataManage.Instance.PData_GetIndex(p_Id)].ShouPai_ChuPai_Other(mjInfo);
        }
        if (rectChuPai_ != null)
        {
            mjGameInfo_M.mj_Cp_Transform = rectChuPai_.gameObject.transform;
            Debug.Log("———出牌看看名字：" + rectChuPai_.gameObject.name);
            OpenOrClose_ChuPai_ZhiZhe(rectChuPai_.transform.parent);
        }
    }

    /// <summary>重置麻将点击的选中麻将
    /// </summary>
    void Reset_ClickMJ()
    {
        if (mjClickM.mj_Select_Object != null)
        {//重置点击选中的麻将
            mjClickM.Mj_Click_Move(false);
            mjClickM.mj_Select_Object = null;
        }
    }
    #endregion
    //void Ins_


    #region/*UI*/

    #endregion

    #region/*NET Interface*/
    void Mj_OpenMay_BaoPai(uint charid, List<uint> cards)
    {
        Debug.Log("打开可以爆？？？");
        //W3Debug_.Instance.W3Log(W3DebugType.Op_Peng.ToString() + "|" + card);

        if (charid == BaseProto.playerInfo.m_id)
        {
            baotingC.listTingHands = cards;
            baotingC.tingCharid = charid;

            inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnBaoPai);
        }
    }

    void Mj_OpenMay_FeiPeng(uint charid, uint card)
    {
        Debug.Log("打开可以飞碰？？？" + card);
        if (!IsFeiPeng)
        {//没有飞碰直接点过
            inGm_UI.OnClick_Btn_Guo();
            return;
        }
        mjGameInfo_M.keYi_FeiPeng = card;
        //W3Debug_.Instance.W3Log(W3DebugType.Op_Peng.ToString() + "|" + card);
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnFeiPai);
    }

    void Mj_OpenMay_TiPai(uint card)
    {
        mjGameInfo_M.keYi_TiPai = card;
        Debug.Log("打开可以提牌？？？");
        //W3Debug_.Instance.W3Log(W3DebugType.Op_Peng.ToString() + "|" + card);
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnTiPai);
    }
    /// <summary>提示可以进行操作打开相关按钮——碰
    /// </summary>
    void IFCMjGameNet_OpenMayOperation.Mj_OpenMay_Peng(uint card)
    {
        mjGameInfo_M.keYi_Peng = card;
        Debug.Log("打开可以碰？？？");
        //W3Debug_.Instance.W3Log(W3DebugType.Op_Peng.ToString() + "|" + card);
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnPeng);
    }

    /// <summary>提示可以进行操作打开相关按钮——杠
    /// </summary>
    void IFCMjGameNet_OpenMayOperation.Mj_OpenMay_Gang(List<uint> card, uint orCharid_)
    {
        mjGameInfo_M.keYi_Gang = card[0];
        Debug.Log("打开可以杠？？？");
        //W3Debug_.Instance.W3Log(W3DebugType.Op_Gang.ToString() + "|" + card);
        mjGameInfo_M.oriCharId = orCharid_;
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnGang);
    }

    /// <summary>提示可以进行操作打开相关按钮——胡
    /// </summary>
    void IFCMjGameNet_OpenMayOperation.Mj_OpenMay_Hu(uint cardid, uint orCharid_)
    {
        mjGameInfo_M.keYi_Hu = cardid;
        Debug.Log("打开可以胡？？？");
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);
        mjGameInfo_M.oriCharId = orCharid_;
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnHu);
    }

    /// <summary>提示可以进行操作打开相关按钮——过
    /// </summary>
    void IFCMjGameNet_OpenMayOperation.Mj_OpenMay_Guo()
    {
        inGm_UI.Open_PengGangHuBtn(MJOpBtnName.BtnGuo);
    }

    void IFCMjGameNet_Update.UpdateStartHandCards(List<uint> all_SP, int seat)
    {
        DataManage.Instance.MjXuanQue = 0;
        SetHandCardRotation(true);
        inGm_UI.btn_YaoQing.gameObject.SetActive(false);
        inGm_UI.uiWin_End1_MJ.gameObject.SetActive(false);

        baotingC.Init_TingEvent();

        listStartHandCards = all_SP;
        End_Init();
        StartCoroutine(Game_StartInit());

    }

    void IFCMjGameNet_Update.UpdateMopai(uint charid, bool daChu, uint cardId)
    {
        if (cardId == 0)
        {
            Debug.Log("<color=red> ??????UpdateMopai CardId==0 MoPai ??????</color>" + cardId);
            return;
        }

        DataManage.Instance.MJSX_Add(cardId.ToCard());
        mjGameInfo_M.Set_MoPai(charid, DataManage.Instance.MJSX_Get(cardId));
        mjGameInfo_M.oriCharId = charid;

        //zj_HuiFang += Zj_HuiFang_O_Type.o_MoPai.ToString() + "|Id" + charid + "|+CardId" + Gm_Manager.G_M.All_Resources.Get_Mj_Sx(CardId).mj_SpriteName + "\n";
        Debug.Log(charid + "Player_AddMoPai" + DataManage.Instance.MJSX_Get(cardId).mj_SpriteName);
        PaiQ_Reduce(1);
        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            //if (DataManage.Instance.MjXuanQue != 0)
            //{
            //    allPlayerMJAry[0].Que_UpdateHandCards();
            //}
            //else {
            //    allPlayerMJAry[0].ShouPai_RankMYAll();
            //}//==↓
            inGm_UI.tog_ZDDQ.isOn = true;
            inGm_UI.tog_ZDDQ.interactable = false;
            GameObject gmHandCard = allPlayerMJAry[0].MoPai_Add_MY(DataManage.Instance.MJSX_Get(cardId));
            if (DataManage.Instance.MjXuanQue != 0 && gmHandCard != null && gmHandCard.GetComponent<MeshRenderer>() != null)
            {
                if (gmHandCard.GetComponentInParent<MJInfos>().MjINFO_.mj_SpriteName.IndexOf(allPlayerMJAry[0].Que_TYPE()) >= 0)
                {
                    gmHandCard.GetComponent<MeshRenderer>().material = locaHandCard_Que;
                }
            }
            if (mjClickM.mj_Select_Object != null)
            {//
                mjClickM.Mj_Click_Move(false);
                mjClickM.mj_Select_Object = null;
            }

            if (baotingC.isTing)
            {//如果报听了
                StartCoroutine(WaitTimeInveke_Event(delegate ()
                {
                    Ting_ZiDongChuPai();
                }, 1.5f));
            }
            else
            {
                yn_KeYiChuPai = true;
            }
        }
        else
        {
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MoPai_Add_OtherPlayer(DataManage.Instance.MJSX_Get(cardId));
        }
    }

    void IFCMjGameNet_Update.UpdateChuPai(uint charid, uint cardId)
    {
        DataManage.Instance.MJSX_Add(cardId.ToCard());
        mjGameInfo_M.Set_ChuPai(charid, DataManage.Instance.MJSX_Get(cardId));
        mjGameInfo_M.oriCharId = charid;

        string strPath = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        strPath = strPath + "MJ/" + DataManage.Instance.MJSX_Get(cardId).mj_SpriteName;
        Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charid), strPath);

        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {//如果是本地玩家出牌， 则可以点击自动对齐
            inGm_UI.tog_ZDDQ.interactable = true;
        }

        MJOperation_ChuPai(charid, DataManage.Instance.MJSX_Get(cardId));
        Debug.Log("玩家：" + charid + "\t出牌：" + DataManage.Instance.MJSX_Get(cardId).mj_SpriteName);
    }

    void IFCMjGameNet_Update.UpdatePengPai(uint charId, uint cardId)
    {
        DataManage.Instance.MJSX_Add(cardId.ToCard());

        string strPath = DataManage.Instance.PData_GetData(charId).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        strPath = strPath + "" + AudioPathOp_TYPE.Op_Peng.ToString();
        Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charId), strPath);

        MJ_Operation_Peng(charId, DataManage.Instance.MJSX_Get(cardId));

        Reset_ClickMJ();
    }

    void IFCMjGameNet_Update.UpdateGangPai(uint charid, uint cardId, uint oriCharid)
    {
        DataManage.Instance.MJSX_Add(cardId.ToCard());

        MJ_Operation_Gang(charid, DataManage.Instance.MJSX_Get(cardId), oriCharid);
        Reset_ClickMJ();
    }

    void IFCMjGameNet_Update.UpdateHule(uint charid, uint cardId, uint oriCharid)
    {
        DataManage.Instance.MJSX_Add(cardId.ToCard());

        Debug.Log(" YouRenKeYiHuPai?Player Id:  " + charid + "HuPai_Id" + DataManage.Instance.MJSX_Get(cardId).mj_SpriteName);
        Debug.Log(" C_ZhanJi_:  ");
        //自摸还是平胡的 特效
        MemoryPool_3D.Instance.GetShowPS_Model(charid == oriCharid ? ResPath_Assets.se_ZiMo : ResPath_Assets.se_Hu, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));
        if (charid != oriCharid)
        {//显示点炮特效
            MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_DianPao, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(oriCharid)));
        }
        string strPath = DataManage.Instance.PData_GetData(charid).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        strPath = strPath + "" + AudioPathOp_TYPE.Op_Hu.ToString();
        Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(charid), strPath);
        Reset_ClickMJ();
    }

    void UpdateXuanQue(uint charid, uint cardType)
    {
        Debug.Log(charid + "玩家选缺：" + cardType + "MeshRenderName" + locaHandCard_Que.name);
        string str_Que = "";
        switch (cardType)
        {
            case 1:
                str_Que = "万";
                break;
            case 2:
                str_Que = "筒";
                break;
            case 3:
                str_Que = "条";
                break;
            default:
                break;
        }

        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {//记录本地的选缺
            DataManage.Instance.MjXuanQue = cardType;
            allPlayerMJAry[0].Que_UpdateHandCards();
            Debug.Log(charid + "玩家选缺：" + str_Que);
            this.Que_UpdateMeshRender();
            Debug.Log(charid + "玩家AAA选缺：" + str_Que);
        }

        inGm_UI.allPlayerInfoUis[DataManage.Instance.PData_GetIndex(charid)].Set_Que(str_Que);
    }

    void UpdateFeiPeng(uint charid, uint cardId, uint oriCharid)
    {
        Debug.Log("更新玩家 飞碰");
        DataManage.Instance.MJSX_Add(cardId.ToCard());
        Mj_Sx_ mjSx = DataManage.Instance.MJSX_Get(cardId);
        /*
        飞碰流程：1、去掉1张手牌被碰牌。2、去掉一张红中、3、去掉其他玩家的出牌
        */
        if (mjGameInfo_M.mj_Cp_Transform != null)
        {//删除出牌。
            //mjGameInfo_M.mj_Cp_Transform.gameObject.SetActive(false);
            MemoryPool_3D.Instance.MJ3D_Recycle(mjGameInfo_M.mj_Cp_Transform.gameObject);
            mjGameInfo_M.mj_Cp_Transform = null;
            OpenOrClose_ChuPai_ZhiZhe();
        }

        if (DataManage.Instance.PData_GetIndex(charid) == 0)
        {//本地玩家
            allPlayerMJAry[0].ShouPai_Destroy_My(1, mjSx, false);
            allPlayerMJAry[0].ShouPai_Destroy_My(1, DataManage.Instance.MJSX_Get(53), false);
            allPlayerMJAry[0].MJ_Op_Other(MJ_OpType.FeiPeng, mjSx);

            Reset_ClickMJ();
            yn_KeYiChuPai = true;
        }
        else
        {//非本地玩家
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].ShouPai_Destroy_Other(2, false);
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MJ_Op_Other(MJ_OpType.FeiPeng, mjSx);
        }

        MemoryPool_3D.Instance.GetShowPS_Model(ResPath_Assets.se_Peng, t_PSPosition.GetChild(DataManage.Instance.PData_GetIndex(charid)));//特效

    }
    void UpdateBao(uint charid/*, uint cardId, uint oriCharid*/)
    {
        Debug.Log("更新玩家 爆牌" + charid);

        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            baotingC.isTing = true;
        }
        inGm_UI.UpdateShowBaoTing(charid);

        Reset_ClickMJ();
    }
    void Ting_Start()
    {

    }

    void Ting_ZiDongChuPai()
    {
        if (yn_KeYiChuPai && inGm_UI.needMJ_Ui.IsShowBtn && allPlayerMJAry[0].mj_MoPai.transform.childCount > 0)
        {
            PublicEvent.GetINS.Fun_SentPopCard(allPlayerMJAry[0].mj_MoPai.MjINFO_.mJCardID);
        }
    }

    void UpdateTiPai(uint charid, uint cardId)
    {
        Debug.Log("更新玩家 提牌");
        DataManage.Instance.MJSX_Add(cardId.ToCard());
        Mj_Sx_ mjSx = DataManage.Instance.MJSX_Get(cardId);
        /*提牌流程：1、销毁手牌1张。2、更新飞碰为碰。3、手牌添加红中
        */
        if (charid == BaseProto.playerInfo.m_id)
        {
            allPlayerMJAry[0].ShouPai_Destroy_My(1, mjSx, false);
            allPlayerMJAry[0].MJ_Op_Other(MJ_OpType.TiPai, mjSx);
            allPlayerMJAry[0].ShouPai_Add_My(DataManage.Instance.MJSX_Get(53));
            LocaHandCardsSort();

            Reset_ClickMJ();
            yn_KeYiChuPai = true;
        }
        else
        {
            allPlayerMJAry[DataManage.Instance.PData_GetIndex(charid)].MJ_Op_Other(MJ_OpType.TiPai, mjSx);
        }
    }

    void UpdatePlayerUi(Player_Data[] pAry)
    {
        Dir_Init();
    }
    public void Dir_Init()
    {
        #region 玩家东南西北方位设置
        if (i_IsInitDIR == 0)
        {
            uint setPosition = DataManage.Instance.MyPlayer_Data.position;//0;// GameManager.GM._AllPlayerData[0].Position;
            Debug.Log("My Position:" + setPosition);
            dirLight[0].SetActive(setPosition == 0);
            dirLight[1].SetActive(setPosition == 1);
            dirLight[2].SetActive(setPosition == 2);
            dirLight[3].SetActive(setPosition == 3);
            i_IsInitDIR = 1;
        }
        #endregion
    }

    /// <summary>指定一下中间指针旋转角度
    /// </summary>
    public void Update_ZhiZhen_R(uint charid)
    {
        int dir = DataManage.Instance.PData_GetIndex(charid);
        //needMJ_Ui.CenterPointer_Open1Pointer(seatNumber);
        //StartOr_Stop_Countdown(true, 10, needMJ_Ui.t_GameTime);//倒计时的显示

        uint setPosition = DataManage.Instance.MyPlayer_Data.position;
        GameObject dongSet = dirLight[setPosition].transform.FindChild("fangxiang_dong").gameObject;
        GameObject nanSet = dirLight[setPosition].transform.FindChild("fangxiang_nan").gameObject;
        GameObject xiSet = dirLight[setPosition].transform.FindChild("fangxiang_xi").gameObject;
        GameObject beiSet = dirLight[setPosition].transform.FindChild("fangxiang_bei").gameObject;
        Debug.Log("Update_ZhiZhen_R" + dir + "___" + setPosition);

        switch (setPosition)
        {//0=东 1=南 2=西 3=北
            case 0:
                dongSet.GetComponent<MeshRenderer>().material = dir == 0 ? fxLight_Liang : fxLight_An;
                nanSet.GetComponent<MeshRenderer>().material = dir == 1 ? fxLight_Liang : fxLight_An;
                xiSet.GetComponent<MeshRenderer>().material = dir == 2 ? fxLight_Liang : fxLight_An;
                beiSet.GetComponent<MeshRenderer>().material = dir == 3 ? fxLight_Liang : fxLight_An;
                break;
            case 1:
                dongSet.GetComponent<MeshRenderer>().material = dir == 3 ? fxLight_Liang : fxLight_An;
                nanSet.GetComponent<MeshRenderer>().material = dir == 0 ? fxLight_Liang : fxLight_An;
                xiSet.GetComponent<MeshRenderer>().material = dir == 1 ? fxLight_Liang : fxLight_An;
                beiSet.GetComponent<MeshRenderer>().material = dir == 2 ? fxLight_Liang : fxLight_An;
                break;
            case 2:
                dongSet.GetComponent<MeshRenderer>().material = dir == 2 ? fxLight_Liang : fxLight_An;
                nanSet.GetComponent<MeshRenderer>().material = dir == 3 ? fxLight_Liang : fxLight_An;
                xiSet.GetComponent<MeshRenderer>().material = dir == 0 ? fxLight_Liang : fxLight_An;
                beiSet.GetComponent<MeshRenderer>().material = dir == 1 ? fxLight_Liang : fxLight_An;
                break;
            case 3:
                dongSet.GetComponent<MeshRenderer>().material = dir == 1 ? fxLight_Liang : fxLight_An;
                nanSet.GetComponent<MeshRenderer>().material = dir == 2 ? fxLight_Liang : fxLight_An;
                xiSet.GetComponent<MeshRenderer>().material = dir == 3 ? fxLight_Liang : fxLight_An;
                beiSet.GetComponent<MeshRenderer>().material = dir == 0 ? fxLight_Liang : fxLight_An;
                break;
        }
    }

    void ZhuangFirstChupai(uint charid)
    {
        Debug.Log("庄家出牌？？");
        if (FirstChupai)
        {
            if (ZhuangJiaid == 0 && FirstChupai)
            {
                yn_KeYiChuPai = true;
                PublicEvent.GetINS.Fun_SentClientMo(0);
            }
            FirstChupai = false;
        }
        else
        {
            if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
            {
                yn_KeYiChuPai = true;
            }
        }
    }

    //得到服务器通知的庄家
    public void ReserveZhuangJia(uint charid)
    {
        zhuangJia_Id = charid;
        inGm_UI.Set_ZhuangJia(charid);
        ZhuangJiaid = DataManage.Instance.PData_GetIndex(charid);// this.Get_Player_Index(charid.ToString());
    }

    /// <summary>回收所有的麻将
    /// </summary>
    public void End_Init()
    {
        for (int i = 0; i < allPlayerMJAry.Length; i++)
        {
            allPlayerMJAry[i].Init_MjAll();
        }
        listPaiQ_GmAry.Clear();
        //yield return null;
    }

    void End_WaitTimeInveke()
    {
        End_ClearOtherHandCard();
        Debug.Log("Open_ End_Plane???????????????");
        Debug.Log("ZhanJiHuiFang\n" /*+ zj_HuiFang*/);
        OpenOrClose_ChuPai_ZhiZhe();
        Reset_ClickMJ();
        DataManage.Instance.RoomSyJuShu--;
        inGm_UI.Update_JuShu();
        inGm_UI.uiWin_End1_MJ.gameObject.SetActive(true);
        inGm_UI.uiWin_End1_MJ.btn_End1Setout.interactable = true;
        //////MjOver_1 mjOv1 = UIManager.Instance.ShowUI(AllPrefabName.UiWin_MjEnd_1, UIManager.Instance.canvas_T).GetComponent<MjOver_1>();
        //////mjOv1.Init();
        //////mjOv1.btn_Share_Data.onClick.RemoveAllListeners();
        //////mjOv1.btn_Share_Data.onClick.AddListener(delegate ()
        //////{
        //////    Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        //////    Debug.Log("点击了分享");
        //////    SdkEvent.Instance.ShareResult();
        //////});
        //////mjOv1.EndShow_ResAll();
        int i_HUCount = 0;
        //所有玩家Date信息
        for (int i = 0; i < rsp.players.Count; i++)
        {
            if (rsp.players[i].huInfos.Count > 0)
            {
                i_HUCount++;
            }
            //all_Player_UIgm[i].img_Center_Prompt.enabled = false;
            //char[] str = text.text.ToCharArray();
            //if (str[0] == '-') {
            if (rsp.players[i].charId == BaseProto.playerInfo.m_id)
            {
                Debug.Log("Look:" + rsp.diamond);
                BaseProto.playerInfo.m_diamond = rsp.diamond;
            }
            Debug.Log("Look_ Game Player ID:" + rsp.players[i].charId.ToString());
            Debug.Log("Look_ Game Player ID:" + rsp.players[i].restCards.Count.ToString());
            inGm_UI.allPlayerInfoUis[DataManage.Instance.PData_GetIndex((rsp.players[i].charId))].Set_DiFen(rsp.players[i].restGold);// "";
            //////mjOv1.end_Mj_P_UI[i].Set_PlayerUIGai(rsp.players[i], DataManage.Instance.PData_GetData(rsp.players[i].charId).P_Name, zhuangJia_Id.ToString());
            if (rsp.players[i].charId != DataManage.Instance.MyPlayer_Data.p_ID)
            {//其他玩家更新手牌
                End_UpdateHandCard(rsp.players[i].charId, rsp.players[i].restCards);
            }
            else
            {//本地玩家更新出单局结算界面
                string str_Sr = "";// rsp.players[i].changeGold >= 0? rsp.players[i].changeGold .ToString(): "";
                string str_ZC = "";// rsp.players[i].changeGold < 0 ? rsp.players[i].changeGold.ToString() : "";
                for (int i_G = 0; i_G < rsp.players[i].gangInfos.Count; i_G++)
                {
                    //3，放杠明杠。2、暗杠 。1、碰上杠
                    string str_G1 = "";
                    switch (rsp.players[i].gangInfos[i_G].catag)
                    {
                        case 1:
                            for (int i_X1 = 0; i_X1 < rsp.players[i].gangInfos[i_G].gainGolds.Count; i_X1++)
                            {
                                str_G1 += "碰上杠+" + rsp.players[i].gangInfos[i_G].gainGolds[i_X1].gainGold + "\t";
                            }

                            break;
                        case 2:
                            for (int i_X1 = 0; i_X1 < rsp.players[i].gangInfos[i_G].gainGolds.Count; i_X1++)
                            {
                                str_G1 += "暗杠+" + rsp.players[i].gangInfos[i_G].gainGolds[i_X1].gainGold + "\t";
                            }
                            break;
                        case 3:
                            for (int i_X1 = 0; i_X1 < rsp.players[i].gangInfos[i_G].gainGolds.Count; i_X1++)
                            {
                                str_G1 += "明杠+" + rsp.players[i].gangInfos[i_G].gainGolds[i_X1].gainGold + "\t点杠玩家：" + DataManage.Instance.PData_GetData(rsp.players[i].charId).P_Name + "\t";
                            }
                            break;
                        default:
                            break;
                    }
                    str_Sr += str_G1 + "\n";
                }
                for (int i_G = 0; i_G < rsp.players[i].fanggangInfos.Count; i_G++)
                {
                    //3，放杠明杠。2、暗杠 。1、碰上杠

                    string str_G1 = "";
                    if (rsp.players[i].fanggangInfos[i_G].lostGolds != null)
                    {
                        switch (rsp.players[i].fanggangInfos[i_G].catag)
                        {
                            case 1:
                                str_G1 = "碰上杠-" + rsp.players[i].fanggangInfos[i_G].lostGolds.lostGold + "\t杠的玩家：" + DataManage.Instance.PData_GetData(rsp.players[i].fanggangInfos[i_G].ganger).P_Name;
                                break;
                            case 2:
                                str_G1 = "暗杠-" + rsp.players[i].fanggangInfos[i_G].lostGolds.lostGold + "\t杠的玩家：" + DataManage.Instance.PData_GetData(rsp.players[i].fanggangInfos[i_G].ganger).P_Name;
                                break;
                            case 3:
                                str_G1 = "明杠-" + rsp.players[i].fanggangInfos[i_G].lostGolds.lostGold + "\t杠的玩家：" + DataManage.Instance.PData_GetData(rsp.players[i].fanggangInfos[i_G].ganger).P_Name;
                                break;
                            default:
                                break;
                        }
                    }
                    str_ZC += str_G1 + "\n";
                }

                for (int i_G = 0; i_G < rsp.players[i].fpInfos.Count; i_G++)
                {
                    string str_G1 = "";
                    str_G1 += "点炮【" + inGm_UI.uiWin_End1_MJ.GetHuType(rsp.players[i].fpInfos[i_G].catag) + "】-" + rsp.players[i].fpInfos[i_G].lostGolds.lostGold + "\t接炮玩家：" + DataManage.Instance.PData_GetData(rsp.players[i].fpInfos[i_G].huer).P_Name;
                    str_ZC += str_G1 + "\n";
                }
                if (rsp.players[i].huInfos.Count > 0)
                {
                    string strIsZiMo = rsp.players[i].charId == rsp.players[i].huInfos[0].oricharId ? "<自摸>" : "<接炮>";
                    strIsZiMo = "<color=red>" + strIsZiMo + "</color>";
                    for (int i_XX = 0; i_XX < rsp.players[i].huInfos[0].gainGolds.Count; i_XX++)
                    {
                        str_Sr += strIsZiMo + "【" + inGm_UI.uiWin_End1_MJ.GetHuType(rsp.players[i].huInfos[0].catag) + "】+" + rsp.players[i].huInfos[0].gainGolds[i_XX].gainGold + "\t\t支付者:" + DataManage.Instance.PData_GetData(rsp.players[i].huInfos[0].gainGolds[i_XX].charId).P_Name + "\n";
                    }
                    str_Sr += "\n";
                }
                string strIsADD = rsp.players[i].changeGold >= 0 ? "+" : "";
                inGm_UI.uiWin_End1_MJ.t_EndResult.text = "<color=red><size=50>合计：" + strIsADD + rsp.players[i].changeGold + "</size></color>\n——————————————\n <color=purple><size=40>收入:</size>\n\n" + str_Sr + "</color>\n——————————————\n<size=50>支出:</size>\n\n" + str_ZC + "\n\n\n";
                inGm_UI.uiWin_End1_MJ.OpenOrClose_Result(true);
            }

            //////End_MaJiang_Player_Gm_UI end_1 = mjOv1.end_Mj_P_UI[i];
            //////waitTime_1 = 0.1f;
            //////StartCoroutine(WaitTimeInveke_Event(delegate ()
            //////{
            //////    end_1.Set_Trim();
            //////}));

        }
        SetHandCardRotation(false);

        //////mjOv1.btn_EndStart.onClick.AddListener(delegate ()
        //////{
        //////    Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        //////    Debug.Log("点击了开始");
        //////    inGm_UI.OnClick_Btn_SetoutStart();
        //////    inGm_UI.needMJ_Ui.OpenOrClose_ZhuangJia(false);
        //////    UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_MjEnd_1);

        //////    waitTime_1 = 0.6f;
        //////    StartCoroutine(WaitTimeInveke_Event(delegate ()
        //////    {
        //////        if (DataManage.Instance.roomCardNumCount != 0 && i_HUCount == 0)
        //////        {//如果剩余的牌数以及没有人胡牌， 那么可能是投票退出了房间
        //////            GameManager.GM.Game_RetrueHome();
        //////        }
        //////    }));
        //////});
        End_IFAll();

        //////mjOv1.EndShow_Set();
        //End_Init();
    }

    ProtoBuf.MJGameOver rsp;
    void End_YiJu(ProtoBuf.MJGameOver rsp_Value)
    {
        this.rsp = rsp_Value;
        Invoke("End_WaitTimeInveke", 2f);
    }

    ProtoBuf.MJGameOver endAllrsp;
    public void END_SetEndAll(ProtoBuf.MJGameOver _rsp)
    {
        endAllrsp = _rsp;
    }
    /// <summary>判断是否全局结束
    /// </summary>
    void End_IFAll()
    {
        if (endAllrsp != null && endAllrsp.befores.Count > 0)
        {
            //MjOver_1 mjOv1 = UIManager.Instance.FindUI(AllPrefabName.UiWin_MjEnd_1).GetComponent<MjOver_1>();
            //mjOv1.Open_EndStart();
            //mjOv1.btn_EndQueDing.onClick.AddListener(delegate ()
            //{
            //    Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
            //    PublicEvent.GetINS.Fun_ReciveAllGameOverResult(endAllrsp.befores);
            //});
            inGm_UI.uiWin_End1_MJ.btn_CloseWin.onClick.AddListener(delegate ()
            {
                Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                PublicEvent.GetINS.Fun_ReciveAllGameOverResult(endAllrsp.befores);
            });
        }
    }

    /// <summary>结束， 刷新其他玩家的手牌
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="handCardList"></param>
    void End_UpdateHandCard(uint charid, List<uint> handCardList)
    {
        handCardList.Sort();
        int i_Seat = DataManage.Instance.PData_GetIndex(charid);
        for (int i = 0; i < handCardList.Count; i++)
        {
            DataManage.Instance.MJSX_Add(handCardList[i].ToCard());
            Mj_Sx_ mjsx = DataManage.Instance.MJSX_Get(handCardList[i]);
            allPlayerMJAry[i_Seat].ShouPai_Add_My(mjsx);
        }
        //allPlayerMJAry[i_Seat].ShouPai_RankRight_MY();
    }

    /// <summary>清除其他玩家的手牌
    /// </summary>
    void End_ClearOtherHandCard()
    {
        for (int i = 1; i < allPlayerMJAry.Length; i++)
        {
            allPlayerMJAry[i].Init_MjAll_ShouPai();
        }
    }

    /// <summary>重连恢复麻将
    /// </summary>
    /// <param name="mjRoom"></param>
    void ReUpdateMJ(ProtoBuf.MJRoomInfo mjRoom)
    {

        inGm_UI.needMJ_Ui.Prep_OpenOrClose_Btn(false);
        inGm_UI.needMJ_Ui.Prep_CloseOKStyleAll();
        inGm_UI.btn_YaoQing.gameObject.SetActive(false);
        inGm_UI.gmXuanQue_Bg.SetActive(false);
        DataManage.Instance.MyPlayer_Data.isgaming = true;

        uint chupaiCharid = 0;
        int chupaiseat = -1;
        zhuangJia_Id = mjRoom.zjId;
        int i_AddMoPai = 0;
        //该谁出牌了 
        Debug.Log(" ChongXinLianJie_ ");
        //try
        //{
        if (mjRoom.cardsInfos.Count > 0)
        {
            inGm_UI.Set_ShengYu_PaiShu((int)mjRoom.cardsInfos[0].roundRestCardNum);
        }

        for (int i = 0; i < mjRoom.cardsInfos.Count; i++)
        {
            // 牌数为3n+2      及该玩家还没出牌 显示[13]  更新指针

            if ((mjRoom.cardsInfos[i].handCards.Count - 2) % 3 == 0)
            {
                chupaiCharid = mjRoom.charIds[i];
                chupaiseat = DataManage.Instance.PData_GetIndex(chupaiCharid);//
                break;
            }

        }

        #region 更新四方向的牌
        //更新四方向的牌
        for (int i = 0; i < mjRoom.charIds.Count; i++)
        {

            //////all_Player_Data[Get_Player_Index(mjRoom.charIds[i].ToString())].isgaming = true;
            //更新已经落下的牌
            for (int j = 0; j < mjRoom.cardsInfos[i].passCards.Count; j++)
            {
                Mj_Sx_ mjSx = mjRoom.cardsInfos[i].passCards[j].ToCard();
                DataManage.Instance.MJSX_Add(mjSx);
                allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].ChuPai_ADD(mjSx.mj_SpriteName);

            }


            //int num = 0;
            //更新碰牌  num++
            for (int j = 0; j < mjRoom.cardsInfos[i].pengCards.Count; j++)
            {
                Mj_Sx_ mjSx = mjRoom.cardsInfos[i].pengCards[j].ToCard();
                DataManage.Instance.MJSX_Add(mjSx);
                if (DataManage.Instance.PData_GetIndex(mjRoom.charIds[i]) == 0)
                {//本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
                }
                else
                {//非本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.PengPai, mjSx);
                }
            }

            //更新碰牌  num++
            for (int j = 0; j < mjRoom.cardsInfos[i].feiPengCards.Count; j++)
            {
                Mj_Sx_ mjSx = mjRoom.cardsInfos[i].feiPengCards[j].ToCard();
                DataManage.Instance.MJSX_Add(mjSx);
                if (DataManage.Instance.PData_GetIndex(mjRoom.charIds[i]) == 0)
                {//本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.FeiPeng, mjSx);
                }
                else
                {//非本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.FeiPeng, mjSx);
                }
            }


            //更新杠牌   num++
            for (int j = 0; j < mjRoom.cardsInfos[i].mingGangCards.Count; j++)
            {
                Mj_Sx_ mjSx = mjRoom.cardsInfos[i].mingGangCards[j].ToCard();
                DataManage.Instance.MJSX_Add(mjSx);
                //////Player_Start_Pai_Manage(3, this.Get_Player_Index(mjRoom.charIds[i].ToString()), Gm_Manager.G_M.All_Resources.Get_Mj_Sx(mjRoom.cardsInfos[i].mingGangCards[j]).mj_SpriteName);
                if (DataManage.Instance.PData_GetIndex(mjRoom.charIds[i]) == 0)
                {//本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.MingGang, mjSx);
                }
                else
                {//非本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.MingGang, mjSx);
                }
            }

            //all_Player_UIgm[i].img_Center_Prompt.enabled = false;
            for (int j = 0; j < mjRoom.cardsInfos[i].anGangCards.Count; j++)
            {
                Mj_Sx_ mjSx = mjRoom.cardsInfos[i].anGangCards[j].ToCard();
                DataManage.Instance.MJSX_Add(mjSx);
                //////Player_Start_Pai_Manage(3, this.Get_Player_Index(mjRoom.charIds[i].ToString()), Gm_Manager.G_M.All_Resources.Get_Mj_Sx(mjRoom.cardsInfos[i].anGangCards[j]).mj_SpriteName);
                if (DataManage.Instance.PData_GetIndex(mjRoom.charIds[i]) == 0)
                {//本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.AnGang, mjSx);
                }
                else
                {//非本地玩家
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].MJ_Op_Other(MJ_OpType.AnGang, mjSx);
                }
            }

            //更新手牌
            int i_Yn_Mp = chupaiCharid == DataManage.Instance.MyPlayer_Data.p_ID ? 1 : 0;

            if (mjRoom.charIds[i] == DataManage.Instance.MyPlayer_Data.p_ID)
            {

                for (int i_2 = 0; i_2 < mjRoom.cardsInfos[i].huCards.Count; i_2++)
                {//有人已经胡了牌
                    DataManage.Instance.MJSX_Add(mjRoom.cardsInfos[i].huCards[i_2].ToCard());
                }

                //UpdateCardValue(mjRoom.cardsInfos[i].handCards, 0);
                List<uint> handCards = new List<uint>();// mjRoom.cardsInfos[i].handCards;
                for (int iA = 0; iA < mjRoom.cardsInfos[i].handCards.Count - i_Yn_Mp; iA++)
                {
                    handCards.Add(mjRoom.cardsInfos[i].handCards[iA]);
                }
                handCards.Sort();//手牌排序
                for (int i_1 = 0; i_1 < handCards.Count /*- i_Yn_Mp*/; i_1++)
                {
                    Mj_Sx_ mjSx = handCards[i_1].ToCard();
                    DataManage.Instance.MJSX_Add(mjSx);
                    allPlayerMJAry[0].ShouPai_Add_My(mjSx);
                }
                if (handCards.Count < 13)
                {//如果手牌小于13
                    allPlayerMJAry[0].ShouPai_RankRight_MY();
                }

                if (mjRoom.cardsInfos[i].hasBaoTing)
                {
                }

            }
            else
            {//不是本地玩家
                for (int i_1 = 0; i_1 < mjRoom.cardsInfos[i].handCards.Count - i_Yn_Mp; i_1++)
                {
                    Mj_Sx_ mjSx = mjRoom.cardsInfos[i].handCards[i_1].ToCard();
                    DataManage.Instance.MJSX_Add(mjSx);
                    //////Player_Start_Pai_Manage(1, this.Get_Player_Index(mjRoom.charIds[i].ToString()), Gm_Manager.G_M.All_Resources.Get_Mj_Sx(mjRoom.cardsInfos[i].handCards[i_1]).mj_SpriteName);
                    allPlayerMJAry[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].ShouPai_Add_OtherPlayer(new Mj_Sx_());
                }
                //allPlayerM_Mj[DataManage.Instance.PData_GetIndex(mjRoom.charIds[i])].Mj_Ask_UpdataStartMJOther(mjRoom.cardsInfos[i].handCards.Count - i_Yn_Mp);

                if (mjRoom.cardsInfos[i].hasBaoTing)
                {
                    //////all_Player_UIgm[Get_Player_Index(mjRoom.charIds[i].ToString())].p_Img_TouXiang.transform.GetChild(0).GetComponent<Image>().enabled = true;
                }
            }

            if (i_AddMoPai == 0 && chupaiCharid == mjRoom.charIds[i])
            {
                //牌值为最后一张手牌(摸牌)
                _iFC_Update.UpdateMopai(chupaiCharid, false, mjRoom.cardsInfos[i].handCards[mjRoom.cardsInfos[i].handCards.Count - 1]);
                i_AddMoPai++;
                //UpdateMopai(chupaiCharid, false, mjRoom.cardsInfos[i].handCards[mjRoom.cardsInfos[i].handCards.Count - 1]);
            }

            ///更新选缺
            switch (mjRoom.cardsInfos[i].xQue)
            {
                case ProtoBuf.MJCardType.MJ_CARD_TYPE_WAN:
                    UpdateXuanQue(mjRoom.cardsInfos[i].charId, 1);
                    break;
                case ProtoBuf.MJCardType.MJ_CARD_TYPE_TONG:
                    UpdateXuanQue(mjRoom.cardsInfos[i].charId, 2);
                    break;
                case ProtoBuf.MJCardType.MJ_CARD_TYPE_TIAO:
                    UpdateXuanQue(mjRoom.cardsInfos[i].charId, 3);
                    break;
                default:
                    Debug.Log("XuanQue??????" + mjRoom.cardsInfos[i].xQue);
                    break;
            }
        }
        Update_ZhiZhen_R(chupaiCharid);/*指针指向该出牌的人*/
        #endregion

        //没人能出牌
        //if (chupaiseat == -1)
        //{
        //    //拿到最后一张落下去的牌 从所有玩家出牌列表的最后一位遍历
        //}

        //PengHouChuPai = false;
        //IsInRepick = false;

        //绿箭头.SetActive(false);
        //处理GameOp
        //
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
                        case ProtoBuf.MJGameOP.MJ_OP_PREP:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                            break;
                        //case ProtoBuf.MJGameOP.MJ_OP_X3Z:
                        //    break;
                        case ProtoBuf.MJGameOP.MJ_OP_XQ:
                            if (seat == 0)
                                inGm_UI.gmXuanQue_Bg.SetActive(true);

                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_MOPAI:
                            //UpdateMopai(mjRoom.roomCache.charList[x].charId, false, 0);

                            Debug.Log("ChongLian_5");
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:
                            if (/*GameManager.GM.GetPlayerNum(mjRoom.roomCache.charList[x].charId)*/ seat == 0)
                                yn_KeYiChuPai = true;
                            Debug.Log("ChongLian_6");
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GUO:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_PENG:
                            Debug.Log("ChongLian_8");
                            if (mjRoom.roomCache.charList[x].oriCharId != 0 && mjRoom.roomCache.charList[x].oriCharId != mjRoom.roomCache.charList[x].charId)
                            {
                                //UpdateChupai(mjRoom.roomCache.charList[x].oriCharId, mjRoom.roomCache.charList[x].cardList[0]);
                                Debug.Log("ChongLian_8_1");
                                MJProto.Inst().PopcardIsWaiting = false;
                                MJProto.Inst().isPengGangHu = true;
                                //清楚打出去的牌
                            }
                            if (seat == 0)
                                PublicEvent.GetINS.Fun_reciveCanPlay(mjRoom.roomCache.charList[x].charId, ProtoBuf.MJGameOP.MJ_OP_PENG, mjRoom.roomCache.charList[x].cardList, mjRoom.roomCache.charList[x].oriCharId);


                            //碰
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG:
                            //gang
                            Debug.Log("ChongLian_9");
                            if (mjRoom.roomCache.charList[x].oriCharId != 0 && mjRoom.roomCache.charList[x].oriCharId != mjRoom.roomCache.charList[x].charId)
                            {
                                //UpdateChupai(mjRoom.roomCache.charList[x].oriCharId, mjRoom.roomCache.charList[x].cardList[0]);
                                Debug.Log("ChongLian_9_1");
                                MJProto.Inst().PopcardIsWaiting = false;
                                MJProto.Inst().isPengGangHu = true;
                            }
                            else
                            {
                                //UpdateMopai(mjRoom.roomCache.charList[x].charId, false, mjRoom.roomCache.charList[x].cardList[0]); 
                            }
                            if (seat == 0)
                                PublicEvent.GetINS.Fun_reciveCanPlay(mjRoom.roomCache.charList[x].charId, ProtoBuf.MJGameOP.MJ_OP_GANG, mjRoom.roomCache.charList[x].gangList, mjRoom.roomCache.charList[x].oriCharId);

                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG2:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG3:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_GANG4:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_HU:
                            if (mjRoom.roomCache.charList[x].oriCharId != 0 && mjRoom.roomCache.charList[x].oriCharId != mjRoom.roomCache.charList[x].charId)
                            {
                                //UpdateChupai(mjRoom.roomCache.charList[x].oriCharId, mjRoom.roomCache.charList[x].cardList[0]);
                                Debug.Log("ChongLian_10_1");
                                MJProto.Inst().PopcardIsWaiting = false;
                                MJProto.Inst().isPengGangHu = true;
                            }
                            if (seat == 0)
                                PublicEvent.GetINS.Fun_reciveCanPlay(mjRoom.roomCache.charList[x].charId, ProtoBuf.MJGameOP.MJ_OP_HU, mjRoom.roomCache.charList[x].cardList, mjRoom.roomCache.charList[x].oriCharId);

                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_TING:
                            if (seat == 0)
                                Mj_OpenMay_BaoPai(mjRoom.roomCache.charList[x].charId, mjRoom.roomCache.charList[x].cardList);
                            break;
                        //case ProtoBuf.MJGameOP.MJ_OP_X3Z_RESULT:
                        //    break;
                        case ProtoBuf.MJGameOP.MJ_OP_ROUND_OVER:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_VOTE_JSROOM:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_VOTE_RESULT:
                            break;
                        //case ProtoBuf.MJGameOP.MJ_OP_QUERY_MULTIPLY:
                        //    break;
                        case ProtoBuf.MJGameOP.MJ_OP_ON_LINE:

                            break;
                        case ProtoBuf.MJGameOP.MJ_PLAYER_OP_RESULT:
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_FEI:
                            if (seat == 0)
                                Mj_OpenMay_FeiPeng(mjRoom.roomCache.charList[x].charId, mjRoom.roomCache.charList[x].cardList[0]);
                            break;
                        case ProtoBuf.MJGameOP.MJ_OP_TI:
                            if (seat == 0)
                                Mj_OpenMay_TiPai(mjRoom.roomCache.charList[x].cardList[0]);
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
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError("Get Error _" + e.Message);
        //    throw;
        //}

    }

    //碰杠胡委托总接口
    void IPengGangHu(ProtoBuf.MJGameOP MjOp, uint charid, uint card, uint oriCharid = 0)
    {
        Debug.Log("调用了么");


        switch (MjOp)
        {

            case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:
                ZhuangFirstChupai(charid);

                break;

            case ProtoBuf.MJGameOP.MJ_OP_GUO:
                //MJProto.Inst().IsPopCardDown = true;/*ErrorDelete*/
                MJProto.Inst().isPengGangHu = false;
                if (card != mjGameInfo_M.mj_Cp_CardInfo.mJCardID)
                {
                    yn_KeYiChuPai = true;
                    //自己摸牌杠 过后可以出牌
                }
                break;
            case ProtoBuf.MJGameOP.MJ_OP_PENG:

                MJProto.Inst().isPengGangHu = true;
                _iFC_Update.UpdatePengPai(charid, card);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_GANG:

                MJProto.Inst().isPengGangHu = true;
                _iFC_Update.UpdateGangPai(charid, card, oriCharid);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_HU:

                MJProto.Inst().isPengGangHu = true;
                _iFC_Update.UpdateHule(charid, card, oriCharid);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_TING:
                UpdateBao(charid);
                break;

            case ProtoBuf.MJGameOP.MJ_OP_FEI:
                UpdateFeiPeng(charid, card, oriCharid);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_TI:
                UpdateTiPai(charid, card);
                break;

            default:
                break;

        }
    }

    /// <summary>设置其他玩家手牌的旋转角度， 两种。 True=立着。False = 正躺
    /// </summary>
    /// <param name="bb"></param>
    void SetHandCardRotation(bool bb)
    {
        for (int i = 1; i < allPlayerMJAry.Length; i++)
        {
            if (allPlayerMJAry[i].t_ShouPai_Parent != null && allPlayerMJAry[i].t_ShouPai_Parent.GetComponent<HandCard_T>() != null)
            {
                if (bb)
                {
                    allPlayerMJAry[i].t_ShouPai_Parent.GetComponent<HandCard_T>().TS_LiZhe();
                }
                else
                {
                    allPlayerMJAry[i].t_ShouPai_Parent.GetComponent<HandCard_T>().TS_ZhengTang();
                }
            }
        }
    }

    /// <summary>更新缺牌材质
    /// </summary>
    void Que_UpdateMeshRender()
    {
        StartCoroutine(Que_InvokeXXX());
    }
    IEnumerator Que_InvokeXXX()
    {
        bool isInvekeX = false;
        while (isInvekeX == false)
        {
            Debug.Log("QUE Update MeshRendrer AAAAAAA");
            List<MJInfos> queList = allPlayerMJAry[0].Que_GetAllQue();
            Debug.Log("QUE Update MeshRendrer" + queList.Count);
            int i_QueCount = queList.Count;
            for (int i = 0; i < queList.Count; i++)
            {
                i_QueCount++;
                if (queList[i] != null && queList[i].transform.childCount > 0)
                {
                    queList[i].transform.GetChild(0).GetComponent<MeshRenderer>().material = locaHandCard_Que;
                }
            }
            isInvekeX = true;
        }
        yield return null;
    }

    /// <summary>是否选缺中，缺必须先打完
    /// </summary>
    bool Que_IsThere(GameObject gmMaJ)
    {
        if (DataManage.Instance.MjXuanQue == 0)
        {//没有选缺
            return true;
        }

        int iValue = allPlayerMJAry[0].Que_GetAllQue().Count;
        bool bResult = false;
        if (iValue == 0)
        {//剩余缺牌的数量
            return true;
        }
        else
        {
            bResult = gmMaJ.name.IndexOf(allPlayerMJAry[0].Que_TYPE()) >= 0;
            return bResult;
        }

        //return false;
    }

    /// <summary>本地手牌排序
    /// </summary>
    void LocaHandCardsSort()
    {
        if (DataManage.Instance.MjXuanQue != 0)
        {
            allPlayerMJAry[0].Que_UpdateHandCards();
        }
        else
        {
            allPlayerMJAry[0].ShouPai_RankMYAll();
        }
    }

    /// <summary>结束界面 准备按钮
    /// </summary>
    void OnClick_Btn_End1Setout()
    {
        Debug.Log("OnClick_Btn_End1Setout");
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);

        inGm_UI.Init_BaoTing();

        inGm_UI.OnClick_Btn_SetoutStart();
        inGm_UI.needMJ_Ui.OpenOrClose_ZhuangJia(false);
        inGm_UI.uiWin_End1_MJ.btn_End1Setout.interactable = false;
    }

    void Chat_V_P(uint p_Id)
    {
        inGm_UI.Chat_V_P(p_Id);
    }
    #endregion
}

/// <summary>关于麻将点击（点击的高度，以及还原的高度、以及选中的麻将）
/// </summary>
[Serializable]
public struct Mj_ClickHight
{
    public Vector3 mj_Click_Down;
    public Vector3 mj_Click_Up;
    /// <summary>当前点击的麻将对象
    /// </summary>
    public Transform mj_Select_Object;

    /// <summary>当前点击的麻将对象
    /// </summary>
    public Transform mj_DownDrag_Object;

    public bool isSelect;

    public Mj_ClickHight(Transform t_, float heig = 32, bool isSelect_ = false)
    {
        mj_Click_Down = t_.position;
        mj_Click_Up = new Vector3(t_.position.x, t_.position.y + heig, t_.position.z);
        mj_Select_Object = t_;
        isSelect = isSelect_;
        mj_DownDrag_Object = null;
    }

    public void Mj_Click_Move(bool yn_)
    {
        isSelect = yn_;
        mj_Select_Object.position = yn_ ? mj_Click_Up : mj_Click_Down;
    }
}

[Serializable]
public struct MJGameC_Info
{
    /// <summary>记录最新出牌ID
    /// </summary>
    public uint mj_Cp_CharId;
    public Mj_Sx_ mj_Cp_CardInfo;
    public Transform mj_Cp_Transform;

    /// <summary>记录最新摸牌ID
    /// </summary>
    public uint mj_Mp_CharID;
    public Mj_Sx_ mj_Mp_CardInfo;

    public uint oriCharId;

    /// <summary>记录服务器通知的可以碰
    /// </summary>
    public uint keYi_Peng;
    /// <summary>记录服务器通知的可以杠
    /// </summary>
    public uint keYi_Gang;
    /// <summary>记录服务器通知的可以胡
    /// </summary>
    public uint keYi_Hu;

    /// <summary>记录服务器通知的可以飞碰的牌
    /// </summary>
    public uint keYi_FeiPeng;

    /// <summary>记录服务器通知的可以提牌
    /// </summary>
    public uint keYi_TiPai;

    /// <summary>选缺
    /// </summary>
    public uint queCardTYPE;

    /// <summary>是否发生了杠，并从麻将牌墙倒着摸牌
    /// </summary>
    public bool yn_EndReducePaiQ;// = false;
    public MJGameC_Info(string s_)
    {
        mj_Cp_CharId = 0;
        mj_Cp_CardInfo = new Mj_Sx_();
        mj_Cp_Transform = null;
        mj_Mp_CharID = 0;
        mj_Mp_CardInfo = new Mj_Sx_();
        yn_EndReducePaiQ = false;

        oriCharId = 0;

        keYi_Peng = 0;
        keYi_Gang = 0;
        keYi_Hu = 0;

        keYi_FeiPeng = 0;
        keYi_TiPai = 0;
        queCardTYPE = 0;
    }

    public void Set_ChuPai(uint charID, Mj_Sx_ cpSX)
    {
        mj_Cp_CharId = charID;
        mj_Cp_CardInfo = cpSX;
    }

    public void Set_MoPai(uint charID, Mj_Sx_ mpSX)
    {
        mj_Mp_CharID = charID;
        mj_Mp_CardInfo = mpSX;
    }
}

/// <summary>关于麻将拖动排序
/// </summary>
public class MjDragRank
{
    
    private GameObject dragObj;
    
    private GameObject dragTargetObj;
    
    private int dragObj_Index;
    
    private int dragTargetObj_Index;

    /// <summary>记录正在被拖动的麻将
    /// </summary>
    public GameObject DragObj
    {
        get
        {
            return dragObj;
        }

        set
        {
            dragObj = value;
        }
    }

    /// <summary>记录拖动麻将 触发到的麻将
    /// </summary>
    public GameObject DragTargetObj
    {
        get
        {
            return dragTargetObj;
        }

        set
        {
            dragTargetObj = value;
        }
    }

    /// <summary>被拖动麻将在 麻将位置数组的Index
    /// </summary>
    public int DragObj_Index
    {
        get
        {
            return dragObj_Index;
        }

        set
        {
            dragObj_Index = value;
        }
    }

    /// <summary>拖动麻将触发碰到的麻将 在 麻将位置数组的Index
    /// </summary>
    public int DragTargetObj_Index
    {
        get
        {
            return dragTargetObj_Index;
        }

        set
        {
            dragTargetObj_Index = value;
        }
    }
    public MjDragRank()
    {
    }

    public void Init_()
    {
        DragObj = null;
        DragTargetObj = null;
        DragObj_Index = 0;
        DragTargetObj_Index = 0;
    }
}

/// <summary>作用于记录鼠标点击的位置，当点击中移动距离大于多少才能达成开始拖动
/// </summary>
[System.Serializable]
public class InputMouse_
{
    public Vector2 v2_DownPosition;
    public const int i_MayDragDistance = 5;
    public GameObject gmMoveObj;
    public bool isDragStart = false;
    public InputMouse_()
    {

    }
    public InputMouse_(Vector2 mouseDownPosition)
    {
        isDragStart = false;
        v2_DownPosition = mouseDownPosition;
    }
}

/// <summary>爆牌？听牌
/// </summary>
[System.Serializable]
public class BaoTingCCC
{
    public bool isTing = false;
    public System.Action tingStart;
    public uint tingCharid;
    public List<uint> listTingHands = new List<uint>();

    public BaoTingCCC()
    {
    }

    public void Init_TingEvent()
    {
        tingCharid = 0;
        isTing = false;
        tingStart = null;
        listTingHands.Clear();
    }
}