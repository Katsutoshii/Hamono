using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelController : MonoBehaviour {

  public int nextLevel;
	private GameObject fadeToBlackEffect;

	void Start() {
		fadeToBlackEffect = Resources.Load<GameObject>("Prefabs/Environment/FadeToBlack");
	}

  public void NextLevel() {
		Instantiate(fadeToBlackEffect);
		StartCoroutine(NextScene());
	}

	private IEnumerator NextScene() {
		yield return new WaitForSeconds(.5f);
		PlayerPrefs.SetInt("next_level", nextLevel);
		SceneManager.LoadScene(3); // takes player to the loading scene
	}
}
