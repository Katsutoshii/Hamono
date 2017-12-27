using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;

public class NPC : TextScript {
	
	private GameObject NPCMessage;

	public Texture2D cursorTexture;
	public Texture2D speechBubble;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

	void Awake() {
		NPCMessage = transform.GetChild(0).gameObject;
		NPCMessage.SetActive(false);
		textOutput = NPCMessage.transform.GetChild(1).GetComponent<TextTyper>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogStarted && Input.GetMouseButtonDown(0)) StartText(); // continue dialog if we click anywhere
	}

	/// <summary>
	/// Called every frame while the mouse is over the GUIElement or Collider.
	/// </summary>
	void OnMouseOver() {
		Cursor.SetCursor(speechBubble, hotSpot, cursorMode);
		if(Input.GetMouseButtonDown(1)) {
			StartText();
		}
	}

	/// <summary>
	/// Called when the mouse is not any longer over the GUIElement or Collider.
	/// </summary>
	void OnMouseExit() {
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

	// cleans up screen after dialogue
	protected override void ResetTextOutput() {
		NPCMessage.SetActive(false);
	}

	// sets up the text
	protected override void SetUpText() {
		TextTyper NPCMessageText;

		// shows on screen
		if (!NPCMessage.active) NPCMessage.SetActive(true);

		// gets TextTyper object
		NPCMessageText = NPCMessage.transform.GetChild(1).GetComponent<TextTyper>();
		NPCMessage.transform.GetChild(2).GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
		NPCMessageText.NPC = gameObject.GetComponent<NPC>();
		Debug.Log("completed speech?: " + completedSpeech);
	}

	// Grabs the nearest NPC able to chat
	// distance defines the area space that picks up NPCs
	private GameObject NearestNPC(float distance = 2.3f) {
		GameObject[] NPCList = GameObject.FindGameObjectsWithTag("NPC");
		GameObject nearestNPC = null;

		foreach (GameObject NPC in NPCList) {
			if (Vector2.Distance(transform.position, NPC.transform.position) <= distance)
				nearestNPC = NPC;
		}
		return nearestNPC;
	}
}
