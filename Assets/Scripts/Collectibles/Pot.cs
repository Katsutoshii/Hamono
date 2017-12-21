using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pot : MonoBehaviour {

	private GameObject coinPrefab;
	private GameObject heartPrefab;
	private GameObject potPiecePrefab;
	private GameObject sparkPrefab;
	private AudioSource audioSource;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider2D;
	public int numCoins;
	public int numHearts;
	public int numPieces;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		
    	coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
    	heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
		potPiecePrefab = Resources.Load<GameObject>("Prefabs/Environment/PotPiece");
		sparkPrefab = Resources.Load<GameObject>("Prefabs/FX/Spark");
	}

	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other) {
		switch (other.name) {
			case "PlayerSlashHurtBox":
				Break();
				break;

			case "PlayerDashHurtBox":
				Break();
				break;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {  
		switch (LayerMask.LayerToName(other.gameObject.layer)) {
			case "Spikes":
				Break();
				break;
		}
	}
	
	private void Break() {
		audioSource.Play();
		for (int i = 0; i < Random.Range(0, numCoins + 1); i++) 
			PoolManager.instance.ReuseObject(coinPrefab, 
				HamonoLib.RandomOffset(transform.position), coinPrefab.transform.rotation, coinPrefab.transform.localScale);

		for (int i = 0; i < Random.Range(0, numHearts + 1); i++) 
			PoolManager.instance.ReuseObject(heartPrefab, 
				HamonoLib.RandomOffset(transform.position), heartPrefab.transform.rotation, heartPrefab.transform.localScale);

		for (int i = 0; i < numPieces; i++)
			PoolManager.instance.ReuseObject(potPiecePrefab, 
				HamonoLib.RandomOffset(transform.position), HamonoLib.RandomOffset(transform.localEulerAngles), potPiecePrefab.transform.localScale);

			PoolManager.instance.ReuseObject(sparkPrefab, 
				HamonoLib.RandomOffset(transform.position), HamonoLib.RandomOffset(transform.localEulerAngles), sparkPrefab.transform.localScale);

		spriteRenderer.color = new Color (0, 0, 0, 0);
		boxCollider2D.enabled = false;

		Destroy(gameObject, 1f);
	}
}
