﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : PooledObject {
	private Player player;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D trigger;
	private Collider2D collector;
	public AudioClip collected;
	private Rigidbody2D rb;
	private Animator animator;

	public float gravitationalPullDistance;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Player>();
		trigger = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = false;
		spriteRenderer = GetComponent<SpriteRenderer>();

		animator = GetComponent<Animator>();
		animator.SetBool("collected", false);
		spriteRenderer.color = Color.white;

		trigger.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
		if (distanceToPlayer <= gravitationalPullDistance)
			AutoPath();
	}

	// follows user if close enough
	private void AutoPath() {
		rb.gravityScale = 2f;
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y;

		if (yDist >= .2f && xDist <= .2f) {
			rb.velocity = new Vector2(xDist * 5f, yDist * 5f);
		}
  }

	public override void OnObjectReuse() {
		
		trigger = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
		rb.isKinematic = false;
		
		rb.simulated = true;
		spriteRenderer = GetComponent<SpriteRenderer>();

		animator = GetComponent<Animator>();
		animator.SetBool("collected", false);
		spriteRenderer.color = Color.white;

		trigger.enabled = true;
    }

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		switch (other.collider.name) {
			case "Player":
				collector = other.collider;
				Collect();
				break;
      	}
	}

	private void Collect() {
		animator.SetBool("collected", true);
		trigger.enabled = false;
		rb.simulated = false;
		StartCoroutine(Fade());
	}

	private IEnumerator Fade() {
		
		Vector3 velocity = rb.velocity;
		for (float a = 1f; a > 0; a -= 0.1f) {
			rb.position = Vector3.SmoothDamp(transform.position, collector.transform.position, ref velocity, 0.5f, 10f);
			Color color = spriteRenderer.color;
			color.a = a;
			spriteRenderer.color = color;
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(0.2f);

		gameObject.SetActive(false);
		yield return null;
	}
}
