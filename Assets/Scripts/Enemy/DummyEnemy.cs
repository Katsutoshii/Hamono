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
        switch(typeOfBot) {
          case "normal":
            leftHit = true;
            base.animator.SetBool("leftHit", leftHit);
            PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            break;

          case "head":
            if (player.attackType == Player.AttackType.upSlash) {
              leftHit = true;
              base.animator.SetBool("leftHit", leftHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
            break;

          case "body":
            if (player.attackType == Player.AttackType.straightSlash) {
              leftHit = true;
              base.animator.SetBool("leftHit", leftHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
            break;

          case "legs":
            if (player.attackType == Player.AttackType.downSlash) {
              leftHit = true;
              base.animator.SetBool("leftHit", leftHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
            break;
        }

    }
    else {
      switch(typeOfBot) {
          case "normal":
            rightHit = true;
            base.animator.SetBool("rightHit", rightHit);
            PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            break;

          case "head":
            if (player.attackType == Player.AttackType.upSlash) {
              rightHit = true;
              base.animator.SetBool("rightHit", rightHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
            break;

          case "body":
            if (player.attackType == Player.AttackType.straightSlash) {
              rightHit = true;
              base.animator.SetBool("rightHit", rightHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
            break;

          case "legs":
            if (player.attackType == Player.AttackType.downSlash) {
              rightHit = true;
              base.animator.SetBool("rightHit", rightHit);
              PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
            }
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
