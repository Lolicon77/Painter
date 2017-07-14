using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaintOnImage : MonoBehaviour, IDragHandler, IEndDragHandler {

	public Image image;
	public Canvas canvas;

	public int radius = 3;

	private RectTransform canvasRectTransform;
	private RectTransform imageRectTransform;
	private Vector3 imageScreenPos;
	private Vector2 imageLeftDownCorner;

	private Texture2D texture2D;

	private List<Vector2> pointList = new List<Vector2>();

	private Vector2 lastFrameScreenPos;
	private Vector2 lastFramePos;
	private Vector2 currentFramePos;
	private int minX;
	private int maxX;
	private int minY;
	private int maxY;

	void Awake() {
		canvasRectTransform = canvas.transform as RectTransform;
		imageScreenPos = canvas.worldCamera.WorldToScreenPoint(image.transform.position);
		imageRectTransform = image.transform as RectTransform;
		Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, imageScreenPos, canvas.worldCamera, out pos)) {
			imageLeftDownCorner = pos - canvasRectTransform.rect.width * 0.5f * Vector2.right - canvasRectTransform.rect.height * 0.5f * Vector2.up;
		}

		texture2D = new Texture2D(Mathf.FloorToInt(imageRectTransform.rect.width), Mathf.FloorToInt(imageRectTransform.rect.height), TextureFormat.RGBA32, false, true) {
			filterMode = FilterMode.Bilinear,
			wrapMode = TextureWrapMode.Clamp,
		};
	}

	public void OnDrag(PointerEventData eventData) {
		//		currentFramePos = eventData.position - imageLeftDownCorner;
		Vector2 pos;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, eventData.position, canvas.worldCamera, out pos)) {
			currentFramePos = pos;
		}
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, lastFrameScreenPos, canvas.worldCamera, out pos)) {
			lastFramePos = pos;
		}

		//		var delta = currentFramePos - lastFramePos;
		if (Vector2.Distance(lastFramePos, Vector2.zero) >= 0.2f) {
			pointList.Clear();

			int lfX = Mathf.FloorToInt(lastFramePos.x);
			int lfY = Mathf.FloorToInt(lastFramePos.y);
			int cfX = Mathf.FloorToInt(currentFramePos.x);
			int cfY = Mathf.FloorToInt(currentFramePos.y);
			int deltaX = cfX - lfX;
			int deltaY = cfY - lfY;

			float k = 0;
			float kInverse = 0;
			if (deltaX != 0) {
				k = (float)deltaY / deltaX;
				kInverse = 1 / k;
			}

			if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY)) {
				var sign = (int)Mathf.Sign(cfX - lfX);
				for (int i = lfX; sign > 0 ? i <= cfX : i >= cfX; i += sign) {
					var v2 = new Vector2(i, lfY + k * (i - lfX));
					pointList.Add(v2);
				}
			} else {
				var sign = (int)Mathf.Sign(cfY - lfY);
				for (int i = lfY; sign > 0 ? i <= cfY : i >= cfY; i += sign) {
					var v2 = new Vector2(lfX + (i - lfY) * kInverse, i);
					pointList.Add(v2);
				}
			}
			Paint(pointList.ToArray());
			image.sprite = Sprite.Create(texture2D, new Rect(0, 0, Mathf.Floor(imageRectTransform.rect.width), Mathf.Floor(imageRectTransform.rect.height)), Vector2.zero);
		}
		lastFrameScreenPos = eventData.position;
	}

	void Paint(Vector2[] points) {
		for (int k = 0; k < points.Length; k++) {
			minX = Mathf.RoundToInt(points[k].x) - radius + 1;
			maxX = Mathf.RoundToInt(points[k].x) + radius;
			minY = Mathf.RoundToInt(points[k].y) - radius + 1;
			maxY = Mathf.RoundToInt(points[k].y) + radius;
			for (int i = minX; i < maxX; i++) {
				for (int j = minY; j < maxY; j++) {
					if (i >= 0 && i < texture2D.width && j >= 0 && j < texture2D.height) {
						texture2D.SetPixel(i, j, Color.blue);
					}
				}
			}
		}
		texture2D.Apply(false, false);
	}

	public void OnEndDrag(PointerEventData eventData) {
		lastFrameScreenPos = Vector2.zero;
	}
}
