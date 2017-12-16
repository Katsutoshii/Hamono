using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for enemies
public class Enemy : MonoBehaviour {

  protected Player player;
  protected Rigidbody2D rb;
  protected SpriteRenderer spriteRenderer;
  protected GameObject coinPrefab;
  protected GameObject heartPrefab;
  public State state;
  protected Animator animator;
  public AudioSource audioSource;

  public bool grounded;
  protected bool prevNotice;
  protected bool died;

  public float walkingSpeed;
  public float jumpingPower;
  public float distanceNearPlayer;

  protected bool lockOnPlayer;

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
	protected float SLASHING_X_DIST = 0.5f;
	protected float SLASHING_Y_DIST = 0.5f;
  public float KP;

  private float autoPathStartTime;

  public virtual void Start() {
    Debug.Log("Enemy start!");
    rb = gameObject.GetComponent<Rigidbody2D>();
    rb.isKinematic = false;

    audioSource = GetComponent<AudioSource>();
    animator = GetComponent<Animator>();
    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    spriteRenderer.color = Color.white;

    coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
    heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");

    player = FindObjectOfType<Player>();

    lockOnPlayer = false;
    state = State.walking;
    prevNotice = false;
    died = false;
  }

  void Update() {

    StaticHealthBar();

    CheckForPlayerProximity();

    HandleState();

    UpdateAnimatorVariables();
  }

  protected float noticedStartTime;
  protected virtual void Walk() {}

  // enemy notices player
  protected void Noticed() {
    rb.velocity = new Vector2(0, rb.velocity.y);
    animator.SetBool("noticed", state == State.noticed);
    if (Time.time - noticedStartTime > .7f)
      // give time for the animation to run
      state = State.walking;
  }

  private float damagedStartTime;
	protected void Damaged() {
		spriteRenderer.color = Color.red;
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		
		if (Time.time - damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Enemies");

			state = State.walking;
		}
	}

  protected virtual void UpdateAnimatorVariables() {
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
          rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
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

  protected void RotateBasedOnDirection() {
    if (state != State.walking) return;
    if (Mathf.Abs(rb.velocity.x) > 0.05f) {
      if (rb.velocity.x < 0)
        transform.localScale = new Vector3(1, 1, 1);
      else
        transform.localScale = new Vector3(-1, 1, 1);
    }
  }

  protected void StaticHealthBar() {
    transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
  }

  // checks to see if it's close enough to player
  protected void CheckForPlayerProximity() {
    float distance = Vector2.Distance(transform.position, player.transform.position);
    if (distance <= distanceNearPlayer) {
      Debug.Log("Enemy Near player!");
      lockOnPlayer = true;
    }
    // the player got out of range for the enemy to follow her
    if (distance >= 10f) {
      lockOnPlayer = false;
      prevNotice = false;
    }
  }

  // attacks player
  protected virtual void Attack() {
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

  protected Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  }

  // follows player
  protected virtual void AutoPath() {
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

    if (Mathf.Abs(xDist) < 0.1 && Mathf.Abs(yDist) < 0.1) {
			return;
		}

    // if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= 0.1) 
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
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
  protected float deathStartTime;
  protected virtual void Damage(float damageAmount, float knockback, Collider2D source) {
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

  // enemy died
  protected virtual void Death() {
    rb.velocity = new Vector2(0, rb.velocity.y);
    if (Time.time - deathStartTime > .3f) spriteRenderer.color = Color.white;
    // deletes the game object
    if (Time.time - deathStartTime > .8f) {
      for (int i = 0; i < 4; i++)
        PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
      Destroy(gameObject);
    }
  }

  protected virtual void HandleState() {
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
}
