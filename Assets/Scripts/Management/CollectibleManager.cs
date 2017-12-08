using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour {
	public GameObject coinPrefab;
	public GameObject potPiecePrefab;

	// Use this for initialization
	void Start () {
		PoolManager.instance.CreatePool(coinPrefab, 20);
		PoolManager.instance.CreatePool(potPiecePrefab, 20);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
