using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Dustcloud : PooledObject {
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Init();
    }

    void Update() {
        // fade the cloud
        Color color = spriteRenderer.color;
        color.a -= 0.05f;
        spriteRenderer.color = color;
        if (color.a <= 0) gameObject.SetActive(false);
    }

    private void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        animator = GetComponent<Animator>();
        animator.Play("Dustcloud");
    }

    public override void OnObjectReuse() {
        Init();
    }
}
