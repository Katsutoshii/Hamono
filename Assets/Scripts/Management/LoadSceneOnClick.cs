using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {
	private LoadingBar loadingBar;
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		loadingBar = FindObjectOfType<LoadingBar>();
		PlayerPrefs.DeleteAll();
	}
	public void LoadScene(int sceneID) {
		Debug.Log("starting scene " + sceneID);
		loadingBar.RunAnimation();
		SceneManager.LoadSceneAsync(sceneID);
	}
}
