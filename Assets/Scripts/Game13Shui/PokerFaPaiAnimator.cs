using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>扑克13水，关于发牌动画
/// </summary>
public class PokerFaPaiAnimator : MonoBehaviour {

    public Transform[] targetLayoutAry;
    public Image[] pokerBmAry;
    public Transform pokerBmParnet;
    public float fMoveSpeed =500;
    [SerializeField]private float waitTimeX = 0.5f;

    public int notFPAnimaIndex;

    // Use this for initialization
    void Awake () {
        pokerBmAry = pokerBmParnet.GetComponentsInChildren<Image>(true);
        
    }

    //void OnGUI()
    //{
    //    if (GUILayout.Button("Start XXX"))
    //    {
    //        Anima_Start();//测试开始发牌动画
    //    }

    //    if (GUILayout.Button("Rest "))
    //    {
    //        Anima_RecycleALL();
    //    }
    //}

    IEnumerator StartFaPai()
    {
        Debug.Log("Start TIME.time"+Time.time);
        for (int i = 0; i < targetLayoutAry.Length; i++)
        {//空位不发牌
            targetLayoutAry[i].gameObject.SetActive(i != notFPAnimaIndex);
        }
        for (int i_1 = 0; i_1 < 13; i_1++)
        {
            for (int i = 0; i < targetLayoutAry.Length; i++)
            {
                Transform pokerIns = PokerBm_Get();
                if (pokerIns==null)
                {
                    break;
                }
                pokerIns.SetParent(transform);
                //Transform layoutParentIns = targetLayoutAry[i];
                //MoveAnima moveTarget = pokerIns.gameObject.AddComponent<MoveAnima>();
                //moveTarget.Init_Var(layoutParentIns, true,1024,128,delegate() {
                //    pokerIns.SetParent(layoutParentIns);
                //    pokerIns.localScale = Vector3.one;
                //    pokerIns.position = Vector3.zero;
                //    if (pokerIns.GetComponent<MoveAnima>()!=null)
                //    {
                //        Destroy(pokerIns.GetComponent<MoveAnima>());
                //    }
                //});
                StartCoroutine(MoveAnima(pokerIns, targetLayoutAry[i]));
                yield return new WaitForEndOfFrame();
                pokerIns.SetParent(targetLayoutAry[i]);
                pokerIns.localScale = Vector3.one;
                pokerIns.position = Vector3.zero;
                //yield return new WaitForSeconds(0.1f);
                //yield return new WaitForEndOfFrame();
            }
        }
        Debug.Log("END TIME.time" + Time.time);
        yield return null;
    }

    IEnumerator MoveAnima(Transform pokerIns,Transform pokerLayout)
    {
        float jlWaitTime = waitTimeX;
        while (gameObject.activeInHierarchy && jlWaitTime > 0 && pokerIns != null && pokerLayout != null)
        {
            pokerIns.position = Vector3.MoveTowards(pokerIns.position, pokerLayout.position, fMoveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();//yield return new WaitForSeconds(Time.deltaTime);
            jlWaitTime -= Time.deltaTime;
        }
        //yield return new WaitForEndOfFrame();
        yield return null;
    }

    Transform PokerBm_Get()
    {
        for (int i = pokerBmAry.Length-1; i >=0; i--)
        {
            if (pokerBmAry[i].transform.parent == pokerBmParnet)
            {
                return pokerBmAry[i].transform;
            }
        }
        return null;
    }

    public void Anima_Start(int notIndex = 11)
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        notFPAnimaIndex = notIndex;
        StopCoroutine("StartFaPai");
        Anima_RecycleALL();
        StartCoroutine("StartFaPai");
    }

    public void Anima_RecycleALL()
    {
        for (int i = 0; i < pokerBmAry.Length; i++)
        {
            pokerBmAry[i].transform.SetParent(pokerBmParnet);
        }
    }
}
