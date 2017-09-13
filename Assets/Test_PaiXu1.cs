using UnityEngine;
using System.Collections;

public class Test_PaiXu1 : MonoBehaviour {
    public int[] i_Ary;
    public int iValue;
    public int iIndex;
    public Transform[] t_Ary;
    public Vector3 v3D;
    //Left + New Vector3(0.041,0,0);
    // Use this for initialization
    void Start () {
        v3D = t_Ary[1].localPosition - t_Ary[0].localPosition;

        Debug.Log(v3D);
    }

    public void GetIndex()
    {
        i_Ary = new int[] { 1, 3, 5, 7, 18, 18, 32, 48, 49 };
        Debug.Log("GetIndex");

        for (int i = 0; i < i_Ary.Length-1; i++)
        {
            if (iValue >= i_Ary[i] && iValue < i_Ary[i + 1])
            {
                Debug.Log("INDEX :" + i);
                iIndex = i;
                return;
            }
        }

        iIndex = i_Ary.Length-1;
        Debug.Log("INDEX :" + iIndex);

        return ;
    }
}
