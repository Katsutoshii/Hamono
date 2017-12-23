using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiLaser : Projectile {
    Player player;
    public override void OnObjectReuse() {
        base.OnObjectReuse();
        player = FindObjectOfType<Player>();
        Vector2 direction = player.transform.position - transform.position;
        direction /= direction.magnitude;
        rb.velocity = direction * speed;
	}
}
