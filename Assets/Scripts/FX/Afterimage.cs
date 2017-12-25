using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Afterimage : PooledObject {

    private SpriteRenderer spriteRenderer;
    public Color startColor;
    public float alphaDecrement;
    public Vector3 rotation;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {
        Color c = spriteRenderer.color;
        c.a -= alphaDecrement;
        spriteRenderer.color = c;
        if (c.a <= 0) gameObject.SetActive(false);
    }

    public override void OnObjectReuse() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = startColor;
    }
}
