using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Player player;
	public bool following = false;
    public float SCROLL_SPEED;
    public float smoothTime;

    private Vector3 offset;         // offset between the player and the camera
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () 
    {   
        // calc the player's x y, keep the z offset
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        // Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }
    
    // LateUpdate is called after Update each frame
    void LateUpdate () 
    {
        
        following = !player.slashIndicator.drawing;
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        if (following) transform.position = Vector3.SmoothDamp(transform.position, 
            player.transform.position + offset, ref velocity, smoothTime);
    }
}