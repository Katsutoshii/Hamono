using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashHurtBox : MonoBehaviour {

	private Player player;
	private BoxCollider2D hurtBox;

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<Player>();
		hurtBox = GetComponent<BoxCollider2D>();
		hurtBox.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (player.state == Player.State.dashing) {
			hurtBox.enabled = true;
		}
		else {
			hurtBox.enabled = false;
		}
	}
}
