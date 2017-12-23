using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PooledObject {
    public float speed;
	protected SpriteRenderer spriteRenderer;
	protected Rigidbody2D rb;
    protected Animator animator;
	
	public override void OnObjectReuse() {
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
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemies")) {
            StartCoroutine(HitAnimation());
        }
    }

    protected float HIT_ANIMATION_DURATION = 0.5f;
    protected virtual IEnumerator HitAnimation() {
        rb.velocity = Vector2.zero;
        animator.SetBool("collided", true);
        yield return new WaitForSeconds(HIT_ANIMATION_DURATION);
        gameObject.SetActive(false);
    }
}
