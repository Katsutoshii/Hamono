using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy {
    public override void Start() {
        base.Start();
        StartCoroutine(ChangeRandomWalkCycle());
    }
    
    bool randomWalkToRight;
    public float randomChangetime;
    private IEnumerator ChangeRandomWalkCycle() {
        while( true) {
            randomWalkToRight = Random.Range(0, 1f) >= 0.5f;
            yield return new WaitForSeconds(randomChangetime);
        }
    }
    

    private void RandomWalk() {
        if (!jumping) {
            if (randomWalkToRight) rb.velocity = new Vector2(walkingSpeed, rb.velocity.y);
            else rb.velocity = new Vector2(-walkingSpeed, rb.velocity.y);
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

	public bool jumping = false;
	protected IEnumerator Jump(float jumpPower) {
		jumping = true;
		rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        RotateBasedOnDirection();
		yield return new WaitUntil(() => rb.velocity.y <= 0.1f);

        jumping = false;
		yield return null;
	}

    public void MakeJump() {
        if (!jumping) {
            float yDist = player.transform.position.y - transform.position.y;
            StartCoroutine(Jump(Mathf.Max(2 * yDist, 1f)));
        }
    }
}