using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiLaser : Projectile {
    public override void OnObjectReuse() {
        base.OnObjectReuse();
        Player player = FindObjectOfType<Player>();
        Vector2 direction = player.transform.position - transform.position;
        direction /= direction.magnitude;
        rb.velocity = direction * speed;
	}
}