using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>缓存池——
/// 《路径池》描述：非单例。当根据Name获取GameObject为空时，实例化预制路径+Name.重新获取并返回。
/// </summary>
public class PublicPathPool : MonoBehaviour, IBasePool
{

    public string poolResourcesPath;
    [SerializeField]
    private Transform tPoolParent;
    [SerializeField]
    private List<GameObject> listPool = new List<GameObject>();
    public UnityEngine.Events.UnityEvent onAwake = new UnityEngine.Events.UnityEvent();

    // Use this for initialization
    void Awake()
    {
        if (onAwake != null)
        {
            onAwake.Invoke();
        }
    }

    /// <summary>可能会拖拽给 ：onAwake
    /// </summary>
    public void Init_PoolAddObj()
    {
        GameObject[] gmAry = Resources.LoadAll<GameObject>(poolResourcesPath);
        for (int i = 0; i < gmAry.Length; i++)
        {
            PoolAddGameObject(gmAry[i]);
        }
    }

    private void PoolAddGameObject(string gmName)
    {
        GameObject gmRes = Resources.Load<GameObject>(poolResourcesPath + gmName);
        if (gmRes == null)
        {
            Debug.LogError("Resources.Load<GameObject>" + poolResourcesPath + gmName + " == NULL");
        }
        PoolAddGameObject(gmRes);
    }

    private void PoolAddGameObject(GameObject gmRes)
    {
        GameObject gmIns = Instantiate(gmRes);
        PoolObj_Info pObj = gmIns.AddComponent<PoolObj_Info>();
        pObj.objName = gmRes.name;
        pObj.IsUse = false;
        gmIns.name = gmRes.name;
        gmIns.transform.SetParent(tPoolParent);
        gmIns.transform.localScale = Vector3.one;
        gmIns.SetActive(false);
        listPool.Add(gmIns);
    }

    public GameObject PoolGetGameObject(string gmName, Transform tParent, bool isInit = true)
    {
        GameObject gmIns = PoolGetGameObject(gmName);
        gmIns.transform.SetParent(tParent);

        if (isInit)
        {
            gmIns.transform.localScale = Vector3.one;
        }

        return gmIns;
        //throw new NotImplementedException();
    }

    public GameObject PoolGetGameObject(string gmName)
    {
        for (int i = 0; i < listPool.Count; i++)
        {
            if (listPool[i].GetComponent<PoolObj_Info>().IsUse == false && listPool[i].name == gmName)//listPool[i].activeInHierarchy == false && listPool[i].transform.parent == tPoolParent && listPool[i].name == gmName)
            {
                listPool[i].SetActive(true);
                listPool[i].GetComponent<PoolObj_Info>().IsUse = true;
                return listPool[i];
            }
        }
        PoolAddGameObject(gmName);
        return PoolGetGameObject(gmName);
        //throw new NotImplementedException();
    }

    public void PoolRecycle(GameObject recycleGmObj)
    {
        if (recycleGmObj==null)
        {
            Debug.Log("<color=red> PoolRecycle GameObject == NULL </color>");
            return;
        }
        if (recycleGmObj.GetComponent<Button>()!=null)
        {
            recycleGmObj.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        if (recycleGmObj.GetComponent<PoolObj_Info>() != null)
        {
            recycleGmObj.name = recycleGmObj.GetComponent<PoolObj_Info>().name;
            recycleGmObj.GetComponent<PoolObj_Info>().IsUse = false;
        }
        recycleGmObj.SetActive(false);
        recycleGmObj.transform.SetParent(tPoolParent);
        //throw new NotImplementedException();
    }

    public void PoolRecycle(GameObject[] recycleGmObjAry)
    {
        for (int i = 0; i < recycleGmObjAry.Length; i++)
        {
            PoolRecycle(recycleGmObjAry[i]);
        }
    }

    public void PoolRecycle<MonoB>(MonoB[] recycleGmObjAry) where MonoB : MonoBehaviour
    {
        for (int i = 0; i < recycleGmObjAry.Length; i++)
        {
            PoolRecycle(recycleGmObjAry[i].gameObject);
        }
    }

    /// <summary>回收ListPool 的所有对象
    /// </summary>
    public void PoolRecycleAll()
    {
        for (int i = 0; i < listPool.Count; i++)
        {
            if (listPool[i]!=null)
            {
                PoolRecycle(listPool[i]);
            }
        }
    }

    public void PoolClearAll()
    {
        for (int i = 0; i < listPool.Count; i++)
        {
            if (listPool[i] != null)
            {
                Destroy(listPool[i]);
            }
        }
        listPool.Clear();
        //throw new NotImplementedException();
    }

}

public interface IBasePool
{
    GameObject PoolGetGameObject(string gmName);

    void PoolRecycle(GameObject recycleGmObj);

    void PoolClearAll();
}

public class PoolObj_Info : MonoBehaviour
{
    public string objName;
    private bool isUse;

    public bool IsUse
    {
        get
        {
            return isUse;
        }

        set
        {
            isUse = value;
        }
    }
}