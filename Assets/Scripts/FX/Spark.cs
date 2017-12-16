using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Spark : PooledObject {
    private new Light light;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Update() {
        // fade the cloud
        Color color = spriteRenderer.color;
        color.a -= 0.025f;
        spriteRenderer.color = color;
        light.intensity -= 0.4f;
        if (light.intensity <= 0) {
            rb.gravityScale = 0;
            gameObject.SetActive(false);
        }
    }

    public override void OnObjectReuse() {
        Debug.Log("On object reuse!");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;

        light = GetComponentInChildren<Light>();
        light.intensity = 12;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
		rb.velocity = new Vector2(Random.Range(-5f, 5f), Random.Range(0f, 5f));
    }
}
