using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class VersionDetection : MonoBehaviour
{
    private const string editionPath = "Edition";

    string versionNumberUrl = "http://www.yuezhanhuashui.com/versionNumber.txt";
    [SerializeField]
    private string[] sArray;
    public GameObject gm_Mask;
    public Text t_Text;

    void Awake()
    {
        StartCoroutine(SetConfig());
    }

    public bool isNull = false;

    int waitFor = 0;
    int MaxWaitFor = 3;
    int configReturnValue;


    bool IsNewest(string strValue)
    {
        if ("Bate1.0" == strValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void LoadConfig(string versionNum, string url, string tis, string waits)
    {
        if (gm_Mask == null)
        {
            gm_Mask = UIManager.Instance.ShowUI(AllPrefabName.uiWin_Prompt_Edition, UIManager.Instance.canvas_T);
            t_Text = gm_Mask.GetComponentInChildren<Text>();
        }

        if (IsNewest(versionNum))
        {
            gm_Mask.gameObject.SetActive(false);
        }
        else
        {
            //版本号非最新。打开最新版本的下载地址 。启动一个禁止触发的页面
            gm_Mask.SetActive(true);
            t_Text.text = tis.ToString();
            InvokeRepeating("InvokeR_", 0.01f, 0.2f);
            StartCoroutine(wait(int.Parse(waits), url));
        }
    }

    int i_MaxTime = 60;
    void InvokeR_()
    {
        gm_Mask.transform.SetAsLastSibling();
        if (Time.time>=i_MaxTime)
        {
            CancelInvoke("InvokeR_");
        }
    }

    IEnumerator wait(int te, string url)
    {
        yield return new WaitForSeconds(te);
        Application.OpenURL(url);
    }

    /// <summary>
    /// 检查更新
    /// </summary>
    /// <returns></returns>
    IEnumerator SetConfig()
    {
        WWW www = new WWW(versionNumberUrl);
        yield return www;
        sArray = www.text.Split('|');
        //Tools.Tool.MJ公告 = sArray[5];
        if (sArray.Length>=6)
        {
            DataManage.Instance.StrSystemNotice = sArray[5];

            if (sArray.Length >= 7)
            {
                DataManage.Instance.StrSystemBuyDiaMsg = sArray[6];
            }
            //if (sArray.Length >= 8)
            //{
            //    Gm_Manager.G_M.systemGongGao_Str1 = sArray[7];
            //}
            LoadConfig(sArray[1], sArray[2], sArray[3], sArray[4]);
        }
    }

}
