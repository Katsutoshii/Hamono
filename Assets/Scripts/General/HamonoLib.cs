using UnityEngine;

public class HamonoLib {
    
    public static Vector3 RandomOffset(Vector3 position) {
        return new Vector3(position.x + Random.Range(0f, 0.5f),
        position.y + Random.Range(0f, 0.5f),
        position.z);
    }
}