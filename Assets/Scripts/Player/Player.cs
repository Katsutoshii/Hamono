using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	// current status of player
	public enum State {
		idle,
		autoPathing,
		ready,
		dashing,
		slashing,
		talking,
		finishedTalking,
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

	// player representation
	private SpriteRenderer spriteRenderer;
	public Rigidbody2D rb;
	public Animator animator;

	// temporary state checkers
	public bool autoPathing;
	public bool grounded;
	public bool invincible;

	// directional targets
	private Vector2 targetA; 	// start point of a slash
	private Vector2 targetB;		// end point of a slash

	// visible player information
	public SlashIndicator slashIndicator;
	public StaminaBar stamina;
	public HealthBar health;
	public float maxHealth;
	public float healthAmount;
	public float damagedTime;
	public float hurtAlpha;

	// counters
	public int comboCount;
	public int coinCount;
	public Text coinCountText;
	

	// special effects game objects
	public GameObject dustCloudPrefab;
	public GameObject afterimagePrefab;
	public GameObject swordAfterimagePrefab;

	// ui/ux elements
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;
	private AudioSource audioSource;

	// public constants
	public const float SLASHING_THRESHOLD = 3.5f;
	public const float KP = 4f;
	public float maxSpeed;
	public float generateStamina;
	public float dashStaminaCost;
	public float slashStaminaCost;

	// private constants
	private const float ATTACK_TIMEOUT = 0.5f;
	private const float AUTOPATH_TIMEOUT = 1.5f;
	private const float DASH_SPEED = 8f;
	private const float DASH_TARGET_THRESHOLD = 0.8f;
	private const float GRAVITY_SCALE = 2f;
	private const float TURNING_THRESHOLD = 0.1f;


	// private start times
	private float attackStartTime;
	private float autoPathStartTime;
	private float alphaToggleTime;

	// Use this for initialization
	void Start () {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
		rb.isKinematic = false;

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		audioSource = gameObject.GetComponent<AudioSource>();

		// start states
		state = State.idle;
		attackType = AttackType.none;

		PoolManager.instance.CreatePool(dustCloudPrefab, 1);
		PoolManager.instance.CreatePool(afterimagePrefab, 10);
		PoolManager.instance.CreatePool(swordAfterimagePrefab, 20);

	}
	
	// Update is called once per frame
	void Update() {
		if (!(state == State.damaged || state == State.dead))Controls();
		if (grounded) stamina.IncreaseStamina(generateStamina);
		if (attackType == AttackType.none) attackResponse = AttackResponse.none;

		// handles the current state
		HandleState();	
		
		if (state != State.damaged) RotateSpriteForVelocity();
		if(state != State.dashing) LimitVelocity();

		UpdateAnimatorVariables();
	}

	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking && state != State.finishedTalking) {

			autoPathStartTime = Time.time;
			state = State.autoPathing;
			targetA = MouseWorldPosition2D();

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		} else if (state == State.finishedTalking) state = State.idle;

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
		animator.SetBool("damaged", state == State.damaged);
		animator.SetBool("dead", state == State.dead);
	}

	
	private void RotateSpriteForVelocity() {
		if (!(state == State.idle || state == State.dashing)) return;
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


	private const float SLASHING_X_DIST = 1f;
	private const float SLASHING_Y_DIST = 0.5f;
	private const float AUTOPATH_Y_THRESHOLD = 1.5f; 
	private const float AUTOPATH_Y_FACTOR = 6.25f;
	private const float JUMP_X_THRESHOLD = 3.5f;

	// method to handle the autopathing
	private void AutoPath() {

		rb.gravityScale = GRAVITY_SCALE;
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y + 0.5f;

		// timeout if the player
		if (Time.time > autoPathStartTime + AUTOPATH_TIMEOUT) {
			state = State.idle;
			rb.velocity = new Vector2(0, 0);
			return;
		}

		float tempSlashingXDist = SLASHING_X_DIST;
		if (!Input.GetMouseButton(0) && attackType == AttackType.none) tempSlashingXDist /= 3;

		bool positionReached = Mathf.Abs(xDist) < tempSlashingXDist && 		// we are close enough in the x direciton
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

		if (Mathf.Abs(xDist) >= tempSlashingXDist) 
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
		else if (Mathf.Abs(xDist) <= 0.05) {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}

		if (yDist >= AUTOPATH_Y_THRESHOLD && xDist <= JUMP_X_THRESHOLD && grounded) {
			StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR, 20f)));
		}
	}

	private const float JUMP_DELAY = 0.1f;
	private bool jumping = false;
	private IEnumerator Jump(float jumpPower) {
		jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		PoolManager.instance.ReuseObject(dustCloudPrefab, jumpPos, transform.rotation, transform.localScale);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(JUMP_DELAY);
		
		jumping = false;
		yield return null;
	}

	private const float READY_FLOAT_TIMEOUT = 0.5f;
	private float readyStartTime;
	// method for when autopathing is complete and ready to make an attack
	private void Ready() {
		
		rb.velocity = new Vector3(0, 0);
		Attack();	// will do nothing unless an attack is set

		if ((attackType == AttackType.none && 
			state != State.dashing && 
			state != State.slashing &&
			(Time.time - readyStartTime > READY_FLOAT_TIMEOUT && !grounded) // time out if floating
			)) {

			state = State.idle;
			attackType = AttackType.none;
		}
		
		rb.gravityScale = 0;
	}

	// method to handle dashing
	// this is only called when auto pathing is completed!
	private void Dash() {
		gameObject.layer = 14;
		if (stamina.isExhausted()) {
			rb.velocity = new Vector3(0, 0, 0);
			state = State.idle;
			attackType = AttackType.none;
			return;
		}
		
		float distanceB = Vector2.Distance(rb.position, targetB);
		stamina.DecreaseStamina(dashStaminaCost * distanceB / 2);

		if (Time.time > attackStartTime + ATTACK_TIMEOUT) {
			state = State.idle;
			attackType = AttackType.none;
		}
		
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.velocity = (targetB - rb.position) * DASH_SPEED;
			SpawnSwordAfterimage();
		} 

		// otherwise we have completed the dash
		else {
			rb.velocity = new Vector3(0, 0, 0);
			gameObject.layer = 11;
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
		if (attackType != AttackType.none) stamina.DecreaseStamina(slashStaminaCost);
		attackStartTime = Time.time;

		// handles current attack type
		HandleAttack();

		UpdateAnimatorVariables();
	}

	private const float MIN_ATTACK_THRESH = 0.2f;
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
			state = State.idle;
			attackType = AttackType.none;
		}
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
		if (other.collider.name.Length < 4) Debug.Log("Object names must be greater than 4");
		switch (other.collider.name.Substring(0, 4)) {
			case "Coin":
				coinCount++;
				coinCountText.text = "" + coinCount;
				break;

			case "Hear":
				healthAmount += 1;
				healthAmount = Mathf.Min(healthAmount, maxHealth);
				health.HandleHealth(healthAmount);
				break;


			case "Spik":
				if (!invincible) Damage(0.5f, 0f, other.collider);
				rb.velocity = new Vector2(rb.velocity.x, 9f);
				break;
		}
	}

	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		switch (other.name) {
			case "EnemyHurtBox":
				if (state != State.dashing && state != State.slashing && state != State.damaged && !invincible) Damage(0.5f, 4f, other);
				else attackResponse = AttackResponse.normal;
				break;
		}
	}

	private void Damage(float damageAmount, float knockback, Collider2D source) {
		Debug.Log("Damaged");
		invincible = true;
		damagedStartTime = Time.time;
		attackResponse = AttackResponse.missed;
		state = State.damaged;
		if (knockback != 0)
			rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
				transform.position.y - source.transform.position.y + 1f);

		healthAmount -= damageAmount;
		if ( healthAmount < 0) healthAmount = 0;

		health.HandleHealth(healthAmount);
	}

	private float damagedStartTime;
	private void Damaged() {
		spriteRenderer.color = Color.red;
		if (healthAmount == 0f) StartCoroutine(Death());		

		// check if done being damaged
		if (Time.time - damagedStartTime > damagedTime) {
			spriteRenderer.color = Color.white;
			state = State.idle;
			StartCoroutine(ToggleAlpha());
		}
	}

	public int invincibleFlashes = 4;
	private IEnumerator ToggleAlpha() {
		for (int i = 0; i < invincibleFlashes; i++) {
			Color color = Color.white;
			if (i % 2 == 0) color.a = hurtAlpha;
			spriteRenderer.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		Debug.Log("invincible = false");
		invincible = false;
		yield return null;
	}

	private IEnumerator Death() {
		
		Debug.Log("Player died!");
		state = State.damaged;
		yield return new WaitForSeconds(0.1f);

		state = State.dead;
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(1);

		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}

	private void HandleState() {
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

			case State.dead:
				break;

			case State.slashing:
				CheckForSlashEnd();
				rb.gravityScale = 0;
				break;

			case State.idle:
				gameObject.layer = 11;
				rb.velocity = new Vector2(0, rb.velocity.y);
				rb.gravityScale = GRAVITY_SCALE;
				spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
				if (grounded) stamina.IncreaseStamina(generateStamina);
				break;
		}	
	}

	private void HandleAttack() {
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
				attackResponse = AttackResponse.none;
				break;
		}
	}
}
