using UnityEngine;
using System.Collections;

public class UiWinL_InGameBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void OnClick_Btn_OpenSetting()
    {
        Debug.Log("打开设置");
        UIManager.Instance.ShowUI(AllPrefabName.uiWin_Setting, UIManager.Instance.canvas_T);
    }

    public void OnClick_Btn_OpenChatPhiz()
    {
        Debug.Log("打开聊天");
        UIManager.Instance.ShowUI(AllPrefabName.UiWin_PhizChat, UIManager.Instance.canvas_T);
    }

    /// <summary>【拖拽赋值】录音按钮按下，
    /// </summary>
    public void OnClickDown_MKF()
    {
        Debug.Log("录音Start");
        gmMkfStyle = UIManager.Instance.ShowUI(AllPrefabName.Img_MkfStyle, transform.parent);
        //gmMkfStyle.SetActive(true);
        if (Application.platform!= RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            return;
        }
        Event.Inst().StartMic();
        
    }
    GameObject gmMkfStyle;
    /// <summary>【拖拽赋值】录音按钮抬起，
    /// </summary>
    public void OnClickUp_MKF()
    {
        Debug.Log("录音END");
        gmMkfStyle.SetActive(false);
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
            return;
        }
        MicphoneTest.IsCancelMic = false;
        Event.Inst().StartMic();
        
    }
}
