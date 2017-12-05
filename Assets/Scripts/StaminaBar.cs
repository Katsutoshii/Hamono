using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    public float fillAmount;

    private Image bar;

    // Use this for initialization
    void Start()
    {
      bar = GameObject.Find("Bar").GetComponent<Image>();
    }

    void Update() {
      HandleStamina();
    }

    private void HandleStamina() {
      bar.fillAmount = fillAmount;
    }
}
