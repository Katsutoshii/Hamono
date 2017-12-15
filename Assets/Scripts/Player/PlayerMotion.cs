using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMotion : MonoBehaviour {

    private Player player;
    private Rigidbody2D rb;
	private const float TURNING_THRESHOLD = 0.1f;

	public const float KP = 4f;
	public float maxSpeed;

    // effect prefabs
    // special effects game objects
	public GameObject dustCloudPrefab;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()

    {
        player = gameObject.GetComponent<Player>();
        rb = player.rb;
        
        // create pools for motion effects
		PoolManager.instance.CreatePool(dustCloudPrefab, 1);
    }

	public void RotateSpriteForVelocity() {
		if (!(player.state == Player.State.idle || player.state == Player.State.dashing)) return;
		// turn the sprite around based on velocity
		if (player.rb.velocity.x > TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (1, 1, 1);
		} 
		else if (player.rb.velocity.x < -TURNING_THRESHOLD) {
			transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

	public void LimitVelocity() {
		// limit the velocity
		if (player.rb.velocity.x > maxSpeed) {
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		else if (rb.velocity.x < -maxSpeed) {
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}
	}


	private const float SLASHING_X_DIST = 1f;
	private const float SLASHING_Y_DIST = 0.5f;
	private const float AUTOPATH_Y_THRESHOLD = 1.5f; 
	private const float AUTOPATH_Y_FACTOR = 6.25f;
	private const float JUMP_X_THRESHOLD = 3.5f;
    
    
	private const float AUTOPATH_TIMEOUT = 1.5f;
    

	// method to handle the autopathing
	public void AutoPath() {

		rb.gravityScale = Player.GRAVITY_SCALE;
		float xDist = player.targetA.x - transform.position.x;
		float yDist = player.targetA.y - transform.position.y + 0.5f;

		// timeout if the player
		if (Time.time > player.autoPathStartTime + AUTOPATH_TIMEOUT) {
			player.state = Player.State.idle;
			rb.velocity = new Vector2(0, 0);
			return;
		}

		float tempSlashingXDist = SLASHING_X_DIST;
		if (!Input.GetMouseButton(0) && player.attackType == Player.AttackType.none) tempSlashingXDist /= 3;

		bool positionReached = Mathf.Abs(xDist) < tempSlashingXDist && 		// we are close enough in the x direciton
			(Mathf.Abs(yDist) < SLASHING_Y_DIST || 							// and we are close enough on the y
			(Mathf.Abs(yDist) < AUTOPATH_Y_THRESHOLD && player.grounded));			// OR we are gorunded and meet the grounded thresh

		if (positionReached) {
			// if we are at the position to start slashing, freeze until we have an attack!
			if (Input.GetMouseButton(0) || player.attackType != Player.AttackType.none) {		// if we have an attack queued or we are still drawing
				
				player.state = Player.State.ready;
				player.readyStartTime = Time.time;
			}
			else {
				player.state = Player.State.idle;
			}
			return;
		}

		// if we are crouching for a jump
		if (player.jumping && player.grounded) {
			rb.velocity = new Vector2(0, rb.velocity.y);
			return;
		}

		if (Mathf.Abs(xDist) >= tempSlashingXDist) 
			rb.velocity = new Vector2(xDist * KP, rb.velocity.y);
		else if (Mathf.Abs(xDist) <= 0.05) {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}

		if (yDist >= AUTOPATH_Y_THRESHOLD && xDist <= JUMP_X_THRESHOLD && player.grounded) {
			StartCoroutine(Jump(Mathf.Min(Mathf.Sqrt(Mathf.Abs(yDist)) * AUTOPATH_Y_FACTOR, 20f)));
		}
	}

	private const float JUMP_DELAY = 0.1f;
	IEnumerator Jump(float jumpPower) {
		player.jumping = true;
		rb.velocity = Vector2.zero;
		Vector3 jumpPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
		
		yield return new WaitForSeconds(JUMP_DELAY);
		PoolManager.instance.ReuseObject(dustCloudPrefab, jumpPos, transform.rotation, transform.localScale);
		rb.velocity = Vector2.up * jumpPower;
		yield return new WaitForSeconds(JUMP_DELAY);
		
		player.jumping = false;
		yield return null;
	}
}