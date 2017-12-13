using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tripwire : MonoBehaviour {	
	private Animator animator;
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		animator = GetComponent<Animator>();
		animator.SetBool("tripped", false);
	}

	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if (transform.childCount >0) {
			Debug.Log("Tripped!");
			animator.SetBool("tripped", true);
			Destroy(transform.GetChild(0).gameObject);
		}
	}
}
