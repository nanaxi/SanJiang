using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using ProtoBuf;
using DG.Tweening;
//nausea这个歌可以听下
public struct CardInfo
{
    /// <summary>
    /// 牌面值 1-10  
    /// </summary>
    public int num;
    /// <summary>
    /// 牌面是大写还是小写。  true为大写
    /// </summary>
    public bool isMax;
    public bool isMask;
    public CardInfo(int num, bool isMax)
    {
        this.num = num;
        this.isMax = isMax;
        this.isMask = false;
    }
}
[System.Serializable]
public class CardArray
{
    public GameObject[] card;
}
public static class ExtendClass
{
    public static CardInfo ToCardInfo(this uint value)
    {
        CardInfo info;

        if (value < 11)
        {
            info = new CardInfo((int)value, false);
        }
        else
        {
            info = new CardInfo((int)value - 16, true);
        }
        return info;
    }
    public static Sprite ToDzpCard(this uint card)
    {
        if (card == 0)
        {
            Debug.Log("0牌");
            return null;
        }
        else
        {
            if (card.ToCardInfo().isMax)
                return Resources.Load<GameObject>("DaEr/card/A" + card.ToCardInfo().num).GetComponent<Image>().sprite;
            else
                return Resources.Load<GameObject>("DaEr/card/" + card.ToCardInfo().num).GetComponent<Image>().sprite;
        }

    }
}
public class UiWin_DzpPlayRoom : MonoBehaviour
{
    public Text RoomNum = null;
    public Text TimeNum = null;
    List<uint> AllPlayer = new List<uint>();
    //场景内按钮
    public Button Voice = null;
    public Button FaceAndChat = null;
    public Button Setting = null;
    public Button Quit = null;

    /// <summary>
    /// 玩家头像按钮
    /// </summary>
    public Button[] PlayerHeadButton = new Button[3];
    /// <summary>
    /// 游戏事件按钮
    /// </summary>
    public Button Hu = null;
    public Button Zimo = null;
    public Button Guo = null;
    public Button Ready = null;
    public Button Chi = null;
    public Button Ting = null;
    //场景内手牌管理,让所有手牌显示隐藏
    public GameObject GameCardObj = null;
    void Awake()
    {
        DZP_PublicEvent.GetINS.Event_AskPopCard += AskPlayerPop;
        DZP_PublicEvent.GetINS.Event_AskChiCard += AskClientChiCard;
        DZP_PublicEvent.GetINS.Event_AskHuCard += AskClientHuCard;
        DZP_PublicEvent.GetINS.Event_AskPlayerTing += AskTingCards;

        DZP_PublicEvent.GetINS.Event_GetFirstCard += GetPlayerHandCard;
        DZP_PublicEvent.GetINS.Event_PlayerComing += SetPlayerInformation;
        DZP_PublicEvent.GetINS.Event_PlayerMoCard += GetPlayerMocardAnim;
        DZP_PublicEvent.GetINS.Event_PlayerPopCard += GetPopedCard;
        DZP_PublicEvent.GetINS.Event_PlayerPengCard += GetPengGangCard;
        DZP_PublicEvent.GetINS.Event_PlayerLongCard += GetLongCard;
        DZP_PublicEvent.GetINS.Event_GetBuCard += GetBuPai;
        DZP_PublicEvent.GetINS.Event_SwitchPlayer += SwitchplayerLight;
        DZP_PublicEvent.GetINS.Event_GameRestart += RestGame;
        DZP_PublicEvent.GetINS.Event_GetPlayerReady += GetPlayerReady;
        DZP_PublicEvent.GetINS.Event_PlayerChiCard += GetChiCard;
    }
    void Start()
    {
        if (Ready != null)
            Ready.onClick.AddListener(ClientReady);
        if (Guo != null)
            Guo.onClick.AddListener(PressGuo);
        //Guo.gameObject.SetActive(false);
        //Chi.gameObject.SetActive(false);
        if (Hu != null)
            Hu.onClick.AddListener(PressHu);
        //Hu.gameObject.SetActive(false);
        if (Ting != null)
            Ting.onClick.AddListener(PressTing);
        //Ting.gameObject.SetActive(false);
        if (Quit != null)
        {
            Quit.onClick.AddListener(delegate
            {
                //UIManager.Instance.ShowUI(AllPrefabName.uiWin_DzpAskVote, UIManager.Instance.canvas_T);
            });
        }

    }
    void OnDestroy()
    {
        DZP_PublicEvent.GetINS.Event_AskPopCard -= AskPlayerPop;
        DZP_PublicEvent.GetINS.Event_AskChiCard -= AskClientChiCard;
        DZP_PublicEvent.GetINS.Event_AskHuCard -= AskClientHuCard;
        DZP_PublicEvent.GetINS.Event_AskPlayerTing -= AskTingCards;

        DZP_PublicEvent.GetINS.Event_GetFirstCard -= GetPlayerHandCard;
        DZP_PublicEvent.GetINS.Event_PlayerComing -= SetPlayerInformation;
        DZP_PublicEvent.GetINS.Event_PlayerPopCard -= GetPopedCard;
        DZP_PublicEvent.GetINS.Event_PlayerMoCard -= GetPlayerMocardAnim;
        DZP_PublicEvent.GetINS.Event_PlayerPengCard -= GetPengGangCard;
        DZP_PublicEvent.GetINS.Event_PlayerLongCard -= GetLongCard;
        DZP_PublicEvent.GetINS.Event_GetBuCard -= GetBuPai;
        DZP_PublicEvent.GetINS.Event_SwitchPlayer -= SwitchplayerLight;
        DZP_PublicEvent.GetINS.Event_GameRestart -= RestGame;
        DZP_PublicEvent.GetINS.Event_GetPlayerReady -= GetPlayerReady;
        DZP_PublicEvent.GetINS.Event_PlayerChiCard -= GetChiCard;
    }

