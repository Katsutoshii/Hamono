using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Player : MonoBehaviour {

	public float dashStaminaCost;
	// public constants
	public float slashStaminaCost;

	// effect prefabs
	public GameObject afterimagePrefab;
	public GameObject swordAfterimagePrefab;
	
	// private constants
	private const float ATTACK_TIMEOUT = 0.5f;

	private const float DASH_SPEED = 8f;
	private const float DASH_TARGET_THRESHOLD = 0.8f;
	private const float READY_FLOAT_TIMEOUT = 0.5f;
	
	private float readyStartTime;

	// method for when autopathing is complete and ready to make an attack
	public void Ready() {
		rb.gravityScale = 0; // float
		
		rb.velocity = Vector2.zero;

		// rotate based on slash direction
		if (targetA.x > targetB.x) transform.localScale = new Vector3(-1, 1, 1);
		else if (targetA.x < targetB.x) transform.localScale = new Vector3(1, 1, 1);

		if (attackType != AttackType.none) Attack();	// attack if we have an attack queued
		else attackResponse = AttackResponse.none;

		if ((attackType == AttackType.none && 
			state != State.dashing && 
			state != State.slashing &&
			(Time.time - readyStartTime > READY_FLOAT_TIMEOUT && !grounded) // time out if floating
			)) {
			
			ResetToIdle();
		}
	}

	// method to handle dashing
	// this is only called when auto pathing is completed!
	public void Dash() {
		rb.gravityScale = 0;
		gameObject.layer = LayerMask.NameToLayer("Dashing");

		if (staminaBar.isExhausted() || Time.time > attackStartTime + ATTACK_TIMEOUT) {
			ResetToIdle();
			
			rb.velocity = Vector2.zero;
			return;
		}
		
		float distanceB = Vector2.Distance(rb.position, targetB);
		staminaBar.DecreaseStamina(dashStaminaCost * distanceB * distanceB);
		
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.velocity = (targetB - rb.position) * DASH_SPEED;
			RotateSpriteForVelocity();
			SpawnSwordAfterimage();
			SpawnAfterimage();
		} 

		// otherwise we have completed the dash
		else {
			ResetToIdle();
			rb.velocity = Vector2.zero;
		}
	}

	private int afterimageCount = 0;
	public int numAfterimage;
	public Color afterimageColor;

	// method to create the after images for the dash
	private void SpawnAfterimage() {
		
		afterimageCount++;
		if(afterimageCount % 3 != 0) return;

		PoolManager.instance.ReuseObject (afterimagePrefab, transform.position, transform.eulerAngles, transform.localScale);
    }

	private void SpawnSwordAfterimage() {
        Vector3 eulerAngles = new Vector3(0, 0, Mathf.Atan2(rb.velocity.y, 
				rb.velocity.x) * 180 / Mathf.PI);
			
		Vector3 localScale = new Vector3(rb.velocity.magnitude / 20, 1, 1);

		PoolManager.instance.ReuseObject (swordAfterimagePrefab, transform.position, eulerAngles, localScale);
	}

	// private start times
	private float attackStartTime;
	
	private float alphaToggleTime;
	// method to perform the slash
	public void Attack() {
		if (attackType != AttackType.none) staminaBar.DecreaseStamina(slashStaminaCost);
		attackStartTime = Time.time;

		// handles current attack type
		HandleAttack();

		UpdateAnimatorVariables();
	}

	public const float MIN_ATTACK_THRESH = 0.5f;
	public void GetAttackType() {
		
		targetB = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		float dist = Vector2.Distance(targetA, targetB);

		if (dist < MIN_ATTACK_THRESH) attackType = AttackType.none;
		else if (dist > SLASHING_THRESHOLD) {
			attackType = AttackType.dash;
			// dashing is handle on a frame-by-frame basis
		}
		else
			attackType = CalcSlashType(); 	// sets slashType to the correct type of slash
	}
	
	// method to get the slash type based on targetA and targetB
	private AttackType CalcSlashType(){
		AttackType slashType;
		// if this is a jab
		float angle = Mathf.Atan2(targetB.y - targetA.y, 
			Mathf.Abs(targetB.x - targetA.x)) * 180 / Mathf.PI;

		if(angle > 30) slashType = AttackType.upSlash;
		else if(angle < -30) slashType = AttackType.downSlash;
		else slashType = AttackType.straightSlash;
		
		return slashType;
	}

	public void CheckForSlashEnd() {

		// check if the slash is over by seeing if the current playing animation is idle
		if (!(animator.GetBool("slashing") || 
				animator.GetBool("upSlashing") || 
				animator.GetBool("downSlashing"))) {
			ResetToIdle();
		}
	}

	public void ResetLayer() {
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	private void HandleAttack() {
		switch (attackType) {
			case AttackType.upSlash:
				state = State.slashing;
				break;

			case AttackType.downSlash:
				state = State.slashing;
				break;

			case AttackType.straightSlash:
				state = State.slashing;
				break;

			case AttackType.dash:
				state = State.dashing;
				break;
		}
	}
}
