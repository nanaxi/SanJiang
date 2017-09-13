using UnityEngine;
using System.Collections;

public abstract class SingletonParent<SingletonType> where SingletonType : new()
{
    #region Instance Define
    static SingletonType inst = default(SingletonType); //null;
    /// <summary>单例
    /// </summary>
    static public SingletonType Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new SingletonType();
            }
            return inst;
        }
    }
    #endregion
    
    public abstract void Init();
    //{
    //}
}

public class Test_Manage1 : SingletonParent<Test_Manage1>
{
    public override void Init()
    {
        Debug.Log("Test_Manage1 。Init()");
    }
}
