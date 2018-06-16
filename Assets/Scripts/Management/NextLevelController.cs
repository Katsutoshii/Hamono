using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelController : MonoBehaviour {

  	public int nextLevel;
	public GameObject fadeToBlackEffect;
	public Animator fadeToBlackAnimator;
	private Player player;

	void Start() {
		Debug.Log("next door!");
		fadeToBlackEffect = GameObject.Find("FadeToBlack");
		fadeToBlackAnimator = fadeToBlackEffect.GetComponentInChildren<Animator>();
		
		Debug.Log("fade to black = " + fadeToBlackEffect.ToString());
	}

	private IEnumerator NextScene() {
		player = FindObjectOfType<Player>();
		
		yield return new WaitForSeconds(0.6f);
		PlayerPrefs.SetFloat("health", player.healthAmount);
		PlayerPrefs.SetInt("coin_count", player.coinCount);
		PlayerPrefs.SetInt("next_level", nextLevel);
		SceneManager.LoadScene(5); // takes player to the loading scene
	}

	/// <summary>
	/// Sent each frame where another object is within a trigger collider
	/// attached to this object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerStay2D(Collider2D other)
	{
			if (other.name == "Player") {
				Player player = other.gameObject.GetComponent<Player>();
			 	if (player.state == Player.State.idle && player.rb.velocity.x == 0 && player.rb.velocity.y == 0) StartCoroutine(NextScene());
			}
	}
}
