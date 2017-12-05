using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  
  public float maxSpeed;

  public enum State {
    idle,
    walking,
    attacking,
    damaged,
    dead,
  }

  void Start() {

  }

  void Update() {
    
  }
}
