using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricitySpark : MonoBehaviour {

  void Awake() {
    Light lightSource = gameObject.AddComponent<Light>() as Light;
    Debug.Log("ijemma: " + gameObject);
    lightSource.type = LightType.Point;
    lightSource.range = 10f; 
    lightSource.color = new Color(0, .31f, .83f, 1f);
    lightSource.intensity = 1f;
  }
}
