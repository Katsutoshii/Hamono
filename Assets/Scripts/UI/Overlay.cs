using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour {

  public bool toggleOverlay;
  public float toggleFrequency;

  private Image backdrop;
  private Image overlayImage;

  private Player player;

  void Start() {
    backdrop = GameObject.Find("Backdrop").GetComponent<Image>();
    overlayImage = GameObject.Find("OverlayImage").GetComponent<Image>();
    player = GameObject.Find("Player").GetComponent<Player>();
    player.state = Player.State.talking;
    Debug.Log("player.state: " + player.state);
    if (toggleOverlay) StartCoroutine(ToggleImage());
  }

  private IEnumerator ToggleImage() {
    int toggleCount = 0;
    while (toggleCount < 7) {
      yield return new WaitForSeconds(toggleFrequency);
      overlayImage.color = new Color(1, 1, 1, overlayImage.color.a * -1);
      toggleCount++;
    }
    StartCoroutine(FadeOut());
  }

  private IEnumerator FadeOut() {
    while (backdrop.color.a > 0) {
      yield return new WaitForSeconds(.2f);
      backdrop.color = new Color(backdrop.color.r, backdrop.color.g, backdrop.color.b, backdrop.color.a - .1f);
    }
    gameObject.SetActive(false);
    player.state = Player.State.idle;
  }
}
