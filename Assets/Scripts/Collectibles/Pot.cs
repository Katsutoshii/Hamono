using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour {

	public GameObject coinPrefab;
	public GameObject potPiecePrefab;
	private AudioSource audioSource;
	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider2D;
	public int numCoins;
	public int numPieces;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider2D = GetComponent<BoxCollider2D>();
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

	private void Break() {
		audioSource.Play();
		for (int i = 0; i < numCoins; i++) 
			PoolManager.instance.ReuseObject(coinPrefab, RandomOffset(transform.position), coinPrefab.transform.rotation, coinPrefab.transform.localScale);

		for (int i = 0; i < numPieces; i++)
			PoolManager.instance.ReuseObject(potPiecePrefab, RandomOffset(transform.position), RandomOffset(transform.localEulerAngles), potPiecePrefab.transform.localScale);

		spriteRenderer.color = new Color (0, 0, 0, 0);
		boxCollider2D.enabled = false;

		Destroy(gameObject, 1f);
	}

	private Vector3 RandomOffset(Vector3 position) {
    return new Vector3(position.x + Random.Range(0f, 0.5f),
      position.y + Random.Range(0f, 0.5f),
      position.z);
  	}
}
