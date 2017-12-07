using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour {

  public Player player;
  public Rigidbody2D rb;
  private SpriteRenderer spriteRenderer;
  public GameObject coinPrefab;
  public State state;

  public float walkingSpeed;
  public float distanceNearPlayer;
  
  private float direction;
  private bool lockOnPlayer;
  private float lastTime;
  private System.Random random;
  private float[] directionOptions = new float[] {-1f, 1f};

  public float healthAmount;
  public float receiveSlashDamage;
  public float receiveDashDamage;

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
    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
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
    
    switch (state) {
      case State.attacking:
        Attack();
        break;

      case State.damaged:
        spriteRenderer.color = Color.red;
        break;

      default:
        spriteRenderer.color = Color.white;
        break;
    }

    if (NearPlayer() || lockOnPlayer) {
      // follow the player
      AutoPath();
     } else {
      // randomly walk around
      RandomWalkCycle();
    }
  }

  // handles case when enemy runs into something
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      
      Debug.Log("collider: " + collider);

      switch (collider.gameObject.layer) {
        case 8: // we hit a wall, so turn around
          direction *= -1;
          transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
          break;
      }
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
      player.attackResponse = Player.AttackResponse.missed;
    } else {
      // the enemy is damaged
      player.attackResponse = Player.AttackResponse.normal;
      if (player.state == Player.State.dashing) Damage(receiveDashDamage);
    }
  }

  // enemy is damaged
  private void Damage(float damage) {
    state = State.damaged;
    
    healthAmount -= damage;
    // damage done by the player
    if (healthAmount <= 0) {
      state = State.dead;
      Debug.Log("Enemy: I died");
      Death();
    }
  }

  // enemy died
  private void Death() {
    // deletes the game object
    for (int i = 0; i < 4; i++)
      PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);

    Destroy(gameObject);
  }

  private Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + GetRandomNumber(0f, 0.5f),
      position.y + GetRandomNumber(0f, 0.5f),
      position.z);
  }

  public float GetRandomNumber(float minimum, float maximum)
  { 
      return ((float) random.NextDouble()) * (maximum - minimum) + minimum;
  }

  // follows player
  private void AutoPath() {
		rb.gravityScale = GRAVITY_SCALE;
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

    if (Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {
      state = State.attacking;
      direction = 0;
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

  /// <summary>
  /// Sent when another object enters a trigger collider attached to this
  /// object (2D physics only).
  /// </summary>
  /// <param name="other">The other Collider2D involved in this collision.</param>
  void OnTriggerEnter2D(Collider2D other)
  {
      Debug.Log("Trigger " + other.name + " enter!");

      switch (other.name) {
        case "SlashHurtBox":
          Damage(receiveSlashDamage);
          Debug.Log("Health = " + healthAmount);
          break;
      }
  }
}
