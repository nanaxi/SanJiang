using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class UiWin_Prompt : MonoBehaviour {

    static public void OpenPrompt(string str_)
    {
        PromptManage.AddPrompt(new PromptModel(str_,delegate() { }));
        //UiWin_Prompt[] all_Prompt = Component.FindObjectsOfType<UiWin_Prompt>();
        //for (int i = 0; i < all_Prompt.Length; i++)
        //{
        //    if (all_Prompt[i].GetComponentInChildren<Text>().text.IndexOf(str_) >= 0 &&
        //        str_.Length == all_Prompt[i].GetComponentInChildren<Text>().text.Length)
        //    {
        //        Debug.Log("已存在该类型的提示窗口");
        //        return;
        //    }
        //}
        //GameObject ui_Ins = UIManager.Instance.ShowPerfabUI(AllPrefabName.uiWin_Prompt_Msg0,UIManager.Instance.canvas_T);

        //ui_Ins.transform.SetParent(Component.FindObjectOfType<Canvas>().transform);
        //UiWin_Prompt promptView = ui_Ins.GetComponent<UiWin_Prompt>();
        //promptView.GetComponentInChildren<Text>().text = str_;
        //promptView.GetComponentInChildren<Button>().onClick.AddListener(delegate() {
        //    promptView.OnClick_Btn_Close_Prompt();
        //});
        //promptView.IF_Index(promptView.transform);
    }

    public void IF_Index(Transform msgGmObj)
    {
        StartCoroutine(InvokeR_Event(0.5f, delegate
        {
            if (msgGmObj.GetSiblingIndex() != (msgGmObj.parent.childCount - 1))
            {
                msgGmObj.SetAsLastSibling();
                //Debug.Log("看看还在执行吗？" + msgGmObj.GetSiblingIndex());
            }

        }, msgGmObj.gameObject));
    }

    public void OnClick_Btn_Close_Prompt()
    {
        Destroy(gameObject);//
    }

    public IEnumerator InvokeR_Event(float time_, UnityAction stay_Event, GameObject gmClose_InvekeStop)
    {
        while (gmClose_InvekeStop != null && gmClose_InvekeStop.activeInHierarchy)
        {
            stay_Event.Invoke();
            yield return new WaitForSeconds(time_);
        }
        yield return null;
    }
}
