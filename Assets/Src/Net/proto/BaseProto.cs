using System;
using UnityEngine;
using System.Collections;
using ProtoBuf;

// 玩家信息

public struct PlayerInfo
{
    /// <summary>
    /// 玩家唯一id
    /// </summary>
    public UInt32 m_id;
    /// <summary>
    /// channel_user_id
    /// </summary>             
    public string m_account;
    /// <summary>
    /// // 玩家当前在哪里（比如玩家已经在玩麻将，此时强制杀掉进程，重新登录后，会再次进入上次的游戏内）
    /// </summary>
    public GameType m_inGame;
    /// <summary>
    /// 玩家所创建的房间号，如果没有则为0
    /// </summary>        
    public UInt32 m_cdRoomId;
    /// <summary>
    /// 玩家当前所在的房间号，如果没有则为0
    /// </summary> 
    public UInt32 m_atRoomId;
    /// <summary>
    ///玩家等级，暂时用不到，留着扩展
    /// </summary>
    public UInt32 m_level;
    /// <summary>
    ///玩家房卡数
    /// </summary>          
    public UInt32 m_roomCard;
    /// <summary>
    ///玩家钻石数
    /// </summary>
    public UInt32 m_diamond;
    /// <summary>
    ///玩家金币数
    /// </summary>
    public int m_gold;

    public string m_ip;

    public int sex;//（1MAN 2Woman）

    public string Head { get; internal set; }
}

