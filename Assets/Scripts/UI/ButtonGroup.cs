using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/// <summary>Click Select Btn Style
/// </summary>
public class ButtonGroup : MonoBehaviour {

    public Button initBtn;
    public Button[] btnAry;
    public Sprite spriteStandby, spriteSelected;
    public Color color_Standby, color_Selected;

    public bool isGetAllChildBtn = false;
    // Use this for initialization
    void Awake () {

        if (isGetAllChildBtn)
        {
            btnAry =  GetComponentsInChildren<Button>();
        }
        
        for (int i = 0; i < btnAry.Length; i++)
        {
            BtnGroupStyle btnStyle =  btnAry[i].gameObject.AddComponent<BtnGroupStyle>();
            Button btn_ = btnAry[i].gameObject.GetComponent<Button>();
            //btn_.onClick.RemoveListener();
            btnStyle.onClick_.AddListener(delegate() {
                Btn_Select(btn_);
            });
        }
    }

    void OnEnable()
    {
        if (initBtn!=null)
        {
            Btn_Select(initBtn);
        }
    }

    public void Btn_Select(Button btn_)
    {
        
        for (int i = 0; i < btnAry.Length; i++)
        {
            Sprite sprite_XX = btnAry[i].image.sprite;
            btnAry[i].image.sprite = btn_.gameObject == btnAry[i].gameObject? spriteSelected : spriteStandby;
            btnAry[i].image.sprite = btnAry[i].image.sprite == null ? sprite_XX : btnAry[i].image.sprite;
            btnAry[i].image.color = btn_.gameObject == btnAry[i].gameObject ? color_Selected : color_Standby;
        }
    }
    
}

public class BtnGroupStyle : MonoBehaviour, IPointerUpHandler
{
    public UnityEngine.Events.UnityEvent onClick_ = new UnityEngine.Events.UnityEvent();
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onClick_!=null)
        {
            onClick_.Invoke();
        }
    }
}