using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Player : MonoBehaviour {

	
	public Vector2 maxSpeed;
	private const float TURNING_THRESHOLD = 0.1f;

	public const float KP = 4f;

    // effect prefabs
    // special effects game objects
	public GameObject dustCloudPrefab;

	public void RotateSpriteForVelocity() {
		if (!(state == State.idle || state == State.dashing)) return;
		// turn the sprite around based on velocity
		if (rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (1, 1, 1);
		} 
		else if (rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

	public void LimitVelocity() {
		// limit the velocity
		rb.velocity = new Vector2(Bound(rb.velocity.x, -maxSpeed.x, maxSpeed.x), Bound(rb.velocity.y, -maxSpeed.y, maxSpeed.y));
	}

    private float Bound(float val, float min, float max) {
        return Mathf.Max(Mathf.Min(val, max), min);
    }

	private void StartAutoPath() {
		if (grounded) canJump = true;
		else canJump = false;
		
		state = State.autoPathing;
		autoPathStartTime = Time.time;
	}

	private const float SLASHING_X_DIST = 1.1f;
	private const float SLASHING_Y_DIST = 0.5f;
	private const float AUTOPATH_Y_THRESHOLD = 1.2f; 
	private const float AUTOPATH_Y_FACTOR = 5.85f;
	private const float AUTOPATH_X_FACTOR = 0.9f;
	private const float JUMP_X_THRESHOLD = 10f;
    
    
	private const float AUTOPATH_TIMEOUT = 1.5f;
	private const float SCALE_REACHER = 2f;
	public float autoPathLimitY;
	private bool canJump;
    

	// method to handle the autopathing
	public void AutoPath() {
        rb.gravityScale = GRAVITY_SCALE;
        if (grounded) staminaBar.IncreaseStamina(generateStamina);

		if (touchingEnemy && Vector2.Distance(targetA, transform.position) < 1) targetA = transform.position;

		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;
        if (transform.position.y < targetA.y) yDist += 0.7f;

		// timeout if the player is too far from the target in the y direction
		if (Time.time > autoPathStartTime + AUTOPATH_TIMEOUT || Mathf.Abs(targetA.y - transform.position.y) > autoPathLimitY) {
			ResetToIdle();
			return;
		}

		float tempSlashingXDist = SLASHING_X_DIST;
		if (!Input.GetMouseButton(0) && attackType == AttackType.none) tempSlashingXDist /= 3;

		bool positionReached = Mathf.Abs(xDist) < tempSlashingXDist && 		// we are close enough in the x direciton
			(Mathf.Abs(yDist) < SLASHING_Y_DIST || 							// and we are close enough on the y
			(Mathf.Abs(yDist) < AUTOPATH_Y_THRESHOLD && grounded));			// OR we are gorunded and meet the grounded thresh

		if (positionReached) {
            rb.velocity = new Vector2(0, rb.velocity.y);
			// if we are at the position to start slashing, freeze until we have an attack!
			if (Input.GetMouseButton(0) || attackType != AttackType.none) {		// if we have an attack queued or we are still drawing
				state = State.ready;
				readyStartTime = Time.time;
			}
			else {
				state = State.idle;
			}
			return;
		}

		// motion towards the x target
		if (Mathf.Abs(xDist) >= tempSlashingXDist && !(grounded & jumping)) {
			rb.velocity = new Vector2(xDist * KP + 0.1f * Mathf.Sign(xDist), rb.velocity.y);
		}

		// prevents overshoot
		if (Mathf.Abs(xDist) <= 0.01f) rb.velocity = new Vector2(0, rb.velocity.y);

		RotateSpriteForVelocity();

		if (!jumping && grounded && canJump) { 
			if ((yDist >= AUTOPATH_Y_THRESHOLD && xDist <= JUMP_X_THRESHOLD) || (yDist >= -2 && onEdge)) {
				jumping = true;
				StartCoroutine(Jump(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR + Mathf.Sqrt(Mathf.Abs(xDist)) * AUTOPATH_X_FACTOR));
			}	
		}
	}

	private const float JUMP_DELAY = 0.05f;
	IEnumerator Jump(float jumpPower) {
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		PoolManager.instance.ReuseObject(dustCloudPrefab, jumpPos, transform.rotation, transform.localScale);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(0.5f);
		yield return new WaitUntil(() => rb.velocity.y <= 0);
		
		jumping = false;
		yield return null;
	}
}