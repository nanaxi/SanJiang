using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum AllPrefabName
{
    /// <summary>版本提示界面， 使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_Prompt_Edition,

    /// <summary>用户协议界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_YongHuXieYi,

    uiWin_Login,
    /// <summary>游戏大厅，使用SetUIobject(False)关闭
    /// </summary>
    uiWin_Home,
    /// <summary>设置界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_Setting,
    /// <summary>查看玩家信息界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_PlayerInfo,
    /// <summary>充值提示界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_BuyDiamondPrompt,
    /// <summary>创建房间界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_CreateRoom,
    /// <summary>加入房间界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_JoinRoom,
    /// <summary>游戏公告界面，待定
    /// </summary>
    uiWin_GameGongGao,
    /// <summary>麻将游戏中界面，使用SetUIobject(False)关闭
    /// </summary>
    uiWin_InGame_MJ,
    /// <summary>麻将结束界面，使用DestroyObjectUI()关闭
    /// </summary>
    UiWin_MjEnd_1,
    /// <summary>麻将表情界面，使用DestroyObjectUI()关闭
    /// </summary>
    UiWin_PhizChat,
    /// <summary>麻将全局结算界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_QuanJuJieSuan,
    /// <summary>退出游戏提示界面，使用DestroyObjectUI(False)关闭
    /// </summary>
    uiWin_Prompt_QuitGame,
    /// <summary>退出房间提示，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_Prompt_QuitRoom,
    /// <summary>消息提示界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_Prompt_Msg0,

    /// <summary>消息提示界面ShowUI， 倒计时自动关闭
    /// </summary>
    UiWin_PromptSelfClose,

    /// <summary>投票结果界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_TouPiaoJieGuo,
    /// <summary>战绩界面，使用DestroyObjectUI()关闭
    /// </summary>
    uiWin_ZhanJi,

    UiWin_ZJHF,

    /// <summary>13水游戏界面
    /// </summary>
    UiWin_InGame_13Z,

    /// <summary>重新连接提示面板
    /// </summary>
    uiWin_Prompt_Relink,

    /// <summary>同一IP提示界面， 使用Destroy()关闭
    /// </summary>
    UiWin_PromptIp,

    /// <summary>分享界面，使用Destroy()关闭
    /// </summary>
    UiWin_ShareUI,

    BtnPhiz,
    Btn_ChangYongYu_,
    Img_MkfStyle,
    uiWin_DzpPlayRoom,
    uiWin_DzpGameOver,
    uiWin_DzpGameEnd,
}

