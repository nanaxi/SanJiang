using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SetLayout : MonoBehaviour
{
    public float m_waitTime = 0.01f;
    public LayoutElement m_LayoutElement;
    public Vector2 offsetWH;
    public bool isSetMinWidth = false;
    public bool isSetYMinHight = true;
    //// Use this for initialization
    //void Start()
    //{

    //}
    void OnEnable()
    {
        StartCoroutine(InvekeFunctioin());
    }

    IEnumerator InvekeFunctioin()
    {
        yield return new WaitForSeconds(m_waitTime);
        if (m_LayoutElement != null && GetComponent<RectTransform>() != null)
        {
            if (isSetMinWidth)
            {
                m_LayoutElement.minWidth = GetComponent<RectTransform>().rect.x + offsetWH.x;
            }
            if (isSetYMinHight)
            {
                m_LayoutElement.minHeight = GetComponent<RectTransform>().rect.y + offsetWH.y;
            }
        }
        yield return null;
    }
}
