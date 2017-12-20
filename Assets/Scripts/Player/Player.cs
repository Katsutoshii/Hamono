using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Player : MonoBehaviour {

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

	public bool immediateAutoPathing;
	public State state;
	public AttackType attackType = AttackType.none;
	public AttackResponse attackResponse = AttackResponse.none;

	// player representation
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
	private Animator animator;

	// temporary state checkers
	public bool autoPathing;
	public bool grounded;
	public bool onEdge;
	public bool invincible;
	public bool paused;

	// directional targets
	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash

	// visible player information
	private SlashIndicator slashIndicator;
	private StaminaBar staminaBar;
	private HealthBar healthBar;
	public float maxHealth;
	public float healthAmount;
	public float damagedTime;
	public float hurtAlpha;

	// counters
	public int comboCount;
	public int coinCount;
	private Text coinCountText;
	public bool jumping = false;
	
	// ui/ux elements
	private GameObject pauseMenuPrefab;
	private AudioSource audioSource;
	public float generateStamina;
	
    public const float SLASHING_THRESHOLD = 3.25f;
	
	public const float GRAVITY_SCALE = 2f;

	// Use this for initialization
	void Start () {

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		audioSource = gameObject.GetComponent<AudioSource>();
		animator = gameObject.GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		
		rb.isKinematic = false;

		// start states
		state = State.idle;
		attackType = AttackType.none;

		// get UI objects
		slashIndicator = FindObjectOfType<SlashIndicator>();
		staminaBar = FindObjectOfType<StaminaBar>();
		healthBar = FindObjectOfType<HealthBar>();
		coinCountText = GameObject.Find("CoinCount").GetComponent<Text>();
		pauseMenuPrefab = Resources.Load<GameObject>("Prefabs/UI/PauseMenu");
		
		// create pools for attack effects
		PoolManager.instance.CreatePool(dustCloudPrefab, 1);
		PoolManager.instance.CreatePool(afterimagePrefab, 10);
		PoolManager.instance.CreatePool(swordAfterimagePrefab, 20);
	}
	
	// Update is called once per frame
	void Update() {
		if (!(state == State.damaged || state == State.dead)) Controls();
		if (!(state == State.dashing || state == State.slashing || invincible)) ResetLayer();

		// handles the pause menu
		if (Input.GetKeyDown("space") && !paused) PauseMenu();

		// handles the current state
		HandleState();

		// handles the attack responses
		HandleAttackResponses();	

		if(state != State.dashing) LimitVelocity();

		UpdateAnimatorVariables();
	}

	
	public float autoPathStartTime;
	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(1)) && state != State.talking && state != State.finishedTalking && !paused) {

			autoPathStartTime = Time.time;
			attackType = AttackType.none;
			if (immediateAutoPathing || Input.GetMouseButton(1)) state = State.autoPathing;
			targetA = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		} else if (state == State.finishedTalking) state = State.idle;

		if (Input.GetMouseButtonUp(0)) {
			
			if (!immediateAutoPathing) state = State.autoPathing;
			GetAttackType();
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

	private void Damage(float damageAmount, float knockback, Collider2D source) {
		Debug.Log("Damaged! invincible = " + invincible);
		invincible = true;
		damagedStartTime = Time.time;
		attackResponse = AttackResponse.missed;
		state = State.damaged;
		if (knockback != 0)
			rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
				transform.position.y - source.transform.position.y + 1f);

		healthAmount -= damageAmount;
		if ( healthAmount < 0) healthAmount = 0;

		healthBar.HandleHealth(healthAmount);
	}

	public float damagedStartTime;
	private void Damaged() {
		spriteRenderer.color = Color.red;
		if (healthAmount == 0) StartCoroutine(Death());		

		// check if done being damaged
		if (Time.time - damagedStartTime > damagedTime) {
			spriteRenderer.color = Color.white;
			ResetToIdle();
			StartCoroutine(ToggleAlpha());
		}
	}

	public int invincibleFlashes = 4;
	private IEnumerator ToggleAlpha() {
		gameObject.layer = LayerMask.NameToLayer("Dashing");
		for (int i = 0; i < invincibleFlashes; i++) {
			Color color = Color.white;
			if (i % 2 == 0) color.a = hurtAlpha;
			spriteRenderer.color = color;
			yield return new WaitForSeconds(0.1f);
		}
		invincible = false;
		gameObject.layer = LayerMask.NameToLayer("Player");
		yield return null;
	}

	private IEnumerator InvincibleBuffer() {
		Debug.Log("are we in here?");
		invincible = true;
		yield return new WaitForSeconds(1f);
		invincible = false;
		yield return null;
	}

	private IEnumerator Death() {
		state = State.damaged;
		yield return new WaitForSeconds(0.1f);

		state = State.dead;
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(1);

		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}

	private void PauseMenu() {
		GameObject pauseMenu = Instantiate(pauseMenuPrefab);
		paused = true;
	}

	private void HandleState() {
		switch (state) {
			
			case State.autoPathing:
				Controls();
				AutoPath();
				break;

			case State.ready:
				Controls();
				Ready();
				break;
			
			case State.dashing:
				Dash();
				break;

			case State.damaged:
				Damaged();
				break;

			case State.dead:
				break;

			case State.slashing:
				CheckForSlashEnd();
				break;

			case State.idle:
				Controls();
				if (grounded) staminaBar.IncreaseStamina(generateStamina);
				rb.gravityScale = GRAVITY_SCALE;
				rb.velocity = new Vector2(0, rb.velocity.y);
				break;
		}	
	}
	
	public void ResetToIdle() {
		state = Player.State.idle;
		gameObject.layer = LayerMask.NameToLayer("Player");
		attackType = Player.AttackType.none;
		rb.gravityScale = Player.GRAVITY_SCALE;
		rb.velocity = new Vector2(0, rb.velocity.y);
	}

	private void Idle() {
		rb.velocity = new Vector2(0, rb.velocity.y);
		rb.gravityScale = GRAVITY_SCALE;
		spriteRenderer.color = new Color(1f, 1f, 1f, spriteRenderer.color.a);
		if (grounded) staminaBar.IncreaseStamina(generateStamina);
	}
}
