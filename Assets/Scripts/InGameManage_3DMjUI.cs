using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameManage_3DMjUI : UiWin_Parent
{
    public Start_Mj_NeedUI needMJ_Ui;//= new Start_Mj_NeedUI();
    public InGameManage_3DMJ _inGameM_3DMJ;
    public INGamePlayer_Ui[] allPlayerInfoUis;
    public PhizAnima phizAnima_M;
    //public SVMJDrag svMJDrag;//拖拽赋值
    public bool isInitSetActive = false;
    public Button btn_YaoQing;
    public Toggle tog_ZDDQ, tog_FeiPeng;

    /// <summary>单局结束界面
    /// </summary>
    public UiWin_End1_MJ uiWin_End1_MJ;

    public GameObject gmXuanQue_Bg;
    // Use this for initialization
    void Start()
    {
        if (!isInitVar)
        {
            Init_Var();
        }
    }

    public void Init_Var()
    {
        if (GameObject.Find("MjZhuoZi") != null)
        {
            _inGameM_3DMJ = GameObject.Find("MjZhuoZi").GetComponent<InGameManage_3DMJ>();
        }
        if (allPlayerInfoUis == null)
        {
            allPlayerInfoUis = transform.FindChild("All_PlayerUiBg").GetComponentsInChildren<INGamePlayer_Ui>();
        }
        phizAnima_M = transform.FindChild("Phiz_Bg").GetComponent<PhizAnima>();
        //svMJDrag= transform.FindChild("SVMJDrag").GetComponent<SVMJDrag>();
        transform.SetAsFirstSibling();

        needMJ_Ui = new Start_Mj_NeedUI(transform);
        if (GameManager.GM.GmType == GameSceneType.gm_MjZhanJiHuiFang)
        {
            needMJ_Ui.Prep_OpenOrClose_Btn(false);
        }
        gmXuanQue_Bg.SetActive(false);
        Set_OnEventList<Button>(gmXuanQue_Bg.GetComponentsInChildren<Button>());
        Set_OnEventList<Button>(new Button[] {
            needMJ_Ui.btn_Peng_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Gang_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Hu_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_Guo_Pai.GetComponent<Button>(),
            needMJ_Ui.btn_SetoutStart.GetComponent<Button>(),
            needMJ_Ui.btn_BaoPai.GetComponent<Button>(),
            needMJ_Ui.btn_FeiPai.GetComponent<Button>(),
            needMJ_Ui.btn_TiPai.GetComponent<Button>()
        });

        Button[] btnAry_Right = transform.FindChild("Img_BtnBg0").GetComponentsInChildren<Button>();
        Set_OnEventList<Button>(btnAry_Right);
        btn_YaoQing.onClick.RemoveAllListeners();
        btn_YaoQing.onClick.AddListener(delegate() { SdkEvent.Instance.OnClick_Btn_GameYaoQing(); });

        tog_ZDDQ.onValueChanged.AddListener(delegate(bool bl) { this.OnTog_ZiDongDuiQi(tog_ZDDQ); });
        tog_FeiPeng.onValueChanged.AddListener(delegate (bool bl) { this.OnTog_FeiPeng(tog_FeiPeng); });


        DataManage.Instance.onChangePlayerData += this.UpdatePlayerUi;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
        
        needMJ_Ui.OpenOrClose_ZhuangJia(false);
        needMJ_Ui.Prep_CloseOKStyleAll();

        StartCoroutine(LoadChildScript());
        isInitVar = true;
    }

    void OnDisable()
    {
        for (int i = 0; i < allPlayerInfoUis.Length; i++)
        {
            if (allPlayerInfoUis[i] != null)
            {
                allPlayerInfoUis[i].Init_();
            }
        }
        tog_FeiPeng.isOn = true;
        tog_ZDDQ.isOn = true;
        tog_FeiPeng.interactable = true;
        tog_ZDDQ.interactable = true;
        uiWin_End1_MJ.gameObject.SetActive(false);
        btn_YaoQing.gameObject.SetActive(true);
        needMJ_Ui.Prep_OpenOrClose_Btn(true);
        needMJ_Ui.Prep_CloseOKStyleAll();
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);

        Init_BaoTing();
    }
    void OnDestroy()
    {
        DataManage.Instance.onChangePlayerData -= this.UpdatePlayerUi;
    }
    IEnumerator LoadChildScript()
    {
        bool b_Stop = false;
        while (!b_Stop)
        {
            int i_Count1 = 0;
            int i_Count2 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (allPlayerInfoUis[i].isInitVar)
                {//麻将牌面管理是否加载完成
                    i_Count1++;
                }
                //if (allPlayerInfoUis[i].isInitVar)
                //{//玩家信息显示UI 是否加载完成
                //    i_Count2++;
                //}
            }
            if (i_Count1 == 4 /*&& i_Count2 == 4*/)
            {

                b_Stop = true;
                gameObject.SetActive(isInitSetActive);
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.05f);
            }
        }

        yield return null;
    }

    public void StartSetRoomTime()
    {
        StartCoroutine(Repeating_Event(30,delegate() { SetRoomTime(); }));

    }

    #region/*———关于Toggle事件———*/
    void OnTog_ZiDongDuiQi(Toggle tog)
    {
        _inGameM_3DMJ.IsAlign = tog.isOn;
    }

    void OnTog_FeiPeng(Toggle tog)
    {
        _inGameM_3DMJ.IsFeiPeng = tog.isOn;
    }

    #endregion

    #region/*———关于按钮事件———*/

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        //base.Set_OnEventList<Component_>(all_);
        if (typeof(Button) == typeof(Component_))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                btn_.onClick.AddListener(delegate ()
                {
                    this.OnClick_(btn_.gameObject);
                });
            }
        }
    }

    void OnClick_(GameObject btn_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        Debug.Log("Click Button Name == " + btn_.gameObject.name);
        if (btn_.gameObject.name.IndexOf("Btn_XuanQue")>=0)
        {
            OnClick_Btn_XuanQue(btn_);

            return;
        }

        if (btn_.transform.parent.gameObject.name == "Img_BtnBg0")
        {
            switch (btn_.name)
            {
                case "Btn_OpenSetting":
                    Debug.Log("打开设置");
                    UIManager.Instance.ShowUI(AllPrefabName.uiWin_Setting, UIManager.Instance.canvas_T);
                    break;
                case "Btn_RequestQuitRoom":
                    if (GameManager.GM.GmType == GameSceneType.gm_MjGame)
                    {
                        If_OpenWindowQuitRoom();
                    }
                    else if (GameManager.GM.GmType == GameSceneType.gm_MjZhanJiHuiFang)
                    {
                        UIManager.Instance.SetUIobject(true, AllPrefabName.uiWin_Home);
                        UIManager.Instance.SetUIobject(true, AllPrefabName.uiWin_ZhanJi);
                        UIManager.Instance.DestroyObjectUI( AllPrefabName.UiWin_ZJHF);
                        MemoryPool_3D.Instance.MJ3D_RecycleALL();
                        DataManage.Instance.PData_RemoveOtherPlayerData();
                        Model3dManage.Instance.DestroyObjectModel("MJ_ZJHF");
                        Destroy(gameObject);
                    }

                    break;
                case "Btn_OpenChatPhiz":
                    if (GameManager.GM.GmType == GameSceneType.gm_MjGame)
                    {
                        UIManager.Instance.ShowUI(AllPrefabName.UiWin_PhizChat, UIManager.Instance.canvas_T);
                    }
                    break;
                default:
                    break;
            }
            return;
        }
        if (GameManager.GM.GmType == GameSceneType.gm_MjGame)
        {
            switch (btn_.name)
            {
                case "Btn_Peng_Pai":
                    OnClick_Btn_Peng();
                    break;
                case "Btn_Gang_Pai":
                    OnClick_Btn_Gang();
                    break;
                case "Btn_Hu_Pai":
                    OnClick_Btn_Hu();
                    break;
                case "Btn_Guo_Pai":
                    OnClick_Btn_Guo();
                    break;
                case "Btn_FeiPai":
                    OnClick_Btn_FeiPeng();
                    break;
                case "Btn_BaoPai":
                    OnClick_Btn_Bao();
                    break;
                case "Btn_TiPai":
                    OnClick_Btn_TiPai();
                    break;
                case "Btn_SetoutStart":
                    OnClick_Btn_SetoutStart();
                    break;
                    
                default:
                    break;
            }
        }
    }
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
                PublicEvent.GetINS.VoteRequest(true);
                break;
            case 4:
                PublicEvent.GetINS.VoteRequest(false);
                break;
            case 5:
                PublicEvent.GetINS.OnExitRoom();
                break;
            default:
                break;
        }

        //if (this.quitRoom_Ui.yn_Click > 0)
        //{
        //    this.quitRoom_Ui.OpenOrClsoe_BtnTouPiao(this.quitRoom_Ui, true);
        //}
        
    }
    //float waitTime_1 = 0;
    IEnumerator WaitTimeInveke_Event(float waitTime_1,UnityEngine.Events.UnityAction event_)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(waitTime_1);
        event_.Invoke();
    }

    IEnumerator Repeating_Event(float waitTime_1, UnityEngine.Events.UnityAction event_)
    {

        while (gameObject.activeInHierarchy)
        {
            event_.Invoke();
            yield return new WaitForSeconds(waitTime_1);
        }

    }

    /// <summary>录音按钮按下，拖拽赋值
    /// </summary>
    public void OnClickDown_MKF()
    {
        Debug.Log("录音Start");
        Event.Inst().StartMic();
        gmMkfStyle = UIManager.Instance.ShowUI(AllPrefabName.Img_MkfStyle,transform);
        gmMkfStyle.SetActive(true);
    }
    GameObject gmMkfStyle;
    /// <summary>录音按钮抬起，拖拽赋值
    /// </summary>
    public void OnClickUp_MKF()
    {
        Debug.Log("录音END");
        MicphoneTest.IsCancelMic = false;
        Event.Inst().StartMic();
        gmMkfStyle.SetActive(false);
    }
    public void OnClick_Btn_SetoutStart()
    {
        PublicEvent.GetINS.Fun_SentClientPre();
        Debug.Log("Click Setout Start");
    }

    void OnClick_Btn_Peng()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);
        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));

        PublicEvent.GetINS.Fun_SentPeng(_inGameM_3DMJ.mjGameInfo_M.keYi_Peng);
        _inGameM_3DMJ.mjGameInfo_M.keYi_Peng = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    void OnClick_Btn_Gang()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);
        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));

        PublicEvent.GetINS.Fun_SentGang(_inGameM_3DMJ.mjGameInfo_M.keYi_Gang, _inGameM_3DMJ.mjGameInfo_M.oriCharId);
        _inGameM_3DMJ.mjGameInfo_M.keYi_Gang = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    void OnClick_Btn_Hu()
    {
        //W3Debug_.Instance.W3Log(W3DebugType.Ask_Hu.ToString() + "|" + cardid);

        //Audio_Manage.Instance.Play_Audio(Resources.Load<AudioClip>(AudioPath_.audioBtnPath_ + AudioPathBtn_.Button_Click.ToString()));
        Debug.Log("you HuPai？");
        PublicEvent.GetINS.Fun_SentHu(_inGameM_3DMJ.mjGameInfo_M.keYi_Hu, _inGameM_3DMJ.mjGameInfo_M.oriCharId);

        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    void OnClick_Btn_Bao()
    {
        //声音
        PublicEvent.GetINS.Fun_SentBao(0);
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }
    void OnClick_Btn_FeiPeng()
    {
        Debug.Log("Click_ Fei");
        //声音
        PublicEvent.GetINS.Fun_SentFeiPeng(_inGameM_3DMJ.mjGameInfo_M.keYi_FeiPeng);
        _inGameM_3DMJ.mjGameInfo_M.keYi_FeiPeng = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
        _inGameM_3DMJ.mjGameInfo_M.keYi_FeiPeng = 0;
    }
    void OnClick_Btn_TiPai()
    {
        //声音
        PublicEvent.GetINS.Fun_SentTiPai(_inGameM_3DMJ.mjGameInfo_M.keYi_TiPai);
        _inGameM_3DMJ.mjGameInfo_M.keYi_TiPai = 0;
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    public void OnClick_Btn_Guo()
    {
        //声音
        PublicEvent.GetINS.Fun_SentGuo(0);
        needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.背景关闭所有按钮, false);
    }

    void OnClick_Btn_XuanQue(GameObject btn_XuanQue)
    {
        uint i_Value = 0;
        switch (btn_XuanQue.name)
        {
            case "Btn_XuanQue_W":
                i_Value = 1;
                break;
            case "Btn_XuanQue_B":
                i_Value = 2;
                break;
            case "Btn_XuanQue_T":
                i_Value = 3;
                break;
        }
        PublicEvent.GetINS.Fun_SentSelectQue(i_Value);
        gmXuanQue_Bg.SetActive(false);
    }

    public void END_AddOnClick(Button[] btnAry)
    {
        Button[] btn_AllC = btnAry;//
        for (int i = 0; i < btn_AllC.Length; i++)
        {//设定一下按钮事件
            Button btnNEW = btnAry[i];
            btnNEW.onClick.RemoveAllListeners();
            switch (btnNEW.gameObject.name)
            {
                case "Btn_Share_Data":
                    btnNEW.onClick.AddListener(delegate ()
                    {
                        OnClick_END_Btn_Share();
                    }
                    );

                    break;
                //case "Btn_MjEnd_Start_Game":
                //    btnNEW.onClick.AddListener(delegate ()
                //    {
                //        OnClick_END_Btn_MjEnd_Start();
                //    });
                //    break;
                case "Btn_CloseWindow":
                    btnNEW.onClick.AddListener(delegate ()
                    {
                        OnClick_END_Btn_CloseWindow();
                    });
                    break;
                default:
                    break;
            }
        }
    }

    //关于结束界面按钮事件
    void OnClick_END_Btn_CloseWindow()
    {

    }
    void OnClick_END_Btn_Share()
    {

    }

    void OnClick_END_Btn_MjEnd_Start()
    {//结束界面点击准备按钮
        
        
    }

    #endregion;

    #region/*———Player Ui Show———*/
    public void UpdatePlayerUi(Player_Data[] list_RoomPData)
    {
        if (gameObject.activeInHierarchy == false)
        {
            Debug.Log("<color=red>没有进行麻将游戏</color>");
            return;
        }
        if (list_RoomPData.Length > allPlayerInfoUis.Length)
        {
            Debug.LogError("<color=red>这里可能出现错误！</color>");
        }
        SetLeftTop_UI();
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

    #endregion
    public void SetLeftTop_UI()
    {
        if (DataManage.Instance._roomEnterRsp!=null&&
            DataManage.Instance._roomEnterRsp.mjRoom!=null
            )
        {
            needMJ_Ui.t_RoomID.text = "房间号：" + DataManage.Instance._roomEnterRsp.mjRoom.roomId.ToString();
            needMJ_Ui.t_ShengYuJuShu.text = "局数：" + DataManage.Instance._roomEnterRsp.mjRoom.roomRuleInfo.xwmjRule.roundNum.ToString();

            needMJ_Ui.t_RoomInfo.text = DataManage.Instance.RoomInfoNxStr;
            needMJ_Ui.t_ShengYuPaiShu.text = "";
        }
       
    }

    public void Update_JuShu()
    {
        needMJ_Ui.t_ShengYuJuShu.text ="局数："+DataManage.Instance.RoomSyJuShu.ToString();
    }

    public void Open_Player_ZhunBei(uint charID)
    {
        //Open_ZhunBeiFont(i);
        needMJ_Ui.Prep_OpenPrepStyle(DataManage.Instance.PData_GetIndex(charID), true);
        DataManage.Instance.PData_SetReady(charID, true);
        if (charID == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            needMJ_Ui.Prep_OpenOrClose_Btn(false);
            _inGameM_3DMJ.End_Init();
        }
        Debug.Log("准备了？？？" + charID);

    }
    public void Set_ShengYu_PaiShu(int shengyupai_)
    {
        if (DataManage.Instance.roomCardNumCount.ToString().Length>=2 && shengyupai_ == 0)
        {
            return;
        }
        DataManage.Instance.roomCardNumCount = shengyupai_;
        needMJ_Ui.t_ShengYuPaiShu.text = "剩余牌数：" + shengyupai_.ToString();
    }

    public void Open_PengGangHuBtn(MJOpBtnName btnName)
    {
        switch (btnName)
        {
            case MJOpBtnName.BtnPeng:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.碰, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnGang:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.杠, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnHu:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.胡, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnGuo:
                break;
            case MJOpBtnName.BtnFeiPai:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.飞碰, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            case MJOpBtnName.BtnTiPai:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.提, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;

            case MJOpBtnName.BtnBaoPai:
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.爆, true);
                needMJ_Ui.Open_Close_Mj_Need_UI(Mj_NeedUi_Btn_Name.过, true);
                break;
            default:
                break;
        }
    }

    public void Set_ZhuangJia(uint charid)
    {
        Debug.Log("Set ZhuangJia UI???" + charid);
        int i_Index = DataManage.Instance.PData_GetIndex(charid);// Get_Player_Index(zhuangJia_Id);
        needMJ_Ui.img_ZhuangJia.SetParent(allPlayerInfoUis[i_Index].btnHead.transform.parent.parent);
        needMJ_Ui.OpenOrClose_ZhuangJia(true);
        needMJ_Ui.img_ZhuangJia.anchoredPosition = Vector2.one;
        //Update_ZhiZhen_R(zj_Id);//更新指针？
    }

    public void Chat_V_P(uint p_Id)
    {
        allPlayerInfoUis[DataManage.Instance.PData_GetIndex(p_Id)].img_VoiceStyle.gameObject.SetActive(true);
        StartCoroutine(WaitTimeInveke_Event(6, delegate ()
        {
            if (gameObject.activeInHierarchy)
            {
                allPlayerInfoUis[DataManage.Instance.PData_GetIndex(p_Id)].img_VoiceStyle.gameObject.SetActive(false);
            }
        }));
    }

    public void SetIsOnLine(uint charid, uint isOnLine)
    {
        allPlayerInfoUis[DataManage.Instance.PData_GetIndex(charid)].SetIsOnLine(isOnLine==1);
    }


    public void SetRoomTime()
    {

        System.DateTime moment = System.DateTime.Now;
        // Year gets 1999.
        int year = moment.Year;
        // Month gets 1 (January).
        int month = moment.Month;
        // Day gets 13.
        int day = moment.Day;
        
        needMJ_Ui.t_RoomTime.text = year + "年" + month + "月" + day + "日" + "\t" + moment.Hour + ":" + moment.Minute;
        //Debug.Log(year + "年" + month + "月" + day + "日" + "\t" + moment.Hour + ":" + moment.Minute);
    }

    public void Init_BaoTing()
    {
        for (int i = 0; i < allPlayerInfoUis.Length; i++)
        {
            allPlayerInfoUis[i].BaoTing_OpenOrClose(false);
        }
    }

    /// <summary>显示爆听
    /// </summary>
    /// <param name="charid"></param>
    public void UpdateShowBaoTing(uint charid)
    {
        allPlayerInfoUis[DataManage.Instance.PData_GetIndex(charid)].BaoTing_OpenOrClose(true);
    }

}

public enum MJOpBtnName
{
    BtnPeng, BtnGang, BtnHu, BtnGuo, BtnTiPai, BtnFeiPai, BtnBaoPai
}
