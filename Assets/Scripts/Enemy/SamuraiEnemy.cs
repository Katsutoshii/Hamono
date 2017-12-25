using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiEnemy : Enemy {
    public override void Start() {
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
    

    protected override void RandomWalk() {
        if (grounded) {
            if (randomWalkToRight) rb.velocity = Vector2.right * walkingSpeed;
            else rb.velocity = Vector2.left * walkingSpeed;
            RotateBasedOnDirection();
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
