using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;

public class Story : MonoBehaviour {


  private TextTyper storyText;
	 [TextArea(3,10)]
 	public string text;
	
	public bool completedSpeech;
  private bool startedSpeech;
	private bool dialogStarted;

	// Use this for initialization
	void Start () {

		completedSpeech = false;
    startedSpeech = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) StartStory(); // continue dialog if we click anywhere
	}

	private void StartStory() {
		dialogStarted = true;
		Debug.Log("starting story");
		// triggers a speech bubble

		storyText = GameObject.Find("StoryText").GetComponent<TextTyper>();
		Debug.Log("completed speech?: " + completedSpeech);

		if (completedSpeech) {
			// ending conversation
			dialogStarted = false;
			completedSpeech = false;
      startedSpeech = false;
		} else if (!startedSpeech && !completedSpeech) {
			storyText.TypeText(text);
			completedSpeech = false;
      startedSpeech = true;
    } else if (startedSpeech && !completedSpeech)
      storyText.Skip();
	}
}
