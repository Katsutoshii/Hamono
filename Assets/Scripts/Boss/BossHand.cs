using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHand : Enemy {

	public float deathDirection;
	public Vector3 offset;
	private Boss boss;
	public float speedX;
	private Vector2 target;
	private BoxCollider2D boxCollider2D;
	// Use this for initialization
	public override void Start () {
		base.Start();

		boxCollider2D = GetComponent<BoxCollider2D>();
		spriteRenderer.sortingLayerName = "BackgroundDetails";
		boss = GetComponentInParent<Boss>();
		state = State.idle;
	}

	// after the entry, initializes the fist
	public void Ready() {
		Debug.Log("fist ready");
		spriteRenderer.sortingLayerName = "Foreground";
		gameObject.layer = LayerMask.NameToLayer("Enemies");
		boxCollider2D.enabled = true;
	}

	public override void Update() {
		base.Update();
		if (state == State.idle) state = State.noticed;
		if (boss.state == Boss.State.entering) rb.position = boss.transform.position + offset;
	}

	protected override void RotateBasedOnDirection() {
		// prevents any rotation
  	}

	protected override void AutoPath() {
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

		if (Mathf.Abs(xDist) < attackRange) {
			state = State.attacking;
			StartCoroutine(Attack());
			return;
		}

		// if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= 0.1) 
			rb.velocity = new Vector2(xDist * KP + 1.5f * Mathf.Sign(xDist), 0);
	}

	public override void UpdateAnimatorVariables() {
		animator.SetFloat("speed", rb.velocity.magnitude);
		animator.SetBool("damaged", state == State.damaged);
		animator.SetBool("idle", state == State.idle);
		animator.SetBool("walking", state == State.walking);
		animator.SetBool("dead", state == State.dead);
		animator.SetBool("noticed", lockOnPlayer);
		animator.SetBool("grounded", grounded);
		animator.SetBool("blocking", state == State.blocking);
		animator.SetBool("attacking", state == State.attacking);
		animator.SetFloat("deathDirection", deathDirection);
  }

	public float dropSpeed;
	public float risingSpeed;
	public bool charging;

	protected override IEnumerator Attack() {
		Debug.Log("charging!");
		yield return new WaitForSeconds(1f);

		charging = true;
		Debug.Log("rising up");
		yield return new WaitUntil(() => rb.position.y >= 1.56f);

		rb.velocity = Vector2.zero;

		Debug.Log("rising done");
		if (state != State.dead) state = State.idle;
	}

	public override void Damage(float damageAmount, float knockback, Collider2D source) {
		if (grounded) {
			base.Damage(damageAmount, knockback, source);
			boss.healthAmount -= 5;
		}
	}

	protected override void Idle() {
		rb.velocity = Vector2.zero;
	}
}
