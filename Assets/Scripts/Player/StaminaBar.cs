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

    // Use this for initialization
    void Start()
    {
      bar = GameObject.Find("Bar").GetComponent<Image>();
      player = GameObject.Find("Player").GetComponent<Player>();
    }

    // increases the player's stamina
    public void increaseStamina(float amount) {
      this.bar.fillAmount += amount;
    }

    // decreases the player's stamina
    public void decreaseStamina(float amount) {
      this.bar.fillAmount -= amount;
    }

}
