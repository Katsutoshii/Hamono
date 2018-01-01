using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0219  

public class PoundHand : Enemy {

  private Vector2 originalScale;

  protected void Awake() {
    originalScale = transform.localScale;
  }

  public virtual void Update() {

    this.UpdateHealthBar();
    base.Update();

    transform.localScale = originalScale;
  }

  public override void UpdateHealthBar() {}

  public override void HandleState() {
    // switch (state) {
    //   // case State.idle:
    //   //   base.Idle();
    //   //   break;

    //   case State.walking:
    //     base.Walk();
    //     break;
    // }
    Walk();
    if (stunned) rb.velocity = new Vector2(0, rb.velocity.y);
  }
}