    /////////////////////////////////////////////////////////////////
    /////游戏场景内
    /////////////////////////////////////////////////////////////////
    MJRoomInfo DaErRoom;//重连之后使用最多
    internal void setData(MJRoomInfo mjRoom)
    {
        uint[] TempList = new uint[3];
        GetRoomNum(mjRoom.roomId);
        junum = (int)mjRoom.roomRuleInfo.deRule.roundNum - 1;
        TempList = SortAllPlayerList(mjRoom.charIds);
        for (int i = 0; i < TempList.Length; i++)
        {
            for (int z = 0; z < mjRoom.charInfos.Count; z++)
            {
                if (TempList[i] == mjRoom.charInfos[z].charId)
                {
                    DZP_PublicEvent.GetINS.AddPlayer(i, mjRoom.charInfos[z].charId, mjRoom.charInfos[z].userName, mjRoom.charInfos[z].ip, mjRoom.charInfos[z].portrait, true);
                    //SetPlayerInformation(i, mjRoom.charInfos[z].charId, mjRoom.charInfos[z].userName, mjRoom.charInfos[z].ip, mjRoom.charInfos[z].portrait);
                    break;
                }
            }
        }
        DaErRoom = mjRoom;
        if (DaErRoom != null)
        {
            ///缓存的之前的吃碰杠
            int playerCount = DaErRoom.cardsInfos.Count;
            for (int z = 0; z < playerCount; z++)
            {
                //得到手牌
                if (DaErRoom.cardsInfos[z].charId == BaseProto.playerInfo.m_id)
                {
                    if (DaErRoom.cardsInfos[z].handCards.Count > 1)
                        DZP_PublicEvent.GetINS.Fun_GetFirstCard(BaseProto.playerInfo.m_id, DaErRoom.cardsInfos[z].handCards);
                }
                for (int p = 0; p < DaErRoom.cardsInfos[z].passCards.Count; p++)
                {
                    GetPopedCard(DaErRoom.cardsInfos[z].charId, DaErRoom.cardsInfos[z].passCards[p]);
                }
                //吃
                List<ChiPai> temp = DaErRoom.cardsInfos[z].chiCards;
                if (temp.Count != 0)
                    GetChiCard(DaErRoom.cardsInfos[z].charId, 0, temp, 0);

                //碰
                for (int p = 0; p < DaErRoom.cardsInfos[z].pengCards.Count; p++)
                {
                    GetPengGangCard(DaErRoom.cardsInfos[z].charId, DaErRoom.cardsInfos[z].pengCards[p], 0, false, true);
                }
                //明杠
                for (int p = 0; p < DaErRoom.cardsInfos[z].mingGangCards.Count; p++)
                {
                    GetPengGangCard(DaErRoom.cardsInfos[z].charId, DaErRoom.cardsInfos[z].mingGangCards[p], 0, true, true);
                }
                //暗杠
                for (int p = 0; p < DaErRoom.cardsInfos[z].anGangCards.Count; p++)
                {
                    GetLongCard(DaErRoom.cardsInfos[z].charId, DaErRoom.cardsInfos[z].anGangCards[p], 0, true);
                }
            }


            //当前玩家的准备状态
            playerCount = DaErRoom.charStates.Count;
            for (int i = 0; i < playerCount; i++)
            {
                if (DaErRoom.charStates[i].isZB > 0 && BaseProto.playerInfo.m_id == DaErRoom.charStates[i].charId)
                {
                    Ready.gameObject.SetActive(false);
                }
            }


            //当前的吃碰杠请求
            playerCount = DaErRoom.roomCache.charList.Count;
            for (int i = 0; i < playerCount; i++)
            {
                //杠
                for (int z = 0; z < DaErRoom.roomCache.charList[i].gangList.Count; z++)
                {
                    GetPengGangCard(DaErRoom.roomCache.charList[i].charId, DaErRoom.roomCache.charList[i].gangList[z], 0, true);
                }
                SwitchplayerLight(DaErRoom.roomCache.charList[i].charId);
                for (int z = 0; z < DaErRoom.roomCache.charList[i].opList.Count; z++)
                {
                    switch (DaErRoom.roomCache.charList[i].opList[z])
                    {
                        case 7:
                            Debug.Log("重连碰");
                            GetPengGangCard(DaErRoom.roomCache.charList[i].charId, DaErRoom.roomCache.charList[i].cardList[z], DaErRoom.roomCache.charList[i].oriCharId, false, true);
                            break;
                        case 9:
                            Debug.Log("Gang");
                            GetPengGangCard(DaErRoom.roomCache.charList[i].charId, DaErRoom.roomCache.charList[i].cardList[z], DaErRoom.roomCache.charList[i].oriCharId, true, true);
                            break;
                        case 10:
                            Debug.Log("AnGang");
                            GetLongCard(DaErRoom.roomCache.charList[i].charId, DaErRoom.roomCache.charList[i].cardList[z], DaErRoom.roomCache.charList[i].oriCharId, true);
                            break;
                        case 14:
                            Debug.Log("请求吃");
                            AskClientChiCard(DaErRoom.roomCache.charList[i].charId, DaErRoom.roomCache.charList[i].oriCharId, DaErRoom.roomCache.charList[i].ChiList, DaErRoom.roomCache.charList[i].cardList[z]);

                            break;
                        case 5:
                            Debug.Log("请求出牌");
                            AskPlayerPop(DaErRoom.roomCache.charList[i].charId);
                            break;
                        default:
                            break;
                    }
                }

            }

        }
    }

