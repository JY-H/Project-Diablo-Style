// This class is responsible for allowing a panel or UI element to be dragged.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[AddComponentMenu("Inventory/Samples/DragPanel")]
public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler {

	public bool clampPanel = true;				// Whether or not the script should clamp the panel inside the drag area
	public RectTransform dragArea;				// The area where you can move your panel in
	public float clampOffsetMinX,clampOffsetMinY,clampOffsetXMax,clampOffsetYMax;
	
	private Vector2 originalLocalPointerPosition;
	private Vector3 originalPanelLocalPosition;
	private RectTransform panelRectTransform;

	private Vector2 localPointerPosition;
	private Vector3 offsetToOriginal;

	private bool canDrag = true;
	
	void Awake () {
		panelRectTransform = transform.parent as RectTransform;			// Getting the RECT TRANSFORM of the panel you want to drag
	}
	
	public void OnPointerDown (PointerEventData data) {
		if (panelRectTransform == null || dragArea == null) {
			canDrag = false;
			return;
		}
		else {
			canDrag = true;
		}

		originalPanelLocalPosition = panelRectTransform.localPosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle (dragArea, data.position, data.pressEventCamera, out originalLocalPointerPosition);
	}
	
	public void OnDrag (PointerEventData data) {
		if (canDrag) {
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle (dragArea, data.position, data.pressEventCamera, out localPointerPosition)) {
				offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
				panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
			}

			if (clampPanel)
				ClampToWindow ();
		}
	}
	
	// Clamp panel to area of parent
	void ClampToWindow () {
		Vector3 pos = panelRectTransform.localPosition;
		
		Vector3 minPosition = dragArea.rect.min - panelRectTransform.rect.min;
		Vector3 maxPosition = dragArea.rect.max - panelRectTransform.rect.max;
		
		pos.x = Mathf.Clamp (panelRectTransform.localPosition.x, minPosition.x + clampOffsetMinX, maxPosition.x + clampOffsetXMax);
		pos.y = Mathf.Clamp (panelRectTransform.localPosition.y, minPosition.y + clampOffsetMinY, maxPosition.y + clampOffsetYMax);
		
		panelRectTransform.localPosition = pos;
	}
}