using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SdkEvent : MonoBehaviour {
    #region Instance Define
    static SdkEvent inst;
    static public SdkEvent Instance
    {
        get
        {
            if (inst==null)
            {
                GameObject gm = new GameObject();
                gm.name = "___SdkEvent";
                inst = gm.AddComponent<SdkEvent>();
            }
            return inst;
        }
    }
    #endregion


    // Use this for initialization
    void Awake () {
        //ShareResult()
        if (inst==null)
        {
            inst = this;
        }
    }
    #region/*———分享图片———*/

    /// <summary>分享图片
    /// </summary>
    public void ShareResult()
    {
        Dictionary<string, string> info = new Dictionary<string, string>();
        info["mediaType"] = "1"; //分享类型： 0-文字 1-图片 2-网址  
        info["shareTo"] = "0"; //分享到：0-聊天 1-朋友圈 2-收藏  
        info["imagePath"] = CaptureScreenshot2();
        //info["text"] = "測試！";
        AnySDKManager.SendShare(info);
    }
    string CaptureScreenshot2()
    {
        // 先创建一个的空纹理，大小可根据实现需要来设置  
        Texture2D screenShot = new Texture2D(Screen.width - (int)((Screen.width * 0.05f) * 2), Screen.height - (int)((Screen.height * 0.05f) * 5), TextureFormat.RGB24, false);

        try
        {
            // 读取屏幕像素信息并存储为纹理数据，  
            screenShot.ReadPixels(new Rect(Screen.width * 0.05f, (int)((Screen.height * 0.05f) * 2.5), Screen.width - (int)((Screen.width * 0.05f) * 2f), Screen.height - (int)((Screen.height * 0.05f) * 5)), 0, 0);
            screenShot.Apply();
        }
        catch (Exception e)
        {
            Debug.Log("<color=red>ReadPixels</color>" + e.Message);
            //throw;
        }
        
        // 然后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = GameManager.GM.JsonPath + "/Screenshot.png";
        Debug.Log("Path :" + filename);
        System.IO.File.WriteAllBytes(filename, bytes);
        // 最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。  
        return filename;
    }
    #endregion

    /// <summary>邀请好友
    /// </summary>
    public void OnClick_Btn_GameYaoQing()
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        try
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            info["mediaType"] = "2"; //分享类型： 0-文字 1-图片 2-网址  
            info["shareTo"] = "0"; //分享到：0-聊天 1-朋友圈 2-收藏  

            Debug.Log(" LOOk ：YaoQing~~~~~~~~~~~~~~");

            info["title"] = "约战划水";
            info["imagePath"] = GameManager.GM.JsonPath + "/icon.png";
            info["url"] = "http://www.yuezhanhuashui.com/";
            info["text"] = "房间号:" + BaseProto.playerInfo.m_cdRoomId + ",局数:" + DataManage.Instance.roomJuShu_Max.ToString() + ",邀请您参与" +  DataManage.Instance.RoomInfoNxStr .Replace("\n", "") + ",速度加入[约战划水].";
            info["thumbSize"] = "64";
            AnySDKManager.SendShare(info);

        }
        catch (Exception e)
        {
            UiWin_Prompt.OpenPrompt("加载分享出错？？？" + e.Message);
            throw;
        }

    }

    /// <summary>邀请好友
    /// </summary>
    public void OnClick_Home_GameYaoQing()
    {
        try
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            info["mediaType"] = "2"; //分享类型： 0-文字 1-图片 2-网址  
            info["shareTo"] = "0"; //分享到：0-聊天 1-朋友圈 2-收藏  

            Debug.Log(" LOOk ：YaoQing~~~~~~~~~~~~~~");

            info["title"] = "约战划水";
            info["imagePath"] = GameManager.GM.JsonPath + "/icon.png";
            info["url"] = "http://www.yuezhanhuashui.com/";
            info["text"] = "最经典的划水玩法，精致细腻的游戏画面，快来约战划水吧！";
            info["thumbSize"] = "64";
            AnySDKManager.SendShare(info);

        }
        catch (Exception e)
        {
            UiWin_Prompt.OpenPrompt("加载分享出错？？？" + e.Message);
            throw;
        }

    }

}

