using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour {

  public Player player;
  public Rigidbody2D rb;
  public State state;

  public float walkingSpeed;
  public float distanceNearPlayer;
  
  private float direction;
  private bool lockOnPlayer;
  private float lastTime;
  private System.Random random;
  private float[] directionOptions = new float[] {-1f, 1f};

  public float healthAmount;
  public float receiveDamage;

  public enum State {
    idle,
    walking,
    attacking,
    damaged,
    dead,
  }

  // constants
	public float SLASHING_X_DIST;
	public float SLASHING_Y_DIST;
  public float KP;
  public float GRAVITY_SCALE;

  private float autoPathStartTime;

  void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
    lockOnPlayer = false;
    lastTime = Time.time;
    random = new System.Random();
    state = State.walking;
    direction = walkingSpeed;
  }

  void Update() {
    rb.velocity = new Vector2(direction, rb.velocity.y);

    if (direction < 0)
      transform.localScale = new Vector3(-1, 1, 1);
    else
      transform.localScale = new Vector3(1, 1, 1);

    bool nearPlayer = NearPlayer();
    if (nearPlayer || lockOnPlayer) {
      // follow the player
      AutoPath();
    } else {
      // randomly walk around
      RandomWalkCycle();
    }
  }

  // handles case when enemy runs into a wall
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      if (collider.gameObject.layer == 8) {
        direction *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
      }
      Debug.Log("collider: " + collider);
  }

  private void RandomWalkCycle() {
    float currentTime = Time.time;
    if (currentTime - lastTime >= 3f) {
      lastTime = currentTime;
      if (rb.velocity.x != 0) {
        direction = 0;
        state = State.idle;
      } else {
        float directionScale = directionOptions[random.Next(directionOptions.Length)];
        direction = walkingSpeed * directionScale;
        state = State.walking;
      }
    }
  }

  // checks to see if it's close enough to player
  private bool NearPlayer() {
    float distance = Vector2.Distance(transform.position, player.transform.position);
    if (distance <= distanceNearPlayer) {
      lockOnPlayer = true;
      return true;
    }
    // the player got out of range for the enemy to follow her
    if (distance >= 10f)
      lockOnPlayer = false;
    return false;
  }

  // attacks player
  private void Attack() {
    // attack the player
    if (player.state != Player.State.dashing && player.state != Player.State.slashing) {
      // the player is damaged
    } else {
      // the enemy is damaged
      Debug.Log("Enemy: I'm damaged");
      state = State.damaged;
      Damage();
    }
  }

  // enemy is damaged
  private void Damage() {
    // damage done by the player
    if (healthAmount <= 0) {
      state = State.dead;
      Debug.Log("Enemy: I died");
      Death();
    }
    healthAmount -= receiveDamage;
  }

  // enemy died
  private void Death() {
    // deletes the game object
    Destroy(gameObject);
  }

  // follows player
  private void AutoPath() {
		rb.gravityScale = GRAVITY_SCALE;
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

    if (Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {
      state = State.attacking;
      Attack();
			return;
		}

		// fixes overshooting
		if (Mathf.Abs(xDist) < SLASHING_X_DIST)
			rb.velocity = new Vector2(0, rb.velocity.y);

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST) {
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
      direction = xDist * KP;
    }
  }
}
