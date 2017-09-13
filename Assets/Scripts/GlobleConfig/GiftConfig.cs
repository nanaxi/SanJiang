using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiftConfig : SingletonParent<GiftConfig>
{
    private Dictionary<int, GiftSX> dicGift = new Dictionary<int, GiftSX>();
    public override void Init()
    {
        GiftAdd(new GiftSX(101, "MeiGui", "", ""));
        GiftAdd(new GiftSX(102, "QinWen", "", ""));
        GiftAdd(new GiftSX(103, "TuoXie", "", ""));
        //throw new NotImplementedException();
    }

    void GiftAdd(GiftSX gift)
    {
        if (dicGift.ContainsKey(gift.id))
        {
            return;
        }
        dicGift.Add(gift.id, gift);
    }

    public GiftSX Gift_GetConfig(int id)
    {
        if (dicGift.ContainsKey(id))
        {
            return dicGift[id];
        }
        return null;
    }

    public GiftSX Gift_GetConfig(string name)
    {
        foreach (KeyValuePair<int, GiftSX> item in dicGift)
        {
            if (item.Value.name == name)
            {
                return item.Value;
            }
        }

        return null;
    }
}
public class GiftSX
{
    public int id;
    public string name;
    public string pathICON;
    public string pathSprites;

    public GiftSX(int iiD, string strName, string pathIcon_, string pathSprites_)
    {
        id = iiD;
        name = strName;
        pathICON = pathIcon_;
        pathSprites = pathSprites_;
    }
}