using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>分享界面
/// </summary>
public class UiWin_ShareUI : MonoBehaviour {

    [SerializeField]private Button btn_WxHy;
    [SerializeField]
    private Button btn_WxPyq;

    [SerializeField]
    private Button btn_CloseWin;

    // Use this for initialization
    void Start () {

        btn_WxHy.onClick.AddListener(delegate () { OnClick_Btn_WxHy(); });
        btn_WxPyq.onClick.AddListener(delegate () { OnClick_Btn_WxPyq(); });
        btn_CloseWin.onClick.AddListener(delegate () { OnClick_Btn_CloseWin(); });

    }

    void OnClick_Btn_WxHy()
    {
        Debug.Log("OnClick_Btn_WxHy()");
    }

    void OnClick_Btn_WxPyq()
    {
        Debug.Log("OnClick_Btn_WxPyq()");
    }

    void OnClick_Btn_CloseWin()
    {
        Debug.Log("OnClick_Btn_CloseWin()");
        UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_ShareUI);
    }
}
