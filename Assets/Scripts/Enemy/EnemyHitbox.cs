using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour {
    public Enemy enemy;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
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
                enemy.Damage(enemy.receiveSlashDamage, enemy.receiveSlashKnockback, other);
                break;

            case "PlayerDashHurtBox":
                enemy.Damage(enemy.receiveDashDamage, enemy.receiveDashKnockback, other);
                break;
            
        }

        switch (LayerMask.LayerToName(other.gameObject.layer)) {
            case "Spikes":
                if (enemy.state == Enemy.State.damaged) break;
                enemy.Damage(1f, 0, other);
                break;
        }
    }

    /// <summary>
    /// Sent each frame where a collider on another object is touching
    /// this object's collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D other = collision.collider;
        Debug.Log("Collision with " + other.name + " " + LayerMask.LayerToName(other.gameObject.layer));
        switch (LayerMask.LayerToName(other.gameObject.layer)) {
            case "Spikes":
                if (enemy.state == Enemy.State.damaged) break;
                enemy.Damage(1f, 0, other);
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        Debug.Log("Collision with " + other.name + " " + LayerMask.LayerToName(other.gameObject.layer));
    }
}
