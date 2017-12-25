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

    public override void Kill() {

        spriteRenderer.color = Color.white;

        // deletes the game object
        for (int i = 0; i < 4; i++)
        PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
    }
}