using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

  [SerializeField]
  public float fillAmount;

  private Player player;
  private bool exhausted;

  private Image firstHeart;
  private Image secondHeart;
  private Image thirdHeart;

  private float maxHearts = 3f;

  // Use this for initialization
  void Start() {
    player = GameObject.Find("Player").GetComponent<Player>();
    firstHeart = GameObject.Find("First Heart").GetComponent<Image>();
    secondHeart = GameObject.Find("Second Heart").GetComponent<Image>();
    thirdHeart = GameObject.Find("Third Heart").GetComponent<Image>();
  }

  void Update() {
    HandleHealth();
  }

  private void HandleHealth() {
    float healthAmount = player.healthAmount / maxHearts;
    this.exhausted = false;
    if (healthAmount >= 3) {
      // handle all heart to be full
    } else if (healthAmount >= 2) {
      this.thirdHeart.fillAmount = healthAmount - 2;
    } else if (healthAmount >= 1) {
      // change third heart to be empty - black dot
      this.secondHeart.fillAmount = healthAmount - 1;
    } else if (healthAmount > 0) {
      // change the third and second hearts to be empty - black dot
      this.firstHeart.fillAmount = healthAmount;
    } else {
      // player is dead
      this.exhausted = true;
    }
  }
}
