using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>游戏入口，登录界面
/// </summary>
public class UiWin_Login : UiWin_Parent
{
    public GUIStyle btnlogin_GUIStyle;
    private GameObject sE_Login;
    InputField inputF_;
    private bool isRandomName = false;
    // Use this for initialization
    void Start()
    {
        isRandomName = false;
        GetComponentInChildren<Button>().interactable = !isRandomName;
        inputF_ = transform.GetComponentInChildren<InputField>();
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());

#if UNITY_STANDALONE_WIN
        transform.GetComponentInChildren<InputField>().gameObject.SetActive(true);
#else
        //transform.GetComponentInChildren<InputField>().gameObject.SetActive(false);
        //////if (inputF_ != null)
        //////{//移动平台关闭输入框
        //////    inputF_.gameObject.SetActive(false);
        //////}
        transform.GetComponentInChildren<InputField>().gameObject.SetActive(true);
#endif
    }
    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        base.Set_OnEventList<Component_>(all_);
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
    }

    private void OnClick_(GameObject btn_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        Debug.Log("Click Btn:" + btn_.name);
        if (btn_.gameObject.name.IndexOf("Btn_Login") >= 0)
        {
            OnClick_Login();
            
        }else if (btn_.gameObject.name.IndexOf("Label_YongHuXieYi") >= 0)
        {
            GameObject gmWin = UIManager.Instance.ShowUI(  AllPrefabName.uiWin_YongHuXieYi,UIManager.Instance.canvas_T);
            Button btnClose = gmWin.transform.GetComponentInChildren<Button>();
            btnClose.onClick.AddListener(delegate() { UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_YongHuXieYi); });
        }
        
    }
    void OnClick_Login()
    {

//////#if UNITY_STANDALONE_WIN
        inputF_ = transform.GetComponentInChildren<InputField>();
        PlayerPrefs.SetString(Pp_Name.NxHs_ACC.ToString(), inputF_.text);

        PublicEvent.GetINS.AppLogin(inputF_.text, inputF_.text);
        //UIManager.Instance.ShowUI(AllPrefabName.uiWin_Home, UIManager.Instance.canvas_T);
        Debug.Log("Login???InputF_:" + inputF_.text);
//////#else
//////        //transform.GetComponentInChildren<InputField>().gameObject.SetActive(false);
        
//////        AnySDKManager.SendLogin();
//////        //PublicEvent.GetINS.AppLogin(inputF_.text, inputF_.text);
//////#endif
//////        Debug.Log("Login???"+Application.platform);
    }
    

    void OnGUI()
    {
        if (isRandomName)
        {
            if (GUI.Button(new Rect((Screen.width / 2 - 128), Screen.height - 170, 256, 64), "随机账号登录（测试专用）")) // GUILayout.Button("随机账号登录（测试专用）"))
            {
                string[] strNames = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
                int i_NameRandom_2 = UnityEngine.Random.Range(1, 1000000);
                string strNameRandom = strNames[UnityEngine.Random.Range(0, strNames.Length)] + i_NameRandom_2.ToString();
                inputF_.text = strNameRandom;
                isRandomName = false;
                OnClick_Login();
            }
        }
    }

    /// <summary>当销毁
    /// </summary>
    void OnDestroy()
    {
        if (sE_Login!=null )
        {
            sE_Login.SetActive(false);
        }
    }

    /// <summary>当可用
    /// </summary>
    void OnEnable()
    {
        sE_Login = GameObject.Find("Camera_Login").transform.FindChild("LoginBg_0").gameObject;
        sE_Login.SetActive(true);
    }
}
