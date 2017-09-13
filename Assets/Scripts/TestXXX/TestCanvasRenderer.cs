using UnityEngine;
using System.Collections;

public class TestCanvasRenderer : MonoBehaviour {
    [Range(0, 1.0f)]
    public float fColorA;

    public CanvasRenderer canvaRd;
	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (canvaRd!=null)
        {
            canvaRd.SetAlpha(fColorA);
        }

    }
}