    /// <summary>
    /// 设定玩家的ui显示
    /// </summary>
    void SetPlayerInformation(int num, uint charid, string name, string ip, string head)
    {
        var ThisPlayer = transform.FindChild("Players/Player" + num);
        ThisPlayer.GetChild(0).GetComponent<Text>().text = "姓名:" + name;
        ThisPlayer.GetChild(1).GetComponent<Text>().text = "id:" + charid;
    }
    /// <summary>
    /// 玩家排序
    /// </summary>
    uint[] SortAllPlayerList(List<uint> player)
    {
        uint[] TempList = new uint[3];
        int TempNum = 0;
        //string Paixu = "";
        //for (int i = 0; i < player.Count; i++)
        //{
        //    Paixu += player[i] + "  ";
        //}
        //Debug.Log("原来的排序：" + Paixu);


        for (int i = 0; i < player.Count; i++)
        {
            if (player[i] == BaseProto.playerInfo.m_id)
            {
                Debug.Log("找到了!");
                TempNum = i;
                Debug.Log("当前的TempNum：" + TempNum);
                break;
            }
        }
        int z = 0;
        for (int i = TempNum; i < 3; i++)
        {
            for (; z < TempList.Length;)
            {
                if (i < player.Count)
                {
                    TempList[z] = player[i];
                    Debug.Log("当前的i：" + i);
                }
                else
                {
                    TempList[z] = 0;
                    Debug.Log("当前的z：" + z);
                }
                z++;
                break;
            }
        }
        for (int i = 0; i < TempNum; i++)
        {
            for (; z < TempList.Length;)
            {
                if (i < player.Count)
                    TempList[z] = player[i];
                else
                    TempList[z] = 0;
                z++;
                break;
            }
        }
        string Paixu = "";
        for (int i = 0; i < TempList.Length; i++)
        {
            Paixu += TempList[i] + "  ";
        }
        Debug.Log("排序：" + Paixu);
        return TempList;
    }
    void GetRoomNum(uint roomNum)
    {
        RoomNum.text = "房间号：" + roomNum.ToString();
    }
    public void ClientReady()
    {
        PublicEvent.GetINS.Fun_SentClientPre();
    }
    void PressGuo()
    {
        PublicEvent.GetINS.Fun_SentGuo(0);
        Chi.gameObject.SetActive(false);
        Ting.gameObject.SetActive(false);
        Hu.gameObject.SetActive(false);
        Guo.gameObject.SetActive(false);

        int p = SetChiArea.childCount;
        for (int i = 0; i < p; i++)
        {
            Destroy(SetChiArea.GetChild(i).gameObject);
        }

    }
    void SetChiCard(List<ProtoBuf.ChiPaiOP> chi, uint card)
    {
        int p = SetChiArea.childCount;
        for (int i = 0; i < p; i++)
        {
            Destroy(SetChiArea.GetChild(i).gameObject);
        }
        for (int i = 0; i < chi.Count; i++)
        {
            TempCard = Instantiate(ChiCard);
            TempCard.name = chi[i].chitype.ToString();
            uint z = chi[i].chitype;
            TempCard.GetComponent<Button>().onClick.AddListener(delegate
            {
                Debug.Log("发送吃牌！");
                SentChiCard(z, card);
                int pi = SetChiArea.childCount;
                for (int times = 0; times < pi; times++)
                {
                    Destroy(SetChiArea.GetChild(0).gameObject);
                }
            });
            TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chi[i].cardo.ToDzpCard();
            TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chi[i].cardt.ToDzpCard();
            TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chi[i].cards.ToDzpCard();
            TempCard.SetActive(true);
            TempCard.transform.SetParent(SetChiArea, false);
        }
    }
    void SetChiCard(List<ProtoBuf.ChiPai> chi, uint card)
    {
        int p = SetChiArea.childCount;
        for (int i = 0; i < p; i++)
        {
            Destroy(SetChiArea.GetChild(i).gameObject);
        }
        for (int i = 0; i < chi.Count; i++)
        {
            TempCard = Instantiate(ChiCard);
            TempCard.name = chi[i].chitype.ToString();
            uint z = chi[i].chitype;
            TempCard.GetComponent<Button>().onClick.AddListener(delegate
            {
                Debug.Log("发送吃牌！");
                SentChiCard(z, card);
                int pi = SetChiArea.childCount;
                for (int times = 0; times < pi; times++)
                {
                    Destroy(SetChiArea.GetChild(0).gameObject);
                }
            });
            TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chi[i].cardo.ToDzpCard();
            TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chi[i].cardt.ToDzpCard();
            TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chi[i].cards.ToDzpCard();
            TempCard.SetActive(true);
            TempCard.transform.SetParent(SetChiArea, false);
        }
    }
    void SentChiCard(uint ChiType, uint card)
    {
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_CHI;
        pack.oricharId = Orcharid;
        pack.cardId = card;
        pack.charId = BaseProto.playerInfo.m_id;
        pack.param = ChiType;
        MJProto.Inst().MJGameOPRequest(pack);
    }

