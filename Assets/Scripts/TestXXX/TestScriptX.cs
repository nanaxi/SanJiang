using UnityEngine;
using System.Collections;

public class TestScriptX : MonoBehaviour {

    public GameObject gmResCube;


    public int i_10JZ;
    public string str_16JZ;

    public uint uint_16JZ;
    // Use this for initialization
    void Start () {
        GameObject gmIns = Instantiate(gmResCube);
        gmIns.GetComponent<TestMono>().TestDebug("LLLLLL");
        gmIns.name = "XXX";
        i_10JZ = (int)'A';
    }

    void OnGUI()
    {
        //if (GUI.Button(new Rect(128, 128, 128, 64), "10 To 16"))
        //{
        //    str_16JZ = i_10JZ.ToString("x8");
        //    uint_16JZ = uint.Parse(str_16JZ);
        //}

        //if (GUI.Button(new Rect(128, 192, 128, 64), "16 To 10"))
        //{
        //    i_10JZ = System.Int32.Parse("266", System.Globalization.NumberStyles.HexNumber);
        //}

        //if (GUI.Button(new Rect(128, 128, 128, 64), "Send Share"))
        //{
        //    PublicEvent.GetINS.SentShare();
        //}
    }
}


