using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

  private Button resumeButton;
  private Button quitButton;

  private Player player;

  void Start() {
    resumeButton = GameObject.Find("Resume").GetComponent<Button>();
    resumeButton.onClick.AddListener(Resume);
    quitButton = GameObject.Find("Quit").GetComponent<Button>();
    quitButton.onClick.AddListener(Quit);

    player = GameObject.Find("Player").GetComponent<Player>();
  }

  void Update() {
    
  }

  public void Resume() {
    Destroy(gameObject);
    player.paused = false;
  }

  public void Quit() {
    Destroy(gameObject);
    // SceneManager.LoadScene(0);
    player.paused = false;
  }

}
