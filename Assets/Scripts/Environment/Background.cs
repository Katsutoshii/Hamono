using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {

  void Update() {
    ResizeSpriteToScreen();
  }

  void ResizeSpriteToScreen() {
    SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
    if (sr == null) return;

    transform.localScale = new Vector3(1, 1, 1);

    float width = sr.sprite.bounds.size.x - .3f;
    float height = sr.sprite.bounds.size.y - .5f;

    float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
    float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

    transform.localScale = new Vector2(worldScreenWidth / width, worldScreenHeight / height);
  }
}
