using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_HomePlayerUI : UiWin_Parent {
    public Button btnHead;
    public Text[] textAry;
    [SerializeField]
    private Player_Data p_TheData;
	// Use this for initialization
	void Start () {
        if (!isInitVar)
        {
            Init_Var();
        }
    }

    public void Init_Var()
    {
        textAry = GetComponentsInChildren<Text>();
        btnHead = GetComponentInChildren<Button>();
        isInitVar = true;   
    }

    //设置玩家钻石
    public void SetPlayerGold(int iDia = 0)
    {
        textAry[2].text = iDia.ToString();//BaseProto.playerInfo.m_diamond.ToString();
    }

    public void Set_PlayerInfoUI(Player_Data pData)
    {
        p_TheData = pData;
        if (!isInitVar)
        {
            Init_Var();
        }
        textAry[0].text = pData.P_Name;
        textAry[1].text ="ID:"+ pData.p_ID.ToString();
        textAry[2].text = BaseProto.playerInfo.m_diamond.ToString();// pData.p_Diamond.ToString();
        btnHead.image.sprite = DataManage.Instance.Head_GetSprite(pData.p_ID);
    }
}
