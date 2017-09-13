using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Model3dManage {
    #region Instance Define
    static Model3dManage inst = null;
    static public Model3dManage Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new Model3dManage();
            }
            return inst;
        }
    }
    #endregion

    private Dictionary<string, GameObject> dicUI = new Dictionary<string, GameObject>();
    // Use this for initialization
    void Start () {
	
	}

    public void Init()
    {

    }

    public GameObject ShowModel(string _modelName, Transform parent_)
    {
        if (dicUI.ContainsKey(_modelName))
        {
            if (dicUI[_modelName] != null)
            {
                dicUI[_modelName].transform.SetAsLastSibling();
                SetModelObject(true, _modelName);
                return dicUI[_modelName];
            }
            else
            {
                dicUI.Remove(_modelName);
            }
        }
        string path = "Prefabs/Models3D/" + _modelName;
        GameObject _prefab = Resources.Load(path) as GameObject;

        if (_prefab == null)
        {
            Debug.LogError(path);
            return null;
        }
        GameObject gmIns = GameObject.Instantiate(_prefab) as GameObject;
        //Debug.Log(""+parent_==null);

        gmIns.transform.SetParent(parent_);// GetAuchor(_anchor);

        dicUI.Add(_modelName, gmIns);

        return gmIns;
    }


    string FrontUIName = "";
    GameObject prefab = null;
    public GameObject ShowPerfabModel(string _modelName, Transform parent)
    {

        string path = "Prefabs/Models3D/" + _modelName;
        prefab = null;
        prefab = Resources.Load(path) as GameObject;

        if (prefab == null)
        {
            Debug.LogError("Prefabs/Models3D/" + _modelName);
            return null;
        }
        GameObject ui = GameObject.Instantiate(prefab) as GameObject;
        ui.transform.SetParent(parent);
        RectT_S.Set_PointAndSize_Px(ui.transform, prefab.transform);

        return ui;
    }
    public void DestroyObjectModel(string _modelName, float _waitTime = 0f)
    {
        if (dicUI.ContainsKey(_modelName) == true)
        {
            //BaseUI_C ui = BaseUI_C.GetBaseUI_C(dicUI[_uiName]);
            //ui.Destroy();
            if (dicUI[_modelName] != null)
            {
                UnityEngine.Object.Destroy(dicUI[_modelName], _waitTime);
            }
            dicUI.Remove(_modelName);
        }
    }

    public GameObject FindModel(string _modelName)
    {
        if (dicUI.ContainsKey(_modelName) == false)
        {
            return null;
        }
        return dicUI[_modelName];// BaseUI_C.GetBaseUI_C(dicUI[_uiName]);
    }

    public void SetModelObject(bool IsActive, string _modelName)
    {
        if (!dicUI.ContainsKey(_modelName))
        {
            Debug.Log(_modelName + "<color=red>SetUIobject == Null</color>");
            return;
        }
        dicUI[_modelName].SetActive(IsActive);
    }

}
