using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiWinX_Phiz : MonoBehaviour {

    public UnityEngine.Events.UnityAction onClickPhiz;
    [SerializeField]
    private ScrollRect sVRect_Phiz;//拖拽赋值，表情和常用语滑块

    private bool isInit = false;
    // Use this for initialization
    void Start () {
        Init_ShowUi();
    }

    public void Init_ShowUi()
    {
        if (isInit)
        {
            return;
        }
        string[] strPhiz = DataManage.Instance.Phiz_GetConfig();
        for (int i = 0; i < strPhiz.Length; i++)
        {///初始化实例化表情
            Button gmPhiz = UIManager.Instance.ShowPerfabUI(AllPrefabName.BtnPhiz, sVRect_Phiz.content).GetComponent<Button>();
            gmPhiz.gameObject.name = strPhiz[i];
            gmPhiz.image.sprite = Resources.Load<Sprite>(ResPath_Assets.sprite_Phiz + strPhiz[i]);
            gmPhiz.onClick.AddListener(delegate () {
                Audio_Manage.Instance.Play_Audio(AudioPath.btn_Click1);
                OnClick_Btn_Phiz_(gmPhiz.gameObject.name);
            });
        }
        isInit = true;
    }

    void OnClick_Btn_Phiz_(string btnName)
    {
        Debug.Log(btnName + "你想要播放表情？" + DataManage.Instance.Phiz_GetPath(btnName));
        PublicEvent.GetINS.SentMegssageImage(btnName);
        //OpenOrClose_UiWin();
    }
}
