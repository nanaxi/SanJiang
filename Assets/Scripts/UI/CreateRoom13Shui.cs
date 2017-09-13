using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>创建房间——十三水
/// </summary>
public class CreateRoom13Shui : CreateRoomParent {

    /// <summary>作用于缓存创建房间的数据保证下次打开还原
    ///  必须赋值 ppKey,且不能和其他创建房间的Key一样
    /// </summary>
    [SerializeField]
    private PPData ppData_13Z = new PPData();

    public IntAry iAryJuShu_;

    public Toggle tog_WanFa;
    public Toggle tog_RenShu;
    public Toggle tog_MaPai;
    public Toggle tog_FangFei;
    public CreateJuShu juShu;
    public Button btn_CreateRoom;

    void Awake()
    {
        if (iAryJuShu_ == null)
        {
            Init_Var();
        }
        Debug.Log("执行先后BBBB1");
    }

    // Use this for initialization
    void Start()
    {
        juShu.btn_ChangeJuShuADD.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(true); });
        juShu.btn_ChangeJuShuReduce.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(false); });
        btn_CreateRoom.onClick.AddListener(delegate () { OnClick_Btn_CreateRoom(); });

        //juShu.inputF_JuShu.onValueChanged.AddListener(delegate (string s) { OnChangeInpuF_JuShu(juShu.inputF_JuShu); });
    }
    public override void Init_Var()
    {
        iAryJuShu_ = new IntAry(new int[] { 4, 8, 12, 16, 20, 24 });

    }

    /// <summary>当通过加减号更改局数
    /// </summary>
    /// <param name="isAdd"></param>
    public override void OnClick_BtnChange_JuShu(bool isAdd)
    {
        int i_Result = 4;
        //如果当前显示的是创建13水
        i_Result = iAryJuShu_.ChangeIndex(isAdd);
        juShu.inputF_JuShu.text = i_Result.ToString();
        //base.OnClick_BtnChange_JuShu(isAdd);
    }

    public override void OnChangeInpuF_JuShu(InputField inputF)
    {
        int iValue = int.Parse(inputF.text.Trim());
        iValue = iValue * 3;
        inputF.transform.FindChild("T_RC_Count").GetComponent<Text>().text = "局 / " + iValue + "房卡";
        Debug.Log("房卡数：" + iValue);
        
    }

    public override void OnClick_Btn_CreateRoom()
    {
        ProtoBuf.THIRtRule gameRule = new ProtoBuf.THIRtRule();
        gameRule.roundNum = uint.Parse(juShu.inputF_JuShu.text.Trim());
        if (tog_FangFei.isOn)
        {//冠军房费
            gameRule.thirt.Add( ProtoBuf.THIRtRule.THIRt.GJFF);
        }
        if (tog_RenShu.isOn)
        {//人数
            gameRule.thirt.Add(ProtoBuf.THIRtRule.THIRt.THRee);
        }
        if (tog_MaPai.isOn)
        {//码牌
            gameRule.thirt.Add(ProtoBuf.THIRtRule.THIRt.MaCard);
        }

        if (tog_WanFa.isOn)
        {//宜宾玩法1
            gameRule.thirt.Add(ProtoBuf.THIRtRule.THIRt.TPYEONE);
        }
        Debug.Log("OnClick_Btn_CreateRoom");
        PublicEvent.GetINS.NewRoom(gameRule);

    }

    public override void PlayerPrefs_Get()
    {
        Toggle[] togAry = null;
        ppData_13Z.PPData_Init();//从缓存中读取创建房间界面数据

        togAry = GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            string strPPData = ppData_13Z.PPData_GetValue(togAry[i].gameObject.name);
            if (strPPData.Length > togAry[i].gameObject.name.Length)
            {
                togAry[i].isOn = strPPData.IndexOf("701") >= 0;
            }
        }

        iAryJuShu_.IIndex = int.Parse(ppData_13Z.PPData_GetValue(juShu.inputF_JuShu.gameObject.name).Replace(juShu.inputF_JuShu.gameObject.name, "").Trim());
        juShu.inputF_JuShu.text = iAryJuShu_.GetIndexValue().ToString();

        togAry = null;
    }

    public override void PlayerPrefs_Set()
    {

        List<string> listAry = new List<string>();
        Toggle[] togAry = null;
        #region /*———13水创建房间界面数据缓存———*/
        togAry = GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            listAry.Add(togAry[i].gameObject.name + (togAry[i].isOn ? "701" : "702"));
        }

        listAry.Add(juShu.inputF_JuShu.gameObject.name +
            iAryJuShu_.IIndex.ToString()
            );
        ppData_13Z.PPDate_SetPP(listAry.ToArray());
        togAry = null;
        #endregion

        listAry = null;
    }
}
