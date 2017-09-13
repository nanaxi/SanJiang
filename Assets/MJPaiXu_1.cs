using UnityEngine;
using System.Collections;

public class MJPaiXu_1 : MonoBehaviour {
    public float positionX = 0.035f;
	// Use this for initialization
	void Start () {
        
	}

    public void OnGUI()
    {
        if (GUILayout.Button("Set MJ Position"))
        {
            SetMJPositon();
        }
    }

    public void SetMJPositon()
    {
        Transform[] allChild = transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChild.Length; i++)
        {
            allChild[i].position = new Vector3(positionX * i, 0, 0);
        }
    }

}
