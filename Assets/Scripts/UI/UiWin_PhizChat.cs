using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class UiWin_PhizChat : UiWin_Parent
{
    private enum Btn_AllName
    {
        Btn_ChangYongYu_, Btn_Phiz_, Btn_SendMsg, Btn_SelectOptions_, Btn_CloseWindows
    }
    
    [SerializeField]private Need_Scene_Component need_S_Component;
    public ScrollRect sVRect_Phiz, sVRect_Chat;//拖拽赋值，表情和常用语滑块
    public InputField inputF_Chat;//拖拽赋值，自定义聊天输入框，

    public RectTransform t_Bg0;
    [SerializeField]
    private bool isOpenChatBg;
    float openSpeed = 108;
    // Use this for initialization
    void Start()
    {
        
        Debug.Log(GetComponent<RectTransform>().sizeDelta+"SoWhat???"+GetComponent<RectTransform>().anchoredPosition);
        //t_Bg0 = transform.FindChild("Bg0").GetComponent<RectTransform>();
        //openSpeed = t_Bg0.sizeDelta.y * 0.1f;
        isOpenChatBg = true;
        this.Set_OnEventList<Button>(GetComponentsInChildren<Button>());
        need_S_Component = new Need_Scene_Component(transform);
        //OnClick_Btn_SelectOptions_(0);
        Init_();

        StartCoroutine(Wait_Init());
        
    }

    void OnEnable()
    {
        Invoke("SetSB_Index",0.1f);
    }

    void SetSB_Index()
    {
        transform.SetAsLastSibling();
    }

    /// <summary>等待初始化
    /// </summary>
    IEnumerator Wait_Init()
    {
        transform.SetAsFirstSibling();
        //OpenOrClose_UiWin();
        while (isOpenChatBg)
        {
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
        yield return null;
    }

    void Init_()
    {
        string[] strPhiz = DataManage.Instance.Phiz_GetConfig();
        for (int i = 0; i < strPhiz.Length; i++)
        {///初始化实例化表情
            Button gmPhiz = UIManager.Instance.ShowPerfabUI( AllPrefabName.BtnPhiz, sVRect_Phiz.content).GetComponent<Button>();
            gmPhiz.gameObject.name = strPhiz[i];
            gmPhiz.image.sprite = Resources.Load<Sprite>(ResPath_Assets.sprite_Phiz+ strPhiz[i]);
            gmPhiz.onClick.AddListener(delegate() {
                Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                OnClick_Btn_Phiz_(gmPhiz.gameObject.name);
            });
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
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                btn_.onClick.AddListener(delegate () {
                    OnClick_(btn_.gameObject);
                });
            }
        }
        //base.Set_OnEventList<Component_>(all_);
    }
    public void OpenOrClose_UiWin(bool isActive = false)
    {
        gameObject.SetActive(isActive);
        transform.SetAsFirstSibling();
    }

    int i_direction = 0;
    
    IEnumerator OpenOrClose_MoveAnima()
    {
        Vector2 move_V2 = isOpenChatBg ? new Vector2(0, t_Bg0.sizeDelta.y) :Vector2.zero;

        while (Vector2.Distance(t_Bg0.anchoredPosition,move_V2)>10)
        {
            t_Bg0.anchoredPosition = Vector2.MoveTowards(t_Bg0.anchoredPosition,move_V2, openSpeed);
            yield return new WaitForEndOfFrame();
        }
        isOpenChatBg = !isOpenChatBg;
        yield return null;
    }

    #region/*—————————按钮事件区域—————————————*/
    void OnClick_(GameObject btn_Gm)
    {
        Debug.Log("OnClick Gm :" + btn_Gm.name);
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        if (btn_Gm.name.IndexOf("Btn_CloseWin") >= 0)
        {
            OpenOrClose_UiWin();
        }
        else if (btn_Gm.name.IndexOf(Btn_AllName.Btn_ChangYongYu_.ToString()) >= 0)
        {
            OnClick_Btn_ChangYongYu_(btn_Gm);
        }
        else if (btn_Gm.name.IndexOf(Btn_AllName.Btn_SelectOptions_.ToString()) >= 0)
        {
            //OnClick_Btn_SelectOptions_(btn_Gm.transform.GetSiblingIndex());
        }
        else if (btn_Gm.name.IndexOf(Btn_AllName.Btn_SendMsg.ToString()) >= 0)
        {
            
            if (inputF_Chat.text.Length>0)
            {
                Debug.Log("你想发送？？？" + inputF_Chat.text);
                //Add_ChatInfo_JL("测试发送ID", need_S_Component.inputF_ChatValue.text);
                PublicEvent.GetINS.SentMegssageText(inputF_Chat.text);
                inputF_Chat.text = "";

                OpenOrClose_UiWin();
            }
            OpenOrClose_UiWin();
        }
    }

    void OnClick_Btn_SelectOptions_(int i_Index)
    {
        if (i_Index<0 ||i_Index>=need_S_Component.chat_Op_Bg_00.childCount)
        {
            Debug.LogError( "错误的值"+i_Index);
            return;
        }

        for (int i = 0; i < need_S_Component.chat_Op_Bg_00.childCount; i++)
        {
            need_S_Component.chat_Op_Bg_00.GetChild(i).gameObject.SetActive(i == i_Index);
        }
        if (i_Index==2)
        {
            StartCoroutine(Set_SRect_V());
        }
    }

    void OnClick_Btn_Phiz_(string btnName)
    {
        Debug.Log(btnName + "你想要播放表情？" +DataManage.Instance.Phiz_GetPath(btnName));
        PublicEvent.GetINS.SentMegssageImage(btnName);
        OpenOrClose_UiWin();
    }

    void OnClick_Btn_Chat_(string btnName)
    {
        Debug.Log(btnName + "你想要发送常用语？" + DataManage.Instance.Chat_GetChat(btnName));
        PublicEvent.GetINS.SentMegssageText(DataManage.Instance.Chat_GetChat(btnName));
        OpenOrClose_UiWin();
    }

    void OnClick_Btn_ChangYongYu_(GameObject btn_Gm)
    {
        Debug.Log("你想要发送常用语？");
    }
    #endregion

    /// <summary>添加聊天记录
    /// </summary>

    #region/*———聊天记录显示———*/
    public void Add_ChatInfo_JL(string p_Name , string p_Chat)
    {
        //string str_Chat = p_Name + ": " + p_Chat+"\n";
        Text t_ChatShow = Show_Chat();
        t_ChatShow.transform.parent.gameObject.SetActive(true);
        t_ChatShow.text = "<color=red>" + p_Name + "：</color>\n" + p_Chat;
        StartCoroutine(Set_SRect_V());
    }
    
    Text Show_Chat()
    {
        for (int i = 0; i < need_S_Component.t_ShowChatInfo.Length; i++)
        {
            if (need_S_Component.t_ShowChatInfo[i].text.Length<2)
            {
                return need_S_Component.t_ShowChatInfo[i];
            }
        }
        //如果消息达到16条上限，则初始化一遍
        need_S_Component.Init_CloseChatShow();
        return Show_Chat();
    }

    IEnumerator Set_SRect_V()
    {
        yield return new  WaitForEndOfFrame();
        need_S_Component.chatShowSRect_Bg.verticalNormalizedPosition = 0;
        yield return null;
    }
    #endregion

    [System.Serializable]
    class Need_Scene_Component
    {
        public Transform chat_Op_Bg_00;
        public Text[] t_ShowChatInfo;
        public ScrollRect chatShowSRect_Bg;
        public InputField inputF_ChatValue;
        //public string SetInputF_AddChat
        //{
        //    set {
        //        if (t_ShowChatInfo.text.Length>=512)
        //        {
        //            t_ShowChatInfo.text = "";
        //        }
        //        t_ShowChatInfo.text += value;
        //    }
        //}
        public Need_Scene_Component(Transform window_JFR)
        {
            //chat_Op_Bg_00 = GameObject.Find("Chat_Op_Bg_00").transform;
            //chat_Op_Bg_00.GetChild(1).gameObject.SetActive(false);
            //chat_Op_Bg_00.GetChild(2).gameObject.SetActive(false);
            ////t_ShowChatInfo = chat_Op_Bg_00.GetChild(2).transform.GetComponentInChildren<Text>();

            //chatShowSRect_Bg = chat_Op_Bg_00.GetChild(2).transform.GetComponentInChildren<ScrollRect>();
            //t_ShowChatInfo = chatShowSRect_Bg.content.GetComponentsInChildren<Text>();
            
            //inputF_ChatValue = chat_Op_Bg_00.GetChild(2).transform.GetComponentInChildren<InputField>();

            //Init_CloseChatShow();
        }

        public void Init_CloseChatShow()
        {
            for (int i = 0; i < t_ShowChatInfo.Length; i++)
            {
                t_ShowChatInfo[i].text = "";
                t_ShowChatInfo[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

}
