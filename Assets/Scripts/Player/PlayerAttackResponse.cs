using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0219, CS0414 

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
        StartCoroutine(FreezeTime());
        staminaBar.IncreaseStamina(generateStamina * 40f);
    }

    private void Strong() {
        staminaBar.IncreaseStamina(generateStamina * 40f);
    }

    private void Blocked() {
        staminaBar.DecreaseStamina(missStaminaPenalty * 2);
    }

    public float missStaminaPenalty;
    private void Missed() {
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
