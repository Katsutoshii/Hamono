using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Player : MonoBehaviour {
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

 	/// <summary>
	/// Sent each frame where another object is within a trigger collider
	/// attached to this object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerStay2D(Collider2D other)
	{
			switch (other.name) {
				case "Next Level Door":
					if (state == State.idle && rb.velocity.x == 0 && rb.velocity.y == 0) NextLevel();
					break;
			}
	}

	private void NextLevel() {
		Instantiate(fadeToBlackEffect);
		StartCoroutine(NextScene());
	}

	private IEnumerator NextScene() {
		yield return new WaitForSeconds(.5f);
		PlayerPrefs.SetInt("next_level", 2);
		SceneManager.LoadScene(3);
	}
}
