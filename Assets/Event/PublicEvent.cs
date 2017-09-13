using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using ProtoBuf;



public class PublicEvent
{

    static PublicEvent _INS;

    public static PublicEvent GetINS
    {
        get
        {
            if (_INS == null)
            {
                _INS = new PublicEvent();
            }
            return _INS;
        }
    }

    public PublicEvent()
    {
        _INS = this;
    }

    #region 登录模块

    public Action LoginRest;

    /// <summary>
    /// 玩家登录转发
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="ID"></param>
    public void AppLogin(string Name, string ID)
    {
        GlobalSettings.LoginUserName = Name;
        GlobalSettings.LoginUserId = ID;
        LoginProcessor.Inst().Login();

        //LoginProcessor.Inst().AccountLoginRequest();
    }
    public void AppLoginOut()
    {
        LoginProcessor.Inst().ApplyLogout();
    }

    /// <summary>
    /// 登录的返回结果的委托
    /// </summary>
    /// <param name="Result"></param>
    public delegate void Delegate_LoginResult(ProtoBuf.AccountLoginRsp Result);
    /// <summary>
    /// 登录的返回结果的委托事件
    /// </summary>
    /// <param name="Result"></param>
    public event Delegate_LoginResult Event_LoginResult;
    /// <summary>
    /// 调用登录返回结果的委托事件结果
    /// </summary>
    /// <param name="Result"></param>
    public void Fun_LoginResult(ProtoBuf.AccountLoginRsp Result)
    {


        if (Result.result == ProtoBuf.AccountLoginRsp.Result.ACCOUNT_LOGIN_SUCCESS)
        {
            PublicEvent.GetINS.DiamondRequst();
            DataManage.Instance.PData_InitMyData(new Player_Data(Result.userName.ToName(), Result.charId, Result.ip, GlobalSettings.avatarUrl, (int)Result.diamond, (int)Result.diamond, BaseProto.playerInfo.sex));
            /*"http://localhost/Image/tx_1.png"*/

            BaseProto.Inst().EnterGameRequest(ProtoBuf.GameType.GT_GC/*GT_TT*/);
            if (Result.atRoomId == 0)
            {
                //没在游戏中
                Debug.Log("登陆成功，并创建大厅");

                if (LoginRest != null)
                {
                    LoginRest();
                }

            }
            else
            {
                //在游戏中
                //直接收到roominfor
                //or 再次申请加入原房间号
                if (GameManager.GM.IsRelink_C)
                {
                    GameManager.GM.Relink_InitDelete();
                }
                Debug.Log("重新连接:" + Result.atRoomId);

                AppJoin(Result.atRoomId);
            }
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_Relink);
        }
        else
        {
            Debug.Log("没有登录成功哦！");
            //var window = GameManager.GM.SearchEmpty().AddComponent<Notic>();
            //window.Ins();
            //window.InputText(true, "\n\n没有登录成功哦！"+ Result.result);
            //window.GraduallyRest();

            if (LoginFail != null)
                LoginFail();
        }
    }

    public Action LoginFail;
    #endregion
    #region 进入大厅

    /// <summary>
    /// 进入大厅成功 触发方法引用
    /// </summary>
    public Action Event_SuccessIntoHall;
    /// <summary>
    /// 进入大厅失败 触发方法引用
    /// </summary>
    public Action Event_FaillIntoHall;
    /// <summary>
    /// 进入大厅成功 触发方法
    /// </summary>
    public void Fun_SuccessIntoHall()
    {
        if (Event_SuccessIntoHall != null)
            Event_SuccessIntoHall();
    }
    /// <summary>
    /// 进入大厅失败 触发方法
    /// </summary>
    public void Fun_FaillIntoHall()
    {
        Event_FaillIntoHall();
    }


    ///// <summary>
    ///// 接收玩家头像信息的委托
    ///// </summary>
    //public delegate void RecivePlayerHeadData(string head);
    ///// <summary>
    ///// 接收玩家头像信息的委托的事件
    ///// </summary>
    //public event RecivePlayerHeadData Event_playerHeadData;
    ///// <summary>
    ///// 接收玩家头像信息事件的触发方法
    ///// </summary>
    //public void Fun_playerHeadData(string head)
    //{
    //    Event_playerHeadData(head);
    //}
    /// <summary>
    /// 接收公告信息的委托
    /// </summary>
    public delegate void RecivePublicText(string value);
    /// <summary>
    /// 接收公告信息的委托事件
    /// </summary>
    public event RecivePublicText Event_recivePublicText;
    /// <summary>
    /// 接收服务器发送的公告信息
    /// </summary>
    public void Fun_recivePublicText(string value)
    {
        Event_recivePublicText(value);
    }
    /// <summary>
    /// 接收玩家的战绩得分的委托
    /// </summary>
    public delegate void ReciveCombatGains(List<string[]> vlaue);
    /// <summary>
    /// 接收玩家的战绩得分的委托的事件
    /// </summary>
    public event ReciveCombatGains Event_reciveCombatGains;
    /// <summary>
    /// 接收玩家的战绩得分的触发方法
    /// </summary>
    public void Fun_reciveCombatGains(List<string[]> value)
    {
        Event_reciveCombatGains(value);
    }
    /// <summary>
    /// 接收Message的委托
    /// </summary>
    public delegate void ReciveMessage(string value);
    /// <summary>
    /// 接收Message的委托方法
    /// </summary>
    public event ReciveMessage Event_reciveMessage;
    /// <summary>
    /// 接收玩家的战绩得分的触发方法
    /// </summary>
    public void Fun_reciveMessage(string value)
    {
        Event_reciveMessage(value);
    }
    /// <summary>
    /// 关掉玩家信息窗口委托
    /// </summary>
    public Action PlayerInformationRest;

    /// <summary>更新钻石
    /// </summary>
    /// <param name="rsp"></param>
    public void ReciveDiamond(QueryInfoRsp rsp)
    {
        if (ReciveDiamon != null)
        {
            //ReciveDiamon((int)rsp.charInfoDy.diamond);
            BaseProto.playerInfo.m_diamond = rsp.charInfoDy.diamond;
            DataManage.Instance.PData_GetData(BaseProto.playerInfo.m_id).Set_Diamond = (int)BaseProto.playerInfo.m_diamond;

        }
    }

    /// <summary>请求钻石数据
    /// </summary>
    public void DiamondRequst()
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.queryType = QueryInfoReq.QueryType.CharInfo;
        pack.param2 = 0;//
        BaseProto.Inst().QueryInfoRequest(pack);
    }

    public Action<int> ReciveDiamon;

    /// <summary>发送 请求分享
    /// </summary>
    public void SentShare()
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.queryType = QueryInfoReq.QueryType.CharInfo;
        pack.param2 = 0;//
        BaseProto.Inst().SendShar(pack);
    }

    /// <summary>接收 关于分享得到钻石
    /// </summary>
    public void RcvShare(QueryInfoRsp rsp)
    {
        //等于当前钻石

        BaseProto.playerInfo.m_diamond = rsp.param2;
        DataManage.Instance.PData_GetData(BaseProto.playerInfo.m_id).Set_Diamond = (int)BaseProto.playerInfo.m_diamond;

    }

    #endregion

    #region 战绩以及回放
    public void ZhanjiHuiFangRequst()
    {
        QueryInfoReq pack = new QueryInfoReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.queryType = QueryInfoReq.QueryType.ZhanJi;
        //pack.gameType = 0;

        pack.param1 = 0;
        pack.param2 = 9;

        BaseProto.Inst().QueryInfoRequest(pack);
    }

    public Action<QueryInfoRsp> Fun_ReciveZhanJiHuiFang;
    public void ReciveZhanJiHuiFang(QueryInfoRsp rsp)
    {

        //if (RestZhanjiHuiFangWindow != null)
        //{
        //    RestZhanjiHuiFangWindow();
        //    //开始回放
        //    Debug.Log("开始回放");
        //}
        //else {

        //var window = GameManager.GM.SearchEmpty().AddComponent<CombatGains>();
        //window.Ins(rsp);

        if (Fun_ReciveZhanJiHuiFang != null)
            Fun_ReciveZhanJiHuiFang(rsp);

        if (RestNoticWindow != null)
        {
            RestNoticWindow();

            Debug.Log("打开战绩");

        }

        //}

    }

    public Action RestNoticWindow;

    //public Action RestZhanjiHuiFangWindow;

    //回放开始
    public Action ReViewStart;

    //回放结束
    public Action ReViewEnd;



    #endregion


    #region 创建房间

    /// <summary>
    /// 向服务器发送当前的房间规则
    /// </summary>
    /// <param name="RoomRule"></param>
    public void NewRoom(/*int[] RoomRule*/XWMJRule xwmjRule)
    {
        CreateRoomReq reqPack = new CreateRoomReq();

        reqPack.gameType = GameType.GT_XWMJ;
        reqPack.mjRoom = new MJRoomRuleInfo();
        reqPack.mjRoom.gameRule = MJRoomRule.MJ_ROOM_RULE_XWMJ;
        //XWMJRule xwMJRule = xwmjRule;
        reqPack.mjRoom.xwmjRule = xwmjRule;
        Debug.Log("tEST01");
        BaseProto.playerInfo.m_inGame = GameType.GT_XWMJ;//AddTime 2017年6月1日 10:08
        BaseProto.Inst().CreateRoomRequest(reqPack);
    }

    /// <summary>向服务器发送当前的房间规则
    /// </summary>
    /// <param name="RoomRule">十三水</param>
    public void NewRoom(/*int[] RoomRule*/THIRtRule thirtRule)
    {
        CreateRoomReq reqPack = new CreateRoomReq();

        reqPack.gameType = GameType.GT_THIRt;
        reqPack.mjRoom = new MJRoomRuleInfo();
        reqPack.mjRoom.gameRule = MJRoomRule.CD_ROOM_RULE_THURt;

        reqPack.mjRoom.thirtRule = thirtRule;
        Debug.Log("Request Create Room");
        BaseProto.playerInfo.m_inGame = GameType.GT_THIRt;//AddTime 2017年6月1日 10:08
        BaseProto.Inst().CreateRoomRequest(reqPack);
    }
    /// <summary>
    /// 向服务器发送当前的房间规则
    /// </summary>
    /// <param name="RoomRule"></param>
    public void NewRoom(/*int[] RoomRule*/DERule DaEr)
    {
        CreateRoomReq reqPack = new CreateRoomReq();

        reqPack.gameType = GameType.GT_DE;
        reqPack.mjRoom = new MJRoomRuleInfo();
        reqPack.mjRoom.gameRule = MJRoomRule.MJ_ROOM_RULE_DE;
        //XWMJRule xwMJRule = xwmjRule;
        reqPack.mjRoom.deRule = DaEr;
        Debug.Log("tEST01");
        BaseProto.playerInfo.m_inGame = GameType.GT_DE;//AddTime 2017年6月1日 10:08
        BaseProto.Inst().CreateRoomRequest(reqPack);
    }


    /// <summary>
    /// 接收当前是否创建房间成功的委托
    /// </summary>
    public delegate void ReciveCreatRoomSuccess();
    public delegate void ReciveCreatRoomFail();
    public delegate void ReciveCreatRoomHas();
    /// <summary>
    /// 没钱了
    /// </summary>
    public delegate void ReciveCreatRoomNEM();
    /// <summary>
    /// 接收当前是否创建房间成功的委托方法
    /// </summary>
    public event ReciveCreatRoomSuccess Event_reciveCreatRoomSuccess;
    public event ReciveCreatRoomFail Event_reciveCreatRoomFail;
    public event ReciveCreatRoomHas Event_reciveCreatRoomHas;
    public event ReciveCreatRoomNEM Event_reciveCreatRoomNEM;
    /// <summary>
    /// 接收当前是否创建房间的委托事件的触发方法，有创建成功和创建失败
    /// </summary>
    public void Fun_reciveIsCreatRoom(CreateRoomRsp crp)
    {
        if (RestNoticWindow != null)
            RestNoticWindow();
        /*
       [global::ProtoBuf.ProtoEnum(Name = @"SUCC", Value = 1)]SUCC = 1,

       [global::ProtoBuf.ProtoEnum(Name = @"FAIL", Value = 2)]FAIL = 2,

       [global::ProtoBuf.ProtoEnum(Name = @"HAS", Value = 3)]HAS = 3,

       [global::ProtoBuf.ProtoEnum(Name = @"NOT_ENOUGH_MONEY", Value = 4)]NOT_ENOUGH_MONEY = 4
          */
        switch (crp.result)
        {
            case CreateRoomRsp.Result.SUCC:
                Debug.Log("创建房间成功！");
                switch (crp.gameType)
                {
                    case GameType.GT_NULL:
                        break;
                    case GameType.GT_GC:
                        break;
                    case GameType.GT_MJ:
                        break;
                    case GameType.GT_DE:
                        break;
                    case GameType.GT_XWMJ:
                        break;
                    case GameType.GT_THIRt:
                        break;
                    default:
                        break;
                }
                Event_reciveCreatRoomSuccess();
                break;
            case CreateRoomRsp.Result.FAIL:
                Debug.Log("\n\n创建房间失败！！");
                UiWin_Prompt.OpenPrompt("\n\n创建房间失败！！");
                break;
            case CreateRoomRsp.Result.HAS:
                Debug.Log("\n\n该账号已经存在在该房间内，创建房间失败！");
                //UiWin_Prompt.OpenPrompt("\n\n该账号已经存在在该房间内，创建房间失败！");
                PromptManage.AddPromptSelfClose(new PromptModelSelfClose("该账号已经存在在该房间内，创建房间失败！", 1));
                break;
            case CreateRoomRsp.Result.NOT_ENOUGH_MONEY:
                Debug.Log("\n\n房卡不足,创建房间失败！");
                //UiWin_Prompt.OpenPrompt("\n\n房卡不足,创建房间失败！");
                PromptManage.AddPromptSelfClose(new PromptModelSelfClose("房卡不足,创建房间失败！", 1));
                break;
            default:
                break;
        }
    }

    #endregion

    #region 加入房间
    public void AppJoin(uint RoomNum)
    {
        BaseProto.playerInfo.m_cdRoomId = RoomNum;
        BaseProto.Inst().EnterRoomRequest();
        //返回房间号码
    }
    /// <summary>
    /// 加入房间反馈成功的委托
    /// </summary>
    public delegate void JoinRoomSuccess();
    /// <summary>
    /// 加入房间反馈成功的委托事件
    /// </summary>
    public event JoinRoomSuccess Event_joinRoomSuccess;

    /// <summary>
    /// 加入房间反馈失败的委托
    /// </summary>
    public delegate void JoinRoomFail();
    /// <summary>
    /// 加入房间反馈失败的委托事件
    /// </summary>
    /// 
    public event JoinRoomFail Event_joinRoomFail;

    /// <summary>
    /// 加入房间反馈人满的委托事件
    /// </summary>
    public delegate void JoinRoomFull();
    /// <summary>
    /// 加入房间反馈人满的委托事件
    /// </summary>
    public event JoinRoomFull Event_joinRoomFull;

    /// <summary>
    /// 加入房间反馈人满的委托事件
    /// </summary>
    public delegate void JoinRoomHaSin();
    /// <summary>
    /// 加入房间反馈人满的委托事件
    /// </summary>
    public event JoinRoomFull Event_joinRoomHaSin;
    public void Fun_JoinResult(ProtoBuf.EnterRoomRsp rsp)
    {
        if (RestNoticWindow != null)
            RestNoticWindow();
        switch (rsp.result)
        {
            case EnterRoomRsp.Result.SUCC:
                //Debug.Log("进入游戏成功");

                Debug.Log("Fun_JoinResult,进入房间成功");


                if (LoginRest != null)
                {
                    Debug.Log("我要关掉Login");

                    LoginRest();
                }
                if (Event_reciveCreatRoomSuccess != null)
                {
                    Debug.Log("我要关掉CreatRoom");
                    Event_reciveCreatRoomSuccess();

                }
                //var window = GameManager.GM.SearchEmpty().AddComponent<Playroom_>();
                //window.Ins(rsp);

                if (Event_joinRoomSuccess != null)
                {
                    Debug.Log("我要关掉lobby");
                    Event_joinRoomSuccess();
                }
                DataManage.Instance.SetRoomEnterRsp = rsp;
                ReciveData(rsp.mjRoom);
                UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_Relink);

                //GameManager.GM.inGame_C.RoomRsp = rsp;
                break;
            case EnterRoomRsp.Result.FAIL:
                Debug.Log("房间不存在");
                //UiWin_Prompt.OpenPrompt("\n房间不存在，进入房间失败！");
                PromptManage.AddPromptSelfClose(new PromptModelSelfClose("房间不存在，进入房间失败！", 1));
                break;
            case EnterRoomRsp.Result.FULL:
                Debug.Log("房间已满");
                UiWin_Prompt.OpenPrompt("\n\n房间已满,进入房间失败！");
                break;
            case EnterRoomRsp.Result.HASIN:
                //Debug.Log("该账号已经存在在该房间内");
                //UiWin_Prompt.OpenPrompt("\n\n该账号已经存在在该房间内,进入房间失败！");
                PromptManage.AddPromptSelfClose(new PromptModelSelfClose("该账号已经存在在该房间内,进入房间失败！", 1));
                break;
            default:
                break;
        }

    }

    #endregion



    #region 游戏内
    /// <summary>
    /// 进入房间之后要加载的数据
    /// </summary>
    /// <param name="mjRoom"></param>
    public void ReciveData(ProtoBuf.MJRoomInfo mjRoom)
    {
        Debug.Log(mjRoom.inGame);
        //mjRoom.inGame == 6
        //int round = (int)mjRoom.roomRuleInfo.xzddRule.roundNum;
        int round = 1;
        if (mjRoom.roomRuleInfo.xwmjRule != null)
        {
            round = (int)mjRoom.roomRuleInfo.xwmjRule.roundNum;
        }
        else if (mjRoom.roomRuleInfo.thirtRule != null)
        {
            round = (int)mjRoom.roomRuleInfo.thirtRule.roundNum;
        }

        int me = 0;
        int Mypos = 0;
        int[] te = new int[GameManager.playerCount];
        int[] ty = new int[GameManager.playerCount];
        for (int i = 0; i < mjRoom.charIds.Count; i++)
        {
            if (mjRoom.charIds[i] == BaseProto.playerInfo.m_id)
            {
                //i是数据中玩家的位置 自己坐到0号座位
                te[i] = 0;
                me = i;
                Mypos = (int)mjRoom.charStates[i].position;
                //GameDate.MyPosition = Mypos;
                Debug.Log(Mypos + " Mypos");
                ty[0] = Mypos;
                for (int j = 1; j < GameManager.playerCount; j++)
                {
                    if (Mypos - j >= 0)
                    {
                        ty[j] = Mypos - j;
                    }
                    else
                        ty[j] = GameManager.playerCount + (Mypos - j);
                }
                for (int k = 0; k < ty.Length; k++)
                {
                    Debug.Log(ty[k] + "   Positon排序");
                }
                break;
            }
        }
        for (int i = 0; i < mjRoom.charIds.Count; i++)
        {
            for (int j = 0; j < ty.Length; j++)
            {
                if (mjRoom.charStates[i].position == ty[j])
                {
                    te[i] = j;
                    break;
                }
            }
        }
        Debug.Log(mjRoom.charIds.Count + " RoomID" + mjRoom.roomId);




        //for (int i = 0; i < mjRoom.charIds.Count; i++)
        //{
        //    if (mjRoom.charIds[i] == BaseProto.playerInfo.m_id)
        //    {
        //        //i是数据中玩家的位置 自己坐到0号座位
        //        te[i] = 0;
        //        me = i;
        //        //int num = 1;
        //        int num = 3;
        //        //重排TE[] 


        //        for (int j = i + 1; j < GameManager.playerCount; j++)                  /*自身为0 0123    0321 */
        //        {                                                                   /*自身为1 3012    1032   2103*/
        //            te[j] = num;
        //            //num++;
        //            num--;
        //            Debug.Log("+1");
        //        }


        //        for (int j = 0; j < i; j++)
        //        {
        //            te[j] = num;
        //            //num++;
        //            num--;

        //            Debug.Log("+1");

        //        }
        //        break;
        //    }
        //}

        //拿到排好序的[]



        //if (PublicEvent.GetINS.Fun_SameIpTip != null)
        //{
        //    PublicEvent.GetINS.Fun_SameIpTip(mjRoom.charIds.Count);

        //}



        if (mjRoom.roomRuleInfo.deRule != null)
        {
            round = (int)mjRoom.roomRuleInfo.deRule.roundNum;
            UiWin_DzpPlayRoom tempPlayRoom = UIManager.Instance.ShowUI(AllPrefabName.uiWin_DzpPlayRoom, UIManager.Instance.canvas_T).GetComponent<UiWin_DzpPlayRoom>();
            tempPlayRoom.setData(mjRoom);///就是在这里接入的大字牌！！！！！！
        }
        else
        {
            Player_Data[] test_AllData = new Player_Data[4];
            //Debug.Log(mjRoom.inGame+ "inGame1");
            InGameManage_3DMjUI gameMJUI = UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ).GetComponent<InGameManage_3DMjUI>();

            //按顺序放入GM中的ALLinfo中
            for (int i = 0; i < mjRoom.charIds.Count; i++)
            {


                //GameManager.GM.ReSetAllPlayerData(mjRoom.charInfos[i], te[i], mjRoom.charStates[i].restGold);
                Player_Data a_ = new Player_Data();

                a_.ReSetPlayerData(mjRoom.charInfos[i], mjRoom.charStates[i], te[i], mjRoom.charStates[i].isZB);
                test_AllData[i] = a_;

                //判断玩家是否在准备
                if (mjRoom.charStates[i].isZB > 0)
                {
                    if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_XWMJ)
                    {
                        gameMJUI.Open_Player_ZhunBei(mjRoom.charInfos[i].charId);
                    }
                    else if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                    {

                    }
                }
                //Debug.Log(mjRoom.inGame + "inGame2");

                if (mjRoom.inGame != 1)
                {
                    //是否在游戏中
                    for (int x = 0; x < DataManage.Instance.PData_GetDataAry().Length; x++)
                    {
                        DataManage.Instance.PData_GetDataAry()[x].isgaming = true;
                        DataManage.Instance.PData_GetDataAry()[x].isReady = false;
                        //GameManager.GM._AllPlayerData[te[i]].isReady = false;
                    }
                }
                //Debug.Log(mjRoom.inGame + "inGame3");

                //判断处在第一次以外的准备阶段 
                if (mjRoom.inGame == 1)
                {
                    if (round < 4 | (8 < round && round < 8))
                    {
                        for (int k = 0; k < 4; k++)
                        {

                            //GameManager.GM._AllPlayerData[k].isgaming = true;
                            Debug.Log("PlayerIsGameP...0");
                            if (k == me)
                            {
                                //PublicEvent.GetINS.Fun_SentClientPre();
                                PublicEvent.GetINS.Fun_SentClientPre();
                                Fun_ReUpdateWatingUi(mjRoom);
                            }
                        }

                    }
                    else if (round == 8 || round == 4)
                    {

                        for (int x = 0; x < mjRoom.charIds.Count; x++)
                        {//3.遍历玩家分数
                            if (mjRoom.charInfos[x].gold != 0)
                            {//若有玩家分数不等于0

                                for (int k = 0; k < 4; k++)
                                {

                                    //GameManager.GM._AllPlayerData[k].isgaming = true;
                                    Debug.Log("PlayerIsGameP...1");
                                    if (k == me)
                                    {
                                        //PublicEvent.GetINS.Fun_SentClientPre();
                                        PublicEvent.GetINS.Fun_SentClientPre();
                                        Fun_ReUpdateWatingUi(mjRoom);
                                    }
                                }

                            }
                        }
                    }
                }


                //Debug.Log(mjRoom.inGame + "inGame4");


                this.Fun_PlayerUpdata(te[i]);

            }

            if (PublicEvent.GetINS.Fun_SameIpTip != null)
            {
                PublicEvent.GetINS.Fun_SameIpTip(mjRoom.charIds.Count);

            }
            //for (int i = 0; i < mjRoom.charIds.Count; i++)
            //{
            //    this.Fun_PlayerUpdata(te[i]);
            //}

            //Debug.Log(mjRoom.inGame + "inGame5");
            //GameManager.GM.create_Room_Data.room_Boos_ID = test_AllData[0].p_ID;
            for (int i = 0; i < test_AllData.Length; i++)
            {
                if (test_AllData[i] == null)
                {
                    test_AllData[i] = new Player_Data();
                }
            }

            DataManage.Instance.roomBoosId = test_AllData[0].p_ID;
            test_AllData = DataManage.Instance.PData_Rank(test_AllData);// Player_Data_PaiXu.For_PaiXu(test_AllData);
            DataManage.Instance.PData_Update(test_AllData);
            //Gm_Manager.G_M.test_Look = new Player_Data[test_AllData.Length];
            //Gm_Manager.G_M.test_Look = test_AllData;

            if (mjRoom.inGame != 1)
            {
                //重新登录 并在游戏阶段事件
                //GameManager.GM._AllPlayerData[0].isgaming = true;
                GameObject inGm3DMj = Model3dManage.Instance.FindModel("MJGameController");
                if (inGm3DMj != null)
                {
                    if (inGm3DMj.GetComponent<InGameManage_3DMJ>().isInitSetVar == false)
                    {
                        inGm3DMj.GetComponent<InGameManage_3DMJ>().Init_Game();
                    }
                }

                Fun_ReUpdateMj(mjRoom);
                PublicEvent.GetINS.DiamondRequst();

            }
        }




    }
    /// <summary>
    /// 刷新玩家信息的委托
    /// </summary>
    public delegate void JoinPlayerData(ProtoBuf.CharacterInfo temp);
    public delegate void ExitPlayerData(ProtoBuf.CharacterInfo temp);
    public delegate void JoinRoomPlayerUpdata(int seat);
    /// <summary>
    /// 其它玩家加入时刷新玩家UI信息的委托事件
    /// </summary>
    public event JoinPlayerData Event_Join_PlayerData;
    public event ExitPlayerData Event_Exit_PlayerData;
    public event JoinRoomPlayerUpdata Event_PlayerUpdata;
    public void Fun_PlayerUpdata(int seat)
    {
        Debug.Log("其它玩家加入时刷新玩家UI信息的委托事件");
        if (Event_PlayerUpdata != null)
        {
            Event_PlayerUpdata(seat);
        }

    }

    /// <summary>
    /// 刷新玩家UI信息的委托事件的方法
    /// </summary>
    public void Fun_JoinPlayerData(ProtoBuf.CharacterInfo temp)
    {
        Debug.Log("加入进来的人的名字：" + temp.userName);
        Event_Join_PlayerData(temp);
    }
    public void Fun_ExitPlayerData(ProtoBuf.CharacterInfo temp)
    {
        Event_Exit_PlayerData(temp);
    }



    /// <summary>
    /// 等待过程中重连更新UI
    /// </summary>
    public delegate void ReUpdateWatingUi(ProtoBuf.MJRoomInfo mjRoom);
    public event ReUpdateWatingUi Event_ReUpdateWatingUi;
    public void Fun_ReUpdateWatingUi(ProtoBuf.MJRoomInfo mjRoom)
    {
        Debug.Log("等待过程中重连更新UI");
        if (Event_ReUpdateWatingUi != null)
        {
            Event_ReUpdateWatingUi(mjRoom);
        }
    }
    /// <summary>
    /// 游戏过程中重连更新麻将
    /// </summary>
    public delegate void ReUpdateMj(ProtoBuf.MJRoomInfo mjRoom);
    public event ReUpdateMj Event_ReUpdateMj;
    public void Fun_ReUpdateMj(ProtoBuf.MJRoomInfo mjRoom)
    {
        if (Event_ReUpdateMj != null)
        {
            Event_ReUpdateMj(mjRoom);
        }
        else
        {
            Debug.LogError("ChongLian == NULL");
        }
    }

    /// <summary>
    /// 开始前同Ip提示
    /// </summary>
    public Action<int> Fun_SameIpTip;

    public Action<uint, bool> Fun_ReciveClietOnline;
    #endregion

    #region 服务器发送给本地的操作
    /// <summary>
    /// 告诉玩家哪些人已经准备了的委托
    /// </summary>
    public delegate void RecivePlayerReady(uint PlayerID);
    /// <summary>
    /// 服务器发送给玩家，告诉玩家哪些人已经准备了的委托事件
    /// </summary>
    public event RecivePlayerReady Event_recivePlayerReady;
    /// <summary>
    /// 告诉玩家哪些人已经准备了的触发方法
    /// </summary>
    public void Fun_recivePlayerReady(uint PlayerID)
    {
        if (Event_recivePlayerReady != null)
        {
            Event_recivePlayerReady(PlayerID);
        }
        Debug.Log("RCV_ Player Ready" + PlayerID);
    }



    /// <summary>
    /// 通知哪个玩家是庄家的委托
    /// </summary>
    public delegate void ReciveZhuang(uint ZhuangID);
    /// <summary>
    /// 服务器发送给玩家提示通知哪个玩家是庄家的委托事件
    /// </summary>
    public event ReciveZhuang Event_reciveZhuang;
    /// <summary>
    /// 告诉玩家哪些人已经准备了的委托事件的触发方法
    /// </summary>
    public void Fun_reciveZhuang(uint ZhuangID)
    {
        //        Debug.Log("Fun_reciveZhuang");
        Event_reciveZhuang(ZhuangID);
    }

    /// <summary>
    /// 通知当前本地玩家进行摸牌的委托
    /// </summary>
    public delegate void ReciveGetCard(uint Charid, bool Dachu, uint card);
    /// <summary>
    /// 服务器发送给玩家提示通知当前本地玩家进行摸牌的委托事件
    /// </summary>
    public event ReciveGetCard Event_reciveGetCard;
    /// <summary>
    /// 通知当前本地玩家进行摸牌的委托事件的方法
    /// </summary>
    public void Fun_reciveGetCard(uint Charid, bool Dachu, uint card)
    {
        Event_reciveGetCard(Charid, Dachu, card);
    }

    /// <summary>
    /// 客户端现在可以选择的操作的委托
    /// </summary>
    public delegate void ReciveCanPlay(MJGameOP MjOp, List<uint> cards);
    /// <summary>
    /// 服务器发送给客户端现在可以选择的操作的委托事件 过碰杠胡出牌
    /// </summary>
    public event ReciveCanPlay Event_reciveCanPlay;
    /// <summary>
    /// 客户端现在可以选择 过碰杠胡出牌，委托事件的方法
    /// </summary>
    public void Fun_reciveCanPlay(uint charid, MJGameOP MjOp, List<uint> cards, uint OriCharid/*,uint cardid  可碰杠胡的牌*/ )
    {
        //Event_reciveCanPlay(MjOp, cards); //自己调用的方法
        //Debug.Log("调用了么？");


        switch (MjOp)
        {
            ////case MJGameOP.MJ_OP_XQ://暂时不需要
            ////break;
            //case MJGameOP.MJ_OP_GUO:
            //    Fun_reciveGuo();
            //    break;
            case MJGameOP.MJ_OP_CHUPAI:
                Event_ZhuangChuDiYiZhang(charid);
                Fun_KeyiMoPai(charid, OriCharid);

                break;
            case MJGameOP.MJ_OP_PENG:
                Debug.Log("说我可以碰我不信");
                Fun_KeYiPeng(charid, cards[0]);
                break;
            case MJGameOP.MJ_OP_GANG:

                Fun_KeYiGang(charid, cards, OriCharid);

                break;
            case MJGameOP.MJ_OP_HU:
                Fun_KeYiHu(charid, cards[0], OriCharid);
                break;
            case MJGameOP.MJ_OP_FEI:
                Fun_KeYiFeiPeng(charid, cards[0]);
                break;
            case MJGameOP.MJ_OP_TI:
                Fun_KeYiTiPai(charid, cards[0]);
                break;
                //case MJGameOP.MJ_OP_HU:
                //    Fun_KeYiHu(charid, cards[0], OriCharid);
                break;
            case MJGameOP.MJ_OP_ROUND_OVER:
                break;
            case MJGameOP.MJ_OP_VOTE_JSROOM:
                break;
            case MJGameOP.MJ_OP_VOTE_RESULT:
                break;
            default:
                break;
        }
    }





    /// <summary>
    /// 接收其他玩家已经碰杠胡过的消息的委托
    /// </summary>
    /// <param name="MjOp"></param>
    /// <param name="PlayerID"></param>
    public delegate void ReciveOtherCanPlay(MJGameOP MjOp, uint PlayerID, uint card, uint oricharid);
    public event ReciveOtherCanPlay Event_ReciveOtherCanPlay;
    /// <summary>
    /// 接收其他玩家已经碰杠胡过的消息
    /// </summary>
    /// <param name="MjOp"></param>
    public void Fun_reciveOtherPengGangHu(MJGameOP MjOp, uint PlayerID, uint card, uint oricharid)
    {
        Event_ReciveOtherCanPlay(MjOp, PlayerID, card, oricharid);
    }

    /// <summary>
    /// 发送给玩家一堆手牌
    /// </summary>
    public delegate void ReciveGetFirstCards(List<uint> Cards, int i);
    /// <summary>
    /// 服务器发送给玩家一堆手牌的委托事件 
    /// </summary>
    public event ReciveGetFirstCards Event_reciveGetFirstCards;
    /// <summary>
    /// 服务器发送给玩家一堆手牌委托事件的方法
    /// </summary>
    public void Fun_reciveGetHandCards(List<uint> Cards, int seat)
    {
        Debug.Log("Update Start HandCards");
        Event_reciveGetFirstCards(Cards, seat);
    }

    /// <summary>
    /// 发送给玩家鬼牌用于骰子
    /// </summary>
    public delegate void ReciveGetGuiCards(List<uint> Cards, int i);
    /// <summary>
    /// 发送给玩家鬼牌用于骰子的委托事件 
    /// </summary>
    public event ReciveGetGuiCards Event_reciveGetGuiCards;
    /// <summary>
    /// 服务器发送给玩家发送给玩家鬼牌用于骰子方法
    /// </summary>
    public void Fun_reciveGetGuiCards(List<uint> cards, int seat)
    {
        if (BaseProto.playerInfo.m_inGame == GameType.GT_THIRt)
        {
            if (cards.Count > 0)
            {
                Event_reciveGetGuiCards(cards, seat);
            }
        }
        List<uint> guiCards = new List<uint>();
        if (cards.Count > 0)
        {
            guiCards.Add(cards[0] / 1000);
            guiCards.Add(cards[0] % 1000);
        }
        DataManage.Instance.SetRoomTouZi(guiCards);
        if (Event_reciveGetGuiCards != null)
        {
            Event_reciveGetGuiCards(guiCards, seat);
        }
    }

    /// <summary>
    /// 发送给玩家鬼牌用于骰子
    /// </summary>
    public delegate void ReciveGetIsOnLine(uint charid, uint isOnLine);
    /// <summary>
    /// 发送给玩家鬼牌用于骰子的委托事件 
    /// </summary>
    public event ReciveGetIsOnLine Event_reciveGetIsOnLine;
    /// <summary>
    /// 服务器发送给玩家发送给玩家鬼牌用于骰子方法
    /// </summary>
    public void Fun_reciveGetIsOnLine(uint charid, uint isOnLine)
    {
        Debug.Log("玩家断线或重连？");
        if (Event_reciveGetIsOnLine != null)
        {
            Event_reciveGetIsOnLine(charid, isOnLine);
        }
    }

    /// <summary>
    /// 开局后通知庄家可以出牌了
    /// </summary>

    public Action<uint> Event_ZhuangChuDiYiZhang;





    /// <summary>
    /// 客户端接收到服务器发送过来的三张牌
    /// </summary>
    /// <param name="Change3ZhangCard"></param>
    public delegate void ReciveChange3ZhangResult(List<uint> Change3ZhangCard);
    public event ReciveChange3ZhangResult Event_ReciveChange3ZhangResult;
    /// <summary>
    /// 服务器发送给客户端得到的换三张的结果
    /// </summary>
    public void Change3ZhangResult(List<uint> Change3ZhangCard)
    {
        Event_ReciveChange3ZhangResult(Change3ZhangCard);
    }



    /// <summary>
    /// 服务器发送给客户端，该玩家有人选缺，并显示的委托
    /// </summary>
    public delegate void ReciveSelectQue(uint PlayerID, uint CardType);
    /// <summary>
    /// 服务器发送给客户端，该玩家有人选缺，并显示的委托事件
    /// </summary>
    public event ReciveSelectQue Event_reciveSelectQue;
    /// <summary>
    /// 服务器发送给客户端，该玩家有人选缺，并显示的委托事件方法
    /// </summary>
    public void Fun_reciveSelectQue(uint PlayerID, uint CardType)
    {
        Debug.Log("Update XuanQue Fun_reciveSelectQue" + PlayerID);
        if (Event_reciveSelectQue != null)
        {
            Event_reciveSelectQue(PlayerID, CardType);
        }
    }


    /// <summary>
    /// 服务器提示其他玩家所发出的消息的委托
    /// </summary>
    public delegate void ReciveMessagePreDefine(uint Sender, string Value);
    public delegate void ReciveMessageText(uint Sender, string Value);

    /// <summary>
    /// 服务器发给当前客户端，该玩家所发出的消息的委托事件
    /// </summary>
    public event ReciveMessagePreDefine Fun_reciveMessagePreDefine;
    public event ReciveMessageText Fun_reciveMessageText;

    /// <summary>
    /// 服务器发给当前客户端，该玩家所发出的消息的委托事件的方法
    /// </summary>
    public void Fun_reciveOtherMessage(ChatMessageRsp pack)
    {

        switch (pack.msgType)
        {
            case ChatMessageRsp.MsgType.PreDefine:
                {
                    Fun_reciveMessagePreDefine(pack.senderId, pack.msgString);
                    Debug.Log("收到的图片ID:" + pack.msgString);
                    //Debug.Log("发送的玩家" + pack.senderId);
                }
                break;
            case ChatMessageRsp.MsgType.InputText:
                {

                    Fun_reciveMessageText(pack.senderId, pack.msgString);

                }
                break;
            case ChatMessageRsp.MsgType.InputVoice:
                {
                    Debug.Log("收到别人发的语音");
                    //自定义语音直接播放
                    Event.Inst().F_PlaySound(pack.msgString);
                    if (Event_RecivePlayer_VChat != null)
                    {
                        Event_RecivePlayer_VChat(pack.senderId);
                    }
                    //F_PlaySound(pack.senderId, pack.msgString);
                }
                break;
            default:
                break;
                //}
        }
        //自己发出来的消息，在本地客户端自己发出来
    }
    public delegate void RecivePlayer_VChat(uint PlayerId);
    /// <summary>
    /// 服务器发送给本地客户端，该玩家发出了一语音
    /// </summary>
    public event RecivePlayer_VChat Event_RecivePlayer_VChat;

    public delegate void OnClick(bool isSend = true);
    public OnClick StartMic;
    public delegate void playSound(uint SendId, string url);
    public event playSound PlaySound;

    public void F_PlaySound(uint SendID, string url)
    {
        PlaySound(SendID, url);
    }


    /// <summary>
    /// 服务器发送给本地客户端，该玩家发出了一张牌
    /// </summary>
    public delegate void ReciveOtherPopCard(uint PlayerId, uint CardId);
    /// <summary>
    /// 服务器发送给本地客户端，该玩家发出了一张牌的消息事件
    /// </summary>
    public event ReciveOtherPopCard Event_ReciveOtherPopCard;
    /// <summary>
    /// 服务器发送给本地客户端，该玩家发出了一张牌的消息事件的方法
    /// </summary>
    public void Fun_reciveOtherPopCard(uint PlayerId, uint CardId)
    {
        Event_ReciveOtherPopCard(PlayerId, CardId);
    }

    /// <summary>
    /// 服务器发送给本地客户端，13张马牌
    /// </summary>
    public delegate void Recive13ZMaPai(uint card);
    /// <summary>
    /// 服务器发送给本地客户端，13张马牌
    /// </summary>
    public event Recive13ZMaPai Event_Recive13ZMaPai;

    /// <summary>
    /// 服务器发送给本地客户端，13张马牌开始
    /// </summary>
    public void Fun_Recive13MaPai(uint card)
    {
        if (Event_Recive13ZMaPai != null)
        {
            Event_Recive13ZMaPai(card);
        }
    }

    /// <summary>
    /// 服务器发送给本地客户端，比牌
    /// </summary>
    public delegate void Recive13ZBiPai(List<ComparedCard> cardDatas);
    /// <summary>
    /// 服务器发送给本地客户端，开始比牌
    /// </summary>
    public event Recive13ZBiPai Event_Recive13ZBiPai;

    /// <summary>
    /// 服务器发送给本地客户端，开始比牌
    /// </summary>
    public void Fun_Recive13BiPai(List<ComparedCard> cardDatas)
    {
        if (Event_Recive13ZBiPai != null)
        {
            Event_Recive13ZBiPai(cardDatas);
        }
    }

    /// <summary>
    /// 服务器发送给本地客户端，打枪委托
    /// </summary>
    public delegate void ReciveDaQiang(uint charid, List<uint> charidAry);
    /// <summary>
    /// 服务器发送给本地客户端，开始打枪
    /// </summary>
    public event ReciveDaQiang Event_Recive13ZDaQiang;

    /// <summary>
    /// 服务器发送给本地客户端，开始打枪
    /// </summary>
    public void Fun_Recive13DaQiang(uint charid, List<uint> charidAry)
    {
        if (Event_Recive13ZDaQiang != null)
        {
            Event_Recive13ZDaQiang(charid, charidAry);
        }
    }

    #region 提示客户端的PLAYROOM可以‘杠’‘碰’‘胡’‘过’‘出牌’这个操作的委托
    /// <summary>
    /// 提示客户端‘过’这个操作的委托
    /// </summary>
    public delegate void ReciveGuo();
    /// <summary>
    /// ReciveGuo类型，服务器发送给客户端‘过’这个操作的委托事件
    /// </summary>
    public event ReciveGuo Event_reciveGuo;
    /// <summary>
    /// 提示客户端‘过’这个操作的委托事件的方法
    /// </summary>
    public void Fun_reciveGuo()
    {
        Event_reciveGuo();
    }


    /// <summary>
    /// 提示客户端可以‘碰’这个操作的委托
    /// </summary>
    public delegate void RecivePeng(uint cardid);
    /// <summary>
    /// RecivePeng类型，服务器发送客户端‘碰’这个操作的委托事件
    /// </summary>
    public event RecivePeng Event_KeYiPeng;
    /// <summary>
    /// 提示客户端‘碰’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiPeng(uint charid, uint cardid)
    {
        if (charid == BaseProto.playerInfo.m_id)//如果是本地玩家再显示他的碰Button
            Event_KeYiPeng(cardid);
    }


    /// <summary>
    /// 提示客户端‘胡’这个操作的委托
    /// </summary>
    public delegate void ReciveHu(uint cardid, uint OriCharid);
    /// <summary>
    /// ReciveHu类型，服务器发送给客户端可以‘胡’这个操作的委托事件
    /// </summary>
    public event ReciveHu Event_KeYiHu;
    /// <summary>
    /// 提示客户端可以‘胡’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiHu(uint charid, uint cardid, uint OriCharid)
    {
        if (charid == BaseProto.playerInfo.m_id)//如果是本地玩家再显示他的胡Button

            Event_KeYiHu(cardid, OriCharid);
    }


    /// <summary>
    /// 提示客户端可以‘杠’这个操作的委托
    /// </summary>
    public delegate void ReciveGangList(List<uint> cardid, uint OriCharid);
    /// <summary>
    /// ReciveGang类型，服务器发送给客户端可以‘杠’这个操作的委托事件
    /// </summary>
    public event ReciveGangList Event_KeYiGang;

    /// <summary>
    /// 本地判断可杠的列表返回
    /// </summary>
    public delegate List<uint> ReturnGanglist(uint cardid, uint OriCharid);

    public ReturnGanglist Event_ReturnGanglist;

    public Action GangGuoLe;

    /// <summary>
    /// 提示客户端可以‘杠’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiGang(uint charid, List<uint> cardid, uint OriCharid)
    {
        if (charid == BaseProto.playerInfo.m_id)
        {
            Debug.Log(cardid.Count + "张牌可杠");
            for (int i = 0; i < cardid.Count; i++)
            {
                Debug.Log(BaseProto.playerInfo.m_id + " 杠的牌花 ");
            }

            Event_KeYiGang(cardid, OriCharid);

        }//如果是本地玩家再显示他的杠Button
    }


    public delegate void KeyiMoPai(uint Charid, uint OriCharid);
    public event KeyiMoPai Event_KeyiMoPai;

    public void Fun_KeyiMoPai(uint Charid, uint OriCharid)
    {
        if (Event_KeyiMoPai != null)
        {
            Event_KeyiMoPai(Charid, OriCharid);
        }
    }

    /// <summary>
    /// 提示客户端可以‘爆’这个操作的委托
    /// </summary>
    public delegate void ReciveBaoPai(uint charid, List<uint> cards);
    /// <summary>
    /// RecivePeng类型，服务器发送客户端‘爆’这个操作的委托事件
    /// </summary>
    public event ReciveBaoPai Event_KeYiBao;
    /// <summary>
    /// 提示客户端‘爆’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiBao(uint charid, List<uint> cards)
    {
        Debug.Log("KeYi Bao Ting " + charid);
        if (charid == BaseProto.playerInfo.m_id)//
            if (Event_KeYiBao != null)
            {
                Event_KeYiBao(charid, cards);
            }
    }

    /// <summary>
    /// 提示客户端可以‘飞碰’这个操作的委托
    /// </summary>
    public delegate void ReciveFeiPeng(uint charid, uint cardid);
    /// <summary>
    /// RecivePeng类型，服务器发送客户端‘飞碰’这个操作的委托事件
    /// </summary>
    public event ReciveFeiPeng Event_KeYiFei;
    /// <summary>
    /// 提示客户端‘飞碰’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiFeiPeng(uint charid, uint cardid)
    {
        if (charid == BaseProto.playerInfo.m_id)//
            Event_KeYiFei(charid, cardid);
    }

    /// <summary>
    /// 提示客户端可以‘提牌’这个操作的委托
    /// </summary>
    public delegate void ReciveTiPai(uint cardid);
    /// <summary>
    /// RecivePeng类型，服务器发送客户端‘提牌’这个操作的委托事件
    /// </summary>
    public event ReciveTiPai Event_KeYiTiPai;
    /// <summary>
    /// 提示客户端‘提牌’这个操作的委托事件的方法
    /// </summary>
    public void Fun_KeYiTiPai(uint charid, uint cardid)
    {
        if (charid == BaseProto.playerInfo.m_id)//
            Event_KeYiTiPai(cardid);
    }

    #endregion


    /// <summary>
    /// 玩家头像
    /// </summary>
    /// <param name="headValue"></param>
    public delegate void HeadImage(string headValue);
    /// <summary>
    /// 接收头像的委托事件
    /// </summary>
    public event HeadImage headImage;
    /// <summary>
    /// 房间内的玩家头像
    /// </summary>
    public void Fun_headImage(string value)
    {
        //if(GameManager.GM._AllPlayerData[0].ID== BaseProto.playerInfo.m_id)
        //headImage(value);
    }


    public delegate void DirLight(uint Dir);
    public event DirLight Event_DirLight;
    public void Fun_DirLight(uint Dir)
    {
        Debug.Log("Fun_DirLight" + Dir);
        if (Event_DirLight != null)
        {
            Event_DirLight(Dir);
        }
    }

    public Action<int> Fun_UpdatePaishu;








    #endregion   

    #region 发送给服务器的操作
    /// <summary>
    /// 发送给服务器客户端已经准备好了
    /// </summary>
    public void Fun_SentClientPre()
    {

        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_PREP;
        MJProto.Inst().MJGameOPRequest(pack);
        Debug.Log("发送准备");
    }

    /// <summary>
    /// 发送给服务器客户端已经摸牌了(仅第一次庄家摸牌时候使用)
    /// </summary>
    public void Fun_SentClientMo(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.cardId = CardNum;
        pack.op = MJGameOP.MJ_OP_MOPAI;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    /// <summary>
    /// 出牌，本地玩家此时出牌，记录了玩家的id和出牌信息，发送给服务器
    /// </summary>
    public void Fun_SentPopCard(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_CHUPAI;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Guo类型，玩家点击 ‘过’，发送给服务器
    /// </summary>
    public void Fun_SentGuo(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_GUO;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = false;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Peng类型，本地玩家点击‘特殊牌’发送给服务器
    /// </summary>
    public void Fun_SentTeShuPai(uint CardNum)
    {
        Debug.Log("你想爆牌？");
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = (MJGameOP)THIRTGameOP.THIRT_OP_TING;
        pack.cardId = 0;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// FeiPeng类型，本地玩家点击‘飞碰’发送给服务器
    /// </summary>
    public void Fun_SentFeiPeng(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_FEI;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Peng类型，本地玩家点击‘爆’发送给服务器
    /// </summary>
    public void Fun_SentBao(uint CardNum)
    {
        Debug.Log("你想爆牌？");
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_TING;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Peng类型，本地玩家点击‘提牌’发送给服务器
    /// </summary>
    public void Fun_SentTiPai(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_TI;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;

        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Peng类型，本地玩家点击‘碰’发送给服务器
    /// </summary>
    public void Fun_SentPeng(uint CardNum)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_PENG;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = true;
        //点击之后牌要落下消失，并且出现在本地玩家这里
        //CardManager
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Gang类型，本地玩家点击'杠'发送给服务器
    /// </summary>
    public void Fun_SentGang(uint CardNum, uint OriCharid)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_GANG;
        pack.oricharId = OriCharid;
        pack.cardId = CardNum;
        pack.charId = BaseProto.playerInfo.m_id;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = true;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// Hu类型，本地玩家点击‘胡’发送给服务器
    /// </summary>
    public void Fun_SentHu(uint CardNum, uint OriCharid)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_HU;

        pack.oricharId = OriCharid;
        pack.charId = BaseProto.playerInfo.m_id;
        pack.cardId = CardNum;
        //MJProto.Inst().IsPopCardDown = true;
        //MJProto.Inst().isPengGangHu = true;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// 13张摆牌确认，本地玩家点击‘确认’发送给服务器
    /// </summary>
    public void Fun_SentHu13Z_BPOK(List<uint> cards)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = (MJGameOP)THIRTGameOP.THIRT_OP_HU; //MJGameOP.MJ_OP_HU;
        for (int i = 0; i < cards.Count; i++)
        {
            pack.x3zCardId.Add(cards[i]);
        }
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }


    /// <summary>
    /// SelectQue类型,玩家点击缺后，把牌发送给服务器1是万，2是筒，3是条
    /// </summary>
    /// /// <param name="num">1是万，2是筒，3是条</param>
    public void Fun_SentSelectQue(uint num)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_XQ;
        pack.charId = BaseProto.playerInfo.m_id;
        pack.param = num;
        MJProto.Inst().MJGameOPRequest(pack);
        Debug.Log("选缺");
    }



    /// <summary>
    /// 换3张
    /// </summary>
    /// <param name="CardId"></param>
    public void SentChange3Zhang(List<uint> CardId)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        for (int i = 0; i < CardId.Count; i++)
        {
            pack.x3zCardId.Add(CardId[i]);
        }

        MJProto.Inst().MJGameOPRequest(pack);
    }


    /// <summary>
    /// SentMegssageId，把玩家发送的预制消息发给服务器
    /// </summary>
    public void SentMegssageId(uint MesId)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.PreDefine;
        pack.msgId = MesId;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }

    /// <summary>
    /// 向服务器发送当前的自定义文字
    /// </summary>
    public void SentMegssageText(string MesText)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.InputText;
        pack.msgString = MesText;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    /// <summary>
    /// SentMegssageId，把玩家发送的图片发给服务器
    /// </summary>
    public void SentMegssageImage(string MesId)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.PreDefine;
        pack.msgString = MesId;
        Debug.Log("发送的图片ID:" + MesId);
        BaseProto.Inst().SendChatMsgRequest(pack);
    }

    /// <summary>
    /// SentMegssageId，把玩家发送的声音发给服务器
    /// </summary>
    public void SentMegssageVoice(byte[] Mesbyte)
    {
        ChatMessageReq pack = new ChatMessageReq();
        pack.senderId = BaseProto.playerInfo.m_id;
        pack.msgType = ChatMessageReq.MsgType.InputVoice;
        pack.msgBytes = Mesbyte;
        BaseProto.Inst().SendChatMsgRequest(pack);
    }
    #endregion 

    #region 游戏结束
    public delegate void ReciveGameResult();
    /// <summary>
    /// 服务器端发送结果，接收游戏结果
    /// </summary>
    public ReciveGameResult reciveGameResult;
    public void Fun_reciveGameResult()
    {
        reciveGameResult();
    }

    public delegate void ReciveCardResult();
    /// <summary>
    /// 接收玩家的牌面信息
    /// </summary>
    public event ReciveCardResult reciveCardResult;
    public void Fun_reciveCardResult()
    {

    }
    /// <summary>
    /// 游戏结束之后再来一局
    /// </summary>
    public delegate void NextGame();
    public event NextGame Event_nextGame;
    public void Fun_nextGame()
    {
        ///////////////////////////有问题！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        ProtoBuf.EnterRoomRsp rsp = null;
        rsp.charId = BaseProto.playerInfo.m_id;
        //rsp.
    }
    #endregion

    #region 投票以及退出房间  再来一盘  游戏结束  网络断线


    /// <summary>
    /// 发起投票 打包发给服务器
    /// </summary>
    public void VoteRequest(bool istongyi)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.op = MJGameOP.MJ_OP_VOTE_JSROOM;
        if (BaseProto.playerInfo.m_inGame == GameType.GT_THIRt)
        {
            pack.op = (MJGameOP)THIRTGameOP.THIRT_OP_VOTE_JSROOM; //.MJ_OP_VOTE_JSROOM;
        }
        if (istongyi)
            pack.param = 2;
        else
            pack.param = 1;

        Debug.Log("投票发送了");
        MJProto.Inst().MJGameOPRequest(pack);
    }

    /// <summary>
    /// 发起投票 打包发给服务器
    /// </summary>
    public void VoteRequest_Game13Z(bool istongyi)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.op = (MJGameOP)THIRTGameOP.THIRT_OP_VOTE_JSROOM; //MJGameOP.MJ_OP_VOTE_JSROOM;
        if (istongyi)
            pack.param = 2;
        else
            pack.param = 1;

        Debug.Log("投票发送了");
        MJProto.Inst().MJGameOPRequest(pack);
    }

    public delegate void ReciveOtherVote(uint charid, bool istongyi);
    /// <summary>
    ///接到别人投票的事件
    /// </summary>
    public event ReciveOtherVote Event_ReciveOherVote;
    /// <summary>
    /// 接收投票方法
    /// </summary>
    public void Fun_ReciveOherVote(uint charid, uint istongyi)
    {

        if (istongyi == 1)
            Event_ReciveOherVote(charid, false);
        else if (istongyi == 2)
            Event_ReciveOherVote(charid, true);

    }


    public delegate void ReciveVoteResult(bool isjiesan);
    /// <summary>
    ///接到别人投票的事件
    /// </summary>
    public event ReciveVoteResult Event_ReciveVoteResult;
    /// <summary>
    /// 投票解散房间成功或者失败
    /// </summary>
    public void Fun_ReciveVoteResult(uint isjiesan)
    {
        Debug.Log(isjiesan);
        if (isjiesan == 1)
            Event_ReciveVoteResult(false);
        else
            Event_ReciveVoteResult(true);

    }


    public void OnExitRoom()
    {

        BaseProto.Inst().ExitRoomRequest();

    }
    /// <summary>
    /// 接受服务器退出房间成功
    /// </summary>
    public delegate void ExitRoomSucc();
    /// <summary>
    /// 退出房间成功事件
    /// </summary>
    public event ExitRoomSucc Event_ExitRoomSucc;
    /// <summary>
    /// 退出房间
    /// </summary>
    public void Fun_ExitRoomSucc()
    {
        Event_ExitRoomSucc();
    }


    //接收每局结束结果
    public delegate void ReciveRoundOverResult(MJGameOver rsp);

    public event ReciveRoundOverResult Event_ReciveRoundOverResult;

    public void Fun_ReciveRoundOverResult(MJGameOver rsp)
    {
        Debug.Log("END 1Ju Fun_ReciveRoundOverResult   ");
        if (Event_ReciveRoundOverResult != null)
        {
            Event_ReciveRoundOverResult(rsp);
        }
    }


    //玩家点击再来一局
    public delegate void ReadyToPlayNew();

    public event ReadyToPlayNew Event_ReadyToPlayNew;

    public void Fun_ReadyToPlayNew()
    {
        Event_ReadyToPlayNew();
    }

    //游戏结束了
    public delegate void ReciveGameOverResult();

    public event ReadyToPlayNew Event_ReciveGameOverResult;

    public void Fun_ReciveGameOverResult()
    {
        if (Event_ReciveGameOverResult != null)
        {
            Event_ReciveGameOverResult();
        }

    }

    public void DisconnectNet()
    {

        if (LoginRest != null)
        {
            Debug.Log("我要关掉Login");

            LoginRest();
        }
        //回收游戏场景
        if (Event_ExitRoomSucc != null)
        {
            Event_ExitRoomSucc();
        }
        //回收大厅
        if (Event_joinRoomSuccess != null)
        {
            Debug.Log("我要关掉lobby");
            Event_joinRoomSuccess();
        }
        //回收创建房间
        if (Event_reciveCreatRoomSuccess != null)
        {
            Debug.Log("我要关掉CreatRoom");
            Event_reciveCreatRoomSuccess();

        }
        //回收加入房间
        if (ReciveJoinRoomWindow != null)
        {
            ReciveJoinRoomWindow();
        }
        //回收战绩
        if (ReciveCombatGainsWindow != null)
        {
            ReciveCombatGainsWindow();
        }
        //回收回放窗口
        if (ReciveReviewUIWindow != null)
        {
            ReciveReviewUIWindow();
        }

        //回收信息窗口
        if (ReciveMessageWindow != null)
        {
            ReciveMessageWindow();
        }
        //回收小窗口
        if (RestNoticWindow != null)
            RestNoticWindow();
        //*录音预制回收
        //  GameManager.GM.GetSoundFab().SetActive(false);


        //退出服务器
        AppLoginOut();

        //退出微信服务器
        //AnySDKManager.SendLogout();

        //弹窗 "网络异常 请重新登录"
        //var window = GameManager.GM.SearchEmpty().AddComponent<Login>();
        //window.Ins();

        //var window5 = GameManager.GM.SearchEmpty().AddComponent<Notic>();
        //window5.Ins();
        //window5.InputText(true, "\n\n网络异常,请重新登录！");
        //window5.GraduallyRest();
    }

    //回收加入房间窗口
    public Action ReciveJoinRoomWindow;
    //回收信息窗口
    public Action ReciveMessageWindow;
    //回收战绩窗口
    public Action ReciveCombatGainsWindow;
    //回收回放窗口
    public Action ReciveReviewUIWindow;
    //回收录音预制
    public Action ReciveSoundPrefab;


    public delegate void ReciveRound_AllOverResult(List<MJGameOver> end_All_S);
    public ReciveRound_AllOverResult Event_ReciveRound_AllOverResult;
    public void Fun_ReciveAllGameOverResult(List<MJGameOver> end_All_S)
    {
        if (Event_ReciveRound_AllOverResult != null)
        {
            Debug.Log("End_____All" + end_All_S.Count);

            Event_ReciveRound_AllOverResult(end_All_S);

        }
    }
    #endregion


}
