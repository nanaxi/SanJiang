using UnityEngine;
using System.Collections;

public class TestMono : MonoBehaviour {

    string strInit;
    void Awake()
    {
        strInit = "——Awake???——";
        Debug.Log("Awake()-->");
    }
	// Use this for initialization
	void Start () {
        Debug.Log("Start()-->");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable()-->");
    }

    public void TestDebug(string str)
    {
        Debug.Log("TestDebug-->" + str+ strInit);
    }
}
