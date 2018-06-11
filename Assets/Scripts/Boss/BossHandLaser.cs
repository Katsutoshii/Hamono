using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHandLaser : MonoBehaviour
{

    /// <summary>
    /// Activates the game object to start shooting
    /// </summary>
    public void Shoot() 
    {
        gameObject.SetActive(true);
    }

    public void StopShooting()
    {
        gameObject.SetActive(false);
    }
}