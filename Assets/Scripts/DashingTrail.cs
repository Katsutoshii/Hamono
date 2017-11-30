using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingTrail : MonoBehaviour {

	private Player player;

    // Use this for initialization
    void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }
	// Update is called once per frame
	void Update () {
		

		if (player.state == Player.State.dashing) {
			if (player.rb.velocity.x < 0) transform.localScale = new Vector3(-1, 1, 1);
			else transform.localScale = new Vector3(1, 1, 1);
			SpawnTrail();
		}
		else {
			transform.localScale = new Vector3(0, 0, 0);
		}
	}

	void SpawnTrail()
    {
        GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        trailPart.transform.position = transform.position;
		trailPartRenderer.color = Color.white;
        Destroy(trailPart, 0.1f); // replace 0.5f with needed lifeTime
 
        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }
 
    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color color = trailPartRenderer.color;
        color.a -= 0.1f; // replace 0.5f with needed alpha decrement
        trailPartRenderer.color = color;
 
        yield return new WaitForEndOfFrame();
    }
}
