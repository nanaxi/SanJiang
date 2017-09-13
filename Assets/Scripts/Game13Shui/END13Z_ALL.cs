using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>13水 全局结算
/// </summary>
public class END13Z_ALL : MonoBehaviour {
    public INGamePlayer_Ui[] playerUIAry;//【拖拽赋值】玩家UI

    /// <summary>用来显示ID 等等的父物体
    /// </summary>
    public Transform[] textAryParent;

    public Button btn_CloseALL;
    //   // Use this for initialization
    //   void Start () {

    //}

    /// <summary>【拖拽赋值】
    /// </summary>
    public void OnClick_Btn_CloseALL()
    {
        Debug.Log("Click  Close ALL");
    }

    /// <summary>【拖拽赋值】
    /// </summary>
    public void OnClick_Btn_WeiXin()
    {
        Debug.Log("Click 微信");
    }

    /// <summary>【拖拽赋值】
    /// </summary>
    public void OnClick_Btn_PYQ()
    {
        Debug.Log("Click 朋友圈");
    }

    public void UpdatePlayerUI(Player_Data[] playerAry)
    {
        if (playerUIAry.Length != playerAry.Length)
        {
            Debug.LogError("playerUIAry.Length!=uiAry.Length");
            return;
        }

        for (int i = 0; i < playerAry.Length; i++)
        {
            playerUIAry[i].Set_PlayerInfoUI(playerAry[i]);
            if (playerAry[i].p_ID == 0)
            {
                playerUIAry[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>更新总结算界面，杂项Text的显示
    /// </summary>
    /// <param name="charid">玩家ID</param>
    /// <param name="iGold">积分</param>
    /// <param name="iSheng">胜局</param>
    /// <param name="iShu">输局</param>
    /// <param name="iPing">平局</param>
    /// <param name="iRoomCard">房卡变化</param>
    public void UpdatePlayerUI_TextAry(uint charid, int iGold, int iSheng, int iShu, int iPing, int iRoomCard)
    {
        Transform tLayoutParent = textAryParent[GetPlayerIndex(charid)];
        Text[] tGlAry = tLayoutParent.GetComponentsInChildren<Text>(true);
        tGlAry[0].text = charid.ToString();
        tGlAry[1].text = iGold.ToString();
        tGlAry[2].text = iSheng.ToString();
        tGlAry[3].text = iShu.ToString();
        tGlAry[4].text = iPing.ToString();
        tGlAry[5].text = "-" + iRoomCard.ToString();
    }

    /// <summary>更新总结算界面，大赢家
    /// </summary>
    /// <param name="charid">玩家ID</param>
    public void UpdatePlayerUI_DaYingJia( int iGold)
    {
        for (int i = 0; i < textAryParent.Length; i++)
        {
            if (textAryParent[i].GetChild(1).GetComponent<Text>().text == iGold.ToString())
            {
                playerUIAry[i].transform.FindChild("Head/Img_DYJ").GetComponent<Image>().enabled = true;
            }
        }
    }

    public void ClearAllText()
    {
        for (int i = 0; i < textAryParent.Length; i++)
        {
            Text[] tGlAry = textAryParent[i].GetComponentsInChildren<Text>(true);
            for (int i_1 = 0; i_1 < tGlAry.Length; i_1++)
            {
                tGlAry[i].text = "";
            }
        }
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
}
