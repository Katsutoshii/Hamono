using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;

public class NPC : MonoBehaviour {

	private Player player;
	
	private GameObject npcText;
	public GameObject speechText;
	 [TextArea(3,10)]
 	public string text;
	
	public HashSet<GameObject> allSpeech;
	
	public bool completedSpeech;
	private bool dialogStarted;

	public Texture2D cursorTexture;
	public Texture2D speechBubble;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

	// Use this for initialization
	void Start () {
		
		player = GameObject.Find("Player").GetComponent<Player>();

		completedSpeech = false;
		allSpeech = new HashSet<GameObject>();
		
		npcText = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogStarted && Input.GetMouseButtonDown(0)) StartDialogue(); // continue dialog if we click anwhere
	}

	/// <summary>
	/// Called every frame while the mouse is over the GUIElement or Collider.
	/// </summary>
	void OnMouseOver()
	{
		Cursor.SetCursor(speechBubble, hotSpot, cursorMode);
		if(Input.GetMouseButtonDown(0)) {
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
		TextTyper NPCTextChild;

		if (npcText == null) {
			npcText = Instantiate(speechText);
			npcText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, 0);
		}
		NPCTextChild = npcText.transform.GetChild(1).gameObject.GetComponent<TextTyper>();
		NPCTextChild.NPC = gameObject.GetComponent<NPC>();
		Debug.Log("completed speech?: " + completedSpeech);
		if (completedSpeech) {
			// ending conversation
			foreach (GameObject item in allSpeech)
				Destroy(item);
			dialogStarted = false;
			completedSpeech = false;
			npcText = null;
			player.state = Player.State.idle;
		} else if (!completedSpeech && player.state != Player.State.talking) {
			// starting converstation
			player.state = Player.State.talking;
			allSpeech.Add(npcText);
			NPCTextChild.TypeText(text);
			completedSpeech = false;
		} else if (!completedSpeech && player.state == Player.State.talking) {
			NPCTextChild.Skip();
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
