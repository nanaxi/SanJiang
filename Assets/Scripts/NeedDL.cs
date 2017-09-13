using UnityEngine;
//using System.Collections;
using System.Collections.Generic;
//using System;

public class NeedDL {

    /// <summary>List<>ADD(ARY[].GetComponent<TYPE1>())
    /// </summary>
    static public List<TYPE1> AryToList<T,TYPE1>(T[] t_Ary) where T:Component where TYPE1:Component
    {
        List<TYPE1> rValue = new List<TYPE1>();
        for (int i = 0; i < t_Ary.Length; i++)
        {
            rValue.Add(t_Ary[i].GetComponent<TYPE1>());
        }
        return rValue;
    }

    /// <summary>List<>ADD(ARY[].GetComponent<TYPE1>())
    /// </summary>
    static public TYPE1[] AryToAry<T, TYPE1>(T[] t_Ary) where T : Component where TYPE1 : Component
    {
        TYPE1[] rValue = new TYPE1[t_Ary.Length];
        for (int i = 0; i < t_Ary.Length; i++)
        {
            rValue[i] = t_Ary[i].GetComponent<TYPE1>();
        }
        return rValue;
    }

    static public Key DicValueGetKey<Key, Value>(Value _value, Dictionary<Key, Value> dic)
    {
        Key rValue = default(Key);
        try
        {
            foreach (KeyValuePair<Key, Value> kvp in dic)
            {
                if (kvp.Value.Equals(_value))
                {
                    //...... kvp.Key;
                    return kvp.Key;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("ERROR ??"+e);
            throw;
        }

        Debug.LogError("DIC  Get Key Error,  Key==Null");
        return rValue;
    }
}
