using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFist : MonoBehaviour {

	private Boss boss;
	// Use this for initialization
	void Start () {
		boss = GetComponentInParent<Boss>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