    uint HuOrCharID, HuCard = 0;
    void PressHu()
    {
        Guo.gameObject.SetActive(false);
        Hu.gameObject.SetActive(false);
        Debug.Log("胡的人:" + HuOrCharID + "  " + "胡的牌：" + HuCard);
        PublicEvent.GetINS.Fun_SentHu(HuCard, HuOrCharID);
    }
    void PressTing()
    {
        Guo.gameObject.SetActive(false);
        Ting.gameObject.SetActive(false);
        MJGameOpReq pack = new MJGameOpReq();
        pack.op = MJGameOP.MJ_OP_TING;
        pack.oricharId = 0;
        pack.cardId = 0;
        pack.charId = BaseProto.playerInfo.m_id;
        MJProto.Inst().MJGameOPRequest(pack);
    }
    void GetPlayerReady(uint player)
    {
        switch (DataManage.Instance.PData_GetIndex(player))
        {
            case 0:
                Ready.gameObject.SetActive(false);
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }


    ///测试
    ///CardArray
    public CardArray[] HandCard;
    /// <summary>
    /// 万一有用呢
    /// </summary>
    public uint[,] HandCardValue = new uint[10, 4];
    public GameObject PopCard = null;
    public Transform MyPopArea;
    public Transform LeftPopArea;
    public Transform RightPopArea;

    public Transform MyPengArea;
    public Transform LeftPengArea;
    public Transform RightPengArea;
    public Transform SetChiArea;

    public bool CanPopCard = false;
    CardInfo TempCardNum;

    public GameObject PengCard = null;
    public GameObject LongCard = null;
    public GameObject ChiCard = null;
    public GameObject AnimCard = null;


    /// <summary>
    /// 超出长度的牌
    /// </summary>
    public List<uint> OtherCard = new List<uint>();
    public List<string> BlackCard = new List<string>();
    string HandCardsLog = "";
    public void GetPlayerHandCard(uint doer, List<uint> cards)
    {
        BlackCard.Clear();
        int SameCount = 0;
        cards.Sort();
        ResetHandCard();
        OtherCard.Clear();


        for (int i = 0; i < cards.Count; i++)
        {
            HandCardsLog += "  " + cards[i];
            for (int z = 0; z < cards.Count; z++)
            {
                if (cards[i] == cards[z])
                {
                    SameCount++;
                    if (SameCount > 2)
                    {
                        BlackCard.Add(cards[i].ToString());
                        SameCount = 0;
                        //break;
                    }
                }
                if (z == cards.Count - 1)
                    SameCount = 0;


            }
        }
        Debug.LogWarning(HandCardsLog);
        for (int i = 0; i < cards.Count; i++)
        {
            TempCardNum = cards[i].ToCardInfo();
            CardArray t = HandCard[TempCardNum.num - 1];
            for (int z = 0; z < 4; z++)
            {
                if (t.card[z].name == "0")
                {
                    SetMyHandCard(t.card[z].transform, cards[i]);
                    break;
                }

                if (z == 3)
                {
                    OtherCard.Add(cards[i]);
                    break;
                }
            }
        }
        for (int i = 0; i < OtherCard.Count; i++)
        {
            PrivateGetBuPai(0, OtherCard[i]);
        }

        SortCard();

    }

    IEnumerator SetBlackCard()
    {
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++)
        {
            for (int z = 0; z < 4; z++)
            {
                for (int p = 0; p < BlackCard.Count; p++)
                {
                    Debug.LogWarning("黑名单：" + BlackCard[p]);
                    if (HandCard[i].card[z].name == BlackCard[p])
                    {
                        HandCard[i].card[z].GetComponent<BoxCollider2D>().enabled = false;
                        HandCard[i].card[z].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                        Debug.LogWarning("禁止:" + HandCard[i].card[z].name);
                    }
                }
            }
        }
    }

