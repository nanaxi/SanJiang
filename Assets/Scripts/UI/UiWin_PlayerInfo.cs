using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_PlayerInfo : UiWin_Parent {

    public Button btnHead;
    public Text[] textAry;
    [SerializeField]private Player_Data p_TheData;
    
	// Use this for initialization
	void Start ()
    {
        if (!isInitVar)
        {
            Init_Var();
        }
        Set_OnEventList<Button>(GetComponentsInChildren<Button>());

    }

    public void Init_Var()
    {
        textAry = transform.FindChild("Img_Bg0/Img_TBg").GetComponentsInChildren<Text>();
        btnHead = transform.FindChild("Img_Bg0/Head").GetComponentInChildren<Button>();
        isInitVar = true;
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        //base.Set_OnEventList<Component_>(all_);
        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                UnityEngine.Events.UnityAction btnOnClck_ = null;
                switch (btn_.gameObject.name)
                {
                    case "Btn_CloseWin":
                        btnOnClck_ = delegate () {
                            //Destroy(gameObject);
                            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_PlayerInfo);
                        };
                        break;
                    case "shoppingButton":
                        btnOnClck_ = delegate () {
                            if (UIManager.Instance.FindUI(AllPrefabName.uiWin_Home).activeInHierarchy)
                            {
                                UIManager.Instance.FindUI(AllPrefabName.uiWin_Home).GetComponent<UiWin_Home>().OnClick_Btn_OpenBuyPrompt();
                            }
                        };
                        break;
                    default:
                        break;
                }

                btn_.onClick.AddListener(delegate ()
                {
                    Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                    Debug.Log("Click Btn:" + btn_.gameObject.name);
                    if (btnOnClck_!=null)
                    {
                        btnOnClck_.Invoke();
                    }
                });
            }
        }
    }

    public void Set_PlayerInfoUI(Player_Data pData)
    {
        p_TheData = pData;
        if (!isInitVar)
        {
            Init_Var();
        }
        textAry[0].text ="名字："+ pData.P_Name;
        textAry[1].text = "ID：" + pData.p_ID.ToString();
        textAry[2].text = "IP：" + pData.p_Ip;
        textAry[3].text = "房卡：" + pData.p_Diamond.ToString();
        btnHead.image.sprite = DataManage.Instance.Head_GetSprite(pData.p_ID);
    }
}
