using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashIndicator : MonoBehaviour {

	public Vector3 targetA;
	public bool drawing = false;
	public Player player;

	public SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		Debug.Log("On Start for slash");
		gameObject.transform.localScale = new Vector3(0, 0, 0);
		drawing = false;

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		// start or stop the drawing the indicator
		if (Input.GetMouseButton(0) && !drawing && player.state != Player.State.talking) {
			drawing = true;

			Vector3 clickWorldPoint =ScreenToWorldPoint(Input.mousePosition);
			
			transform.position = clickWorldPoint;
			targetA = clickWorldPoint;
		}
		else if (!Input.GetMouseButton(0) && drawing && player.state != Player.State.talking) {
			drawing = false;
		}

		// when drawing, scale the UI indicator based on the start position and the current mouse position
		if (drawing) {
			Vector3 clickWorldPoint = ScreenToWorldPoint(Input.mousePosition);
			Vector3 targetAScreenPoint = Camera.main.WorldToScreenPoint(targetA);

			if (Vector2.Distance(targetA, clickWorldPoint) > Player.SLASHING_THRESHOLD)
				spriteRenderer.color = Color.blue;
			else spriteRenderer.color = Color.red;

			float width = Mathf.Sqrt(
				(Input.mousePosition.x - targetAScreenPoint.x) * (Input.mousePosition.x - targetAScreenPoint.x)
				 + (Input.mousePosition.y - targetAScreenPoint.y) * (Input.mousePosition.y - targetAScreenPoint.y)
			) * 100 / spriteRenderer.sprite.rect.width;
				
			transform.localScale = new Vector3(width, 2f, 2f);

			float angle = Mathf.Atan2(clickWorldPoint.y - targetA.y, 
				clickWorldPoint.x - targetA.x) * 180 / Mathf.PI;

			transform.eulerAngles = new Vector3(0, 0, angle);
		}
		else {
			transform.localScale = new Vector3(0, 0, 0);
		}
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		
	}

	private Vector3 ScreenToWorldPoint(Vector3 screenPoint) {
		screenPoint.z = 10f;	// this is the camera distance
		return Camera.main.ScreenToWorldPoint(screenPoint);
	}
}
