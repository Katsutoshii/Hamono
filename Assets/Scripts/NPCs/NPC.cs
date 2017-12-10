using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;

public class NPC : MonoBehaviour {

	private Player player;
	
	public GameObject npcText;
	public GameObject speechText;
	 [TextArea(3,10)]
 	public string text;
	
	public HashSet<GameObject> allSpeech;
	
	public bool completedSpeech;

	// Use this for initialization
	void Start () {
		
		player = GameObject.Find("Player").GetComponent<Player>();

		completedSpeech = false;
		allSpeech = new HashSet<GameObject>();
		
		npcText = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Called every frame while the mouse is over the GUIElement or Collider.
	/// </summary>
	void OnMouseOver()
	{
		if(Input.GetMouseButtonDown(1)) {
			StartDialogue();
		}
	}

	private void StartDialogue() {
		Debug.Log("starting dialogue");
		// triggers a speech bubble
		TextTyper NPCTextChild;

		if (npcText == null) {
			npcText = Instantiate(speechText);
			npcText.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, 0);
		}
		NPCTextChild = npcText.transform.GetChild(0).gameObject.GetComponent<TextTyper>();
		NPCTextChild.NPC = gameObject.GetComponent<NPC>();
		Debug.Log("completed speech?: " + completedSpeech);
		if (completedSpeech) {
			// ending conversation
			foreach (GameObject item in allSpeech)
				Destroy(item);
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
