using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Player : MonoBehaviour {

	private bool touchingEnemy = false;

    /// <summary>
	/// Called each frame the player collides with something
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionStay2D(Collision2D other)
	{
		if (other.collider.name.Length < 4) Debug.Log("Object names must be greater than 4");
		switch (other.collider.name.Substring(0, 4)) {
			case "Spik":
				if (!invincible) {
					Damage(0.5f, 0f, other.collider);
					rb.velocity = new Vector2(rb.velocity.x, 9f);
					
				}
				break;

			case "Hear":
				healthAmount += 1;
				healthAmount = Mathf.Min(healthAmount, maxHealth);
				healthBar.HandleHealth(healthAmount);
				break;
		}

		switch (LayerMask.LayerToName(other.gameObject.layer)) {
			case "Enemies":
				touchingEnemy = true;
				break;

		}
	}

	/// <summary>
	/// Called each frame the player collides with something
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.collider.name.Length < 4) Debug.Log("Object names must be greater than 4");
		switch (other.collider.name.Substring(0, 4)) {
			case "Coin":
				coinCount++;
				PlayerPrefs.SetInt("coin_coint", coinCount);
				coinCountText.text = "" + coinCount;
				break;

			case "Hear":
				healthAmount += 1;
				healthAmount = Mathf.Min(healthAmount, maxHealth);
				healthBar.HandleHealth(healthAmount);
				break;
		}
	}

	/// <summary>
	/// OnCollisionExit is called when this collider/rigidbody has
	/// stopped touching another rigidbody/collider.
	/// </summary>
	/// <param name="other">The Collision data associated with this collision.</param>
	void OnCollisionExit(Collision other)
	{
		switch (LayerMask.LayerToName(other.gameObject.layer)) {
			case "Enemies":
				touchingEnemy = false;
				break;
		}
	}
	
	/// <summary>
	/// OnTriggerEnter is called when the Collider other enters the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{

		switch (other.name) {
			case "EnemyHurtBox":
				if (state != State.dashing && state != State.slashing && state != State.damaged && !invincible)
					Damage(0.5f, 4f, other);
				else attackResponse = AttackResponse.normal; 
				break;
		}
	}
}
