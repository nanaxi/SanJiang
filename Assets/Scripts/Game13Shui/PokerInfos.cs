using UnityEngine;
using System.Collections;

public class PokerInfos : MonoBehaviour {
    public Poker_X pokerInfo;

    private bool isSelect;

    public bool IsSelect
    {
        get
        {
            return isSelect;
        }

        set
        {
            isSelect = value;
        }
    }
    public Vector3 v3_Down;
    public Vector3 v3_Up;
    // Use this for initialization
    //void Start()
    //{

    //}            

    void OnEnable()
    {
        Init_ClickHight();
    }
    public void Init_ClickHight()
    {
        v3_Down = GetComponent<RectTransform>().anchoredPosition3D;
        Vector3 v3New = new Vector3(v3_Down.x, v3_Down.y + 64, v3_Down.z);
        v3_Up = v3New;

    }

    public void SelectPoker(bool isS)
    {
        GetComponent<RectTransform>().anchoredPosition3D = isS ? v3_Up : v3_Down;
        isSelect = isS;
    }

}

[System.Serializable]
public class Poker_X
{
    private uint cardId;
    private string cardName;
    private uint cardHuaSe;
    private uint cardValue;

    /// <summary>扑克ID
    /// </summary>
    public uint CardId
    {
        get{ return cardId; }
        set{ cardId = value; }
    }

    /// <summary>扑克名字
    /// </summary>
    public string CardName
    {
        get{ return cardName; }
        set{ cardName = value;}
    }

    /// <summary>扑克花色
    /// </summary>
    public uint CardHuaSe
    {
        get{return cardHuaSe;}

        set{cardHuaSe = value;}
    }

    /// <summary>扑克大小
    /// </summary>
    public uint CardValue
    {
        get{return cardValue;}

        set{cardValue = value; }
    }

    public Poker_X()
    { }

    public Poker_X(uint cId,string cName,uint cHuaSe,uint cValue)
    {
        CardId = cId;
        CardName = cName;
        CardHuaSe = cHuaSe;
        CardValue = cValue;
    }
    
}