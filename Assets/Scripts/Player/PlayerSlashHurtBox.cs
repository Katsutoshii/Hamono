﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlashHurtBox : MonoBehaviour {
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
		if (player.state == Player.State.slashing) {
			hurtBox.enabled = true;
			switch (player.attackType) {
				case Player.AttackType.straightSlash:
					hurtBox.offset = new Vector2(0.3f, 0);
					break;
				case Player.AttackType.upSlash:
					hurtBox.offset = new Vector2(0.3f, 0.2f);
					break;
				case Player.AttackType.downSlash:
					hurtBox.offset = new Vector2(0.3f, -0.2f);
					break;	
			}
			
		}
		else {
			hurtBox.offset = new Vector2(0, 0);
			hurtBox.enabled = false;
			
		}
	}
}
