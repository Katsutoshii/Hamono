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

  void OnTriggerEnter2D(Collider2D other)
  {
      if (other.name == "Obstacle") {
        direction *= -1;
      }

  }
  
}
