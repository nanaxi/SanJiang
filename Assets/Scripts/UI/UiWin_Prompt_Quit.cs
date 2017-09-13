using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_Prompt_Quit :UiWin_Parent{
    
    
	// Use this for initialization
	void Start () {
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                UnityEngine.Events.UnityAction btnOnClck_ = null;
                switch (btn_.gameObject.name)
                {
                    case "Btn_Cancel":
                        btnOnClck_ = delegate () {

                        };
                        break;
                    case "Btn_Confirm":
                        btnOnClck_ = delegate () {
                            btn_.interactable = false;
                            Quit_F();
                        };
                        break;
                    case "Btn_CloseWin":
                        btnOnClck_ = delegate () {
                            //Destroy(gameObject);
                            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitGame);
                        };
                        break;
                    default:
                        break;
                }
                if (btnOnClck_!=null)
                {
                    btn_.onClick.AddListener(delegate ()
                    {
                        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                        Debug.Log("Click Btn:" + btn_.gameObject.name);
                        btnOnClck_.Invoke();
                    });
                }
            }
        }
    }

    public void Quit_F()
    {
        StartCoroutine(SendQuitGame());

        //#if UNITY_EDITOR
        //        UnityEditor.EditorApplication.isPlaying = false;
        //#else
        //		Application.Quit();
        //#endif

    }


    /// <summary>切换微信
    /// </summary>
    IEnumerator SendQuitGame()
    {
        AnySDKManager.SendLogout();//微信登出
        LoginProcessor.Inst().ApplyLogout();//告诉服务器退出了
        yield return new WaitForSeconds(1);
        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_Home);
        UIManager.Instance.ShowUI( AllPrefabName.uiWin_Login ,UIManager.Instance.canvas_T);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitGame);
        //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        yield return null;
    }
}
