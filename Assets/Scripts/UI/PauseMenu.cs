using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

  private Player player;
  private Slider musicSlider;
  private Slider sfxSlider;
  private AudioSource musicAudio;
  private AudioSource[] sfxAudio;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {

    player = GameObject.Find("Player").GetComponent<Player>();
    musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
    sfxSlider = GameObject.Find("SFXSlider").GetComponent<Slider>();
    musicAudio = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
    musicAudio.ignoreListenerVolume = true;
    
    // represents the current volume of the game
    musicSlider.value = musicAudio.volume;
    sfxSlider.value = AudioListener.volume;

    Time.timeScale = 0.0f;
  }

  public void ChangeSFXVolume() {
    AudioListener.volume = sfxSlider.value;
  }

  // event handler for the resume button
  public void Resume() {
    StartCoroutine(player.AfterEventWait());
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
