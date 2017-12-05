using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    public float fillAmount;

    private Image bar;
    private Player player;
    private bool exhausted;

    // Use this for initialization
    void Start()
    {
      bar = GameObject.Find("Bar").GetComponent<Image>();
      player = GameObject.Find("Player").GetComponent<Player>();
    }

    // increases the player's stamina
    public void increaseStamina(float amount) {
      if (this.bar.fillAmount < 1)
        this.bar.fillAmount += amount;
      exhausted = false;
    }

    // decreases the player's stamina
    public void decreaseStamina(float amount) {
      if (this.bar.fillAmount > 0)
        this.bar.fillAmount -= amount;
      else
        exhausted = true;
    }

    // checks to see if stamina is exhausted
    public bool isExhausted() {
      return exhausted;
    }

}