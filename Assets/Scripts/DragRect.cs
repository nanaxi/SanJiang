using UnityEngine;
using UnityEngine.EventSystems;

public class DragRect : MonoBehaviour ,IDragHandler{
    Canvas canvas;
    RectTransform rectTransform;
    public RectTransform targetRectT;
    Vector2 pos;
    [SerializeField]private Vector3 tagetDistance;
    public bool isSetIndex = false;

    public void OnDrag(PointerEventData eventData)
    {
        canvas = canvas == null ? GetComponentInParent<Canvas>() : canvas;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos))
        {
            rectTransform.anchoredPosition = pos;
            if (targetRectT != null)
                targetRectT.anchoredPosition3D = rectTransform.anchoredPosition3D - tagetDistance;
            
            if (isSetIndex)
                targetRectT.SetAsLastSibling();
            
        }
    }

    // Use this for initialization
    void Awake()
    {
        rectTransform = transform as RectTransform;
    }

    void OnEnable()
    {
        if (targetRectT != null)
            tagetDistance = rectTransform.anchoredPosition3D - targetRectT.anchoredPosition3D;
    }
}
