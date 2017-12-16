using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour {
	private GameObject coinPrefab;
	private GameObject potPiecePrefab;
	private GameObject heartPrefab;

	// Use this for initialization
	void Start () {		
    	coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
    	heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
		potPiecePrefab = Resources.Load<GameObject>("Prefabs/Environment/PotPiece");

		PoolManager.instance.CreatePool(coinPrefab, 20);
		PoolManager.instance.CreatePool(potPiecePrefab, 20);
		PoolManager.instance.CreatePool(heartPrefab, 3);
	}
}
