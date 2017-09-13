using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uiWin_DzpAskVote : MonoBehaviour
{
    public Button Conform;
    public Button Cancle;
    // Use this for initialization
    void Start()
    {
        if (Conform != null)
        {
            Conform.onClick.AddListener(delegate
            {
                DZP_PublicEvent.GetINS.VoteRequest(true);
                Destroy(gameObject);
            });
        }
        if (Cancle != null)
        {
            Cancle.onClick.AddListener(delegate {
                Destroy(gameObject);
            });
        }
    }

}
