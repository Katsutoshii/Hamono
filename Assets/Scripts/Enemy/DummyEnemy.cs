using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Enemy {
  public override void Start() {
    Debug.Log("DummyEnemy start");
    base.Start();
  }

  public override void Damage(float damageAmount, float knockback, Collider2D source) {
    base.spriteRenderer.color = Color.red;
    base.state = State.damaged;
  }

  public override void Damaged() {
		spriteRenderer.color = Color.red;
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		
		if (Time.time - base.damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Enemies");

			state = State.walking;
		}
	}

  // overriding functions to suppress errors
  public override void StaticHealthBar() {}

  public override void UpdateHealthBar() {}

  public override void CheckForPlayerProximity() {}
}
