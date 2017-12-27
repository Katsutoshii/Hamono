using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;

public class NPC : MonoBehaviour {

	private Player player;
	
	private GameObject NPCMessage;
	 [TextArea(3,10)]
 	public string text;
	
	public bool completedSpeech;
	private bool dialogStarted;

	public Texture2D cursorTexture;
	public Texture2D speechBubble;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

	void Awake() {
		NPCMessage = transform.GetChild(0).gameObject;
		NPCMessage.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		
		player = GameObject.Find("Player").GetComponent<Player>();
		completedSpeech = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (dialogStarted && Input.GetMouseButtonDown(0)) StartDialogue(); // continue dialog if we click anywhere
	}

	/// <summary>
	/// Called every frame while the mouse is over the GUIElement or Collider.
	/// </summary>
	void OnMouseOver()
	{
		Cursor.SetCursor(speechBubble, hotSpot, cursorMode);
		if(Input.GetMouseButtonDown(1)) {
			StartDialogue();
		}
	}

	/// <summary>
	/// Called when the mouse is not any longer over the GUIElement or Collider.
	/// </summary>
	void OnMouseExit()
	{
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
	}

	private void StartDialogue() {
		dialogStarted = true;
		Debug.Log("starting dialogue");
		// triggers a speech bubble
		TextTyper NPCMessageText;

		// shows on screen
		if (!NPCMessage.active) NPCMessage.SetActive(true);

		// gets TextTyper object
		NPCMessageText = NPCMessage.transform.GetChild(1).GetComponent<TextTyper>();
		NPCMessage.transform.GetChild(2).GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
		NPCMessageText.NPC = gameObject.GetComponent<NPC>();
		Debug.Log("completed speech?: " + completedSpeech);

		if (completedSpeech) {
			// ending conversation
			dialogStarted = false;
			completedSpeech = false;
			NPCMessage.SetActive(false);
			player.state = Player.State.finishedTalking;
		} else if (!completedSpeech && player.state != Player.State.talking) {
			// starting converstation
			player.ResetToIdle();
			player.state = Player.State.talking;
			NPCMessageText.TypeText(text);
			completedSpeech = false;
		} else if (!completedSpeech && player.state == Player.State.talking) {
			NPCMessageText.Skip();
		}
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
