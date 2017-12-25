using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;

public class Story : MonoBehaviour {

	public GameObject speechText;
	 [TextArea(3,10)]
 	public string text;
	
	public HashSet<GameObject> allSpeech;
	
	public bool completedSpeech;
	private bool dialogStarted;

	// Use this for initialization
	void Start () {

		completedSpeech = false;
		allSpeech = new HashSet<GameObject>();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) StartDialogue(); // continue dialog if we click anywhere
	}

	private void StartDialogue() {
		dialogStarted = true;
		Debug.Log("starting dialogue");
		// triggers a speech bubble
		TextTyper NPCTextChild;

		NPCTextChild = GameObject.Find("StoryText").GetComponent<TextTyper>();
		Debug.Log("completed speech?: " + completedSpeech);

		if (completedSpeech) {
			// ending conversation
			foreach (GameObject item in allSpeech)
				Destroy(item);
			dialogStarted = false;
			completedSpeech = false;
		} else {
			// starting converstation
			// allSpeech.Add(npcText);
			NPCTextChild.TypeText(text);
			completedSpeech = false;
    }
		// } else if (!completedSpeech && player.state == Player.State.talking) {
		// 	NPCTextChild.Skip();
		// }
	}
}
