using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  
  public float maxSpeed;
  public float direction;

  public enum State {
    idle,
    walking,
    attacking,
    damaged,
    dead,
  }

  public Rigidbody2D rb;

  void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
  }

  void Update() {
    rb.velocity = new Vector2(direction, rb.velocity.y);
  }

  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      if (collider.gameObject.layer == 8) {
        direction *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
      }
      Debug.Log("other: " + collider);
  }
}
