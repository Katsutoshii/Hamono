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
	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash

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
	public bool jumping = false;
	

	

	// ui/ux elements
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;
	private AudioSource audioSource;
	public float generateStamina;
	
    public const float SLASHING_THRESHOLD = 3.5f;
	
	public const float GRAVITY_SCALE = 2f;



	// private start times
	public float attackStartTime;
	
	public float readyStartTime;
	private float alphaToggleTime;

	private PlayerMotion motionHandler;
	private PlayerAttacks attackHandler;

	// Use this for initialization
	void Start () {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
		rb.isKinematic = false;

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		audioSource = gameObject.GetComponent<AudioSource>();

		motionHandler = gameObject.GetComponent<PlayerMotion>();
		attackHandler = gameObject.GetComponent<PlayerAttacks>();

		// start states
		state = State.idle;
		attackType = AttackType.none;
	}
	
	// Update is called once per frame
	void Update() {
		if (!(state == State.damaged || state == State.dead)) Controls();
		if (grounded) stamina.IncreaseStamina(generateStamina);
		if (attackType == AttackType.none) attackResponse = AttackResponse.none;

		// handles the current state
		HandleState();	
		
		if (state != State.damaged) motionHandler.RotateSpriteForVelocity();
		if(state != State.dashing) motionHandler.LimitVelocity();

		UpdateAnimatorVariables();
	}

	
	public float autoPathStartTime;
	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking && state != State.finishedTalking) {

			autoPathStartTime = Time.time;
			state = State.autoPathing;
			targetA = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		} else if (state == State.finishedTalking) state = State.idle;

		if (Input.GetMouseButtonUp(0)) {
			attackHandler.GetAttackType();
		}
	}

	public void UpdateAnimatorVariables() {
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

	public float damagedStartTime;
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
				motionHandler.AutoPath();
				break;

			case State.ready:
				attackHandler.Ready();
				break;
			
			case State.dashing:
				attackHandler.Dash();
				break;

			case State.damaged:
				Damaged();
				break;

			case State.dead:
				break;

			case State.slashing:
				attackHandler.CheckForSlashEnd();
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
}
