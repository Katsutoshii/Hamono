using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

  private Button resumeButton;
  private Button quitButton;

  private Player player;
  private bool paused;

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() {
    GameManager.paused = true;
    resumeButton = GameObject.Find("Resume").GetComponent<Button>();
    resumeButton.onClick.AddListener(Resume);
    quitButton = GameObject.Find("Quit").GetComponent<Button>();
    quitButton.onClick.AddListener(Quit);

    player = GameObject.Find("Player").GetComponent<Player>();

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
