using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhpLog : MonoBehaviour {
    #region Instance Define
    static PhpLog inst = null;
    static public PhpLog Instance
    {
        get
        {
            if (inst==null)
            {
                inst = Component.FindObjectOfType<PhpLog>();
            }
            return inst;
        }
    }
    #endregion

    public string gameNmae = "游戏名字";


    // Use this for initialization
    void Awake () {
        inst = this;
        //Add_Log("游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字游戏名字");
    }

    void Start()
    {

    }

    static public void Add_Log(string log)
    {
        System.DateTime moment = System.DateTime.Now;
        string dTime = "【" +moment.Year + "-" + moment.Month + "-" + moment.Day + "-" + moment.Hour + ":" + moment.Minute + "】___";
        dTime = dTime + log.Replace("\n", "<换行>");
        inst.StartCoroutine(inst.PHP_UpLoading(dTime));
    }

    IEnumerator PHP_UpLoading(string log)
    {
        string urlX = "http://192.168.1.156/w3debug.php?Request_=logerror&log=《" + gameNmae+"》" + log;
        WWW w3 = new WWW(urlX);
        yield return w3;
        if (w3.error == null)
        {
            Debug.Log("<color=red>" + w3.text + "</color>");
        }
        else
        {
            Debug.Log("PHP_UpLoading Error?");
        }
        yield return null;
    }
}
