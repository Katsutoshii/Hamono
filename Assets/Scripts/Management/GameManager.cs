using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public float timeScale;
	public static bool paused;
    
	public Texture2D cursorTexture;
	public CursorMode cursorMode;
	public Vector2 hotSpot;

	// for pooling
	private GameObject coinPrefab;
	private GameObject potPiecePrefab;
	private GameObject heartPrefab;
	private GameObject sparkPrefab;
	private GameObject samuraiLaserPrefab;
	
	private GameObject pauseMenuPrefab;

	
	private AudioSource musicPlayer;
	private Player player;

	// Use this for initialization
	void Start () {
		cursorTexture = (Texture2D) Resources.Load("Graphics/UI/ui_cursor");
		cursorMode = CursorMode.Auto;
		hotSpot = Vector2.zero;
        
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
		Time.timeScale = timeScale;	

		player = FindObjectOfType<Player>();

		coinPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Coin");
		heartPrefab = Resources.Load<GameObject>("Prefabs/Collectibles/Heart");
		potPiecePrefab = Resources.Load<GameObject>("Prefabs/Environment/PotPiece");
		sparkPrefab = Resources.Load<GameObject>("Prefabs/FX/Spark");
		samuraiLaserPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/SamuraiLaser");
		musicPlayer = GetComponentInChildren<AudioSource>();
		

		PoolManager.instance.CreatePool(coinPrefab, 20);
		PoolManager.instance.CreatePool(potPiecePrefab, 20);
		PoolManager.instance.CreatePool(heartPrefab, 3);
		PoolManager.instance.CreatePool(sparkPrefab, 12);
		PoolManager.instance.CreatePool(samuraiLaserPrefab, 4);

		Screen.SetResolution(960, 540, true);
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		// stop playing music if player dies
		if (player != null && player.state == Player.State.dead) musicPlayer.Stop();
	}
}