public class BaseProto : ISingleton<BaseProto>
{



    
    /// <summary>
    /// 查询 战绩和回访数据
    /// </summary>
    /// <param name="charId"></param>
    /// <returns></returns>
    //public bool QueryInfoRequest(UInt32 charId)
    public bool QueryInfoRequest( QueryInfoReq pack)
    {
        //QueryInfoReq pack = new QueryInfoReq();
        //pack.queryType = QueryInfoReq.QueryType.ZhanJi;
        //QueryInfoReq.QueryType.CharInfo
        //pack.param1 = 0;
        //pack.param2 = 9;//
        pack.charId = playerInfo.m_id;
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_QUERY_INFO;
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    /// <summary>
    /// 接收查询到的战绩信息和回访数据
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>
    public bool QueryInfoResponse(QueryInfoRsp rsp)
    {
        switch (rsp.queryType)
        {
            case QueryInfoRsp.QueryType.ZhanJi:
                PublicEvent.GetINS.ReciveZhanJiHuiFang(rsp);
                break;
            case QueryInfoRsp.QueryType.HuiFang:
                break;
            case QueryInfoRsp.QueryType.FriendList:
                break;
            case QueryInfoRsp.QueryType.CharInfo:
                PublicEvent.GetINS.ReciveDiamond(rsp);
                break;
            default:
                break;
        }
        //rsp.charInfoDy.
        return true;
    }

    /// <summary>
    /// 发送请求 分享
    /// </summary>
    public bool SendShar(QueryInfoReq pack)
    {
       
        pack.charId = playerInfo.m_id;
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_SHARA_NUM;
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    /// <summary>
    /// 接收查询 关于分享
    /// </summary>
    public bool RcvShara(QueryInfoRsp rsp)
    {
        Debug.Log("RcvShara  AAA" + rsp.param2);
        if (rsp.param1 > 0)
        {//分享得到了钻石
            //rsp.param2 == 有多少钻石
            PublicEvent.GetINS.RcvShare(rsp);
        }
        //rsp.charInfoDy.
        return true;
    }

    /// <summary>
    ///在进入游戏的时候已经录入了相关的信息。 m_id ，m_account， m_inGame，m_cdRoomId，m_atRoomId，m_level，m_roomCard，m_diamond，m_gold，m_ip
    /// </summary>
    static public PlayerInfo playerInfo;
    static public uint SeverPlayerNum = 0;


    // 选择某个游戏，比如选择麻将
    public bool EnterGameRequest(GameType gameType)
    {
        EnterGameReq pack = new EnterGameReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_ENTER_GAME;
        pack.charId = playerInfo.m_id;
        pack.enterGame = gameType;
        Debug.LogWarning(string.Format("Send CLI_TO_LGI_ENTER_GAME {0}", gameType));
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    // 选择某个游戏，比如选择麻将 --> 服务器返回包
    public bool EnterGameResponse(EnterGameRsp rsp)
    {
        if (rsp.result == EnterGameRsp.Result.SUCC)
        {
            playerInfo.m_id = rsp.charId;
            playerInfo.m_inGame = rsp.enterGame;
            playerInfo.m_account = rsp.channel_user_id;
            Debug.LogWarning("Recv LGI_TO_CLI_ENTER_GAME Succ");
            PublicEvent.GetINS.Fun_SuccessIntoHall();
        }
        else
        {
            PublicEvent.GetINS.Fun_FaillIntoHall();
            Debug.LogWarning("Recv LGI_TO_CLI_ENTER_GAME Fail");
        }
        return true;
    }

    // 退出某个游戏，比如选择麻将
    public bool ExitGameRequest()
    {
        ExitGameReq pack = new ExitGameReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_EXIT_GAME;
        pack.charId = playerInfo.m_id;
        pack.exitGame = playerInfo.m_inGame;
        Debug.LogWarning("Send CLI_TO_LGI_EXIT_GAME");
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    // 退出某个游戏，比如退出麻将 --> 服务器返回包
    public bool ExitGameResponse(ExitGameRsp rsp)
    {
        if (rsp.result == ExitGameRsp.Result.SUCC)
        {
            playerInfo.m_inGame = rsp.exitGame;
            Debug.LogWarning("Recv LGI_TO_CLI_EXIT_GAME Succ");
        }
        else
        {
            Debug.LogWarning("Recv LGI_TO_CLI_EXIT_GAME Fail");
        }
        return true;
    }

    // 创建某个游戏的房间
    public bool CreateRoomRequest(CreateRoomReq reqPack)
    {
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_CREATE_ROOM;
        CreateRoomReq new_ReqPack = reqPack;
        new_ReqPack.charId = BaseProto.playerInfo.m_id;
        reqPack.charId = BaseProto.playerInfo.m_id;
        reqPack.account = "";
        Debug.LogWarning("Send CLI_TO_LGI_CREATE_ROOM");
        return GameNetWork.Inst().SendDataToLoginServer(command, new_ReqPack);
    }



    // 创建某个游戏的房间 --> 服务器返回包
    public bool CreateRoomResponse(CreateRoomRsp rsp)
    {
        if (rsp.result == CreateRoomRsp.Result.SUCC)
        {

            playerInfo.m_id = rsp.charId;
            playerInfo.m_cdRoomId = rsp.roomId;
            playerInfo.m_inGame = rsp.gameType;
            Debug.LogWarning("Recv LGI_TO_CLI_CREATE_ROOM Succ");
            PublicEvent.GetINS.Fun_reciveIsCreatRoom(rsp);
            EnterRoomRequest();
        }
        else
        {
            Debug.LogWarning("Recv LGI_TO_CLI_CREATE_ROOM Fail");
            PublicEvent.GetINS.Fun_reciveIsCreatRoom(rsp);
        }
        return true;
    }

    // 进入某个游戏的房间
    public bool EnterRoomRequest()
    {
        //BaseProto.Inst().EnterGameRequest(ProtoBuf.GameType.GT_XWMJ/*GT_TT*/);

        EnterRoomReq pack = new EnterRoomReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_ENTER_ROOM;
        //pack.gameType = GameType.GT_XWMJ;
        pack.charId = playerInfo.m_id;
        pack.gameType = playerInfo.m_inGame;// GameType.GT_THIRt; //playerInfo.m_inGame;//GameType.GT_XWMJ;
        
        pack.roomId = playerInfo.m_cdRoomId;
        pack.account = "";
        Debug.LogWarning("Send CLI_TO_LGI_ENTER_ROOM");
        Debug.Log("请求进入房间");
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    // 进入某个游戏的房间 --> 服务器返回包
    public bool EnterRoomResponse(EnterRoomRsp rsp)
    {

        //if(rsp.mjRoom == null) {
        //    playerInfo.m_atRoomId = rsp.roomId;
        //    return true; }
        if (rsp.result == EnterRoomRsp.Result.SUCC)
        {
            SeverPlayerNum = (uint)rsp.mjRoom.charIds.Count;
            //rsp.mjRoom.roomRuleInfo.xlchRule.xlch
            Debug.Log("服务器返回包：进入房间成功！EnterRoomResponse");
            playerInfo.m_inGame = rsp.gameType;
            Debug.Log("EnterRoom SUCC " + rsp.gameType);

            if (rsp.gameType == GameType.GT_XWMJ
                || rsp.gameType == GameType.GT_THIRt || rsp.gameType == GameType.GT_DE)
            {
                Debug.Log("GT_TT");
                PublicEvent.GetINS.Fun_JoinResult(rsp);
                //进入房间之后就把当前的房间号码记录下来 
                playerInfo.m_atRoomId = rsp.roomId;

            }

            Debug.LogWarning("Recv LGI_TO_CLI_ENTER_ROOM Succ");
        }
        else
        {
            PublicEvent.GetINS.Fun_JoinResult(rsp);

            Debug.Log("进入服务器失败");
        }
        return true;
    }

    // 退出某个游戏的房间
    public bool ExitRoomRequest()
    {
        ExitRoomReq pack = new ExitRoomReq();
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_EXIT_ROOM;
        pack.charId = playerInfo.m_id;
        pack.roomId = playerInfo.m_atRoomId;
        Debug.LogWarning("Send CLI_TO_LGI_EXIT_ROOM");
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    // 退出某个游戏的房间 --> 服务器返回包
    public bool ExitRoomResponse(ExitRoomRsp rsp)
    {
        if (rsp.result == ExitRoomRsp.Result.SUCC)
        {
            playerInfo.m_atRoomId = 0;
            //var window = GameManager.GM.SearchEmpty().AddComponent<Homelobby>();
            //window.Ins();
            PublicEvent.GetINS.Fun_ExitRoomSucc();
            Debug.LogWarning("Recv LGI_TO_CLI_EXIT_ROOM Succ");
        }
        else
        {
            Debug.LogWarning("Recv LGI_TO_CLI_EXIT_ROOM Fail");
        }
        return true;
    }

    // 发送聊天消息
    public bool SendChatMsgRequest(ChatMessageReq pack)
    {
        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_CHAT_MESAGE;
        //  Debug.Log("发送消息"+pack.senderId+pack.msgType);

        return GameNetWork.Inst().SendDataToLoginServer(command, pack);

    }

    // 收到别人发来的聊天消息
    public bool RecvChatMsgResponse(ChatMessageRsp rsp)
    {
        Debug.Log("收到别人发的消息");
        
        PublicEvent.GetINS.Fun_reciveOtherMessage(rsp);
        return true;
    }

    // 服务器广播消息，比如走马灯之类的 LGI_TO_CLI_NOTIFY_MESSAGE
    public bool NotifyMessage(NotifyServerMessage rsp)
    {
        switch (rsp.msgType)
        {
            case NotifyServerMessage.MsgType.ROLL:
                PublicEvent t = new PublicEvent();
                t.Fun_recivePublicText(rsp.roll);
                break;
            case NotifyServerMessage.MsgType.LETTER:

                break;
            default:
                break;
        }

        return true;
    }

    // LGI_TO_CLI_ROOM_INFO
    /// <summary>
    /// 有玩家进入或者退出房间，刷新房间里面的消息
    /// </summary>
    /// <param name="roomInfo"></param>
    /// <returns></returns>
    public bool OnSyncRoomInfo(SyncRoomInfo roomInfo)
    {
        ProtoBuf.CharacterInfo temp = null;
        int i_PoSition = 0;
        for (int i = 0; i < roomInfo.charIds.Count; i++)
        {
            if (roomInfo.charIds[i] == roomInfo.triggerCharId)
            {
                temp = roomInfo.charInfos[i];
                break;
            }
        }
        Debug.Log("OnSyncRoomInfo  Player");
        switch (roomInfo.gameType)
        {
            case GameType.GT_XWMJ:
                if (temp == null)
                {
                    //int seat = GameManager.GM.GetPlayerNum(roomInfo.triggerCharId);
                    Debug.Log("有人退出房间！！" + roomInfo.triggerCharId.ToString());
                    DataManage.Instance.PData_Reduce(roomInfo.triggerCharId);
                    //////Gm_Manager.G_M.Start_Gm_Ui.UpdatePlayerUI_Quit(roomInfo.triggerCharId.ToString());
                }
                else
                {
                    SeverPlayerNum = (uint)roomInfo.charIds.Count;
                    //PublicEvent.GetINS.Fun_SameIpTip((int)SeverPlayerNum);/*ErrorDelete*/
                    
                    Debug.Log("有人加入房间！！" + temp.charId);

                    Player_Data new_ = new Player_Data();
                    new_.p_ID = temp.charId;
                    new_.P_Name = temp.userName.ToName();
                    new_.p_TxPath = temp.portrait;// "http://localhost/Image/tx_1.png";// "file://" + Application.dataPath + "/Resources/tx_1.png";// temp.portrait;
                    new_.p_gold = (int)temp.gold;
                    new_.p_Ip = temp.ip;
                    new_.sex = (int)temp.sex;
                    Debug.Log("有人加入房间！！" + new_.p_ID);
                    DataManage.Instance.PData_Add(new_);
                }

                break;
            case GameType.GT_THIRt:
                if (temp == null)
                {
                    //int seat = GameManager.GM.GetPlayerNum(roomInfo.triggerCharId);
                    Debug.Log("有人退出房间！！" + roomInfo.triggerCharId.ToString());
                    DataManage.Instance.PData_Reduce(roomInfo.triggerCharId);
                    //////Gm_Manager.G_M.Start_Gm_Ui.UpdatePlayerUI_Quit(roomInfo.triggerCharId.ToString());
                }
                else
                {
                    SeverPlayerNum = (uint)roomInfo.charIds.Count;
                    //PublicEvent.GetINS.Fun_SameIpTip((int)SeverPlayerNum);/*ErrorDelete*/

                    Debug.Log("有人加入房间！！" + temp.charId);

                    Player_Data new_ = new Player_Data();
                    new_.p_ID = temp.charId;
                    new_.P_Name = temp.userName.ToName();
                    new_.p_TxPath = temp.portrait;// "http://localhost/Image/tx_1.png";// "file://" + Application.dataPath + "/Resources/tx_1.png";// temp.portrait;
                    new_.p_gold = (int)temp.gold;
                    new_.p_Ip = temp.ip;
                    new_.sex = (int)temp.sex;
                    Debug.Log("有人加入房间！！" + new_.p_ID);
                    DataManage.Instance.PData_Add(new_);
                }
                break;
            case GameType.GT_DE:
                if (temp == null)
                {
                    //int seat = GameManager.GM.GetPlayerNum(roomInfo.triggerCharId);
                    Debug.Log("有人退出房间！！" + roomInfo.triggerCharId.ToString());
                    DataManage.Instance.PData_Reduce(roomInfo.triggerCharId);
                    //////Gm_Manager.G_M.Start_Gm_Ui.UpdatePlayerUI_Quit(roomInfo.triggerCharId.ToString());
                }
                else
                {
                    SeverPlayerNum = (uint)roomInfo.charIds.Count;
                    //PublicEvent.GetINS.Fun_SameIpTip((int)SeverPlayerNum);/*ErrorDelete*/

                    Debug.Log("有人加入房间！！" + temp.charId);

                    Player_Data new_ = new Player_Data();
                    new_.p_ID = temp.charId;
                    new_.P_Name = temp.userName.ToName();
                    new_.p_TxPath = temp.portrait;// "http://localhost/Image/tx_1.png";// "file://" + Application.dataPath + "/Resources/tx_1.png";// temp.portrait;
                    new_.p_gold = (int)temp.gold;
                    new_.p_Ip = temp.ip;
                    new_.sex = (int)temp.sex;
                    Debug.Log("有人加入房间！！" + new_.p_ID);
                    DataManage.Instance.PData_Add(new_);

                    DZP_PublicEvent.GetINS.AddPlayer(DZP_PublicEvent.GetINS.GetPlayerNull(), temp.charId, temp.userName.ToName(), temp.ip, temp.portrait);
                }

                break;
            default:
                break;
        }
        return true;
    }
}
