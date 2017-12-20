using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockbox : MonoBehaviour {
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
                enemy.Damage(0, enemy.receiveSlashKnockback, other);
                break;

            case "PlayerDashHurtBox":
                enemy.Damage(0, enemy.receiveDashKnockback, other);
                break;
        }
    }
}