using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RayTest : MonoBehaviour
{
    public Camera _camera;
    public Transform t_MjSelect;
    public Transform t_ShouPaiParent;
    public MJInfos[] t_ShouAry;
    //public Vector3[] v3_PositionAry;
    public float mj_Distance;
    public Vector2 v2_M1;
    [SerializeField]private Mj_ClickHight mjClickM;
    public GameObject gm_MoPaiMove;
    // Use this for initialization
    void Start()
    {
        mj_Distance = Vector3.Distance(t_ShouPaiParent.GetChild(0).position, t_ShouPaiParent.GetChild(1).position);
        t_ShouAry = t_ShouPaiParent.GetComponentsInChildren<MJInfos>();
        //v3_PositionAry = new Vector3[t_ShouAry.Length];
        mjClickM = new Mj_ClickHight();

        for (int i = 1; i < t_ShouAry.Length; i++)
        {
            Vector3 v3_ = t_ShouAry[i-1].transform.position;
            v3_.x += mj_Distance;
            if (i==t_ShouAry.Length-1)
            {
                //v3_.x += mj_Distance*0.5f;
            }
            t_ShouAry[i].transform.position = v3_;
            //v3_PositionAry[i] = t_ShouAry[i].transform.position;
        }
        List<uint> i_TestAry = new List<uint>();
        for (int i = 0; i < 13; i++)
        {
            i_TestAry.Add(DataManage.Instance.mjCardId_Ary[Random.Range(0, 33)]);
        }
        i_TestAry.Sort();

        for (int i = 0; i < i_TestAry.Count; i++)
        {
            MJInstantiate_( NeedDL.DicValueGetKey<string, uint>(i_TestAry[i],DataManage.Instance.dicMJSize) ,t_ShouAry[i].transform);
        }
        string mjName1 = NeedDL.DicValueGetKey<string, uint>(DataManage.Instance.mjCardId_Ary[Random.Range(0, 34)], DataManage.Instance.dicMJSize);
        gm_MoPaiMove = MJInstantiate_(mjName1, t_ShouAry[t_ShouAry.Length - 1].transform);

        int i1Index = ForTrim_My_MaJaing(gm_MoPaiMove.transform);
        i1Index = i1Index == 0 ? i1Index : i1Index + 1;
        i1Index = Mathf.Clamp(i1Index,0,t_ShouAry.Length-2);
        StartCoroutine(Move_1(i1Index, gm_MoPaiMove.transform));

        //ForTrim_My_MaJaing();
        _camera = GetComponent<Camera>();
    }

    public GameObject MJInstantiate_(string strName,Transform tParent)
    {
        GameObject gmRes = Resources.Load<GameObject>("Prefabs/MJ/"+strName);
        Debug.Log(""+ strName);
        GameObject gmIns = Instantiate(gmRes) as GameObject;
        gmIns.name = strName;
        tParent.gameObject.name = strName;
        gmIns.transform.SetParent(tParent);
        gmIns.layer = tParent.gameObject.layer;
        gmIns.transform.localEulerAngles = Vector3.zero;
        //gmIns.transform.localPosition = new Vector3(0, 0, 0.08f);
        gmIns.transform.localScale = Vector3.one;
        gmIns.transform.localPosition = Vector3.zero;
        return gmIns;
    }

    public int ForTrim_My_MaJaing(Transform my_Mj)
    {
        uint iValue = DataManage.Instance.Card_GetSize(my_Mj.gameObject.name);
        if (iValue < DataManage.Instance.Card_GetSize(t_ShouAry[0].gameObject.name))
        {
            return 0;
        }
        for (int i = 0; i < t_ShouAry.Length - 2; i++)
        {
            if (t_ShouAry[i].gameObject.name.Length==2 && t_ShouAry[i + 1].gameObject.name.Length == 2 && iValue >= DataManage.Instance.Card_GetSize(t_ShouAry[i].gameObject.name) && 
                iValue < DataManage.Instance.Card_GetSize(t_ShouAry[i + 1].gameObject.name))
            {
                Debug.Log("INDEX :" + i);
                return i;
            }
            else if (t_ShouAry[i].transform.childCount==0)
            {
                return i-1;
            }
        }
        return t_ShouAry.Length - 1;
    }

    IEnumerator Move_1(int i_Index,Transform t_)
    {
        float jlDic = Vector3.Distance(t_ShouAry[0].transform.position,t_ShouAry[1].transform.position);

        t_ShouAry[i_Index].gameObject.name = "NULL";
        Transform tParenJL1 = t_ShouAry[i_Index].transform;
        yield return new WaitForSeconds(0.6f);
        for (int i = i_Index; i < t_ShouAry.Length-2; i++)
        {
            if (t_ShouAry[i].transform.childCount>0)
            {
                int iCount1 = t_ShouAry[i + 1].transform.childCount;
                Transform t_ChildMJ = t_ShouAry[i].transform.GetChild(0);
                t_ChildMJ.SetParent(t_ShouAry[i + 1].transform);
                t_ChildMJ.localPosition = Vector3.zero;
                //yield return new WaitForEndOfFrame();
                t_ChildMJ.parent.name = t_ChildMJ.gameObject.name;
                if (iCount1 == 0)
                {
                    break;
                }
                //yield return new WaitForSeconds(1f);
            }

        }
        yield return new WaitForSeconds(0.1f);
        gm_MoPaiMove.transform.SetParent(tParenJL1);
        gm_MoPaiMove.transform.parent.gameObject.name = gm_MoPaiMove.name;
        Vector3 jlPosition1 = gm_MoPaiMove.transform.position;
        Vector3 jlPosition2 = gm_MoPaiMove.transform.parent.position;
        jlPosition1.y += 0.1f;
        jlPosition2.y += 0.1f;
        gm_MoPaiMove.transform.position = jlPosition1;
        yield return new WaitForSeconds(0.2f);
        while (Vector3.Distance(gm_MoPaiMove.transform.position,jlPosition2)>0.0001f)
        {
            gm_MoPaiMove.transform.position = Vector3.MoveTowards(gm_MoPaiMove.transform.position, jlPosition2,Time.deltaTime*5);
            yield return new WaitForEndOfFrame();
        }
        jlPosition2 = gm_MoPaiMove.transform.parent.position;
        yield return new WaitForSeconds(0.1f);
        while (Vector3.Distance(gm_MoPaiMove.transform.position, jlPosition2) > 0.0001f)
        {
            gm_MoPaiMove.transform.position = Vector3.MoveTowards(gm_MoPaiMove.transform.position, jlPosition2, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        gm_MoPaiMove.transform.localPosition = Vector3.zero;
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到
                GameObject gameObj = hitInfo.collider.gameObject;
                Debug.Log("click object name is " + gameObj.name);
                t_MjSelect = gameObj.transform;
                if (t_MjSelect.parent == t_ShouPaiParent && t_MjSelect != null && t_MjSelect.GetComponent<BoxCollider>() != null)
                {
                    if (mjClickM.mj_Select_Object == null)
                    {
                        mjClickM = new Mj_ClickHight(t_MjSelect, mj_Distance * 0.5f);
                        mjClickM.Mj_Click_Move(true);
                    }
                    else
                    {
                        if (mjClickM.mj_Select_Object == t_MjSelect)
                        {
                            mjClickM.Mj_Click_Move(!mjClickM.isSelect);
                        }
                        else
                        {
                            mjClickM.Mj_Click_Move(false);
                            mjClickM = new Mj_ClickHight(t_MjSelect, mj_Distance * 0.5f);
                            mjClickM.Mj_Click_Move(true);
                        }
                    }
                }
            }
        }
    }
}


