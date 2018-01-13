using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossFist : Enemy {

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
		gameObject.layer = LayerMask.NameToLayer("Default");
	}

	// after the entry, initializes the fist
	public void Ready() {
		Debug.Log("fist ready");
		spriteRenderer.sortingLayerName = "Entities";
		gameObject.layer = LayerMask.NameToLayer("Enemies");
		boxCollider2D.enabled = true;
	}

	public override void Update() {
		base.Update();
		if (boss.state == Boss.State.entering) rb.position = boss.transform.position + offset;
	}

	protected override void RotateBasedOnDirection() {
		// prevents any rotation
  	}

	protected override void AutoPath() {
		// Debug.Log("Autopathing for " + gameObject.name);
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

		if (Mathf.Abs(xDist) < attackRange) {
			state = State.attacking;
			StartCoroutine(Attack());
			return;
		}

		// if we need to move in the x or y direction, do so
			if (Mathf.Abs(xDist) >= 0.1) 
				rb.velocity = new Vector2(xDist * KP + 0.5f * Mathf.Sign(xDist), 0);
	}

	public float dropSpeed;
	public float risingSpeed;
	protected override IEnumerator Attack() {
		Debug.Log("move up a bit!");
		yield return new WaitForSeconds(0.5f);

		Debug.Log("slam down!"); 
		rb.velocity = Vector2.zero + Vector2.down * dropSpeed;
		yield return new WaitUntil(() => grounded);

		rb.velocity = Vector2.zero;
		yield return new WaitForSeconds(2);
		
		Debug.Log("rising up");
		rb.velocity = Vector2.up * risingSpeed;
		yield return new WaitUntil(() => rb.position.y >= 1.56f);
		state = State.idle;
	}

	public override void Damage(float damageAmount, float knockback, Collider2D source) {
		if (grounded) damageAmount = 0;
		base.Damage(damageAmount, knockback, source);
	}

	protected override void Idle() {
		rb.velocity = Vector2.zero;
	}
}
