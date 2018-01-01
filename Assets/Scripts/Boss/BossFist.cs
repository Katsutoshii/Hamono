using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFist : MonoBehaviour {

	private Boss boss;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider2D;
	// Use this for initialization
	void Start () {
		boss = GetComponentInParent<Boss>();

		boxCollider2D = GetComponent<BoxCollider2D>();
		boxCollider2D.enabled = false;

		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "BackgroundDetails";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Ready() {
		spriteRenderer.sortingLayerName = "EntityBackground";
		boxCollider2D.enabled = true;
	}
}
