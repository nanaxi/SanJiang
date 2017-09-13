using System;
using UnityEngine;
using System.Collections;
using ProtoBuf;

public class MJProto : ISingleton<MJProto>
{
    public bool MJGameOPRequest(MJGameOpReq pack)
    {
        pack.charId = BaseProto.playerInfo.m_id;


        UInt16 command = (UInt16)CLIToLGIProtocol.CLI_TO_LGI_MJ_GAME_OP;
        return GameNetWork.Inst().SendDataToLoginServer(command, pack);
    }

    // LGI_TO_CLI_MJ_HAND_CARDS
    public bool OnMJHandCardInfo(MJHandCardInfo rsp)
    {
        if (BaseProto.playerInfo.m_inGame == GameType.GT_DE)
            DZP_PublicEvent.GetINS.Fun_GetFirstCard(rsp.charId, rsp.cards);
        else {
            PublicEvent.GetINS.Fun_reciveGetGuiCards(rsp.guiCards, 0);
            //调用发放手牌方法
            PublicEvent.GetINS.Fun_reciveGetHandCards(rsp.cards, 0);
        }
       
        
        return true;
    }
    public bool isPengGangHu = false;
    /// <summary>
    /// 牌是否可以落下，在打出一张牌之后为Ture，当
    /// </summary>
    public bool PopcardIsWaiting = false;

    public bool IsQiangGang = false;
    public bool IsGangCardDown = false;

    uint LastPlayerCard;
    //public uint LastChupaiCharid = 0;
    //public uint LastGangPaiCharid = 0;

    bool UpdatePaiShu = false;
    int CardNum = -1;


    // LGI_TO_CLI_MJ_ASK_GAME_OP
    /// <summary>
    /// 通知本地玩家操作或者通知其他玩家进行操作
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>

