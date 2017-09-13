using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>摆牌牌型判断
/// </summary>
public class Poker13Type
{
    /// <summary>对子
    /// </summary>
    static public List<List<GameObject>> PokerType_DuiZi(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        for (int i = 0; i < pokerAry.Length; i++)
        {
            if (i + 1 < pokerAry.Length)
            {
                if (pokerAry[i].pokerInfo.CardValue == pokerAry[i + 1].pokerInfo.CardValue)
                {
                    List<GameObject> listDuiZi = new List<GameObject>();
                    listDuiZi.Add(pokerAry[i].gameObject);
                    listDuiZi.Add(pokerAry[i + 1].gameObject);
                    listResult.Add(listDuiZi);
                }
            }
        }
        return listResult;
    }

    /// <summary>两对子
    /// </summary>
    static public List<List<GameObject>> PokerType_DuiZiX2(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listDuiZi = PokerType_DuiZi(pokerAry);// new List<List<GameObject>>();
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        List<uint> listDZValue = new List<uint>();
        for (int i = 0; i < listDuiZi.Count; i++)
        {//移除相同的对子
            if (!listDZValue.Contains(listDuiZi[i][0].GetComponent<PokerInfos>().pokerInfo.CardValue))
            {
                listDZValue.Add(listDuiZi[i][0].GetComponent<PokerInfos>().pokerInfo.CardValue);
            }
            else
            {
                listDuiZi.Remove(listDuiZi[i]);
            }
        }
        for (int i = 0; i < listDuiZi.Count; i++)
        {
            if (i + 1 < listDuiZi.Count)
            {
                List<GameObject> list2DuiZi = new List<GameObject>(listDuiZi[i]);
                list2DuiZi.Add(listDuiZi[i + 1][0]);
                list2DuiZi.Add(listDuiZi[i + 1][1]);

                listResult.Add(list2DuiZi);
            }
        }
        return listResult;
    }

