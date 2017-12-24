using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSamuraiEnemy : SamuraiEnemy {
    private GameObject laserPrefab;
    public override void Start() {
        base.Start();
        
        laserPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/SamuraiLaser");
    }

    public override void LaserOut() {
        PoolManager.instance.ReuseObject(laserPrefab, transform.position, transform.rotation, laserPrefab.transform.localScale);
    }

    public override void LaserIn() {

    }
}