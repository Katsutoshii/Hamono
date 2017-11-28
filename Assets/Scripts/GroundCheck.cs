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

    void OnTriggerExit2D(Collider2D collider2D)
    {
        player.grounded = false;
    }
}
