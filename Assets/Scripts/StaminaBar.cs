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

    void Update() {
      HandleStamina();
    }

    private void HandleStamina() {
      if (player.state == Player.State.autoPathing && fillAmount < 1)
        fillAmount += 0.005f;
      else if (player.state == Player.State.dashing && fillAmount > 0)
        fillAmount -= 0.02f;
      else
        bar.fillAmount = fillAmount;
    }
}
