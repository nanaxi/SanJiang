using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary> 提示 —— 倒计时自动关闭
/// </summary>
public class UiWin_PromptSelfClose : MonoBehaviour
{

    private PromptModelSelfClose promptModel;
    [SerializeField]
    private Text tShowPrompt;

    [SerializeField]
    private Text t_Time;
    private CanvasRenderer[] canvasRDAry;
    public float testTime = 3;

    [Range(0, 1.0f)]
    [SerializeField]
    private float colorAValue;
    // Use this for initialization
    void Awake()
    {
        canvasRDAry = GetComponentsInChildren<CanvasRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (promptModel != null)
        {
            if (promptModel.fCountDownTime > 0)
            {
                promptModel.fCountDownTime -= Time.deltaTime;
                promptModel.fCountDownTime = Mathf.Clamp(promptModel.fCountDownTime, 0, promptModel.fCountDownTime);
                t_Time.text = (System.Math.Round(promptModel.fCountDownTime, 1)).ToString();
            }
            else if (promptModel.fCountDownTime <= 0)
            {
                colorAValue -= Time.deltaTime;
                for (int i = 0; i < canvasRDAry.Length; i++)
                {
                    canvasRDAry[i].SetAlpha(colorAValue);
                }
                if (colorAValue <= 0)
                {
                    promptModel = null;
                    gameObject.SetActive(false);
                }
            }

        }
    }

    //void OnGUI()
    //{
    //    if (GUILayout.Button("———测试———"))
    //    {
    //        OpenPrompt(new PromptModelSelfClose("这是一个测试~~~", testTime));
    //    }
    //}

    public void OpenPrompt(PromptModelSelfClose promptM)
    {
        promptModel = promptM;
        tShowPrompt.text = promptModel.promptMsg;

        Init_CanvasRendererAry();
    }

    public void Init_CanvasRendererAry()
    {
        for (int i = 0; i < canvasRDAry.Length; i++)
        {
            canvasRDAry[i].SetAlpha(1);
        }
        colorAValue = 1;
    }
}
