using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiEnemy : Enemy {
    public override void Start() {
        Debug.Log("SamuraiEnemy start");
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

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    public override void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.name) {
            case "PlayerSlashHurtBox":
                if (player.attackType == Player.AttackType.straightSlash)
                    Damage(receiveSlashDamage, receiveSlashKnockback, other);
                else {
                    player.attackResponse = Player.AttackResponse.blocked;
                    Damage(0, receiveSlashKnockback/2, other);
                }
                
                break;

            case "PlayerDashHurtBox":
                player.attackResponse = Player.AttackResponse.blocked;
                Damage(0, receiveDashKnockback/2, other);
                break;
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

    
	private const float JUMP_DELAY = 0.5f;
	private bool jumping = false;
	protected IEnumerator Jump(float jumpPower, float xVelocity) {
        Debug.Log("Enemy jump!");
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
}
