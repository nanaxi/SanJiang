using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_PlayerInfoX1 : MonoBehaviour {

    [SerializeField]
    private Text t_Name;
    [SerializeField]
    private Text t_ID;
    [SerializeField]
    private Text t_IP;
    private Player_Data playerData;

    Canvas canvas;
    Vector2 pos;

    VectorX2 v2X2;

    public Player_Data PlayerData
    {
        get
        {
            return playerData;
        }

        private set
        {
            playerData = value;
        }
    }

    private RectTransform cacheMtRectT;
    private RectTransform cacheParent;
    public RectTransform testRectTarget;
    // Use this for initialization
    void Awake() {
        cacheMtRectT = GetComponent<RectTransform>();
        cacheParent = transform.parent.GetComponent<RectTransform>();
    }

    void SetV2X2()
    {
        canvas = canvas == null ? GetComponentInParent<Canvas>() : canvas;
        cacheMtRectT.pivot = new Vector2(0.5f, 0.5f);

    }
    public void SetPlayerInfoUI(Player_Data p_Data, Transform rectTarget)
    {
        if (p_Data != null)
        {
            PlayerData = p_Data;
            t_Name.text = PlayerData.P_Name;
            t_ID.text = PlayerData.p_ID.ToString();
            t_IP.text = PlayerData.p_Ip.ToString();
        }

        canvas = canvas == null ? GetComponentInParent<Canvas>() : canvas;

        if (rectTarget != null)
        {
            cacheMtRectT.SetParent(rectTarget);
            cacheMtRectT.localScale = Vector3.one;
            cacheMtRectT.anchoredPosition3D = Vector3.zero;
            cacheMtRectT.SetParent(cacheParent);
            //StartCoroutine(WaitTimeSetIndex());
        }
        gameObject.SetActive(true);
    }

    IEnumerator WaitTimeSetIndex()
    {
        yield return new WaitForEndOfFrame();
        cacheMtRectT.SetParent(cacheParent);
    }

    /// <summary>【拖拽赋值？】  礼物按钮 事件
    /// </summary>
    /// <param name="giftBtn"></param>
    public void OnClick_Btn_Gift(GameObject giftBtn)
    {
        int id = 0;
        try
        {
            id = int.Parse(giftBtn.name.Replace("Btn_Gift",""));
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        GiftSX giftSX = GiftConfig.Instance.Gift_GetConfig(id);
        if (giftSX == null)
        {
            return;
        }
        Debug.Log(giftSX.name + "你想发送礼物给玩家 " + PlayerData.P_Name);
        gameObject.SetActive(false);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        SetPlayerInfoUI(null, testRectTarget);
    //    }
    //}
}

public class VectorX2
{
    public Vector2 v2_Min;
    public Vector2 v2_Max;
    public VectorX2()
    {
    }
}