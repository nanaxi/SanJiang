using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWin_Seting : UiWin_Parent {

	// Use this for initialization
	void Start () {
        Set_OnEventList(GetComponentsInChildren<Slider>());
        Set_OnEventList(GetComponentsInChildren<Button>());
    }

    public override void Set_OnEventList<Component_>(Component_[] all_)
    {
        //base.Set_OnEventList<Component_>(all_);
        if (typeof(Component_) == typeof(Slider))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Slider sld_ = all_[i].GetComponent<Slider>();
                sld_.onValueChanged.AddListener(delegate (float f_)
                {
                    OnValueChanged_(sld_);
                });

                if (sld_.gameObject.name.IndexOf(All_Slider_OnDrag.Slider_Miusc_V.ToString()) >= 0)
                {
                    sld_.value = PlayerPrefs.GetFloat(Pp_Name.NxHs_Audio1V_.ToString());
                }
                else if (sld_.gameObject.name.IndexOf(All_Slider_OnDrag.Slider_Sound_V.ToString()) >= 0)
                {
                    sld_.value = PlayerPrefs.GetFloat(Pp_Name.NxHs_AudioBgV_.ToString());//初始化声音
                    Debug.Log("Set_Audio?");
                }
            }
        }

        if (typeof(Component_) == typeof(Button))
        {
            for (int i = 0; i < all_.Length; i++)
            {
                Button btn_ = all_[i].GetComponent<Button>();
                UnityEngine.Events.UnityAction btnOnClck_ = null;
                switch (btn_.gameObject.name)
                {
                    case "Btn_Confirm":
                        btnOnClck_ = delegate () {
                            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Setting);
                        };
                        break;
                    case "Btn_CloseWin":
                        btnOnClck_ = delegate () {
                            //Destroy(gameObject);
                            UIManager.Instance.DestroyObjectUI(AllPrefabName.uiWin_Setting);
                        };
                        break;
                    default:
                        break;
                }
                if (btnOnClck_ != null)
                {
                    btn_.onClick.AddListener(delegate ()
                    {
                        Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                        Debug.Log("Click Btn:" + btn_.gameObject.name);
                        btnOnClck_.Invoke();
                    });
                }
            }
        }
    }

    void OnValueChanged_(Slider sld_)
    {
        switch (sld_.gameObject.name)
        {
            case "Slider_Miusc_V":
                Audio_Manage.Instance.Set_AudioS_Volume(sld_.value);
                break;
            case "Slider_Sound_V":
                Audio_Manage.Instance.Set_AudioLister_V(sld_.value);
                break;
            default:
                break;
        }
    }
}

public enum Pp_Name
{
    NxHs_AudioBgV_, NxHs_Audio1V_, NxHs_IsRelink, NxHs_ACC
}
/// <summary>所有的滑动条
/// </summary>
public enum All_Slider_OnDrag
{
    Slider_Miusc_V, Slider_Sound_V
}