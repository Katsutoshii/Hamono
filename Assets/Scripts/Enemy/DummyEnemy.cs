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

  public override void Damage(float damageAmount, float knockback, Collider2D source) {
    base.state = State.damaged;

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
    StartCoroutine(ResetHit());
  }

  private IEnumerator ResetHit() {
      yield return new WaitForSeconds(0.2f);
      rightHit = false;
      leftHit = false;
  }

  public override void UpdateAnimatorVariables() {
    base.UpdateAnimatorVariables();
    animator.SetBool("leftHit", leftHit);
    animator.SetBool("rightHit", rightHit);
  }

  public override void CheckForPlayerProximity() {}
}
