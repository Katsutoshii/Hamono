using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Boss : MonoBehaviour {

	public StaminaBar healthBar;	// health bar object
	public float healthAmount;		// boss current hp amount
	public float maxHealthAmount; 	// boss total health
	public float speedX;			// horizontal speed
	public float speedY;			// vertical speed
	public Vector2 target;			// target location to move towards
	public Player player;			// player
	private BossFist leftFist;		// the left fist for phase 1
	private BossFist rightFist;		// the right fist for phase 1
	public BossLaserHands laserHands;
	private NPC finalMessage;

	private int phase;			// which phase we are on
	private Rigidbody2D rb;
	public enum State {
		entering,
		idle,
		autoPathing,
		damaged,
		dead
	}

	public State state;

	// Use this for initialization
	void Start () {
		phase = 0;
		state = State.entering;
		healthAmount = 100;

		player = FindObjectOfType<Player>();
		rb = GetComponent<Rigidbody2D>();
		healthBar = GameObject.Find("BossHealthBar").GetComponent<StaminaBar>();
		leftFist = transform.Find("LeftFist").GetComponent<BossFist>();
		rightFist = transform.Find("RightFist").GetComponent<BossFist>();
		laserHands = transform.Find("LaserHands").GetComponent<BossLaserHands>();
		finalMessage = GameObject.Find("Speech").GetComponent<NPC>();

		StartCoroutine(RiseToLevel());
	}
	
	// Update is called once per frame
	void Update () {
		HandleState();
		CheckForBossPhaseChange();
	}

	private void HandleState() {
		healthBar.SetStamina(healthAmount / 100);
		switch (state) {
			case State.idle:
				Idle();
				break;

			case State.autoPathing:
				AutoPath();
				break;

			default:
				break;
		}
	}

	private void CheckForBossPhaseChange() {
		if (leftFist == null && rightFist == null && phase < 1) {
			// introduces the laser hands
			Debug.Log("handle next level");
			phase += 1;
			laserHands.Activate();
		}

		else if (healthAmount <= 0) {
			StartCoroutine("Death");
			state = State.dead;
		}
	}

	public void StartBattle() {
		Debug.Log("Starting battle");
		rb.velocity = Vector2.zero;
		
		leftFist.Ready();
		rightFist.Ready();
		state = State.autoPathing;
		
		StartCoroutine(AttackCycle());
	}

	public float attackCycleTime;
	private IEnumerator AttackCycle() {
		while (phase < 2) {
			yield return new WaitForSeconds(attackCycleTime);
			target = new Vector2(player.transform.position.x, transform.position.y);

			if (TargetReachedX()) {
				FistAttack();
				yield return new WaitUntil(AttackFinished);
			}
		}

		yield return null;
	}

	private bool AttackFinished() {
		if (leftFist.state == Enemy.State.idle && rightFist.state == Enemy.State.idle
				|| leftFist == null && rightFist.state == Enemy.State.idle
				|| leftFist.state == Enemy.State.idle && rightFist == null) {
			state = State.autoPathing;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Makes the fists attack
	/// </summary>
	private void FistAttack() {
		Debug.Log("boss attack!");
		state = State.idle;

		if (player.transform.position.x < transform.position.x) 
			leftFist.state = Enemy.State.walking;
		else 
			rightFist.state = Enemy.State.walking;
	}

	private void Idle() {
		rb.velocity = Vector2.zero;
		target = player.transform.position;
	}

	private void AutoPath() {
		float distX = target.x - transform.position.x;

		rb.velocity = Vector2.right * Mathf.Min(Mathf.Abs(speedX * distX), speedX) * Mathf.Sign(distX);
	}

	
	private bool TargetReachedX() {
		return Mathf.Abs(target.x - transform.position.x) < 6;
	}

	private bool TargetReachedY() {
		return Mathf.Abs(target.y - transform.position.y) < 0.1f;
	}

	private IEnumerator RiseToLevel() {
		// Debug.Log("Rising to level");
		target = new Vector2(transform.position.x, 1f);

		rb.velocity = Vector2.up * speedY;
		yield return new WaitUntil(TargetReachedY);
		// Debug.Log("Level reached");
		StartBattle();
		yield return null;
	}

	private IEnumerator Death() {
		// Debug.Log("Rising to level");
		target = new Vector2(transform.position.x, -1f);

		rb.velocity = Vector2.down * speedY;
		laserHands.Die();
		yield return new WaitUntil(TargetReachedY);

		EndGame();
		// Debug.Log("Level reached");
		yield return null;
	}

	private void EndGame() {
		// TODO: end the game!
		// SceneManager.LoadScene(0);	// loads title screen
		finalMessage.StartTextTyper();
	}
}
