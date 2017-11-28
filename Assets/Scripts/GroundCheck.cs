using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private Player player;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        player.grounded = true;
        player.jumps = 2;   // reset the number of jumps
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        player.grounded = true;
        player.jumps = 2;
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        player.grounded = false;
    }
}
