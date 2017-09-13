using UnityEngine;
using System.Collections;

public class RectT_Marquee : MonoBehaviour {

    public RectTransform moveRectT;//被移动的对象
    [SerializeField]
    private Vector2 v2_StartPosition = new Vector2(1920, 0);//初始位置，跑马灯跑到结尾，回到的初始位置
    [SerializeField]private Vector2 v2_MovePosition = new Vector2(-4, 0);//跑马灯移动速度
    [SerializeField]
    private float f_WaitTimeMove = 0.01f;//每隔多少秒移动一下跑马灯

    public int SetMoveSpeed
    {
        set { v2_MovePosition.x = value; }
    }
    
    // Use this for initialization
    void Start() {
        if (moveRectT!=null)
        {
            moveRectT.anchoredPosition = v2_StartPosition;
            StartCoroutine("MoveAnima");
        }
    }

    IEnumerator MoveAnima()
    {
        while (gameObject.activeInHierarchy && moveRectT!=null)
        {
            if (Mathf.Abs(v2_MovePosition.x)>32)
            {
                v2_MovePosition.x = v2_MovePosition.x.ToString().IndexOf("-") >= 0 ? -32 : 32;
            }
            moveRectT.anchoredPosition += v2_MovePosition;
            yield return new WaitForEndOfFrame();
            if (moveRectT.anchoredPosition.x< -(moveRectT.sizeDelta.x))
            {
                moveRectT.anchoredPosition = v2_StartPosition;
            }
            yield return new WaitForSeconds(f_WaitTimeMove);
        }
        yield return null;
    }

    public void OnEnable()
    {
        StopCoroutine("MoveAnima");
        StartCoroutine("MoveAnima");
    }
}
