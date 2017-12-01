using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Player player;
	public bool following = false;
    public float SCROLL_SPEED;

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
        
        following = !player.slashIndicator.drawing;
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if (following) transform.position += SCROLL_SPEED * ((player.transform.position + offset) - transform.position);
    }
}