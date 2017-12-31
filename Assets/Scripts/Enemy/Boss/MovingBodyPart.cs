using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0219  

/// Base class for boss body parts
public class MovingBodyPart : MonoBehaviour {

  public float rotationalSpeed;
  public bool clockwise;

  private float angle = 0;
  private float speed; // 2 * PI in degrees is 360, so you get 5 seconds to complete a circle
  float radius = .2f;

  void Awake() {
    speed = (2 * Mathf.PI) / rotationalSpeed;
  }

  void Start() {}

  void Update() {
    Move();
  }

  private void Move() {
    if (clockwise) angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
    else angle -= speed * Time.deltaTime;
    transform.position = new Vector2(Mathf.Cos(angle) * radius, 2.7f - Mathf.Sin(angle) * radius);
  }

}
