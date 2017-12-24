using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

  private Slider musicSlider;
  private Slider sfxSlider;
  private AudioSource musicAudio;
  private AudioSource[] sfxAudio;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {

    musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
    sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
    musicAudio = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
    sfxAudio = (AudioSource[]) GameObject.FindObjectsOfType(typeof(AudioSource));
    
    // represents the current volume of the game
    musicSlider.value = musicAudio.volume;
    sfxSlider.value = sfxAudio[0].volume;

    Time.timeScale = 0.0f;
  }

  public void ChangeSFXVolume() {
    foreach (AudioSource source in sfxAudio) {
      if (source.name != "MusicPlayer") {
        source.volume = sfxSlider.value;
      }
    }
  }

  // event handler for the resume button
  public void Resume() {
    Time.timeScale = 1.0f;
    gameObject.SetActive(false);
    GameManager.paused = false;
  }

  // event handler for the quit button
  public void Quit() {
    // goes back to the title screen
    SceneManager.LoadSceneAsync(0);
    Resume();    
  }
}
