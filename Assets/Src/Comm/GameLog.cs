using System;
using UnityEngine;

public delegate void LogTraceReceiver(string content);

public enum LogLevel
{
    No,
    Error,
    Trace,
    Warning,
    Info
}

public static class Log
{
    public static LogLevel LogLevel = LogLevel.Info;
    public static LogTraceReceiver TraceReceiver;

    private static object _format(object msg, params object[] args)
    {
        string str = msg as string;
        if ((args.Length != 0) && !string.IsNullOrEmpty(str))
        {
            return string.Format(str, args);
        }
        return msg;
    }

    public static void Error(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Error)
        {
            object str_ = _format(msg, args);
            Debug.LogWarning(str_);
            if (str_.ToString().IndexOf("System.Net.Sockets.SocketException")>=0)
            {
                Debug.Log("<color=red>Socket Error?</color>");
                //UiWin_Prompt.OpenPrompt("<color=red>提示：与服务器断开连接，请检查您的网络");
                UIManager.Instance.ShowUI( AllPrefabName.uiWin_Prompt_Relink ,UIManager.Instance.canvas_T);
                //Gm_Manager.G_M.Open_Prompt("<color=red>提示：与服务器断开连接，请检查您的网络！\n确定将退出游戏！</color>",delegate() { Gm_Manager.G_M.Quit_F(); });
            }
            else if (str_.ToString().IndexOf("Connect Login Server Failed, Please ReLogin") >= 0)
            {
                Debug.Log("<color=red>Socket Error?___1</color>");
                string str_1 = GameManager.GM.IsRelink_C ? "<color=red>提示：重新连接失败！\n请稍候再试！</color>" : "<color=red>提示：登录游戏失败！请稍候再试！</color>";
                UiWin_Prompt.OpenPrompt(str_1);
                //Gm_Manager.G_M.Open_Prompt("<color=red>提示：登录游戏失败！请稍候再试！</color>", delegate () { Gm_Manager.G_M.Quit_F(); });
            }
        }
    }

    public static void Warning(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Warning)
        {
            Debug.LogWarning(_format(msg, args));
        }
    }

    public static void Info(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Info)
        {
            Debug.Log(_format(msg, args));
        }
    }

    public static void Info(object msg, UnityEngine.Object context)
    {
        if (LogLevel >= LogLevel.Info)
        {
            Debug.Log(msg, context);
        }
    }

    public static void Trace(object msg, params object[] args)
    {
        if (LogLevel >= LogLevel.Trace)
        {
            object message = _format(string.Format("Trace: {0}", msg), args);
            if (Application.isEditor)
            {
                Debug.Log(message);
            }
            else if ((TraceReceiver != null) && (message != null))
            {
                TraceReceiver(message.ToString());
            }
        }
    }

    public static void Assert(bool condition, object msg, params object[] args)
    {
        if (!condition)
        {
            Debug.LogError(_format(msg, args));
        }
    }

    public static void Exception(System.Exception ex)
    {
        if (LogLevel >= LogLevel.Error)
        {
            Debug.LogException(ex);
        }
    }
}
