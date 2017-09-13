using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>聊天记录显示
/// </summary>
public class UiWinX_ChatRecord : MonoBehaviour
{
    [SerializeField]private uint maxChatLength = 5000;

    [SerializeField]
    private Text t_ChatRecord;

    [SerializeField]
    private ScrollRect svChatRecord;

    void OnEnable()
    {
        ShowChatAdd(DataManage.Instance.ChatRecord_GetAndClear());
    }

    // Use this for initialization
    void Start()
    {
        //ClearChatRecord();
    }

    /// <summary>显示聊天记录
    /// </summary>
    public void ShowChatAdd(string strChat)
    {
        if (t_ChatRecord.text.Length > maxChatLength)
        {
            t_ChatRecord.text = "";
        }
        t_ChatRecord.text += strChat + "\n\n";
        StartCoroutine(SetSRect());
    }
    
    /// <summary>显示聊天记录
    /// </summary>
    public void ShowChatAdd(string[] strChatAry)
    {
        for (int i = 0; i < strChatAry.Length; i++)
        {
            ShowChatAdd(strChatAry[i]);
        }            
    }

    IEnumerator SetSRect()
    {
        yield return new WaitForEndOfFrame();
        if (svChatRecord != null)
        {
            svChatRecord.verticalNormalizedPosition = 1.0f;
        }
    }
    public void ClearChatRecord()
    {
        t_ChatRecord.text = "";
    }
}
