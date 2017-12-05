using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

	Dictionary<int, Queue<ObjectInstance>> pools = new Dictionary<int, Queue<ObjectInstance>>();

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
			pools.Add(poolKey, new Queue<ObjectInstance>());

			for (int i = 0; i < poolSize; i++) {
				ObjectInstance newObject = new ObjectInstance(Instantiate (prefab) as GameObject);
				pools[poolKey].Enqueue(newObject);
			}
		}
	}

	/// method to reuse an object form the pool
	public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation) {
		int poolKey = prefab.GetInstanceID();

		if (pools.ContainsKey(poolKey)) {
			ObjectInstance objectToReuse = pools[poolKey].Dequeue();
			pools[poolKey].Enqueue(objectToReuse);

			objectToReuse.Reuse(position, rotation);
		}
	}

	public class ObjectInstance {
		GameObject gameObject;
		Transform transform;
		bool hasPoolObjectComponent;
		PooledObject poolObjectScript;

		public ObjectInstance(GameObject objectInstance) {
			gameObject = objectInstance;
			transform = gameObject.transform;
			gameObject.SetActive(false);

			if (gameObject.GetComponent<PooledObject>()) {
				hasPoolObjectComponent = true;
				poolObjectScript = gameObject.GetComponent<PooledObject>();
			}
		}

		public void Reuse(Vector3 position, Quaternion rotation) {
			if (hasPoolObjectComponent) {
				poolObjectScript.OnObjectReuse();
			}

			gameObject.SetActive(true);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
	}
}
