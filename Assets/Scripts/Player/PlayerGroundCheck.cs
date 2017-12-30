using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    private Player player;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain") ||
                other.gameObject.layer == LayerMask.NameToLayer("LevelBoundaries") ||
                other.gameObject.layer == LayerMask.NameToLayer("Enemies") || 
                other.gameObject.layer == LayerMask.NameToLayer("Spikes"))
            player.grounded = true;
    }	

    void OnTriggerExit2D(Collider2D collider2D)
    {
        player.grounded = false;
    }
}
