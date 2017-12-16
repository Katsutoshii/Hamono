using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private float startComboTime;
    private float lastComboTime;
    public float timeFreezeDuration;


    // handles everything after a response is given for an attack
    private void HandleAttackResponses() {
        switch (attackResponse) {
            case AttackResponse.normal:
                Normal();
                break;

            case AttackResponse.strong:
                Strong();
                break;

            case AttackResponse.blocked:
                Blocked();
                break;

            case AttackResponse.missed:
                Missed();
                break;

            case AttackResponse.combo:
                Combo();
                break;

            default:
                break;
        }

        // clear the attack after processing it
        attackResponse = AttackResponse.none;
    }

    private void Normal() {
        Debug.Log("this is a normal response to attacking");
        StartCoroutine(FreezeTime());
        staminaBar.IncreaseStamina(generateStamina * 10f);
    }

    private void Strong() {
        staminaBar.IncreaseStamina(generateStamina * 20f);
    }

    private void Blocked() {

    }

    public float missStaminaPenalty;
    private void Missed() {
        Debug.Log("the player missed");
        staminaBar.DecreaseStamina(missStaminaPenalty);
    }

    private void Combo() {
        if (comboCount > 0) staminaBar.IncreaseStamina(generateStamina * 20f * comboCount);
    }

    private bool timeFrozen;
    IEnumerator FreezeTime(){
        timeFrozen = true;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeFreezeDuration);
        timeFrozen = false;
        Time.timeScale = 1;
    }
}
