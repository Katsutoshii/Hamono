using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject {
	private SpriteRenderer spriteRenderer;
	private Rigidbody2D rb;
    private Animator animator;
	
	public override void OnObjectReuse() {

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		spriteRenderer.color = Color.white;

		rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("collided", false);
		
	}

	/// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D collider = other.collider;

    }

    private const float HIT_ANIMATION_DURATION = 0.5f;
    private IEnumerator HitAnimation() {
        animator.SetBool("collided", true);
        yield return new WaitForSeconds(HIT_ANIMATION_DURATION);
        gameObject.SetActive(false);
    }
}