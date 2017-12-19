using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Enemy {

  public string typeOfBot;

  protected bool leftHit;
  protected bool rightHit;

  public override void Start() {
    base.Start();
  }

  public override void Update() {
    base.HandleState();
    this.UpdateAnimatorVariables();
  }

  public override void GetHealthBar() {}

  public override void Damage(float damageAmount, float knockback, Collider2D source) {
    base.state = State.damaged;

    base.damagedStartTime = Time.time;

    HandleAttack(source);
  }
  
  private void HandleAttack(Collider2D source) {
    if (source.transform.position.x < transform.position.x) {
      switch(player.attackType) {

        case Player.AttackType.straightSlash:
        if (typeOfBot == "body") {
          leftHit = true;
          base.animator.SetBool("leftHit", leftHit);
        }
        break;

        case Player.AttackType.upSlash:
        if (typeOfBot == "head") {
          leftHit = true;
          base.animator.SetBool("leftHit", leftHit);
        }
        break;

        case Player.AttackType.downSlash:
        if (typeOfBot == "legs") {
          leftHit = true;
          base.animator.SetBool("leftHit", leftHit);
        }
        break;

        default:
          rightHit = true;
          base.animator.SetBool("rightHit", rightHit);
        break;
      }

    }
    else {
      switch(player.attackType) {

        case Player.AttackType.straightSlash:
        if (typeOfBot == "body") {
          rightHit = true;
          base.animator.SetBool("rightHit", rightHit);
        }
        break;

        case Player.AttackType.upSlash:
        if (typeOfBot == "head") {
          rightHit = true;
          base.animator.SetBool("rightHit", rightHit);
        }
        break;

        case Player.AttackType.downSlash:
        if (typeOfBot == "legs") {
          rightHit = true;
          base.animator.SetBool("rightHit", rightHit);
        }
        break;
        
        default:
          rightHit = true;
          base.animator.SetBool("rightHit", rightHit);
        break;
      }
    }
  }

  public override void Damaged() {
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		if (Time.time - base.damagedStartTime > 0.5f) {
      leftHit = false;
      rightHit = false;
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Dummies");

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
