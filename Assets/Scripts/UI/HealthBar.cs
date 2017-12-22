using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

  [SerializeField]
  public float fillAmount;

  private Image[] hearts;

  public Sprite noHeart;

  public int numHearts;

  // Use this for initialization
  void Start() {
    hearts = GetComponentsInChildren<Image>();
    numHearts = hearts.Length;
  }

  public void HandleHealth(float healthAmount) {
    for (int i = 0; i < (int) (healthAmount - healthAmount % 1f); i++) {
      hearts[i].fillAmount = 1f;
    }
    if ((int)healthAmount == numHearts) {
      return;
    }

    // if we have a fractional amount of health left, set the fill amount
    if (healthAmount % 1f != 0) hearts[(int) healthAmount].fillAmount = healthAmount % 1f;
    else {
      hearts[(int) healthAmount].sprite = noHeart;
      hearts[(int) healthAmount].fillAmount = 1;
    }


    // set the rest of the iamges to no heart
    for (int i = (int) healthAmount + 1; i < numHearts; i++) {
      hearts[i].sprite = noHeart;
      hearts[i].fillAmount = 1;
    }
  }
}
