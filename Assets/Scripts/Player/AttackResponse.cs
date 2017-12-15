using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResponse : MonoBehaviour
{
    private Player player;
    private float startComboTime;
    private float lastComboTime;
    public CameraController camera;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    void Update() {
        HandleAttacks();
    }

    // handles everything after a response is given for an attack
    private void HandleAttacks() {
        switch (player.attackResponse) {
            case Player.AttackResponse.normal:
                Normal();
                break;

            case Player.AttackResponse.strong:
                Strong();
                break;

            case Player.AttackResponse.blocked:
                Blocked();
                break;

            case Player.AttackResponse.missed:
                Missed();
                break;

            case Player.AttackResponse.combo:
                Combo();
                break;

            default:
                break;
        }
    }

    private void Normal() {
        Debug.Log("this is a normal response to attacking");
        player.stamina.IncreaseStamina(player.generateStamina * 10f);
    }

    private void Strong() {
        player.stamina.IncreaseStamina(player.generateStamina * 20f);
    }

    private void Blocked() {

    }

    private void Missed() {
        Debug.Log("the player missed");
        player.stamina.DecreaseStamina(player.dashStaminaCost * 6f);
    }

    private void Combo() {
        if (player.comboCount > 0) player.stamina.IncreaseStamina(player.generateStamina * 20f * player.comboCount);
    }
}
