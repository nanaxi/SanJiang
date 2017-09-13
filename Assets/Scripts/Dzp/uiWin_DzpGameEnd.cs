using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class uiWin_DzpGameEnd : MonoBehaviour
{
    public Button Share = null;
    public Button Continue = null;
    /// <summary>
    /// player的位置存储
    /// </summary>
    public Transform P1, P2, P3;
    public GameObject RedFlag, BlueFlag;
    // Use this for initialization
    void Start()
    {
        if (Share != null)
            Share.onClick.AddListener(delegate
            {

            });
        if (Continue != null)
            Continue.onClick.AddListener(delegate
            {
                UIManager.Instance.FindUI(AllPrefabName.uiWin_DzpPlayRoom).GetComponent<UiWin_DzpPlayRoom>().junum = 0;
                GameManager.GM.Game_RetrueHome();
                Destroy(this.gameObject);
            });
        //SetResult(0, 45);
        //SetResult(1, 45);
        //SetResult(2, 45);
    }
    ProtoBuf.MJGameOver Rsp;
    public void ReciveRsp(ProtoBuf.MJGameOver rsp=null)
    {
        Rsp = rsp;
        if (Rsp != null)
        {
            List<uint> TempList = new List<uint>();
            for (int i = 0; i < Rsp.players.Count; i++)
            {
                SetResult(Rsp.players[i].charId, Rsp.players[i].changeGold);
                SetPlayerInforMation(Rsp.players[i].charId);
            }
        }

    }
    /// <summary>
    /// 用来显示胡息、翻数、积分
    /// </summary>
    void SetResult(uint charid, int Score)
    {
        string TempSocre = null;
        if (Score >= 0)
        {
            TempSocre = "    +" + Score.ToString();
        }
        else
        {
            TempSocre = "    " + Score.ToString();
        }
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))  
        //switch (charid)
        {
            case 0:
                P1.Find("Head/RestRoomCard/Text").GetComponent<Text>().text = "剩余  房卡  " + DZP_PublicEvent.GetINS.GetPlayerRoomCard(charid);
                P1.Find("Head/Score/Text").GetComponent<Text>().text = TempSocre;
                break;
            case 1:
                P2.Find("Head/RestRoomCard/Text").GetComponent<Text>().text = "剩余  房卡  " + DZP_PublicEvent.GetINS.GetPlayerRoomCard(charid);
                P2.Find("Head/Score/Text").GetComponent<Text>().text = TempSocre;
                break;
            case 2:
                P3.Find("Head/RestRoomCard/Text").GetComponent<Text>().text = "剩余  房卡  " + DZP_PublicEvent.GetINS.GetPlayerRoomCard(charid);
                P3.Find("Head/Score/Text").GetComponent<Text>().text = TempSocre;
                break;
            default:
                break;
        }
    }
    void SetPlayerInforMation(uint charid)
    {
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))
        {
            case 0:
                P1.FindChild("Head/NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P1.FindChild("Head/ID").GetComponent<Text>().text = "ID:"+DZP_PublicEvent.GetINS.GetPlayerName(charid);
                break;
            case 1:
                P2.FindChild("Head/NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P1.FindChild("Head/ID").GetComponent<Text>().text = "ID:" + DZP_PublicEvent.GetINS.GetPlayerName(charid);
                break;
            case 2:
                P3.FindChild("Head/NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P1.FindChild("Head/ID").GetComponent<Text>().text = "ID:" + DZP_PublicEvent.GetINS.GetPlayerName(charid);
                break;
            default:
                break;
        }
    }
}
