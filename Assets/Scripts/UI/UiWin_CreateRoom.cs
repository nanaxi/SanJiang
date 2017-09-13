using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UiWin_CreateRoom : UiWin_Parent {
    

    [SerializeField]private Transform img_CreateViewBg;

    [SerializeField]
    private Button btn_GM_Option1;
    [SerializeField]
    private Button btn_GM_Option2;
    [SerializeField]
    private Button btn_GM_Option3;

    [SerializeField]
    private Button btn_CloseWin;

    [SerializeField]
    private PPData ppData_XWMJ = new PPData();

    

    [SerializeField]
    private CreateRoomXWMJ createXWMJ = new CreateRoomXWMJ();

    [SerializeField]
    private CreateRoomXWD2 createDa2 = new CreateRoomXWD2();

    [SerializeField]
    private CreateRoom13Shui create13S = new CreateRoom13Shui();
    // Use this for initialization
    void Awake() {

        createXWMJ.Init_Var();
        create13S.Init_Var();
        createDa2.Init_Var();

        this.Init();
    }

    public override void Init()
    {

        btn_GM_Option1.onClick.AddListener(delegate () { OnClick_Btn_GM_Option1(btn_GM_Option1.transform.GetSiblingIndex()); });
        btn_GM_Option2.onClick.AddListener(delegate () { OnClick_Btn_GM_Option1(btn_GM_Option2.transform.GetSiblingIndex()); });
        btn_GM_Option3.onClick.AddListener(delegate () { OnClick_Btn_GM_Option1(btn_GM_Option3.transform.GetSiblingIndex()); });

        btn_CloseWin.onClick.AddListener(delegate () { this.OnClick_btn_CloseWin(); });
    }

    void PPData_Init()
    {
        if (!PlayerPrefs.HasKey(PPName_CreateRoom.SanJ_CR_XWMJ.ToString())
            || !PlayerPrefs.HasKey(PPName_CreateRoom.SanJ_CR_XWD2.ToString())
            || !PlayerPrefs.HasKey(PPName_CreateRoom.SanJ_CR_13S.ToString())
            )
        {
            PPData_Set();
        }

        createXWMJ.PlayerPrefs_Get();
        create13S.PlayerPrefs_Get();
        createDa2.PlayerPrefs_Get();

    }

    /// <summary>设置创建房间的缓存
    /// </summary>
    void PPData_Set()
    {

        createXWMJ.PlayerPrefs_Set();
        create13S.PlayerPrefs_Set();
        
    }

    void OnEnable()
    {
        Debug.Log("执行先后AAAAAAAA1");
        PPData_Init();
        OnClick_Btn_GM_Option1(2);
    }
    void OnDisable()
    {
        this.PPData_Set();
    }

    /// <summary>选择某一种创建房间的界面
    /// </summary>
    void OnClick_Btn_GM_Option1(int iIndex)
    {
        if (iIndex < 0 || iIndex >= img_CreateViewBg.childCount)
        {
            Debug.Log("<color=red>ERROR Range</color>");
            return;
        }
        for (int i = 0; i < img_CreateViewBg.childCount; i++)
        {
            img_CreateViewBg.GetChild(i).gameObject.SetActive(i == iIndex);
        }
    }

    /// <summary>按钮事件——局数加减
    /// </summary>
    void OnClick_Btn_ChangeJuShu(bool b_)
    {
        
        
    }
    /// <summary>按钮事件——关闭窗口
    /// </summary>
    void OnClick_btn_CloseWin()
    {
        Debug.Log("OnClick_btn_CloseWin");
        UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_CreateRoom);
    }


    /// <summary>按钮事件——创建兴文大贰
    /// </summary>
    void OnClick_Btn_CreateRoom_XWD2()
    {
        
    }

    /// <summary>当改变局数,给InputField 拖拽赋值
    /// </summary>
    public void OnChangeInpuF_JuShu(InputField inputF)
    {
        int iValue = int.Parse(inputF.text.Trim());
        iValue = iValue * 3;
        inputF.transform.FindChild("T_RC_Count").GetComponent<Text>().text = "局\tX" + iValue + "房卡";
        //createXWMJ.btn_CreateRoom_XWMJ.GetComponentInChildren<Text>().text = "创建 （房卡X" + iValue + "";
        //createDa2.btn_CreateRoom_Da2.GetComponentInChildren<Text>().text = "创建 （房卡X" + iValue + "";
        Debug.Log("房卡数："+iValue);
    }
}

