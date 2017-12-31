using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : Enemy {
    private GameObject laserPrefab;
    public override void Start() {
        base.Start();
        
        laserPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/SamuraiLaser");
    }

    public void LaserOut() {
        PoolManager.instance.ReuseObject(laserPrefab, transform.position, transform.rotation, laserPrefab.transform.localScale);
    }

    // kill without destroying game object
    public override void Kill() {

        spriteRenderer.color = Color.white;

        DropLoot();
    }
}