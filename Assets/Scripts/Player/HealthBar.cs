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

  public Sprite noHeart;

  private float maxHearts = 3f;
  private float heartSections = 4f;

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
      float roundingFigure = heartSections * (healthAmount - 2);
      this.thirdHeart.fillAmount = Mathf.Round(roundingFigure) / heartSections;
    } else if (healthAmount >= 1) {
      // change third heart to be empty - black dot
      float roundingFigure = heartSections * (healthAmount - 1);
      this.thirdHeart.sprite = noHeart;
      this.secondHeart.fillAmount = Mathf.Round(roundingFigure) / heartSections;
    } else if (healthAmount > 0) {
      // change the third and second hearts to be empty - black dot
      float roundingFigure = heartSections * healthAmount;
      this.secondHeart.sprite = noHeart;
      this.thirdHeart.sprite = noHeart;
      this.firstHeart.fillAmount = Mathf.Round(roundingFigure) / heartSections;
    } else {
      // player is dead
      this.exhausted = true;
    }
  }
}
