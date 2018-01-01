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

	public void Ready() {
		spriteRenderer.sortingLayerName = "Entities";
		gameObject.layer = LayerMask.NameToLayer("Enemies");
		boxCollider2D.enabled = true;
	}

	public override void Update() {
		base.Update();
		if (boss.state == Boss.State.entering) rb.position = boss.transform.position + offset;
	}

	public override void UpdateHealthBar() {
  	}

	// checks to see if it's close enough to player
	public override void CheckForPlayerProximity() {
		lockOnPlayer = true;
	}

	protected override void RotateBasedOnDirection() {
  	}

	protected override void AutoPath() {
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

		if (Mathf.Abs(xDist) < attackRange) {
			StartCoroutine(Attack());
				return;
			}

		// if we need to move in the x or y direction, do so
			if (Mathf.Abs(xDist) >= 0.1) 
				rb.velocity = new Vector2(xDist * KP, 0);
	}

	protected override IEnumerator Attack() {
		state = State.attacking;
		Debug.Log("slam down!"); 

		// attack here
		yield return new WaitForSeconds(1);
		state = State.idle;
	}
}
