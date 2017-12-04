﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Player : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public float jumpPower;
	public int jumps;

	public int comboCount;

	public HashSet<GameObject> allSpeech;
	public bool completedSpeech;

	public enum State {
		idle,
		running,
		autoPathing,
		ready,
		dashing,
		slashing,
		talking,
		damaged,
		dead,
	};

	public enum AttackType {
		upSlash,
		downSlash,
		jab,
		upJab,
		downJab,
		dash,
		none
	}

	public enum AttackResponse {
		normal,
		strong,
		blocked,
		missed,
		combo,
		none
	}

	public State state;
	public AttackType attackType = AttackType.none;
	public AttackResponse attackResponse = AttackResponse.none;

	public GameObject NPCText;
	public GameObject SpeechText;

	public bool grounded;
	public bool autoPathing;
	public bool completedAutoPathing; // triggeers dash/slash after completed autopathing

	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash
	public SlashIndicator slashIndicator;
	public Dustcloud dustcloud;
	public Rigidbody2D rb;
    public Animator anim;

	// constants
	public float SLASHING_X_DIST;
	public float SLASHING_Y_DIST;
	public float SLASHING_THRESHOLD;
	public float AUTO_JUMP_FACTOR;
	public float TURNING_THRESHOLD;
	public float KP;
	public float GRAVITY_SCALE;
	public float DASH_SPEED;
	public float DASH_TARGET_THRESHOLD;
	public float JAB_THRESHOLD;
	public float ATTACK_TIMEOUT;


	private float attackStartTime;

	// Use this for initialization
	void Start () {
		Debug.Log("Start");
		rb = gameObject.GetComponent<Rigidbody2D>();
    	anim = gameObject.GetComponent<Animator>();
		state = State.idle;
		attackType = AttackType.none;
		completedSpeech = false;
		allSpeech = new HashSet<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
		Controls();

		// actions based on the state
		switch (state) {
			case State.autoPathing:
				AutoPath();
				break;

			case State.ready:
				Ready();
				break;
			
			case State.dashing:
				Dash();
				SpawnAfterimages();
				break;

			case State.slashing:
				CheckForSlashEnd();
				break;

			default:
				rb.gravityScale = GRAVITY_SCALE;
				break;
		}		

		RotateSpriteForVelocity();
		if(state != State.dashing) LimitVelocity();

		// update animator variables
    	anim.SetBool("grounded", grounded);
    	anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
		anim.SetBool("dashing", state == State.dashing);
	}

	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking) {

			// get the object we clicked on
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast (ray, out hit)) {
				
				Debug.Log("Clicked on " + hit.transform.name);
				Debug.Log("Layer " + hit.transform.gameObject.layer);

				// handle special cases
				switch (hit.transform.gameObject.layer) {
					// don't autopath onto terrain
					case 8: // terrain
						return;
				}
			}

			state = State.autoPathing;
			targetA = MouseWorldPosition2D();

			// turn the sprite around
			if (targetA.x > transform.position.x)
				transform.localScale = new Vector3(1, 1, 1);
			else 
				transform.localScale = new Vector3(-1, 1, 1);
		}

		if (Input.GetMouseButtonUp(0)) {
			GetAttackType();
		}
	}

	// Grabs the nearest NPC able to chat
	// distance defines the area space that picks up NPCs
	private GameObject NearestNPC(float distance = 2.3f) {
		GameObject[] NPCList = GameObject.FindGameObjectsWithTag("NPC");
		GameObject nearestNPC = null;
		foreach (GameObject NPC in NPCList) {
			Debug.Log(NPC.transform.position);
			if (Vector2.Distance(transform.position, NPC.transform.position) <= distance)
				nearestNPC = NPC;
		}
		return nearestNPC;
	}

	private void RotateSpriteForVelocity() {
		// turn the sprite around based on velocity
		if (rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (1, 1, 1);
			if (state == State.idle)
				state = State.running;
		} else if (rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
			if (state == State.idle)
				state = State.running;
		} else if (state == State.running) state = State.idle;
	}
	private void LimitVelocity() {
		// limit the velocity
		if (rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if (rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}

	public float AUTOPATH_Y_THRESHOLD; 
	public float AUTOPATH_Y_FACTOR;

	// method to handle the autopathing
	private void AutoPath() {
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;

		// if we are at the position to start slashing, freeze until we have an attack!
		if (Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {
			state = State.ready;
			readyStartTime = Time.time;
			return;
		}
		if (Mathf.Abs(xDist) < SLASHING_X_DIST) {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST) {
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
		}

		if (yDist >= AUTOPATH_Y_THRESHOLD && grounded) {
			StartCoroutine(Jump(Mathf.Min(yDist * AUTOPATH_Y_FACTOR, 20f)));
		}
	}

	public float READY_FLOAT_TIMEOUT;
	private float readyStartTime;
	// method for when autopathing is complete and ready to make an attack
	private void Ready() {
		if (Time.time - readyStartTime > READY_FLOAT_TIMEOUT || !Input.GetMouseButton(0)) {
			state = State.idle;
			rb.gravityScale = GRAVITY_SCALE;
		}
		rb.gravityScale = 0;
		rb.velocity = new Vector3(0, 0);
		Attack();	// will do nothing unless an attack is set
	}

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI() {
		GUI.Label(new Rect(20, 20, 100, 100), "Velocity = " + rb.velocity.ToString());
	}

	// method to handle dashing
	// this is only called when auto pathing is completed!
	private void Dash() {
		if (Time.time > attackStartTime + ATTACK_TIMEOUT) {
			state = State.idle;
			attackType = AttackType.none;
		}
		float distanceB = Vector2.Distance(rb.position, targetB);
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.gravityScale = 0;
			rb.velocity = (targetB - rb.position) * DASH_SPEED;
		} 

		// otherwise we have completed the dash
		else {
			rb.velocity = new Vector3(0, 0, 0);
			state = State.idle;
			attackType = AttackType.none;
		}
	}

	private int afterimageCount = 0;
	public int numAfterimage;
	public Color afterimageColor;

	// method to create the after images for the dash
	private void SpawnAfterimages() {
		
		afterimageCount++;
		if(afterimageCount % 3 != 0) return;

		GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
		trailPartRenderer.sortingLayerName = "EntityBackground";
        trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;

		trailPartRenderer.color = afterimageColor;
		trailPart.transform.position = transform.position;
		trailPart.transform.localScale = transform.localScale;
		Destroy(trailPart, 0.3f); // replace 0.5f with needed lifeTime
 
        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }

    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer) {
        Color color = trailPartRenderer.color;
        for (float f = 0.8f; f >= 0; f -= 0.08f) {
            Color c = trailPartRenderer.color;
            c.a = f;
            trailPartRenderer.color = c;
            yield return new WaitForEndOfFrame();
        }
    }

	private Vector2 MouseWorldPosition2D(){
		Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	}

	public void CancelAutomation() {
		if(state == State.autoPathing || state == State.dashing || state == State.slashing) {
			attackType = AttackType.none;
			rb.velocity = new Vector3(0, 0, 0);
			rb.gravityScale = GRAVITY_SCALE;
		}
	}
	
	public float JUMP_DELAY;
	private IEnumerator Jump(float jumpPower) {
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		anim.Play("PlayerJumpUp");
		
		yield return new WaitForSeconds(JUMP_DELAY);
		
		dustcloud.MakeCloud(jumpPos);

		Debug.Log("jump! with vel = " + jumpPower);
		rb.velocity = new Vector2(rb.velocity.x, 0); // prevents stacking velocity
		rb.velocity += Vector2.up * jumpPower;
		anim.Play("PlayerJumping");
		yield return null;
	}

	// method to perform the slash
	private void Attack(){
		Debug.Log("Attacking");
		
		rb.gravityScale = 0;
		rb.Sleep();
		attackStartTime = Time.time;

		switch (attackType) {
			case AttackType.upJab:
				state = State.slashing;
				anim.Play("PlayerJab");
				break;

			case AttackType.jab:
				state = State.slashing;
				anim.Play("PlayerJab");
				break;

			case AttackType.downJab:
				state = State.slashing;
				anim.Play("PlayerDownJab");
				break;

			case AttackType.upSlash:
				state = State.slashing;
				anim.Play("PlayerUpSlash");
				break;

			case AttackType.downSlash:
				state = State.slashing;
				anim.Play("PlayerJab");
				break;

			case AttackType.dash:
				state = State.dashing;
				break;
			
			case AttackType.none:
				break;
		}
	}

	public float MIN_ATTACK_THRESH;
	public void GetAttackType() {
		
		targetB = MouseWorldPosition2D();
		state = State.autoPathing;

		float dist = Vector2.Distance(targetA, targetB);

		if (dist < MIN_ATTACK_THRESH) attackType = AttackType.none;
		else if (dist > SLASHING_THRESHOLD) {
			attackType = AttackType.dash;
			// dashing is handle on a frame-by-frame basis
		}
		else attackType = CalcSlashType(); 	// sets slashType to the correct type of slash
	}
	
	// method to get the slash type based on targetA and targetB
	private AttackType CalcSlashType(){
		AttackType slashType;
		Debug.Log("finding slash type");
		// if this is a jab
		if(Vector2.Distance(transform.position, targetA) < JAB_THRESHOLD) {
			float angle = Mathf.Atan2(targetB.y - targetA.y, 
				targetB.x - targetA.x) * 180 / Mathf.PI;
			Debug.Log("It's a jab! Angle = " + angle);

			if(angle > 30) slashType = AttackType.upJab;
			else if(angle < -30) slashType = AttackType.downJab;
			else slashType = AttackType.jab;
		}
		// otherwise this must be a slash
		else {
			Debug.Log("It's a slash!");
			if(targetA.y >= targetB.y) slashType = AttackType.downSlash;
			else slashType = AttackType.upSlash;
		}

		return slashType;
	}

	private void CheckForSlashEnd() {
		// check if the slash is over by seeing if the current playing animation is idle
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") && Time.time > attackStartTime + 0.2) {
			Debug.Log("Slash ended!");
			rb.WakeUp();
			state = State.idle;
			rb.gravityScale = GRAVITY_SCALE;
			attackType = AttackType.none;
		}
		else rb.Sleep();
	}
}