#region/*———测试代码———*/
//if (Input.GetMouseButtonDown(0))
//{
//    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
//    RaycastHit hitInfo;
//    if (Physics.Raycast(ray, out hitInfo))
//    {
//        Debug.DrawLine(ray.origin, hitInfo.point);//划出射线，只有在scene视图中才能看到
//        GameObject gameObj = hitInfo.collider.gameObject;
//        Debug.Log("click object name is " + gameObj.name);
//        t_MjSelect = gameObj.transform;
//        if (t_MjSelect.parent==t_ShouPaiParent&&t_MjSelect != null && t_MjSelect.GetComponent<BoxCollider>() != null)
//        {
//            t_MjSelect.GetComponent<BoxCollider>().enabled = false;
//        }
//        if (gameObj.tag == "boot")//当射线碰撞目标为boot类型的物品 ，执行拾取操作
//        {
//            Debug.Log("pick up!");
//        }
//    }
//}

//if (Input.GetMouseButton(0))
//{
//    Ray ray = _camera.ScreenPointToRay(Input.mousePosition);//从摄像机发出到点击坐标的射线
//    RaycastHit hitInfo;
//    if (Physics.Raycast(ray, out hitInfo))
//    {
//        Debug.DrawLine(ray.origin, hitInfo.point, Color.red);//划出射线，只有在scene视图中才能看到
//        GameObject gameObj = hitInfo.collider.gameObject;

//        if (t_MjSelect != null)
//        {
//            t_MjSelect.position = hitInfo.point;
//        }

//        Vector3 v3_1;
//        if (gameObj.transform != t_MjSelect && gameObj.transform.parent == t_MjSelect.parent)
//        {
//            //v3_1 = Move_1(gameObj.transform);
//        }
//    }
//}
#endregion