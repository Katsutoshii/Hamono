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
    musicSlider.value = GetMusicVolume();
    sfxSlider.value = GetSFXVolume();

    Time.timeScale = 0.0f;
  }

  private float GetMusicVolume() {
    if (PlayerPrefs.GetInt("set_music_volume") == 0) {
      // music volume has never been set
      PlayerPrefs.SetInt("set_music_volume", 1);
      PlayerPrefs.SetFloat("music_volume", musicAudio.volume);
      return musicAudio.volume;
    } else {
      // music volume has been set
      return PlayerPrefs.GetFloat("music_volume");
    }
  }

  private float GetSFXVolume() {
    if (PlayerPrefs.GetInt("set_sfx_volume") == 0) {
      // sfx volume has never been set
      PlayerPrefs.SetInt("set_sfx_volume", 1);
      PlayerPrefs.SetFloat("sfx_volume", AudioListener.volume);
      return AudioListener.volume;
    } else {
      // sfx volume has been set
      return PlayerPrefs.GetFloat("sfx_volume");
    }
  }

  public void ChangeSFXVolume() {
    AudioListener.volume = sfxSlider.value;
    PlayerPrefs.SetInt("set_sfx_volume", 1);
    PlayerPrefs.SetFloat("sfx_volume", sfxSlider.value);
  }

  public void ChangeMusicVolume() {
    if (musicAudio != null) {
      musicAudio.volume = musicSlider.value;
      PlayerPrefs.SetInt("set_sfx_volume", 1);
      PlayerPrefs.SetFloat("sfx_volume", sfxSlider.value);
    }
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
