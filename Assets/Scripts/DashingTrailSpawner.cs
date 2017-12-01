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
        GameObject trailPart = new GameObject();
        SpriteRenderer trailPartRenderer = trailPart.AddComponent<SpriteRenderer>();
        trailPartRenderer.sprite = GetComponent<SpriteRenderer>().sprite;
        trailPart.transform.position = transform.position;
        trailPart.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(player.targetB.y - player.targetA.y, 
				player.targetB.x - player.targetA.x) * 180 / Mathf.PI);
        Destroy(trailPart, 0.2f); // replace 0.5f with needed lifeTime
 
        StartCoroutine("FadeTrailPart", trailPartRenderer);
    }
 
    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color color = trailPartRenderer.color;
        for (float f = 1f; f >= 0; f -= 0.1f) {
            Color c = trailPartRenderer.color;
            c.a = f;
            trailPartRenderer.color = c;
            yield return new WaitForEndOfFrame();
        }
    }
}
