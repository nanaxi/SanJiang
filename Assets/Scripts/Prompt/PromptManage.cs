using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PromptManage : MonoBehaviour
{

    static private List<PromptModel> listPromptPool = new List<PromptModel>();
    static private List<PromptModelSelfClose> listSelfClosePrompt = new List<PromptModelSelfClose>();
    public UiWin_PromptOnly winPrompt;

    // Update is called once per frame
    void Update()
    {
        if (listPromptPool.Count > 0)
        {
            winPrompt = UIManager.Instance.ShowUI(AllPrefabName.uiWin_Prompt_Msg0, UIManager.Instance.canvas_T).GetComponent<UiWin_PromptOnly>();      
            if (winPrompt != null && winPrompt.IsShow == false)
            {
                PromptModel pMd = listPromptPool[0];
                listPromptPool.RemoveAt(0);
                winPrompt.OpenPrompt(pMd);
            }
        }
        else if (winPrompt != null && winPrompt.IsShow == true)
        {
            winPrompt.transform.SetAsLastSibling();
        }

        if (listSelfClosePrompt.Count > 0)
        {//如果有自动关闭的提示消息，就显示。
            UiWin_PromptSelfClose uiwinIns = UIManager.Instance.ShowUI(AllPrefabName.UiWin_PromptSelfClose, UIManager.Instance.canvas_T).GetComponent<UiWin_PromptSelfClose>();
            uiwinIns.OpenPrompt(listSelfClosePrompt[0]);
            listSelfClosePrompt.RemoveAt(0);
        }
    }

    static public void AddPrompt(PromptModel promptMd_)
    {
        for (int i = 0; i < listPromptPool.Count; i++)
        {
            if (listPromptPool[i].promptMsg == promptMd_.promptMsg)
            {
                return;
            }
        }

        listPromptPool.Add(promptMd_);
    }

    static public void AddPromptSelfClose(PromptModelSelfClose promptMd_)
    {
        for (int i = 0; i < listPromptPool.Count; i++)
        {
            if (listPromptPool[i].promptMsg == promptMd_.promptMsg)
            {
                return;
            }
        }
        listSelfClosePrompt.Add(promptMd_);
    }
}

public delegate void PromptEvent();
public class PromptModel
{
    public PromptEvent confirmEvent;
    public string promptMsg;
    public PromptModel(string strPromptMsg, PromptEvent event_ = null)
    {
        confirmEvent = event_;
        promptMsg = strPromptMsg;
    }
}

public delegate void PromptEventBool(bool isOK);
public class PromptModelBool
{
    public PromptEventBool confirmEvent;
    public string promptMsg;
    public PromptModelBool(string strPromptMsg, PromptEventBool event_ = null)
    {
        confirmEvent = event_;
        promptMsg = strPromptMsg;
    }
}

public class PromptModelSelfClose
{
    public float fCountDownTime;
    public string promptMsg;
    public PromptModelSelfClose(string strPromptMsg, float fTime = 3)
    {
        fCountDownTime = fTime;
        promptMsg = strPromptMsg;
    }
}