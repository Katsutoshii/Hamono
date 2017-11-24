using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed = 50f;
	public float maxSpeed = 4f;
	public float jumpPower = 300f;
	public float kp = 100f;

	public bool grounded;
	public bool autoPathing;

	public Vector2 targetA;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		rb = gameObject.GetComponent<Rigidbody2D>();
		autoPathing = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(key:KeyCode.W)) {
			rb.AddForce(Vector2.up * jumpPower);
			autoPathing = false;
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
		// for move left and right manually
		if(Input.GetKey(key:KeyCode.A)) {
			rb.AddForce(Vector2.left * speed);
			autoPathing = false;
		}
		if(Input.GetKey(key:KeyCode.D)) {
			rb.AddForce(Vector2.right * speed);
			autoPathing = false;
		}

		// if we clicked, start autopathing towards that direction
		if(Input.GetMouseButtonDown(0)) { // if left click
			autoPathing = true;
			Vector3 v3 = Input.mousePosition;
			// v3.z = 10.0;
			v3 = Camera.main.ScreenToWorldPoint(v3);

			targetA = new Vector2(v3.x, v3.y);
		}

		if(autoPathing) {
			float error = targetA.x - transform.position.x;

			if(Mathf.Abs(error) > 1) {
				rb.AddForce(Vector2.left  * -1 * error * kp);
			}
			else {
				autoPathing = false;
			}
		}

		// limit the speed
		if(rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if(rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}

    void OnGUI()
    {
        GUI.Box(new Rect(100, 100, 150, 150), rb.velocity.ToString());
    }
}
