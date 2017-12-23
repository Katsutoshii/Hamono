using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy {

    public float bulletFrequency;
	public bool jumping = false;
    private GameObject bulletPrefab;

    public override void Start() {
        base.Start();
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FlyerBullet");
        StartCoroutine(ChangeRandomWalkCycle());
        StartCoroutine(ShootBullet());
    }   

    bool randomWalkToRight;
    public float randomChangetime;
    private IEnumerator ChangeRandomWalkCycle() {
        while (true) {
            randomWalkToRight = Random.Range(0, 1f) >= 0.5f;
            yield return new WaitForSeconds(randomChangetime);
        }
    }
    

    private void RandomWalk() {
        if (!jumping) {
            if (randomWalkToRight) {
                rb.velocity = new Vector2(walkingSpeed, (rb.velocity.y < -3f) ? 2f : rb.velocity.y);
            } else {
                rb.velocity = new Vector2(-walkingSpeed, (rb.velocity.y < -3f) ? 2f : rb.velocity.y);
            }
        }
    }

    protected override void Walk() {
        spriteRenderer.color = Color.white;

        if (lockOnPlayer) {
            // follow the player
            if (!prevNotice) {
                state = State.noticed;
                prevNotice = true;
                noticedStartTime = Time.time;
            }
            AutoPath();
        }

        else {
            // randomly walk around
            healthBarPrefab.GetComponent<Canvas>().enabled = false;
            RandomWalk();
        }
    }

    protected virtual void AutoPath() {
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

        if (Mathf.Abs(xDist) < attackRange) {
            StartCoroutine(Attack());
            return;
        }

        // if we need to move in the x or y direction, do so
        if (Mathf.Abs(xDist) >= 0.1) 
            rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
        
        // hovers at correct height
        if (Mathf.Abs(yDist) <= 0.5 || player.transform.position.y >= transform.position.y + 0.5f)
            rb.velocity = new Vector2(rb.velocity.x, 3f);

        RotateBasedOnDirection();
    }

    private IEnumerator ShootBullet() {
        while (true) {
        yield return new WaitForSeconds(bulletFrequency);
            FlyerBullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<FlyerBullet>();
        }

    }

}