public class UIManager
{
    #region Instance Define
    static UIManager inst = null;
    static public UIManager Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new UIManager();
            }
            return inst;
        }
    }
    #endregion

    private Dictionary<AllPrefabName, GameObject> dicUI = new Dictionary<AllPrefabName, GameObject>();
    private UIManager()
    {

    }
    #region 2dUICamera
    private GameObject uiCameraObj = null;
    public GameObject UICameraObj
    {
        get
        {
            return uiCameraObj;
        }
    }
    #endregion
    public Dictionary<AllPrefabName, string> dicPrefabPath = new Dictionary<AllPrefabName, string>();
    public Transform canvas_T;

    void Update()
    {
        
    }
	
    public void Init()
    {
        canvas_T = GameObject.Find("Canvas").transform;
    }
    #region /*———关于UI的1、生成。2、销毁。3、隐藏。4、获取———*/
    public string GetPath(AllPrefabName _uiName)
    {
        if (dicPrefabPath.ContainsKey(_uiName))
        {
            return dicPrefabPath[_uiName];
        }
        Debug.LogError("没有找到你想要的UI路径");
        return "";
    }
	
	public GameObject ShowUI(AllPrefabName _uiName,Transform parent_)
    {
        if (dicUI.ContainsKey(_uiName))
        {
            if (dicUI[_uiName] != null)
            {
                dicUI[_uiName].transform.SetAsLastSibling();
                SetUIobject(true, _uiName);
                return dicUI[_uiName];
            }
            else
            {
                dicUI.Remove(_uiName);
            }
        }
        string path = "Prefabs/UI/" + _uiName;
        GameObject _prefab = Resources.Load(path) as GameObject;
		
        if (_prefab == null)
        {
            Debug.LogError(path);
            return null;
        }
        GameObject ui = GameObject.Instantiate(_prefab) as GameObject;
        //Debug.Log(""+parent_==null);
        ui.transform.SetParent(parent_);// GetAuchor(_anchor);
        RectT_S.Set_PointAndSize_Px(ui.transform, _prefab.transform);
        dicUI.Add(_uiName, ui);
		
        return ui;
    }


	string FrontUIName = "";
	GameObject prefab = null;
	public GameObject ShowPerfabUI(AllPrefabName _uiName,Transform parent)
    {

        string path = "Prefabs/UI/" + _uiName;
        prefab = null;
        prefab = Resources.Load(path) as GameObject;

        if (prefab == null)
        {
            Debug.LogError("Prefabs/UI/" + _uiName);
            return null;
        }
        GameObject ui = GameObject.Instantiate(prefab) as GameObject;
		ui.transform.SetParent(parent);
        RectT_S.Set_PointAndSize_Px(ui.transform, prefab.transform);

        return ui;
    }
	public void DestroyObjectUI(AllPrefabName _uiName ,float _waitTime=0f)
	{
		 if (dicUI.ContainsKey(_uiName) == true)
         {
            //BaseUI_C ui = BaseUI_C.GetBaseUI_C(dicUI[_uiName]);
            //ui.Destroy();
            if (dicUI[_uiName]!=null)
            {
                UnityEngine.Object.Destroy(dicUI[_uiName], _waitTime);
            }
            dicUI.Remove(_uiName);
        }
	}

	public GameObject FindUI(AllPrefabName _uiName)
	{
        if (dicUI.ContainsKey(_uiName) == false)
        {
            return null;
        }
        return dicUI[_uiName];// BaseUI_C.GetBaseUI_C(dicUI[_uiName]);
	}
	
	public void SetUIobject(bool IsActive, AllPrefabName name)
	{
		if(!dicUI.ContainsKey(name))
		{
            Debug.Log(name+"<color=red>SetUIobject == Null 001</color>");
			return;
		}
        if (dicUI[name] != null)
        {
            dicUI[name].SetActive(IsActive);
        }
        else
        {
            Debug.Log(name + "<color=red>SetUIobject == Null 002</color>");
        }
	}

    public GameObject Open_UiObject(AllPrefabName name_)
    {
        GameObject gmValue = null;
        switch (name_)
        {
            case AllPrefabName.uiWin_Login:
                break;
            case AllPrefabName.uiWin_Home:
                gmValue = ShowUI(name_, canvas_T.transform);
                break;
            case AllPrefabName.uiWin_Setting:
                break;
            case AllPrefabName.uiWin_PlayerInfo:
                break;
            case AllPrefabName.uiWin_BuyDiamondPrompt:
                break;
            case AllPrefabName.uiWin_CreateRoom:
                break;
            case AllPrefabName.uiWin_JoinRoom:
                break;
            case AllPrefabName.uiWin_GameGongGao:
                break;
            case AllPrefabName.uiWin_InGame_MJ:
                break;
            case AllPrefabName.UiWin_MjEnd_1:
                break;
            case AllPrefabName.UiWin_PhizChat:
                break;
            case AllPrefabName.uiWin_QuanJuJieSuan:
                break;
            case AllPrefabName.uiWin_Prompt_QuitGame:
                break;
            case AllPrefabName.uiWin_Prompt_QuitRoom:
                break;
            case AllPrefabName.uiWin_Prompt_Msg0:
                break;
            case AllPrefabName.uiWin_TouPiaoJieGuo:
                break;
            case AllPrefabName.uiWin_ZhanJi:
                break;
            default:
                break;
        }
        return gmValue;
    }
    public void Close_UiObject(AllPrefabName name_)
    {
        
    }

    //初始化uimanageer
    public void UIManagerClear()
	{
		//dicAnchors.Clear();
	}

    #endregion
}
