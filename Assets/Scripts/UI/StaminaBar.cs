using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0414  

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    public float fillAmount;

    public Image bar;
    private Player player;
    private bool exhausted;

    // Use this for initialization
    void Start()
    {
      player = GameObject.Find("Player").GetComponent<Player>();
    }

    // increases the player's stamina
    public void IncreaseStamina(float amount) {
      if (this.bar.fillAmount < 1) 
        this.bar.fillAmount += amount;
      this.exhausted = false;
    }

    // decreases the player's stamina
    public void DecreaseStamina(float amount) {
      if (this.bar.fillAmount > 0)
        this.bar.fillAmount -= amount;
      else
        this.exhausted = true;
    }

    // checks to see if character is exhausted
    public bool isExhausted() {
      return this.exhausted;
    }

    // visual representation of stamina
    private void HandleColor() {
      if (this.bar.fillAmount <= .3) {
        // turn red
        Debug.Log("turning red");
        this.bar.color = new Color(1, 0, 0, 1);
      } else if (this.bar.fillAmount <= .7) {
        // turn orange
        Debug.Log("turning orange");
        this.bar.color = new Color(1, 0.39f, 0, 1);
      } else {
        // turn green
        Debug.Log("turning green");
        this.bar.color = new Color(1, 1, 1, 1);
      }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        fillAmount = bar.fillAmount;
        HandleColor();
    }
}
