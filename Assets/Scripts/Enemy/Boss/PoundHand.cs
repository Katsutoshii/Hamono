using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0219  

public class PoundHand : Enemy {

  private Vector2 originalScale;
  private MovingBodyPart moveBody;
  private bool reachedPeak;
  public GameObject slammingPrefab;

  protected void Awake() {
    originalScale = transform.localScale;
  }

  public override void Start() {
    base.Start();
    PoolManager.instance.CreatePool(slammingPrefab, 1);
    moveBody = gameObject.GetComponent<MovingBodyPart>();
    reachedPeak = false;
  }

  public override void Update() {

    this.UpdateHealthBar();
    base.Update();
    

    if (rb.velocity.y == 0 && transform.position.y <= moveBody.originalY) StartCoroutine(Landed());

    if (state == State.attacking && !reachedPeak) Slam();
    transform.localScale = originalScale;
  }

  public override void UpdateHealthBar() {}

  private IEnumerator Landed() {
    rb.velocity = Vector2.zero;
    yield return new WaitForSeconds(3f);
    moveBody.move = true;
    reachedPeak = false;
  }

  private void Slam() {
    moveBody.move = false;
    MoveUp();
  }

  private void MoveUp() {
    float heightLimit = moveBody.originalY + 5f;
    if (transform.position.y < heightLimit) {
      rb.velocity = Vector2.up * 3f;
    } else {
      reachedPeak = true;
      MoveDown();
    }
  }

  private void MoveDown() {
    rb.velocity = new Vector2(0, -10f);
  }
}
