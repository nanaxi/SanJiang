using UnityEngine;
using System.Collections;

/// <summary>关于Spine 动画的 点击播放指定动画
/// </summary>
public class SpineClick : MonoBehaviour {
    bool isMzPlay = false;//是否正在播放Spine点击动画
    public float waitTime_Mz = 2f;//等待
    public string anima_Stand = "stand", anima_OnClick = "dianji";//请在Inspector根据实际情况赋值

    public string clickAudioPath;//点击 的时候播放的声音
    SkeletonAnimation spineAnimation;
    // Use this for initialization
    void Start () {
        spineAnimation = GetComponent<SkeletonAnimation>();
    }
    
    void OnMouseDown()
    {
        OnClick_SpineObj();
    }

    void OnClick_SpineObj()
    {
        if (isMzPlay == false)
        {
            isMzPlay = true;

            Audio_Manage.Instance.Play_Audio(clickAudioPath);

            StartCoroutine(SP_MeiZiPlay());
        }
    }

    IEnumerator SP_MeiZiPlay()
    {
        
        spineAnimation.AnimationName = anima_OnClick;
        float f_WaitTime = waitTime_Mz;
        while (f_WaitTime > 0)
        {//等待动画播放完毕才能够再次触发点击动画
            f_WaitTime -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        isMzPlay = false;
        spineAnimation.AnimationName = anima_Stand;
        yield return null;
    }
}
