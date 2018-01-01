using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiEnemy : Enemy {
    public override void Start() {
        base.Start();
        
        foreach (BoxCollider2D box in hurtBox) {
            box.offset = Vector2.zero;
            box.size = Vector2.zero;
        }

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
        foreach (BoxCollider2D box in hurtBox) {
            box.offset = new Vector2(-1f, 0);
            box.size = Vector2.one;
        }
    }

    public virtual void LaserIn() {
        foreach (BoxCollider2D box in hurtBox) {
            box.offset = Vector2.zero;
            box.size = Vector2.zero;
        }
    }
}
