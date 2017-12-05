using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Afterimage : PooledObject {

    private SpriteRenderer spriteRenderer;
    public Color startColor;
    public float alphaIncrement;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = startColor;
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {
        Color c = spriteRenderer.color;
        c.a -= alphaIncrement;
        spriteRenderer.color = c;
    }

    public override void OnObjectReuse() {
        transform.localScale = 
    }
}