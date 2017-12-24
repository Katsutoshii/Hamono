using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelController : MonoBehaviour {

  public int nextLevel;
	public GameObject fadeToBlackEffect;

	void Start() {
		Debug.Log("next door!");
		fadeToBlackEffect = GameObject.Find("/UI/FadeToBlack");
		fadeToBlackEffect.SetActive(false);
	}

	private IEnumerator NextScene() {
		
		fadeToBlackEffect.SetActive(true);
		yield return new WaitForSeconds(0.6f);
		PlayerPrefs.SetInt("next_level", nextLevel);
		SceneManager.LoadSceneAsync(3); // takes player to the loading scene
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
