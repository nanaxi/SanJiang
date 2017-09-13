using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>13张， 该类执行：代表一个玩家对牌进行操作
/// </summary>
public class InGame_Player13Z : MonoBehaviour {

    public Transform layoutParent_BP;//【拖拽赋值】显示每道比牌的Layout
    public GridLayoutGroup[] layoutAry_BiPai;//比牌的Layout

    public Transform showGoldParent;//【拖拽赋值】显示每道比牌得分
    public Text[] tAryShowGold;//显示每道比牌的得分

    [SerializeField]
    private GameObject sortNotOkStyle;//【拖拽赋值】显示已经完成了摆牌的背景
    [SerializeField]private GameObject sortOkStyle;//【拖拽赋值】显示已经完成了摆牌的背景

    [SerializeField]private GameObject prepOkStyle;//【拖拽赋值】显示已经准备

    [SerializeField]
    private GameObject gmDaQaingStyle;//打枪

    [SerializeField]
    private GameObject gmBeiDaQaingStyle;//被打枪

    private uint charId;

    public uint CharId
    {
        get{ return charId;}
        set{ charId = value;}
    }

    // Use this for initialization
    void Awake()
    {
        layoutAry_BiPai = layoutParent_BP.GetComponentsInChildren<GridLayoutGroup>(true);
        tAryShowGold = showGoldParent.GetComponentsInChildren<Text>(true);
    }

    /// <summary>向指定位置添加比牌
    /// </summary>
    /// <param name="iIndex">==  0—>头道。 1—>中道 。 2—>尾道 </param>
    public void BiPai_ADD(int iIndex, PokerInfos[] pokerAry, int iGold = 0,string cardType ="")
    {
        if (iIndex < 0 || iIndex >= layoutAry_BiPai.Length)
        {
            Debug.LogError("iIndex >= Ary.Length");
            return;
        }

        Transform tLayout = layoutAry_BiPai[iIndex].transform;
        
        for (int i = 0; i < pokerAry.Length; i++)
        {
            pokerAry[i].transform.SetParent(tLayout);
            pokerAry[i].transform.localScale = Vector3.one;
        }
        
        string strFuHao = iGold > 0 ? "+ " : "";
        string strGold = iGold.ToString();
        strGold = strGold.Replace("-", "- ");
        tAryShowGold[iIndex].text = "<color=white>" + cardType + "</color>" + strFuHao + strGold;

    }

    /// <summary>获取 已经比牌了的个数。
    /// </summary>
    /// <param name="iIndex">==  0—>头道。 1—>中道 。 2—>尾道</param>
    /// <returns></returns>
    public int GetBiPai_Count(int iIndex)
    {
        if (iIndex < 0 || iIndex >= layoutAry_BiPai.Length)
        {
            Debug.LogError("iIndex >= Ary.Length");
            return 0;
        }
        int tLayoutChildCount = layoutAry_BiPai[iIndex].transform.childCount;
        return tLayoutChildCount;
    }

    public void Clear_AllPoker()
    {
    }

    /// <summary>获取该玩家的比牌位置上的所有牌
    /// </summary>
    public PokerInfos[] GetBP_AllPoker()
    {
        PokerInfos[] resultAry = layoutParent_BP.GetComponentsInChildren<PokerInfos>(true);
        return resultAry;
    }

    public void ShowGold_ClearAll()
    {
        for (int i = 0; i < tAryShowGold.Length; i++)
        {
            tAryShowGold[i].text = "";
        }
    }

    public void PrepOK_OpenOrClose(bool isOpen)
    {
        prepOkStyle.SetActive(isOpen);
    }

    /// <summary>玩家已摆好
    /// </summary>
    /// <param name="isOpen"></param>
    public void BaiHao_OpenOrClose(bool isOpen)
    {
        if (isOpen)
        {
            sortNotOkStyle.SetActive(false);
        }
        sortOkStyle.SetActive(isOpen);
    }

    /// <summary>打枪
    /// </summary>
    /// <param name="isOpen"></param>
    public void DaQiang_OpenOrClose(bool isOpen)
    {
        gmDaQaingStyle.SetActive(isOpen);
    }

    /// <summary>被打枪
    /// </summary>
    /// <param name="isOpen"></param>
    public void BeiDaQiang_OpenOrClose(bool isOpen)
    {
        gmBeiDaQaingStyle.SetActive(isOpen);
    }
}

/// <summary>比牌记录
/// </summary>
public class BiPaiJiLu_X
{
    /// <summary>摆在道上的扑克
    /// </summary>
    public PokerInfos[] pokerAry;
    /// <summary>道得到了多少分
    /// </summary>
    public int gold;
    /// <summary>道的类型
    /// </summary>
    public string pokerType;
    public BiPaiJiLu_X()
    { }
}