using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateRoomXWD2 : CreateRoomParent
{
    [SerializeField]
    private PPData ppData_XWD2 = new PPData();
    public IntAry iAryJuShu_;

    public Toggle tog_FangFei;
    public Toggle[] tog_DiFen;

    public Toggle tog_ZiMoFanBei;
    public Toggle tog_ZhengHuFanBei;
    public Toggle tog_FangPaoBaoPei;
    public CreateJuShu juShu;
    public Button btn_CreateRoom;

    // Use this for initialization
    void Start()
    {
        juShu.btn_ChangeJuShuADD.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(true); });
        juShu.btn_ChangeJuShuReduce.onClick.AddListener(delegate () { OnClick_BtnChange_JuShu(false); });
        btn_CreateRoom.onClick.AddListener(delegate { OnClick_Btn_CreateRoom(); });
    }

    public override void Init_Var()
    {
        iAryJuShu_ = new IntAry(new int[] { 4, 8, 12, 16, 20, 24, 28, 32, 36, 40 });
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
        btn_CreateRoom.GetComponentInChildren<Text>().text = "创建 （房卡X" + iValue + "";
        Debug.Log("房卡数：" + iValue);
    }

    public override void OnClick_Btn_CreateRoom()
    {
        ProtoBuf.DERule d2Rule = new ProtoBuf.DERule();
        d2Rule.roundNum = uint.Parse(juShu.inputF_JuShu.text.Trim());
        if (tog_FangFei.isOn)
        {//是否冠军房费
            d2Rule.de.Add(ProtoBuf.DERule.DE.GJFF);
        }

        if (tog_ZiMoFanBei.isOn)
        {//是否自摸加番
            d2Rule.de.Add(ProtoBuf.DERule.DE.ZMJF);
        }
        if (tog_ZhengHuFanBei.isOn)
        {//是否整胡翻倍
            d2Rule.de.Add(ProtoBuf.DERule.DE.ZHJF);
        }
        if (tog_FangPaoBaoPei.isOn)
        {//是否放炮包陪
            d2Rule.de.Add(ProtoBuf.DERule.DE.FPBPCJ);
        }
        Debug.Log("创建大贰？");
        PublicEvent.GetINS.NewRoom(d2Rule);
    }

    public override void PlayerPrefs_Get()
    {
        Toggle[] togAry = null;
        ppData_XWD2.PPData_Init();
        #region /*———兴文麻将创建房间界面数据缓存———*/
        togAry = GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            string strPPData = ppData_XWD2.PPData_GetValue(togAry[i].gameObject.name);
            if (strPPData.Length > togAry[i].gameObject.name.Length)
            {
                togAry[i].isOn = strPPData.IndexOf("701") >= 0;
            }
        }

        iAryJuShu_.IIndex = int.Parse(ppData_XWD2.PPData_GetValue(juShu.inputF_JuShu.gameObject.name).Replace(juShu.inputF_JuShu.gameObject.name, "").Trim());
        juShu.inputF_JuShu.text = iAryJuShu_.GetIndexValue().ToString();

        #endregion

        togAry = null;
    }

    public override void PlayerPrefs_Set()
    {
        List<string> listAry = new List<string>();
        Toggle[] togAry = null;


        #region /*———兴文大2创建房间界面数据缓存———*/
        togAry = transform.GetChild(1).GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < togAry.Length; i++)
        {
            listAry.Add(togAry[i].gameObject.name + (togAry[i].isOn ? "701" : "702"));
        }

        listAry.Add(juShu.inputF_JuShu.gameObject.name +
            iAryJuShu_.IIndex.ToString()
            );
        ppData_XWD2.PPDate_SetPP(listAry.ToArray());
        togAry = null;
        listAry = null;
        #endregion
    }
}
