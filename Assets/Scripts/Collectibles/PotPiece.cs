using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotPiece : PooledObject {

	public Sprite[] sprites;

	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
	
	public override void OnObjectReuse() {
		sprites = Resources.LoadAll<Sprite>("Graphics/Environment/PotParts");
		foreach (Sprite sprite in sprites) {
			Debug.Log(sprite.name);
		}

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = Color.white;

		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 100f), 0));
		
		spriteRenderer.sprite = sprites[Random.Range(2, sprites.Length)];
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		Color color = spriteRenderer.color;
		color.a -= 0.01f;
		spriteRenderer.color = color;
		if (color.a <= 0) gameObject.SetActive(false);
	}
}
