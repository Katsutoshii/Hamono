using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

  [SerializeField]
  private int scene;
  private Text loadingText;

  public Sprite[] sprites;
	private int spritePerFrame = 6;
	private bool loop = true;
	private bool destroyOnEnd = false;

	private int index = 0;
	private Image image;
	private int frame = 0;

  void Start() {
    loadingText = transform.GetChild(1).transform.GetChild(1).GetComponent<Text>();
  }

	void Awake() {
		image = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
	}

  void Update() {
    StartCoroutine(LoadNewScene());

    PlayerAnimation();

    loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, .8f));
    
  }

  // animation for image.sprite
  private void PlayerAnimation() {
    float xPosition = image.transform.position.x + .1f;
    if (image.transform.position.x - 30f > Screen.width)
      image.transform.position = new Vector2(-6f, image.transform.position.y);
    image.transform.position = new Vector2(image.transform.position.x + 6f, image.transform.position.y);

    if (!loop && index == sprites.Length) return;
		frame ++;
		if (frame < spritePerFrame) return;
		image.sprite = sprites [index];
		frame = 0;
		index ++;
		if (index >= sprites.Length) {
			if (loop) index = 0;
			if (destroyOnEnd) Destroy (gameObject);
		}
  }

  // loads a new scene
  IEnumerator LoadNewScene() {
    yield return new WaitForSeconds(3f);

    AsyncOperation async = Application.LoadLevelAsync(scene);

    while (!async.isDone)
      yield return null;
  }
}
