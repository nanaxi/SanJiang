using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_PromptRelink : MonoBehaviour {

    public Button btn_QueDing;
    public Button btn_QuXiao;
    
	//// Use this for initialization
	//void Start () {
                	
	//}

    public void OnClick_Btn_IsRelink(bool bl)
    {
        GameManager.GM.IsRelink = bl;
        GameManager.GM.Relink();
        btn_QueDing.interactable = false;
        btn_QuXiao.interactable = false;
        StartCoroutine(SetIndex());
    }

    IEnumerator SetIndex()
    {
        while (gameObject!=null && gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(8);
            btn_QueDing.interactable = true;
            btn_QuXiao.interactable = true;
            transform.SetAsLastSibling();
        }
        yield return null;
    }
}
