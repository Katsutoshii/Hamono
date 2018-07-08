using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RedBlueGames.Tools.TextTyper;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    StartCoroutine("EndGame");
  }

  private IEnumerator EndGame() {
    yield return new WaitForSeconds(45f); // wait for the animation to be done
    SceneManager.LoadScene(0);
  }
}

