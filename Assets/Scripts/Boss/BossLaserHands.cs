using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLaserHands : Enemy
{

    public float deathDirection;
    public Vector3 offset;
    public Boss boss;
    public float speedX;
    private Vector2 target;
    private BoxCollider2D boxCollider2D;
    private BossHand leftHand;
    private BossHand rightHand;
    private BossHandLaser handLaser;

    // Use this for initialization
    public override void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;

        // initialize instance variables
        audioSource = GetComponent<AudioSource>();
        hurtBox = transform.GetChild(0).GetComponents<BoxCollider2D>();
        player = FindObjectOfType<Player>();

        // set state variables
        lockOnPlayer = false;
        prevNotice = false;
        died = false;

        boss = GetComponentInParent<Boss>();
        leftHand = transform.Find("LeftHand").GetComponent<BossHand>();
        rightHand = transform.Find("RightHand").GetComponent<BossHand>();
        handLaser = transform.Find("HandLaser").GetComponent<BossHandLaser>();
        state = State.idle;
    }

    // after the entry, initializes the fist
    public void Ready()
    {
        Debug.Log("hands ready");
        spriteRenderer.sortingLayerName = "Foreground";
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        boxCollider2D.enabled = true;
    }

    public override void Update()
    {
        base.Update();
        if (state == State.idle) state = State.noticed;
        if (boss.state == Boss.State.entering) rb.position = boss.transform.position + offset;
    }

    protected override void RotateBasedOnDirection()
    {
        // prevents any rotation
    }

    /// <summary>
    /// Activates the hands for phase 2 of the boss fight
    /// </summary>
    public void Activate()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Move towards the player
    /// </summary>
    protected override void AutoPath()
    {
        Debug.Log("Pathing towards player!");
        float xDist = player.transform.position.x - transform.position.x;
        float yDist = player.transform.position.y - transform.position.y;

        // if close enough to attack, attack!
        if (Mathf.Abs(xDist) < attackRange && Mathf.Abs(yDist) < attackRange / 4 )
        {
            state = State.attacking;
            StartCoroutine(Attack());
            return;
        }

        // if we need to move in the x or y direction, do so
        float xComp = 0f, yComp = 0f;
        if (Mathf.Abs(xDist) >= 0.1)
            xComp = xDist * KP + 1.5f * Mathf.Sign(xDist);

        if (Mathf.Abs(yDist) >= 0.05)
            yComp = yDist * KP + 1.5f * Mathf.Sign(yDist);

        rb.velocity = new Vector2(xComp, yComp);
    }

    public override void UpdateAnimatorVariables()
    {
    }

    public float dropSpeed;
    public float risingSpeed;
    public bool charging;
    public bool interrupted;


    protected override IEnumerator Attack()
    {
        rb.velocity = Vector2.zero;

        Debug.Log("charging!");
        interrupted = false;
        leftHand.StartChargingLaser();
        rightHand.StartChargingLaser();
        yield return new WaitForSeconds(1f);

        // allows slashing the hands to stop the charge
        if (!interrupted)
        {
            Debug.Log("shooting");

            leftHand.StartShootingLaser();
            rightHand.StartShootingLaser();
            handLaser.Shoot();


            yield return new WaitForSeconds(0.5f);

            Debug.Log("shooting done");
            leftHand.StopShooting();
            rightHand.StopShooting();
            handLaser.StopShooting();
        }

        if (state != State.dead) state = State.idle;
    }

    /// <summary>
    /// Placeholder to prevent the superobject from taking damage -- only the child objects take damage
    /// </summary>
    /// <param name="damageAmount"></param>
    /// <param name="knockback"></param>
    /// <param name="source"></param>
    public override void Damage(float damageAmount, float knockback, Collider2D source)
    {
    }

    protected override void Idle()
    {
        // do nothing when idle
    }

    protected override void Noticed()
    {
        state = State.walking;
    }
}
