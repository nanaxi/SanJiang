using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_PromptOnly : MonoBehaviour
{

    [SerializeField]
    private Text tPrompt;

    [SerializeField]
    private GameObject tBg;

    [SerializeField]
    private PromptModel prmptModel_;

    private bool isShow;

    public bool IsShow
    {
        get
        {
            return isShow;
        }
    }

    void Start()
    {
        GetComponentInChildren<Button>().onClick.AddListener(delegate () { OnClick_BtnClose(); });
    }

    public void OpenPrompt(PromptModel prompt_)
    {
        prmptModel_ = prompt_;
        tPrompt.text = prompt_.promptMsg;
        //tBg.SetActive(true);
        isShow = true;
    }

    public void OnClick_BtnClose()
    {
        if (prmptModel_ != null && prmptModel_.confirmEvent != null)
        {
            prmptModel_.confirmEvent.Invoke();
        }
        isShow = false;
        transform.SetAsFirstSibling();
        tBg.SetActive(false);
    }

    void OnDisable()
    {
        isShow = false;
    }
}
