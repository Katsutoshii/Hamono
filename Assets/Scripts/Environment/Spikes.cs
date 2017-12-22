using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spikes : MonoBehaviour {

  private GameObject electricity;
  
  void Start() {

    electricity = Resources.Load<GameObject>("Prefabs/Environment/ElectricitySparks");
    Tilemap tilemap = GetComponent<Tilemap>();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    Instantiate(electricity, new Vector2(x - 22.5f, y - 3f), Quaternion.identity);
                }
            }
        }
  }
}
