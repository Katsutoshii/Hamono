using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject player;
	public bool following = false;

    private Vector3 offset;         // offset between the player and the camera

    // Use this for initialization
    void Start () 
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }
    
    // LateUpdate is called after Update each frame
    void LateUpdate () 
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if(following) transform.position = player.transform.position + offset;
    }
}