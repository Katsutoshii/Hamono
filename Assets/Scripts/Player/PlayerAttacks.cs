using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerAttacks : MonoBehaviour {

	private Player player;
	private Rigidbody2D rb;
	public float dashStaminaCost;
	// public constants
	public float slashStaminaCost;

	// effect prefabs
	public GameObject afterimagePrefab;
	public GameObject swordAfterimagePrefab;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
		player = gameObject.GetComponent<Player>();
		rb = player.rb;

		// create pools for attack effects
		PoolManager.instance.CreatePool(afterimagePrefab, 10);
		PoolManager.instance.CreatePool(swordAfterimagePrefab, 20);
    }
	
	// private constants
	private const float ATTACK_TIMEOUT = 0.5f;

	private const float DASH_SPEED = 8f;
	private const float DASH_TARGET_THRESHOLD = 0.8f;
	private const float READY_FLOAT_TIMEOUT = 0.5f;

	// method for when autopathing is complete and ready to make an attack
	public void Ready() {
		
		rb.velocity = new Vector3(0, 0);
		Attack();	// will do nothing unless an attack is set

		if ((player.attackType == Player.AttackType.none && 
			player.state != Player.State.dashing && 
			player.state != Player.State.slashing &&
			(Time.time - player.readyStartTime > READY_FLOAT_TIMEOUT && !player.grounded) // time out if floating
			)) {

			player.state = Player.State.idle;
			player.attackType = Player.AttackType.none;
		}
		
		rb.gravityScale = 0;
	}

	// method to handle dashing
	// this is only called when auto pathing is completed!
	public void Dash() {
		gameObject.layer = 14;
		if (player.stamina.isExhausted()) {
			rb.velocity = new Vector3(0, 0, 0);
			player.state = Player.State.idle;
			player.attackType = Player.AttackType.none;
			return;
		}
		
		float distanceB = Vector2.Distance(rb.position, player.targetB);
		player.stamina.DecreaseStamina(dashStaminaCost * distanceB / 2);

		if (Time.time > player.attackStartTime + ATTACK_TIMEOUT) {
			player.state = Player.State.idle;
			player.attackType = Player.AttackType.none;
		}
		
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.velocity = (player.targetB - rb.position) * DASH_SPEED;
			SpawnSwordAfterimage();
			SpawnAfterimage();
		} 

		// otherwise we have completed the dash
		else {
			rb.velocity = new Vector3(0, 0, 0);
			gameObject.layer = 11;
			player.state = Player.State.idle;
			player.attackType = Player.AttackType.none;
		}
		
		rb.gravityScale = 0;
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

	// method to perform the slash
	public void Attack() {
		if (player.attackType != Player.AttackType.none) player.stamina.DecreaseStamina(slashStaminaCost);
		player.attackStartTime = Time.time;

		// handles current attack type
		HandleAttack();

		player.UpdateAnimatorVariables();
	}

	private const float MIN_ATTACK_THRESH = 0.2f;
	public void GetAttackType() {
		
		player.targetB = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		float dist = Vector2.Distance(player.targetA, player.targetB);

		if (dist < MIN_ATTACK_THRESH) player.attackType = Player.AttackType.none;
		else if (dist > Player.SLASHING_THRESHOLD) {
			player.attackType = Player.AttackType.dash;
			// dashing is handle on a frame-by-frame basis
		}
		else
			player.attackType = CalcSlashType(); 	// sets slashType to the correct type of slash
	}
	
	// method to get the slash type based on targetA and targetB
	private Player.AttackType CalcSlashType(){
		Player.AttackType slashType;
		// if this is a jab
		float angle = Mathf.Atan2(player.targetB.y - player.targetA.y, 
			Mathf.Abs(player.targetB.x - player.targetA.x)) * 180 / Mathf.PI;

		if(angle > 30) slashType = Player.AttackType.upSlash;
		else if(angle < -30) slashType = Player.AttackType.downSlash;
		else slashType = Player.AttackType.straightSlash;
		
		return slashType;
	}

	public void CheckForSlashEnd() {

		// check if the slash is over by seeing if the current playing animation is idle
		if (!(player.animator.GetBool("slashing") || 
				player.animator.GetBool("upSlashing") || 
				player.animator.GetBool("downSlashing"))) {
			player.state = Player.State.idle;
			player.attackType = Player.AttackType.none;
		}
	}

	private void HandleAttack() {
		switch (player.attackType) {
			case Player.AttackType.upSlash:
				player.state = Player.State.slashing;
				break;

			case Player.AttackType.downSlash:
				player.state = Player.State.slashing;
				break;

			case Player.AttackType.straightSlash:
				player.state = Player.State.slashing;
				break;

			case Player.AttackType.dash:
				player.state = Player.State.dashing;
				break;
			
			case Player.AttackType.none:
				player.attackResponse = Player.AttackResponse.none;
				break;
		}
	}
}