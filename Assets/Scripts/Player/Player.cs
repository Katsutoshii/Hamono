using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public float maxSpeed;
	public int comboCount;
	public float healthAmount;
	public int coinCount;
	public Text cointCountText;


	public enum State {
		idle,
		autoPathing,
		ready,
		dashing,
		slashing,
		talking,
		damaged,
		dead,
	};

	public enum AttackType {
		upSlash,
		downSlash,
		straightSlash,
		dash,
		none
	}

	public enum AttackResponse {
		normal,
		strong,
		blocked,
		missed,
		combo,
		none
	}

	public State state;
	public AttackType attackType = AttackType.none;
	public AttackResponse attackResponse = AttackResponse.none;

	public SpriteRenderer spriteRenderer;

	public bool grounded;
	public bool autoPathing;

	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash
	public SlashIndicator slashIndicator;
	public Dustcloud dustcloud;
	public Rigidbody2D rb;
	public Animator animator;
	public StaminaBar stamina;
	public HealthBar health;
	public GameObject afterimagePrefab;
	public GameObject swordAfterimagePrefab;
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;
	private AudioSource audioSource;

	// constants
	public float SLASHING_X_DIST;
	public float SLASHING_Y_DIST;
	public float SLASHING_THRESHOLD;
	public float AUTO_JUMP_FACTOR;
	public float TURNING_THRESHOLD;
	public float KP;
	public float GRAVITY_SCALE;
	public float DASH_SPEED;
	public float DASH_TARGET_THRESHOLD;
	public float ATTACK_TIMEOUT;
	public float AUTOPATH_TIMEOUT;
	public float DASH_STAMINA_COST;
	public float GENERATE_STAMINA;


	private float attackStartTime;
	private float autoPathStartTime;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
		rb.isKinematic = false;

    	animator = gameObject.GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		audioSource = gameObject.GetComponent<AudioSource>();

		state = State.idle;
		attackType = AttackType.none;


		PoolManager.instance.CreatePool(afterimagePrefab, 10);
		PoolManager.instance.CreatePool(swordAfterimagePrefab, 20);

	}
	
	// Update is called once per frame
	void Update() {
	
		if (state != State.damaged) Controls();
		if (grounded) stamina.IncreaseStamina(GENERATE_STAMINA);

		// actions based on the state
		switch (state) {
			
			case State.autoPathing:
				AutoPath();
				break;

			case State.ready:
				Ready();
				break;
			
			case State.dashing:
				Dash();
				SpawnAfterimage();
				break;

			case State.damaged:
				Damaged();
				break;

			case State.slashing:
				CheckForSlashEnd();
				rb.gravityScale = 0;
				break;

			default:
				rb.velocity = new Vector2(0, rb.velocity.y);
				rb.gravityScale = GRAVITY_SCALE;
				if (grounded) stamina.IncreaseStamina(GENERATE_STAMINA);
				break;
		}		

		RotateSpriteForVelocity();
		if(state != State.dashing) LimitVelocity();

		UpdateAnimatorVariables();
	}

	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking) {

			autoPathStartTime = Time.time;
			state = State.autoPathing;
			targetA = MouseWorldPosition2D();

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		}

		if (Input.GetMouseButtonUp(0)) {
			GetAttackType();
		}
	}

	private void UpdateAnimatorVariables() {
		// update animator variables
    	animator.SetBool("grounded", grounded);
    	animator.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
		animator.SetFloat("velocityY", rb.velocity.y);

		animator.SetBool("dashing", state == State.dashing);

		animator.SetBool("upSlashing", state == State.slashing && attackType == AttackType.upSlash);
		animator.SetBool("downSlashing", state == State.slashing && attackType == AttackType.downSlash);
		animator.SetBool("upSlashing", state == State.slashing && attackType == AttackType.upSlash);
		animator.SetBool("slashing", state == State.slashing && attackType == AttackType.straightSlash);


		animator.SetBool("prejumping", jumping && grounded);
		animator.SetBool("ready", state == State.ready);
		animator.SetBool("idle", state == State.idle);
	}

	
	private void RotateSpriteForVelocity() {
		// turn the sprite around based on velocity
		if (rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (1, 1, 1);
		} 
		else if (rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

	private void LimitVelocity() {
		// limit the velocity
		if (rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if (rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}

	public float AUTOPATH_Y_THRESHOLD; 
	public float AUTOPATH_Y_FACTOR;
	public float JUMP_X_THRESHOLD;

	// method to handle the autopathing
	private void AutoPath() {

		rb.gravityScale = GRAVITY_SCALE;
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y + 0.5f;

		// timeout if the player
		if (Time.time > autoPathStartTime + AUTOPATH_TIMEOUT) {
			Debug.Log("timeout for autopath");
			state = State.idle;
			rb.velocity = new Vector2(0, 0);
			return;
		}

		bool positionReached = Mathf.Abs(xDist) < SLASHING_X_DIST && 		// we are close enough in the x direciton
			(Mathf.Abs(yDist) < SLASHING_Y_DIST || 							// and we are close enough on the y
			(Mathf.Abs(yDist) < AUTOPATH_Y_THRESHOLD && grounded));			// OR we are gorunded and meet the grounded thresh

		if (positionReached) {
			// if we are at the position to start slashing, freeze until we have an attack!
			if (Input.GetMouseButton(0) || attackType != AttackType.none) {		// if we have an attack queued or we are still drawing
				
				state = State.ready;
				readyStartTime = Time.time;
			}
			else {
				state = State.idle;
			}
			return;
		}

		// if we are crouching for a jump
		if (jumping && grounded) {
			rb.velocity = new Vector2(0, rb.velocity.y);
			return;
		}

		// fixes overshooting
		if (Mathf.Abs(xDist) < SLASHING_X_DIST)
			rb.velocity = new Vector2(0, rb.velocity.y);

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST)
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);

		if (yDist >= AUTOPATH_Y_THRESHOLD && xDist <= JUMP_X_THRESHOLD && grounded)
			StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR, 20f)));
	}

	public float JUMP_DELAY;
	private bool jumping = false;
	private IEnumerator Jump(float jumpPower) {
		jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		dustcloud.MakeCloud(jumpPos);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(JUMP_DELAY);
		
		jumping = false;
		yield return null;
	}

	public float READY_FLOAT_TIMEOUT;
	private float readyStartTime;
	// method for when autopathing is complete and ready to make an attack
	private void Ready() {
		Debug.Log("Ready: attack type = " + attackType.ToString());
		
		rb.velocity = new Vector3(0, 0);
		Attack();	// will do nothing unless an attack is set

		if ((attackType == AttackType.none && 
			state != State.dashing && 
			state != State.slashing &&
			(Time.time - readyStartTime > READY_FLOAT_TIMEOUT && !grounded) // time out if floating
			)) {

			Debug.Log("Cancel ready!");
			state = State.idle;
			attackType = AttackType.none;
		}
		
		rb.gravityScale = 0;
	}

	// method to handle dashing
	// this is only called when auto pathing is completed!
	private void Dash() {
		if (stamina.isExhausted()) {
			rb.velocity = new Vector3(0, 0, 0);
			state = State.idle;
			attackType = AttackType.none;
			return;
		}
		stamina.DecreaseStamina(DASH_STAMINA_COST);
		if (Time.time > attackStartTime + ATTACK_TIMEOUT) {
			state = State.idle;
			attackType = AttackType.none;
		}
		float distanceB = Vector2.Distance(rb.position, targetB);
		
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.velocity = (targetB - rb.position) * DASH_SPEED;
			SpawnSwordAfterimage();
		} 

		// otherwise we have completed the dash
		else {
			rb.velocity = new Vector3(0, 0, 0);
			state = State.idle;
			attackType = AttackType.none;
		}
		
		rb.gravityScale = 0;
	}

	private int afterimageCount = 0;
	public int numAfterimage;
	public Color afterimageColor;

	// method to create the after images for the dash
	private void SpawnAfterimage() {
		
		afterimageCount++;
		if(afterimageCount % 3 != 0) return;

		PoolManager.instance.ReuseObject (afterimagePrefab, transform.position, transform.eulerAngles, transform.localScale);
    }

	private void SpawnSwordAfterimage() {
        Vector3 eulerAngles = new Vector3(0, 0, Mathf.Atan2(rb.velocity.y, 
				rb.velocity.x) * 180 / Mathf.PI);
			
		Vector3 localScale = new Vector3(rb.velocity.magnitude / 20, 1, 1);

		PoolManager.instance.ReuseObject (swordAfterimagePrefab, transform.position, eulerAngles, localScale);
	}

	private Vector2 MouseWorldPosition2D() {
		Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	}


	// method to perform the slash
	private void Attack() {
		Debug.Log("Checking attack!");
		attackStartTime = Time.time;

		switch (attackType) {
			case AttackType.upSlash:
				state = State.slashing;
				break;

			case AttackType.downSlash:
				state = State.slashing;
				break;

			case AttackType.straightSlash:
				state = State.slashing;
				break;

			case AttackType.dash:
				state = State.dashing;
				break;
			
			case AttackType.none:
				break;
		}

		UpdateAnimatorVariables();
	}

	public float MIN_ATTACK_THRESH;
	public void GetAttackType() {
		
		targetB = MouseWorldPosition2D();
		//state = State.autoPathing;

		float dist = Vector2.Distance(targetA, targetB);

		if (dist < MIN_ATTACK_THRESH) attackType = AttackType.none;
		else if (dist > SLASHING_THRESHOLD) {
			attackType = AttackType.dash;
			// dashing is handle on a frame-by-frame basis
		}
		else
			attackType = CalcSlashType(); 	// sets slashType to the correct type of slash
	}
	
	// method to get the slash type based on targetA and targetB
	private AttackType CalcSlashType(){
		AttackType slashType;
		// if this is a jab
		float angle = Mathf.Atan2(targetB.y - targetA.y, 
			Mathf.Abs(targetB.x - targetA.x)) * 180 / Mathf.PI;

		if(angle > 30) slashType = AttackType.upSlash;
		else if(angle < -30) slashType = AttackType.downSlash;
		else slashType = AttackType.straightSlash;
		
		return slashType;
	}

	private void CheckForSlashEnd() {

		// check if the slash is over by seeing if the current playing animation is idle
		if (!(animator.GetBool("slashing") || animator.GetBool("upSlashing") || animator.GetBool("downSlashing"))) {
			Debug.Log("Slash over!");
			state = State.idle;
			attackType = AttackType.none;
		}
	}

	private void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

	// method to play sounds from animator
	public void PlayOneShot(AudioClip sound) {
		audioSource.PlayOneShot(sound);
	}

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		Debug.Log("Player: Collision enter " + other.collider.name);
		switch (other.collider.name.Substring(0, 4)) {
			case "Coin":
				coinCount++;
				cointCountText.text = "" + coinCount;
				break;
		}
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Player: Trigger enter " + other.name);
		switch (other.name) {
			case "EnemyHurtBox":
				if (state != State.dashing && state != State.slashing) Damage(0.5f, other);
				break;
		}
	}

	private void Damage(float damageAmount, Collider2D source) {
		if (state != State.damaged) {
			damagedStartTime = Time.time;
			state = State.damaged;
			rb.AddForce(Vector2.up * 20);

			healthAmount -= damageAmount;
			if ( healthAmount < 0) healthAmount = 0;

			if (healthAmount == 0) Death();
			health.HandleHealth(healthAmount);
		}
	}

	private float damagedStartTime;
	private void Damaged() {
		spriteRenderer.color = Color.red;
		
		if (Time.time - damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;
			state = State.idle;
		}
	}

	private void Death() {
		Debug.Log("Player died!");
	}
}
