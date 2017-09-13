using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetBase;
using UnityEngine;
using ProtoBuf;

public class LoginNetHandle 
{
    public LoginNetHandle()
    {
    }

    ~LoginNetHandle()
    {
    }

    public int Init(NetPackHandle handle)
    {
        RegistNetHandle(handle);
        return 0;
    }

    public void Shutdown()
    {
    }

    protected int RegistNetHandle(NetPackHandle handle)
    {
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_CHECK_VERSION, OnCheckVersion);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_ACCOUNT_LOGIN, OnAccountLogin);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_NOTIFY_MESSAGE, OnNotifyMessage);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_QUERY_INFO, OnQueryInfo);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_SHATA_NUM, OnQuerysSharaNum);//0602分享
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_ENTER_GAME, OnEnterGame);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_EXIT_GAME, OnExitGame);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_CREATE_ROOM, OnCreateRoom);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_ENTER_ROOM, OnEnterRoom);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_EXIT_ROOM, OnExitRoom);

        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_CHAT_MESAGE, OnRecvChatMessage);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_ROOM_INFO, OnRoomInfo);

        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_MJ_HAND_CARDS, OnMJHandCards);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_MJ_ASK_GAME_OP, OnMJAskGameOP);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_MJ_NOTIFY_GAME_OP, OnMJNotifyGameOP);
        handle.RegistNetPackHandle((UInt16)LGIToCLIProtocol.LGI_TO_CLI_MJ_GAME_OVER, OnMJGameOver);


        return 0;
    }

    private int OnCmdResponse(Stream stream, NetSession session)
    {
        return 0;
    }

    int OnCheckVersion(Stream stream, NetSession session)
    {
        CheckVersionRsp rspPack = Serializer.Deserialize<CheckVersionRsp>(stream);
        LoginProcessor.Inst().CheckVersionResponse(rspPack);
        return 0;
    }

    int OnAccountLogin(Stream stream, NetSession session)
    {
        AccountLoginRsp rspPack = Serializer.Deserialize<AccountLoginRsp>(stream);
        LoginProcessor.Inst().AccountLoginResponse(rspPack);
        return 0;
    }

    private int OnNotifyMessage(Stream stream, NetSession session)
    {
        return 0;
    }

    private int OnQueryInfo(Stream stream, NetSession session)
    {
        QueryInfoRsp rspPack = Serializer.Deserialize<QueryInfoRsp>(stream);
        BaseProto.Inst().QueryInfoResponse(rspPack);
        return 0;
    }

    private int OnQuerysSharaNum(Stream stream, NetSession session)
    {
        QueryInfoRsp rspPack = Serializer.Deserialize<QueryInfoRsp>(stream);
        BaseProto.Inst().RcvShara(rspPack);
        return 0;
    }

    private int OnEnterGame(Stream stream, NetSession session)
    {
        EnterGameRsp rspPack = Serializer.Deserialize<EnterGameRsp>(stream);
        BaseProto.Inst().EnterGameResponse(rspPack);
        return 0;
    }

    private int OnExitGame(Stream stream, NetSession session)
    {
        ExitGameRsp rspPack = Serializer.Deserialize<ExitGameRsp>(stream);
        BaseProto.Inst().ExitGameResponse(rspPack);
        return 0;
    }

    private int OnCreateRoom(Stream stream, NetSession session)
    {
        CreateRoomRsp rspPack = Serializer.Deserialize<CreateRoomRsp>(stream);
        BaseProto.Inst().CreateRoomResponse(rspPack);
        return 0;
    }

    private int OnEnterRoom(Stream stream, NetSession session)
    {
        EnterRoomRsp rspPack = Serializer.Deserialize<EnterRoomRsp>(stream);
        BaseProto.Inst().EnterRoomResponse(rspPack);
        return 0;
    }

    private int OnExitRoom(Stream stream, NetSession session)
    {
        ExitRoomRsp rspPack = Serializer.Deserialize<ExitRoomRsp>(stream);
        BaseProto.Inst().ExitRoomResponse(rspPack);
        return 0;
    } 

    private int OnRecvChatMessage(Stream stream, NetSession session)
    {
        ChatMessageRsp rspPack = Serializer.Deserialize<ChatMessageRsp>(stream);
        BaseProto.Inst().RecvChatMsgResponse(rspPack);
        return 0;
    }

    private int OnRoomInfo(Stream stream, NetSession session)
    {
        SyncRoomInfo rspPack = Serializer.Deserialize<SyncRoomInfo>(stream);
        BaseProto.Inst().OnSyncRoomInfo(rspPack);
        return 0;
    }

    private int OnMJHandCards(Stream stream, NetSession session)
    {
        MJHandCardInfo rspPack = Serializer.Deserialize<MJHandCardInfo>(stream);
        MJProto.Inst().OnMJHandCardInfo(rspPack);
        return 0;
    }

    private int OnMJAskGameOP(Stream stream, NetSession session)
    {
        AskMJGameOP rspPack = Serializer.Deserialize<AskMJGameOP>(stream);
        MJProto.Inst().OnAskMJGameOP(rspPack);
        return 0;
    }

    private int OnMJNotifyGameOP(Stream stream, NetSession session)
    {
        NotifyMJGameOP rspPack = Serializer.Deserialize<NotifyMJGameOP>(stream);
        MJProto.Inst().OnNotifyMJGameOP(rspPack);
        return 0;
    }

    private int OnMJGameOver(Stream stream, NetSession session)
    {
        MJGameOver rspPack = Serializer.Deserialize<MJGameOver>(stream);
        MJProto.Inst().OnMJGameOver(rspPack);
        return 0;
    }


}
