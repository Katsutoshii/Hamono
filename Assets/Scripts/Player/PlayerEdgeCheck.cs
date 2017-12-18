using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEdgeCheck : MonoBehaviour
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
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("On trigger enter for edge check: with " + other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            player.onEdge = false;
    }

    /// <summary>
    /// Sent when another object leaves a trigger collider attached to
    /// this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("On trigger exit for edge check: with " + other.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Terrain") && player.grounded)
            player.onEdge = true;
    }
}
