using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable CS0219  

/// Base class for text scripts
public class TextScript : MonoBehaviour {

  protected Player player;
  protected TextTyper textOutput;

  [TextArea(3, 10)]
  public string text;

  public bool completedSpeech;
	protected bool dialogStarted;

  // Use this for initialization
	void Start () {
		
		player = GameObject.Find("Player").GetComponent<Player>();
		completedSpeech = false;

	}

  // handles the text at the end of the interaction
  protected virtual void ResetTextOutput() {}

  // sets up extra prefabs and/or game objects
  protected virtual void SetUpText() {}

  // moves to next line of text
  protected virtual void StartText() {
    dialogStarted = true;
		Debug.Log("starting dialogue");

    SetUpText();

		if (completedSpeech) {
			// ending conversation
			Debug.Log("ending dialogue");
			dialogStarted = false;
			completedSpeech = false;
      ResetTextOutput();
			player.state = Player.State.finishedTalking;
		} else if (!completedSpeech && player.state != Player.State.talking) {
			// starting converstation
			player.ResetToIdle();
			player.state = Player.State.talking;
			textOutput.TypeText(text);
			completedSpeech = false;
		} else if (!completedSpeech && player.state == Player.State.talking) {
			textOutput.Skip();
		}
  }
}
