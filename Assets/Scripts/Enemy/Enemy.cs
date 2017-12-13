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

  public bool grounded;
  private bool prevNotice;
  private bool died;

  public float walkingSpeed;
  public float jumpingPower;
  public float distanceNearPlayer;

  private float direction;
  private bool lockOnPlayer;
  private float lastTime;
  private float[] directionOptions = new float[] {-1f, 1f};

  public float healthAmount;
  public float receiveSlashDamage;
  public float receiveDashDamage;
  public float receiveSlashKnockback;
  public float receiveDashKnockback;

  public enum State {
    idle,
    walking,
    noticed,
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
    spriteRenderer.color = Color.white;
    lockOnPlayer = false;
    lastTime = Time.time;
    state = State.walking;
    direction = walkingSpeed;
    prevNotice = false;
    died = false;
    StartCoroutine(ChangeRandomWalkCycle());

  }

  void Update() {
    CheckForPlayerProximity();
    UpdateAnimatorVariables();

    switch (state) {
      case State.attacking:
        Attack();
        break;

      case State.damaged:
        Damaged();
        break;

      case State.walking:
        Walk();
        break;
      
      case State.noticed:
        Noticed();
        break;
      
      case State.dead:
        Death();
        break;
    }
  }

  bool randomWalkToRight;
  public float randomChangetime;
  private IEnumerator ChangeRandomWalkCycle() {
    while( true) {
      randomWalkToRight = Random.Range(0, 1f) >= 0.5f;
      yield return new WaitForSeconds(randomChangetime);
    }
  }

  private float noticedStartTime;
  private void Walk() {
    RotateBasedOnDirection();
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
      RandomWalkCycle();
    }
  }

  // enemy notices player
  private void Noticed() {
    rb.velocity = new Vector2(0, rb.velocity.y);
    animator.SetBool("noticed", state == State.noticed);
    if (Time.time - noticedStartTime > .7f)
      // give time for the animation to run
      state = State.walking;
  }

  private float damagedStartTime;
	private void Damaged() {
		spriteRenderer.color = Color.red;
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		
		if (Time.time - damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Enemies");

			state = State.walking;
		}
	}

  private void UpdateAnimatorVariables() {
    animator.SetFloat("speed", rb.velocity.magnitude);
    animator.SetBool("damaged", state == State.damaged);
    animator.SetBool("idle", state == State.idle);
    animator.SetBool("walking", state == State.walking);
    animator.SetBool("dead", state == State.dead);
    animator.SetBool("noticed", state == State.noticed);
    animator.SetBool("grounded", grounded);
  }

  // handles case when enemy runs into something
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;

      switch (collider.gameObject.layer) {
        case 8: // we hit a wall, so turn around
          direction *= -1;
          transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
          break;

        case 15: // we hit a boundary, so turn around
          rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
          break;
      }

      switch (collision.collider.name) {
        case "Spikes":
          if( state == State.damaged) break;
          Damage(1f, 0, collision.collider);
          rb.velocity += 3 * Vector2.up;
          break;
      }
  }


  private void RandomWalkCycle() {
    if (grounded) {
      if (randomWalkToRight) {
        rb.velocity = walkingSpeed * Vector2.right;
        if (rb.velocity.x > 0) StartCoroutine(Jump(jumpingPower));
      } else {
        rb.velocity = walkingSpeed * Vector2.left;
        if (rb.velocity.x < 0) StartCoroutine(Jump(jumpingPower));
      }
    }
  }

  private void RotateBasedOnDirection() {
    if (state != State.walking) return;
    if (Mathf.Abs(rb.velocity.x) > 0.05f) {
      if (rb.velocity.x < 0)
        transform.localScale = new Vector3(1, 1, 1);
      else
        transform.localScale = new Vector3(-1, 1, 1);
    }
  }

  // checks to see if it's close enough to player
  private void CheckForPlayerProximity() {
    float distance = Vector2.Distance(transform.position, player.transform.position);
    if (distance <= distanceNearPlayer) {
      lockOnPlayer = true;
    }
    // the player got out of range for the enemy to follow her
    if (distance >= 10f) {
      lockOnPlayer = false;
      prevNotice = false;
    }
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
      state = State.damaged;
    }
  }

  private Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  }

  // follows player
  private void AutoPath() {
		rb.gravityScale = GRAVITY_SCALE;
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

    if (Mathf.Abs(xDist) < 0.1 && Mathf.Abs(yDist) < 0.1) {
			return;
		}

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= 0.1) {
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
    }

    // adds jumping
    if (grounded) {
      if (randomWalkToRight) {
          if (rb.velocity.x > 0) StartCoroutine(Jump(jumpingPower));
        } else {
          if (rb.velocity.x < 0) StartCoroutine(Jump(jumpingPower));
        }
    }
  }

  
  /// <summary>
  /// Sent when another object enters a trigger collider attached to this
  /// object (2D physics only).
  /// </summary>
  /// <param name="other">The other Collider2D involved in this collision.</param>
  void OnTriggerEnter2D(Collider2D other)
  {
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
  private float deathStartTime;
  private void Damage(float damageAmount, float knockback, Collider2D source) {
		if (state == State.damaged || state == State.dead) return;
    damagedStartTime = Time.time;
    state = State.damaged;

    if (knockback != 0)
      rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
        transform.position.y - source.transform.position.y + 1.5f);

    healthAmount -= damageAmount;
    if ( healthAmount < 0) healthAmount = 0;

    if (healthAmount == 0) {
      if (state != State.dead) {
        deathStartTime = Time.time;
        // destroys the hurtbox
        state = State.dead;
        spriteRenderer.color = Color.red;
        Destroy(gameObject.transform.GetChild(0).GetComponent<Collider2D>());
      }   
    }
	}

	private const float JUMP_DELAY = 0.1f;
	private bool jumping = false;
	private IEnumerator Jump(float jumpPower) {
		jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(JUMP_DELAY);
		
		jumping = false;
		yield return null;
	}
  // enemy died
  private void Death() {
    rb.velocity = new Vector2(0, rb.velocity.y);
    if (Time.time - deathStartTime > .3f) spriteRenderer.color = Color.white;
    // deletes the game object
    if (Time.time - deathStartTime > .8f) {
      for (int i = 0; i < 4; i++)
        PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
      Destroy(gameObject);
    }
  }
}
