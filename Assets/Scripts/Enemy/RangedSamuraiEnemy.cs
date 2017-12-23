using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSamuriaenemy : SamuraiEnemy {
    private GameObject laserPrefab;
    public override void Start() {
        Debug.Log("RangedSamuraiEnemy start");
        base.Start();
        
        laserPrefab = Resources.Load<GameObject>("Prefabs/Enemies/SamuraiLaser");
    }

    public override void LaserOut() {
        PoolManager.instance.ReuseObject(laserPrefab, transform.position, transform.rotation, laserPrefab.transform.localScale);
    }

    public override void LaserIn() {

    }
}