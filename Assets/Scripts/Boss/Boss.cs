using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Boss : MonoBehaviour {

	public float healthAmount;
	public float maxHealthAmount;
	public int phase;
	public float speedX;
	public float speedY;
	public Vector2 target;
	private Player player;
	private BossFist leftFist;
	private BossFist rightFist;
	private BossHand leftHand;
	private BossHand rightHand;
	private GameObject laserHands;
	private int levelCount;
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
		levelCount = 0;
		state = State.entering;
		healthAmount = 100;

		player = FindObjectOfType<Player>();
		rb = GetComponent<Rigidbody2D>();
		leftFist = transform.Find("LeftFist").GetComponent<BossFist>();
		rightFist = transform.Find("RightFist").GetComponent<BossFist>();
		// laserHands = FindLaserHands();

		StartCoroutine(RiseToLevel());
	}
	
	// Update is called once per frame
	void Update () {
		 HandleState();
		//  HandleNextLevel();
	}

	private void HandleState() {
		switch (state) {
			case State.idle:
				Idle();
				break;

			case State.autoPathing:
				AutoPath();
				break;
		}
	}

	private void HandleNextLevel() {
		if (leftFist == null && rightFist == null && levelCount < 1) {
			// introduces the laser hands
			levelCount += 1;
			laserHands.SetActive(true);
			leftHand = laserHands.transform.Find("LeftHand").GetComponent<BossHand>();
			rightHand = laserHands.transform.Find("RightHand").GetComponent<BossHand>();
		}
	}

	private GameObject FindLaserHands() {
		Transform[] trs = this.gameObject.GetComponentsInChildren<Transform>(true);
		foreach(Transform t in trs) {
			if (t.name == "LaserHands") {
				Debug.Log("found the laser hands");
				return t.gameObject;
			}
		}
		return new GameObject();
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
				Attack();
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

	private void Attack() {
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
		Debug.Log("Rising to level");
		target = new Vector2(transform.position.x, 1f);

		rb.velocity = Vector2.up * speedY;
		yield return new WaitUntil(TargetReachedY);
		Debug.Log("Level reached");
		StartBattle();
		yield return null;
	}
}
