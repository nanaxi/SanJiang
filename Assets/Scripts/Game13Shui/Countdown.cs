using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>倒计时
/// 1、设置当时间改变delegate —> onChangeTime、当时间结束delegate —>onTimeZero
/// 2、开始倒计时 CountDown_Start(int 最大倒计时);
/// </summary>
public class Countdown : MonoBehaviour
{
    public delegate void OnChangeInt(int time);
    public delegate void OnIntZero();

    /// <summary>当倒计时改变
    /// </summary>
    public OnChangeInt onChangeTime;

    /// <summary>当倒计时为 0
    /// </summary>
    public OnIntZero onTimeZero;

    public Text tShowTime;

    [SerializeField]
    private int timeValue;

    public int TimeValue
    {
        get
        {
            return timeValue;
        }

        private set
        {
            timeValue = value;
            if (tShowTime != null)
            {
                tShowTime.text = timeValue.ToString();
            }

            if (onChangeTime != null)
            {
                //Debug.Log("Change CountDown:" + timeValue);
                onChangeTime(timeValue);
            }
            if (onTimeZero != null && timeValue == 0)
            {
                Debug.Log("END CountDown:" + timeValue);
                onTimeZero();
            }
        }
    }

    [SerializeField]
    private bool isPlay = false;

    /// <summary>开始倒计时
    /// </summary>
    public void CountDown_Start()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        Debug.Log("Start CountDown:" + timeValue);
        StopCoroutine("CountDown_CCC");
        isPlay = true;
        StartCoroutine("CountDown_CCC");
    }

    /// <summary>开始倒计时
    /// </summary>
    public void CountDown_Start(int timeMax)
    {
        TimeValue = timeMax;
        CountDown_Start();
    }

    /// <summary>倒计时—>暂停
    /// </summary>
    public void CountDown_Stop()
    {
        isPlay = false;
    }

    /// <summary>倒计时—>暂停后的继续
    /// </summary>
    public void CountDown_Play()
    {
        isPlay = false;
    }

    IEnumerator CountDown_CCC()
    {
        while (timeValue > 0)
        {
            TimeValue = TimeValue - 1;

            while (isPlay == false)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }

    //void OnGUI()
    //{
    //    GUI.Box(new Rect(128, 256, 128, 32), "Time ：" + TimeValue);

    //    if (GUI.Button(new Rect(128, 288, 128, 32), "Test Start"))
    //    {
    //        CountDown_Start();
    //    }
    //}
}
