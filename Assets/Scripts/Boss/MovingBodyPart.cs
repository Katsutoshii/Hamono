using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBodyPart : MonoBehaviour {

  private Rigidbody2D rb;

  public float rotationalSpeed;
  public float radius = .2f;
  public bool clockwise;

  private float angle = 0;
  private float speed; // 2 * PI in degrees is 360, so you get 5 seconds to complete a circle

  public bool move;

  void Awake() {
    speed = (2 * Mathf.PI) / rotationalSpeed;
    move = true;
  }

  void Start() {
    rb = gameObject.GetComponent<Rigidbody2D>();
  }

  void Update() {
   if (move) Move();
  }

  private void Move() {
    if (clockwise) angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
    else angle -= speed * Time.deltaTime;
    rb.velocity = transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
  }

}
