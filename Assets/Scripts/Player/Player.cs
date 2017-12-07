using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Player : MonoBehaviour {

	public float maxSpeed;
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
		straightSlash,
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
	public SpriteRenderer spriteRenderer;

	public bool grounded;
	public bool autoPathing;
	public bool completedAutoPathing; // triggeers dash/slash after completed autopathing

	public Vector2 targetA; 	// start point of a slash
	public Vector2 targetB;		// end point of a slash
	public SlashIndicator slashIndicator;
	public Dustcloud dustcloud;
	public Rigidbody2D rb;
	public Animator anim;
	public StaminaBar stamina;
	public GameObject afterimagePrefab;
	public GameObject swordAfterimagePrefab;
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

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
	public float ATTACK_TIMEOUT;
	public float AUTOPATH_TIMEOUT;
	public float DASH_STAMINA_COST;
	public float GENERATE_STAMINA;


	private float attackStartTime;
	private float autoPathStartTime;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody2D>();
    	anim = gameObject.GetComponent<Animator>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

		state = State.idle;
		attackType = AttackType.none;


		completedSpeech = false;
		allSpeech = new HashSet<GameObject>();

		PoolManager.instance.CreatePool(afterimagePrefab, 10);
		PoolManager.instance.CreatePool(swordAfterimagePrefab, 20);

		NPCText = null;
	}
	
	// Update is called once per frame
	void Update() {
	
		Controls();

		// actions based on the state
		switch (state) {
			
			case State.autoPathing:
				AutoPath();
				rb.gravityScale = GRAVITY_SCALE;
				break;

			case State.ready:
				Ready();
				rb.gravityScale = 0;
				break;
			
			case State.dashing:
				Dash();
				SpawnAfterimage();
				rb.gravityScale = 0;
				break;

			case State.slashing:
				CheckForSlashEnd();
				rb.gravityScale = 0;
				break;

			default:
				rb.velocity = new Vector2(0, rb.velocity.y);
				rb.gravityScale = GRAVITY_SCALE;
				if (grounded) stamina.increaseStamina(GENERATE_STAMINA);
				break;
		}		

		RotateSpriteForVelocity();
		if(state != State.dashing) LimitVelocity();

		// update animator variables
    	anim.SetBool("grounded", grounded);
    	anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
		anim.SetBool("dashing", state == State.dashing);
	}

	private bool falling;
	// method to handle all control inputs inside main loop
	private void Controls() {
		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking) {

			// get the object we clicked on
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
			if(hit.collider != null) {
				
				Debug.Log("Clicked on " + hit.transform.name);
				Debug.Log("Layer " + hit.transform.gameObject.layer);

				// handle special cases
				switch (hit.transform.gameObject.layer) {
					// don't autopath onto terrain
					case 8: // terrain
						return;
				}
			}

			autoPathStartTime = Time.time;
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

		if (rb.velocity.y < 0 && !grounded && !falling) {
			falling = true;
			anim.Play("PlayerFalling");
		}
		else if (falling && grounded) {
			falling = false;
			anim.Play("PlayerLanding");
		}

		if (Input.GetMouseButtonDown(1)) {
			StartDialogue();
		}
	}

	private void StartDialogue() {
		// triggers a speech bubble

			GameObject nearestNPC = NearestNPC();
			TextTyper NPCTextChild;
			if (NPCText == null) {
				NPCText = Instantiate(SpeechText);
				NPCText.transform.position = new Vector2(nearestNPC.transform.position.x, nearestNPC.transform.position.y + 1.2f);
			}
			NPCTextChild = NPCText.transform.GetChild(0).gameObject.GetComponent<TextTyper>();
			if (completedSpeech && state == State.talking) {
				// ending conversation
				foreach (GameObject item in allSpeech)
					Destroy(item);
				state = State.idle;
				completedSpeech = false;
				NPCText = null;
			} else if (state != State.talking && nearestNPC != null && !completedSpeech) {
				// starting converstation
				state = State.talking;
				allSpeech.Add(NPCText);
				NPCTextChild.TypeText("Hey! I'm an NPC. Talk to me. \n I'm talking for a really long time. \n You probably find this extremely annoying.");
				completedSpeech = false;
			} else if (state == State.talking) {
				// skipping content
				NPCTextChild.Skip();
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
		} else if (rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
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
	public float JUMP_X_THRESHOLD;

	// method to handle the autopathing
	private void AutoPath() {
		if (grounded) stamina.increaseStamina(GENERATE_STAMINA);
		rb.gravityScale = GRAVITY_SCALE;
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y + 0.5f;

		// timeout if the player cannot reach destination
		if (Time.time > autoPathStartTime + AUTOPATH_TIMEOUT) {
			state = State.idle;
			rb.velocity = new Vector2(0, 0);
			return;
		}

		// if we are at the position to start slashing, freeze until we have an attack!
		if (Mathf.Abs(xDist) < SLASHING_X_DIST && (Mathf.Abs(yDist) < SLASHING_Y_DIST
			|| (Mathf.Abs(yDist) < AUTOPATH_Y_THRESHOLD && grounded))) {
			state = State.ready;
			readyStartTime = Time.time;
			return;
		}

		if (prejumping) {
			rb.velocity = new Vector2(0, 0);
			return;
		}

		// fixes overshooting
		if (Mathf.Abs(xDist) < SLASHING_X_DIST)
			rb.velocity = new Vector2(0, rb.velocity.y);

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST)
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);

		if (yDist >= AUTOPATH_Y_THRESHOLD && xDist <= JUMP_X_THRESHOLD && grounded)
			StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR, 20f)));
	}

	public float JUMP_DELAY;
	private bool prejumping = false;
	private IEnumerator Jump(float jumpPower) {
		prejumping = true;
		rb.velocity = Vector2.zero;
		Debug.Log("Jump!");
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		anim.Play("PlayerJumpUp");
		
		yield return new WaitForSeconds(JUMP_DELAY);
		prejumping = false;
		dustcloud.MakeCloud(jumpPos);

		Debug.Log("jump! with vel = " + jumpPower);
		rb.velocity = Vector2.up * jumpPower;
		anim.Play("PlayerJumping");
		yield return null;
	}

	public float READY_FLOAT_TIMEOUT;
	private float readyStartTime;
	// method for when autopathing is complete and ready to make an attack
	private void Ready() {
		Debug.Log("Ready");
		
		rb.velocity = new Vector3(0, 0);
		Attack();	// will do nothing unless an attack is set

		if (attackType == AttackType.none && state != State.dashing && state != State.slashing &&
			(Time.time - readyStartTime > READY_FLOAT_TIMEOUT || !Input.GetMouseButton(0))) {
			Debug.Log("Cancel ready!");
			state = State.idle;
			return;
		}
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
		if (stamina.isExhausted()) {
			rb.velocity = new Vector3(0, 0, 0);
			state = State.idle;
			attackType = AttackType.none;
			return;
		}
		stamina.decreaseStamina(DASH_STAMINA_COST);
		Debug.Log("dash");
		if (Time.time > attackStartTime + ATTACK_TIMEOUT) {
			state = State.idle;
			attackType = AttackType.none;
		}
		float distanceB = Vector2.Distance(rb.position, targetB);
		
		// if we are mid dash
		if (distanceB > DASH_TARGET_THRESHOLD) {
			rb.velocity = (targetB - rb.position) * DASH_SPEED;
			SpawnSwordAfterimage();
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

	private Vector2 MouseWorldPosition2D() {
		Vector3 worldSpaceClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector2(worldSpaceClickPosition.x, worldSpaceClickPosition.y);
	}

	public void CancelAutomation() {
		if(state == State.autoPathing || state == State.dashing || state == State.slashing) {
			attackType = AttackType.none;
			state = State.idle;
			rb.velocity = new Vector3(0, 0, 0);
		}
	}

	// method to perform the slash
	private void Attack(){
		attackStartTime = Time.time;

		switch (attackType) {
			case AttackType.upSlash:
				state = State.slashing;
				anim.Play("PlayerUpSlash");
				break;

			case AttackType.downSlash:
				state = State.slashing;
				anim.Play("PlayerDownJab");
				break;

			case AttackType.straightSlash:
				state = State.slashing;
				anim.Play("PlayerJab");
				break;

			case AttackType.dash:
			Debug.Log("Setting state to dashing");
				state = State.dashing;
				anim.Play("PlayerDash");
				break;
			
			case AttackType.none:
				break;
		}
		attackType = AttackType.none;
	}

	public float MIN_ATTACK_THRESH;
	public void GetAttackType() {
		
		targetB = MouseWorldPosition2D();
		//state = State.autoPathing;

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
		float angle = Mathf.Atan2(targetB.y - targetA.y, 
			Mathf.Abs(targetB.x - targetA.x)) * 180 / Mathf.PI;
		Debug.Log("It's a jab! Angle = " + angle);

		if(angle > 30) slashType = AttackType.upSlash;
		else if(angle < -30) slashType = AttackType.downSlash;
		else slashType = AttackType.straightSlash;
		
		return slashType;
	}

	private void CheckForSlashEnd() {
		// check if the slash is over by seeing if the current playing animation is idle
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle") && Time.time > attackStartTime + 0.2) {
			Debug.Log("Slash ended!");
			state = State.idle;
			attackType = AttackType.none;
		}
	}

	private void OnMouseEnter() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}
}
