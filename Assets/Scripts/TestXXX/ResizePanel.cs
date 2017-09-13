using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler {
	
	public Vector2 minSize = new Vector2 (100, 100);
	public Vector2 maxSize = new Vector2 (400, 400);

    public Vector2 dataPosi;
    public Vector2 localPointerPosition;

    private RectTransform panelRectTransform;
	private Vector2 originalLocalPointerPosition;
	private Vector2 originalSizeDelta;
	
	void Awake () {
		panelRectTransform = transform.parent.GetComponent<RectTransform> ();
	}
	
	public void OnPointerDown (PointerEventData data) {
		originalSizeDelta = panelRectTransform.sizeDelta;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}
	
	public void OnDrag (PointerEventData data) {
		if (panelRectTransform == null)
			return;
		
		
		RectTransformUtility.ScreenPointToLocalPointInRectangle (panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
        dataPosi = data.position;
        Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
		
		Vector2 sizeDelta = originalSizeDelta + new Vector2 (offsetToOriginal.x, -offsetToOriginal.y);
		sizeDelta = new Vector2 (
			Mathf.Clamp (sizeDelta.x, minSize.x, maxSize.x),
			Mathf.Clamp (sizeDelta.y, minSize.y, maxSize.y)
		);
		
		panelRectTransform.sizeDelta = sizeDelta;
	}
}