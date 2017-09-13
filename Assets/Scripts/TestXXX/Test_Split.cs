using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Test_Split : MonoBehaviour {
    public uint pokerCardID;

    //string s = "abcdeabcdeabcde";
    //public string[] strAry1;
    public Queue<GameObject> gmQueue = new Queue<GameObject>();// new Stack<GameObject>();
    public int queCount;

    public Stack<GameObject> gmStack = new Stack<GameObject>();
    public int stackCount;
    public List<GameObject> gmList = new List<GameObject>();
    // Use this for initialization
    void Start() {
        //strAry1 = s.Split(new char[] { 'c','d','e'});
        //strAry1 = s.Split(new string[] {"cde"},System.StringSplitOptions.RemoveEmptyEntries );
        //Debug.Log("Time:"+System.DateTime.Now.Millisecond);
        //strAry1 = (from strValue in strAry1
        //           select strValue
        //           ).Take(2).ToArray();
        //Debug.Log("Time2:" + System.DateTime.Now.Millisecond);
        for (int i = 0; i < 10; i++)
        {
            GameObject gmIns = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gmIns.name = "Test_"+i;
            gmStack.Push(gmIns);
            gmQueue.Enqueue(gmIns);
        }
        stackCount = gmStack.Count;
    }

    public void GetStack()
    {
        if (gmStack.Count>0)
        {
            gmList.Add(gmStack.Pop());
        }
        stackCount = gmStack.Count;
    }

    public void GetQueue()
    {
        if (gmQueue.Count > 0)
        {
            gmList.Add(gmQueue.Dequeue());
        }
        gmQueue.TrimExcess();
        queCount = gmQueue.Count;
    }

    public void PokerTest()
    {
        Poker_X pTest = pokerCardID.ToPoker_X();
        Debug.Log("PokerName:"+pTest.CardName +"\tID"+pTest.CardId);
    }
}


