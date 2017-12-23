using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : Enemy {
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
        if (grounded && !jumping) {
            if (randomWalkToRight) StartCoroutine(Jump(jumpingPower, walkingSpeed));
            else StartCoroutine(Jump(jumpingPower, walkingSpeed));
        }
    }

    protected override void Walk() {

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
            RandomWalk();
        }
    }

    
	private const float JUMP_DELAY = 0.5f;
	private bool jumping = false;
	protected IEnumerator Jump(float jumpPower, float xVelocity) {
		jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		rb.velocity = Vector2.up * jumpingPower + Vector2.right * xVelocity;
        RotateBasedOnDirection();
		yield return new WaitForSeconds(JUMP_DELAY);
		
		jumping = false;
		yield return null;
	}

    protected override void AutoPath() {
        if (stunned) {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

        // adds jumping
        if (grounded && !jumping) StartCoroutine(Jump(jumpingPower, xDist));
    }
}
