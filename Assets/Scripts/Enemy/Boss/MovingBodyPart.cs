using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingBodyPart : MonoBehaviour {

  public float rotationalSpeed;
  public float radius = .2f;
  public bool clockwise;

  private float angle = 0;
  private float speed; // 2 * PI in degrees is 360, so you get 5 seconds to complete a circle

  public float originalX;
  public float originalY;

  public bool move;

  void Awake() {
    speed = (2 * Mathf.PI) / rotationalSpeed;
    originalX = transform.position.x;
    originalY = transform.position.y;
    move = true;
  }

  void Start() {}

  void Update() {
   if (move) Move();
  }

  private void Move() {
    if (clockwise) angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
    else angle -= speed * Time.deltaTime;
    transform.position = new Vector2(originalX + Mathf.Cos(angle) * radius, originalY + Mathf.Sin(angle) * radius);

  }

}
