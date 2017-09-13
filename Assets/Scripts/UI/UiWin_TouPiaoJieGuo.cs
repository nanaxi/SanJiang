using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>投票结果
/// </summary>
public class UiWin_TouPiaoJieGuo : UiWin_Parent {
    public Player_Data[] roomPlayerData;
    public Text[] touPiaoTog;
    public Text t_PId_Start_TouPiao;
    public Button btn_Yes, btn_No;
    
    private GameObject img_Btn_Mask;
    public bool yn_Open = false;
    public GameObject uiWin_Tpjg;
    public int yn_Click;

    [SerializeField]private Sprite sprite_Tpjg_Y;
    [SerializeField]
    private Sprite sprite_Tpjg_N;

    public void OpenOrClsoe_MaskBtn( bool yn_O)
    {
        if (img_Btn_Mask == null)
        {
            return;
        }
        img_Btn_Mask.SetActive(yn_O);
    }

    public void Init_UI( Player_Data[] roomPlayerD_, string p_Name)
    {
        if (!yn_Open)
        {
            uiWin_Tpjg = gameObject;
            btn_Yes = transform.FindChild("Img_Bg0/Btn_Y").GetComponent<Button>();
            btn_No = transform.FindChild("Img_Bg0/Btn_N").GetComponent<Button>();
            btn_Yes.onClick.AddListener(delegate () {
                Debug.Log("btn_Yes.onClick");
                PublicEvent.GetINS.VoteRequest(true); });
            btn_No.onClick.AddListener(delegate() {
                Debug.Log("btn_No.onClick");
                PublicEvent.GetINS.VoteRequest(false); });
            //img_Btn_Mask = transform.FindChild("Img_Btn_Mask").gameObject;
            //img_Btn_Mask.SetActive(false);
            yn_Click = 0;
            this.touPiaoTog = transform.FindChild("Img_Bg0/TPJG_Bg0").GetComponentsInChildren<Text>();
            t_PId_Start_TouPiao = transform.FindChild("Img_Bg0/T_Title").GetComponent<Text>();
            t_PId_Start_TouPiao.text = "玩家：<color=red>" + p_Name + "</color> 发起解散房间投票，请投票！";
            this.roomPlayerData = roomPlayerD_;

            for (int i = 0; i < roomPlayerData.Length; i++)
            {
                touPiaoTog[i].GetComponentInChildren<Text>().text = roomPlayerData[i].P_Name;
                touPiaoTog[i].GetComponentInChildren<Image>().enabled = false;
            }
            
            yn_Open = true;
        }
        
    }

    public void Set_QuiaRoom_TouPiao(uint playerId, bool yn_TongYi)
    {
        for (int i = 0; i < roomPlayerData.Length; i++)
        {
            if (playerId == roomPlayerData[i].p_ID)
            {
                touPiaoTog[i].GetComponentInChildren<Image>().enabled = true;
                touPiaoTog[i].GetComponentInChildren<Image>().sprite = yn_TongYi ? sprite_Tpjg_Y : sprite_Tpjg_N;
                if (playerId == DataManage.Instance.MyPlayer_Data.p_ID)
                {
                    btn_Yes.interactable = false;
                    btn_No.interactable = false;
                }
            }
        }
    }
    // Use this for initialization
    void Start () {
	
	}
	

}
