using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed = 50f;
	public float maxSpeed = 3f;
	public float jumpPower = 4000f;

	public bool grounded;

	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		rb = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(key:KeyCode.W)) {
			rb.AddForce(Vector2.up * jumpPower);
		}

		// turn the sprite around
		if(rb.velocity.x > 0.1f) 
			transform.localScale = new Vector3(1, 1, 1);
		else if(rb.velocity.x < -0.1f) 
			transform.localScale = new Vector3(-1, 1, 1);
		
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		if(Input.GetKey(key:KeyCode.A)) {
			rb.AddForce(Vector2.left * speed);
		}
		if(Input.GetKey(key:KeyCode.D)) {
			rb.AddForce(Vector2.right * speed);
		}

		// limit the speed
		if(rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if(rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}
}
