using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHand : Enemy
{

    public float deathDirection;
    public Vector3 offset;
    private Boss boss;
    public float speedX;
    private Vector2 target;
    private BoxCollider2D boxCollider2D;
    // Use this for initialization
    public override void Start()
    {

        // initialize instance variables
        audioSource = GetComponent<AudioSource>();
        hurtBox = transform.GetChild(0).GetComponents<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        boss = GetComponentInParent<BossLaserHands>().boss;

        player = FindObjectOfType<Player>();


        // load prefabs for pooled objects
        coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
        heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
        sparkPrefab = Resources.Load<GameObject>("Prefabs/FX/Spark");

        // set state variables
        lockOnPlayer = false;
        state = State.walking;
        prevNotice = false;
        died = false;

    }

    // after the entry, initializes the fist
    public void Ready()
    {
        Debug.Log("fist ready");
        spriteRenderer.sortingLayerName = "Foreground";
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        boxCollider2D.enabled = true;
    }

    public override void Update()
    {
        base.Update();
        if (state == State.idle) state = State.noticed;
        if (boss.state == Boss.State.entering) transform.position = boss.transform.position + offset;
    }

    protected override void RotateBasedOnDirection()
    {
        // prevents any rotation
    }

    /// <summary>
    /// Prevents any autopathing, which is handled by super object
    /// </summary>
    protected override void AutoPath()
    {
        // float xDist = player.transform.position.x - transform.position.x;
        // float yDist = player.transform.position.y - transform.position.y + 0.5f;

        // if (Mathf.Abs(xDist) < attackRange)
        // {
        //     state = State.attacking;
        //     StartCoroutine(Attack());
        //     return;
        // }

        // // if we need to move in the x or y direction, do so
        // if (Mathf.Abs(xDist) >= 0.1)
        //     rb.velocity = new Vector2(xDist * KP + 1.5f * Mathf.Sign(xDist), 0);
    }



    public override void UpdateAnimatorVariables()
    {
        // animator.SetFloat("speed", rb.velocity.magnitude);
        // animator.SetBool("damaged", state == State.damaged);
        // animator.SetBool("idle", state == State.idle);
        // animator.SetBool("walking", state == State.walking);
        // animator.SetBool("dead", state == State.dead);
        // animator.SetBool("noticed", lockOnPlayer);
        // animator.SetBool("grounded", grounded);
        // animator.SetBool("blocking", state == State.blocking);
        // animator.SetBool("attacking", state == State.attacking);
    }

    public bool charging;

    /// <summary>
    /// Placeholder for attacking, real attacking happens on command by boss laser hands' callback
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Attack()
    {
        yield return null;
    }

    /// <summary>
    /// Callback for starting the charge
    /// </summary>
    public void StartChargingLaser()
    {

    }

    /// <summary>
    /// Callback for starting the laser shot
    /// </summary>
    public void StartShootingLaser()
    {

    }

    /// <summary>
    /// Callback for stoping the shooting of the laser
    /// </summary>
    public void StopShooting()
    {

    }

    public override void Damage(float damageAmount, float knockback, Collider2D source)
    {
        // add a small amount of damage to make it turn red
        float trivialDamage = 0.1f;
        base.Damage(trivialDamage, knockback, source);

        healthAmount += trivialDamage;
        boss.healthAmount -= 5;
    }

    protected override void Idle()
    {
        rb.velocity = Vector2.zero;
    }

    protected override void Noticed()
    {
        animator.SetBool("noticed", state == State.noticed);
        if (Time.time - noticedStartTime > .7f)
            // give time for the animation to run
            state = State.walking;
    }
}
