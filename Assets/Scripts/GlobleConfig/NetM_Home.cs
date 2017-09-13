using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetM_Home : SingletonParent<NetM_Home>, IFC_Login_
{
    private bool isQuitRoom = false;

    public bool IsQuitRoom
    {
        get
        {
            return isQuitRoom;
        }

        set
        {
            isQuitRoom = value;
        }
    }

    /// <summary>投票拒绝的ID
    /// </summary>
    private uint tpJJCharId;

    // Use this for initialization
    void Start()
    {
        IsQuitRoom = false;
        NetEvent_Set();
    }

    void OnDestroy()
    {
        NetEvent_Remove();
    }

    public override void Init()
    {
        IsQuitRoom = false;
        NetEvent_Set();
        //throw new NotImplementedException();
    }

    #region/*———PublicEvent事件设置区域———*/
    public void NetEvent_Set()
    {
        //base.Set_NetEvent();
        IFC_Login_ myLogin = this;// GetComponent<IFC_Login_>();
        PublicEvent.GetINS.LoginRest += myLogin.Login_Success;
        PublicEvent.GetINS.LoginFail += myLogin.Login_Fail;

        PublicEvent.GetINS.Event_reciveCreatRoomSuccess += this.CreateRoom_Ok;

        PublicEvent.GetINS.Fun_reciveMessagePreDefine += this.Player_Play_MessagePreDefine;

        PublicEvent.GetINS.Fun_reciveMessageText += this.Player_Play_MessageText;
        //PublicEvent.GetINS.Fun_reciveMessageVoice += this.Player_Play_MessageVoice;
        PublicEvent.GetINS.Event_ReciveVoteResult += this.QuitRoom_Ok;

        PublicEvent.GetINS.Event_ReciveOherVote += this.Recive_QuitRoomTouPiao;

        PublicEvent.GetINS.Event_ExitRoomSucc += this.QuitRoom_Ok;

        PublicEvent.GetINS.Event_ReciveRound_AllOverResult += Open_End_MjAllUI;
    }

    void NetEvent_Remove()
    {
        IFC_Login_ myLogin = this;//GetComponent<IFC_Login_>();
        PublicEvent.GetINS.LoginRest -= myLogin.Login_Success;
        PublicEvent.GetINS.LoginFail -= myLogin.Login_Fail;

        PublicEvent.GetINS.Event_reciveCreatRoomSuccess -= this.CreateRoom_Ok;

        PublicEvent.GetINS.Fun_reciveMessagePreDefine -= this.Player_Play_MessagePreDefine;

        PublicEvent.GetINS.Fun_reciveMessageText -= this.Player_Play_MessageText;
        //PublicEvent.GetINS.Fun_reciveMessageVoice += this.Player_Play_MessageVoice;
        PublicEvent.GetINS.Event_ReciveVoteResult -= this.QuitRoom_Ok;

        PublicEvent.GetINS.Event_ReciveOherVote -= this.Recive_QuitRoomTouPiao;

        PublicEvent.GetINS.Event_ExitRoomSucc -= this.QuitRoom_Ok;

        PublicEvent.GetINS.Event_ReciveRound_AllOverResult -= Open_End_MjAllUI;
    }
    #endregion



    #region/*———PublicEvent事件区域———*/

    void IFC_Login_.Login_Success()
    {
        Debug.Log("AAA Logi Success?" + BaseProto.playerInfo.m_id + "Name_[" + BaseProto.playerInfo.m_account);
        GameManager.GM.GmType = GameSceneType.gm_Home;
        UIManager.Instance.Open_UiObject(AllPrefabName.uiWin_Home);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Login);
    }

    void IFC_Login_.Login_Fail()
    {
        Debug.Log("AAA Logi Fail?");
    }

    void CreateRoom_Ok()
    {
        Debug.Log("CreateRoom_Ok Room OK????");

        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_Home);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_JoinRoom);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_CreateRoom);
        switch (BaseProto.playerInfo.m_inGame)
        {
            //case ProtoBuf.GameType.GT_NULL:
            //    break;
            //case ProtoBuf.GameType.GT_GC:
            //    break;
            //case ProtoBuf.GameType.GT_MJ:
            //    break;
            //case ProtoBuf.GameType.GT_DE:
            //    break;
            case ProtoBuf.GameType.GT_XWMJ:
                GameManager.GM.GmType = GameSceneType.gm_MjGame;
                UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ).SetActive(true);
                GameObject mjGame_ = Model3dManage.Instance.ShowModel("MJGameController", null);
                if (mjGame_.GetComponent<InGameManage_3DMJ>() == null)
                {

                }
                break;
            case ProtoBuf.GameType.GT_THIRt:
                GameObject gmIns = UIManager.Instance.ShowUI(AllPrefabName.UiWin_InGame_13Z, UIManager.Instance.canvas_T);
                gmIns.GetComponent<InGameManage_13Z>().Net_SetEvent();
                break;
            default:
                break;
        }

        
        //mjGame_.GetComponent<InGameManage_3DMJ>().Init_Game();
        GameManager.GM.IF_Assets();
    }

    void Player_Play_MessagePreDefine(uint Sender, string Value)
    {
        if (DataManage.Instance._roomEnterRsp.gameType== ProtoBuf.GameType.GT_XWMJ)//UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ) != null)
        {//接收广播放玩家的表情
            UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ).GetComponent<InGameManage_3DMjUI>().phizAnima_M.Play_PhizAnima(DataManage.Instance.PData_GetIndex(Sender), DataManage.Instance.Phiz_GetPath(Value));
        }else if (DataManage.Instance._roomEnterRsp.gameType == ProtoBuf.GameType.GT_THIRt)
        {//接收广播放玩家的表情
            UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z).GetComponent<InGameManage_13Z>().phizAnima_M.Play_PhizAnima(DataManage.Instance.PData_GetIndex(Sender), DataManage.Instance.Phiz_GetPath(Value));
        }
    }
    void Player_Play_MessageText(uint Sender, string Value)
    {
        if (DataManage.Instance._roomEnterRsp.gameType == ProtoBuf.GameType.GT_XWMJ) //UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ) != null)
        {//接收广播放玩家的消息
            UIManager.Instance.FindUI(AllPrefabName.uiWin_InGame_MJ).GetComponent<InGameManage_3DMjUI>().phizAnima_M.Open_ShowChat(DataManage.Instance.PData_GetIndex(Sender), Value);
        }
        else if (DataManage.Instance._roomEnterRsp.gameType == ProtoBuf.GameType.GT_THIRt)
        {//接收广播放玩家的表情
            UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z).GetComponent<InGameManage_13Z>().phizAnima_M.Open_ShowChat(DataManage.Instance.PData_GetIndex(Sender), Value);
        }
        string strPath = DataManage.Instance.PData_GetData(Sender).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
        switch (BaseProto.playerInfo.m_inGame)
        {
            case ProtoBuf.GameType.GT_DE:
                strPath = DataManage.Instance.PData_GetData(Sender).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
                break;
            case ProtoBuf.GameType.GT_XWMJ:
                strPath = DataManage.Instance.PData_GetData(Sender).sex == 1 ? AudioPath.sexMan : AudioPath.sexWoMan;
                break;
            case ProtoBuf.GameType.GT_THIRt:
                strPath = DataManage.Instance.PData_GetData(Sender).sex == 1 ? AudioPath.sexMan_GM13Z + "KJY/" : AudioPath.sexWoMan_GM13Z + "KJY/";
                break;
            default:
                break;
        }
        Audio_Manage.Instance.Player_Play_Audio(DataManage.Instance.PData_GetIndex(Sender), strPath + DataManage.Instance.Chat_GetChatPath(Value));
    }

    void QuitRoom_Ok(bool yn_Ok)
    {
        if (yn_Ok)
        {
            QuitRoom_Ok();
        }
        else
        {
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_TouPiaoJieGuo);
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
            //UiWin_Prompt.OpenPrompt("解散房间失败！");
            PromptManage.AddPromptSelfClose(new PromptModelSelfClose("解散房间失败！\n\n" + DataManage.Instance.PData_GetData(tpJJCharId).P_Name + "\t 不同意解散！", 3));
        }

    }
    void QuitRoom_Ok()
    {
        PromptManage.AddPromptSelfClose(new PromptModelSelfClose("<color=green>解散房间成功！</color>", 2));

        IsQuitRoom = true;
        //////GameManager.GM.Game_RetrueHome();///ADDD2017年4月28日 11:12:35
        //////if (DataManage.Instance.RoomSyJuShu == 8 && DataManage.Instance.roomJuShu_Max != 16|| DataManage.Instance.RoomSyJuShu == 16)
        //////{
        //////    //GameManager.GM.Game_RetrueHome();
        //////    Invoke("QuitRoomOk_IF", 3f);
        //////}
        //GameManager.GM.Invoke("QuitRoomOk_IF", 3f);
        GameManager.GM.StartCoroutine(this.QuitRoomOk_IF(3));

        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_TouPiaoJieGuo);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
    }

    IEnumerator QuitRoomOk_IF(float fWaitTime)
    {
        Debug.Log("IEnumerator QuitRoomOk_IF ");
        yield return new WaitForSeconds(fWaitTime);
        switch (BaseProto.playerInfo.m_inGame)
        {
            case ProtoBuf.GameType.GT_NULL:
                break;
            case ProtoBuf.GameType.GT_GC:
                break;
            case ProtoBuf.GameType.GT_MJ:
                break;
            case ProtoBuf.GameType.GT_DE:
                break;
            case ProtoBuf.GameType.GT_XWMJ:
                if (UIManager.Instance.FindUI(AllPrefabName.UiWin_MjEnd_1) == null && UIManager.Instance.FindUI(AllPrefabName.uiWin_Home).activeInHierarchy == false)
                {
                    GameManager.GM.Game_RetrueHome();
                }
                break;
            case ProtoBuf.GameType.GT_THIRt:
                Debug.Log("IEnumerator QuitRoomOk_IF  GameType.GT_THIRt  ");
                if (UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z) != null
                    && UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z).GetComponent<InGameManage_13Z>().isEnd1Show == false
                    && UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z).GetComponent<InGameManage_13Z>().isEndAllShow == false
                    && UIManager.Instance.FindUI(AllPrefabName.uiWin_Home).activeInHierarchy == false)
                {//是否打开结算界面。  如果没有打开。 就返回Home
                    GameManager.GM.Game_RetrueHome();
                }
                else if (UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z) != null && UIManager.Instance.FindUI(AllPrefabName.uiWin_Home).activeInHierarchy == false)
                {
                    UIManager.Instance.FindUI(AllPrefabName.UiWin_InGame_13Z).GetComponent<InGameManage_13Z>().isTPCG = true;
                }

                break;
            default:
                break;
        }
        yield return null;
    }

    void Recive_QuitRoomTouPiao(uint charid, bool istongyi)
    {
        Debug.Log("Recive Open  QuitRoom Request");
        if (!istongyi)
        {
            tpJJCharId = charid;
        }

        UiWin_TouPiaoJieGuo uiTpjg = UIManager.Instance.ShowUI(AllPrefabName.uiWin_TouPiaoJieGuo, UIManager.Instance.canvas_T).GetComponent<UiWin_TouPiaoJieGuo>();
        uiTpjg.transform.SetAsLastSibling();
        uiTpjg.Init_UI(DataManage.Instance.PData_GetDataAry(), DataManage.Instance.PData_GetData(charid).P_Name);
        uiTpjg.Set_QuiaRoom_TouPiao(charid, istongyi);
    }

    void Open_End_MjAllUI(List<ProtoBuf.MJGameOver> end_Data)
    {
        Debug.Log("Open_EndALL Panel Data"+ DataManage.Instance._roomEnterRsp.gameType.ToString());
        if (DataManage.Instance._roomEnterRsp.gameType == ProtoBuf.GameType.GT_THIRt)
        {
            return;
        }

        UiWin_QJJS qjjs_ = UIManager.Instance.ShowUI(AllPrefabName.uiWin_QuanJuJieSuan, UIManager.Instance.canvas_T).GetComponent<UiWin_QJJS>();
        qjjs_.Init(qjjs_.transform);
        qjjs_.Fun_ReciveAllGameOverResult(end_Data);

    }

    
    #endregion
}


