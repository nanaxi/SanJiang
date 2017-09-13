using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

[Serializable]
public struct Players
{
    public Image playerHead;
    public Text playerName;
    public Text playerGold;
    public GameObject Self;
    public Text ID;
}
public class UiWin_DzpRePlayRoom : MonoBehaviour {
    public ProtoBuf.MJRoundRecord RoomRsp = null;
    public Text RoomNum = null;
    //public Image Player1 = null, Player2 = null, Player3 = null;
    //public Text Player1Name;
    public Button Play = null, Pause = null, Quit = null;
    public Text CardNum = null;
    public Players[] Player = new Players[3];
    //手牌
    public List<GameObject> MyHandCard = new List<GameObject>();
    public List<GameObject> LeftHandCard = new List<GameObject>();
    public List<GameObject> RightHandCard = new List<GameObject>();
    /// <summary>
    /// 手牌数据
    /// </summary>
    List<uint> MyhandCardList = new List<uint>();
    List<uint> LefthandCardList = new List<uint>();
    List<uint> RighthandCardList = new List<uint>();
    public Transform MyPopCardArea = null;
    public Transform LeftPopCardArea = null;
    public Transform RightPopCardArea = null;
    public Transform MyPGArea = null;
    public Transform LeftPGArea = null;
    public Transform RightPGArea = null;
    /// <summary>
    /// 动画用的牌
    /// </summary>
    public GameObject AnimCard = null;
    /// <summary>
    /// 打出去的手牌
    /// </summary>
    public GameObject PopCard = null;
    /// <summary>
    /// 碰 明杠
    /// </summary>
    public GameObject PengGangCard = null;
    void Awake()
    {
        Default();
    }
    // Use this for initialization
    void Start()
    {
        transform.SetAsLastSibling();
    }
    void Default()
    {
        if (Play != null)
            Play.onClick.AddListener(delegate { IsPlay = true; });
        if (Pause != null)
            Pause.onClick.AddListener(delegate { IsPlay = false; });
        if (Quit != null)
            Quit.onClick.AddListener(QuitToMain);
    }
    bool IsPlay = true;
    void QuitToMain()
    {
        GameManager.GM.Game_RetrueHome();
        Destroy(this.gameObject);
    }
    public void SetFlashDate(ProtoBuf.MJRoundRecord rsp)
    {
        RoomRsp = rsp;
        if (RoomRsp != null)           
        {
            Debug.Log("得到数据");
            RoomNum.text = rsp.playBack.roomId.ToString();
            for (int i = 2; i >= 0; i--)
            {
                //DataManage.Instance.Head_GetSprite(rsp.players[i].portrait, SetHead, i);
                Player[i].playerName.text = "名字：" + rsp.players[i].name;
               // Player[i].playerGold.text = "积分：" + rsp.players[i].gold.ToString();
                Player[i].ID.text = "ID:" + rsp.players[i].charId;

                for (int z = 0; z < rsp.playBack.players[i].handCards.Count; z++)
                {
                    if (i == 0)
                        MyhandCardList.Add(rsp.playBack.players[0].handCards[z]);
                    if (i == 1)
                        LefthandCardList.Add(rsp.playBack.players[1].handCards[z]);
                    if (i == 2)
                        RighthandCardList.Add(rsp.playBack.players[2].handCards[z]);
                }


            }
            MyhandCardList.Sort();
            LefthandCardList.Sort();
            RighthandCardList.Sort();
            if (MyhandCardList.Count == 21)
                Player[0].Self.SetActive(true);
            if (LefthandCardList.Count == 21)
                Player[1].Self.SetActive(true);
            if (RighthandCardList.Count == 21)
                Player[2].Self.SetActive(true);
            ReFreshHandCard();
            StartCoroutine("PlayFlashBack");
        }
    }
    void ReFreshHandCard()
    {

        for (int i = 0; i < MyHandCard.Count; i++)
        {
            MyHandCard[i].SetActive(false);
            MyHandCard[i].transform.parent.gameObject.SetActive(false);
            MyHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            MyHandCard[i].name = "none";
        }

        for (int i = 0; i < LeftHandCard.Count; i++)
        {
            LeftHandCard[i].SetActive(false);
            LeftHandCard[i].transform.parent.gameObject.SetActive(false);
            LeftHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            LeftHandCard[i].name = "none";
        }

        for (int i = 0; i < RightHandCard.Count; i++)
        {
            RightHandCard[i].SetActive(false);
            RightHandCard[i].transform.parent.gameObject.SetActive(false);
            RightHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            RightHandCard[i].name = "none";
        }
        MyhandCardList.Sort();
        LefthandCardList.Sort();
        RighthandCardList.Sort();
        for (int i = 0; i < MyhandCardList.Count; i++)
        {
            int te = MyhandCardList[i].ToCardInfo().num;
            for (int z = 0; z < 4; z++)
            {
                if (te > 100)
                    break;
                if (MyHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite == null)
                {
                    MyHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(MyhandCardList[i]);
                    MyHandCard[z * 10 + te - 1].SetActive(true);
                    MyHandCard[z * 10 + te - 1].transform.parent.gameObject.SetActive(true);
                    if (MyhandCardList[i].ToCardInfo().isMax)
                        MyHandCard[z * 10 + te - 1].name = "A" + te.ToString();
                    else
                        MyHandCard[z * 10 + te - 1].name = te.ToString();
                    break;
                }
                if (z == 3)
                {
                    ///超出长度，没法安置
                    MyhandCardList[i] = 100 * MyhandCardList[i];
                    break;
                }
            }
        }
        for (int z = 0; z < MyhandCardList.Count; z++)
        {
            if (MyhandCardList[z] > 100)
            {
                MyhandCardList[z] = MyhandCardList[z] / 100;
                for (int i = 0; i < MyHandCard.Count; i++)
                {
                    if (MyHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite == null)

                    {
                        int te = MyhandCardList[z].ToCardInfo().num;
                        MyHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(MyhandCardList[z]);
                        MyHandCard[i].SetActive(true);
                        MyHandCard[i].transform.parent.gameObject.SetActive(true);
                        if (MyhandCardList[z].ToCardInfo().isMax)
                            MyHandCard[i].name = "A" + te.ToString();
                        else
                            MyHandCard[i].name = te.ToString();
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < LefthandCardList.Count; i++)
        {
            int te = LefthandCardList[i].ToCardInfo().num;
            for (int z = 0; z < 4; z++)
            {
                if (te > 100)
                    break;
                if (LeftHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite == null)
                {
                    LeftHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(LefthandCardList[i]);
                    LeftHandCard[z * 10 + te - 1].SetActive(true);
                    LeftHandCard[z * 10 + te - 1].transform.parent.gameObject.SetActive(true);
                    if (LefthandCardList[i].ToCardInfo().isMax)
                        LeftHandCard[z * 10 + te - 1].name = "A" + te.ToString();
                    else
                        LeftHandCard[z * 10 + te - 1].name = te.ToString();
                    break;
                }
                if (z == 3)
                {
                    ///超出长度，没法安置
                    LefthandCardList[i] = 100 * LefthandCardList[i];
                    break;
                }
            }
        }
        for (int z = 0; z < LefthandCardList.Count; z++)
        {
            if (LefthandCardList[z] > 100)
            {
                LefthandCardList[z] = LefthandCardList[z] / 100;
                for (int i = 0; i < LeftHandCard.Count; i++)
                {
                    if (LeftHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite == null)

                    {
                        int te = LefthandCardList[z].ToCardInfo().num;
                        LeftHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(LefthandCardList[z]);
                        LeftHandCard[i].SetActive(true);
                        LeftHandCard[i].transform.parent.gameObject.SetActive(true);
                        if (LefthandCardList[z].ToCardInfo().isMax)
                            LeftHandCard[i].name = "A" + te.ToString();
                        else
                            LeftHandCard[i].name = te.ToString();
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < RighthandCardList.Count; i++)
        {
            int te = RighthandCardList[i].ToCardInfo().num;
            for (int z = 0; z < 4; z++)
            {
                if (te > 100)
                    break;
                if (RightHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite == null)
                {
                    RightHandCard[z * 10 + te - 1].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(RighthandCardList[i]);
                    RightHandCard[z * 10 + te - 1].SetActive(true);
                    RightHandCard[z * 10 + te - 1].transform.parent.gameObject.SetActive(true);
                    if (RighthandCardList[i].ToCardInfo().isMax)
                        RightHandCard[z * 10 + te - 1].name = "A" + te.ToString();
                    else
                        RightHandCard[z * 10 + te - 1].name = te.ToString();
                    break;
                }
                if (z == 3)
                {
                    ///超出长度，没法安置
                    RighthandCardList[i] = 100 * RighthandCardList[i];
                    break;
                }
            }
        }
        for (int z = 0; z < RighthandCardList.Count; z++)
        {
            if (RighthandCardList[z] > 100)
            {
                RighthandCardList[z] = RighthandCardList[z] / 100;
                for (int i = 0; i < RightHandCard.Count; i++)
                {
                    if (RightHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite == null)

                    {
                        int te = RighthandCardList[z].ToCardInfo().num;
                        RightHandCard[i].transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(RighthandCardList[z]);
                        RightHandCard[i].SetActive(true);
                        RightHandCard[i].transform.parent.gameObject.SetActive(true);
                        if (RighthandCardList[z].ToCardInfo().isMax)
                            RightHandCard[i].name = "A" + te.ToString();
                        else
                            RightHandCard[i].name = te.ToString();
                        break;
                    }
                }
            }
        }
    }
    bool KuaiJin = true;
    /// <summary>
    /// 吃的次数，默认为0，逐渐递加，然后变0
    /// </summary>
    int ChiTimes = 0;
    int chiLimit = 0;
    //临时变量！
    int LogTimes = 0;
    IEnumerator PlayFlashBack()
    {
        for (int i = 0; i < RoomRsp.playBack.opInfo.Count; i++)
        {
            if (IsPlay)
            {
                var Temp = RoomRsp.playBack.opInfo[i];
                if (!KuaiJin)
                    yield return new WaitForSeconds(2f);
                //CardNum.text = "剩余手牌：" + Temp.rest;
                if (RoomRsp.playBack.opInfo[i].op == ProtoBuf.MJGameOP.MJ_OP_MOPAI)
                {
                    LogTimes++;
                }
                if (RoomRsp.playBack.opInfo[i].op == ProtoBuf.MJGameOP.MJ_OP_MOPAI && (1 + i) < RoomRsp.playBack.opInfo.Count && RoomRsp.playBack.opInfo[i].op != ProtoBuf.MJGameOP.MJ_OP_GANG4)
                {
                    if (RoomRsp.playBack.opInfo[(1 + i)].op == ProtoBuf.MJGameOP.MJ_OP_MOPAI || RoomRsp.playBack.opInfo[(1 + i)].op == ProtoBuf.MJGameOP.MJ_OP_GUO)
                    {
                        yield return new WaitForSeconds(1.0f);
                        PopCardAnimation(GetPlayerNum(Temp.charId), Temp.card, "pop");
                        continue;
                    }
                    else
                    {
                        if (RoomRsp.playBack.opInfo[i].op == ProtoBuf.MJGameOP.MJ_OP_GUO)
                        {
                            Debug.Log("过了！");
                            KuaiJin = true;
                        }
                        else
                        {
                            if (RoomRsp.playBack.opInfo[i].op == ProtoBuf.MJGameOP.MJ_OP_MOPAI)
                            {
                                GetCardAnimation(GetPlayerNum(Temp.charId), Temp.card, Temp.rest);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (Temp.op != ProtoBuf.MJGameOP.MJ_OP_GUO)
                    {
                        yield return new WaitForSeconds(1.0f);
                        PlayOption(Temp.op, Temp.charId, Temp.oricharId, Temp.card, Temp.rest);
                    }
                    continue;
                }
            }
            else
            {
                while (!IsPlay)
                {
                    yield return null;
                }

            }
        }
    }
    Vector2 StartPos, EndPos;
    int DoerNum = 0;
    List<uint> ChiCards = new List<uint>();
    void PlayOption(ProtoBuf.MJGameOP eachPBPlayerOP, uint charid, uint orcharid, uint card, int rest)
    {
        DoerNum = GetPlayerNum(charid);
        switch (eachPBPlayerOP)
        {
            case ProtoBuf.MJGameOP.MJ_OP_PREP:
                break;
            case ProtoBuf.MJGameOP.MJ_OP_ZJ:
                break;
            case ProtoBuf.MJGameOP.MJ_OP_MOPAI:
                Debug.Log("摸牌：" + card + "   ID:" + charid);
                GetCardAnimation(GetPlayerNum(charid), card, rest);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_CHUPAI:
                chiLimit = 0;
                KuaiJin = false;
                ChiCards.Clear();
                ///出牌动画
                DeleteCard(DoerNum, card);
                Debug.Log("手动出牌");
                PopCardAnimation(DoerNum, card, "pop");
                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_GUO:
                KuaiJin = true;
                break;
            case ProtoBuf.MJGameOP.MJ_OP_PENG:
                Debug.Log("碰牌！");
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                StartCoroutine(GetPengCard(DoerNum, card));
                DeletePopCard(GetPlayerNum(orcharid), card);

                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_GANG://暗杠
                Debug.Log("暗杠！：" + card + "   DoerID：" + charid + "   OrChardID：" + orcharid);
                DeletePopCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);

                StartCoroutine(GetGangCard(DoerNum, card, true));
                ReFreshHandCard();

                break;
            case ProtoBuf.MJGameOP.MJ_OP_GANG2://自摸杠（碰之后摸一个，杠！）
                Debug.Log("自摸杠！：" + card + "   DoerID：" + charid + "   OrChardID：" + orcharid);
                DeleteCard(GetPlayerNum(charid), card);
                DeleteCard(GetPlayerNum(charid), card);
                DeleteCard(GetPlayerNum(charid), card);
                DeletePopCard(GetPlayerNum(orcharid), card);
                DeletePGCard(DoerNum, card);

                StartCoroutine(GetGangCard(DoerNum, card));
                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_GANG3://点杠
                Debug.Log("点杠！：" + card + "   DoerID：" + charid + "   OrChardID：" + orcharid);
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                DeletePopCard(GetPlayerNum(orcharid), card);

                StartCoroutine(GetGangCard(DoerNum, card));
                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_GANG4://畏牌   就是扫
                Debug.LogError("畏牌！：" + card);
                DeletePopCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                DeleteCard(DoerNum, card);
                StartCoroutine(GetWeiCard(DoerNum, card));

                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_HU:
                AnimCard.SetActive(false);
                ReFreshHandCard();
                break;
            case ProtoBuf.MJGameOP.MJ_OP_TING:
                Debug.LogError("补牌！" + card);
                GetCard(DoerNum, card);
                break;
            case ProtoBuf.MJGameOP.MJ_OP_CHI:
                KuaiJin = true;
                ChiTimes++;
                if (ChiTimes == 1)
                {
                    Debug.Log("吃的本体：" + card);
                    DeletePopCard(GetPlayerNum(orcharid), card);
                }
                else
                {
                    DeleteCard(DoerNum, card);
                    chiLimit++;
                    if (chiLimit <= 3)
                    {
                        ChiCards.Add(card);
                        Debug.Log("前三次吃！：" + chiLimit + " 牌:" + card);
                        if (ChiCards.Count == 3)
                        {
                            Debug.Log("吃的人:" + DoerNum);
                            StartCoroutine(GetChiCard(DoerNum, ChiCards));
                            ChiCards.Clear();
                            chiLimit = 0;
                            KuaiJin = false;
                        }
                    }
                }
                if (ChiTimes > 3)
                {
                    ChiTimes = 0;
                }
                break;
            case ProtoBuf.MJGameOP.MJ_OP_ROUND_OVER:
                Debug.Log("游戏结束！");
                AnimCard.SetActive(false);
                break;
            case ProtoBuf.MJGameOP.MJ_PLAYER_OP_RESULT:
                break;
            case ProtoBuf.MJGameOP.MJ_OP_NULL:
                break;
            default:
                break;
        }
    }
    Vector2 GetCardAnimation(int player = 0, uint card = 0, int rest = 80)//缺少往前的数据
    {
        Debug.Log("得到牌");
        P1.Kill();
        P2.Kill();
        P3.Kill();
        AnimCard.SetActive(true);
        AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(card);
        AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = GetCardSprite(card);
        switch (player)
        {
            case 0:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
                AnimCard.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, 90));
                P1 = AnimCard.transform.DOLocalMoveY(-100, 0.4f);
                P1.SetEase(Ease.InOutQuad);
                P1.SetAutoKill(false);
                P1.PlayForward();
                P1.OnComplete(delegate
                {
                    if (rest < 1)
                    {
                        AnimCard.SetActive(false);
                        GetCard(player, card);
                    }
                });
                break;
            case 1:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(-380,190);
                AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                P2 = AnimCard.transform.DOLocalMoveX(350, 0.4f);
                P2.SetEase(Ease.InOutQuad);
                P2.SetAutoKill(false);
                P2.PlayForward();
                P2.OnComplete(delegate
                {
                    if (rest < 1)
                    {
                        AnimCard.SetActive(false);
                        GetCard(player, card);
                    }
                });
                break;
            case 2:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(380, 190);
                AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                P3 = AnimCard.transform.DOLocalMoveX(350, 0.4f);
                P3.SetEase(Ease.InOutQuad);
                P3.SetAutoKill(false);
                P3.PlayForward();
                P3.OnComplete(delegate
                {
                    if (rest < 1)
                    {
                        AnimCard.SetActive(false);
                        GetCard(player, card);
                    }
                });
                break;
            default:
                break;
        }

        return AnimCard.GetComponent<RectTransform>().position;
    }
    Tween P1 = null;
    Tween P2 = null;
    Tween P3 = null;
    /// <summary>
    /// 出牌动画
    /// </summary>
    /// <param name="player"></param>
    /// <param name="card"></param>
    /// <returns></returns>
    Vector2 PopCardAnimation(int player = 0, uint card = 0, string CHU = "none")//缺少往前的数据
    {
        Debug.Log("打出牌！" + card);
        P1.Kill();
        P2.Kill();
        P3.Kill();
        AnimCard.SetActive(true);
        AnimCard.transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(card);
        AnimCard.transform.GetChild(1).GetComponent<Image>().sprite = GetCardSprite(card);
        switch (player)
        {
            case 0:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(391, -117);
                AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                P1 = AnimCard.transform.DOLocalMoveX(100, 0.4f);
                P1.SetEase(Ease.InOutQuad);
                P1.PlayForward();
                P1.OnComplete(delegate
                {
                    GetPopOutCard(player, card);
                });
                break;
            case 1:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(-380, 190);
                AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                P2 = AnimCard.transform.DOLocalMoveX(0, 0.4f);
                P2.SetEase(Ease.InOutQuad);
                P2.PlayForward();
                P2.OnComplete(delegate
                {
                    GetPopOutCard(player, card);
                });

                break;
            case 2:
                AnimCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(380, 190);
                AnimCard.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 0, 1);
                P3 = AnimCard.transform.DOLocalMoveX(0, 0.4f);
                P3.SetEase(Ease.InOutQuad);
                P3.PlayForward();
                P3.OnComplete(delegate
                {
                    GetPopOutCard(player, card);
                });
                break;
            default:
                break;
        }

        return AnimCard.GetComponent<RectTransform>().position;
    }
    Tween tempAnim = null;
    /// <summary>
    /// 补间动画
    /// </summary>
    /// <param name="StartObj"></param>
    /// <param name="EndObj"></param>
    IEnumerator TempAnimation(Transform EndObj)
    {
        yield return new WaitForSeconds(0.1f);

        //EndObj.parent.GetComponent<GridLayoutGroup>().enabled = false;
        //yield return new WaitForSeconds(0.1f);
        //var t = Instantiate(AnimCard.gameObject);
        //AnimCard.SetActive(false);
        //t.transform.SetParent(EndObj.parent, false);
        //tempAnim = t.transform.DOMove(EndObj.GetComponent<RectTransform>().anchoredPosition, 0.8f);

        //yield return new WaitForSeconds(0.8f);

        //tempAnim.OnComplete(delegate
        //{
        //    t.SetActive(false);
        //    EndObj.parent.GetComponent<GridLayoutGroup>().enabled = true;
        //    //Destroy(t);
        //});
        
    }
    /// <summary>
    /// 删除手牌数据
    /// </summary>
    /// <param name="Player">Player代表的是人物的引索</param>
    /// <param name="Card">牌面的uint值</param>
    void DeleteCard(int Player, uint Card)
    {
        switch (Player)
        {
            case 0:
                MyhandCardList.Remove(Card);

                break;
            case 1:
                LefthandCardList.Remove(Card);

                break;
            case 2:
                RighthandCardList.Remove(Card);

                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 删除碰杠的牌
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="card"></param>
    void DeletePGCard(int Player, uint card)
    {
        switch (Player)
        {
            case 0:
                if (MyPGArea.childCount > 0)
                {
                    for (int i = 0; i < MyPGArea.childCount; i++)
                    {
                        if (MyPGArea.GetChild(i).name == card.ToString())
                            Destroy(MyPGArea.GetChild(i).gameObject);
                    }
                }

                break;
            case 1:
                for (int i = 0; i < LeftPGArea.childCount; i++)
                {
                    if (LeftPGArea.GetChild(i).name == card.ToString())
                        Destroy(LeftPGArea.GetChild(i).gameObject);
                }
                break;
            case 2:
                for (int i = 0; i < RightPGArea.childCount; i++)
                {
                    if (RightPGArea.GetChild(i).name == card.ToString())
                        Destroy(RightPGArea.GetChild(i).gameObject);
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 删除玩家打出去的牌
    /// </summary>
    void DeletePopCard(int Player, uint card)
    {
        switch (Player)
        {
            case 0:
                if (MyPopCardArea.childCount > 0)
                    if (MyPopCardArea.GetChild(MyPopCardArea.childCount - 1).name == card.ToString())
                        Destroy(MyPopCardArea.GetChild(MyPopCardArea.childCount - 1).gameObject);
                break;
            case 1:
                if (LeftPopCardArea.childCount > 0)
                    if (LeftPopCardArea.GetChild(LeftPopCardArea.childCount - 1).name == card.ToString())
                        Destroy(LeftPopCardArea.GetChild(LeftPopCardArea.childCount - 1).gameObject);
                break;
            case 2:
                if (RightPopCardArea.childCount > 0)
                    if (RightPopCardArea.GetChild(RightPopCardArea.childCount - 1).name == card.ToString())
                        Destroy(RightPopCardArea.GetChild(RightPopCardArea.childCount - 1).gameObject);
                break;
            default:
                break;
        }
    }
    void GetCard(int player = 0, uint card = 0)
    {
        switch (player)
        {
            case 0:
                MyhandCardList.Add(card);
                break;
            case 1:
                LefthandCardList.Add(card);
                break;
            case 2:
                RighthandCardList.Add(card);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 打出牌
    /// </summary>
    /// <param name="num">位置</param>
    /// <param name="Card">牌值</param>
    /// <returns></returns>
    GameObject GetPopOutCard(int num, uint Card)
    {
        TempPopCard = Instantiate(PopCard);
        TempPopCard.SetActive(true);
        TempPopCard.name = Card.ToString();
        switch (num)
        {
            case 0:
                TempPopCard.transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
                TempPopCard.transform.SetParent(MyPopCardArea, false);
                break;
            case 1:

                TempPopCard.transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
                TempPopCard.transform.SetParent(LeftPopCardArea, false);
                break;
            case 2:

                TempPopCard.transform.GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
                TempPopCard.transform.SetParent(RightPopCardArea, false);
                break;
            default:
                break;
        }
        return TempPopCard;
    }
    IEnumerator GetPengCard(int doer, uint Card)
    {
        var t = Instantiate(PengGangCard);
        t.transform.GetChild(0).gameObject.SetActive(true);
        t.transform.GetChild(1).gameObject.SetActive(true);
        t.transform.GetChild(2).gameObject.SetActive(true);
        t.transform.GetChild(3).gameObject.SetActive(false);

        t.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);

        t.name = Card.ToString();
        switch (doer)
        {
            case 0:
                t.transform.SetParent(MyPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(MyPGArea.GetChild(MyPGArea.childCount - 1)));
                break;
            case 1:
                t.transform.SetParent(LeftPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(LeftPGArea.GetChild(LeftPGArea.childCount - 1)));
                break;
            case 2:
                t.transform.SetParent(RightPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(RightPGArea.GetChild(RightPGArea.childCount - 1)));
                break;
            default:
                break;
        }
        t.SetActive(true);
        yield return null;
    }
    IEnumerator GetWeiCard(int doer, uint Card)
    {
        var t = Instantiate(PengGangCard);
        t.transform.GetChild(0).gameObject.SetActive(true);
        t.transform.GetChild(1).gameObject.SetActive(true);
        t.transform.GetChild(2).gameObject.SetActive(true);
        t.transform.GetChild(3).gameObject.SetActive(false);

        t.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(0).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
        t.transform.GetChild(1).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
        t.transform.GetChild(2).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);

        t.name = Card.ToString();
        switch (doer)
        {
            case 0:
                t.transform.SetParent(MyPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(MyPGArea.GetChild(MyPGArea.childCount - 1)));
                break;
            case 1:
                t.transform.SetParent(LeftPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(LeftPGArea.GetChild(LeftPGArea.childCount - 1)));
                break;
            case 2:
                t.transform.SetParent(RightPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(RightPGArea.GetChild(RightPGArea.childCount - 1)));
                break;
            default:
                break;
        }
        t.SetActive(true);
        yield return null;
    }
    IEnumerator GetChiCard(int doer, List<uint> Card)
    {
        var t = Instantiate(PengGangCard);
        t.transform.GetChild(0).gameObject.SetActive(true);
        t.transform.GetChild(1).gameObject.SetActive(true);
        t.transform.GetChild(2).gameObject.SetActive(true);
        t.transform.GetChild(3).gameObject.SetActive(false);

        t.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card[0]);
        t.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card[1]);
        t.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card[2]);
        switch (doer)
        {
            case 0:
                t.transform.SetParent(MyPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(MyPGArea.GetChild(MyPGArea.childCount - 1)));
                break;
            case 1:
                t.transform.SetParent(LeftPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(LeftPGArea.GetChild(LeftPGArea.childCount - 1)));
                break;
            case 2:
                t.transform.SetParent(RightPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(RightPGArea.GetChild(RightPGArea.childCount - 1)));
                break;
            default:
                break;
        }
        t.SetActive(true);
        yield return null;
    }
    IEnumerator GetGangCard(int doer, uint Card, bool AnGang = false)
    {
        var t = Instantiate(PengGangCard);
        t.transform.GetChild(0).gameObject.SetActive(true);
        t.transform.GetChild(1).gameObject.SetActive(true);
        t.transform.GetChild(2).gameObject.SetActive(true);
        t.transform.GetChild(3).gameObject.SetActive(true);

        t.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        t.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = GetCardSprite(Card);
        if (AnGang)
        {
            t.transform.GetChild(0).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
            t.transform.GetChild(1).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
            t.transform.GetChild(2).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
            t.transform.GetChild(3).GetComponent<Image>().color = new Color(0.58f, 0.58f, 0.58f);
            //PlayAudio("kaiduo", DoerNum);
            //UpadateBQ(doer, OPType.angang);
        }
        else
        {
            //PlayAudio("kaiduo", DoerNum);
            //UpadateBQ(doer, OPType.gang);
        }
        switch (doer)
        {
            case 0:
                t.transform.SetParent(MyPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(MyPGArea.GetChild(MyPGArea.childCount - 1)));
                break;
            case 1:
                t.transform.SetParent(LeftPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(LeftPGArea.GetChild(LeftPGArea.childCount - 1)));
                break;
            case 2:
                t.transform.SetParent(RightPGArea, false);
                yield return null;
                StartCoroutine(TempAnimation(RightPGArea.GetChild(RightPGArea.childCount - 1)));
                break;
            default:
                break;
        }
        t.SetActive(true);
        yield return null;
    }

    GameObject TempPopCard = null;
    int GetPlayerNum(uint charid)
    {
        for (int i = 0; i < RoomRsp.playBack.players.Count; i++)
        {
            if (RoomRsp.playBack.players[i].charId == charid)
            {
                return i;
            }
        }
        Debug.Log("获取人物号码出错" + charid);
        Debug.LogWarning("获取人物号码出错！");
        return -1;
    }
    void SetHead(Sprite sprite, int num = 0)
    {
        Player[num].playerHead.sprite = sprite;
    }
    Sprite GetCardSprite(uint card)
    {
        return card.ToDzpCard();
    }

    //public Image AnimPos0, AnimPos1, AnimPos2;
    //void UpadateBQ(int seat, OPType Value)
    //{
    //    switch (seat)
    //    {
    //        case 0:
    //            AnimPos0.sprite = Value.GetSprite();
    //            StartCoroutine(PlayAnim(AnimPos0));
    //            break;
    //        case 1:
    //            AnimPos1.sprite = Value.GetSprite();
    //            StartCoroutine(PlayAnim(AnimPos1));
    //            break;
    //        case 2:
    //            AnimPos2.sprite = Value.GetSprite();
    //            StartCoroutine(PlayAnim(AnimPos2));
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //IEnumerator PlayAnim(Image Value)
    //{
    //    Tween t;
    //    Value.gameObject.SetActive(true);
    //    yield return null;
    //    t = Value.GetComponent<RectTransform>().DOScale(1.5f, 0.4f);
    //    yield return new WaitForSeconds(0.5f);
    //    t.PlayBackwards();
    //    yield return null;
    //    Value.gameObject.SetActive(false);

    //}
}
