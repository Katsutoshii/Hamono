using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFist : MonoBehaviour {

	private Boss boss;
	private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
		boss = GetComponentInParent<Boss>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerName = "BackgroundDetails";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Ready() {
		spriteRenderer.sortingLayerName = "EntityBackground";
	}
}
