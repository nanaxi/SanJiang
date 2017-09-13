using UnityEngine;
using System.Collections;

public class MoveAnima : MonoBehaviour {

    /// <summary>移动到的目标点
    /// </summary>
    [SerializeField]
    private Transform t_Target;

    /// <summary>移动结束
    /// </summary>
    private UnityEngine.Events.UnityAction onMoveEnd;

    /// <summary>移动速度
    /// </summary>
    [SerializeField]
    private float moveSpeed;

    /// <summary>移动到目标点 于目标点的误差值
    /// </summary>
    [SerializeField]
    private float errorValue;

    [SerializeField]
    private bool isMove;

    [SerializeField]
    private bool isMoveOver;
    

    public bool IsMoveOver
    {
        get
        {
            return isMoveOver;
        }

        set
        {
            isMoveOver = value;
        }
    }

    // Use this for initialization
    void Start () {
	
	}

    /// <summary>初始化移动到指定目标点
    /// tTarget = 目标点; isStart=是否自动开始;mSpeed = 速度;eValue=误差值;mEndAction=;
    /// </summary>
    /// <param name="tTarget">目标点</param>
    /// <param name="isStart">是否自动开始</param>
    /// <param name="mSpeed">速度</param>
    /// <param name="eValue">与目标距离在误差值以内可以停止</param>
    /// <param name="mEndAction">当已经移动到了目标点将执行的委托</param>
    /// <param name="isDestoryThe">是否自毁该移动脚本</param>
    public void Init_Var(Transform tTarget,bool isStart, float mSpeed , float eValue ,UnityEngine.Events.UnityAction mEndAction )
    {
        t_Target = tTarget;
        moveSpeed = mSpeed;
        errorValue = eValue;
        onMoveEnd = mEndAction;

        isMove = isStart;
    }

	// Update is called once per frame
	void Update () {

        if (isMove)
        {
            if (Vector3.Distance(transform.position, t_Target.position) >= errorValue)
            {
                transform.position = Vector3.MoveTowards(transform.position, t_Target.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                if (onMoveEnd != null)
                {
                    onMoveEnd.Invoke();
                }

                //isMove = false;
                IsMoveOver = true;
            }
        }
	}
}
