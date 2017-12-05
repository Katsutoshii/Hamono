using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

	Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();

	// singleton for static access to this functionality
	static PoolManager _instance;
	public static PoolManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<PoolManager>();
			}
			return _instance;
		}
	}

	// method to create a pool of game objects
	public void CreatePool(GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID();

		if (!pools.ContainsKey(poolKey)) {
			pools.Add(poolKey, new Queue<GameObject>());

			for (int i = 0; i < poolSize; i++) {
				GameObject newObject = Instantiate (prefab) as GameObject;
				newObject.SetActive(false);
				pools[poolKey].Enqueue(newObject);
			}
		}
	}

	/// method to reuse an object form the pool
	public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation) {
		int poolKey = prefab.GetInstanceID();

		if (pools.ContainsKey(poolKey)) {
			GameObject objectToReuse = pools[poolKey].Dequeue();
			pools[poolKey].Enqueue(objectToReuse);

			objectToReuse.SetActive(true);
			objectToReuse.transform.position = position;
			objectToReuse.transform.rotation = rotation;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
