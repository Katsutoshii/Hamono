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

    void Update() {
        HandleAttacks();
    }

    private void HandleAttacks() {
        switch (player.attackResponse) {
            case Player.AttackResponse.normal:
                break;
            case Player.AttackResponse.strong:
                break;
            case Player.AttackResponse.blocked:
                break;
            case Player.AttackResponse.missed:
                player.spriteRenderer.color = Color.red;
                Damaged();
                break;
            case Player.AttackResponse.combo:
                break;
            default:
                player.spriteRenderer.color = Color.white;
                break;
        }
    }

    private void Damaged() {
        Debug.Log("Player damaged: " + player.healthAmount);
        player.attackResponse = Player.AttackResponse.none;
        if (player.healthAmount <= 0) {
            // player died
            Death();
            return;
        }
        player.healthAmount -= .2f;
    }

    private void Death() {
        // notify the user that the player died
        // trigger death animation/sequence
        player.attackResponse = Player.AttackResponse.none;
        Debug.Log("Player died");
        // Destroy(player.gameObject);
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerStay2D(Collider2D other)
    {
      if (player.state == Player.State.idle) {
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
