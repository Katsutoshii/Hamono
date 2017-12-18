using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : Enemy {
  public override void Start() {
    Debug.Log("DummyEnemy start");
    base.Start();
  }
}
