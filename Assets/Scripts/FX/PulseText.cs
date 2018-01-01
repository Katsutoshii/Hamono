using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseText : MonoBehaviour {
	private RectTransform rectTransform;
	public float amplitude;
    public float frequency;
    public float offset;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
		float scaleVal = amplitude * Mathf.Sin(frequency * Time.time) + offset;
        rectTransform.localScale = new Vector3(scaleVal, scaleVal, 1);
    }
}
