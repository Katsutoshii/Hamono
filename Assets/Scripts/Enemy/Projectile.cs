using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject {
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
    private Animator animator;
	
	public override void OnObjectReuse() {
        Debug.Log("making laser!");
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = Color.white;

		rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("collided", false);
		
	}

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name != "RangedSamurai") {
            StartCoroutine(HitAnimation());
        }
    }

    private const float HIT_ANIMATION_DURATION = 0.5f;
    private IEnumerator HitAnimation() {
        animator.SetBool("collided", true);
        yield return new WaitForSeconds(HIT_ANIMATION_DURATION);
        gameObject.SetActive(false);
    }
}