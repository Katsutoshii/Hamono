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
  protected GameObject sparkPrefab;
  protected GameObject healthBar;
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
  public bool stunned;


  // constants
	protected float SLASHING_X_DIST = 0.5f;
	protected float SLASHING_Y_DIST = 0.5f;
  public float KP;
  public float size;


  public virtual void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
    rb.isKinematic = false;

    audioSource = GetComponent<AudioSource>();
    hurtBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
    spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    spriteRenderer.color = Color.white;

    try {    
      healthBar = transform.Find("EnemyHealthBar").gameObject;
    }
    catch { }


    maxHealthAmount = healthAmount;
    
    player = FindObjectOfType<Player>();

    coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
    heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
    sparkPrefab = Resources.Load<GameObject>("Prefabs/FX/Spark");

    lockOnPlayer = false;
    state = State.walking;
    prevNotice = false;
    died = false;
  }

  public virtual void Update() {

    UpdateHealthBar();

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


  protected void RotateBasedOnDirection() {
    if (state != State.walking) return;
    if (Mathf.Abs(rb.velocity.x) > 0.05f) {
      if (rb.velocity.x < 0)
        transform.localScale = new Vector3(size, size, 1);
      else
        transform.localScale = new Vector3(-size, size, 1);
    }
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
    if (player.transform.position.x > transform.position.x)
				transform.localScale = new Vector3(-size, size, 1);
    else 
      transform.localScale = new Vector3(size, size, 1);

    
    hurtBox.enabled = true;
    rb.velocity = new Vector2(0, rb.velocity.y);
    state = State.attacking;
    yield return new WaitForSeconds(attackDuration);

    if (state == State.attacking) state = State.idle;
    yield return new WaitForSeconds(attackDuration);

    if (state == State.idle) state = State.walking;
    yield return null;
  }

  protected Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  }

  public float attackRange;
  private float autoPathStartTime;
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
  
  // when enemy is first damaged
  public virtual void Damage(float damageAmount, float knockback, Collider2D source) {
		if (state == State.damaged || state == State.dead) return;

    state = State.damaged;
		if (damageAmount > 0) {
      Debug.Log("Turning red!");
      spriteRenderer.color = Color.red;
    }
    else state = State.blocking;
    StartCoroutine(Damaged());

    // spawn sparks
    for (int i = 0; i < 4; i++)
        PoolManager.instance.ReuseObject(sparkPrefab, RandomOffset(transform.position), transform.rotation, sparkPrefab.transform.localScale);

    if (knockback != 0)
      rb.velocity = knockback * new Vector2(transform.position.x - source.transform.position.x, 
        transform.position.y - source.transform.position.y + 1.5f);
    else rb.velocity = Vector2.zero;

    healthAmount -= damageAmount;
    if ( healthAmount < 0) healthAmount = 0;
	}

  private float damageTime = 0.5f;
  private float stunTime = 0.25f;
  public virtual IEnumerator Damaged() {
    hurtBox.enabled = false;
    
    // damaged
    if (!healthBar.activeInHierarchy) healthBar.SetActive(true);
    gameObject.layer = LayerMask.NameToLayer("EnemiesDamaged");
    
    yield return new WaitForSeconds(damageTime);

    // check for death
    if (healthAmount <= 0) {
      state = State.dead;
      yield return null;
    }

    // stunned
    spriteRenderer.color = Color.white;
    hurtBox.enabled = true;
    gameObject.layer = LayerMask.NameToLayer("Enemies");

    rb.velocity = Vector2.zero;
    stunned = true;
    yield return new WaitForSeconds(stunTime);

    stunned = false;
    if (state != State.dead) state = State.walking;

    yield return null;
  }

  public virtual void UpdateHealthBar() {
    Image bar = healthBar.transform.Find("HealthBar/Bar").GetComponent<Image>();

    bar.fillAmount = healthAmount / maxHealthAmount;
    if (bar.fillAmount <= .4)
      bar.color = new Color(1, 0, 0, 1);
    else if (bar.fillAmount <= .7)
      bar.color = new Color(1, 0.39f, 0, 1);
    healthBar.transform.localScale = new Vector3(transform.localScale.x, 1, 1);;
  }


  public void StartDying() {
    stunned = true;
    healthBar.SetActive(false);
    
    state = State.dead;
    gameObject.layer = LayerMask.NameToLayer("Debris");
    rb.velocity = Vector2.zero;
    hurtBox.enabled = false;
  }
  public void Kill() {
    
    spriteRenderer.color = Color.white;

    // deletes the game object
    for (int i = 0; i < 4; i++)
      PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), transform.rotation, coinPrefab.transform.localScale);
    Destroy(gameObject);
  }

  protected virtual void HandleState() {
    switch (state) {
      case State.idle:
        Idle();
        break;

      case State.walking:
        Walk();
        break;
      
      case State.noticed:
        Noticed();
        break;
    }

    if (stunned) rb.velocity = new Vector2(0, rb.velocity.y);
  }

  protected virtual void Idle() {}

  // method to play sounds from animator
	public void PlayOneShot(AudioClip sound) {
		audioSource.PlayOneShot(sound);
	}
}