[System.Serializable]
public class IntAry
{
    private int[] intAry_C;
    /// <summary>数组
    /// </summary>
    public int[] IntAry_C
    {
        get
        {
            return intAry_C;
        }

        set
        {
            intAry_C = value;
        }
    }
    
    /// <summary>当前数组下标
    /// </summary>
    public int IIndex
    {
        get{ return iIndex;}
        set{ iIndex = value;}
    }

    private int iIndex;

    /// <summary>当前数组下标 所代表的值  数组[下标]
    /// </summary>
    [SerializeField]
    private int iResult;
    public IntAry()
    {
    }
    public IntAry(int[] iAry)
    {
        IIndex = 0;
        IntAry_C = iAry;
    }

    /// <summary>增加或减少下标
    /// </summary>
    public int ChangeIndex(bool isADD)
    {
        Debug.Log("Change JuShu?"+isADD);
        IIndex += isADD ? 1 : -1;
        if (IIndex>=IntAry_C.Length)
        {
            IIndex = 0;
        }else if (IIndex < 0)
        {
            IIndex = IntAry_C.Length-1;
        }
        iResult = IntAry_C[IIndex];
        return IntAry_C[IIndex];
    }

    public int GetIndexValue()
    {
        if (IntAry_C.Length > IIndex && IIndex >= 0)
        {
            iResult = IntAry_C[IIndex];
            return IntAry_C[IIndex];
        }
        return 0; 
    }
}



[System.Serializable]
public class CreateJuShu
{
    public InputField inputF_JuShu;
    public Button btn_ChangeJuShuReduce;
    public Button btn_ChangeJuShuADD;
}

public enum PPName_CreateRoom
{
    SanJ_CR_XWMJ, SanJ_CR_XWD2 , SanJ_CR_13S
}

/// <summary>作用于缓存玩家偏好
/// </summary>
[System.Serializable]
public class PPData
{
    public PPName_CreateRoom ppKey;
    //[SerializeField]private string ppValue;
    [SerializeField]private string[] ppAry;
    /// <summary>分隔符
    /// </summary>
    private const string splitStr = "Split";
    public PPData()
    { }

    /// <summary>初始化，读取数据
    /// </summary>
    public void PPData_Init()
    {
        string ppValue = PlayerPrefs.GetString(ppKey.ToString());
        ppAry = ppValue.Split(new string[] {splitStr},System.StringSplitOptions.RemoveEmptyEntries);
    }
    public string PPData_GetValue(string str)
    {
        for (int i = 0; i < ppAry.Length; i++)
        {
            if (ppAry[i].IndexOf(str)>=0)
            {
                return ppAry[i];
            }
        }
        return "";
    }

    public void PPDate_SetPP(string[] strValue)
    {
        string strValueSSS = "";
        for (int i = 0; i < strValue.Length; i++)
        {
            strValueSSS += strValue[i] + splitStr;
        }
        PlayerPrefs.SetString(ppKey.ToString(), strValueSSS);
    }

}

/// <summary>创建房间分类的父类
/// </summary>
public abstract class CreateRoomParent : MonoBehaviour
{
    public abstract void Init_Var();
    public abstract void OnClick_Btn_CreateRoom();
    public abstract void PlayerPrefs_Set();// { }
    public abstract void PlayerPrefs_Get();// { }

    public virtual void OnClick_BtnChange_JuShu(bool isAdd) { }
    public virtual void OnChangeInpuF_JuShu(InputField inputF) { }
}