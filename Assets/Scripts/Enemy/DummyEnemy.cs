using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Enemy {

  protected bool leftHit;
  protected bool rightHit;

  public override void Start() {
    Debug.Log("DummyEnemy start");
    base.Start();
  }

  public override void Update() {
    base.HandleState();
    this.UpdateAnimatorVariables();
  }

  public override void Damage(float damageAmount, float knockback, Collider2D source) {
    base.spriteRenderer.color = Color.red;
    base.state = State.damaged;

    base.damagedStartTime = Time.time;

    if (source.transform.position.x < transform.position.x) {
      leftHit = true;
      base.animator.SetBool("leftHit", leftHit);
    }
    else {
      rightHit = true;
      base.animator.SetBool("rightHit", rightHit);
    }
  }

  public override void Damaged() {
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		if (Time.time - base.damagedStartTime > 1f) {
      leftHit = false;
      rightHit = false;
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Enemies");

			state = State.walking;
		}
	}

  protected virtual void UpdateAnimatorVariables() {
    base.animator.SetBool("leftHit", leftHit);
    base.animator.SetBool("rightHit", rightHit);
  }

  // overriding functions to suppress errors
  public override void StaticHealthBar() {}

  public override void UpdateHealthBar() {}

  public override void CheckForPlayerProximity() {}
}