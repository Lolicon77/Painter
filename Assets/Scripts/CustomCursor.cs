using UnityEngine;

public class CustomCursor : MonoBehaviour {

	public RectTransform canvasRectTransform;
	public Camera uiCamera;
	private Vector2 halfVector;

	void Awake() {
		var halfScreenWidth = canvasRectTransform.rect.width * 0.5f;
		var halfScreenHeight = canvasRectTransform.rect.height * 0.5f;
		halfVector = new Vector2(halfScreenWidth, halfScreenHeight);
		Cursor.visible = false;
	}

	void Update() {
		var rectTrans = transform as RectTransform;
		if (rectTrans != null) {
			Vector2 localPos;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, uiCamera, out localPos)) {
				rectTrans.anchoredPosition = localPos;
			}
		}
	}

}