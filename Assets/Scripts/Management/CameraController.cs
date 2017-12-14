using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Player player;
    public float maxX;
    public float minX;
    public float maxY;
    public float minY;

	public bool following = false;
    public float SCROLL_SPEED;
    public float smoothTime;

	public float shakeDuration;
	public float shakeAmount;
	public float decreaseFactor;
    private Vector3 originalPosition;

    private Vector3 offset;         // offset between the player and the camera
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () 
    {   
        // calc the player's x y, keep the z offset
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        // Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

        // original position of the camera
        originalPosition = transform.position;
    }
    
    // LateUpdate is called after Update each frame
    void LateUpdate () 
    {
        
        following = !player.slashIndicator.drawing && player.attackResponse != Player.AttackResponse.normal;
        if (following) {
            transform.position = Vector3.SmoothDamp(transform.position, 
                player.transform.position + offset, ref velocity, smoothTime);
            
            // bound the position
            transform.position = new Vector3(
                Bound(transform.position.x, minX, maxX),
                Bound(transform.position.y, minY, maxY), 
                transform.position.z);
        }
        
        if (player.state == Player.State.damaged) shakeDuration = .5f;
        else shakeDuration = 0f;
        ShakeCamera();

    }

    private void ShakeCamera() {
        if (shakeDuration > 0) {
            transform.position = originalPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        } else {
            shakeDuration = 0f;
            originalPosition = transform.position;
        }
    }

    private float Bound(float val, float min, float max) {
        return Mathf.Max(Mathf.Min(val, max), min);
    }
}
