using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashIndicator : MonoBehaviour {

	public Vector3 targetA;
	public bool drawing = false;
	private Player player;

	public SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		gameObject.transform.localScale = new Vector3(0, 0, 0);
		drawing = false;

		player = FindObjectOfType<Player>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		// start or stop the drawing the indicator
		if (Input.GetMouseButton(0) && !drawing && player.state != Player.State.talking) {
			drawing = true;

			Vector3 clickWorldPoint = ScreenToWorldPoint(Input.mousePosition);
			
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

			float distance = Vector2.Distance(targetA, clickWorldPoint);

			if (distance < Player.MIN_ATTACK_THRESH)
				spriteRenderer.color = Color.black;
			else if (distance < Player.SLASHING_THRESHOLD) 
				spriteRenderer.color = Color.blue;
			else spriteRenderer.color = Color.cyan;

			float length = Mathf.Sqrt(
				(clickWorldPoint.x - targetA.x) * (clickWorldPoint.x - targetA.x)
				 + (clickWorldPoint.y - targetA.y) * (clickWorldPoint.y - targetA.y)
			)  * 100 / spriteRenderer.sprite.rect.width;

			// if dashing, limit the length based on the stamina
			if (spriteRenderer.color == Color.cyan) length = Mathf.Min(length, 36 * player.staminaBar.fillAmount);
				
			transform.localScale = new Vector3(length, 3, 1);

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
