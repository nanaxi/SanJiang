using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>创建房间——兴文麻将
/// </summary>
public class CreateRoomXWMJ : CreateRoomParent
{
    /// <summary>作用于缓存创建房间的数据保证下次打开还原
    ///  必须赋值 ppKey,且不能和其他创建房间的Key一样
    /// </summary>
    [SerializeField]
    private PPData ppData_XWMJ = new PPData();
    public IntAry iAryJuShu_XWMJ;

    public Toggle tog_FangFei;//冠军房费
    public Toggle tog_DingQue;//定缺
    public Toggle tog_FanShu;//番数
    public Toggle[] tog_DiFen;//底分
    public Toggle tog_HongZhong;//红中8个
    public Toggle tog_HuType;//是否自摸胡
    public Toggle tog_3Player;//3人
    public CreateJuShu juShu;//关于局数增减
    public Button btn_CreateRoom_XWMJ;//按钮创建房间

    void Awake()
    {
        if (iAryJuShu_XWMJ==null)
        {
            Init_Var();
        }
        Debug.Log("执行先后BBBB1");
    }

    void Start()
    {
        juShu.btn_ChangeJuShuADD.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(true); });
        juShu.btn_ChangeJuShuReduce.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(false); });
        btn_CreateRoom_XWMJ.onClick.AddListener(delegate () { OnClick_Btn_CreateRoom(); });
    }

    public override void Init_Var()
    {
        iAryJuShu_XWMJ = new IntAry(new int[] { 4, 8, 12, 16, 20, 24 });
       
    }

    /// <summary>当通过加减号更改局数
    /// </summary>
    /// <param name="isAdd"></param>
    public override void OnClick_BtnChange_JuShu(bool isAdd)
    {
        int i_Result = 4;
        //如果当前显示的是创建兴文麻将
        i_Result = iAryJuShu_XWMJ.ChangeIndex(isAdd);
        juShu.inputF_JuShu.text = i_Result.ToString();
        //base.OnClick_BtnChange_JuShu(isAdd);
    }

    /// <summary>当改变局数
    /// </summary>
    /// <param name="inputF"></param>
    public override void OnChangeInpuF_JuShu(InputField inputF)
    {

        int iValue = int.Parse(inputF.text.Trim());
        iValue = iValue * 3;
        btn_CreateRoom_XWMJ.GetComponentInChildren<Text>().text = "创建 （房卡X" + iValue + "";

        Debug.Log("房卡数：" + iValue);
    }

    /// <summary>创建房间按钮
    /// </summary>
    public override void OnClick_Btn_CreateRoom()
    {
        ProtoBuf.XWMJRule xwRule = new ProtoBuf.XWMJRule();
        xwRule.roundNum = uint.Parse(juShu.inputF_JuShu.text.Trim());
        xwRule.maxFan = (uint)(tog_FanShu.isOn ? 5 : 6);//番数
        xwRule.hongZhongNum = (uint)(tog_HongZhong.isOn ? 8 : 12);//红中
        for (int i = 0; i < tog_DiFen.Length; i++)
        {
            if (tog_DiFen[i].isOn)
            {
                xwRule.baseScore = (uint)(i + 1);
                break;
            }
        }
        if (tog_FangFei.isOn)
        {//房费
            xwRule.xwmj.Add(ProtoBuf.XWMJRule.XWMJ.ChampionPay); 
        }
        if (tog_DingQue.isOn)
        {//定缺
            xwRule.xwmj.Add(ProtoBuf.XWMJRule.XWMJ.XQue);
        }

        if (tog_3Player.isOn)
        {//是否3人房间
            xwRule.xwmj.Add(ProtoBuf.XWMJRule.XWMJ.ThreePlayer);
        }
        //默认必选自摸加番
        xwRule.xwmj.Add(ProtoBuf.XWMJRule.XWMJ.ZMJF);

        //点炮胡还是自摸胡
        xwRule.xwmj.Add(tog_HuType.isOn ? ProtoBuf.XWMJRule.XWMJ.DianPaoHu : ProtoBuf.XWMJRule.XWMJ.ZiMoHu);

        PublicEvent.GetINS.NewRoom(xwRule);
    }

    /// <summary>创建房间界面数据缓存——》 保存
    /// </summary>
    public override void PlayerPrefs_Set()
    {
        List<string> listAry = new List<string>();
        Toggle[] togAry = null;
        #region /*———兴文麻将创建房间界面数据缓存———*/
        togAry = GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            listAry.Add(togAry[i].gameObject.name + (togAry[i].isOn ? "701" : "702"));
        }

        listAry.Add(juShu.inputF_JuShu.gameObject.name +
            iAryJuShu_XWMJ.IIndex.ToString()
            );
        ppData_XWMJ.PPDate_SetPP(listAry.ToArray());
        togAry = null;
        #endregion

        listAry = null;
    }

    /// <summary>创建房间界面数据缓存——》 读取
    /// </summary>
    public override void PlayerPrefs_Get()
    {
        Toggle[] togAry = null;
        ppData_XWMJ.PPData_Init();//从缓存中读取兴文麻将创建房间界面数据
        /*———兴文麻将创建房间界面数据缓存———*/
        togAry =  GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            string strPPData = ppData_XWMJ.PPData_GetValue(togAry[i].gameObject.name);
            if (strPPData.Length > togAry[i].gameObject.name.Length)
            {
                togAry[i].isOn = strPPData.IndexOf("701") >= 0;
            }
        }

        iAryJuShu_XWMJ.IIndex = int.Parse(ppData_XWMJ.PPData_GetValue(juShu.inputF_JuShu.gameObject.name).Replace(juShu.inputF_JuShu.gameObject.name, "").Trim());
        juShu.inputF_JuShu.text = iAryJuShu_XWMJ.GetIndexValue().ToString();

        togAry = null;
    }

   
}
