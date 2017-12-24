using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {
    private new Light light;
    public float amplitude;
    public float frequency;
    public float offset;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        light = GetComponent<Light>();
        frequency += Random.Range(0, 1f);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        light.intensity = amplitude * Mathf.Sin(frequency * Time.time) + offset;
    }
}
