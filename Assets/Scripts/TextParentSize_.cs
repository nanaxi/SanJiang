using UnityEngine;

/// <summary>作用于Text ContentSizeFitter自动拉伸的时候，让父物体也跟着拉伸
/// </summary>
public class TextParentSize_ : MonoBehaviour {
    enum SizeVector3
    {
        X_,Y_
    }
    [SerializeField]private SizeVector3 layoutV3_Type = SizeVector3.Y_;
    [SerializeField]private RectTransform t_Parent;
    [SerializeField]private RectTransform t_T;
    [SerializeField]
    private int i_Offset = 5;
    public void OnDrag_S_Rect()
    {
        if (t_Parent == null || t_T == null)
        {
            t_Parent = GetComponent<RectTransform>();
            t_T = transform.GetChild(0).GetComponent<RectTransform>();
            Debug.LogWarning("t_Parent == null || t_T == null");
        }
        if (t_Parent != null || t_T != null)
        {
            switch (layoutV3_Type)
            {
                case SizeVector3.X_:
                    t_Parent.sizeDelta = new Vector2(Mathf.Abs(t_T.rect.x+i_Offset), t_Parent.sizeDelta.y);
                    break;
                case SizeVector3.Y_:
                    t_Parent.sizeDelta = new Vector2(t_Parent.sizeDelta.x, Mathf.Abs(t_T.rect.y + i_Offset));
                    break;
                default:
                    break;
            }
        }
    }
}
