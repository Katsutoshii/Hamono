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

  public Sprite[] sprites;
	public int spritePerFrame = 6;
	public bool loop = true;
	public bool destroyOnEnd = false;

	private int index = 0;
	private Image image;
	private int frame = 0;

	void Awake() {
		image = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>();
	}


  void Update() {
    if (Input.GetKeyUp(KeyCode.Space) && loadScene == false) {
      loadScene = true;
      StartCoroutine(LoadNewScene());
    }
    float xPosition = image.transform.position.x + .1f;
    if (image.transform.position.x + 6f > Screen.width)
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
