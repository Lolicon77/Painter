using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DymaticSpriteSetting : MonoBehaviour {

	public Image image;

	private Texture2D texture2D;

	private int timer = 0;

	void Start() {
		texture2D = new Texture2D(8, 8, TextureFormat.RGBA32, false, true);
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
	}

	void Update() {
		if (Time.time > 64) {
			return;
		}
		if (Time.time - timer < 1) {
			return;
		}
		texture2D.SetPixel(timer / 8, timer % 8, RandomColor());
		texture2D.Apply(false, false);
		image.sprite = Sprite.Create(texture2D, new Rect(0, 0, 8, 8), Vector2.zero);
		timer++;
	}

	Color RandomColor() {
		return new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
	}

}
