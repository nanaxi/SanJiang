using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>小窗口—》聊天，挂载在聊天界面
/// </summary>
public class UiWinX_Chat : MonoBehaviour {

    public UnityEngine.Events.UnityAction onClickChat;
    [SerializeField]private ScrollRect sVRect_Chat;//拖拽赋值，表情和常用语滑块

    private bool isInit = false;
    // Use this for initialization
    void Start()
    {
        Init_ShowUi();
    }

    public void Init_ShowUi()
    {
        if (isInit)
        {
            return;
        }
        //Button[] chatAry = sVRect_Chat.content.GetComponentsInChildren<Button>();
        for (int i = 0; i < sVRect_Chat.content.childCount; i++)
        {/*先销毁再生成*/
            if (sVRect_Chat.content.GetChild(i)!=null)
            {
                Destroy(sVRect_Chat.content.GetChild(i).gameObject);
            }
        }

        List<string[]> strChat = DataManage.Instance.Chat_GetConfig();
        for (int i = 0; i < strChat.Count; i++)
        {//初始化实例化常用语
            Button gmChat = UIManager.Instance.ShowPerfabUI(AllPrefabName.Btn_ChangYongYu_, sVRect_Chat.content).GetComponentInChildren<Button>();
            gmChat.gameObject.name = strChat[i][0];
            gmChat.GetComponentInChildren<Text>().text = strChat[i][1];
            gmChat.onClick.AddListener(delegate ()
            {
                Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                OnClick_Btn_Chat_(gmChat.gameObject.name);
            });
        }

        isInit = true;
    }

    void OnClick_Btn_Chat_(string btnName)
    {
        Debug.Log(btnName + "你想要发送常用语？" + DataManage.Instance.Chat_GetChat(btnName));
        PublicEvent.GetINS.SentMegssageText(DataManage.Instance.Chat_GetChat(btnName));
        if (onClickChat != null)
        {
            onClickChat.Invoke();
        }
    }
    
}
