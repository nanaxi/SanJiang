using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>加入游戏房间界面
/// </summary>
public class UiWindows_JoinRoom : UiWin_Parent
{
    [SerializeField]
    private Text[] t_NumberAry;

    [SerializeField]
    private GameObject tNumberParent;
    // Use this for initialization
    void Start()
    {
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());
        t_NumberAry = tNumberParent.GetComponentsInChildren<Text>();// transform.FindChild("Img_Bg/Img_Number_Sum_Bg").GetComponentsInChildren<Text>();

        //BaseProto.playerInfo.m_inGame = ProtoBuf.GameType.GT_THIRt;
        Init_InputNumber();

    }

    void OnEnable()
    {
        Init_InputNumber();
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        //base.Set_OnEventList<Component_>(all_);
        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                btn_.onClick.AddListener(delegate ()
                {
                    OnClick_(btn_.gameObject);
                });
            }
        }
    }

    private void OnClick_(GameObject btn_)
    {
        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
        Debug.Log("Click Btn:" + btn_.name);
        if (btn_.gameObject.name.IndexOf("Btn_Number_") >= 0)
        {
            Btn_Number_OnClick(btn_);
        }
        else if (btn_.gameObject.name.IndexOf("Btn_CloseWin") >= 0)
        {
            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_JoinRoom);
        }
        else if (btn_.gameObject.name.IndexOf("Btn_GameType") >= 0)
        {//游戏 类型选择
            OnClick_Btn_GameType(btn_);
        }
    }

    void OnClick_Btn_GameType(GameObject btn_)
    {
        switch (btn_.name )
        {
            case "Btn_GameType_13Z":
                BaseProto.playerInfo.m_inGame = ProtoBuf.GameType.GT_THIRt;
                break;
            case "Btn_GameType_XWMJ":
                BaseProto.playerInfo.m_inGame = ProtoBuf.GameType.GT_XWMJ;
                break;
            case "Btn_GameType_XWD2":
                BaseProto.playerInfo.m_inGame = ProtoBuf.GameType.GT_DE;
                break;
            default:
                break;
        }
    }

    const string numberKeyPatg = "Sprites/NingXiaHuaShui/QieTu/NumberKey/CaiSe/";
    public void Btn_Number_OnClick(GameObject btn_Gm)
    {//
        if (btn_Gm.name.IndexOf("Btn_Number_") >= 0)
        {
            int i1 = -1;
            try
            {
                i1 = int.Parse(btn_Gm.name.Replace("Btn_Number_", ""));
            }
            catch
            {
                Debug.LogError("处理数字按钮名字可能出现了错误");
            }

            if (i1 >= 0 && i1 <= 9)
            {//数字键 按钮事件
                for (int i = 0; i < t_NumberAry.Length; i++)
                {
                    if (t_NumberAry[i].text == "")
                    {
                        t_NumberAry[i].text = i1.ToString();
                        //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.sprite = Resources.Load<Sprite>(numberKeyPatg + i1.ToString());
                        //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.enabled = true;
                        if (i == t_NumberAry.Length - 1)
                        {
                            Debug.Log("输入了6位数？");
                            Btn_JoinGame_OnClick();
                        }
                        break;
                    }
                }
            }
            else if (i1 == 11)
            {//重输按钮事件
                Init_InputNumber();
            }
            else if (i1 == 12)
            {//删除按钮事件
                for (int i = t_NumberAry.Length - 1; i >= 0; i--)
                {
                    if (t_NumberAry[i].text != "")
                    {
                        t_NumberAry[i].text = "";
                        //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.sprite = null;
                        //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.enabled = false;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogError("这个方法作用于加入游戏的时候的数字键输入，请检查哪个按钮错误的调用了这个事件 :" + btn_Gm.name);
        }
    }

    void Init_InputNumber()
    {
        for (int i = t_NumberAry.Length - 1; i >= 0; i--)
        {
            t_NumberAry[i].text = "";
            //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.sprite = null;
            //t_NumberAry[i].transform.parent.GetComponentInChildren<Button>().image.enabled = false;
        }
    }

    /// <summary>输入房间号后，加入游戏按钮事件
    /// </summary>
    /// <param name="allNumberText_Parent">所有显示输入房间号的父物体，用于获取所有房间数字和</param>
    public void Btn_JoinGame_OnClick()
    {
        string allNumber = "";
        for (int i = 0; i < t_NumberAry.Length; i++)
        {
            allNumber += t_NumberAry[i].text;
        }

        //Btn_OpenWindow(T_Window_Type.Window_12);
        print("<color=red> 测试想要加入的房间号为：" + allNumber + "</color>");
        if (allNumber.Length == 6)
        {
            PublicEvent.GetINS.AppJoin((uint)int.Parse(allNumber));//Request Join Room
        }
        else
        {
            Debug.Log("请输入房间号");
        }
    }

}
