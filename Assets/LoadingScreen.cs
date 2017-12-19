using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {
  private bool loadScene = false;

  [SerializeField]
  private int scene;
  public Text loadingText;

  void Update() {
    if (Input.GetKeyUp(KeyCode.Space) && loadScene == false) {
      loadScene = true;
      StartCoroutine(LoadNewScene());
    }

    if (loadScene) {
      loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, .8f));
    }
  }

  IEnumerator LoadNewScene() {
    yield return new WaitForSeconds(3f);

    AsyncOperation async = Application.LoadLevelAsync(scene);

    while (!async.isDone)
      yield return null;
  }
}
