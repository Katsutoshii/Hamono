using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

  [SerializeField]
  public float fillAmount;

  private Image[] hearts;
  private Image firstHeart;
  private Image secondHeart;
  private Image thirdHeart;

  public Sprite noHeart;

  public int numHearts;

  // Use this for initialization
  void Start() {
    hearts = GetComponentsInChildren<Image>();
    numHearts = hearts.Length;

    for (int i = 0; i < hearts.Length; i++)
      Debug.Log("heart " + i + " = " + hearts[i]);;
  }

  public void HandleHealth(float healthAmount) {
    Debug.Log("mod check: " + (healthAmount - healthAmount % 1f));
    for (int i = 0; i < (int) (healthAmount - healthAmount % 1f); i++) {
      Debug.Log("i = " + i);
      hearts[i].fillAmount = 1f;
    }
    if ((int)healthAmount == numHearts) {
      Debug.Log("Full health!");
      return;
    }

    // if we have a fractional amount of health left, set the fill amount
    Debug.Log("Setting heart #" + (int) healthAmount + " to fill amount " + healthAmount %1f);
    hearts[(int) healthAmount].fillAmount = healthAmount % 1f;

    // set the rest of the iamges to no heart
    for (int i = (int) healthAmount + 1; i < numHearts; i++) {
      hearts[i].sprite = noHeart;
    }
  }
}
