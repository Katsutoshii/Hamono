using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : Enemy {
    public override void Start() {
        Debug.Log("JumpingEnemy start");
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
    

    private void RandomWalkCycle() {
        if (grounded) {
            if (randomWalkToRight) {
                rb.velocity = walkingSpeed * Vector2.right;
                if (rb.velocity.x > 0) StartCoroutine(Jump(jumpingPower));
            } 
            else {
                rb.velocity = walkingSpeed * Vector2.left;
                if (rb.velocity.x < 0) StartCoroutine(Jump(jumpingPower));
            }
        }
    }

    protected override void Walk() {
        RotateBasedOnDirection();
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
            RandomWalkCycle();
        }
    }
}