    public void PrivateGetBuPai(uint doer, uint card)
    {
        bool Get = false;
        for (int i = 0; i < 10; i++)
        {
            if (!Get)
            {
                for (int z = 0; z < 4; z++)
                {
                    GameObject t = HandCard[i].card[z];
                    if (t.name == "0")
                    {
                        SetMyHandCard(HandCard[i].card[z].transform, card);
                        Get = true;
                        Debug.Log("补牌：" + i + " " + z);
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }
    }
    public void GetBuPai(uint doer, uint card)
    {
        bool Get = false;
        for (int i = 0; i < 10; i++)
        {
            if (!Get)
            {
                for (int z = 0; z < 4; z++)
                {
                    GameObject t = HandCard[i].card[z];
                    if (t.name == "0")
                    {
                        SetMyHandCard(HandCard[i].card[z].transform, card);
                        Get = true;
                        Debug.Log("补牌：" + i + " " + z);
                        break;
                    }
                }
            }
            else
            {
                break;
            }
        }

    }
    /// <summary>
    /// 重置牌面
    /// </summary>
    void ResetHandCard()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int z = 0; z < 4; z++)
            {
                var t = HandCard[i].card[z].transform;
                if (t != null)
                {
                    t.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    t.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    t.GetComponent<BoxCollider2D>().enabled = true;
                    t.name = "0";
                }
            }
        }
    }
    /// <summary>
    /// 交换手牌
    /// </summary>
    /// <param name="ThisCard"></param>
    /// <param name="TargetCard"></param>
    public void SwapCard(GameObject ThisCard, GameObject TargetCard)
    {
        //ThisCard
        //Image TempObj = null;
        //float AlphaColor = 0.1f;

        string Tempcardid = null;
        if (ThisCard != null && TargetCard != null)
        {
            Tempcardid = ThisCard.name;
            ThisCard.name = TargetCard.name;
            if (ThisCard.name == "0")
            {
                SetMyHandCard(ThisCard.transform, 0);
            }
            else
            {
                uint result;
                uint.TryParse(ThisCard.name, out result);
                SetMyHandCard(ThisCard.transform, result);
            }
            TargetCard.name = Tempcardid;
            if (TargetCard.name == "0")
            {
                SetMyHandCard(TargetCard.transform, 0);
            }
            else
            {

                uint result;
                uint.TryParse(TargetCard.name, out result);
                SetMyHandCard(TargetCard.transform, result);
            }
        }
    }
    /// <summary>
    /// 整理手牌
    /// </summary>
    public void SortCard()
    {
        ///纵向整理
        for (int i = 0; i < HandCard.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (HandCard[i].card[j].name == "0" && HandCard[i].card[1 + j].name != "0")
                {
                    SwapCard(HandCard[i].card[1 + j].gameObject, HandCard[i].card[j].gameObject);
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (HandCard[i].card[j].name == "0" && HandCard[i].card[1 + j].name != "0")
                {
                    SwapCard(HandCard[i].card[1 + j].gameObject, HandCard[i].card[j].gameObject);
                }
            }
            for (int j = 0; j < 3; j++)
            {
                if (HandCard[i].card[j].name == "0" && HandCard[i].card[1 + j].name != "0")
                {
                    SwapCard(HandCard[i].card[1 + j].gameObject, HandCard[i].card[j].gameObject);
                }
            }
        }
        //横向整理
        //for (int i = 0; i < HandCard.Length - 1; i++)
        //{
        //    if (HandCard[i].card[0].name == "0")
        //    {
        //        for (int z = 0; z < 4; z++)
        //        {
        //            SwapCard(HandCard[i].card[z].gameObject, HandCard[(1 + i)].card[z].gameObject);
        //        }
        //    }
        //}
        for (int p = 0; p < 4; p++)
        {
            for (int i = 0; i < HandCard.Length - 1; i++)
            {
                if (HandCard[i].card[0].name == "0")
                {
                    for (int z = 0; z < 4; z++)
                    {
                        SwapCard(HandCard[i].card[z].gameObject, HandCard[(1 + i)].card[z].gameObject);
                    }
                }
            }
        }
        StartCoroutine(SetBlackCard());
    }
    internal void Open_Player_ZhunBei(uint charId)
    {
        Debug.Log("某玩家已经准备了！");
    }
    GameObject TempCard = null;
    RaycastHit RatHit;
    public GameObject TouchCard = null;
    uint FirstTouchCard = 0;
    RaycastHit2D Firsthit;
    RaycastHit2D Sescondhit;
    bool Pressed = false;
    void GetMouse()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            Pressed = true;
            TouchCard.gameObject.SetActive(false);
            TouchCard.transform.GetChild(0).GetComponent<Image>().sprite = null;
            TouchCard.GetComponent<RectTransform>().anchoredPosition = Vector2.one;
            TouchCard.name = "0";
            FirstTouchCard = 0;
            Debug.Log("按下");
            Firsthit = new RaycastHit2D();
            if (TouchCard.GetComponent<RectTransform>().anchoredPosition.y * 2 < GameManager.GM.Canvas.GetComponent<RectTransform>().sizeDelta.y)
            {
                Firsthit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
                if (Firsthit.collider != null)
                {
                    Debug.Log(Firsthit.transform.name);
                    uint.TryParse(Firsthit.transform.name, out FirstTouchCard);
                    if (TouchCard != null && !TouchCard.activeSelf && FirstTouchCard > 0 && Firsthit.transform.name != "0")
                    {

                        var t = TouchCard.transform.GetChild(0).GetComponent<Image>();
                        t.sprite = FirstTouchCard.ToDzpCard();
                        if (t.sprite == null)
                        {
                            TouchCard.SetActive(false);
                            TouchCard.name = "none";
                            t.SetNativeSize();
                        }
                        else
                        {
                            TouchCard.SetActive(true);
                            TouchCard.name = FirstTouchCard.ToString();
                            t.SetNativeSize();
                        }

                    }
                }
            }
        }
        if (Input.GetButton("Fire1"))
        {
            TouchCard.GetComponent<RectTransform>().anchoredPosition = new Vector2((Input.mousePosition.x * GameManager.GM.Canvas.GetComponent<RectTransform>().sizeDelta.x) / Screen.width, (Input.mousePosition.y * GameManager.GM.Canvas.GetComponent<RectTransform>().sizeDelta.y) / Screen.height);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("抬起");
            if (TouchCard != null && TouchCard.activeSelf && TouchCard.name != "0")
            {
                TouchCard.SetActive(false);
                if (Pressed)
                {
                    Pressed = false;
                    if (TouchCard.GetComponent<RectTransform>().anchoredPosition.y * 2 > GameManager.GM.Canvas.GetComponent<RectTransform>().sizeDelta.y)
                    {
                        uint.TryParse(TouchCard.name, out FirstTouchCard);
                        if (CanPopCard && FirstTouchCard > 0)
                        {
                            CanPopCard = false;
                            Debug.Log("打出这张牌！");
                            PublicEvent.GetINS.Fun_SentPopCard(FirstTouchCard);
                            DelMyHandCard(Firsthit.collider.gameObject);
                            SortCard();
                        }
                    }
                    else
                    {
                        Debug.Log("中置线以下交换牌");
                        Sescondhit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
                        if (Sescondhit.collider != null && Firsthit.collider != null)
                        {
                            Debug.Log("第一次碰触：  " + Firsthit.collider.gameObject.name + "第二次碰触：  " + Sescondhit.collider.gameObject.name);
                            SwapCard(Firsthit.collider.gameObject, Sescondhit.collider.gameObject);
                            SortCard();
                        }
                    }
                }
            }
        }
        TouchPressed = false;
    }

    Tween PopCardPlayer1;
    GameObject LastPopedCard = null;
    /// <summary>
    /// 向目标区域发送一张打出去的牌
    /// </summary>
    void GetPopedCard(uint doer, uint card)
    {
        CanPutDownLastCard = true;
        if (LastPopedCard != null)
            LastPopedCard.SetActive(true);
        var temp = Instantiate(PopCard).transform;
        temp.name = card.ToString();
        temp.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        temp.GetChild(0).GetComponent<Image>().SetNativeSize();
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
        {
            case 0:
                temp.transform.SetParent(MyPopArea, false);
                break;
            case 1:
                temp.transform.SetParent(RightPopArea, false);
                break;
            case 2:
                temp.transform.SetParent(LeftPopArea, false);
                break;
            default:
                break;
        }
        temp.gameObject.SetActive(true);
        LastPopedCard = temp.gameObject;
        StartCoroutine(PopCardAnim(card, DZP_PublicEvent.GetINS.GetPlayerNum(doer)));
        SortCard();
    }
    void GetPengGangCard(uint doer, uint card, uint orcharid, bool IsGang = true, bool ReGet = false)
    {
        if (!ReGet || orcharid != 0)
            DelPopedCard(orcharid, card);
        var temp = Instantiate(PengCard).transform;
        temp.gameObject.SetActive(true);
        temp.name = card.ToString();
        temp.GetChild(0).gameObject.SetActive(true);
        temp.GetChild(0).GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        temp.GetChild(1).gameObject.SetActive(true);
        temp.GetChild(1).GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        temp.GetChild(2).gameObject.SetActive(true);
        temp.GetChild(2).GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        if (IsGang)
        {
            temp.GetChild(3).gameObject.SetActive(true);
            temp.GetChild(3).GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        }
        else
            temp.GetChild(3).gameObject.SetActive(false);
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
        {
            case 0:
                if (!ReGet)
                {
                    DelMyHandCard(card);
                    DelMyHandCard(card);
                    DelMyHandCard(card);
                    DelMyHandCard(card);
                }
                temp.SetParent(MyPengArea, false);
                break;
            case 1:
                temp.SetParent(RightPengArea, false);
                break;
            case 2:
                temp.SetParent(LeftPengArea, false);
                break;
            default:
                break;
        }
        if (!ReGet)
        {
            StartCoroutine(MoveToTarget(temp, card, DZP_PublicEvent.GetINS.GetPlayerNum(doer)));
            SortCard();
        }
    }
    void GetLongCard(uint doer, uint card, uint orcharid, bool ReGet = false)
    {
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
        {
            case 0:
                var temp = Instantiate(PengCard).transform;
                temp.gameObject.SetActive(true);
                temp.name = card.ToString();
                for (int i = 0; i < 4; i++)
                {
                    temp.GetChild(i).gameObject.SetActive(true);
                    temp.GetChild(i).GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
                    temp.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                    if (!ReGet)
                        DelMyHandCard(card);
                }
                temp.SetParent(MyPengArea, false);
                if (!ReGet)
                    StartCoroutine(MoveToTarget(temp, card, DZP_PublicEvent.GetINS.GetPlayerNum(doer)));
                break;
            case 1:
                var temp1 = Instantiate(LongCard).transform;
                temp1.gameObject.SetActive(true);
                temp1.SetParent(RightPengArea, false);
                break;
            case 2:
                var temp2 = Instantiate(LongCard).transform;
                temp2.gameObject.SetActive(true);
                temp2.SetParent(LeftPengArea, false);
                break;
            default:
                break;
        }

        SortCard();
    }
    //重入的时候的吃牌
    void GetChiCard(uint doer, uint oricharId, List<ProtoBuf.ChiPai> chipaip, uint card)
    {
        Debug.Log(chipaip.Count);
        Debug.Log("重入的时候的吃" + "   " + "目标人:" + doer);
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
        {
            case 0:
                int t = SetChiArea.childCount;
                for (int i = 0; i < t; i++)
                {
                    Destroy(SetChiArea.GetChild(i).gameObject);
                }
                Guo.gameObject.SetActive(false);
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(MyPengArea, false);
                }
                break;
            case 1:
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(RightPengArea, false);
                }
                break;
            case 2:
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(LeftPengArea, false);
                }
                break;
            default:
                break;
        }
    }
    void GetChiCard(uint doer, uint oricharId, List<ProtoBuf.ChiPaiOP> chipaip, uint card)
    {
        if (oricharId != 0 || card != 0)
            DelPopedCard(oricharId, card);
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
        {
            case 0:
                int t = SetChiArea.childCount;
                for (int i = 0; i < t; i++)
                {
                    Destroy(SetChiArea.GetChild(i).gameObject);
                }
                Guo.gameObject.SetActive(false);
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    DelMyHandCard(chipaip[i].cardo);
                    DelMyHandCard(chipaip[i].cards);
                    DelMyHandCard(chipaip[i].cardt);
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(MyPengArea, false);
                }
                break;
            case 1:
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(RightPengArea, false);
                }
                break;
            case 2:
                for (int i = 0; i < chipaip.Count; i++)
                {
                    TempCard = Instantiate(PengCard);
                    TempCard.name = chipaip[i].chitype.ToString();
                    int z = i;
                    TempCard.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardo.ToDzpCard();
                    TempCard.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cardt.ToDzpCard();
                    TempCard.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = chipaip[i].cards.ToDzpCard();
                    TempCard.transform.GetChild(3).gameObject.SetActive(false);
                    TempCard.SetActive(true);
                    TempCard.transform.SetParent(LeftPengArea, false);
                }
                break;
            default:
                break;
        }
        StartCoroutine(MoveToTarget(TempCard, card, DZP_PublicEvent.GetINS.GetPlayerNum(doer)));
        SortCard();
    }
    bool CanPutDownLastCard = false;
    IEnumerator PopCardAnim(uint card, int Player)
    {
        PopCardPlayer1.Kill();
        AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
        switch (Player)
        {
            case 0:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 190);
                break;
            case 2:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(-488, 278);//(-458, 272);
                break;
            case 1:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(488, 278);//(458, 272);
                break;
            default:
                break;
        }
        AnimCard.transform.localScale = Vector3.one;
        if (card != 0)
        {
            AnimCard.SetActive(true);
            AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
            AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
        }
        do
        {
            yield return null;
        } while (CanPutDownLastCard);
        if (CanPutDownLastCard)
            AnimCard.SetActive(false);
        else
            AnimCard.SetActive(true);
    }
    IEnumerator MoveToTarget(GameObject targetObj, uint card, int Player)
    {
        AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
        //switch (Player)
        //{
        //    case 0:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(230, 45);
        //        break;
        //    case 2:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);//(-458, 272);
        //        break;
        //    case 1:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);//(458, 272);
        //        break;
        //    default:
        //        break;
        //}
        AnimCard.transform.localScale = Vector3.one;
        if (card != 0 && targetObj != null)
        {
            AnimCard.SetActive(true);
            AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
            AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
            yield return new WaitForSeconds(0.8f);
            //Debug.Log("目标位置：" + TargetObj.transform.position + "       " + "起始的卡牌位置" + AnimCard.transform.position);
            PopCardPlayer1 = AnimCard.transform.DOMove(targetObj.transform.position, 0.3f);
            AnimCard.transform.DOScale(0.5f, 0.3f);
            PopCardPlayer1.OnComplete(delegate { AnimCard.SetActive(false); });
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator MoveToTarget(Transform targetObj, uint card, int Player)
    {
        AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
        //switch (Player)
        //{
        //    case 0:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(230, 45);
        //        break;
        //    case 2:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);//(-458, 272);
        //        break;
        //    case 1:
        //        AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);//(458, 272);
        //        break;
        //    default:
        //        break;
        //}
        AnimCard.transform.localScale = Vector3.one;
        if (card != 0 && targetObj != null)
        {
            AnimCard.SetActive(true);
            AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
            AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
            yield return new WaitForSeconds(0.8f);
            //Debug.Log("目标位置：" + TargetObj.position + "       " + "起始的卡牌位置" + AnimCard.transform.position);
            PopCardPlayer1 = AnimCard.transform.DOMove(targetObj.transform.position, 0.3f);
            AnimCard.transform.DOScale(0.5f, 0.3f);
            PopCardPlayer1.OnComplete(delegate { AnimCard.SetActive(false); });
            yield return new WaitForSeconds(0.5f);
        }
    }
    Tween MoCard1, MoCard2, MoCard3;
    void GetPlayerMocardAnim(uint doer, uint card)
    {
        MoCard1.Kill();
        MoCard2.Kill();
        MoCard3.Kill();
        if (card != 0 && doer != 0)
        {
            AnimCard.SetActive(true);
            AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
            AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
            AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 278);
            AnimCard.GetComponent<RectTransform>().localScale = Vector3.one;


            switch (DZP_PublicEvent.GetINS.GetPlayerNum(doer))
            {
                case 0:
                    AnimCard.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                    MoCard1 = AnimCard.transform.DOLocalMoveY(190, 0.3f);
                    MoCard1.OnComplete(delegate
                    {
                        AnimCard.SetActive(false);
                        DZP_PublicEvent.GetINS.Fun_PlayerPopCard(doer, card);
                    });
                    break;
                case 1:
                    AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                    MoCard2 = AnimCard.transform.DOLocalMoveX(488, 0.3f);
                    MoCard2.OnComplete(delegate
                    {
                        AnimCard.SetActive(false);
                        DZP_PublicEvent.GetINS.Fun_PlayerPopCard(doer, card);
                    });
                    break;
                case 2:
                    AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                    MoCard3 = AnimCard.transform.DOLocalMoveX(-488, 0.3f);
                    MoCard3.OnComplete(delegate
                    {
                        AnimCard.SetActive(false);
                        DZP_PublicEvent.GetINS.Fun_PlayerPopCard(doer, card);
                    });
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 删除手牌
    /// </summary>
    /// <param name="Card"></param>
    void DelMyHandCard(uint Card)
    {
        TempCardNum = Card.ToCardInfo();
        uint tempnum;
        for (int i = 0; i < 10; i++)
        {
            for (int z = 0; z < 4; z++)
            {
                if (HandCard[i].card[z].name != "0")
                {
                    uint.TryParse(HandCard[i].card[z].name, out tempnum);
                    if (tempnum > 0)
                    {
                        if (tempnum == Card)
                        {
                            HandCard[i].card[z].transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0);
                            HandCard[i].card[z].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0);
                            HandCard[i].card[z].transform.name = "0";
                            break;
                        }
                    }
                }
            }

        }
    }
    /// <summary>
    /// 删除手牌
    /// </summary>
    /// <param name="TargetCardObj"></param>
    void DelMyHandCard(GameObject TargetCardObj)
    {
        TargetCardObj.transform.GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0);
        TargetCardObj.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0);
        TargetCardObj.transform.name = "0";
    }
    /// <summary>
    /// 设定当前这张牌
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="value"></param>
    void SetMyHandCard(Transform Target, uint value)
    {
        if (value < 1)
        {
            Target.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0f);
            Target.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Target.GetChild(0).GetComponent<Image>().sprite = null;
            Target.name = "0";
            Target.gameObject.SetActive(true);
        }
        else
        {
            Target.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);
            Target.GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);
            Target.GetChild(0).GetComponent<Image>().sprite = value.ToDzpCard();
            Target.name = value.ToString();
            Target.gameObject.SetActive(true);
        }
        Target.GetComponent<BoxCollider2D>().enabled = true;
        Target.parent.gameObject.SetActive(true);
    }
    void DelPengGangCard(uint card)
    {
        for (int i = 0; i < MyPengArea.childCount; i++)
        {
            if (MyPengArea.GetChild(i).name == card.ToString())
            {
                Destroy(MyPengArea.GetChild(i).gameObject);
                break;
            }
        }
    }
    void DelPopedCard(uint charid, uint card)
    {
        Destroy(LastPopedCard);
        //int z = 0;
        //switch (DZP_PublicEvent.GetINS.GetPlayerNum(charid))
        //{
        //    case 0:
        //        if (MyPopArea.childCount > 0)
        //        {
        //            Debug.Log("本地出牌区域有牌");
        //            z = MyPopArea.childCount - 1;
        //            if (card.ToString() == MyPopArea.GetChild(z).gameObject.name)
        //            {
        //                Debug.Log("删除牌:" + card + "  目标牌" + MyPopArea.GetChild(z).gameObject.name);
        //                Destroy(MyPopArea.GetChild(z).gameObject);
        //            }
        //        }
        //        break;
        //    case 1:
        //        if (RightPopArea.childCount > 0)
        //        {
        //            Debug.Log("右边出牌区域有牌");
        //            z = RightPopArea.childCount - 1;
        //            if (card.ToString() == RightPopArea.GetChild(z).gameObject.name)
        //            {
        //                Debug.Log("删除牌:" + card + "  目标牌" + RightPopArea.GetChild(z).gameObject.name);
        //                Destroy(RightPopArea.GetChild(RightPopArea.childCount - 1).gameObject);
        //            }
        //        }
        //        break;
        //    case 2:
        //        if (LeftPopArea.childCount > 0)
        //        {
        //            Debug.Log("左边出牌区域有牌");
        //            z = LeftPopArea.childCount - 1;
        //            if (card.ToString() == LeftPopArea.GetChild(z).gameObject.name)
        //            {
        //                Debug.Log("删除牌:" + card + "  目标牌" + LeftPopArea.GetChild(z).gameObject.name);
        //                Destroy(LeftPopArea.GetChild(LeftPopArea.childCount - 1).gameObject);
        //            }
        //        }
        //        break;
        //    default:
        //        break;
        //}

    }
    public void AskPlayerPop(uint Doer)
    {
        switch (DZP_PublicEvent.GetINS.GetPlayerNum(Doer))
        {
            case 0:
                CanPopCard = true;
                break;
            case 1:
                CanPopCard = false;
                break;
            case 2:
                CanPopCard = false;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 缓存当前的吃牌
    /// </summary>
    List<ChiPaiOP> ChiCardType = new List<ChiPaiOP>();
    List<ChiPai> Re_ChiCardType = new List<ChiPai>();
    uint Orcharid = 0;
    void AskClientChiCard(uint doer, uint orcharid, List<ProtoBuf.ChiPaiOP> chi, uint card)
    {
        CanPutDownLastCard = false;
        AnimCard.SetActive(true);
        AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
        if (doer == BaseProto.playerInfo.m_id)
        {
            ChiCardType.Clear();
            ChiCardType = chi;
            Orcharid = 0;
            Orcharid = orcharid;
            //Chi.gameObject.SetActive(true);
            SetChiCard(chi, card);
            Guo.gameObject.SetActive(true);
        }
    }

    void AskClientChiCard(uint doer, uint orcharid, List<ProtoBuf.ChiPai> chi, uint card)
    {
        CanPutDownLastCard = false;
        AnimCard.SetActive(true);
        AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = card.ToDzpCard();
        AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = card.ToDzpCard();
        if (doer == BaseProto.playerInfo.m_id)
        {
            Re_ChiCardType.Clear();
            Re_ChiCardType = chi;
            Orcharid = 0;
            Orcharid = orcharid;
            //Chi.gameObject.SetActive(true);
            SetChiCard(chi, card);
            Guo.gameObject.SetActive(true);
        }
    }
    void AskClientHuCard(uint doer, uint OrCharID, uint card, bool IsZimo = false)
    {
        CanPutDownLastCard = false;
        if (doer == BaseProto.playerInfo.m_id)
        {
            Debug.Log("胡！");
            Hu.gameObject.SetActive(true);
            Guo.gameObject.SetActive(true);
            HuOrCharID = OrCharID;
            HuCard = card;
        }
    }
    void AskTingCards(uint doer, List<uint> Tingcards)
    {
        if (doer == BaseProto.playerInfo.m_id)
        {
            Ting.gameObject.SetActive(true);
            Guo.gameObject.SetActive(true);
        }
    }
    public GameObject p1 = null, p2 = null, p3 = null;
    void SwitchplayerLight(uint doer)
    {
        int Num = 0;
        Num = DZP_PublicEvent.GetINS.GetPlayerNum(doer);
        switch (Num)
        {
            case 0:
                p1.SetActive(true);
                p2.SetActive(false);
                p3.SetActive(false);
                break;
            case 1:
                p2.SetActive(true);
                p1.SetActive(false);
                p3.SetActive(false);
                break;
            case 2:
                p3.SetActive(true);
                p1.SetActive(false);
                p2.SetActive(false);
                break;
            default:
                break;
        }
    }
    bool TouchPressed = false;

    public int junum { get; set; }
    public bool IsFirstRoundOver = false;
    public bool CanGameOver = false;

    void Update()
    {
        GetMouse();
    }
    //void FixedUpdate()
    //{
    //    //if (Input.GetButtonUp("Fire1")|| Input.GetButton("Fire1")|| Input.GetButtonDown("Fire1"))
    //    //{
    //    //    TouchPressed = true;
    //    //}
    //    //if (TouchPressed)

    //}
    //nausea dirty work
    void RestGame(ProtoBuf.MJGameOver rsp)
    {
        AnimCard.SetActive(false);
        int teamp = MyPengArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(MyPengArea.GetChild(i).gameObject);
        }
        teamp = LeftPengArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(LeftPengArea.GetChild(i).gameObject);
        }
        teamp = RightPengArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(RightPengArea.GetChild(i).gameObject);
        }
        ///打出的跑
        teamp = MyPopArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(MyPopArea.GetChild(i).gameObject);
        }
        teamp = LeftPopArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(LeftPopArea.GetChild(i).gameObject);
        }
        teamp = RightPopArea.childCount;
        for (int i = 0; i < teamp; i++)
        {
            Destroy(RightPopArea.GetChild(i).gameObject);
        }
        ResetHandCard();
        //for (int i = 0; i < 10; i++)
        //{
        //    for (int z = 0; z < 4; z++)
        //    {
        //        HandCards[i,z].name = "0";
        //        HandCards[i,z].transform.GetChild(0).GetComponent<Image>().sprite = null;
        //        HandCards[i, z].transform.GetChild(0).gameObject.SetActive(false);
        //        HandCards[i,z].GetComponent<Image>().enabled=false;
        //    }
        //}
        p1.SetActive(false);
        p2.SetActive(false);
        p3.SetActive(false);
        Ready.gameObject.SetActive(true);

        uiWin_DzpGameOver tempGameOver = UIManager.Instance.ShowUI(AllPrefabName.uiWin_DzpGameOver, UIManager.Instance.canvas_T).GetComponent<uiWin_DzpGameOver>();
        tempGameOver.ReciveRsp(rsp);
    }

}
