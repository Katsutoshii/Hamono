using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashWithMusic : MonoBehaviour {
	public AudioSource audioSource;
	public float updateStep = 0.05f;
	public int sampleDataLength = 256;

	private float currentUpdateTime = 0f;

	private float clipLoudness;
	private float[] clipSampleData;

	// Use this for initialization
	void Awake () {

		if (!audioSource) {
			Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
		}
		clipSampleData = new float[sampleDataLength];

	}

	public static float val = 0;
	private float targetVal = 0;
	// Update is called once per frame
	void Update () {
	
		currentUpdateTime += Time.deltaTime;
		if (currentUpdateTime >= updateStep) {
			currentUpdateTime = 0f;
			audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
			clipLoudness = 0f;
			foreach (var sample in clipSampleData) {
				clipLoudness += Mathf.Abs(sample);
			}

			//clipLoudness += prevLoudness;

			clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
			targetVal = clipLoudness / 2;
		}

		val = 0.015f * (targetVal - val) + val;
	}
}
