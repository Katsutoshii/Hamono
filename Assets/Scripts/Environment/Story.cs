using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;

public class Story : MonoBehaviour {


  private TextTyper storyText;
	 [TextArea(3,10)]
 	public string[] text;
  public Sprite[] sprites;

  private int scriptIndex;
  private int imageIndex;

  private Image storyImage;
	
	public bool completedSpeech;
  private bool startedSpeech;
	private bool dialogStarted;
  private bool clicked;

  public GameObject fadeToBlackEffect;

	// Use this for initialization
	void Start () {

		completedSpeech = false;
    startedSpeech = false;

    storyImage = GameObject.Find("StoryImage").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
    ClearScript();
		if (Input.GetMouseButtonDown(0)) StartStory(); // continue dialog if we click anywhere
	}

  IEnumerator AutoNextScript() {
    yield return new WaitForSeconds(2f);
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

		if (!startedSpeech && !completedSpeech) {
			storyText.TypeText(text[scriptIndex]);
      NextImage();
			completedSpeech = false;
      startedSpeech = true;
      if (scriptIndex < text.Length) scriptIndex++;
    } else if (startedSpeech && !completedSpeech)
      storyText.Skip();
	}

  private void NextImage() {
    storyImage.sprite = sprites[imageIndex];
    if (imageIndex < sprites.Length) imageIndex++;
  }
}
