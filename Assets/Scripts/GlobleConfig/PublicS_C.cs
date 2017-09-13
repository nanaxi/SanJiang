using UnityEngine;
using System.Collections;
using System;

static public class PublicS_C
{

    public static string ToName(this string base64Str)
    {
        string ret_V = base64Str;
        try
        {
            ret_V = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
        }
        catch (Exception e)
        {
            Debug.Log(base64Str + "ToName()转 base64String 可能有问题 ：" + e.Message);
            return base64Str;
        }
        return ret_V;
    }


    /// <summary>解析麻将
    /// </summary>
    /// <param name="cardID"></param>
    /// <returns></returns>
    static public Mj_Sx_ ToCard(this uint cardID)
    {
        Mj_Sx_ cardStruct;

        uint color = (cardID & 0xF0) >> 4;
        uint value = cardID & 0x0F;

        CardsType CT = CardsType.Tong;
        string mj_Name = "";
        switch (color)
        {
            case 0:
                CT = CardsType.Wan;
                mj_Name = value + "W";
                break;
            case 1:
                CT = CardsType.Tiao;
                mj_Name = value + "T";
                break;
            case 2:
                CT = CardsType.Tong;
                mj_Name = value + "B";
                break;
            case 3:
                CT = CardsType.Hua;
                mj_Name = value + "H";
                break;
            default:
                CT = CardsType.Null;
                break;
        }

        cardStruct = new Mj_Sx_((int)value, cardID, mj_Name);
        return cardStruct;

    }

    /// <summary>解析麻将
    /// </summary>
    /// <param name="cardID"></param>
    /// <returns></returns>
    static public Poker_X ToPoker_X(this uint cardID)
    {
        Poker_X cardStruct;

        uint color = (cardID & 0xF0) >> 4;
        uint value = cardID & 0x0F;
        //方块、 梅花、红桃、黑桃 value + "W";
        CardsType CT = CardsType.Tong;
        string poker_Name = "";
        switch (color)
        {
            case 0:
                CT = CardsType.Wan;
                poker_Name = "FK"+value;// value + "W";
                break;
            case 1:
                CT = CardsType.Tiao;
                poker_Name = "MH" + value;
                break;
            case 2:
                CT = CardsType.Tong;
                poker_Name = "HX" + value;
                break;
            case 3:
                CT = CardsType.Hua;
                poker_Name = "HT" + value;
                break;
            default:
                CT = CardsType.Null;
                break;
        }

        cardStruct = new Poker_X(cardID, poker_Name, color,value);//new Mj_Sx_((int)value, cardID, mj_Name);
        return cardStruct;

    }

}
