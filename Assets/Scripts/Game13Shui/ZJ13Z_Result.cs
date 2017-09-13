using UnityEngine;
using System.Collections;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ZJ13Z_Result : MonoBehaviour {

    public ProtoBuf.MJRoundRecord MRR;

    /// <summary>缓存池——>扑克池，装扑克
    /// </summary>
    public PublicPathPool poolPoker;

    /// <summary>单局结算
    /// </summary>
    [SerializeField]
    private END13Z_1Ju end13Z_1Ju;

    public RectTransform maPai_Tag;//马牌标签，如果某玩家的牌是码牌，就给他一个马牌标签

    private uint maPaiCard;

    // Use this for initialization
    void Start () {
	
	}

    public void Init_ShowZJ(MJRoundRecord mrr_)
    {
        MRR = mrr_;
        GOGogo(MRR);
    }

    void OnDisable()
    {
        poolPoker.PoolRecycleAll();
    }

    /// <summary>战绩解析，并添加到播放委托
    /// </summary>
    public string GOGogo(MJRoundRecord mrr_)
    {
        MRR = mrr_;
        string strZhanJi = "";
        //1.玩家初始牌更新
        Player_Data[] zjAllPlayerAry = new Player_Data[4];
        for (int i = 0; i < MRR.players.Count; i++)
        {
            zjAllPlayerAry[i] = new Player_Data(MRR.players[i].name, MRR.players[i].charId, MRR.players[i].ip, MRR.players[i].portrait, MRR.players[i].restDiamond, MRR.players[i].gold, (int)MRR.players[i].sex);
        }
        int i_Count_HHH = 0;
        zjAllPlayerAry = DataManage.Instance.PData_Rank(zjAllPlayerAry);
        DataManage.Instance.PData_Update(zjAllPlayerAry);
        end13Z_1Ju.UpdatePlayerUI(DataManage.Instance.PData_GetDataAry());

        for (int i = 0; i < MRR.players.Count; i++)
        {
            Debug.Log(MRR.players.Count);
            Debug.Log("PlayerId:" + MRR.players[i].charId + "_ShouPai?__" + MRR.playBack.players[i].handCards.Count + "xxxxxxxxx");
            //for (int i1 = 0; i1 < MRR.playBack.players[i].handCards.Count; i1++)
            //{
            //}
            end13Z_1Ju.UpdatePlayerUI_Gold(MRR.players[i].charId, MRR.players[i].gold);
            List<uint> handCards = MRR.playBack.players[i].handCards.Take(13).ToList();
            if (MRR.playBack.players[i].handCards.Count == 14)
            {
                maPaiCard = MRR.playBack.players[i].handCards[13];
            }
            List<GameObject> listResult = GetPokerObjList(handCards);
            end13Z_1Ju.UpdatePlayerUI_Poker(MRR.players[i].charId, listResult.ToArray(), false);// end_MJGmOver.players[i].hasTingPai > 0);

            //MRR.playBack.opInfo[0]
            //end13Z_1Ju.UpdatePlayerUI_TSPTxt(end_MJGmOver.players[i].charId, TSP_GetName(end_MJGmOver.players[i].hasTingPai));
        }

        return strZhanJi;

    }

    /// <summary>马牌标签—》判断手牌是不是马牌，如果是就加一个标签
    /// </summary>
    void MaPaiTag_IfAdd(PokerInfos pokerIns)
    {
        maPai_Tag.SetParent(pokerIns.transform);
        maPai_Tag.localScale = Vector3.one;
        maPai_Tag.anchoredPosition = Vector2.zero;
    }

    /// <summary>根据cardAry 获取扑克Ary
    /// </summary>
    List<GameObject> GetPokerObjList(List<uint> cards)
    {
        List<GameObject> listResult = new List<GameObject>();
        List<Poker_X> pokerXAry = new List<Poker_X>();
        for (int i = 0; i < cards.Count; i++)
        {
            pokerXAry.Add(cards[i].ToPoker_X());
        }
        for (int i = 0; i < pokerXAry.Count; i++)
        {
            GameObject pokerIns = poolPoker.PoolGetGameObject(pokerXAry[i].CardName);
            pokerIns.GetComponent<PokerInfos>().pokerInfo = pokerXAry[i];
            pokerIns.GetComponent<Button>().onClick.RemoveAllListeners();
            listResult.Add(pokerIns);
            if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == maPaiCard)
            {
                MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
            }
            //if (pokerIns.GetComponent<PokerInfos>().pokerInfo.CardId == gm13ZData.maPaiCard)
            //{
            //    MaPaiTag_IfAdd(pokerIns.GetComponent<PokerInfos>());
            //}
        }
        return listResult;
    }
}
