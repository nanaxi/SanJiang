using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public enum DzpOp
{
    Peng,
    Gang,
    AnGang,
    Chi,
    Hu,
}
public class uiWin_DzpGameOver : MonoBehaviour
{
    public Button Share = null;
    public Button Continue = null;
    public GameObject PGcard = null;
    /// <summary>
    /// player的位置存储
    /// </summary>
    public Transform P1, P2, P3;
    public GameObject RestCardType = null;
    public Transform RestCardArea = null;
    // Use this for initialization
    void Start()
    {
        UiWin_DzpPlayRoom PlayRoom = UIManager.Instance.FindUI(AllPrefabName.uiWin_DzpPlayRoom).GetComponent<UiWin_DzpPlayRoom>();
        PlayRoom.junum--;
        if (Share != null)
            Share.onClick.AddListener(delegate
            {

            });
        if (Continue != null)
            Continue.onClick.AddListener(delegate
            {
                if (PlayRoom.junum > 30 || PlayRoom.junum < 0)
                {
                    uiWin_DzpGameEnd tempGameOver = UIManager.Instance.ShowUI(AllPrefabName.uiWin_DzpGameEnd, UIManager.Instance.canvas_T).GetComponent<uiWin_DzpGameEnd>();
                    tempGameOver.ReciveRsp(DZP_PublicEvent.GetINS.Dzp_GameEndRsp);
                    Destroy(this.gameObject);
                }
                else
                {
                    PlayRoom.ClientReady();
                    Destroy(this.gameObject);
                }
                
            });
    }
    ProtoBuf.MJGameOver Rsp;
    public void ReciveRsp(ProtoBuf.MJGameOver rsp)
    {
        Rsp = rsp;
        List<uint> TempList = new List<uint>();
        for (int i = 0; i < Rsp.players.Count; i++)
        {

            for (int z = 0; z < Rsp.players[i].pengCards.Count; z++)
            {

                TempList.Add(Rsp.players[i].pengCards[z]);
                SetPGcard(TempList, DzpOp.Peng, Rsp.players[i].charId);
                TempList.Clear();
            }


            for (int z = 0; z < Rsp.players[i].gangInfos.Count; z++)
            {
                TempList.Clear();
                TempList.Add(Rsp.players[i].gangInfos[z].card);
                SetPGcard(TempList, DzpOp.Gang, Rsp.players[i].charId);
            }


            for (int z = 0; z < Rsp.players[i].chiCards.Count; z++)
            {
                TempList.Clear();
                TempList.Add(Rsp.players[i].chiCards[z].cardo);
                TempList.Add(Rsp.players[i].chiCards[z].cardt);
                TempList.Add(Rsp.players[i].chiCards[z].cards);
                SetPGcard(TempList, DzpOp.Chi, Rsp.players[i].charId);
            }
            SetResult(Rsp.players[i].charId, Rsp.players[i].fuximax, Rsp.players[i].glodcFen, Rsp.players[i].changeGold);
            for (int z = 0; z < Rsp.players[i].huInfos.Count; z++)
            {

                TempList.Clear();
                TempList.Add(Rsp.players[i].huInfos[z].card);
                SetPGcard(TempList, DzpOp.Hu, Rsp.players[i].charId);
            }
            SetPlayerInforMation(Rsp.players[i].charId);
        }
        SetUnreleasedCard(rsp.roundRestCard);
    }
    /// <summary>
    /// 得到一个碰杠的牌
    /// </summary>
    void SetPGcard(List<uint> cards, DzpOp Type, uint charid)
    {
        var TempCard = Instantiate(PGcard);
        TempCard.SetActive(true);
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))
        {
            case 0:
                TempCard.transform.SetParent(P1.FindChild("PGList"), false);
                break;
            case 1:
                TempCard.transform.SetParent(P2.FindChild("PGList"), false);
                break;
            case 2:
                TempCard.transform.SetParent(P3.FindChild("PGList"), false);
                break;
            default:
                break;
        }
        switch (Type)
        {
            case DzpOp.Peng:
                for (int i = 0; i < 3; i++)
                {
                    if (TempCard != null)
                    {
                        TempCard.transform.GetChild(i).GetComponent<Image>().sprite = cards[0].ToDzpCard();
                    }
                }
                Debug.Log("玩家" + charid + "碰：" + cards[0]);
                TempCard.transform.GetChild(3).gameObject.SetActive(false);
                TempCard.transform.GetChild(4).GetComponent<Text>().text = "碰";
                TempCard.transform.GetChild(5).GetComponent<Text>().text = "0";
                break;
            case DzpOp.Gang:
                for (int i = 0; i < 4; i++)
                {
                    if (TempCard != null)
                    {
                        TempCard.transform.GetChild(i).GetComponent<Image>().sprite = cards[0].ToDzpCard();
                    }
                }
                TempCard.transform.GetChild(4).GetComponent<Text>().text = "杠";
                TempCard.transform.GetChild(5).GetComponent<Text>().text = "0";
                break;
            case DzpOp.AnGang:
                for (int i = 0; i < 4; i++)
                {
                    if (TempCard != null)
                    {
                        TempCard.transform.GetChild(i).GetComponent<Image>().sprite = cards[0].ToDzpCard();
                    }
                }
                TempCard.transform.GetChild(4).GetComponent<Text>().text = "暗杠";
                TempCard.transform.GetChild(5).GetComponent<Text>().text = "0";
                break;
            case DzpOp.Chi:
                for (int i = 0; i < cards.Count; i++)
                {
                    if (TempCard != null)
                    {
                        TempCard.transform.GetChild(i).GetComponent<Image>().sprite = cards[i].ToDzpCard();
                    }
                }
                TempCard.transform.GetChild(3).gameObject.SetActive(false);
                TempCard.transform.GetChild(4).GetComponent<Text>().text = "吃";
                TempCard.transform.GetChild(5).GetComponent<Text>().text = "0";
                break;
            case DzpOp.Hu:
                if (TempCard != null)
                {
                    TempCard.transform.GetChild(0).GetComponent<Image>().sprite = cards[0].ToDzpCard();
                    TempCard.transform.GetChild(1).gameObject.SetActive(false);
                    TempCard.transform.GetChild(2).GetComponent<Image>().sprite = cards[0].ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                }
                Debug.Log("玩家" + charid + "胡：" + cards[0]);
               
                TempCard.transform.GetChild(4).GetComponent<Text>().text = "胡";
                TempCard.transform.GetChild(5).GetComponent<Text>().text = "0";
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 用来显示胡息、翻数、积分
    /// </summary>
    void SetResult(uint charid, uint Huxi, uint fan, int Score)
    {
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))
        //switch (charid)
        {
            case 0:
                P1.Find("Huxi").GetComponent<Text>().text = "胡息:" + Huxi.ToString();
                P1.Find("Score").GetComponent<Text>().text = "分数\n" + Score.ToString();
                P1.Find("FanShu").GetComponent<Text>().text = "翻数:" + fan.ToString();
                break;
            case 1:
                P2.Find("Huxi").GetComponent<Text>().text = "胡息:" + Huxi.ToString();
                P2.Find("Score").GetComponent<Text>().text = "分数\n" + Score.ToString();
                P2.Find("FanShu").GetComponent<Text>().text = "翻数:" + fan.ToString();
                break;
            case 2:
                P3.Find("Huxi").GetComponent<Text>().text = "胡息:" + Huxi.ToString();
                P3.Find("Score").GetComponent<Text>().text = "分数\n" + Score.ToString();
                P3.Find("FanShu").GetComponent<Text>().text = "翻数:" + fan.ToString();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 设定还没有被摸到的牌
    /// </summary>
    /// <param name="cards"></param>
    void SetUnreleasedCard(List<uint> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (RestCardType != null)
            {
                var tempcard = Instantiate(RestCardType);
                tempcard.SetActive(true);
                tempcard.transform.GetChild(0).GetComponent<Image>().sprite = cards[i].ToDzpCard();
                tempcard.transform.SetParent(RestCardArea, false);
            }
        }
    }
    void SetPlayerInforMation(uint charid)
    {
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))
        {
            case 0:
                P1.FindChild("NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P1.FindChild("ID").GetComponent<Text>().text = charid.ToString();
                break;
            case 1:
                P2.FindChild("NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P2.FindChild("ID").GetComponent<Text>().text = charid.ToString();
                break;
            case 2:
                P3.FindChild("NickName").GetComponent<Text>().text = DZP_PublicEvent.GetINS.GetPlayerName(charid);
                P3.FindChild("ID").GetComponent<Text>().text = charid.ToString();
                break;
            default:
                break;
        }
    }
    void OnDestroy()
    {

    }
}
