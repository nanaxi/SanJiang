using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class GameManager : MonoBehaviour {

    public string JsonPath
    {
        get
        {
            string path = null;
            if (Application.platform == RuntimePlatform.IPhonePlayer)//判断平台
            {
                //path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);//ios 平台 就会获取documents路径
                //path = path.Substring(0, path.LastIndexOf('/')) + "/Documents/";

                path = Application.persistentDataPath;//安卓平台
            }
            else
            {
                path = Application.persistentDataPath;//安卓平台
            }
            return path;
        }
    }

    [SerializeField]private GameSceneType _gmType;

    public Transform Canvas;
    public static GameManager GM;
    
    public const int playerCount = 4;
    private bool isRelink = false;
    public bool IsRelink
    {
        set { isRelink = value; }
        get { return isRelink; }
    }

    private bool isRelink_Count = false;
    public bool IsRelink_C
    {
        set { isRelink = value; }
        get { return isRelink_Count; }
    }
    public float fpsMeasuringDelta = 2.0f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;
    
    public GameSceneType GmType
    {
        get
        {
            return _gmType;
        }

        set
        {
            _gmType = value;
        }
    }

    // Use this for initialization
    void Awake () {
        //this.Canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
        GM = this;
        Application.targetFrameRate = 45;
        if (PlayerPrefs.HasKey(Pp_Name.NxHs_IsRelink.ToString())==false)
        {
            PlayerPrefs.SetInt(Pp_Name.NxHs_IsRelink.ToString(),120);
        }
    }
    private void Start()
    {
        timePassed = 0.0f;

        Model3dManage.Instance.Init();
        NetM_Home.Instance.Init();
        GameNetWork.Inst().Init();
        LoginProcessor.Inst().Init();
        UIManager.Instance.Init();
        UIManager.Instance.ShowUI(AllPrefabName.uiWin_InGame_MJ, UIManager.Instance.canvas_T);
        //UIManager.Instance.ShowUI(AllPrefabName.UiWin_PhizChat, UIManager.Instance.canvas_T);

        //UIManager.Instance.ShowUI(AllPrefabName.uiWin_Home, UIManager.Instance.canvas_T);
        GmType = GameSceneType.gm_Login;
        UIManager.Instance.ShowUI(AllPrefabName.uiWin_Login, UIManager.Instance.canvas_T);

        GiftConfig.Instance.Init();
    }

    
    void AddScene(Transform trs)
    {
        //trs.SetParent(Canvas,false);
        //trs.SetAsLastSibling();
    }

    private void Update()
    {
        LoginProcessor.Inst().Update();
        GameNetWork.Inst().Update();

        //m_FrameCount = m_FrameCount + 1;
        //timePassed = timePassed + Time.deltaTime;

        //if (timePassed > fpsMeasuringDelta)
        //{
        //    m_FPS = m_FrameCount / timePassed;

        //    timePassed = 0.0f;
        //    m_FrameCount = 0;
        //}
    }

    public void Game_RetrueHome()
    {
        MemoryPool_3D.Instance.MJ3D_RecycleALL();
        Model3dManage.Instance.DestroyObjectModel("MJGameController");
        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_InGame_MJ);
        //UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_MjEnd_1);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_QuanJuJieSuan);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_TouPiaoJieGuo);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_PhizChat);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Setting);

        /*——13水添加——*/
        UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_InGame_13Z);
        /*————*/
        GmType = GameSceneType.gm_Home;
        UIManager.Instance.ShowUI(AllPrefabName.uiWin_Home,UIManager.Instance.canvas_T);
        DataManage.Instance.MyPlayer_Data.isgaming = false;
        DataManage.Instance.PData_RemoveOtherPlayerData();
        DataManage.Instance.Head_DeleteOtherPlayerSprite();

        IF_Assets();
    }


    //void OnGUI()
    //{
    //    GUIStyle bb = new GUIStyle();
    //    bb.normal.background = null;    //这是设置背景填充的
    //    bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
    //    bb.fontSize = 32;       //当然，这是字体大小

    //    //居中显示FPS
    //    GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + m_FPS, bb);
    //}

    public void IF_Assets()
    {
        StartCoroutine("AsyncIF_Assets");
    }
    IEnumerator AsyncIF_Assets()
    {
        yield return Resources.UnloadUnusedAssets();//
        //yield return null;
    }

    public void Relink()
    {
        
        UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_MjEnd_1);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_QuanJuJieSuan);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_TouPiaoJieGuo);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_QuitRoom);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.UiWin_PhizChat);
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Setting);
        GmType = GameSceneType.gm_Home;
        DataManage.Instance.MyPlayer_Data.isgaming = false;
        DataManage.Instance.PData_RemoveOtherPlayerData();
        LoginProcessor.Inst().ApplyLogout();//告诉服务器退出了
        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_Home);
        //
        if (IsRelink)
        {//是否重连
            Login_Relink();
        }
        else
        {
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Prompt_Relink);
            UIManager.Instance.ShowUI(AllPrefabName.uiWin_Login, UIManager.Instance.canvas_T).GetComponent<UiWin_Login>();
        }
        isRelink_Count = true;
    }

    public void Login_Relink()
    {
        Debug.Log("Relilnk:");
#if UNITY_STANDALONE_WIN
        PublicEvent.GetINS.AppLogin(PlayerPrefs.GetString(Pp_Name.NxHs_ACC.ToString()), PlayerPrefs.GetString(Pp_Name.NxHs_ACC.ToString()));
        
#else   
        LoginProcessor.Inst().Login();
#endif
    }

    public void Relink_InitDelete()
    {
        MemoryPool_3D.Instance.MJ3D_RecycleALL();
        Model3dManage.Instance.DestroyObjectModel("MJGameController");
        UIManager.Instance.SetUIobject(false, AllPrefabName.uiWin_InGame_MJ);
        isRelink_Count = false;
    }

    void OnApplicationQuit()
    {
        //Screen.sleepTimeout = i_SleepTimeout;
        LoginProcessor.Inst().UnInit();
        GameNetWork.Inst().UnInit();
        Debug.Log("退出程序，将会把消息发送给服务器端？？？？？？？");
    }

}

public struct Ins_GmObj
{
    public GameObject ui_Res;
    public GameObject ui_Ins;
    public Ins_GmObj(GameObject gmRes,GameObject gmIns)
    {
        ui_Res = gmRes;
        ui_Ins = gmIns;
    }
}