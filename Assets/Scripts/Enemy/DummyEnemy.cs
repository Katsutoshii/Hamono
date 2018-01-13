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
    if (source.transform.position.x < transform.position.x) leftHit = true;
    else rightHit = true;
    PoolManager.instance.ReuseObject(coinPrefab, transform.position, transform.rotation, coinPrefab.transform.localScale);
    StartCoroutine(ResetHit()); // stops animation loop
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
