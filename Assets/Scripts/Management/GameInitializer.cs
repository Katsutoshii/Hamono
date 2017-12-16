using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour {
	public float timeScale;
    
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

	// Use this for initialization
	void Start () {	
        cursorTexture = (Texture2D) Resources.Load("Graphics/UI/ui_cursor");
        cursorMode = CursorMode.Auto;
        hotSpot = Vector2.zero;
        
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        Time.timeScale = timeScale;	
	}
}
