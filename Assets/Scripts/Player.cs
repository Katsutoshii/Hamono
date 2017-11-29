using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public float jumpPower;
	public int jumps;

	public enum State {
		idle,
		running,
		autoPathing,
		dashing,
		slashing
	};

	public enum SlashType {
		upSlash,
		downSlash,
		jab,
		upJab,
		downJab
	}

	public State state;
	public SlashType slashType;
	public bool grounded;
	public bool autoPathing;

	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash
	public SlashIndicator slashIndicator;
	private Rigidbody2D rb;
    private Animator anim;

	// constants
	public float SLASHING_X_DIST;
	public float SLASHING_Y_DIST;
	public float SLASHING_THRESHOLD;
	public float AUTO_JUMP_FACTOR;
	public float TURNING_THRESHOLD;
	public float KP;
	public float GRAVITY_SCALE;
	public float JAB_THRESHOLD;

	private float slashStartTime;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
		state = State.idle;
	}
	
	// Update is called once per frame
	void Update () {
    
		// turn the sprite around
		if(rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3(1, 1, 1);
			if(state == State.idle)
				state = State.running;
		}
		else if(rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3(-1, 1, 1);
			if(state == State.idle)
				state = State.running;
		}
		else if(state == State.running) state = State.idle;

        anim.SetBool("grounded", grounded);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
	}

	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		// for move left and right manually
		if (Input.GetKey(key:KeyCode.A)) {
			CancelAutomation();
			rb.velocity += Vector2.left * speed;
			state = State.running;
		}
		else if (state == State.running) state = State.idle;

		if(Input.GetKey(key:KeyCode.D)) {
			CancelAutomation();
			rb.velocity += Vector2.right * speed;
			state = State.running;
		}
		else if (state == State.running) state = State.idle;

        // for jumping
        if (Input.GetKeyDown(key: KeyCode.W) && jumps > 0)
        {
			CancelAutomation();
			Jump(jumpPower);
			
            state = State.idle;
        }

        // if we clicked, start autopathing towards that direction
        if (Input.GetMouseButtonDown(0)) { // if left click
			jumps = 0;
            state = State.autoPathing;

            targetA = MouseWorldPosition2D();

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		}
		else if (Input.GetMouseButton(0)) {
			if (Vector2.Distance(targetA, MouseWorldPosition2D()) > SLASHING_THRESHOLD)
				slashIndicator.spriteRenderer.color = Color.blue;
			else slashIndicator.spriteRenderer.color = Color.red;
		}
		else {
			rb.gravityScale = GRAVITY_SCALE;
		}

		// if we release the mouse click, then we have finished drawing a slash
		if (Input.GetMouseButtonUp(0)) {
			if (state == State.autoPathing) {
				CancelAutomation();
				state = State.idle;
			}

			targetB = MouseWorldPosition2D();
			if (Vector2.Distance(targetA, targetB) > SLASHING_THRESHOLD) {
				state = State.dashing;
				// dashing is handle on a frame-by-frame basis
			}
			else {
				state = State.slashing;
				CalcSlashType(); 	// sets slashType to the correct type of slash
				Slash();			// start the slash
			}
		}

		// actions based on the state
		switch (state) {
			case State.autoPathing:
				AutoPath();
				break;
			
			case State.dashing:
				Dash();
				break;

			case State.slashing:
				// check if the slash is over by seeing if the current playing animation is idle
				if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") && Time.time > slashStartTime + 0.1) {
					Debug.Log("Slash ended!");
					rb.WakeUp();
					state = State.idle;
				}
				else rb.Sleep();

				break;

			default:
				break;
		}		

		// limit the speed
		if (rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if (rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}

	// method to handle the autopathing
	private void AutoPath() {
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;

		// if we are at the position to start slashing, freeze!
		if(Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {
			rb.gravityScale = 0;
			rb.velocity = new Vector3(0, 0);
			state = State.idle;
			return;
		}

		// otherwise, if we need to move in the x direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST) {
			rb.velocity = new Vector2(xDist * KP, yDist * KP);
		}
	}

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI() {
		GUI.Label(new Rect(20, 20, 100, 100), "Velocity = " + rb.velocity.ToString());
	}

	// method to handle dashing
	private void Dash() {
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;
	}

	private Vector2 MouseWorldPosition2D(){
		Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	}

	public void CancelAutomation() {
		if(state == State.autoPathing || state == State.dashing || state == State.slashing) {
				rb.velocity = new Vector3(0, 0, 0);
				rb.gravityScale = GRAVITY_SCALE;
			}
	}

	private void Jump(float power) {
		rb.velocity = new Vector2(rb.velocity.x, 0); // prevents stacking velocity
		rb.velocity += Vector2.up * power;
		jumps--;
	}

	// method to perform the slash
	private void Slash(){
		Debug.Log("Slashing");
		rb.Sleep();
		slashStartTime = Time.time;

		switch (slashType) {
			case SlashType.upJab:
				break;

			case SlashType.jab:
				break;

			case SlashType.downJab:
				break;

			case SlashType.upSlash:
			
				rb.gravityScale = 0;
				anim.Play("PlayerUpSlash");
				break;

			case SlashType.downSlash:
				break;
		}
	}

	// method to get the slash type based on targetA and targetB
	private void CalcSlashType(){
		Debug.Log("finding slash type");
		// if this is a jab
		if(Vector2.Distance(transform.position, targetA) < JAB_THRESHOLD) {
			float angle = Mathf.Atan2(targetB.y - targetA.y, 
				targetB.x - targetA.x) * 180 / Mathf.PI;
			Debug.Log("It's a jab! Angle = " + angle);

			if(angle > 30) slashType = SlashType.upJab;
			else if(angle < -30) slashType = SlashType.downJab;
			else slashType = SlashType.jab;
		}
		// otherwise this must be a slash
		else {
			Debug.Log("It's a slash!");
			if(targetA.y >= targetB.y) slashType = SlashType.downSlash;
			else slashType = SlashType.upSlash;
		}
	}
}
