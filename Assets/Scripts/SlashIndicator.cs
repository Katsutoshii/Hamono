using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashIndicator : MonoBehaviour {

	public Vector3 targetA;
	public bool drawing = false;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		Debug.Log("On Start for slash");
		gameObject.transform.localScale = new Vector3(0, 0, 0);
		drawing = false;

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {

	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		// start or stop the drawing the indicator
		if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Mouse clicked!");
			drawing = true;
			Vector3 clickWorldPoint =ScreenToWorldPoint(Input.mousePosition);
			Debug.Log("Clicked world point:" + clickWorldPoint.ToString());
			transform.position = clickWorldPoint;
			targetA = clickWorldPoint;
		}
		else if (Input.GetMouseButtonUp(0)) {
			drawing = false;
		}

		// when drawing, scale the UI indicator based on the start position and the current mouse position
		if (drawing) {
			Vector3 clickWorldPoint =ScreenToWorldPoint(Input.mousePosition);

			float width = Mathf.Sqrt(
				(clickWorldPoint.x - targetA.x) * (clickWorldPoint.x - targetA.x)
				 + (clickWorldPoint.y - targetA.y) * (clickWorldPoint.y - targetA.y)
			) * 100 / spriteRenderer.sprite.rect.width;
				
			transform.localScale = new Vector3(width, 0.05f, 0.05f);
		

			float angle = Mathf.Atan2(clickWorldPoint.y - targetA.y, 
				clickWorldPoint.x - targetA.x) * 180 / Mathf.PI;

			transform.eulerAngles = new Vector3(0, 0, angle);
		}
		else {
			transform.localScale = new Vector3(0, 0, 0);
		}
	}

	private Vector3 ScreenToWorldPoint(Vector3 screenPoint) {
		screenPoint.z = 10f;	// this is the camera distance
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}
}
