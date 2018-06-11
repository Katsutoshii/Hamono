using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLaserHands : Enemy
{

    public float deathDirection;
    public Vector3 offset;
    private Boss boss;
    public float speedX;
    private Vector2 target;
    private BoxCollider2D boxCollider2D;
    private BossHand leftHand;
    private BossHand rightHand;
    private BossHandLaser handLaser;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer.sortingLayerName = "BackgroundDetails";
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
        float yDist = player.transform.position.y - transform.position.y + 0.5f;

        if (Mathf.Abs(xDist) < attackRange && Mathf.Abs(yDist) < attackRange )
        {
            state = State.attacking;
            StartCoroutine(Attack());
            return;
        }

        // if we need to move in the x or y direction, do so
        if (Mathf.Abs(xDist) >= 0.1)
            rb.velocity = new Vector2(xDist * KP + 1.5f * Mathf.Sign(xDist), 0);

        if (Mathf.Abs(yDist) >= 0.1)
            rb.velocity = new Vector2(yDist * KP + 1.5f * Mathf.Sign(yDist), 0);
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


            yield return new WaitForSeconds(1f);

            Debug.Log("shooting done");
            leftHand.StopShooting();
            rightHand.StopShooting();
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
        rb.velocity = Vector2.zero;
    }
}
