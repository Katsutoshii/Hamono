using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyerBullet : Projectile {
  Player player;

  void Start() {
    OnObjectReuse();
    HIT_ANIMATION_DURATION = 0.1f;
  }

  public override void OnObjectReuse() {
    base.OnObjectReuse();
    player = FindObjectOfType<Player>();
    Vector2 direction = player.transform.position - transform.position;
    direction /= direction.magnitude;
    rb.velocity = direction * speed;
  }
}
