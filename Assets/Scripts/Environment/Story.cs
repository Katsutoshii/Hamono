using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Story : MonoBehaviour {


  private TextTyper storyText;
	 [TextArea(3,10)]
 	public string[] text;
  public Sprite[] sprites;

  public float readingTime;

  private int scriptIndex;
  private int imageIndex;

  private Image storyImage;
	
	public bool completedSpeech;
  private bool startedSpeech;
	private bool dialogStarted;
  private bool clicked;

  private Animator anim;
  private bool fade;

  private GameObject fadeToBlackEffect;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake()
  {
      fadeToBlackEffect = GameObject.Find("FadeToBlack");
      fadeToBlackEffect.SetActive(false);
  }

	// Use this for initialization
	void Start () {

		completedSpeech = false;
    startedSpeech = false;

    storyImage = GameObject.Find("StoryImage").GetComponent<Image>();
    anim = fadeToBlackEffect.transform.GetChild(0).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
    ClearScript();
		if (Input.GetMouseButtonDown(0)) StartStory(); // continue dialog if we click anywhere
	}

  IEnumerator AutoNextScript() {
    yield return new WaitForSeconds(readingTime);
    if (!clicked)
      StartStory();
    else
      yield return null;
  }

  private void ClearScript() {
    if (completedSpeech) {
      // clearing script
      dialogStarted = false;
      completedSpeech = false;
      startedSpeech = false;
      clicked = false;
      StartCoroutine(AutoNextScript());
    }
  }

	private void StartStory() {
		dialogStarted = true;
		Debug.Log("starting story");
    clicked = true;

		storyText = GameObject.Find("StoryText").GetComponent<TextTyper>();
		Debug.Log("completed speech?: " + completedSpeech);

		if (!startedSpeech && !completedSpeech && scriptIndex < text.Length) {
      storyText.TypeText(text[scriptIndex]);
      StartCoroutine(NextImage());
      completedSpeech = false;
      startedSpeech = true;
      scriptIndex++;
    } else if (startedSpeech && !completedSpeech)
      storyText.Skip();
      else if (scriptIndex >= text.Length) {
        SceneManager.LoadSceneAsync(0); // takes player to title screen
      }
	}

   IEnumerator NextImage() {
    if (imageIndex < sprites.Length) {
      fadeToBlackEffect.SetActive(true);
      yield return new WaitForSeconds(1f);
      storyImage.sprite = sprites[imageIndex];
      imageIndex++;
      // reverses animation
      anim.SetFloat("direction", -1f);
      yield return new WaitForSeconds(1f);
      fadeToBlackEffect.SetActive(false);

    }
  }
}
