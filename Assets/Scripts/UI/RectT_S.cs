using UnityEngine;
using UnityEngine.UI;

public class RectT_S
{
    //private static bool isSetCanvasPX = false;

    /// <summary>找到画布，并设置分辨率
    /// </summary>
    private static void SetCanvas_PX()
    {
        Component.FindObjectOfType<Canvas>().GetComponent<CanvasScaler>().referenceResolution = new Vector2(Screen.width, Screen.height);
    }

    /// <summary>传一个实例化的窗口父物体， 一个实例化之前Res文件夹里面的原件作为参考，来设置分辨率
    /// </summary>
    static public void Set_Sc_Px(Transform t_Ins, Transform t_Res)
    {
        //if (isSetCanvasPX == false)
        //{
        //    SetCanvas_PX();
        //    isSetCanvasPX = true;
        //}
        
        RectTransform[] ins_Ary = t_Ins.GetComponentsInChildren<RectTransform>();
        RectTransform[] res_Ary = t_Res.GetComponentsInChildren<RectTransform>();
        //Debug.Log("LOOK  SetRecTT LengTh" + ins_Ary.Length +" Res LengTh"+res_Ary.Length);
        for (int i = 0; i < res_Ary.Length; i++)
        {
            ins_Ary[i].pivot = res_Ary[i].pivot;
            ins_Ary[i].anchorMin = res_Ary[i].anchorMin;
            ins_Ary[i].anchorMax = res_Ary[i].anchorMax;
            ins_Ary[i].localScale = res_Ary[i].localScale;

            ins_Ary[i].anchoredPosition = new Vector2((res_Ary[i].anchoredPosition.x / 1920) * Screen.width, (res_Ary[i].anchoredPosition.y / 1080) * Screen.height);/// ;res_Ary[i].anchoredPosition;
            ins_Ary[i].rotation = res_Ary[i].rotation;
            if (Mathf.Abs(res_Ary[i].sizeDelta.x - res_Ary[i].sizeDelta.y) < 10)
            {
                ins_Ary[i].sizeDelta = new Vector2((res_Ary[i].sizeDelta.x / 1080) * Screen.height, (res_Ary[i].sizeDelta.y / 1080) * Screen.height);// res_Ary[i].sizeDelta;
            }
            else
            {
                ins_Ary[i].sizeDelta = new Vector2((res_Ary[i].sizeDelta.x / 1920) * Screen.width, (res_Ary[i].sizeDelta.y / 1080) * Screen.height);// res_Ary[i].sizeDelta;
            }

            if (ins_Ary[i].GetComponent<Image>()!=null && res_Ary[i].GetComponent<Image>()!=null)
            {
                ins_Ary[i].GetComponent<Image>().type = res_Ary[i].GetComponent<Image>().type;
            }
        }

        GridLayoutGroup[] gL1 = t_Ins.GetComponentsInChildren<GridLayoutGroup>();
        GridLayoutGroup[] gL2 = t_Res.GetComponentsInChildren<GridLayoutGroup>();
        for (int i = 0; i < gL1.Length; i++)
        {
            gL1[i].padding = gL2[i].padding;
            gL1[i].cellSize = new Vector2((gL2[i].cellSize.x / 1920) * Screen.width, (gL2[i].cellSize.y / 1080) * Screen.height);// gL2[i].cellSize;
            gL1[i].spacing = new Vector2((gL2[i].spacing.x / 1920) * Screen.width, (gL2[i].spacing.y / 1080) * Screen.height); //gL2[i].spacing;
            gL1[i].startCorner = gL2[i].startCorner;
            gL1[i].startAxis = gL2[i].startAxis;
            gL1[i].childAlignment = gL2[i].childAlignment;
            gL1[i].constraint = gL2[i].constraint;
            gL1[i].constraintCount = gL2[i].constraintCount;

            gL1[i].enabled = gL2[i].enabled;
        }
    }
    static public void Set_PointAndSize_Px(Transform t_Ins, Transform t_Res)
    {

        t_Ins.gameObject.name = t_Res.gameObject.name;
        RectTransform ins_Ary = t_Ins.GetComponent<RectTransform>();
        RectTransform res_Ary = t_Res.GetComponent<RectTransform>();

        ins_Ary.pivot = res_Ary.pivot;
        ins_Ary.anchorMin = res_Ary.anchorMin;
        ins_Ary.anchorMax = res_Ary.anchorMax;
        ins_Ary.localScale = res_Ary.localScale;
        ins_Ary.anchoredPosition3D = res_Ary.anchoredPosition3D;// new Vector2((res_Ary.anchoredPosition.x / 1920) * Screen.width, (res_Ary.anchoredPosition.y / 1080) * Screen.height);/// ;res_Ary.anchoredPosition;
        ins_Ary.sizeDelta = res_Ary.sizeDelta;// new Vector2((res_Ary.sizeDelta.x / 1080) * Screen.height, (res_Ary.sizeDelta.y / 1080) * Screen.height);// res_Ary.sizeDelta;
        

        if (ins_Ary.GetComponent<Image>() != null && res_Ary.GetComponent<Image>() != null)
        {
            ins_Ary.GetComponent<Image>().type = res_Ary.GetComponent<Image>().type;
        }

    }
    static public void ResetRectT_Value(RectTransform rectT_)
    {
        rectT_.localScale = Vector3.one;
        //rectT_.localRotation = new Quaternion();
    }
}