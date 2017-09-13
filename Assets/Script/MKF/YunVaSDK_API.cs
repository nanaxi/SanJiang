using UnityEngine;
using System.Collections;
using System;
using YunvaIM;

public class YunVaSDK_API : MonoBehaviour
{
    private string sUserId = "123";
    private string labelText = "ssss";
    string filePath = "";
    private string recordPath = string.Empty;//返回录音地址
    private string recordUrlPath = string.Empty;//返回录音url地址

    //private int i_Jl_;//測試記錄


    void Start()
    {
        //i_Jl_ = 0;
        Event.Inst().StartMic += OnClick;   //注册点击方法. 第一次触发录音第二次触发结束录音..在触发结束录音时如果传入false值则不发送
        Event.Inst().PlaySound += DownloadSound;
        int init = YunVaImSDK.instance.YunVa_Init(0, 1001588, Application.persistentDataPath, false);
        EventListenerManager.AddListener(ProtocolEnum.IM_RECORD_VOLUME_NOTIFY, ImRecordVolume);//录音音量大小回调监听
        OnLogin("");
    }


    bool IsStart = true;
    public void OnClick(bool isSend = true)
    {
        if (IsStart)
        {
            OnStartSoundRecording();
        }
        else
        {
            OnStopSoundRecording(isSend);
        }
        IsStart = !IsStart;
    }

    /// <summary>
    /// 登录方法
    /// </summary>
    void OnLogin(string tt1, string GameSeverID = "1111")
    {
        sUserId = BaseProto.playerInfo.m_id.ToString();
        string ttFormat = "{{\"nickname\":\"{0}\",\"uid\":\"{1}\"}}";
        string tt = string.Format(ttFormat, sUserId, sUserId);
        string[] wildcard = new string[2];
        wildcard[0] = "0x001";
        wildcard[1] = "0x002";
        YunVaImSDK.instance.YunVaOnLogin(tt, GameSeverID, wildcard, 0, (data) =>
        {
            if (data.result == 0)
            {

                //  labelText = string.Format("登录成功，昵称:{0},用户ID:{1}", data.nickName, data.userId);
                YunVaImSDK.instance.RecordSetInfoReq(true);//开启录音的音量大小回调
            }
            else
            {
                // labelText = string.Format("登录失败，错误消息：{0}", data.msg);
            }
        });
    }

    public void OnStartSoundRecording()
    {
        //YunVaImSDK.instance.RecordStopPlayRequest();
        ynStop = true;
        StartCoroutine("StopAudio_");
        //Audio_Manage.AudioManage_Static.StopAudioOrPlay_ALLAudio(false);

        filePath = string.Format("{0}/{1}.amr", Application.persistentDataPath, DateTime.Now.ToFileTime());
        YunVaImSDK.instance.RecordStartRequest(filePath);
    }

    bool ynStop;

    IEnumerator StopAudio_()
    {

        while (ynStop)
        {

            YunVaImSDK.instance.RecordStopPlayRequest();
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void OnStopSoundRecording(bool isSend)
    {
        if (isSend)
        {
            YunVaImSDK.instance.RecordStopRequest(StopRecordResponse);


            ynStop = false;
            StopCoroutine("StopAudio_");
            //Audio_Manage.AudioManage_Static.StopAudioOrPlay_ALLAudio(true);
        }
    }


    public void PlaySound()
    {
        //labelText = "播放语音.........";
        string ext = DateTime.Now.ToFileTime().ToString();
        
        YunVaImSDK.instance.RecordStartPlayRequest(recordPath, "", ext, (data2) =>
            {
            });
    }
    /// <summary>
    /// 上传录音到服务器
    /// </summary>
    public void SendSound()
    {
        string fileId = DateTime.Now.ToFileTime().ToString();
        YunVaImSDK.instance.UploadFileRequest(recordPath, fileId, (data1) =>
        {
            if (data1.result == 0)
            {
                recordUrlPath = data1.fileurl;
                ProtoBuf.ChatMessageReq temp = new ProtoBuf.ChatMessageReq();
                temp.msgType = ProtoBuf.ChatMessageReq.MsgType.InputVoice;
                temp.msgString = recordUrlPath;
                temp.senderId = BaseProto.playerInfo.m_id;
                BaseProto.Inst().SendChatMsgRequest(temp);
                DownloadSound(recordUrlPath);
            }
        });
    }
    public void DownloadSound(string url)
    {
        //i_Jl_++;
        //UiWin_Prompt.OpenPrompt("播放语音.........");
        YunVaImSDK.instance.RecordStartPlayRequest("", url, "", (data2) =>
          {
              if (data2.result == 0)
              {
                  //Debug.Log("播放wanc");
                  //Gm_Manager.G_M.Open_Prompt("播放？？？END_XXXXX");
                  //Audio_Manage.AudioManage_Static.PlayOrStop_BgAudio(true);
              }
              else
              {
                  //Debug.Log("播放失败");
                  //Gm_Manager.G_M.Open_Prompt("播放？？？END_BBBB");
              }
          });

        //Gm_Manager.G_M.Open_Prompt("播放？？？END" + i_Jl_);
    }




    private void StopRecordResponse(ImRecordStopResp data)
    {
        if (!string.IsNullOrEmpty(data.strfilepath))
        {
            recordPath = data.strfilepath;
            SendSound();
        }
    }
    public void ImRecordVolume(object data)
    {
        ImRecordVolumeNotify RecordVolumeNotify = data as ImRecordVolumeNotify;
    }

}
