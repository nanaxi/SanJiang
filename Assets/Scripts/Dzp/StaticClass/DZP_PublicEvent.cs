using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using System;

public class DZP_PublicEvent
{

    static DZP_PublicEvent _INS;

    public static DZP_PublicEvent GetINS
    {
        get
        {
            if (_INS == null)
            {
                _INS = new DZP_PublicEvent();
            }
            return _INS;
        }
    }

    public MJGameOver Dzp_GameEndRsp { get; private set; }

    public ProtoBuf.QueryInfoRsp DzpRecord;
    public DZP_PublicEvent()
    {
        _INS = this;
    }
    ///////////////////////////////////////////////////////////////
    ///人员排序
    //////////////////////////////////////////////////////////////
    public static PlayerInfo[] AllPlayer = new PlayerInfo[3];
    public int GetPlayerNum(uint charid)
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
                return i;
        }
        Debug.Log("人物序列返回错误:" + charid);
        return -1;
    }
    public string GetPlayerRoomCard(uint charid)
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
                return AllPlayer[i].m_roomCard.ToString();
        }
        Debug.Log("人物序列返回错误:" + charid);
        return "none";
    }
    public string GetPlayerIp(uint charid)
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
                return AllPlayer[i].m_ip.ToString();
        }
        Debug.Log("人物序列返回错误");
        return "127.0.0.1";
    }
    public string GetPlayerHead(uint charid)
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
                return AllPlayer[i].Head;
        }
        return null;
    }
    public string GetPlayerName(uint charid)
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
                return AllPlayer[i].m_account;
        }
        return null;
    }
    /// <summary>
    /// 获取一个空位的玩家当前列表的索引
    /// </summary>
    /// <returns></returns>
    public int GetPlayerNull()
    {
        for (int i = 0; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == 0)
                return i;
        }
        Debug.LogWarning("玩家索引出现错误！");
        return -1;
    }

    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="num">指定索引</param>
    /// <param name="charid"></param>
    /// <param name="name"></param>
    /// <param name="ip"></param>
    /// <param name="head"></param>
    /// <param name="AutoSort"></param>
    public void AddPlayer(int num, uint charid, string name, string ip, string head, bool AutoSort = false)
    {
        AllPlayer[num].m_id = charid;
        AllPlayer[num].m_account = name;
        AllPlayer[num].m_ip = ip;
        AllPlayer[num].Head = head;
        Fun_PlayerComing(num, charid, name, ip, head);
    }
    public void DelPlayer(uint charid)
    {
        for (int i = 1; i < AllPlayer.Length; i++)
        {
            if (AllPlayer[i].m_id == charid)
            {
                AllPlayer[i].m_id = 0;
                AllPlayer[i].m_account = "";
                AllPlayer[i].m_ip = "";
                AllPlayer[i].Head = "";
                break;
            }
        }
    }


    public void DZP_NotifRsp(ProtoBuf.NotifyMJGameOP rsp)
    {
        Debug.Log("收到Notif！");
        if (rsp.doer == 0) //服务器直接告诉玩家谁是庄 没有发出的玩家charid参数
        {

            switch (rsp.op)
            {
                case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                    break;
                case ProtoBuf.MJGameOP.MJ_OP_ROUND_OVER:
                    break;
                case ProtoBuf.MJGameOP.MJ_OP_VOTE_JSROOM:
                    break;
                case ProtoBuf.MJGameOP.MJ_OP_VOTE_RESULT:
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (rsp.doer != 0)
            {
                switch (rsp.op)
                {
                    case ProtoBuf.MJGameOP.MJ_OP_PREP:
                        Fun_GetPlayerReady(rsp.doer);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_XQ:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_MOPAI:
                        Fun_PlayerMoCard(rsp.doer, rsp.card);
                        //Fun_PlayerPopCard(rsp.doer, rsp.card);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:
                        Fun_PlayerPopCard(rsp.doer, rsp.card);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_GUO:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_PENG:
                        Fun_PlayerPengCard(rsp.doer, rsp.card, rsp.oricharId, false);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_FEI:
                        if (rsp.doer == BaseProto.playerInfo.m_id && rsp.card > 0 && rsp.card < 50)
                        {
                            Fun_GetBuCard(rsp.doer, rsp.card);
                        }
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_GANG:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_GANG2://正常杠
                        Fun_PlayerPengCard(rsp.doer, rsp.card, rsp.oricharId, true);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_GANG3://龙牌 手牌有四个的 angang
                        {
                            Fun_PlayerLongCard(rsp.doer, rsp.qishoutiCard[0], rsp.oricharId);
                            if (rsp.doer == BaseProto.playerInfo.m_id && rsp.qishouti.Count > 0)
                            {
                                for (int i = 0; i < rsp.qishouti.Count; i++)
                                {
                                    Fun_GetBuCard(rsp.doer, rsp.qishouti[i]);
                                }
                            }
                            //rsp.qishoutiCard龙的那张牌
                            //rsp.qishouti补牌
                        }
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_GANG4:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_HU:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_CHI:
                        {
                            Fun_PlayerChiCard(rsp.doer, rsp.oricharId, rsp.chipaip, rsp.card);
                        }
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_ROUND_OVER:

                        Dzp_GameEnd();
                        Debug.Log("游戏结束？");
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_VOTE_JSROOM:
                        Fun_ReciveOherVote(rsp.doer, rsp.param);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_VOTE_RESULT:
                        Fun_ReciveVoteResult(rsp.param);
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_ON_LINE:
                        break;
                    case ProtoBuf.MJGameOP.MJ_PLAYER_OP_RESULT:
                        break;
                    case ProtoBuf.MJGameOP.MJ_OP_NULL:
                        break;
                    default:
                        break;
                }
            }
        }

    }
    public delegate void PlayerMoCard(uint doer, uint card);
    public event PlayerMoCard Event_PlayerMoCard;
    private void Fun_PlayerMoCard(uint doer, uint card)
    {
        Event_PlayerMoCard(doer,card);
    }

    public void DZP_AskRsp(ProtoBuf.AskMJGameOP rsp)
    {
        for (int i = 0; i < rsp.canOps.Count; i++)
        {
            if (rsp.canOps[i] == ProtoBuf.MJGameOP.MJ_OP_CHUPAI)
            {
                Fun_SwitchPlayer(rsp.doer);
                Fun_AskPopCard(rsp.doer);
            }
            if (rsp.canOps[i] == ProtoBuf.MJGameOP.MJ_OP_CHI)
            {
                //if (rsp.doer == BaseProto.playerInfo.m_id)
                //{
                    Fun_SwitchPlayer(rsp.doer);
                    Fun_AskChiCard(rsp.doer, rsp.oricharId, rsp.chipaip, rsp.card[0]);
                //}
            }
            if (rsp.canOps[i] == ProtoBuf.MJGameOP.MJ_OP_HU)
            {
                //if (rsp.doer == BaseProto.playerInfo.m_id)
                //{
                    Fun_SwitchPlayer(rsp.doer);
                    Debug.Log("出现胡！");
                    Fun_AskHuCard(rsp.doer,rsp.oricharId ,rsp.card[0], false);
                //}
            }
            if (rsp.canOps[i] == ProtoBuf.MJGameOP.MJ_OP_TING)
            {
                //if (rsp.doer == BaseProto.playerInfo.m_id)
                //{
                    Fun_SwitchPlayer(rsp.doer);
                    Fun_AskPlayerTing(rsp.doer, rsp.card);
                //}
            }


        }
    }




    /// <summary>
    /// 发起投票 打包发给服务器
    /// </summary>
    public void VoteRequest(bool istongyi)
    {
        ProtoBuf.MJGameOpReq pack = new ProtoBuf.MJGameOpReq();
        pack.charId = BaseProto.playerInfo.m_id;
        pack.op = ProtoBuf.MJGameOP.MJ_OP_VOTE_JSROOM;
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
    /// 接收投票方法
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
    public void Dzp_GameOver(ProtoBuf.MJGameOver rsp)
    {
        UIManager.Instance.FindUI(AllPrefabName.uiWin_DzpPlayRoom).GetComponent<UiWin_DzpPlayRoom>().IsFirstRoundOver = true;
        Dzp_GameEndRsp = rsp;
        Fun_GameRestart(rsp);
    }
    public delegate void GameEnd(ProtoBuf.MJGameOver rsp);
    public event GameEnd Event_GameEnd;
    public void Dzp_GameEnd(ProtoBuf.MJGameOver rsp = null)
    {
        Event_GameEnd(rsp);
    }
    public delegate void GetFirstCard(uint Doer, List<uint> cards);
    public event GetFirstCard Event_GetFirstCard;
    public void Fun_GetFirstCard(uint Doer, List<uint> cards)
    {
        Event_GetFirstCard(Doer, cards);
    }

    public delegate void PlayerComing(int num, uint charId, string userName, string ip, string portrait);
    public event PlayerComing Event_PlayerComing;
    public void Fun_PlayerComing(int num, uint charId, string userName, string ip, string portrait)
    {
        Event_PlayerComing(num, charId, userName, ip, portrait);
    }
    public delegate void GameRestart(ProtoBuf.MJGameOver rsp);
    public event GameRestart Event_GameRestart;
    public void Fun_GameRestart(ProtoBuf.MJGameOver rsp)
    {
        Event_GameRestart(rsp);
    }
    public delegate void GetPlayerReady(uint doer);
    public event GetPlayerReady Event_GetPlayerReady;
    public void Fun_GetPlayerReady(uint doer)
    {
        Event_GetPlayerReady(doer);
    }

    #region 接受游戏内主动询问事件
    public delegate void AskPopCard(uint Doer);
    public event AskPopCard Event_AskPopCard;
    public void Fun_AskPopCard(uint Doer)
    {
        Event_AskPopCard(Doer);
    }

    public delegate void AskHuCard(uint Doer,uint orcharid, uint card, bool IsZimo);
    public event AskHuCard Event_AskHuCard;
    public void Fun_AskHuCard(uint Doer,uint orcharid, uint card, bool IsZimo)
    {
        Event_AskHuCard(Doer, orcharid, card, IsZimo);
    }

    public delegate void AskChiCard(uint Doer, uint orcharid, List<ProtoBuf.ChiPaiOP> Chilis, uint card);
    public event AskChiCard Event_AskChiCard;
    public void Fun_AskChiCard(uint Doer, uint orcharid, List<ProtoBuf.ChiPaiOP> Chilist, uint card)
    {
        Event_AskChiCard(Doer, orcharid, Chilist, card);
    }

    public delegate void AskPlayerTing(uint Doer, List<uint> TingCards);
    public event AskPlayerTing Event_AskPlayerTing;
    public void Fun_AskPlayerTing(uint Doer, List<uint> TingCards)
    {
        Event_AskPlayerTing(Doer, TingCards);
    }
    #endregion

    #region 接受游戏内广播事件
    public delegate void PlayerPopCard(uint doer, uint card);
    public event PlayerPopCard Event_PlayerPopCard;
    public void Fun_PlayerPopCard(uint doer, uint card)
    {
        Event_PlayerPopCard(doer, card);
    }

    public delegate void PlayerPengCard(uint doer, uint card, uint orcharid, bool IsGang,bool Reget=false);
    public event PlayerPengCard Event_PlayerPengCard;
    public void Fun_PlayerPengCard(uint doer, uint card, uint orcharid, bool IsGang)
    {
        Event_PlayerPengCard(doer, card, orcharid, IsGang,false);
    }

    public delegate void PlayerLongCard(uint doer, uint card, uint orcharid,bool Reget=false);
    public event PlayerLongCard Event_PlayerLongCard;
    public void Fun_PlayerLongCard(uint doer, uint card, uint orcharid)
    {
        Event_PlayerLongCard(doer, card, orcharid,false);
    }

    public delegate void GetBuCard(uint doer, uint card);
    public event GetBuCard Event_GetBuCard;
    public void Fun_GetBuCard(uint doer, uint card)
    {
        Event_GetBuCard(doer, card);
    }

    public delegate void SwitchPlayer(uint doer);
    public event SwitchPlayer Event_SwitchPlayer;
    public void Fun_SwitchPlayer(uint doer)
    {
        Event_SwitchPlayer(doer);
    }

    public delegate void PlayerChiCard(uint doer, uint oricharId, List<ProtoBuf.ChiPaiOP> chipaip, uint card);
    public event PlayerChiCard Event_PlayerChiCard;
    void Fun_PlayerChiCard(uint doer, uint oricharId, List<ProtoBuf.ChiPaiOP> chipaip, uint card)
    {
        Event_PlayerChiCard(doer, oricharId, chipaip, card);
    }
    #endregion
}