    public void OnAskMJGameOP(AskMJGameOP rsp)
    {
        if (BaseProto.playerInfo.m_inGame == GameType.GT_DE)
            DZP_PublicEvent.GetINS.DZP_AskRsp(rsp);
        else
        {
            //如果当前玩家里面的牌里面有可以碰杠胡的牌，就告诉玩家可以碰杠胡了
            Debug.Log("我收到Ask" + rsp.canOps.Count + "       " + "Ask相关玩家Id" + rsp.oricharId);
            if (rsp.canOps.Count >= 1)
            {
                IsGangCardDown = false;
                for (int i = 0; i < rsp.canOps.Count; i++)
                {
                    Debug.Log("Recv_ MJOP" + rsp.canOps[i].ToString());
                    switch (rsp.canOps[i])
                    {
                        case MJGameOP.MJ_OP_XQ:
                            break;
                        case MJGameOP.MJ_OP_MOPAI:
                            break;
                        case MJGameOP.MJ_OP_CHUPAI:
                            PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_CHUPAI, rsp.card, rsp.oricharId);
                            break;
                        case MJGameOP.MJ_OP_GUO:
                            break;
                        case MJGameOP.MJ_OP_PENG:
                            PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_PENG, rsp.card, rsp.oricharId);
                            break;
                        case MJGameOP.MJ_OP_GANG:
                            Debug.Log(rsp.gangOp.Count + "张牌可杠");
                            for (int v = 0; v < rsp.gangOp.Count; v++)
                            {
                                //////Debug.Log(rsp.gangCards[v].ToCard().ToName() + " 杠的牌花 ");
                            }
                            System.Collections.Generic.List<uint> gangList = new System.Collections.Generic.List<uint>();
                            for (int iGang = 0; iGang < rsp.gangOp.Count; iGang++)
                            {
                                gangList.Add(rsp.gangOp[iGang].card);
                            }
                            PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_GANG, /*new System.Collections.Generic.List<uint>()*/gangList, rsp.oricharId);

                            break;
                        case MJGameOP.MJ_OP_GANG2:
                            break;
                        case MJGameOP.MJ_OP_GANG3:
                            break;
                        case MJGameOP.MJ_OP_GANG4:
                            break;
                        case MJGameOP.MJ_OP_HU:
                            PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_HU, rsp.card, rsp.oricharId);
                            break;
                        case MJGameOP.MJ_OP_FEI:
                            PublicEvent.GetINS.Fun_KeYiFeiPeng(rsp.doer, rsp.card[0]);
                            break;
                        case MJGameOP.MJ_OP_TI:
                            PublicEvent.GetINS.Fun_KeYiTiPai(rsp.doer, rsp.tiCards[0]);
                            break;
                        case MJGameOP.MJ_OP_TI2:
                            break;
                        case MJGameOP.MJ_OP_TI3:
                            break;
                        case MJGameOP.MJ_OP_TI4:
                            break;
                        case MJGameOP.MJ_OP_NULL:
                            break;
                        case MJGameOP.MJ_OP_TING:
                            Debug.Log("Ting???????");
                            PublicEvent.GetINS.Fun_KeYiBao(rsp.doer, rsp.tingCards);
                            break;
                        default:
                            break;
                    }
                }

            }
            else
            {
                //可以出牌

                //PopcardIsWaiting = true;
                //IsGangCardDown = true;
                Debug.Log("cardID" + rsp.card.Count + "" + (rsp.card.Count > 0 ? "Card" + rsp.card[0].ToString() : "_NULL"));
                PublicEvent.GetINS.Fun_reciveCanPlay(rsp.doer, MJGameOP.MJ_OP_CHUPAI, rsp.card, rsp.oricharId);

            }

        }

    }


    // LGI_TO_CLI_MJ_NOTIFY_GAME_OP
    /// <summary>
    /// 服务器广播消息
    /// </summary>
    /// <param name="rsp"></param>
    /// <returns></returns>
    public bool OnNotifyMJGameOP(NotifyMJGameOP rsp)
    {
        if (BaseProto.playerInfo.m_inGame == GameType.GT_DE)
            DZP_PublicEvent.GetINS.DZP_NotifRsp(rsp);
        else
        {
            Debug.Log("Server:" + rsp.op + "___" + rsp.doer + "   " + rsp.oricharId + "  " + rsp.card);

            if (rsp.doer == 0) //服务器直接告诉玩家谁是庄 没有发出的玩家charid参数
            {
                switch (rsp.op)
                {
                    //Debug.Log(rsp.op);

                    case MJGameOP.MJ_OP_ZJ:
                        // rsp.param //庄家ID
                        PublicEvent.GetINS.Fun_reciveZhuang(rsp.param);
                        break;
                    //case MJGameOP.MJ_OP_X3Z_RESULT:
                    //    //PublicEvent.GetINS.Change3ZhangResult(rsp.x3zCardOut);    //换三张的结果
                    //    //   rsp.x3zCardOut //换三张的结果
                    //    break;

                    case MJGameOP.MJ_OP_VOTE_RESULT:
                        PublicEvent.GetINS.Fun_ReciveVoteResult(rsp.param);
                        break;
                    case MJGameOP.MJ_OP_ROUND_OVER:
                        Debug.Log("MJGameOP.MJ_OP_ROUND_OVER");
                        PublicEvent.GetINS.Fun_ReciveGameOverResult();
                        break;

                    case MJGameOP.MJ_OP_XQ:
                        if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                        {
                            Debug.Log("比牌没有通过？？：" + BaseProto.playerInfo.m_id);
                            PublicEvent.GetINS.Fun_reciveSelectQue(rsp.charId, 404);
                        }
                        break;
                    case MJGameOP.MJ_OP_CHUPAI:
                        if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                        {
                            Debug.Log("开始比牌？：" + rsp.cantOpCards.Count);
                            PublicEvent.GetINS.Fun_Recive13BiPai(rsp.carddate);
                            break;
                        }
                        break;

                    default:
                        break;
                }
            }

            else if (rsp.doer != 0)
            {

                Debug.Log("收到某玩家消息");
                switch (rsp.op)
                {
                    case MJGameOP.MJ_OP_PREP:
                        PublicEvent.GetINS.Fun_recivePlayerReady(rsp.doer);
                        //PublicEvent.GetINS.Fun_PlayerUpdata(DataManage.Instance.PData_GetIndex(rsp.doer));
                        break;

                    case MJGameOP.MJ_OP_VOTE_RESULT:
                        PublicEvent.GetINS.Fun_ReciveVoteResult(rsp.param);
                        break;
                    case MJGameOP.MJ_OP_XQ:
                        PublicEvent.GetINS.Fun_reciveSelectQue(rsp.doer, rsp.param);
                        break;
                    case MJGameOP.MJ_OP_MOPAI:
                        PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                        //本地玩家摸牌
                        //Debug.Log("本地收到摸牌消息");
                        isPengGangHu = false;
                        PopcardIsWaiting = false;
                        IsGangCardDown = true;
                        IsQiangGang = false;
                        PublicEvent.GetINS.Fun_reciveGetCard(rsp.doer, false, rsp.card);
                        Debug.Log("收到摸牌消息 剩余的牌数" + (int)rsp.param);
                        if ((int)rsp.param != 0)
                        {
                            PublicEvent.GetINS.Fun_UpdatePaishu((int)rsp.param);
                            CardNum = (int)rsp.param;
                            if (!UpdatePaiShu)
                            {
                                UpdatePaiShu = true;
                            }
                        }
                        else if (!UpdatePaiShu)
                        {
                            UpdatePaiShu = true;
                            PublicEvent.GetINS.Fun_UpdatePaishu((int)rsp.param);
                            CardNum = (int)rsp.param;
                        }
                        else if (UpdatePaiShu)
                        {
                            PublicEvent.GetINS.Fun_UpdatePaishu((int)rsp.param);
                            CardNum = (int)rsp.param;
                        }

                        break;
                    case MJGameOP.MJ_OP_CHUPAI:

                        PopcardIsWaiting = true;
                        isPengGangHu = false;
                        PublicEvent.GetINS.Fun_reciveOtherPopCard(rsp.doer, rsp.card);
                        break;
                    case MJGameOP.MJ_OP_GUO:
                        if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                        {
                            Debug.Log("开始比牌？：" + rsp.cantOpCards.Count);
                            PublicEvent.GetINS.Fun_Recive13BiPai(rsp.carddate);
                            break;
                        }
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);/*ErrorDelete*/
                                                                                                                //////PublicEvent_.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                        break;
                    case MJGameOP.MJ_OP_PENG:
                        if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                        {
                            Debug.Log("开始比牌？：" + rsp.cantOpCards.Count);
                            PublicEvent.GetINS.Fun_Recive13BiPai(rsp.carddate);
                            break;
                        }
                        //Debug.Log("玩家已经碰了");
                        PopcardIsWaiting = false;
                        isPengGangHu = true;
                        PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                        //PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);/*ErrorDelete*/
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);

                        break;
                    case MJGameOP.MJ_OP_GANG:
                        //LastGangPaiCharid = rsp.doer;
                        if (DataManage.Instance._roomEnterRsp.gameType == GameType.GT_THIRt)
                        {//13张有人打枪
                            PublicEvent.GetINS.Fun_Recive13DaQiang(rsp.doer, rsp.qishouti);
                        }

                        PopcardIsWaiting = false;
                        isPengGangHu = true;

                        IsGangCardDown = false;
                        IsQiangGang = false;

                        PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                        //PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);/*ErrorDelete*/
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);

                        break;
                    case MJGameOP.MJ_OP_HU:
                        IsGangCardDown = true;
                        PopcardIsWaiting = false;
                        isPengGangHu = true;

                        PublicEvent.GetINS.Fun_DirLight(rsp.doer);
                        //PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);/*ErrorDelete*/
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);

                        break;
                    case MJGameOP.MJ_OP_TING:

                        Debug.Log("Ntf_Ting???????");
                        //////PublicEvent_.GetINS.Fun_KeYiTing(rsp.doer,rsp.op,rsp.tingCards);
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                        break;
                    case MJGameOP.MJ_OP_ON_LINE:
                        //有人断线或重连
                        PublicEvent.GetINS.Fun_reciveGetIsOnLine(rsp.doer, rsp.param);
                        break;

                    case MJGameOP.MJ_OP_FEI:
                        Debug.Log("可以飞碰");
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                        break;

                    case MJGameOP.MJ_OP_TI:
                        Debug.Log("可以提");
                        PublicEvent.GetINS.Fun_reciveOtherPengGangHu(rsp.op, rsp.doer, rsp.card, rsp.oricharId);
                        break;
                    //case MJGameOP.MJ_OP_Notify_DIAMONDS:
                    //    PublicEvent.GetINS.DiamondRequst();
                    //    break;

                    default:
                        break;
                }

                switch (rsp.op)
                {

                    case MJGameOP.MJ_OP_VOTE_JSROOM:
                        //Debug.Log("收到有人投票");
                        PublicEvent.GetINS.Fun_ReciveOherVote(rsp.doer, rsp.param);
                        break;
                }

            }
            if (BaseProto.playerInfo.m_inGame == GameType.GT_THIRt)
            {
                OnNotifyMJGameOP_13Z(rsp);
            }

        }
        return true;
    }

    void OnNotifyMJGameOP_13Z(NotifyMJGameOP rsp)
    {

        Debug.Log("GameType.GT_THIRt:" + rsp.op + "___" + rsp.doer + "   " + rsp.oricharId + "  " + rsp.card);
        THIRTGameOP thirtOP = THIRTGameOP.THIRT_OP_NULL;// (THIRTGameOP)rsp.op;
        try
        {
            thirtOP = (THIRTGameOP)rsp.op;
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>OnNotifyMJGameOP_13Z ERROR:</color>" + e.Message);
            PromptManage.AddPrompt(new PromptModel("未知错误 Error:OnNotifyMJGameOP_13Z", delegate () { }));
            return;
        }

        if (rsp.doer == 0)
        {

        }
        else if (rsp.doer != 0)
        {
            switch (thirtOP)
            {
                //case THIRTGameOP.THIRT_OP_PREP:
                //    break;
                //case THIRTGameOP.THIRT_OP_GUO:
                //    break;
                //case THIRTGameOP.THIRT_OP_HU:
                //    break;
                //case THIRTGameOP.THIRT_OP_TING:
                //    break;
                //case THIRTGameOP.THIRT_OP_START_THAN:
                //    break;
                //case THIRTGameOP.THIRT_OP_NOW_THAN_OVER:
                //    break;
                //case THIRTGameOP.THURT_OP_STATE_WEATE:
                //    break;
                //case THIRTGameOP.THURT_OP_STATE_SHOOT:
                //    break;
                //case THIRTGameOP.THURT_OP_STATE_HOMERUN:
                //    break;
                //case THIRTGameOP.THIRT_OP_ROUND_OVER:
                //    break;
                case THIRTGameOP.THIRT_OP_VOTE_JSROOM:
                    //PublicEvent.GetINS.Fun_ReciveVoteResult(rsp.param);
                    PublicEvent.GetINS.Fun_ReciveOherVote(rsp.doer, rsp.param);
                    break;
                //case THIRTGameOP.THIRT_OP_VOTE_RESULT:
                //    break;
                //case THIRTGameOP.THIRT_OP_ON_LINE:
                //    break;
                //case THIRTGameOP.THIRT_PLAYER_OP_RESULT:
                //    break;
                //case THIRTGameOP.THIRT_OP_NULL:
                //    break;
                default:
                    break;
            }
        }
    }


    // LGI_TO_CLI_MJ_GAME_OVER
    public bool OnMJGameOver(MJGameOver rsp)
    {
        if (BaseProto.playerInfo.m_inGame == GameType.GT_DE)
            DZP_PublicEvent.GetINS.Dzp_GameOver(rsp);
        else
        {
            Debug.Log("What");

            //PublicEvent_.GetINS.Fun_ReciveRoundOverResult(rsp);
            UpdatePaiShu = false;
            PopcardIsWaiting = false;
            isPengGangHu = false;
            IsQiangGang = false;
            IsGangCardDown = false;
            CardNum = -1;
            if (rsp.befores.Count != 0)
            {
                Debug.Log(" All Mj GameMeOver == MaxJuShu");

                PublicEvent.GetINS.Fun_ReciveRoundOverResult(rsp);//
                rsp.befores.Add(rsp);
                if (BaseProto.playerInfo.m_inGame == GameType.GT_XWMJ)
                {
                    Model3dManage.Instance.FindModel("MJGameController").GetComponent<InGameManage_3DMJ>().END_SetEndAll(rsp);
                }
                else if (BaseProto.playerInfo.m_inGame == GameType.GT_THIRt)
                {
                    Debug.Log("13Z END All");
                    PublicEvent.GetINS.Fun_ReciveAllGameOverResult(rsp.befores);
                }

                return true;
            }
            else
            {
                Debug.Log(" End_OneGame" + rsp.befores.Count);
            }

            PublicEvent.GetINS.Fun_ReciveRoundOverResult(rsp);
        }

        //回合结束 创建结算窗口 
        //麻将预制初始化
        return true;
    }
}
