using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedFlyingEnemy : FlyingEnemy {

    public float bulletFrequency;
    private GameObject bulletPrefab;

    public override void Start() {
        base.Start();
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FlyerBullet");
        StartCoroutine(ShootBullet());
    }   

    private IEnumerator ShootBullet() {
        while (true) {
        yield return new WaitForSeconds(bulletFrequency);
            FlyerBullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<FlyerBullet>();
        }

    }

}
