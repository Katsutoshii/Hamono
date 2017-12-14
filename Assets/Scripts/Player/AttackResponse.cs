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
        // player.attackResponse = Player.AttackResponse.none;
    }

    private void Strong() {

    }

    private void Blocked() {

    }

    private void Missed() {
        Debug.Log("the player missed");
        // player.attackResponse = Player.AttackResponse.none;
    }

    private void Combo() {

    }
}
