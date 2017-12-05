using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResponse : MonoBehaviour
{
    private Player player;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        // Enemy - placeholder label for all enemy sprites
        // TODO: change to use layer
        if (collider2D.name == "Enemy") {
            if (player.state != Player.State.running && player.state != Player.State.idle) {
                // We are in attack mode
                // TODO: Calculate what type of enemy it is and distribute attack response
                if (player.attackResponse == Player.AttackResponse.combo) {
                  // Handles future responses with combo in mind
                } else {
                  player.attackResponse = Player.AttackResponse.normal;
                }
            }

            // Handles the different types of attack responses
            switch (player.attackResponse) {
                case Player.AttackResponse.normal:
                    break;
                case Player.AttackResponse.strong:
                    break;
                case Player.AttackResponse.blocked:
                    break;
                case Player.AttackResponse.missed:
                    break;
                case Player.AttackResponse.combo:
                    break;
                default:
                    break;
            }	
        }
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
      if (player.state == Player.State.idle || player.state == Player.State.running) {
        // The player got hit
        player.attackResponse = Player.AttackResponse.none;
        player.comboCount = 0;
      } else {
        // The player is working with combos
        player.attackResponse = Player.AttackResponse.combo;
        player.comboCount++;
        // Resets the number of jumps
      }
    }	

    void OnTriggerExit2D(Collider2D collider2D)
    {
        player.grounded = false;
    }
}
