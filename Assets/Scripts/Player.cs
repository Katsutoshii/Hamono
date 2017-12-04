using System.Collections;
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
		completedAutoPathing = false;
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

		// for move left and right manually
		if (Input.GetKey(key:KeyCode.D) && Input.GetKey(key:KeyCode.A) && state != State.talking) {
			CancelAutomation();
			rb.velocity = new Vector2(0, rb.velocity.y);
			state = State.idle;
		}
		else if (Input.GetKey(key:KeyCode.A) && state != State.talking) {
			CancelAutomation();
			rb.velocity = new Vector2(-speed, rb.velocity.y);
			state = State.running;
		}
		else if(Input.GetKey(key:KeyCode.D) && state != State.talking) {
			CancelAutomation();
			rb.velocity = new Vector2(speed, rb.velocity.y);
			state = State.running;
		} else if (Input.GetKeyDown(key:KeyCode.S)) {
			// triggers a speech bubble
			GameObject nearestNPC = NearestNPC();
			if (nearestNPC != null) {
				if (completedSpeech && state == State.talking) {
				foreach (GameObject item in allSpeech)
					Destroy(item);
				state = State.idle;
				completedSpeech = false;
			} else if (state != State.talking) {
				state = State.talking;
				// Need to trigger correct canvas
				NPCText = Instantiate(SpeechText);
				NPCText.transform.position = new Vector2(nearestNPC.transform.position.x, nearestNPC.transform.position.y + 1.2f);
				allSpeech.Add(NPCText);
				TextTyper NPCTextChild = NPCText.transform.GetChild(0).gameObject.GetComponent<TextTyper>();
				NPCTextChild.TypeText("Hey! I'm an NPC. Talk to me.");
				completedSpeech = false;
			}
			}
		}
		else if (state == State.running) {
			rb.velocity = new Vector2(0, rb.velocity.y);
			state = State.idle;
		}
		

		// for jumping
		if (Input.GetKeyDown(key: KeyCode.W) && jumps > 0 && state != State.talking)
		{
			CancelAutomation();
			anim.Play("PlayerJumpUp");
			state = State.idle;
		}
		if (Input.GetKeyUp(key:KeyCode.W) && jumps > 0 && state != State.talking) {
			Jump(jumpPower);
		}

		// for initiating action
		if (Input.GetMouseButtonDown(0) && state != State.talking) {
				jumps = 0;
				state = State.autoPathing;
				completedAutoPathing = false;
				targetA = MouseWorldPosition2D();

				// turn the sprite around
				if (targetA.x > transform.position.x)
					transform.localScale = new Vector3(1, 1, 1);
				else 
					transform.localScale = new Vector3(-1, 1, 1);
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

	// method to handle the autopathing
	private void AutoPath() {
		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;

		// if we are at the position to start slashing, freeze until we have an attack!
		if(Mathf.Abs(xDist) < SLASHING_X_DIST && Mathf.Abs(yDist) < SLASHING_Y_DIST) {
			completedAutoPathing = true;
			rb.gravityScale = 0;
			rb.velocity = new Vector3(0, 0);
			Attack();	// will do nothing unless an attack is set
		}

		// otherwise, if we need to move in the x or y direction, do so
		if (Mathf.Abs(xDist) >= SLASHING_X_DIST || Mathf.Abs(yDist) >= SLASHING_Y_DIST) {
			completedAutoPathing = false;
			rb.velocity = new Vector2(xDist * KP, yDist * KP);
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
			completedAutoPathing = false;
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
			completedAutoPathing = false;
			attackType = AttackType.none;
			rb.velocity = new Vector3(0, 0, 0);
			rb.gravityScale = GRAVITY_SCALE;
		}
	}
	

	private void Jump(float power) {
		rb.velocity = new Vector2(rb.velocity.x, 0); // prevents stacking velocity
		rb.velocity += Vector2.up * power;
		jumps--;
		anim.Play("PlayerJumping");
		dustcloud.MakeCloud(transform.position);
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

	public void GetAttackType() {
		
		targetB = MouseWorldPosition2D();
		state = State.autoPathing;
		if (Vector2.Distance(targetA, targetB) > SLASHING_THRESHOLD) {
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
			attackType = AttackType.none;
		}
		else rb.Sleep();
	}
}
