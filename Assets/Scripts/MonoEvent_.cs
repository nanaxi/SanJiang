using UnityEngine;
using UnityEngine.Events;

public class MonoEvent_ : MonoBehaviour {

    public UnityEvent onDestyoy_ = new UnityEvent();
    public UnityEvent onDisable_ = new UnityEvent();
    public UnityEvent onEnable_ = new UnityEvent();


    //void Awake()
    //{
    //    Debug.Log("Awake~");
    //}
    //// Use this for initialization
    //void Start () {
    //    Debug.Log("Start~");
    //}

    //public void Test_01()
    //{
    //    Debug.Log("Test_Event~");
    //}

    /// <summary>当销毁
    /// </summary>
    void OnDestroy()
    {
        //print("Script was destroyed");
        onDestyoy_.Invoke();
    }

    /// <summary>当不可用
    /// </summary>
    void OnDisable()
    {
        //print("script was removed");
        onDisable_.Invoke();
    }

    /// <summary>当可用
    /// </summary>
    void OnEnable()
    {
        //print("script was enabled");
        onEnable_.Invoke();
    }

}
