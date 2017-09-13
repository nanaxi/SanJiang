using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestDicEvent : MonoBehaviour {
    public Dictionary<string, NetEvent> dicNetEvent = new Dictionary<string, NetEvent>();
    void Awake()
    {
        Test_Manage1.Instance.Init();

        dicNetEvent.Add(NetPackName.TestMsg1.ToString(),new NetEvent(typeof(TestNet_1),Recive_1));
        dicNetEvent.Add(NetPackName.TestMsgA.ToString(), new NetEvent(typeof(TestNet_A), Recive_A));
        System.Type typeX;
        typeX = typeof(TestNet_1);

    }

    // Use this for initialization
    void Start () {
        foreach (KeyValuePair<string,NetEvent> item in dicNetEvent)
        {
            System.Object obj = new object();
            Debug.Log("ForeachXXX"+item.Key);
            switch (item.Key)
            {
                case "TestMsg1":
                    obj = new TestNet_1();
                    if (dicNetEvent.ContainsKey(NetPackName.TestMsg1.ToString()))
                    {
                        NetEvent netE = dicNetEvent[NetPackName.TestMsg1.ToString()];
                        netE.onReciveMsg(obj);
                    }
                    break;
                case "TestMsgA":
                    obj = new TestNet_A();
                    item.Value.onReciveMsg(obj);
                    break;
                default:
                    break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Recive_1(System.Object obj)
    {
        TestNet_1 t1 = (TestNet_1)obj;
        if (t1 != null)
        {
            Debug.Log("MSG:"+t1.GetMsg());
        }
    }

    public void Recive_A(System.Object obj)
    {
        TestNet_A t1 = (TestNet_A)obj;
        if (t1 != null)
        {
            Debug.Log("MSG:" + t1.GetMsg());
        }
    }
}

public class NetEvent
{
    public System.Type type;
    public ReciveMsg onReciveMsg;
    public NetEvent(System.Type nameX,ReciveMsg onEvent)
    {
        type = nameX;
        onReciveMsg = onEvent;
    }
}

public delegate void ReciveMsg(System.Object obj);

public enum NetPackName
{
    TestMsg1, TestMsgA
}

public class TestNet_1
{
    public string GetMsg()
    {
        return "【class TestNet_1】";
    }
    public TestNet_1()
    { }
}

public class TestNet_A
{
    public string GetMsg()
    {
        return "【class TestNet_A】";
    }
    public TestNet_A()
    { }
}
