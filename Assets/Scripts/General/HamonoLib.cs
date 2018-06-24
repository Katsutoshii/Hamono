using UnityEngine;

public class HamonoLib {
    
    public static Vector3 RandomOffset(Vector3 position, float lower = 0f, float upper = 0.5f) {
        return new Vector3(position.x + Random.Range(lower, upper),
        position.y + Random.Range(lower, upper),
        position.z);
    }
}
