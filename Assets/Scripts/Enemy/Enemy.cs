using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

  public Player player;
  public Rigidbody2D rb;
  private SpriteRenderer spriteRenderer;
  public GameObject coinPrefab;
  public State state;
  private Animator animator;
  public AudioSource audioSource;

  public float walkingSpeed;
  public float distanceNearPlayer;
  
  private float direction;
  private bool lockOnPlayer;
  private float lastTime;
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
    rb.isKinematic = false;

    audioSource = GetComponent<AudioSource>();
    animator = GetComponent<Animator>();
    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    lockOnPlayer = false;
    lastTime = Time.time;
    state = State.walking;
    direction = walkingSpeed;
    StartCoroutine(ChangeRandomWalkCycle());

  }

  void Update() {    
    RotateBasedOnDirection();
    CheckForPlayerProximity();

    switch (state) {
      case State.attacking:
        Attack();
        break;

      case State.damaged:
        Damaged();
        break;

      case State.walking:
        spriteRenderer.color = Color.white;
        Walk();
        break;
    }

    UpdateAnimatorVariables();
  }

  bool randomWalkToRight;
  public float randomChangetime;
  private IEnumerator ChangeRandomWalkCycle() {
    while( true) {
      Debug.Log("Changing direction");
      randomWalkToRight = Random.Range(0, 1f) >= 0.5f;
      yield return new WaitForSeconds(randomChangetime);
    }
  }

  private void Walk() {
    spriteRenderer.color = Color.white;

    if (lockOnPlayer) {
      // follow the player
      AutoPath();
    } 
    else {
      // randomly walk around
      RandomWalkCycle();
    }
  }

  private float damagedStartTime;
	private void Damaged() {
		spriteRenderer.color = Color.red;
		
		if (Time.time - damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;

			state = State.walking;
		}
	}

  private void UpdateAnimatorVariables() {
    animator.SetFloat("speed", rb.velocity.magnitude);
    animator.SetBool("damaged", state == State.damaged);
  }

  // handles case when enemy runs into something
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      
      Debug.Log("Enemy: collider: " + collider);

      switch (collider.gameObject.layer) {
        case 8: // we hit a wall, so turn around
          direction *= -1;
          transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
          break;
        case 13: // we hit a collectible, so we ignore it
          Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collider);
          break;

      }
  }

  private void RandomWalkCycle() {
    if (randomWalkToRight) rb.velocity = walkingSpeed * Vector2.right + Vector2.up * rb.velocity.y;
    else rb.velocity = walkingSpeed * Vector2.left + Vector2.up * rb.velocity.y;

  }

  private void RotateBasedOnDirection() {
    if (direction < 0)
      transform.localScale = new Vector3(-1, 1, 1);
    else
      transform.localScale = new Vector3(1, 1, 1);
  }

  // checks to see if it's close enough to player
  private void CheckForPlayerProximity() {
    float distance = Vector2.Distance(transform.position, player.transform.position);
    if (distance <= distanceNearPlayer) {
      lockOnPlayer = true;
    }
    // the player got out of range for the enemy to follow her
    if (distance >= 10f)
      lockOnPlayer = false;
  }

  // attacks player
  private void Attack() {
    spriteRenderer.color = Color.white;

    // attack the player
    if (player.state != Player.State.dashing && player.state != Player.State.slashing) {
      // the player is damaged
      player.attackResponse = Player.AttackResponse.missed;
    } else {
      // the enemy is damaged
      player.attackResponse = Player.AttackResponse.normal;
    }
  }

  private Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  }

  // follows player
  private void AutoPath() {
    if (state == State.damaged) return; 
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

  private float receiveSlashKnockback = 2f;
  private float receiveDashKnockback = 1f;
  /// <summary>
  /// Sent when another object enters a trigger collider attached to this
  /// object (2D physics only).
  /// </summary>
  /// <param name="other">The other Collider2D involved in this collision.</param>
  void OnTriggerEnter2D(Collider2D other)
  {
      if (state == State.damaged || state == State.dead) return;
      Debug.Log("Trigger " + other.name + " enter!");

      switch (other.name) {
        case "PlayerSlashHurtBox":
          Damage(receiveSlashDamage, receiveSlashKnockback, other);
          break;

        case "PlayerDashHurtBox":
          Damage(receiveDashDamage, receiveDashKnockback, other);
          break;
      }
  }

  
  // when enemy is first damaged
  private void Damage(float damageAmount, float knockback, Collider2D source) {
		if (state != State.damaged) {
			damagedStartTime = Time.time;
			state = State.damaged;
			rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
				transform.position.y - source.transform.position.y + 1.5f);

			healthAmount -= damageAmount;
			if ( healthAmount < 0) healthAmount = 0;

			if (healthAmount == 0) Death();
		}
	}

  // enemy died
  private void Death() {
    Debug.Log("enemy death!");
    // deletes the game object
    for (int i = 0; i < 4; i++)
      PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);

    Debug.Log("destroy me");
    Destroy(gameObject);
  }
}
