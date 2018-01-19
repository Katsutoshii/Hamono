using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Eyes : MonoBehaviour {

  private Rigidbody2D rb;
  private Player player;

  private float radius = .2f;

  private float angle = 0;
  private float speed; // 2 * PI in degrees is 360, so you get 5 seconds to complete a circle

  private Vector3 originalPosition;

  void Awake() {
    speed = (2 * Mathf.PI) / 2f;
  }

  void Start() {
    player = GameObject.Find("Player").GetComponent<Player>();
    rb = gameObject.GetComponent<Rigidbody2D>();

    originalPosition = transform.position;
  }

  void Update() {
    AutoPath();
  }

  void AutoPath() {
    Vector3 lookDir = (player.transform.position - originalPosition).normalized;
    // transform.position = originalPosition + (lookDir * radius);
    // transform.position = new Vector2((originalPosition + (lookDir * radius)).x, originalPosition.y);
    // float angle = Vector2.Angle(player.transform.position, transform.position);
    rb.velocity = transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity + new Vector2(Mathf.Cos((originalPosition + lookDir).x) * radius, Mathf.Sin(angle) * radius);
  }

}
