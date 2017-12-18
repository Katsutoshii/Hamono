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


	private const float SLASHING_X_DIST = 1f;
	private const float SLASHING_Y_DIST = 0.5f;
	private const float AUTOPATH_Y_THRESHOLD = 1.5f; 
	private const float AUTOPATH_Y_FACTOR = 6.25f;
	private const float AUTOPATH_X_FACTOR = 6.25f;
	private const float JUMP_X_THRESHOLD = 3.5f;
    
    
	private const float AUTOPATH_TIMEOUT = 1.5f;
	public float autoPathLimitY;
    

	// method to handle the autopathing
	public void AutoPath() {
        rb.gravityScale = GRAVITY_SCALE;
        if (grounded) staminaBar.IncreaseStamina(generateStamina);

		float xDist = targetA.x - transform.position.x;
		float yDist = targetA.y - transform.position.y;
        if (transform.position.y < targetA.y) yDist += 0.5f;

		// timeout if the player
		if (Time.time > autoPathStartTime + AUTOPATH_TIMEOUT || targetA.y - transform.position.y > autoPathLimitY) {
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

		// if we are crouching for a jump
		if (jumping && grounded) {
			rb.velocity = new Vector2(0, rb.velocity.y);
			return;
		}

		if (Mathf.Abs(xDist) >= tempSlashingXDist) 
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
		else if (Mathf.Abs(xDist) <= 0.05) {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}

		RotateSpriteForVelocity();

		if (!jumping && grounded) { 
			if ((yDist >= AUTOPATH_Y_THRESHOLD && (xDist <= JUMP_X_THRESHOLD))) 
				StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR, 20f)));
			
			else if (yDist >= 0 && onEdge) {
				StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist) * AUTOPATH_Y_FACTOR + Mathf.Abs(xDist) * AUTOPATH_X_FACTOR), 20f)));
			}
		}
	}

	private const float JUMP_DELAY = 0.1f;
	IEnumerator Jump(float jumpPower) {
		jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		PoolManager.instance.ReuseObject(dustCloudPrefab, jumpPos, transform.rotation, transform.localScale);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(JUMP_DELAY);
		
		jumping = false;
		yield return null;
	}
}