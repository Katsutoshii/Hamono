using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  
  public Player player;
  public float maxSpeed;
  public float direction;

  private float nearPlayer;
  private float lockOnPlayer;

  public enum State {
    idle,
    walking,
    attacking,
    damaged,
    dead,
  }

  public Rigidbody2D rb;

  void Start() {
    player = gameObject.GetComponentInParent<Player>();
    rb = gameObject.GetComponent<Rigidbody2D>();
    nearPlayer = false;
    lockOnPlayer = false;
  }

  void Update() {
    rb.velocity = new Vector2(direction, rb.velocity.y);
    if (NearPlayer()) {
      nearPlayer = true;
      lockOnPlayer = true;
    } else {
      nearPlayer = false;
    }
  }


  // handles case when enemy runs into a wall
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      if (collider.gameObject.layer == 8) {
        direction *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
      }
      Debug.Log("other: " + collider);
  }

  // checks to see if it's close enough to player
  private bool NearPlayer(float distance = 2f) {
    if (Vector2.Distance(transform.position, player.transform.position) <= distance)
      return true;
    return false;
  }
}
