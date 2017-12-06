using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour {
  
  public Player player;
  public float maxSpeed;
  public float walkingSpeed;
  public float direction;
  public float distanceNearPlayer;

  private bool lockOnPlayer;
  private float lastTime;
  private System.Random random;
  private int[] directionOptions = new int[] {-1, 1};

  private Vector2 target;

  public enum State {
    idle,
    walking,
    attacking,
    damaged,
    dead,
  }

  public Rigidbody2D rb;
  public State state;

  void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
    lockOnPlayer = false;
    lastTime = Time.time;
    random = new System.Random();
    state = State.walking;
  }

  void Update() {
    rb.velocity = new Vector2(direction, rb.velocity.y);
    bool nearPlayer = NearPlayer();
    if (nearPlayer || lockOnPlayer) {
      // follow the player
    } else {
      // randomly walk around
      // RandomWalkCycle();
    }
      RandomWalkCycle();
  }

  // handles case when enemy runs into a wall
  void OnCollisionEnter2D(Collision2D collision) {
      Collider2D collider = collision.collider;
      if (collider.gameObject.layer == 8) {
        direction *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
      }
      Debug.Log("collider: " + collider);
  }

  private void RandomWalkCycle() {
    float currentTime = Time.time;
    if (currentTime - lastTime >= 3f) {
      lastTime = currentTime;
      if (rb.velocity.x != 0) {
        direction = 0;
        state = State.idle;
      } else {
        int directionScale = directionOptions[random.Next(directionOptions.Length)];
        direction = walkingSpeed * directionScale;
        transform.localScale = new Vector3(directionScale, 1, 1);
        state = State.walking;
      }
    }
  }

  // checks to see if it's close enough to player
  private bool NearPlayer() {
    if (Vector2.Distance(transform.position, player.transform.position) <= distanceNearPlayer) {
      lockOnPlayer = true;
      return true;
    }
    return false;
  }

  // follows player
  private void AutoPath() {

  }
}
