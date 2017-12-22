using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {
    private Image image;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
        image.color = new Color(0, 0, 0, 0);

    }
    public void RunAnimation() {
        image.color = Color.white;
    }
}
