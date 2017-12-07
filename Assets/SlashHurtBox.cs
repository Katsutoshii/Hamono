using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashHurtBox : MonoBehaviour {
	private Player player;
	private BoxCollider2D hurtBox;

	// Use this for initialization
	void Start () {
		player = GetComponentInParent<Player>();
		hurtBox = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player.state == Player.State.slashing) {
			hurtBox.enabled = true;
			
			hurtBox.offset = new Vector2(player.transform.localScale.x * 0.3f, 0);
		}
		else {
			hurtBox.offset = new Vector2(0, 0);
			hurtBox.enabled = false;
			
		}
	}
}
