using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiEnemy : Enemy {
    public override void Start() {
        Debug.Log("SamuraiEnemy start");
        base.Start();
        
        hurtBox.offset = Vector2.zero;
        hurtBox.size = Vector2.zero;

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
        if (grounded) {
            if (randomWalkToRight) rb.velocity = Vector2.right * walkingSpeed;
            else rb.velocity = Vector2.left * walkingSpeed;
            RotateBasedOnDirection();

        }
    }

    protected override void Walk() {
        if (stunned) return;

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
    public virtual void LaserOut() {
        hurtBox.offset = new Vector2(-1f, 0);
        hurtBox.size = Vector2.one;
    }

    public virtual void LaserIn() {
        hurtBox.offset = Vector2.zero;
        hurtBox.size = Vector2.zero;
    }
}
