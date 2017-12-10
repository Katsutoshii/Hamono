using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadScene(int sceneID) {
		SceneManager.LoadScene(sceneID);
	}
}
