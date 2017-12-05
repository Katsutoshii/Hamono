using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingTrailSpawner : MonoBehaviour {

	private Player player;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }
	// Update is called once per frame
	void Update () {
		if (player.state == Player.State.dashing) {
			transform.localScale = new Vector3(1, 1, 1);
			SpawnTrail();
		}
		else {
			transform.localScale = new Vector3(0, 0, 0);
		}
	}

	void SpawnTrail()
    {
    }
}
