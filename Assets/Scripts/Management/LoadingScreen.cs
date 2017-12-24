using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

  public float movementSpeed;
  public int maxLoops;
  private int completedLoops;

  [SerializeField]
  private int scene;
  private Text loadingText;
	private Image image;

  void Start() {
    Debug.Log("Starting load scene");
    loadingText = transform.GetChild(1).transform.GetChild(1).GetComponent<Text>();
    completedLoops = 0;

    scene = PlayerPrefs.GetInt("next_level");
  }

	void Awake() {
		image = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
	}

  void FixedUpdate() {

    PlayerAnimation();

    loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, .8f));
    
  }

  private float x = 0;
  // animation for image.sprite
  private void PlayerAnimation() {
    x += movementSpeed;
    if (x  >= Screen.width) {
      completedLoops++;
      if (completedLoops >= maxLoops) SceneManager.LoadSceneAsync(scene);

      x %= Screen.width;
    }

    image.transform.position = new Vector3(x, image.transform.position.y, image.transform.position.z);
  }
}
