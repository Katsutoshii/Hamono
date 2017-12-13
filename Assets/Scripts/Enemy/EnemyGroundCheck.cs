using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{
    private Enemy enemy;

    // Use this for initialization
    void Start()
    {
        enemy = gameObject.GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            enemy.grounded = true;
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        enemy.grounded = false;
    }
}
