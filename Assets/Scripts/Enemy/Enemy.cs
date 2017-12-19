using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// Base class for enemies
public class Enemy : MonoBehaviour {

  protected Player player;
  protected Rigidbody2D rb;
  protected SpriteRenderer spriteRenderer;
  protected GameObject coinPrefab;
  protected GameObject heartPrefab;
  protected GameObject healthBarPrefab;
  protected GameObject sparkPrefab;
  public State state;
  protected Animator animator;
  public AudioSource audioSource;
  protected BoxCollider2D hurtBox;

  public bool grounded;
  protected bool prevNotice;
  protected bool died;

  public float walkingSpeed;
  public float jumpingPower;
  public float distanceNearPlayer;

  protected bool lockOnPlayer;

  private float maxHealthAmount;
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
    blocking,
    dead,
  }

  // constants
	protected float SLASHING_X_DIST = 0.5f;
	protected float SLASHING_Y_DIST = 0.5f;
  public float KP;

  private float autoPathStartTime;

  public virtual void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
    rb.isKinematic = false;

    audioSource = GetComponent<AudioSource>();
    hurtBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    spriteRenderer.color = Color.white;

    maxHealthAmount = healthAmount;
    
    player = FindObjectOfType<Player>();

    coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
    heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
    sparkPrefab = Resources.Load<GameObject>("Prefabs/FX/Spark");

    GetHealthBar();

    lockOnPlayer = false;
    state = State.walking;
    prevNotice = false;
    died = false;
  }

  public virtual void Update() {

    StaticHealthBar();

    UpdateHealthBar();

    CheckForPlayerProximity();

    HandleState();

    UpdateAnimatorVariables();
  }

  public virtual void GetHealthBar() {
    healthBarPrefab = transform.GetChild(2).gameObject;
    healthBarPrefab.GetComponent<Canvas>().enabled = false;
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

  protected float damagedStartTime;
	public virtual void Damaged() {
		spriteRenderer.color = Color.red;

    if (healthAmount == 0) StartCoroutine(Death());	

    if (!healthBarPrefab.GetComponent<Canvas>().enabled) healthBarPrefab.GetComponent<Canvas>().enabled = true;
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
		
		if (Time.time - damagedStartTime > 0.5f) {
			spriteRenderer.color = Color.white;
      gameObject.layer = LayerMask.NameToLayer("Enemies");

			state = State.walking;
		}
	}

  public virtual void UpdateAnimatorVariables() {
    animator.SetFloat("speed", rb.velocity.magnitude);
    animator.SetBool("damaged", state == State.damaged);
    animator.SetBool("idle", state == State.idle);
    animator.SetBool("walking", state == State.walking);
    animator.SetBool("dead", state == State.dead);
    animator.SetBool("noticed", state == State.noticed);
    animator.SetBool("grounded", grounded);
    animator.SetBool("blocking", state == State.blocking);
    animator.SetBool("attacking", state == State.attacking);
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

  public virtual void StaticHealthBar() {
    transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
  }

  // checks to see if it's close enough to player
  public virtual void CheckForPlayerProximity() {
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
  public float attackDuration;
  protected virtual IEnumerator Attack() { 
    Debug.Log("Attakcing!");
    if (player.transform.position.x > transform.position.x)
				transform.localScale = new Vector3(-1, 1, 1);
    else 
      transform.localScale = new Vector3(1, 1, 1);


    rb.velocity = new Vector2(0, rb.velocity.y);
    state = State.attacking;
    yield return new WaitForSeconds(attackDuration);
    state = State.walking;

    yield return null;
  }

  protected Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  }

  public float attackRange;
  // follows player
  protected virtual void AutoPath() {
		float xDist = player.transform.position.x - transform.position.x;
		float yDist = player.transform.position.y - transform.position.y + 0.5f;

    if (Mathf.Abs(xDist) < attackRange) {
      StartCoroutine(Attack());
			return;
		}

    // if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= 0.1) 
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);

    RotateBasedOnDirection();
  }

  
  /// <summary>
  /// Sent when another object enters a trigger collider attached to this
  /// object (2D physics only).
  /// </summary>
  /// <param name="other">The other Collider2D involved in this collision.</param>
  public virtual void OnTriggerEnter2D(Collider2D other)
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
  public virtual void Damage(float damageAmount, float knockback, Collider2D source) {

		if (state == State.damaged || state == State.dead) return;

    damagedStartTime = Time.time;
    
    
		if (damageAmount > 0) {
      spriteRenderer.color = Color.red;
      state = State.damaged;
    }
    else state = State.blocking;

    // spawn sparks
    for (int i = 0; i < 4; i++)
        PoolManager.instance.ReuseObject(sparkPrefab, RandomOffset(transform.position), transform.rotation, sparkPrefab.transform.localScale);

    if (knockback != 0)
      rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
        transform.position.y - source.transform.position.y + 1.5f);

    healthAmount -= damageAmount;
    if ( healthAmount < 0) healthAmount = 0;
	}

  public virtual void UpdateHealthBar() {
    Image bar = transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
    bar.fillAmount = healthAmount / maxHealthAmount;
    //Debug.Log("cellphone: " + bar.fillAmount);
    if (bar.fillAmount <= .4)
      bar.color = new Color(1, 0, 0, 1);
    else if (bar.fillAmount <= .7)
      bar.color = new Color(1, 0.39f, 0, 1);
  }

  protected virtual IEnumerator Death() {
    rb.velocity = new Vector2(0, rb.velocity.y);
    Destroy(hurtBox);

		state = State.damaged;
    spriteRenderer.color = Color.red;
		yield return new WaitForSeconds(0.3f);

		state = State.dead;
    spriteRenderer.color = Color.white;
    yield return new WaitForSeconds(0.5f);

    // deletes the game object
    for (int i = 0; i < 4; i++)
      PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
    Destroy(gameObject);
    yield return null;
  }

  protected virtual void HandleState() {
    switch (state) {
      case State.damaged:
        Damaged();
        break;

      case State.blocking:
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
  // method to play sounds from animator
	public void PlayOneShot(AudioClip sound) {
		audioSource.PlayOneShot(sound);
	}
  
}
