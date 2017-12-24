using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

  private Slider musicSlider;
  private AudioSource musicAudio;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {

    musicSlider = GameObject.Find("MusicSlider").GetComponent<Slider>();
    musicAudio = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
    
    // represents the current volume of the game
    musicSlider.value = musicAudio.volume;

    Time.timeScale = 0.0f;
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
