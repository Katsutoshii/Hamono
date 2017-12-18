﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickIndicator : MonoBehaviour {
	private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		
		spriteRenderer.color = new Color(1, 1, 1, 0);
	}	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)) {
			StartCoroutine(SpinAndFade());
		}
	}

	IEnumerator SpinAndFade() {
		transform.position = ScreenToWorldPoint(Input.mousePosition);

		spriteRenderer.color = Color.white;
		while (spriteRenderer.color.a >= 0) {
			transform.localScale = Vector3.one * spriteRenderer.color.a * 5;
			Color color = spriteRenderer.color;
			color.a -= 0.02f;
			spriteRenderer.color = color;
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
				transform.localEulerAngles.y, transform.localEulerAngles.z + 2f);

			yield return new WaitForEndOfFrame();
		} 
		yield return null;
	}

	private Vector3 ScreenToWorldPoint(Vector3 screenPoint) {
		screenPoint.z = 10f;	// this is the camera distance
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}
}