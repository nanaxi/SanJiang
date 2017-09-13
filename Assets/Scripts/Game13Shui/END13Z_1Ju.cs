using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>13张 1局结算界面
/// </summary>
public class END13Z_1Ju : MonoBehaviour {
    public INGamePlayer_Ui[] playerUIAry;//【拖拽赋值】玩家UI
    /// <summary>//【拖拽赋值】摆牌的Glayout 父物体
    /// </summary>
    public Transform[] gLayoutParentAry;

    /// <summary>//【拖拽赋值】特殊牌，文字显示的父对象
    /// </summary>
    public Transform[] tSP_BgAry;

    public Transform vLayoutBg;//所有显示Poker的背景

    public Button btnCloseEnd1Ju;

    [SerializeField]
    private Text tTitle;
    // Use this for initialization
    void Awake()
    {
        for (int i = 0; i < playerUIAry.Length; i++)
        {
            playerUIAry[i].Init_Var();
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < tSP_BgAry.Length; i++)
        {
            tSP_BgAry[i].GetComponentInChildren<Text>().text = "";
            tSP_BgAry[i].gameObject.SetActive(false);
        }
        
    }

    public void UpdatePlayerUI(Player_Data[] playerAry)
    {
        if (playerUIAry.Length!= playerAry.Length)
        {
            Debug.LogError("playerUIAry.Length!=uiAry.Length");
            return;
        }

        for (int i = 0; i < playerAry.Length; i++)
        {
            playerUIAry[i].playerCharID = 0;
            playerUIAry[i].Set_PlayerInfoUI(playerAry[i]);
            //playerUIAry[i].btnHead.image.sprite = DataManage.Instance.Head_GetSprite(playerAry[i].p_ID);
            //playerUIAry[i].textAry[1].text = playerAry[i].P_Name + "\nID:" + playerAry[i].p_ID;// ; uiAry[i].GetNameID;

            if (playerAry[i] == null)
            {
                playerUIAry[i].transform.parent.gameObject.SetActive(false);
            }
            else if (playerAry[i] != null && playerAry[i].p_ID == 0)
            {
                playerUIAry[i].transform.parent.gameObject.SetActive(false);
            }
            else if ( playerAry[i] != null && playerAry[i].p_ID != 0)
            {
                playerUIAry[i].transform.parent.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>单局结算更新玩家——》金币
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="iGold"></param>
    public void UpdatePlayerUI_Gold(uint charid,int iGold)
    {
        playerUIAry[GetPlayerIndex(charid)].Set_DiFen(iGold);
        if (charid == DataManage.Instance.MyPlayer_Data.p_ID)
        {
            if (iGold<0)
            {
                tTitle.text = "<color=black>失败</color>";
            }
            else if (iGold > 0)
            {
                tTitle.text = "<color=yellow>胜利</color>";
            }
            else if (iGold == 0)
            {
                tTitle.text = "平局";
            }
        }
    }

    /// <summary>单局结算更新玩家——》扑克牌
    /// </summary>
    /// <param name="charid">玩家ID</param>
    /// <param name="pokerAry">玩家牌组</param>
    /// <param name="isTSP">是否特殊牌</param>
    public void UpdatePlayerUI_Poker(uint charid,GameObject[] pokerAry ,bool isTSP)
    {
        Transform tLayoutParent = gLayoutParentAry[GetPlayerIndex(charid)];
        GridLayoutGroup[] tGlAry = tLayoutParent.GetComponentsInChildren<GridLayoutGroup>(true);
        if (isTSP)
        {
            for (int i = 0; i < pokerAry.Length; i++)
            {
                pokerAry[i].transform.SetParent(tGlAry[0].transform);
                pokerAry[i].transform.localScale = Vector3.one;
            }
        }
        else {
            for (int i = 0; i < pokerAry.Length; i++)
            {
                if (i >= 0 && i < 3)
                {
                    pokerAry[i].transform.SetParent(tGlAry[0].transform);
                    pokerAry[i].transform.localScale = Vector3.one;
                }
                else if (i >= 3 && i < 8)
                {
                    pokerAry[i].transform.SetParent(tGlAry[1].transform);
                    pokerAry[i].transform.localScale = Vector3.one;
                }
                else if (i >= 8 && i < 13)
                {
                    pokerAry[i].transform.SetParent(tGlAry[2].transform);
                    pokerAry[i].transform.localScale = Vector3.one;
                }
            }
        }
        for (int i = 0; i < tGlAry.Length; i++)
        {
            if (tGlAry[i] != null && tGlAry[i].transform.childCount > 0)
            {
                Poker_Sort(tGlAry[i].GetComponentsInChildren<PokerInfos>());
            }
        }
    }

    /// <summary>本地手牌排序 ：1、当去下摆牌的时候
    /// </summary>
    public void Poker_Sort(PokerInfos[] pokerAry)
    {
        pokerAry = (from pokerIns in pokerAry
                    orderby pokerIns.pokerInfo.CardValue descending
                    select pokerIns).ToArray();

        for (int i = 0; i < pokerAry.Length; i++)
        {
            if (pokerAry[i] != null)
            {
                if (pokerAry[i].pokerInfo.CardValue > 1)
                {
                    pokerAry[i].transform.SetSiblingIndex(i);
                }
                else if (pokerAry[i].pokerInfo.CardValue == 1)
                {//A排最左边
                    pokerAry[i].transform.SetAsFirstSibling();
                }
            }
        }
    }

    /// <summary>显示玩家的特殊牌类型名称
    /// </summary>
    /// <param name="charid"></param>
    /// <param name="tspName"></param>
    public void UpdatePlayerUI_TSPTxt(uint charid, string tspName)
    {
        if (tspName.Length<2)
        {
            return;
        }
        Transform tLayoutParent = tSP_BgAry[GetPlayerIndex(charid)];
        tLayoutParent.GetComponentInChildren<Text>().text = tspName;
        tLayoutParent.gameObject.SetActive(true);
    }
    
    public void CloseNullSeat()
    {
        
    }

    int GetPlayerIndex(uint charid)
    {
        for (int i = 0; i < playerUIAry.Length; i++)
        {
            if (playerUIAry[i].playerCharID == charid)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>获取结束界面的所有扑克
    /// </summary>
    public PokerInfos[] GetBP_AllPoker()
    {
        PokerInfos[] resultAry= vLayoutBg.GetComponentsInChildren<PokerInfos>(true);
        return resultAry;
    }

    /// <summary>【拖拽赋值】
    /// </summary>
    public void OnClick_Btn_CloseEND1()
    {
        gameObject.SetActive(false);
    }
}
