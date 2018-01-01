using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0219  

public class PoundHand : Enemy {

  public override void UpdateHealthBar() {}

  public override void CheckForPlayerProximity() {}

  public override void UpdateAnimatorVariables() {
    animator.SetBool("attacking", state == State.attacking);
  }
}