    /// <summary>顺子
    /// </summary>
    static public List<List<GameObject>> PokerType_ShunZi(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        for (int i = 0; i < pokerAry.Length; i++)
        {
            if (i + 4 < pokerAry.Length)
            {
                if (PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 1)) !=null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 2)) != null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 3)) != null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 4)) != null)
                {//如果是A以下
                    List<GameObject> listShunZi = new List<GameObject>();
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue )));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 1)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 2)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 3)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue + 4)));
                    listResult.Add(listShunZi);
                }else if (PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 1)) != null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 2)) != null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 3)) != null
                    && PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 4)) != null)
                {
                    List<GameObject> listShunZi = new List<GameObject>();
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 1)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 2)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 3)));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, (pokerAry[i].pokerInfo.CardValue - 4)));
                    listResult.Add(listShunZi);
                }
                else if (pokerAry[i].pokerInfo.CardValue == 1
                    && PokerType_IsExistPoker(pokerAry, 10) != null
                   && PokerType_IsExistPoker(pokerAry, 11) != null
                   && PokerType_IsExistPoker(pokerAry, 12) != null
                   && PokerType_IsExistPoker(pokerAry, 13) != null)
                {//*10 JQKA*/
                    List<GameObject> listShunZi = new List<GameObject>();
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, 10));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, 11));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, 12));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, 13));
                    listShunZi.Add(PokerType_IsExistPoker(pokerAry, 1));
                    listResult.Add(listShunZi);
                }
            }
        }
        return listResult;
    }

    /// <summary>是否存在某张牌
    /// </summary>
    static public GameObject PokerType_IsExistPoker(PokerInfos[] pokerAry , uint cardValue )
    {
        List<PokerInfos> listResult = new List<PokerInfos>();
        listResult = (from pokerItem in pokerAry
                      where pokerItem.pokerInfo.CardValue == cardValue
                      select pokerItem
                      ).ToList();
        if (listResult.Count>0)
        {    
            return listResult[0].gameObject; //true;
        }
        return null;// false;
    }

    /// <summary>3条
    /// </summary>
    static public List<List<GameObject>> PokerType_3Tiao(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        for (int i = 0; i < pokerAry.Length; i++)
        {
            if (i + 2 < pokerAry.Length)
            {
                if (pokerAry[i].pokerInfo.CardValue == pokerAry[i + 1].pokerInfo.CardValue && pokerAry[i].pokerInfo.CardValue == pokerAry[i + 2].pokerInfo.CardValue)
                {
                    List<GameObject> listDuiZi = new List<GameObject>();
                    listDuiZi.Add(pokerAry[i].gameObject);
                    listDuiZi.Add(pokerAry[i + 1].gameObject);
                    listDuiZi.Add(pokerAry[i + 2].gameObject);
                    listResult.Add(listDuiZi);
                }
            }
        }
        return listResult;
    }

    /// <summary>炸弹
    /// </summary>
    static public List<List<GameObject>> PokerType_ZhaDan(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        for (int i = 0; i < pokerAry.Length; i++)
        {
            if (i + 3 < pokerAry.Length)
            {
                if (pokerAry[i].pokerInfo.CardValue == pokerAry[i + 1].pokerInfo.CardValue
                    && pokerAry[i].pokerInfo.CardValue == pokerAry[i + 2].pokerInfo.CardValue
                    && pokerAry[i].pokerInfo.CardValue == pokerAry[i + 3].pokerInfo.CardValue
                    )
                {
                    List<GameObject> listDuiZi = new List<GameObject>();
                    listDuiZi.Add(pokerAry[i].gameObject);
                    listDuiZi.Add(pokerAry[i + 1].gameObject);
                    listDuiZi.Add(pokerAry[i + 2].gameObject);
                    listDuiZi.Add(pokerAry[i + 3].gameObject);
                    listResult.Add(listDuiZi);
                }
            }
        }
        return listResult;
    }

    /// <summary>葫芦
    /// </summary>
    static public List<List<GameObject>> PokerType_HuLu(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        listResult = PokerType_3Tiao(pokerAry);
        if (listResult.Count < 1)
        {
            return listResult;
        }

        List<uint> list3TInfos = new List<uint>();

        for (int i = 0; i < listResult.Count; i++)
        {
            if (listResult[i][0]!=null)
            {
                if (!list3TInfos.Contains(listResult[i][0].GetComponent<PokerInfos>().pokerInfo.CardValue))
                {
                    list3TInfos.Add(listResult[i][0].GetComponent<PokerInfos>().pokerInfo.CardValue);
                }                            
            }
        }

        //移除对子中的3条类的
        List<List<GameObject>> listDuiZi = new List<List<GameObject>>();
        listDuiZi = PokerType_DuiZi(pokerAry);


        if (listDuiZi.Count<1)
        {//如果只有三条没有对子，
            listResult.Clear();
            return listResult;
        }

        List<List<GameObject>> listSum = new List<List<GameObject>>();
        for (int i = 0; i < listResult.Count; i++)
        {//如果有三条有对子
            
            for (int i_Dz = 0; i_Dz < listDuiZi.Count; i_Dz++)
            {
                if (listResult[i][0].GetComponent<PokerInfos>().pokerInfo.CardValue 
                    != listDuiZi[i_Dz][0].GetComponent<PokerInfos>().pokerInfo.CardValue)
                {
                    List<GameObject> listHuLu = new List<GameObject>(listResult[i]);//先加入3条
                    for (int i_L1 = 0; i_L1 < listDuiZi[i_Dz].Count; i_L1++)
                    {
                        listHuLu.Add(listDuiZi[i_Dz][i_L1].gameObject);//加入对子
                    }
                    listSum.Add(listHuLu);
                }
            }
        }
        return listSum;
    }

    /// <summary>同花顺子, 5张+++
    /// </summary>
    static public List<List<GameObject>> PokerType_TongHuaShun(PokerInfos[] pokerAry)
    {
        //先获取同花牌组
        List<List<GameObject>> listResult = PokerType_TongHua(pokerAry); //new List<List<GameObject>>();
        List<List<GameObject>> listSum = new List<List<GameObject>>();

        for (int i = 0; i < listResult.Count; i++)
        {
            if (listResult[i].Count>=5)
            {//已知 都是同花的情况下， 判断是否是顺子
                //同花数组 根据牌值大小排序。
                PokerInfos[] pokerThAry = (from poker in listResult[i]
                                           orderby poker.GetComponent<PokerInfos>().pokerInfo.CardValue ascending
                                           select poker.GetComponent<PokerInfos>()).ToArray();
                if (pokerThAry[0].pokerInfo.CardValue + 1 == pokerThAry[1].pokerInfo.CardValue
                    && pokerThAry[1].pokerInfo.CardValue + 1 == pokerThAry[2].pokerInfo.CardValue
                    && pokerThAry[2].pokerInfo.CardValue + 1 == pokerThAry[3].pokerInfo.CardValue
                    && pokerThAry[3].pokerInfo.CardValue + 1 == pokerThAry[4].pokerInfo.CardValue
                    )
                {//同花顺
                    listSum.Add(listResult[i]);
                }else if (PokerType_IsExistPoker(pokerThAry, 1) != null
                   && PokerType_IsExistPoker(pokerThAry, 10) != null
                  && PokerType_IsExistPoker(pokerThAry, 11) != null
                  && PokerType_IsExistPoker(pokerThAry, 12) != null
                  && PokerType_IsExistPoker(pokerThAry, 13) != null)
                {//*10 JQKA*/
                    listSum.Add(listResult[i]);
                }

            }
        }
        return listSum;
    }

    /// <summary>同花, 5张
    /// </summary>
    static public List<List<GameObject>> PokerType_TongHua(PokerInfos[] pokerAry)
    {
        List<List<GameObject>> listResult = new List<List<GameObject>>();
        PokerInfos[] pokerAry_ZL;
        for (int i = 0; i < 4; i++)
        {
            pokerAry_ZL = GetPoker_HuaSeAry(pokerAry, i).ToArray();
            if (pokerAry_ZL.Length >= 5)
            {
                //List<GameObject> listAry1 = new List<GameObject>();
                for (int i_XXX = 0; i_XXX < pokerAry_ZL.Length; i_XXX++)
                {
                    if (i_XXX + 4 < pokerAry_ZL.Length)
                    {
                        List<GameObject> listTongHua = new List<GameObject>();
                        for (int i_1 = 0; i_1 < 5; i_1++)
                        {
                            listTongHua.Add(pokerAry_ZL[i_XXX + i_1].gameObject);
                        }
                        listResult.Add(listTongHua);
                    }
                }
            }
        }
        

        return listResult;
    }

    /// <summary>取出指定数组中 的所有  指定扑克花色的 扑克
    /// 花色取值范围： 0、1、2、3
    /// </summary>
    static public List<PokerInfos> GetPoker_HuaSeAry(PokerInfos[] pokerAry, int cardTYPE)
    {
        List<PokerInfos> listResult = new List<PokerInfos>();

        listResult = (from pokerIns in pokerAry
                      where pokerIns.pokerInfo.CardHuaSe == cardTYPE
                      select pokerIns).ToList();

        return listResult;
    }
}
