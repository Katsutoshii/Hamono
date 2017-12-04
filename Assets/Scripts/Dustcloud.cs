using UnityEngine;
using UnityEngine.UI;
using RedBlueGames.Tools.TextTyper;

public class Dustcloud : MonoBehaviour {
    public Animator anim;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
    }

public void MakeCloud(Vector3 pos) {
        transform.position = pos;
        anim.Play("Dustcloud");
    }
}
